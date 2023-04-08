namespace LrcParser.Classes
{
    public interface ILyricLine
    {
        public string CurrentLyric { get; }
        public int StartTime { get; }
    }
}
