using optimized_restore.ConsoleWrapper;
using optimized_restore.Questions;
using optimized_restore.Settings;

namespace optimized_restore
{
    public class Program
    {
        public static void Main()
        {
            var config = SettingsService.GetConfigSectionAsType<List<RestoreConfiguration>>("appsettings.json", "RestoreConfigurations");

            var context = QuestionService.GetAnswersToQuestions(config);

            var backupLocation = context[QuestionKey.BackupLocation];
            var configurationToUse = context[QuestionKey.ConfigurationToUse];
            var executeQueriesAfterRestore = context[QuestionKey.ExecuteQueriesAfterRestore];

            ConsoleService.DisplayContextAsTable(context);

            Console.ReadKey();
        }
    }
}
