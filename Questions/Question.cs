namespace optimized_restore.Questions
{
    public class Question
    {
        public QuestionKey Key { get; set; }
        public Func<Dictionary<QuestionKey, string>, string> Title { get; set; }
        public Func<Dictionary<QuestionKey, string>, IEnumerable<string>> OptionsGetter { get; set; }
    }
}