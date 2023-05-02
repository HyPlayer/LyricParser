using System;

namespace LyricParser.Abstraction
{
    public interface ILyricLine
    {
        public string CurrentLyric { get; }
        public TimeSpan StartTime { get; }
    }
}
