using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Java.IO;
using Java.Nio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Maui.AppInstallHelper
{
    public class InstallationHelper
    {
        public static async Task<bool> AskForRequiredPermission()
        {
            try
            {
                var canRequestInstallPackage = true;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                    canRequestInstallPackage = Platform.CurrentActivity.PackageManager.CanRequestPackageInstalls();

                if (!canRequestInstallPackage)
                {
                    Platform.CurrentActivity.StartActivity(new Intent(
                        Android.Provider.Settings.ActionManageUnknownAppSources,
                        Android.Net.Uri.Parse("package:" + Android.App.Application.Context.PackageName)));
                }

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

                if (writeStatus == PermissionStatus.Granted && readStatus == PermissionStatus.Granted && canRequestInstallPackage)
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
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                    {
                        var packageInstaller = Platform.CurrentActivity.PackageManager.PackageInstaller;
                        var sessionParams = new PackageInstaller.SessionParams(PackageInstallMode.FullInstall);
                        int sessionId = packageInstaller.CreateSession(sessionParams);
                        var session = packageInstaller.OpenSession(sessionId);

                        AddApkToInstallSession(Platform.CurrentActivity.ApplicationContext, Android.Net.Uri.FromFile(file), session);
                        // Create an install status receiver.
                        var intent = new Intent(Platform.CurrentActivity.ApplicationContext, Platform.CurrentActivity.Class);
                        intent.SetAction(Configuration.PackageInstallAction);
                        PendingIntent pendingIntent = null;
                        if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
                            pendingIntent = PendingIntent.GetActivity(Platform.CurrentActivity.ApplicationContext, 0, intent, PendingIntentFlags.Mutable);
                        else
                            pendingIntent = PendingIntent.GetActivity(Platform.CurrentActivity.ApplicationContext, 0, intent, PendingIntentFlags.OneShot);
                        var statusReceiver = pendingIntent.IntentSender;

                        // Commit the session (this will start the installation workflow).
                        session.Commit(statusReceiver);

                        return true;
                    }
                    else if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                    {
                        Android.Net.Uri apkUri = FileProvider.GetUriForFile(Android.App.Application.Context,
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

        private static void AddApkToInstallSession(Context context, Android.Net.Uri apkUri, PackageInstaller.Session session)
        {
            var packageInSession = session.OpenWrite("package", 0, -1);
            var input = context.ContentResolver.OpenInputStream(apkUri);

            try
            {
                if (input != null)
                {
                    input.CopyTo(packageInSession);
                }
                else
                {
                    throw new Exception("Inputstream is null");
                }
            }
            finally
            {
                packageInSession.Close();
                input.Close();
            }

            //That this is necessary could be a Xamarin bug.
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }


        public static void OnNewIntent(Intent intent)
        {
            Bundle extras = intent.Extras;
            if (Configuration.PackageInstallAction.Equals(intent.Action))
            {
                var status = extras.GetInt(PackageInstaller.ExtraStatus);
                var message = extras.GetString(PackageInstaller.ExtraStatusMessage);
                switch (status)
                {
                    case (int)PackageInstallStatus.PendingUserAction:
                        // Ask user to confirm the installation
                        var confirmIntent = (Intent)extras.Get(Intent.ExtraIntent);
                        Platform.CurrentActivity.StartActivity(confirmIntent);
                        break;
                    case (int)PackageInstallStatus.Success:
                        //TODO: Handle success
                        System.Console.WriteLine("Package install success.");
                        break;
                    case (int)PackageInstallStatus.Failure:
                    case (int)PackageInstallStatus.FailureAborted:
                    case (int)PackageInstallStatus.FailureBlocked:
                    case (int)PackageInstallStatus.FailureConflict:
                    case (int)PackageInstallStatus.FailureIncompatible:
                    case (int)PackageInstallStatus.FailureInvalid:
                    case (int)PackageInstallStatus.FailureStorage:
                        //TODO: Handle failures
                        System.Console.WriteLine($"Package install failed, {message}");
                        break;
                }
            }
        }
    }
}
