using AcadeGameMaker.Input.Unity;

namespace AcadeGameMaker.Hub.Presentation.Unity
{
    // REQ-M5D7QA-004/007: this is the only Korean copy table in the shell.
    internal static class HubMenuCopyV1
    {
        internal const string Continue = "계속하기";
        internal const string NewGame = "새 게임";
        internal const string Settings = "설정";
        internal const string Quit = "종료";
        internal const string Dismiss = "×";

        internal static string Notification(ProfileLaunchNotificationKindV1 kind)
        {
            switch (kind)
            {
                case ProfileLaunchNotificationKindV1.RecoveryCompleted: return "프로필 복구를 완료했습니다.";
                case ProfileLaunchNotificationKindV1.RecoveryArtifactPreservationFailed: return "프로필은 복구했지만 손상 파일을 보존하지 못했습니다.";
                case ProfileLaunchNotificationKindV1.PersistenceDeferred: return "프로필을 불러왔지만 변경 사항을 저장하지 못했습니다.";
                default: throw new System.ArgumentOutOfRangeException(nameof(kind));
            }
        }
    }
}
