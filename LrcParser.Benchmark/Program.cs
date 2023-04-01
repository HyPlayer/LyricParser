using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using LrcParser.Implementation;
using Kfstorm.LrcParser;
using Microsoft.Diagnostics.Tracing.Parsers.JScript;
using Opportunity.LrcParser;

namespace MyBenchmarks
{
    public class Program
    {
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
                for (int i = 0; i < 10000; i++)
                {
                    var lyricDataResult = Lyrics.Parse(SongLyric);
                }
            }
            [Benchmark]

            public void KfStormLrcParser()
            {
                for(int i = 0;i<10000 ;i++) 
                {
                    var lyricDataResult = LrcFile.FromText(SongLyric);
                }
                
            }

            [Benchmark]

            public void HyPlayerPraser()
            {
                for (int i = 0; i < 10000; i++)
                {
                    var lyricDataResult = LyricParser.ParseLrcLyrics(SongLyric);
                }
            }
            [Benchmark]
            public void HyPlayerKaraokePraser()
            {
                for (int i = 0; i < 10000; i++)
                {
                    var lyricDataResult = LyricParser.ParseKaraokeLyrics(KaraokeLyric);
                }
            }
        }

        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<RegexPraserVsLrcParser>();
            Console.ReadLine();
        }
    }
}