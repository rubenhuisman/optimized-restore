using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace optimized_restore.Settings
{
    public static class SettingsService
    {
        public static T GetConfigSectionAsType<T>(string configFileName)
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(GetExecutingDirectory())
               .AddJsonFile(configFileName, optional: false);

            return builder.Build().Get<T>();
        }

        public static string GetExecutingDirectory()
        {
            var location = new Uri(Assembly.GetExecutingAssembly().Location);
            return new FileInfo(location.AbsolutePath)?.Directory?.FullName;
        }
    }
}
