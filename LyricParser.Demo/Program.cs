// See https://aka.ms/new-console-template for more information
using LrcParser.Implementation;

var sourceData = "[offset:0]\r\n[01:34.68]说很简单\r\n[00:38.50]但是做却很难\r\n[00:53.00][01:43.88][02:11.23]虽然无所谓写在脸上";
if (sourceData != null)
{
    var result = LyricParser.ParseLrcLyrics(sourceData);
    foreach (var item in result.LyricsLines)
    {
        Console.WriteLine($"TimeSpan:{item.StartTime}  Lyric:{item.CurrentLyric}");
    }
    Console.ReadLine();
}

