using System;

namespace LrcParser.Classes
{
    public class KaraokeWordInfo
    {
        public string CurrentWords;
        public TimeSpan StartTime;
        public TimeSpan Duration;
        public KaraokeWordInfo(string currentWords, TimeSpan startTime, TimeSpan duration)
        {
            CurrentWords = currentWords;
            StartTime = startTime;
            Duration = duration;
        }
    }
}
