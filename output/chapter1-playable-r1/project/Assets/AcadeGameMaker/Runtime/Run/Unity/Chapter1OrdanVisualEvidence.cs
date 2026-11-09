using AcadeGameMaker.Combat.Unity;
using UnityEngine;

namespace AcadeGameMaker.Run.Unity
{
    // CH1 boss art reader only. A missing or terminal +110 publication
    // never licenses an invented telegraph, attack, or hit animation.
    internal static class Chapter1OrdanVisualEvidence
    {
        internal static bool TryRead(Transform ownedEncounterGraph,
            out int patternSlot,out int phase,out int phaseAge)
        {
            patternSlot=0;phase=0;phaseAge=0;
            if(ownedEncounterGraph==null ||
               !ownedEncounterGraph.gameObject.activeInHierarchy)
                return false;
            var adapter=ownedEncounterGraph.GetComponentInChildren<
                OrdanBossEncounterHandoffAdapter>(false);
            if(adapter==null || !adapter.gameObject.activeInHierarchy ||
               adapter.transform.root!=ownedEncounterGraph.root)
                return false;
            var published=adapter.LatestPresentationView;
            if(!published.HasValue || published.Value.IsTerminal)
                return false;
            var view=published.Value;
            var snapshot=view.Snapshot;
            if(snapshot.Tick!=view.Tick || snapshot.PhaseAge<0)
                return false;
            patternSlot=(int)snapshot.PatternSlot;
            phase=(int)snapshot.Phase;
            phaseAge=snapshot.PhaseAge;
            return true;
        }
    }
}
