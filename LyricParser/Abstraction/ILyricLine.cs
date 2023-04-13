namespace LyricParser.Abstraction
{
    public interface ILyricLine
    {
        string CurrentLyric { get; }
        int StartTime { get; }
    }
}
