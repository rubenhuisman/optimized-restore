using optimized_restore.ConsoleWrapper;
using optimized_restore.Settings;

namespace optimized_restore.Questions
{
    public class QuestionService
    {
        public static Dictionary<QuestionKey, string> GetAnswersToQuestions(List<RestoreConfiguration> settings)
        {
            var context = new Dictionary<QuestionKey, string>();

            var questions = new List<Question>
            {
                new Question
                {
                    Key = QuestionKey.BackupLocation,
                    Title = (_) => "Give location of [green]backup file[/] (.bacpac or .bak)",
                    OptionsGetter = (_) => []
                },
                new Question
                {
                    Key = QuestionKey.ConfigurationToUse,
                    Title = (context) => $"Select which [green]optimized-restore[/] configuration you want to use",
                    OptionsGetter = (context) => settings.Select(x => x.Name)
                },
                new Question
                {
                    Key = QuestionKey.ExecuteQueriesAfterRestore,
                    Title = (_) => "Execute [green]queries[/] after restore?",
                    OptionsGetter = (_) => ["Yes", "No"]
                }
            };

            foreach (var question in questions)
            {
                var questionOptions = question.OptionsGetter(context);

                if (questionOptions.Any())
                    context[question.Key] = ConsoleService.GetAnswerWithOptions(question, context, questionOptions);
                else
                    context[question.Key] = ConsoleService.GetTextPrompt(question, context);
            }

            return context;
        }
    }
}
