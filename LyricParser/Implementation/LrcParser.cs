using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using LyricParser.Abstraction;

namespace LyricParser.Implementation
{
    public static class LrcParser
    {
        public static List<ILyricLine> ParseLrc(ReadOnlySpan<char> input)
        {
            List<ILyricLine> lines = new List<ILyricLine>();
            var curStateStartPosition = 0;
            var timeCalculationCache = 0;
            var curTimestamps = ArrayPool<int>.Shared.Rent(16); // Max Count 
            int curTimestamp = 0;
            int currentTimestampPosition = 0;
            var reachesEnd = false;
            var lastCharacterIsLineBreak = false;
            var state = CurrentState.None;
            var timeSpanType = TimeSpanType.None;
            for (var i = 0; i < input.Length; i++)
            {
                ref readonly var curChar = ref input[i];
                // 剥离开, 方便 分支预测
                if (state == CurrentState.Lyric)
                {
                    if (curChar != '\n' && curChar != '\r' && i+ 1 < input.Length)
                    {
                        continue;
                    }
                    else
                    {
                        if (i + 1 < input.Length)
                        {
                            for (int j = 0; j < curTimestamps.Length; j++)
                            {
                                if (curTimestamps[j] == -1) break;
                                lines.Add(new LrcLyricsLine(
                                    input.Slice(curStateStartPosition + 1, i - curStateStartPosition - 1).ToString(),
                                    curTimestamps[j]));
                            }
                            if (input[i + 1] == '\n' || input[i + 1] == '\r') i++;
                            currentTimestampPosition = 0;
                            // Change State
                            state = CurrentState.None;
                            continue;
                        }
                        if(i+1==input.Length)
                        {
                            reachesEnd = true;
                            if (curChar == '\r' || curChar == '\n') 
                            {
                                lastCharacterIsLineBreak = true;
                            }
                        }
                    }
                }
                if (reachesEnd)
                {
                    for (int j = 0; j < curTimestamps.Length; j++)
                    {
                        if (curTimestamps[j] == -1) break;
                        lines.Add(new LrcLyricsLine(
                            input.Slice(curStateStartPosition + 1, i - curStateStartPosition - (lastCharacterIsLineBreak ? 1 : 0)).ToString(),
                            curTimestamps[j])); ;
                    }
                    continue;
                }
                switch (state)
                {
                    case CurrentState.PossiblyLyric:
                        if (curChar == '[')
                        {
                            state = CurrentState.AwaitingStateLyric;
                        }
                        else
                        {
                            i -= 1;
                            state = CurrentState.Lyric;
                        }

                        break;
                    case CurrentState.None:
                        Array.Fill(curTimestamps, -1);
                        if (curChar == '[')
                        {
                            state = CurrentState.AwaitingState;
                        }

                        break;
                    case CurrentState.AwaitingState:
                        if ('0' <= curChar && curChar <= '9')
                        {
                            // Time
                            state = CurrentState.Timestamp;
                            timeSpanType = TimeSpanType.Minutes;
                            curStateStartPosition = i;
                            curTimestamp = 0;
                            i--;
                        }
                        else
                        {
                            state = CurrentState.Attribute;
                            curStateStartPosition = i;
                        }

                        break;
                    case CurrentState.AwaitingStateLyric:
                        if ('0' <= curChar && curChar <= '9')
                        {
                            // Time
                            state = CurrentState.Timestamp;
                            timeSpanType = TimeSpanType.Minutes;
                            curStateStartPosition = i;
                            curTimestamp = 0;
                            i--;
                        }
                        else
                        {
                            state = CurrentState.Lyric;
                            curStateStartPosition = i - 2;
                        }

                        break;
                    case CurrentState.Attribute:
                        if (curChar == ':')
                        {
                            //var attr = input.Slice(curStateStartPosition, i - curStateStartPosition);
                            state = CurrentState.AttributeContent;
                        }

                        break;
                    case CurrentState.AttributeContent:
                        if (curChar == ']') state = CurrentState.None;
                        break;
                    case CurrentState.Timestamp:
                        if (timeSpanType == TimeSpanType.Milliseconds)
                        {
                            if (curChar != ']')
                            {
                                timeCalculationCache = timeCalculationCache * 10 + curChar - '0';
                                continue;
                            }
                            else
                            {
                                var pow = i - curStateStartPosition - 1; // 几位小数
                                curTimestamp = curTimestamp + timeCalculationCache * (int)Math.Pow(10, 3 - pow);
                                curTimestamps[currentTimestampPosition++] = curTimestamp;
                                timeSpanType = TimeSpanType.None;
                                timeCalculationCache = 0;
                                curStateStartPosition = i;
                                curTimestamp = 0;
                                state = CurrentState.PossiblyLyric;
                                continue;
                            }
                        }
                        switch (curChar)
                        {
                            case ':':
                            case '.':
                                if (timeSpanType == TimeSpanType.Minutes)
                                {
                                    curTimestamp = (curTimestamp + timeCalculationCache) * 60;
                                    timeCalculationCache = 0;
                                    timeSpanType = TimeSpanType.Seconds;
                                    continue;
                                }
                                if(timeSpanType == TimeSpanType.Seconds)
                                {
                                    curTimestamp = (curTimestamp + timeCalculationCache) * 1000;
                                    curStateStartPosition = i;
                                    timeCalculationCache = 0;
                                    timeSpanType= TimeSpanType.Milliseconds;
                                    continue;
                                }
                                throw new ArgumentOutOfRangeException();
                            case ']':
                                // 无毫秒数需要从此跳出
                                curTimestamps[currentTimestampPosition++] = (curTimestamp + timeCalculationCache) * 1000;
                                timeCalculationCache = 0;
                                curStateStartPosition = i;
                                curTimestamp = 0;
                                state = CurrentState.PossiblyLyric;
                                continue;
                            default:
                                timeCalculationCache = timeCalculationCache * 10 + curChar - '0';
                                break;
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            ArrayPool<int>.Shared.Return(curTimestamps, true);
            return lines;
        }

        private enum CurrentState
        {
            None,
            AwaitingState,
            AwaitingStateLyric,
            Attribute,
            AttributeContent,
            Timestamp,
            PossiblyLyric,
            Lyric
        }
        private enum TimeSpanType
        {
            Minutes,
            Seconds,
            Milliseconds,
            None
        }
    }
}