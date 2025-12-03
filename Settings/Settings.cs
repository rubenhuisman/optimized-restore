namespace optimized_restore.Settings
{
    public class RestoreConfiguration
    {
        public string Name { get; set; }
        public string DatabaseNamePrefix { get; set; }
        public List<string> QueriesAfterRestore { get; set; }
    }

    public class SettingsObject
    {
        public string SqlServerHost { get; set; }
        public string SqlPackagePath { get; set; }
        public List<RestoreConfiguration> RestoreConfigurations { get; set; }
    }
}