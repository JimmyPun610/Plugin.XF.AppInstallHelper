using Plugin.XF.AppInstallHelper.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plugin.XF.AppInstallHelper
{
    public class XFAppInstallImplementation : AppInstallBase
    {
        [Obsolete("iOS do not need to initialize")]
        public override void Init(string fileProviderAuthorities)
        {
            
        }

        public override void InstallApp(string path, InstallMode installMode)
        {
            
        }
    }
}
