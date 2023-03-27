using System;
using System.Collections.Generic;

namespace LrcParser.Classes
{
    public class LrcLyricsData
    {
        public IList<LrcLyricsLine> LyricsLines { get; }
        public LrcLyricsData(IList<LrcLyricsLine> lyricsLines)
        {
            LyricsLines = lyricsLines;
        }
    }
    public class KaraokeLyricsData
    {
        public IList<KaraokeLyricsLine> LyricsLines { get; }
        public KaraokeLyricsData(IList<KaraokeLyricsLine> lyricsLines)
        {
            LyricsLines = lyricsLines;
        }
    }
}
