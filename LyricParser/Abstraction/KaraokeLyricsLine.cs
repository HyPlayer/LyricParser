using System;
using System.Collections.Generic;
using System.Linq;

namespace LyricParser.Abstraction
{
    public sealed class KaraokeLyricsLine : ILyricLine
    {
        public IReadOnlyList<KaraokeWordInfo> WordInfos { get; }

        public string CurrentLyric
        {
            get
            {
                if (string.IsNullOrEmpty(_currentLyric))
                {
                    _currentLyric = string.Concat(WordInfos.Select(t => t.CurrentWords).ToArray());
                }
                return _currentLyric;
            }
        }
        private string _currentLyric = string.Empty;

        public TimeSpan StartTime { get; }

        public TimeSpan Duration { get; }

        public KaraokeLyricsLine(IEnumerable<KaraokeWordInfo> wordInfos, TimeSpan startTime, TimeSpan duration)
        {
            WordInfos = wordInfos.ToList();
            StartTime = startTime;
            Duration = duration;
        }
    }
}
