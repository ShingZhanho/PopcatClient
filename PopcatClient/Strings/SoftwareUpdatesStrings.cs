using PopcatClient.Languages;

namespace PopcatClient
{
    public static partial class Strings
    {
        public static class SoftwareUpdates
        {
            public static string WarningMsg_UpdateDisabled() => LanguageManager.GetString("warn-msg_update_disabled");
            public static string SucMsg_ApplicationUpToDate() => LanguageManager.GetString("success-msg_up_to_date");
            public static string WarningMsg_NewVersionAvailable(string newVersion) =>
                LanguageManager.GetString("warn-msg_new_update").Substitute("new_ver", newVersion);
            public static string WarningMsg_CannotDetermineIfUpToDate() =>
                LanguageManager.GetString("warn-msg_cannot_determine_if_uptodate");
            public static string WarningMsg_CannotDetermineIfDownloaded() =>
                LanguageManager.GetString("warn-msg_cannot_determine_if_downloaded");
            public static string WarningMsg_CannotDetermineIfReady() =>
                LanguageManager.GetString("warn-msg_cannot_determine_if_ready");
            public static string WarningMsg_AppWillRestartAfterUpdate() =>
                LanguageManager.GetString("warn-msg_update_restart_app");
            public static string Msg_CheckingUpdates() => LanguageManager.GetString("msg_checking_updates");
            public static string ErrMsg_UpdateCheckFailed(string reason) => 
                LanguageManager.GetString("err-msg_update_check_failed")
                    .Substitute("fail_reason", reason);
            public static string Verbose_ErrMsg_UpdateCheckFailed(string stacktrace) =>
                LanguageManager.GetString("verbose@err-msg_update_check_failed")
                    .Substitute("fail_stacktrace", stacktrace);
            public static string Msg_DownloadingAsset() => LanguageManager.GetString("msg_downloading_asset");
            public static string SucMsg_AssetDownloaded(string filepath) =>
                LanguageManager.GetString("success-msg_asset_downloaded")
                    .Substitute("file_path", filepath);
            public static string ErrMsg_DownloadAssetFailed(string reason) =>
                LanguageManager.GetString("err-msg_download_asset_failed")
                    .Substitute("fail_reason", reason);
            public static string Verbose_ErrMsg_DownloadAssetFailed(string stacktrace) =>
                LanguageManager.GetString("verbose@err-msg_download_asset_failed")
                    .Substitute("fail_stacktrace", stacktrace);
            public static string Msg_PreparingAsset() => LanguageManager.GetString("msg_preparing_asset");
            public static string SucMsg_AssetIsReady() => LanguageManager.GetString("success-msg_asset_ready");
            public static string ErrMsg_AssetPrepareFailed(string reason) =>
                LanguageManager.GetString("err-msg_asset_prepare_failed")
                    .Substitute("fail_reason", reason);
            public static string Verbose_ErrMsg_AssetPrepareFailed(string stacktrace) =>
                LanguageManager.GetString("verbose@err-msg_asset_prepare_failed")
                    .Substitute("stacktrace", stacktrace);
            public static string Msg_InstallingUpdate() => LanguageManager.GetString("msg_installing_update");

            public static string ErrMsg_InstallerExitCode(int exitCode) =>
                LanguageManager.GetString("err-msg_installer_exit_code")
                    .Substitute("exit_code", exitCode.ToString());

            public static string Verbose_ErrMsg_InstallerExitCode_1() =>
                LanguageManager.GetString("verbose@2rr-msg_installer_exit_code_1");

            public static string Verbose_ErrMsg_InstallerExitCode_2() =>
                LanguageManager.GetString("verbose@err-msg_installer_exit_code_2");

            public static string Verbose_ErrMsg_InstallerExitCode_3() =>
                LanguageManager.GetString("verbose@err-msg_installer_exit_code_3");

            public static string Verbose_ErrMsg_InstallerExitCode_Others() =>
                LanguageManager.GetString("verbpse@err-msg_installer_exit_code_others");
        }
    }
    
}