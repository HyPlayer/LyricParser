using System.Collections.Generic;
using System.Linq;

namespace LyricParser.Classes
{
    public class KaraokeLyricsLine : ILyricLine
    {
        public IReadOnlyList<KaraokeWordInfo> WordInfos { get; }

        public string CurrentLyric => string.Concat(WordInfos.Select(t => t.CurrentWords).ToArray());

        public int StartTime { get; }

        public int Duration { get; }

        public KaraokeLyricsLine(IEnumerable<KaraokeWordInfo> wordInfos, int startTime, int duration)
        {
            WordInfos = wordInfos.ToList();
            StartTime = startTime;
            Duration = duration;
        }
    }
}
