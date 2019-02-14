# Plugin.XF.AppInstallHelper
Xamarin Form helper for install application

Support Android 4.4+ / iOS 10+

https://www.nuget.org/packages/Plugin.XF.AppInstallHelper/

Nuget installation path
```
Install-Package Plugin.XF.AppInstallHelper
```

Android

Configuration
1. Insert below xml text into AndroidManifest.xml inside <application> tag
  
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
    <provider android:name="android.support.v4.content.FileProvider" android:authorities="{packagename}.fileprovider" android:exported="false" android:grantUriPermissions="true">
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

3. In MainActivity.cs, initialze the library with your file provider authorities
```C#

	global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
	....
	Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);
	Xamarin.Essentials.Platform.Init(this, savedInstanceState);
    Plugin.XF.AppInstallHelper.CrossInstallHelper.Current.Init("{packagename}.fileprovider");
	....
	LoadApplication(new App());
  
  
   public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
   {
		PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
   }
```

4. Call the installation API
```C#
	Plugin.XF.AppInstallHelper.CrossInstallHelper.Current.InstallApp(path, installMode);
```



iOS
iOS does not have any configuration. Api can be called directly.
```C#
	Plugin.XF.AppInstallHelper.CrossInstallHelper.Current.InstallApp(path, installMode);
```