using System;
using System.Diagnostics;

namespace LyricParser.Abstraction
{
    [DebuggerDisplay("Word = {CurrentWords}, Transliteration = {Transliteration}")]
    public sealed class KaraokeWordInfo
    {
        public string CurrentWords;
        public TimeSpan StartTime;
        public TimeSpan Duration;
        public string? Transliteration;
        public KaraokeWordInfo(string currentWords, TimeSpan startTime, TimeSpan duration)
        {
            CurrentWords = currentWords;
            StartTime = startTime;
            Duration = duration;
        }

        
    }
}
