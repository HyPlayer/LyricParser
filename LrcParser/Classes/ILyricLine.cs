using System;

namespace LrcParser.Classes
{
    public interface ILyricLine
    {
        public string CurrentLyric { get; }
        public TimeSpan StartTime { get; }
    }
}
