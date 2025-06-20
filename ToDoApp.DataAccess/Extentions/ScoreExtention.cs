namespace ToDoApp.Application.Extentions
{
    public static class ScoreExtention
    {
        public static double GetValidScore(this double? score)
        {
            return GetValidScore(score, 0);
        }

        public static double GetValidScore(this double? score, double baseScore)
        {
            return score.HasValue && score >= 0 && score <= 10 ? score.Value : baseScore;
        }
    }
}
