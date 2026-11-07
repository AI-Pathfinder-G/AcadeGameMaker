using System;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;
using UnityEngine;

namespace AcadeGameMaker.Run.Unity
{
    internal readonly struct Chapter1RoomPlan
    {
        internal Chapter1RoomPlan(int seed,string[] rooms,string[] sockets,string sha){Seed=seed;_rooms=(string[])rooms.Clone();_sockets=(string[])sockets.Clone();CanonicalSha=sha;}
        private readonly string[] _rooms,_sockets;
        internal int Seed {get;}
        internal string CanonicalSha {get;}
        internal string RoomAt(int index)=>_rooms[index];
        internal string SocketAt(int index)=>_sockets[index];
    }
    // REQ-C1B-ROOM-001: canonical 원문 네 개 외의 생성·재직렬화 계획을 받지 않는다.
    internal sealed class Chapter1RoomCatalog : MonoBehaviour
    {
        [SerializeField] private GameObject[] _rooms;
        [SerializeField] private TextAsset[] _plans;
        internal void ConfigureForAuthoring(GameObject[] rooms,TextAsset[] plans)
        {if(gameObject.activeInHierarchy || _rooms!=null || rooms==null || plans==null)throw new InvalidOperationException("카탈로그는 비활성 상태에서 한 번 구성합니다.");_rooms=(GameObject[])rooms.Clone();_plans=(TextAsset[])plans.Clone();Validate(false);}
        internal void Validate(bool requireBasicMovement)
        {
            if(_rooms==null || _rooms.Length!=6 || _plans==null || _plans.Length!=4)throw new InvalidOperationException("풀6·정수 계획4가 필요합니다.");
            for(var i=0;i<6;i++)
            {
                var prefab=_rooms[i];var id="R0"+(i+1);
                if(prefab==null || prefab.activeSelf || prefab.GetComponentsInChildren<Chapter1RoomInstance>(true).Length!=1)throw new InvalidOperationException("방 prefab이 올바르지 않습니다.");
                var room=prefab.GetComponent<Chapter1RoomInstance>();
                if(room==null || room.RoomId!=id)throw new InvalidOperationException("방 ID와 순서가 다릅니다.");room.ValidateStructure();
                if(requireBasicMovement && !room.IsBasicMovementAccepted)throw new InvalidOperationException("실제 기본 이동 미검증 방은 정규 풀에 편입할 수 없습니다.");
            }
            for(var i=0;i<4;i++)ReadPlan(Seeds[i]);
        }
        internal GameObject Prefab(string roomId){if(roomId==null || roomId.Length!=3 || roomId[2]<'1'||roomId[2]>'6')throw new ArgumentException("지정 방 ID가 아닙니다.");var p=_rooms[roomId[2]-'1'];if(p.GetComponent<Chapter1RoomInstance>().RoomId!=roomId)throw new ArgumentException("지정 방 ID가 아닙니다.");return p;}
        internal Chapter1RoomPlan Select(int? explicitSeed,int? lastOfferedSeed)
        {Validate(true);return ReadPlan(explicitSeed??NextSeed(lastOfferedSeed));}
        internal static int NextSeed(int? last)
        {if(!last.HasValue)return 101;switch(last.Value){case 101:return 303;case 303:return 202;case 202:return 404;case 404:return 101;default:throw new ArgumentException("저장된 제안 seed가 지정 순환 밖입니다.");}}
        private static readonly int[] Seeds={101,202,303,404};
        internal Chapter1RoomPlan ReadPlan(int seed)
        {
            var index=Array.IndexOf(Seeds,seed);if(index<0)throw new ArgumentException("지정 검증 seed 외에는 허용하지 않습니다.");
            var rooms=new[]{"R01",seed==101||seed==202?"R02":"R03",seed==101||seed==303?"R04":"R05","R06"};
            var sockets=new[]{"S01-A",seed==202?"S02-B":seed==404?"S03-B":seed==101?"S02-A":"S03-A",
                seed==303?"S04-B":seed==404?"S05-B":seed==101?"S04-A":"S05-A","S06-A"};
            var canonical="{\"orderedRooms\":[";
            for(var i=0;i<4;i++)canonical+=(i==0?"":",")+"{\"roomId\":\""+rooms[i]+"\",\"socketPlanId\":\""+sockets[i]+"\"}";
            canonical+="],\"schemaVersion\":1,\"seed\":"+seed.ToString(CultureInfo.InvariantCulture)+",\"selectionRuleVersion\":1}\n";
            var expected=Encoding.UTF8.GetBytes(canonical);var asset=_plans[index];
            if(asset==null || asset.bytes.Length!=expected.Length)throw new InvalidOperationException("canonical 계획 원문 길이가 다릅니다.");
            var actual=asset.bytes;for(var i=0;i<actual.Length;i++)if(actual[i]!=expected[i])throw new InvalidOperationException("canonical 계획 원문 바이트가 다릅니다.");
            string sha;using(var h=SHA256.Create())sha=BitConverter.ToString(h.ComputeHash(actual)).Replace("-","");
            for(var i=0;i<4;i++)if(!Prefab(rooms[i]).GetComponent<Chapter1RoomInstance>().SupportsSocketPlan(sockets[i]))throw new InvalidOperationException("계획의 소켓이 저작 방에 없습니다.");
            return new Chapter1RoomPlan(seed,rooms,sockets,sha);
        }
    }
}
