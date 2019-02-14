using Android;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Plugin.CurrentActivity;
using Plugin.XF.AppInstallHelper.Abstractions;
using System;

namespace Plugin.XF.AppInstallHelper
{
    public class XFAppInstallImplementation : AppInstallBase
    {
        /// <summary>
        /// Initialize the library with the file provider
        /// </summary>
        /// <param name="fileProviderAuthorities"></param>
        public override void Init(string fileProviderAuthorities)
        {
            base.Init(fileProviderAuthorities);
        }

        /// <summary>
        /// Create intent to install the application / open google play store of the application
        /// </summary>
        /// <param name="path">
        /// If install from Play store, please fill in the package name in Play store.
        /// If install apk, please provide full local path of the apk file
        /// </param>
        /// <param name="installMode"></param>
        public override void InstallApp(string path, InstallMode installMode)
        {
            if(installMode == InstallMode.OutOfAppStore)
            {
                if(Build.VERSION.SdkInt >= BuildVersionCodes.M)
                {
                    if(ContextCompat.CheckSelfPermission(Android.App.Application.Context, Manifest.Permission.ReadExternalStorage) == (int)Permission.Granted)
                    {
                        Java.IO.File file = new Java.IO.File(path);
                        Android.Net.Uri apkUri = FileProvider.GetUriForFile(Android.App.Application.Context,
                        this._fileProviderAuthorities, file);
                        if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                        {
                            Intent intent = new Intent(Intent.ActionInstallPackage);
                            intent.SetData(apkUri);
                            intent.SetFlags(ActivityFlags.GrantReadUriPermission);
                            Android.App.Application.Context.StartActivity(intent);
                        }
                        else
                        {
                            Intent intent = new Intent(Intent.ActionView);
                            intent.SetDataAndType(apkUri, "application/vnd.android.package-archive");
                            intent.SetFlags(ActivityFlags.NewTask);
                            Android.App.Application.Context.StartActivity(intent);
                        }
                    }
                    else
                    {
                        ActivityCompat.RequestPermissions(CrossCurrentActivity.Current.Activity, new string[] { Manifest.Permission.ReadExternalStorage }, 100);
                    }
                }   
            }else if(installMode == InstallMode.AppStore)
            {
                Intent intent = new Intent(Intent.ActionView);
                intent.SetData(Android.Net.Uri.Parse($"market://details?id={path}"));
                Android.App.Application.Context.StartActivity(intent);
            }
        }
    }
}
