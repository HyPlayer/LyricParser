using System;

namespace LyricParser.Abstraction
{
    public interface ILyricLine
    {
        string CurrentLyric { get; }
        string LyricWithoutPunc { get; }
        TimeSpan StartTime { get; }
        TimeSpan? PossibleStartTime { get; set; }
    }
}
