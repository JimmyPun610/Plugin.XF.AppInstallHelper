using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.Maui.AppInstallHelper
{
    public class AppVersion
    {
        /// <summary>
        /// Matching with Xamarin.Essential VersionTracking.CurrentBuild
        /// </summary>
        public int VersionCode { get; set; }
        public string VersionName { get; set; }
        /// <summary>
        /// Html / String whatever. Depends on how will you display
        /// </summary>
        public string ReleaseNotes { get; set; }
        public bool ForceUpdate { get; set; }
        public string AndroidPath { get; set; }
        public string iOSPath { get; set; }
    }
}
