using System;
using System.IO;

namespace uTikDownloadHelper
{
    class ExtractedResources
    {
        private static string extractResources()
        {
            var resourcesDirectory = Path.Combine(Common.SettingsPath, "app");
            if (!Directory.Exists(resourcesDirectory))
            {
                Directory.CreateDirectory(resourcesDirectory);
            }
            try
            {
                File.WriteAllBytes(Path.Combine(resourcesDirectory, "wget.exe"), Properties.Resources.wget);
            } catch { }
            try
            {
                File.WriteAllBytes(Path.Combine(resourcesDirectory, "vcruntime140.dll"), Properties.Resources.vcruntime140);
            } catch { }
            return resourcesDirectory;
        }
        public string extractedResourcesPath;
        public string wget;
        public string vcruntime140;
        public ExtractedResources()
        {
            extractedResourcesPath = extractResources();
            wget = Path.Combine(extractedResourcesPath, "wget.exe");
            vcruntime140 = Path.Combine(extractedResourcesPath, "vcruntime140.dll");
        }
    }
}
