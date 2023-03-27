using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LrcParser.Classes
{
    public class KaraokeLyricsLine:LyricLineBase
    {
        public IList<KaraokeWordInfo> WordInfos { get; }
        public KaraokeLyricsLine(IList<KaraokeWordInfo> wordInfos,TimeSpan startTime)
        {
            WordInfos = wordInfos;
            StartTime = startTime;
            var currentLyric = string.Concat(wordInfos.Select(t => t.CurrentWords).ToList());
            CurrentLyric = currentLyric;
        }
    }
}
