using LrcParser.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LrcParser.Implementation
{
    public static class LyricParser
    {
        private static readonly Regex LrcTimestampRegex = new Regex(@"\[(?'minutes'\d+):(?'seconds'\d+(\.\d+)?)\]", RegexOptions.Compiled);
        private static readonly Regex LrcOffsetRegex = new Regex(@"\[offset:(?'content'.*)\]", RegexOptions.Compiled);
        private static readonly Regex KaraokeWordRegex = new Regex(@"\(([0-9]*),([0-9]*),0\)", RegexOptions.Compiled);
        private static readonly Regex KaraokeWordInfosRegex = new Regex(@"\([0-9]*,[0-9]*,0\)", RegexOptions.Compiled);
        public static LyricsData ParseKaraokeLyrics(string karaokeLyrics)
        {
            var rawLyricList = karaokeLyrics.Split('\n').ToList();
            var resultLyricList = new List<ILyricLine>();
            foreach (var lyricLine in rawLyricList)
            {
                if (!lyricLine.StartsWith('[')) continue;
                var rawTimestamp = lyricLine.Substring(1, lyricLine.IndexOf(',', 1) - 1);
                var timestamp = double.Parse(rawTimestamp);
                var rawLyricData = lyricLine.Substring(lyricLine.IndexOf(']') + 1);
                var lyricWords = KaraokeWordRegex.Split(rawLyricData).ToList().Skip(1).ToList();
                var lyricWordInfos = KaraokeWordInfosRegex.Matches(rawLyricData).ToList();
                var wordInfoList = new List<KaraokeWordInfo>();
                for (var index = 0; index < lyricWordInfos.Count; index++)
                {
                    if (lyricWordInfos[index].Length <= 0) continue;
                    var startTime = double.Parse(lyricWordInfos[index].Groups[1].Value);
                    var duration = double.Parse(lyricWordInfos[index].Groups[2].Value);
                    var wordInfo = new KaraokeWordInfo(string.Concat(lyricWords[index]), TimeSpan.FromMilliseconds(startTime), TimeSpan.FromMilliseconds(duration));
                    wordInfoList.Add(wordInfo);
                }
                var resultLyricLine = new KaraokeLyricsLine(wordInfoList, TimeSpan.FromMilliseconds(timestamp));
                resultLyricList.Add(resultLyricLine);
            }
            return new LyricsData(resultLyricList);
        }
        public static LyricsData ParseLrcLyrics(string lrcLyrics)
        {
            if (lrcLyrics is null) throw new ArgumentNullException("lrcLyrics");
            var pureLyricList = lrcLyrics.Replace("\r\n", "\n").Replace("\r", "\n").Split("\n").ToList();
            var lyricOffset = TimeSpan.Zero;
            var rawLyricsList = new List<ILyricLine>();
            var offsetString = pureLyricList.Where(t => t.StartsWith("[offset:")).ToList();
            if (offsetString.Count > 0)
            {
                if (offsetString.Count > 1) throw new FormatException("Multiple offsets were found");
                else
                {
                    var matchResult = LrcOffsetRegex.Match(offsetString.First());
                    lyricOffset = TimeSpan.FromMilliseconds(double.Parse(matchResult.Groups["content"].Value));
                }
            }
            foreach (var pureLyric in pureLyricList)
            {
                var lyricString = pureLyric.Substring(pureLyric.LastIndexOf(']') + 1);
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
