using System;

namespace LyricParser.Abstraction
{
    public sealed class LrcLyricsLine : ILyricLine
    {
        public string CurrentLyric { get; }
        public TimeSpan StartTime { get; }
        public string LyricWithoutPunc { get; }
        public TimeSpan? PossibleStartTime { get; set; }
        public LrcLyricsLine(string currentLyric, string lyricWithoutPunc, TimeSpan startTime)
        {
            CurrentLyric = currentLyric;
            LyricWithoutPunc = lyricWithoutPunc;
            StartTime = startTime;
        }
    }
}
