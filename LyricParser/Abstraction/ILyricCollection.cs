using System;
using System.Collections.Generic;

namespace LyricParser.Abstraction
{
    public interface ILyricCollection : IDisposable
    {
        IList<ILyricLine> Lines { get; }
    }
}
