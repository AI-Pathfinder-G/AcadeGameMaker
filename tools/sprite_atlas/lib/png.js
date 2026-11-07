'use strict';

// Minimal deterministic PNG codec: 8-bit RGB/RGBA, non-interlaced input only.
const zlib = require('node:zlib');
const crc32 = require('node:zlib').crc32;
const SIGNATURE = Buffer.from([137,80,78,71,13,10,26,10]);
function paeth(a,b,c) { const p=a+b-c, pa=Math.abs(p-a), pb=Math.abs(p-b), pc=Math.abs(p-c); return pa<=pb&&pa<=pc?a:pb<=pc?b:c; }
function decode(buffer) {
  if (!buffer.subarray(0, 8).equals(SIGNATURE)) throw new Error('not a PNG');
  let offset=8, width, height, colorType, bits, interlace, idat=[];
  while (offset < buffer.length) { const length=buffer.readUInt32BE(offset); const type=buffer.toString('ascii',offset+4,offset+8); const data=buffer.subarray(offset+8,offset+8+length); offset += 12+length;
    if (type==='IHDR') { width=data.readUInt32BE(0); height=data.readUInt32BE(4); bits=data[8]; colorType=data[9]; interlace=data[12]; }
    else if (type==='IDAT') idat.push(data); else if (type==='IEND') break;
  }
  if (bits!==8 || interlace!==0 || ![2,6].includes(colorType)) throw new Error('only 8-bit non-interlaced RGB/RGBA PNG is supported');
  const channels=colorType===6?4:3, stride=width*channels, packed=zlib.inflateSync(Buffer.concat(idat)); let pos=0, previous=Buffer.alloc(stride), pixels=Buffer.alloc(width*height*4);
  for(let y=0;y<height;y++){ const filter=packed[pos++], row=Buffer.from(packed.subarray(pos,pos+stride)); pos+=stride;
    for(let x=0;x<stride;x++){ const left=x>=channels?row[x-channels]:0, up=previous[x], ul=x>=channels?previous[x-channels]:0; if(filter===1)row[x]=(row[x]+left)&255; else if(filter===2)row[x]=(row[x]+up)&255; else if(filter===3)row[x]=(row[x]+Math.floor((left+up)/2))&255; else if(filter===4)row[x]=(row[x]+paeth(left,up,ul))&255; else if(filter!==0)throw new Error(`unsupported PNG filter ${filter}`); }
    for(let x=0;x<width;x++){ const from=x*channels, to=(y*width+x)*4; pixels[to]=row[from];pixels[to+1]=row[from+1];pixels[to+2]=row[from+2];pixels[to+3]=channels===4?row[from+3]:255; } previous=row;
  } return {width,height,pixels};
}
function chunk(type,data){ const name=Buffer.from(type); const out=Buffer.alloc(12+data.length); out.writeUInt32BE(data.length,0);name.copy(out,4);data.copy(out,8);out.writeUInt32BE(crc32(Buffer.concat([name,data]))>>>0,8+data.length);return out; }
function encode({width,height,pixels}) { const scan=Buffer.alloc(height*(1+width*4)); for(let y=0;y<height;y++){scan[y*(1+width*4)]=0; pixels.copy(scan,y*(1+width*4)+1,y*width*4,(y+1)*width*4);} const ihdr=Buffer.alloc(13);ihdr.writeUInt32BE(width,0);ihdr.writeUInt32BE(height,4);ihdr[8]=8;ihdr[9]=6; return Buffer.concat([SIGNATURE,chunk('IHDR',ihdr),chunk('IDAT',zlib.deflateSync(scan,{level:9})),chunk('IEND',Buffer.alloc(0))]); }
function magentaToAlpha(image){ const out={...image,pixels:Buffer.from(image.pixels)};for(let i=0;i<out.pixels.length;i+=4)if(out.pixels[i]===255&&out.pixels[i+1]===0&&out.pixels[i+2]===255){out.pixels[i]=0;out.pixels[i+1]=0;out.pixels[i+2]=0;out.pixels[i+3]=0;}return out; }
function crop(image, rect){ const pixels=Buffer.alloc(rect.width*rect.height*4);for(let y=0;y<rect.height;y++)image.pixels.copy(pixels,y*rect.width*4,((rect.y+y)*image.width+rect.x)*4,((rect.y+y)*image.width+rect.x+rect.width)*4);return {width:rect.width,height:rect.height,pixels}; }
function alphaBounds(image){let minX=image.width,minY=image.height,maxX=-1,maxY=-1;for(let y=0;y<image.height;y++)for(let x=0;x<image.width;x++)if(image.pixels[(y*image.width+x)*4+3]){minX=Math.min(minX,x);minY=Math.min(minY,y);maxX=Math.max(maxX,x);maxY=Math.max(maxY,y);}return maxX<0?null:{x:minX,y:minY,width:maxX-minX+1,height:maxY-minY+1};}
function nearest(image,width,height){ const pixels=Buffer.alloc(width*height*4);for(let y=0;y<height;y++)for(let x=0;x<width;x++){const sx=Math.min(image.width-1,Math.floor(x*image.width/width)),sy=Math.min(image.height-1,Math.floor(y*image.height/height));image.pixels.copy(pixels,(y*width+x)*4,(sy*image.width+sx)*4,(sy*image.width+sx+1)*4);}return {width,height,pixels}; }
module.exports={decode,encode,magentaToAlpha,crop,alphaBounds,nearest};
