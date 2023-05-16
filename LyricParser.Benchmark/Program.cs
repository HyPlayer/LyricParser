using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Kfstorm.LrcParser;
using LyricParser.Implementation;
using Opportunity.LrcParser;

namespace MyBenchmarks
{
    public class Program
    {
        [MemoryDiagnoser]
        public class RegexPraserVsLyricParser
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

            public void OpportunityLiuLyricParser()
            {
                var lyricDataResult = Lyrics.Parse(SongLyric);
                foreach (var lyric in lyricDataResult.Lyrics.Lines)
                {
                }
            }
            [Benchmark]

            public void KfStormLyricParser()
            {
                var lyricDataResult = LrcFile.FromText(SongLyric);
                foreach (var lyric in lyricDataResult.Lyrics)
                {
                }
            }


            [Benchmark]
            public void HyPlayerNewPaser()
            {
                var result = LrcParser.ParseLrc(SongLyric);
                foreach (var lyric in result.Lines)
                {
                }
            }

            [Benchmark]
            public void HyPlayerNewKaraokePraser()
            {
                var lyricDataResult = KaraokeParser.ParseKaraoke(KaraokeLyric);
                foreach (var lyric in lyricDataResult.Lines)
                {
                }
            }
        }

        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<RegexPraserVsLyricParser>();
            Console.WriteLine("Benchmark for fun, don't be serious");
            Console.ReadLine();
        }
    }
}