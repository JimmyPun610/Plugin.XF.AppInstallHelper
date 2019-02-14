using Plugin.XF.AppInstallHelper.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Essentials;

namespace Plugin.XF.AppInstallHelper
{
    public class XFAppInstallImplementation : AppInstallBase
    {
        [Obsolete("iOS do not need to initialize")]
        public override void Init(string fileProviderAuthorities)
        {
            
        }

        /// <summary>
        /// Install the application
        /// </summary>
        /// <param name="path">AppStore : App Id e.g :940347474, OutOfAppStore : itms-services full path</param>
        /// <param name="installMode"></param>
        public override async Task<bool> InstallApp(string path, InstallMode installMode)
        {
            if (installMode == InstallMode.AppStore)
            {
                // App Store URL.
                var appStoreLink = $"https://itunes.apple.com/us/app/apple-store/id{path}?mt=8";
                await Browser.OpenAsync(appStoreLink, BrowserLaunchMode.SystemPreferred);
                return true;
            }
            else if(installMode == InstallMode.OutOfAppStore)
            {
                var supportUri = await Launcher.CanOpenAsync("itms-services://");
                if(supportUri)
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
