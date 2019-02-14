using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.XF.AppInstallHelper.Abstractions
{
    public class AppInstallBase : IHelper
    {

        public string _fileProviderAuthorities = string.Empty;

        public virtual void Init(string fileProviderAuthorities)
        {
            _fileProviderAuthorities = fileProviderAuthorities;
        }

        public virtual async Task<bool> InstallApp(string path, InstallMode installMode)
        {
            throw new NotImplementedException();
        }
    }
}
