using System;
using System.Text;

namespace LyricParser.Abstraction
{
    public sealed class LrcLyricsLine : ILyricLine
    {
        public string CurrentLyric { get; }
        public TimeSpan StartTime { get; }
        public string LyricWithoutPunc 
        { 
            get 
            {
                if (string.IsNullOrEmpty(_lyricWithoutPunc)) 
                {
                    var builder = new StringBuilder();
                    foreach(var curChar in CurrentLyric)
                    {
                        if (!char.IsPunctuation(curChar) && !char.IsWhiteSpace(curChar))
                        {
                            builder.Append(curChar);
                        }
                    }
                    _lyricWithoutPunc = builder.ToString();
                }
                return _lyricWithoutPunc;
            } 
        }
        private string _lyricWithoutPunc;
        public TimeSpan? PossibleStartTime { get; set; }
        public LrcLyricsLine(string currentLyric, TimeSpan startTime)
        {
            CurrentLyric = currentLyric;
            StartTime = startTime;
        }
    }
}
