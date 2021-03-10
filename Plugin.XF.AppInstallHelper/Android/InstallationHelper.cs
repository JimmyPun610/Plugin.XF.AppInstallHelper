using Android;
using Android.Content;
using Android.OS;
using Android.Support.V4.Content;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Plugin.XF.AppInstallHelper
{
    public class InstallationHelper 
    {
        public static async Task<bool> AskForRequiredPermission()
        {
            try
            {
                var writeStatus = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                if (writeStatus != PermissionStatus.Granted)
                {
                    await Permissions.RequestAsync<Permissions.StorageWrite>();
                }
                writeStatus = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

                var readStatus = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                if (readStatus != PermissionStatus.Granted)
                {
                    await Permissions.RequestAsync<Permissions.StorageRead>();
                }

                readStatus = await Permissions.CheckStatusAsync<Permissions.StorageRead>();

                if (writeStatus == PermissionStatus.Granted && readStatus == PermissionStatus.Granted)
                    return true;
            }
            catch (Exception ex)
            {
                //Something went wrong
            }
            return false;
        }

        public static string GetPublicDownloadPath()
        {
            return Android.App.Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
        }

        public static void Init(string fileProviderAuthorities)
        {
            Configuration.FileProviderAuthorities = fileProviderAuthorities;
        }

        public static async Task<bool> InstallApp(string path, InstallMode installMode)
        {
            if (installMode == InstallMode.OutOfAppStore)
            {
                bool permissionGranted = true;
                do
                {
                    permissionGranted = await AskForRequiredPermission();
                } while (!permissionGranted);

                if (permissionGranted)
                {
                    Java.IO.File file = new Java.IO.File(path);

                    if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                    {
                        Android.Net.Uri apkUri = Android.Support.V4.Content.FileProvider.GetUriForFile(Android.App.Application.Context,
                     Configuration.FileProviderAuthorities, file);
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
            }
            else if (installMode == InstallMode.AppStore)
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
    }
}
