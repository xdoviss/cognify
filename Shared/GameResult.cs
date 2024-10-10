namespace cognify.Shared
{
    public class GameResult
    {
        public double ElapsedTime { get; set; }
        public int MistakesCount { get; set; }

        public GameResult(double elapsedTime, int mistakesCount)
        {
            ElapsedTime = elapsedTime;
            MistakesCount = mistakesCount;
        }   
    }
    
}