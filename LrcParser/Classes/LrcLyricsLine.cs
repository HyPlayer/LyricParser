using System;
using System.Collections.Generic;
using System.Text;

namespace LrcParser.Classes
{
    public class LrcLyricsLine : LyricLineBase
    {
        public LrcLyricsLine(string currentLyric, TimeSpan startTime)
        {
            CurrentLyric = currentLyric;
            StartTime = startTime;
        }
    }
}
