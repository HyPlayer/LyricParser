using LrcParser.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LrcParser.Implementation
{
    public static class KaraokeParser
    {
        public static List<ILyricLine> ParseKaraoke(ReadOnlySpan<char> input)
        {
            List<ILyricLine> lines = new();
            List<KaraokeWordInfo> karaokeWordInfos = new List<KaraokeWordInfo>();
            var timespanStringBuilder = new StringBuilder();
            var lyricStringBuilder = new StringBuilder();
            var lyricTimespan = 0;
            var lyricDuration = 0;
            var wordTimespan = 0;
            var wordDuration = 0;
            var state = CurrentState.None;
            for (var i = 0; i < input.Length; i++)
            {
                if (i != input.Length)
                {
                    ref readonly var curChar = ref input[i];

                    if (curChar == '\n' || curChar == '\r' || i + 1 == input.Length)
                    {
                        if (i + 1 < input.Length)
                        {
                            if ((input[i + 1] == '\n' || input[i + 1] == '\r')) i++;
                        }
                        state = CurrentState.None;
                        karaokeWordInfos.Add(new KaraokeWordInfo(lyricStringBuilder.ToString(), wordTimespan, wordDuration));
                        lines.Add(new KaraokeLyricsLine(karaokeWordInfos, lyricTimespan, lyricDuration));
                        karaokeWordInfos.Clear();
                        lyricStringBuilder.Clear();
                        continue;
                    }
                    switch (curChar)
                    {
                        case '[':
                            if (state == CurrentState.Lyric)
                            {
                                if (i + 1 < input.Length)
                                {
                                    if (!char.IsNumber(input[i + 1])) break;
                                }
                            }
                            state = CurrentState.PossiblyLyricTimestamp;
                            continue;
                        case ',':
                            if (state == CurrentState.Lyric)
                            {
                                if (i + 1 < input.Length)
                                {
                                    if (!char.IsNumber(input[i + 1])) break;
                                }
                            }
                            if (state == CurrentState.LyricTimestamp)
                            {
                                state = CurrentState.PossiblyLyricDuration;
                                lyricTimespan = int.Parse(timespanStringBuilder.ToString());
                                timespanStringBuilder.Clear();
                            }
                            else if (state == CurrentState.WordTimestamp)
                            {
                                state = CurrentState.PossiblyWordDuration;
                                wordTimespan = int.Parse(timespanStringBuilder.ToString());
                                timespanStringBuilder.Clear();
                            }
                            else
                            {
                                state = CurrentState.WordUnknownItem;
                                wordDuration = int.Parse(timespanStringBuilder.ToString());
                                timespanStringBuilder.Clear();
                            }
                            continue;
                        case ']':
                            if (state == CurrentState.Lyric)
                            {
                                if (i + 1 < input.Length)
                                {
                                    if (!char.IsNumber(input[i + 1])) break;
                                }
                            }
                            state = CurrentState.None;
                            lyricDuration = int.Parse(timespanStringBuilder.ToString());
                            timespanStringBuilder.Clear();
                            continue;
                        case '(':
                            if (state == CurrentState.Lyric)
                            {
                                if (i + 1 < input.Length)
                                {
                                    if (!char.IsNumber(input[i + 1])) break;
                                }
                                karaokeWordInfos.Add(new KaraokeWordInfo(lyricStringBuilder.ToString(), wordTimespan, wordDuration));
                                lyricStringBuilder.Clear();
                            }
                            state = CurrentState.PossiblyWordTimestamp;
                            continue;
                        case ')':
                            if (state == CurrentState.Lyric)
                            {
                                if (i + 1 < input.Length)
                                {
                                    if (!char.IsNumber(input[i + 1])) break;
                                }
                            }
                            state = CurrentState.Lyric;
                            continue;
                    }
                    switch (state)
                    {
                        case CurrentState.PossiblyLyricTimestamp:
                            if (char.IsNumber(curChar)) state = CurrentState.LyricTimestamp;
                            timespanStringBuilder.Append(curChar);
                            break;
                        case CurrentState.LyricTimestamp:
                            timespanStringBuilder.Append(curChar);
                            break;
                        case CurrentState.PossiblyWordTimestamp:
                            if (char.IsNumber(curChar)) state = CurrentState.WordTimestamp;
                            timespanStringBuilder.Append(curChar);
                            break;
                        case CurrentState.WordTimestamp:
                            timespanStringBuilder.Append(curChar);
                            break;
                        case CurrentState.PossiblyLyricDuration:
                            if (char.IsNumber(curChar)) state = CurrentState.LyricDuration;
                            timespanStringBuilder.Append(curChar);
                            break;
                        case CurrentState.LyricDuration:
                            timespanStringBuilder.Append(curChar);
                            break;
                        case CurrentState.PossiblyWordDuration:
                            if (char.IsNumber(curChar)) state = CurrentState.WordDuration;
                            timespanStringBuilder.Append(curChar);
                            break;
                        case CurrentState.WordDuration:
                            timespanStringBuilder.Append(curChar);
                            break;
                        case CurrentState.Lyric:
                            lyricStringBuilder.Append(curChar);
                            break;

                    }
                }
            }
            return lines;
        }

        private enum CurrentState
        {
            None,
            LyricTimestamp,
            WordTimestamp,
            LyricDuration,
            WordDuration,
            WordUnknownItem,
            PossiblyLyricDuration,
            PossiblyWordDuration,
            PossiblyLyricTimestamp,
            PossiblyWordTimestamp,
            Lyric
        }
    }
}
