using System;
using System.Collections.Generic;

namespace LyricParser.Abstraction
{
    public sealed class LrcLyricCollection : ILyricCollection
    {
        private bool disposedValue;
        public IList<ILyricLine> Lines { get; }
        public Dictionary<string, string> Attributes = new Dictionary<string, string>();
        public LrcLyricCollection(IList<ILyricLine> lines, Dictionary<string, string> attributes)
        {
            Lines = lines;
            Attributes = attributes;
        }
        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Lines.Clear();
                    Attributes.Clear();
                }
                disposedValue = true;
            }
        }

        ~LrcLyricCollection()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
