using System.Collections.Generic;
using System.Linq;

namespace LrcParser.Classes
{
    public class KaraokeLyricsLine : ILyricLine
    {
        public IReadOnlyList<KaraokeWordInfo> WordInfos { get; }

        public string CurrentLyric { get; }

        public int StartTime { get; }

        public int Duration { get; }

        public KaraokeLyricsLine(IList<KaraokeWordInfo> wordInfos, int startTime, int duration)
        {
            WordInfos = wordInfos.ToList();
            StartTime = startTime;
            Duration = duration;
            var currentLyric = string.Concat(wordInfos.Select(t => t.CurrentWords).ToList());
            CurrentLyric = currentLyric;
        }
    }
}
