using System;
using System.Collections.Generic;
using System.Linq;

namespace LrcParser.Classes
{
    public class LyricsData : IDisposable
    {
        public IList<ILyricLine> LyricsLines { get; }
        public Type LyricType => LyricsLines.First().GetType();
        public LyricsData(IList<ILyricLine> lyricsLines)
        {
            LyricsLines = lyricsLines;
        }
        ~LyricsData()
        {
            Dispose(true);
        }
        public void Dispose(bool isByFinalization)
        {
            if (!isByFinalization) GC.SuppressFinalize(this);
            LyricsLines.Clear();
        }

        public void Dispose()
        {
            Dispose(false);
        }
    }
}
