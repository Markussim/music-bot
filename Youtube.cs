using System.Diagnostics;
using System.Web;

class YoutubeService
{
    string cachePath = "./cache";
    private static Stream PlayFile(string path)
    {
        Process? ffmpeg = Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $@"-i ""{path}"" -ac 2 -f s16le -ar 48000 pipe:1",
            RedirectStandardOutput = true,
            UseShellExecute = false
        });

        return ffmpeg.StandardOutput.BaseStream;
    }

    private string DownloadFile(string url)
    {

        string fullPath = $"{cachePath}/{UrlToId(url)}.mp3";

        // Use yt-dlp to download the file
        Process? ytDlp = Process.Start(new ProcessStartInfo
        {
            FileName = "yt-dlp",
            Arguments = $@"-o ""{fullPath}"" --audio-format mp3 --extract-audio --audio-quality 0 {url}",
            RedirectStandardOutput = true,
            UseShellExecute = false
        });

        try
        {
            ytDlp.WaitForExit();

            Console.WriteLine("Downloaded file");

            return fullPath;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            throw new Exception("Failed to download file");
        }

    }

    public Stream GetVideo(string url)
    {
        // Check if the video is already downloaded
        string fullPath = $"{cachePath}/{UrlToId(url)}.mp3";

        if (File.Exists(fullPath))
        {
            return PlayFile(fullPath);
        }

        DownloadFile(url);

        return PlayFile(fullPath);

    }

    private string UrlToId(string url)
    {
        Uri uri = new Uri(url);
        string query = uri.Query;
        System.Collections.Specialized.NameValueCollection queryDictionary = HttpUtility.ParseQueryString(query);
        return queryDictionary["v"];
    }

}
