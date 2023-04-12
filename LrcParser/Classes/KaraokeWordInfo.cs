namespace LyricParser.Classes
{
    public class KaraokeWordInfo
    {
        public string CurrentWords;
        public int StartTime;
        public int Duration;
        public KaraokeWordInfo(string currentWords, int startTime, int duration)
        {
            CurrentWords = currentWords;
            StartTime = startTime;
            Duration = duration;
        }
    }
}
