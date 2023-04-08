using LrcParser.Classes;
using System;
using System.Buffers;
using System.Collections.Generic;

namespace LrcParser.Implementation;

public static class LrcParser
{
    public static List<ILyricLine> ParseLrc(ReadOnlySpan<char> input)
    {
        List<ILyricLine> lines = new();
        var curStateStartPosition = 0;
        bool isCalculatingMicrosecond = false;
        var timeCalculationCache = 0;
        var curTimestamps = ArrayPool<int>.Shared.Rent(16); // Max Count 
        int curTimestamp = 0;
        int currentTimestampPosition = 0;
        var state = CurrentState.None;
        for (var i = 0; i < input.Length; i++)
        {
            ref readonly var curChar = ref input[i];

            // 剥离开, 方便 分支预测
            if (state == CurrentState.Lyric)
            {
                if (curChar != '\n' && curChar != '\r')
                {
                    continue;
                }
                else
                {
                    // Sum up
                    for (int j = 0; j < curTimestamps.Length; j++)
                    {
                        if (curTimestamps[j] == -1) break;
                        lines.Add(new LrcLyricsLine(
                            input.Slice(curStateStartPosition + 1, i - curStateStartPosition - 1).ToString(),
                            curTimestamps[j]));
                    }

                    if (input[i + 1] is '\n' or '\r') i++;
                    // Change State
                    currentTimestampPosition = 0;
                    state = CurrentState.None;
                    continue;
                }
            }

            switch (state)
            {
                case CurrentState.PossiblyLyric:
                    if (curChar == '[')
                    {
                        state = CurrentState.AwaitingState;
                    }
                    else
                    {
                        // Sum up
                        for (int j = 0; j < curTimestamps.Length; j++)
                        {
                            if (curTimestamps[j] == -1) break;
                            lines.Add(new LrcLyricsLine(
                                input.Slice(curStateStartPosition + 1, i - curStateStartPosition - 1).ToString(),
                                curTimestamps[j]));
                        }

                        if(i+1 < input.Length)
                        {
                            if ((input[i + 1] == '\n' || input[i + 1] == '\r')) i++;
                        }
                        // Change State
                        currentTimestampPosition = 0;
                        state = CurrentState.None;
                        continue;
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
                    if (47 < curChar && curChar < 57)
                    {
                        // Time
                        state = CurrentState.Timestamp;
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
                    if (isCalculatingMicrosecond)
                    {
                        if (curChar != ']')
                        {
                            timeCalculationCache = timeCalculationCache * 10 + curChar - 48;
                            continue;
                        }
                        else
                        {
                            var pow = i - curStateStartPosition - 2;
                            curTimestamp = curTimestamp + timeCalculationCache * (int)Math.Pow(10, pow);
                            curTimestamps[currentTimestampPosition++] = curTimestamp;
                            isCalculatingMicrosecond = false;
                            timeCalculationCache = 0;
                            curStateStartPosition = i;
                            state = CurrentState.PossiblyLyric;
                            continue;
                        }
                    }

                    switch (curChar)
                    {
                        case ':':
                            curTimestamp = (curTimestamp + timeCalculationCache) * 60;
                            timeCalculationCache = 0;
                            continue;
                        case '.':
                            curTimestamp = (curTimestamp + timeCalculationCache) * 1000;
                            curStateStartPosition = i;
                            isCalculatingMicrosecond = true;
                            timeCalculationCache = 0;
                            continue;
                        default:
                            timeCalculationCache = timeCalculationCache * 10 + curChar - 48;
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
        Attribute,
        AttributeContent,
        Timestamp,
        PossiblyLyric,
        Lyric
    }
}