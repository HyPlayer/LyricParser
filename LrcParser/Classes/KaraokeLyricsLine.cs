using System;
using System.Collections.Generic;
using System.Linq;

namespace LrcParser.Classes
{
    public class KaraokeLyricsLine : ILyricLine
    {
        public IList<KaraokeWordInfo> WordInfos { get; }

        public string CurrentLyric { get; }

        public int StartTime { get; }

        public KaraokeLyricsLine(IList<KaraokeWordInfo> wordInfos, int startTime)
        {
            WordInfos = wordInfos;
            StartTime = startTime;
            var currentLyric = string.Concat(wordInfos.Select(t => t.CurrentWords).ToList());
            CurrentLyric = currentLyric;
        }
    }
}
