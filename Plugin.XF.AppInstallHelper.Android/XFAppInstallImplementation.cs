using Android;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Widget;
using Plugin.CurrentActivity;
using Plugin.XF.AppInstallHelper.Abstractions;
using System;
using System.Threading.Tasks;

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
        public override async Task<bool> InstallApp(string path, InstallMode installMode)
        {
            if(installMode == InstallMode.OutOfAppStore)
            {
                bool permissionGranted = true;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                {
                    //Android 6.0 Upper
                    Plugin.Permissions.Abstractions.PermissionStatus status = await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage);
                    permissionGranted = ContextCompat.CheckSelfPermission(Android.App.Application.Context, Manifest.Permission.ReadExternalStorage) == (int)Android.Content.PM.Permission.Granted;
                    while (!permissionGranted)
                    {
                        await Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Storage);
                        permissionGranted = ContextCompat.CheckSelfPermission(Android.App.Application.Context, Manifest.Permission.ReadExternalStorage) == (int)Android.Content.PM.Permission.Granted;
                    }
                }
                if (permissionGranted)
                {
                    Java.IO.File file = new Java.IO.File(path);
                  
                    if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                    {
                        Android.Net.Uri apkUri = FileProvider.GetUriForFile(Android.App.Application.Context,
                     this._fileProviderAuthorities, file);
                        Intent intent = new Intent(Intent.ActionInstallPackage);
                        intent.SetData(apkUri);
                        intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                        intent.AddFlags(ActivityFlags.NewTask);
                        Android.App.Application.Context.StartActivity(intent);
                        return true;
                    }
                    else
                    {
                        Intent intent = new Intent(Intent.ActionView);
                        intent.SetDataAndType(Android.Net.Uri.FromFile(file), "application/vnd.android.package-archive");
                        intent.AddFlags(ActivityFlags.NewTask);
                        Android.App.Application.Context.StartActivity(intent);
                        return true;
                    }
                }
                else return false;
            }else if(installMode == InstallMode.AppStore)
            {
                Intent intent = new Intent(Intent.ActionView);
                intent.SetData(Android.Net.Uri.Parse($"market://details?id={path}"));
                Android.App.Application.Context.StartActivity(intent);
                return true;
            }
            else
            //Unknown issue
            return false;
        }


        public override string GetPublicDownloadPath()
        {
            return Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
        }
    }
}
