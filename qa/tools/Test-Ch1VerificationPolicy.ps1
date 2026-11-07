param([string]$RepoRoot = (Join-Path $PSScriptRoot '../..'))
$ErrorActionPreference='Stop'
# Runs source-extracted pure branches and real temporary filesystem cases; never loads Unity assemblies.
$RepoRoot=[IO.Path]::GetFullPath($RepoRoot)
$asset=Join-Path $RepoRoot 'output/chapter1-playable-r1/project/Assets/AcadeGameMaker'
function Read-Source([string]$Relative){return [IO.File]::ReadAllText((Join-Path $asset $Relative))}
function Slice([string]$Text,[string]$Start,[string]$End){$a=$Text.IndexOf($Start,[StringComparison]::Ordinal);if($a -lt 0){throw "Source marker missing: $Start"};$b=$Text.IndexOf($End,$a,[StringComparison]::Ordinal);if($b -lt 0){throw "Source marker missing: $End"};return $Text.Substring($a,$b-$a)}
$pilot=Read-Source 'Runtime/Input/Unity/Chapter1VerificationInputPilot.cs'
$producer=Read-Source 'Runtime/Combat/Unity/OrdanBossHostileDamageProducer.cs'
$boss=Read-Source 'Runtime/Combat/OrdanBossSession.cs'
$pull=Read-Source 'Runtime/Combat/Unity/OrdanBossBalanceAuditPullProducer.cs'
$motor=Read-Source 'Runtime/Movement/PlayerMovementMotor.cs'
if($motor -notmatch 'Lightweight\s*\?\s*65\s*:\s*100'){throw 'Authored gravity multiplier changed; revisit pure bound'}
$timing=Slice $boss '        private static int TelegraphDuration' '        private static string Format'
$driver=Read-Source 'Runtime/Run/Unity/Chapter1SameBuildVerificationDriver.cs'
$fixed=(Read-Source 'Runtime/Movement/FixedQ4096.cs').Replace('using System;','')
$policy=(Slice $pilot '    internal sealed class Chapter1VerificationBossPolicy' '        internal static string FormatTrace')+"`n    }`n"
$geometry=Slice $producer '    internal readonly struct ClosedAabbQ4096' '        private readonly struct RationalQ'
$constants=([regex]::Matches($producer,'(?m)^\s*private const int \w+\s*=\s*[^;]+;')|ForEach-Object{$_.Value}) -join "`n"
$arrays=([regex]::Matches($producer,'(?m)^\s*private static readonly int\[\] \w+\s*=\s*\{[^;]+;')|ForEach-Object{$_.Value}) -join "`n"
$getters=([regex]::Matches($producer,'(?m)^\s*internal static [^\r\n]+Approved[^\r\n]+;')|Where-Object{$_.Value -notmatch 'ApprovedOccluders'}|ForEach-Object{$_.Value}) -join "`n"
$pullConstants=([regex]::Matches($pull,'(?m)^\s*internal const int Approved\w+\s*=\s*[^;]+;')|ForEach-Object{$_.Value}) -join "`n"
$enums=foreach($name in @('OrdanPatternSlot','OrdanBossPhase','OrdanHealthStage')){$match=[regex]::Match($boss,('internal enum '+$name+'\s*\{[^}]+\}'));if(!$match.Success){throw "Missing enum: $name"};$match.Value}
$saveMethod=(Slice $driver '        private static bool ObserveEmptySaveRoot' '        private static double Now').Replace('private static bool','internal static bool')
# Names/value adapters only: no physics implementation and no successful standalone authority.
$adapters='internal enum Key { A,D,Space } internal sealed class Chapter1StandaloneVerificationSession {internal bool Active=>false;internal object Pilot=>null;internal long Id=>0;internal string Mode=>null;}'
$tests=@'
internal static class Program {
 static int count;static readonly object issuer=new object(),lease=new object(),router=new object(),player=new object();
 static void Check(string name,bool value){if(!value)throw new Exception("FAIL "+name);count++;Console.WriteLine("PASS "+name);}
 static Chapter1VerificationBossPolicy.Threat Threat(bool queued=false,bool contact=false,bool pulling=false,bool wall=false)=>new Chapter1VerificationBossPolicy.Threat(OrdanHealthStage.Stage1,false,queued,contact,false,pulling,28672,4915,410,0,wall);
 static void Consume(Chapter1VerificationBossPolicy p,Key[] keys,long sequence,bool ready=true){p.ObserveOwnedConsumption(issuer,lease,"source","case","birth",1,1,1,keys,sequence,ready);}
 static Chapter1VerificationBossPolicy Ready(bool owned=true){var p=new Chapter1VerificationBossPolicy();p.Consumed(false,true,1);p.Consumed(false,true,2);if(owned){p.BindOwnedJump(new Chapter1VerificationBossPolicy.OwnedJumpIdentity(issuer,lease,router,player,"source","case","birth",1,1,1,1,true));Consume(p,Array.Empty<Key>(),1);Consume(p,Array.Empty<Key>(),2);}return p;}
 static Chapter1VerificationBossPolicy.Command Read(Chapter1VerificationBossPolicy p,int tick=101,int age=3,float y=1.04126f,float vy=13.87793f,bool grounded=false,bool locked=false,bool focus=true,bool queued=false,bool contact=false,bool pulling=false,OrdanPatternSlot slot=OrdanPatternSlot.DebtLineA,OrdanBossPhase phase=OrdanBossPhase.Telegraph,float x=-7f,bool wall=false){return p.Read("run",1,tick,x,y,.333f,grounded,locked,focus,false,0,slot,phase,age,.35f,vy,Threat(queued,contact,pulling,wall));}
 static Chapter1VerificationBossPolicy Launch(){var p=Ready();var c=Read(p,100,2,.81f,0,true);if(Array.IndexOf(c.Keys,Key.Space)<0)throw new Exception("launch rejected");Consume(p,c.Keys,3);return p;}
 static bool Envelope(float y=1.04126f,float vy=13.87793f,float x=-7f,float vx=.333f,int direction=1,int age=3,OrdanPatternSlot slot=OrdanPatternSlot.DebtLineA,float latency=.35f)=>Chapter1VerificationBossPolicy.OwnedDebtLineEnvelopeClear(x,y,vx,vy,direction,slot,age,latency);
 static bool Throws(Action a){try{a();return false;}catch{return true;}}
 static void Main(string[] args){
  Check("source gravity binding",AcadeGameMaker.Movement.FixedQ4096.Gravity==125829 && AcadeGameMaker.Movement.FixedQ4096.JumpVelocity==58941 && AcadeGameMaker.Movement.FixedQ4096.TicksPerSecond==60 && AcadeGameMaker.Movement.FixedQ4096.LightweightMaxFallSpeed==63078);
  Check("rounded bounds below authored Q4096 slow motion",19.96f<AcadeGameMaker.Movement.FixedQ4096.Gravity/4096f*.65f && 15.39f<AcadeGameMaker.Movement.FixedQ4096.LightweightMaxFallSpeed/4096f);
  Check("actual fastest next-pattern timing",BossTimingProbe.FastestDebtTicks()==51 && BossTimingProbe.NextTelegraphTicks()==60);
  Check("E2 numeric owned envelope",Envelope());Check("higher DebtLineB rejected",!Envelope(slot:OrdanPatternSlot.DebtLineB));
  var p=Launch();var c=Read(p);Check("actual Read owned fallback",p.Unresolved==null && p.OwnedContinuationActive && Array.IndexOf(c.Keys,Key.Space)>=0);var q=Ready(false);Read(q);Check("unowned same state denied",q.Unresolved!=null && !q.OwnedContinuationActive);
  Check("descending into fire sample",!Envelope(y:3,vy:-2,age:20));Check("insufficient apex",!Envelope(vy:2));Check("wall landing corridor",!Envelope(x:-10,vx:-8,direction:-1));Check("boss body corridor",!Envelope(x:4));Check("ceiling/apex",!Envelope(y:8));Check("next hostile margin",!Envelope(latency:3));
  Check("Stage3 sample margin accepts 1.16 latency",Envelope(latency:1.16f));Check("Stage3 sample margin denies 1.18 latency",!Envelope(latency:1.18f));
  Check("wrong slot",!Envelope(slot:OrdanPatternSlot.SeizureWeight));Check("zero phase age",!Envelope(age:0));Check("expired phase age",!Envelope(age:47));Check("nonfinite velocity",!Envelope(vy:float.NaN));Check("walljump/dash velocity",!Envelope(vx:9.5f));
  var upper=(OrdanBossHostileDamageProducer.ApprovedDebtLineA.MaxY+OrdanBossHostileDamageProducer.ApprovedPlayerHalfHeight)/4096f+.02f;var boundary=(float)Math.Ceiling((upper-Math.Min(Chapter1VerificationBossPolicy.LowerBallisticHeight(0,13.87793f,44f/60),Chapter1VerificationBossPolicy.LowerBallisticHeight(0,13.87793f,46f/60)))*4096)/4096;Check("one quantum below closed line",!Envelope(y:boundary-1f/4096));Check("one quantum above closed line",Envelope(y:boundary+1f/4096));
  p=Launch();c=Read(p,vy:-5);Check("unsafe own trajectory revokes",p.Unresolved!=null && !p.OwnedContinuationActive && c.Keys.Length==0);
  p=Launch();Read(p,locked:true);Check("lock revokes",!p.OwnedContinuationActive);p=Launch();Read(p,queued:true);Check("queued delivery revokes",!p.OwnedContinuationActive);p=Launch();Read(p,contact:true);Check("contact revokes",!p.OwnedContinuationActive);p=Launch();Read(p,pulling:true);Check("pull revokes",!p.OwnedContinuationActive);p=Launch();Read(p,focus:false);Check("focus loss revokes",!p.OwnedContinuationActive);p=Launch();Read(p,tick:102,age:4);Check("tick gap revokes",!p.OwnedContinuationActive);p=Launch();Read(p,slot:OrdanPatternSlot.DebtLineB);Check("slot change revokes",!p.OwnedContinuationActive);p=Launch();Read(p,age:4);Check("phase start change revokes",!p.OwnedContinuationActive);p=Launch();Consume(p,new[]{Key.D},4);Read(p);Check("Space release revokes",!p.OwnedContinuationActive);p=Launch();Consume(p,new[]{Key.D,Key.Space},5);Read(p);Check("receipt gap revokes",!p.OwnedContinuationActive);
  p=Launch();c=Read(p,wall:true);Check("observed wall contact neutralizes owned carry",!p.OwnedContinuationActive && p.Unresolved!=null && c.Keys.Length==0);p=Launch();Read(p,wall:true,x:9);Check("wall revokes even on ordinary safe route",!p.OwnedContinuationActive);p=Launch();Read(p,grounded:true);Check("landing revokes",!p.OwnedContinuationActive);p=Launch();Read(p,phase:OrdanBossPhase.Execute);Check("phase change revokes",!p.OwnedContinuationActive);p=Launch();p.BindOwnedJump(new Chapter1VerificationBossPolicy.OwnedJumpIdentity(issuer,null,router,player,"source","case","birth",1,1,1,1,true));Read(p);Check("lost lease revokes",!p.OwnedContinuationActive);
  p=Launch();for(int tick=101;tick<=144;tick++){var dt=(tick-101)/60f;var cmd=Read(p,tick,tick-98,1.04126f+13.87793f*dt-15.36f*dt*dt-.256f*dt,13.87793f-30.72f*dt);if(p.Unresolved!=null)throw new Exception("finite flight rejected tick "+tick);Consume(p,cmd.Keys,tick-97);}Check("original finite deadline",p.OwnedContinuationDeadline==148);Read(p,145,47);Check("phase guard expires",!p.OwnedContinuationActive);
  var root=args[0];Directory.CreateDirectory(Path.Combine(root,"empty"));Directory.CreateDirectory(Path.Combine(root,"occupied"));File.WriteAllText(Path.Combine(root,"occupied","save.json"),"{}");File.WriteAllText(Path.Combine(root,"file"),"fixture");Check("save empty",SaveRootProbe.ObserveEmptySaveRoot(Path.Combine(root,"empty")));Check("save absent",SaveRootProbe.ObserveEmptySaveRoot(Path.Combine(root,"absent","Chapter1")));Check("save occupied",!SaveRootProbe.ObserveEmptySaveRoot(Path.Combine(root,"occupied")));Check("save file throws",Throws(()=>SaveRootProbe.ObserveEmptySaveRoot(Path.Combine(root,"file"))));Check("save ancestor file throws",Throws(()=>SaveRootProbe.ObserveEmptySaveRoot(Path.Combine(root,"file","Chapter1"))));Check("save invalid path throws",Throws(()=>SaveRootProbe.ObserveEmptySaveRoot("bad\0path")));
  Console.WriteLine("TOTAL "+count+" passed; no Unity physics or runtime evidence.");
 }
}
'@
$code="using System;using System.IO;using System.Collections.Generic;`n"+($enums -join "`n")+"`n"+$adapters+"`n"+$geometry+"`ninternal static class BossTimingProbe {`n"+$timing+"`ninternal static int FastestDebtTicks()=>Duration(OrdanPatternSlot.DebtLineA,OrdanHealthStage.Stage3,OrdanBossPhase.Execute,false)+Duration(OrdanPatternSlot.DebtLineA,OrdanHealthStage.Stage3,OrdanBossPhase.Recovery,false);internal static int NextTelegraphTicks()=>TelegraphDuration(OrdanPatternSlot.SeizureWeight);`n}`ninternal static class OrdanBossHostileDamageProducer {`n"+$constants+"`n"+$arrays+"`n"+$getters+"`n}`ninternal static class OrdanBossBalanceAuditPullProducer {`n"+$pullConstants+"`n}`n"+$policy+"`ninternal static class SaveRootProbe {`n"+$saveMethod+"`n}`n"+$fixed+"`n"+$tests
if($code -match 'using UnityEngine|DllImport|MonoBehaviour'){throw 'Unexpected native/Unity dependency in pure source'}
$dotnet=Get-Command dotnet -ErrorAction Stop
$sdkLine=(& $dotnet.Source --list-sdks|Select-Object -Last 1);if($sdkLine -notmatch '^(\S+)\s+\[(.+)\]'){throw 'Cannot locate SDK'}
$csc=Join-Path (Join-Path $Matches[2] $Matches[1]) 'Roslyn/bincore/csc.dll'
$runtimeLine=(& $dotnet.Source --list-runtimes|Where-Object{$_ -like 'Microsoft.NETCore.App *'}|Select-Object -Last 1);if($runtimeLine -notmatch '^Microsoft.NETCore.App\s+(\S+)\s+\[(.+)\]'){throw 'Cannot locate runtime'}
$version=$Matches[1];$runtimeDir=Join-Path $Matches[2] $version
$dotnetRoot=Split-Path $dotnet.Source -Parent
$packRoot=Join-Path $dotnetRoot 'packs/Microsoft.NETCore.App.Ref'
$major=$version.Split('.')[0]
$pack=Get-ChildItem -LiteralPath $packRoot -Directory|Where-Object{$_.Name -like ($major+'.*')}|Sort-Object {[version]$_.Name}|Select-Object -Last 1
if(!$pack){throw 'Installed reference pack missing; no restore/download attempted'}
$referenceDir=Join-Path $pack.FullName ('ref/net'+$major+'.0')
$temp=Join-Path ([IO.Path]::GetTempPath()) ('ch1-pure-'+[Guid]::NewGuid().ToString('N'));New-Item -ItemType Directory -Path $temp|Out-Null
try {
 $cs=Join-Path $temp 'Policy.cs';$exe=Join-Path $temp 'Policy.dll';[IO.File]::WriteAllText($cs,$code,[Text.UTF8Encoding]::new($false))
 $references=Get-ChildItem -LiteralPath $referenceDir -Filter '*.dll'|ForEach-Object{'/reference:'+$_.FullName}
 $compileOutput=& $dotnet.Source $csc /nologo /noconfig /nostdlib+ /target:exe ('/out:'+$exe) @references $cs 2>&1
 if($LASTEXITCODE -ne 0){Write-Output (($compileOutput -join "`n").Replace($temp,'<temporary>'));throw 'Pure source compile failed'}
 $config=@{runtimeOptions=@{tfm=('net'+$version.Split('.')[0]+'.0');framework=@{name='Microsoft.NETCore.App';version=$version}}}|ConvertTo-Json -Depth 4
 [IO.File]::WriteAllText((Join-Path $temp 'Policy.runtimeconfig.json'),$config,[Text.UTF8Encoding]::new($false))
 & $dotnet.Source $exe (Join-Path $temp 'filesystem');if($LASTEXITCODE -ne 0){throw 'Pure source regression failed'}
} finally {if(Test-Path -LiteralPath $temp){Remove-Item -LiteralPath $temp -Recurse -Force}}
