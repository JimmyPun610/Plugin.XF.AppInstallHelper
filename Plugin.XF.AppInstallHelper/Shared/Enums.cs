using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.XF.AppInstallHelper
{
    public enum InstallMode
    {
        /// <summary>
        /// Android : Downloaded Apk file
        /// iOS : Enterprise distribution
        /// </summary>
        OutOfAppStore = 0,
        /// <summary>
        /// Android : Google play store
        /// iOS : Apple App store
        /// </summary>
        AppStore = 1
    }
}
