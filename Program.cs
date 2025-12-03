using optimized_restore.Questions;
using optimized_restore.Settings;

namespace optimized_restore
{
    public partial class Program
    {
        public static void Main()
        {
            var fullConfiguration = SettingsService.GetConfigSectionAsType<SettingsObject>("appsettings.json");

            var context = QuestionService.GetAnswersToQuestions(fullConfiguration.RestoreConfigurations);

            DatabaseService.HandleRestore(fullConfiguration, context);

            Console.ReadKey();
        }
    }
}
