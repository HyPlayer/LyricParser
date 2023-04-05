using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using LrcParser.Implementation;
using Kfstorm.LrcParser;
using LrcParser.Classes;
using Microsoft.Diagnostics.Tracing.Parsers.JScript;
using Opportunity.LrcParser;

namespace MyBenchmarks
{
    public class Program
    {
        [MemoryDiagnoser]
        public class RegexPraserVsLrcParser
        {
            public string SongLyric = string.Empty;
            public string KaraokeLyric = string.Empty;

            [GlobalSetup]
            public void Setup()
            {
                using (var sample = File.OpenText("LrcDemo.txt"))
                {
                    SongLyric = sample.ReadToEnd();
                }

                using (var sample = File.OpenText("KaraokeDemo.txt"))
                {
                    KaraokeLyric = sample.ReadToEnd();
                }
            }

            [Benchmark]

            public void OpportunityLiuLrcParser()
            {
                var lyricDataResult = Lyrics.Parse(SongLyric);
                foreach (var lyric in lyricDataResult.Lyrics.Lines)
                {
                }
            }
            //[Benchmark]

            public void KfStormLrcParser()
            {
                var lyricDataResult = LrcFile.FromText(SongLyric);
                foreach (var lyric in lyricDataResult.Lyrics)
                {
                }
            }

            //[Benchmark]

            public void HyPlayerParser()
            {
                using (var lyricDataResult = LyricParser.ParseLrcLyrics(SongLyric))
                {
                    foreach (var lyric in lyricDataResult.LyricsLines)
                    {
                    }
                }
            }

            [Benchmark]
            public void HyPlayerNewPaser()
            {
                var result = LrcParser.Implementation.LrcParser.ParseLrc(SongLyric);
                foreach (var lyric in result)
                {
                }
            }
            
            //[Benchmark]
            public void HyPlayerKaraokePraser()
            {
                using (var lyricDataResult = LyricParser.ParseKaraokeLyrics(KaraokeLyric))
                {
                    foreach (var lyric in lyricDataResult.LyricsLines)
                    {
                    }
                }
            }
        }

        public static void Main(string[] args)
        {
            LrcParser.Implementation.LrcParser.ParseLrc(File.ReadAllText("LrcDemo.txt"));
            return;
            var summary = BenchmarkRunner.Run<RegexPraserVsLrcParser>();
            Console.ReadLine();
        }
    }
}