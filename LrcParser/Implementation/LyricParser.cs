using LrcParser.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LrcParser.Implementation
{
    public static class LyricParser
    {
        private static readonly Regex LrcTimestampRegex =
            new Regex(@"\[(?'minutes'\d+):(?'seconds'\d+(\.\d+)?)\]", RegexOptions.Compiled);

        private static readonly Regex LrcOffsetRegex = new Regex(@"\[offset:(?'content'.*)\]", RegexOptions.Compiled);
        private static readonly Regex KaraokeWordRegex = new Regex(@"\([0-9]*,[0-9]*,0\)", RegexOptions.Compiled);

        private static readonly Regex KaraokeWordInfosRegex =
            new Regex(@"\(([0-9]*),([0-9]*),0\)", RegexOptions.Compiled);

        public static LyricsData ParseKaraokeLyrics(string karaokeLyrics)
        {
            var rawLyricList = karaokeLyrics.Split('\n').ToList();
            var resultLyricList = new List<ILyricLine>();
            foreach (var lyricLine in rawLyricList)
            {
                var lyricLineSpan = lyricLine.AsSpan();
                if (lyricLineSpan[0] != '[') continue;
                var rawTimestamp = lyricLineSpan.Slice(1, lyricLineSpan.IndexOf(',') - 1);
                var timestamp = int.Parse(rawTimestamp);
                var rawLyricData = lyricLineSpan.Slice(lyricLine.IndexOf(']') + 1);
                var lyricWords = KaraokeWordRegex.Split(rawLyricData.ToString()).ToList().Skip(1).ToList();
                var lyricWordInfos = KaraokeWordInfosRegex.Matches(rawLyricData.ToString()).ToList();
                var wordInfoList = new List<KaraokeWordInfo>();
                for (var index = 0; index < lyricWordInfos.Count; index++)
                {
                    if (lyricWordInfos[index].Length <= 0) continue;
                    var startTime = double.Parse(lyricWordInfos[index].Groups[1].Value);
                    var duration = double.Parse(lyricWordInfos[index].Groups[2].Value);
                    var wordInfo = new KaraokeWordInfo(string.Concat(lyricWords[index]),
                        TimeSpan.FromMilliseconds(startTime), TimeSpan.FromMilliseconds(duration));
                    wordInfoList.Add(wordInfo);
                }

                var resultLyricLine = new KaraokeLyricsLine(wordInfoList, timestamp);
                resultLyricList.Add(resultLyricLine);
            }

            return new LyricsData(resultLyricList);
        }

        public static LyricsData ParseLrcLyrics(string lrcLyrics)
        {
            if (lrcLyrics is null) throw new ArgumentNullException("lrcLyrics");
            var pureLyricList = lrcLyrics.Replace("\r\n", "\n").Replace("\r", "\n").Split("\n").ToList();
            var lyricOffset = 0.0;
            var rawLyricsList = new List<ILyricLine>();
            var offsetString = pureLyricList.Where(t => t.StartsWith("[offset:")).ToList();
            if (offsetString.Count > 0)
            {
                if (offsetString.Count > 1) throw new FormatException("Multiple offsets were found");
                else
                {
                    var matchResult = LrcOffsetRegex.Match(offsetString.First());
                    lyricOffset = double.Parse(matchResult.Groups["content"].Value);
                }
            }

            foreach (var pureLyric in pureLyricList)
            {
                var pureLyricSpan = pureLyric.AsSpan();
                var lyricString = pureLyricSpan.Slice(pureLyricSpan.LastIndexOf(']') + 1);
                var matchResult = LrcTimestampRegex.Matches(pureLyric);
                foreach (var lyricTimeResult in matchResult)
                {
                    var minutes = int.Parse(((Match)lyricTimeResult).Groups["minutes"].Value);
                    var seconds = double.Parse(((Match)lyricTimeResult).Groups["seconds"].Value);
                    rawLyricsList.Add(new LrcLyricsLine(lyricString.ToString(), (int)(minutes * 60_000 + seconds * 1_000 + lyricOffset)));
                }
            }

            var resultLyricsList = rawLyricsList.OrderBy(t => t.StartTime).ToList();
            return new LyricsData(resultLyricsList);
        }
    }
}