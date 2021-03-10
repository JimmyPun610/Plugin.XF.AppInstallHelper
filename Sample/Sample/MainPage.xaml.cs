using Plugin.XF.AppInstallHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace Sample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            CurrentVersionCodeLabel.Text = $"Current Version Code : {Xamarin.Essentials.VersionTracking.CurrentBuild}";
            CurrentVersionNameLabel.Text = $"Current Version Name : {Xamarin.Essentials.VersionTracking.CurrentVersion}";
            AskForPermission();
        }


        private async void AskForPermission()
        {
            bool allowed = await Plugin.XF.AppInstallHelper.InstallationHelper.AskForRequiredPermission();
            if (!allowed)
            {
                await DisplayAlert("Alert", "You should allow the permission for download apk", "Ok");
            }
            else
            {
                await DisplayAlert("Congra!", "You have permission to do the next step", "OK");
            }
        }

        private async void ForceUpdateBtn_Clicked(object sender, EventArgs e)
        {
            var updatedVersion = CheckVersion(true);
            this.InputTransparent = true;
            await installUpdate(updatedVersion);
            this.InputTransparent = false;
        }

        private async void NonForceUpdateBtn_Clicked(object sender, EventArgs e)
        {
            var updatedVersion = CheckVersion(false);
            this.InputTransparent = true;
            await installUpdate(updatedVersion);
            this.InputTransparent = false;
        }

        private async Task installUpdate(Plugin.XF.AppInstallHelper.AppVersion updatedVersion)
        {
            if (updatedVersion.VersionCode > int.Parse(Xamarin.Essentials.VersionTracking.CurrentBuild))
            {
                bool confirmUpdate = await PromptUpdate(updatedVersion);
                if (confirmUpdate)
                {
                    string installPath = string.Empty;
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        installPath = updatedVersion.iOSPath;
                    }
                    else
                    {
                        installPath = System.IO.Path.Combine(Plugin.XF.AppInstallHelper.InstallationHelper.GetPublicDownloadPath(), "APK.APK");



                        using (HttpClient hc = new HttpClient())
                        {
                            var response = await hc.GetAsync(updatedVersion.AndroidPath);
                            var byteArray = await response.Content.ReadAsByteArrayAsync();
                            System.IO.File.WriteAllBytes(installPath, byteArray);
                        }
                    }
                    bool result = await Plugin.XF.AppInstallHelper.InstallationHelper.InstallApp(installPath, InstallMode.OutOfAppStore);
                }
            }
            else
            {
                await DisplayAlert("Alert", "There is no more update", "OK");
            }
        }

        private async Task<bool> PromptUpdate(Plugin.XF.AppInstallHelper.AppVersion newVerInfo)
        {
            bool confirmUpdate = true;
            if (newVerInfo.ForceUpdate)
            {
                await DisplayAlert($"New version {newVerInfo.VersionName} occur", $"Release notes : \r\n{newVerInfo.ReleaseNotes}", "Update");
            }
            else
            {
                confirmUpdate = await DisplayAlert($"New version {newVerInfo.VersionName} occur", $"Release notes : \r\n{newVerInfo.ReleaseNotes}", "Update", "Skip");
            }
            return confirmUpdate;
        }

        /// <summary>
        /// Simulate getting new version information from internet
        /// </summary>
        /// <param name="forceUpdate"></param>
        /// <returns></returns>
        private Plugin.XF.AppInstallHelper.AppVersion CheckVersion(bool forceUpdate)
        {
            Plugin.XF.AppInstallHelper.AppVersion version2 = new Plugin.XF.AppInstallHelper.AppVersion
            {
                ForceUpdate = forceUpdate,
                ReleaseNotes = "1. testing update\r\n2. Please install",
                VersionCode = 4,
                VersionName = "2.1.0",
                AndroidPath = "https://github.com/JimmyPun610/Plugin.XF.AppInstallHelper/raw/master/App/plugin.xf.appinstallhelper.sample_v4.apk",
                iOSPath = ""
            };
            return version2;
        }

        private void AskForPermissionBtn_Clicked(object sender, EventArgs e)
        {
            AskForPermission();
        }
    }
}
