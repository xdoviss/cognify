namespace cognify.Shared
{
    public class GameResult
    {
        
        public double? TypeRacerWPM { get; set; }
        public int? WordRecallHS { get; set; }
        public int? BoardRecallHS { get; set; }
        //public string UserName { get; set; }
        //Could have the User's name later, when we have login

        // Constructor for setting scores for a specific game
        public GameResult(double? typeRacerWPM = null, int? wordRecallHS = null, int? boardRecallHS = null)
        {
            TypeRacerWPM = typeRacerWPM;
            WordRecallHS = wordRecallHS;
            BoardRecallHS = boardRecallHS;
        }
    }
}
