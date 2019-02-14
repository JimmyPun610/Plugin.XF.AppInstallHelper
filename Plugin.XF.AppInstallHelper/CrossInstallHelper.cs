using Plugin.XF.AppInstallHelper.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plugin.XF.AppInstallHelper
{
    public class CrossInstallHelper
    {
        static Lazy<IHelper> Implementation = new Lazy<IHelper>(() => CreateHelper(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Current settings to use
        /// </summary>
        public static IHelper Current
        {
            get
            {
                var ret = Implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }

        public static IHelper CreateHelper()
        {
#if PORTABLE
            return null;
#else
        return new XFAppInstallImplementation();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
        }
    }
}
  
