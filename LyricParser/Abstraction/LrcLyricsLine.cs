using System;

namespace LyricParser.Abstraction
{
    public class LrcLyricsLine : ILyricLine
    {
        public string CurrentLyric { get; }
        public TimeSpan StartTime { get; }
        public LrcLyricsLine(string currentLyric, TimeSpan startTime)
        {
            CurrentLyric = currentLyric;
            StartTime = startTime;
        }
    }
}
