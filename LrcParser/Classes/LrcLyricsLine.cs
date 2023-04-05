using System;

namespace LrcParser.Classes
{
    public class LrcLyricsLine : ILyricLine
    {
        public string CurrentLyric { get; }
        public int StartTime { get; }
        public LrcLyricsLine(string currentLyric, int startTime)
        {
            CurrentLyric = currentLyric;
            StartTime = startTime;
        }
    }
}
