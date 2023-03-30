using LrcParser.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LrcParser.Implementation
{
    public static class LyricParser
    {
        private static readonly Regex LrcTimestampRegex = new Regex(@"\[(?'minutes'\d+):(?'seconds'\d+(\.\d+)?)\]");
        private static readonly Regex LrcOffsetRegex = new Regex(@"\[offset:(?'content'.*)\]");
        public static LyricsData ParseKaraokeLyrics(string karaokeLyrics)
        {
            throw new NotImplementedException();
        }
        public static LyricsData ParseLrcLyrics(string lrcLyrics)
        {
            if (lrcLyrics is null) throw new ArgumentNullException("lrcLyrics");
            var pureLyricArray = lrcLyrics.Replace("\r\n", "\n").Replace("\r", "\n").Split("\n");
            var lyricOffset = new TimeSpan();
            var rawLyricsList = new List<ILyricLine>();
            var offsetString = pureLyricArray.Where(t => t.StartsWith("[offset:")).ToList();
            if (offsetString.Count > 0)
            {
                if (offsetString.Count > 1) throw new FormatException("Multiple offsets were found");
                else
                {
                    var matchResult = LrcOffsetRegex.Match(offsetString.First());
                    lyricOffset = TimeSpan.FromMilliseconds(double.Parse(matchResult.Groups["content"].Value));
                }
            }
            foreach (var pureLyric in pureLyricArray)
            {
                var lastIndex = pureLyric.LastIndexOf(']');
                var lyricString = pureLyric.Substring(lastIndex +1);
                var matchResult = LrcTimestampRegex.Matches(pureLyric);
                foreach (var lyricTimeResult in matchResult)
                {
                    var minutes = int.Parse(((Match)lyricTimeResult).Groups["minutes"].Value);
                    var seconds = double.Parse(((Match)lyricTimeResult).Groups["seconds"].Value);
                    var timestamp = TimeSpan.FromSeconds(minutes * 60 + seconds).Add(lyricOffset);
                    rawLyricsList.Add(new LrcLyricsLine(lyricString, timestamp));
                }
            }
            var resultLyricsList = rawLyricsList.OrderBy(t => t.StartTime).ToList();
            return new LyricsData(resultLyricsList);
        }
    }
}
