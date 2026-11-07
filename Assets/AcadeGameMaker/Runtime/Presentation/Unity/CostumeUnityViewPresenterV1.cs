using System;

namespace AcadeGameMaker.Presentation.Unity
{
    // REQ-CUA-001/002/009: deliberately a pure view relay, never a scene or UI lookup.
    public sealed class CostumeUnityViewPresenterV1
    {
        private readonly CostumeUnityPresentationAdapterV1 _adapter;
        public CostumeUnityViewPresenterV1(CostumeUnityPresentationAdapterV1 adapter) { _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter)); }
        public CostumeUnityViewV1 View { get { return _adapter.SnapshotView(); } }
        public CostumeUnityAdapterRejectionV1 Highlight(string costumeId) { return _adapter.Highlight(costumeId); }
    }
}
