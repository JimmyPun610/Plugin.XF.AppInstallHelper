# Plugin.XF.AppInstallHelper
Xamarin Form / Maui helper for install application

Support Android 4.4+ / iOS 10+ with Xamarin Form 5.0, Android 10+ / iOS13+ with Maui

For Android, please make sure the target framework is Android 12

Nuget installation path

[Xamarin Forms](https://www.nuget.org/packages/Plugin.XF.AppInstallHelper/)
```
Install-Package Plugin.XF.AppInstallHelper
```
[Maui](https://www.nuget.org/packages/Plugin.Maui.AppInstallHelper/)
```
Install-Package Plugin.Maui.AppInstallHelper -Version 2.2.0-pre
```
### Android

**Configuration**
1. Insert below xml text into AndroidManifest.xml inside <application> tag, replace {packagename} to your app id
  
``` xml
<manifest .....
   <!--Use for Download the APK, Read and wrtie it-->
	<uses-permission android:name="android.permission.INTERNET" />
	<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
	<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
  <!--Use for package installation-->
	<uses-permission android:name="android.permission.INSTALL_PACKAGES" />
	<uses-permission android:name="android.permission.REQUEST_INSTALL_PACKAGES" />
  <application 
    ....
    <provider android:name="androidx.core.content.FileProvider" android:authorities="{packagename}.fileprovider" android:exported="false" android:grantUriPermissions="true">
      <meta-data android:name="android.support.FILE_PROVIDER_PATHS" android:resource="@xml/file_paths" />
    </provider>
    ....
  </application>
</manifest>
```

2. Create xml file named "file_paths.xml" in "Resources\xml" and Build action as "Android Resource"
``` xml
<?xml version="1.0" encoding="utf-8" ?>
<paths xmlns:android="http://schemas.android.com/apk/res/android">
  <external-path name="external_files" path="."/>
  <files-path name="files" path="."/>
  <internal-path name="internal_files" path="."/>
</paths>
```

3. In MainActivity.cs, initialze the library with your file provider authorities, replace {packagename} to your app id
```C#
// Xamarin Forms
protected override void OnCreate(Bundle savedInstanceState)

    base.OnCreate(savedInstanceState);
    .....
    Xamarin.Essentials.Platform.Init(this, savedInstanceState);
    global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
    Plugin.XF.AppInstallHelper.InstallationHelper.Init("{packagename}.fileprovider");
    ....
    LoadApplication(new App());
}
  
public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
{
	Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
	base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
}
   
protected override void OnNewIntent(Intent intent)
{
	Plugin.XF.AppInstallHelper.InstallationHelper.OnNewIntent(intent);
	base.OnNewIntent(intent);
}
```
```C#
//Maui
public class MainActivity : MauiAppCompatActivity
{
	public MainActivity()
	{
	    Plugin.Maui.AppInstallHelper.InstallationHelper.Init("{packagename}.fileprovider");
	}
	public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
	{
	    Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
	    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
	}
	protected override void OnNewIntent(Intent intent)
	{
	    Plugin.Maui.AppInstallHelper.InstallationHelper.OnNewIntent(intent);
	    base.OnNewIntent(intent);
	}
}

```

4. Add LaunchMode.SingleTop to your MainActivity.cs
```
Activity(Label= "{app name}",LaunchMode = LaunchMode.SingleTop.....................)
```

### iOS

iOS does not have any configuration. Api can be called directly.

### Calling the API

The input path can be various. please follow below guide. Return bool which means the installation is triggered or not. It does not means installtion success.

**Android(APK file)**

APK file must located in external storage, otherwise, parse error will occur.

Pass the full file path to API. Install mode as _OutOfAppStore_
```C#
        string apkPath = System.IO.Path.Combine(Plugin.XF.AppInstallHelper.InstallationHelper.GetPublicDownloadPath(), "APK.APK");
	await Plugin.XF.AppInstallHelper.InstallationHelper.InstallApp(apkPath, Plugin.XF.AppInstallHelper.InstallMode.OutOfAppStore);
```
**Android(Play store)**
Pass the package name to API. Install mode as _AppStore_

E.g. For the App chrome with play store url : https://play.google.com/store/apps/details?id=com.android.chrome

Api parameter is _com.android.chrome_
```C#
	await Plugin.XF.AppInstallHelper.InstallationHelper.InstallApp("com.android.chrome", Plugin.XF.AppInstallHelper.InstallMode.AppStore);
```

**iOS(Enterprise distribution or plist)**

Pass the full itms-service url into API. make sure the plist is under **https**. Simulator is unavailable.
```C#
   await Plugin.XF.AppInstallHelper.InstallationHelper.InstallApp("itms-services:///?action=download-manifest&url=https://{iOS_app}.plist", Plugin.XF.AppInstallHelper.InstallMode.OutOfAppStore);
```

**iOS(App store)**

Pass the Id the Api. E.g. App store url is https://itunes.apple.com/us/app/apple-store/id375380948?mt=8.

The API call should be
```C#
   await Plugin.XF.AppInstallHelper.InstallationHelper.InstallApp("375380948", Plugin.XF.AppInstallHelper.InstallMode.AppStore);
```


**Ask for storage permission**

```C#
	//iOS default return true
	//Android 6 or above, depends on user decision
	//Below Android 6, default true
	bool allowed = await Plugin.XF.AppInstallHelper.InstallationHelper.AskForRequiredPermission();
```
