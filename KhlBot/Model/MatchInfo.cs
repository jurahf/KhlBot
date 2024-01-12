namespace KhlBot.Model
{
    public class MatchInfo
    {
        public Club Left { get; set; }

        public Club Right { get; set; }

        public DateTime DateAndTime { get; set; }

        public List<Translation> Translations { get; set; } = new List<Translation>();

        public string Result { get; set; }

        public override string ToString()
        {
            return $"{Left} - {Right}";
        }
    }
}
