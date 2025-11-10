using optimized_restore.Questions;
using Spectre.Console;

namespace optimized_restore.ConsoleWrapper
{
    public static class ConsoleService
    {
        public static string GetAnswerWithOptions(Question question, Dictionary<QuestionKey, string> answers, IEnumerable<string> options)
        {
            var selectionPrompt = new SelectionPrompt<string>()
                    .Title(question.Title(answers))
                    .PageSize(Math.Max(options.Count(), 3))
                    .AddChoices(options);

            return AnsiConsole.Prompt(selectionPrompt);
        }

        public static string GetTextPrompt(Question question, Dictionary<QuestionKey, string> answers)
        {
            var textPrompt = new TextPrompt<string>(question.Title(answers));
            return AnsiConsole.Prompt(textPrompt);
        }

        public static T DoBackgroundTaskWithStatusMessage<T>(string message, Func<T> action) where T : class, new()
        {
            var result = new T();

            AnsiConsole.Status().Start(message, _ => result = action());

            return result;
        }

        internal static void DisplayContextAsTable(Dictionary<QuestionKey, string> context)
        {
            var columns = context.Keys.Select(key => new TableColumn(key.ToString()).Centered()).ToList();
            var rows = context.Values.ToList();

            var table = new Table()
                .AddColumn(new TableColumn("Setting"))
                .AddColumn(new TableColumn("Value"));

            foreach (var keyValue in context)
                table.AddRow(keyValue.Key.ToString(), keyValue.Value);

            AnsiConsole.Write(table);
        }
    }
}
