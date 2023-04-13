namespace LyricParser.Abstraction
{
    public interface ILyricLine
    {
        public string CurrentLyric { get; }
        public int StartTime { get; }
    }
}
