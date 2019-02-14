# Plugin.XF.AppInstallHelper
Xamarin Form helper for install application
https://www.nuget.org/packages/Plugin.XF.AppInstallHelper/

Nuget installation path
```
Install-Package Plugin.XF.AppInstallHelper
```

Android
1. Insert below xml text into AndroidManifest.xml inside <application> tag
  
``` xml
<application 
  ....
  <provider android:name="android.support.v4.content.FileProvider" android:authorities="{packagename}.fileprovider" android:exported="false" android:grantUriPermissions="true">
    <meta-data android:name="android.support.FILE_PROVIDER_PATHS" android:resource="@xml/file_paths" />
  </provider>
  ....
</application>
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
  Plugin.XF.AppInstallHelper.CrossInstallHelper.Current.Init("{packagename}.fileprovider");
```

4. Call the installation API
```C#
Plugin.XF.AppInstallHelper.CrossInstallHelper.Current.InstallApp(path, Plugin.XF.AppInstallHelper.Abstractions.InstallMode.OutOfAppStore);
```
