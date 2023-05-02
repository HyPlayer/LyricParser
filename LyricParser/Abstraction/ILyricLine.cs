using System;

namespace LyricParser.Abstraction
{
    public interface ILyricLine
    {
        string CurrentLyric { get; }
        TimeSpan StartTime { get; }
    }
}
