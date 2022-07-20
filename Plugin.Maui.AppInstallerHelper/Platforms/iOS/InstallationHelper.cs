using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace Plugin.Maui.AppInstallHelper
{
    public class InstallationHelper 
    {
        [Obsolete("iOS no need to ask for permission")]
        public static async Task<bool> AskForRequiredPermission()
        {
            await Task.Delay(1);
            return true;
        }

        public static string GetPublicDownloadPath()
        {
            return string.Empty;
        }

        [Obsolete("iOS do not need to initialize")]
        public static void Init(string fileProviderAuthorities)
        {
        }

        public static async Task<bool> InstallApp(string path, InstallMode installMode)
        {
            if (installMode == InstallMode.AppStore)
            {
                // App Store URL.
                var appStoreLink = $"https://itunes.apple.com/us/app/apple-store/id{path}?mt=8";
                await Browser.OpenAsync(appStoreLink, BrowserLaunchMode.SystemPreferred);
                return true;
            }
            else if (installMode == InstallMode.OutOfAppStore)
            {
                var supportUri = await Launcher.CanOpenAsync("itms-services://");
                if (supportUri)
                {
                    await Launcher.OpenAsync($"{path}");
                    return true;
                }
                else
                {
                    var alertController = UIAlertController.Create("Alert", "Device does not support itms-services url prefix.", UIAlertControllerStyle.Alert);
                    alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
                    UIApplication.SharedApplication.KeyWindow.RootViewController.PresentedViewController?.PresentViewController(alertController, true, null);
                    return false;
                }
            }

            return false;
        }
    }
}
