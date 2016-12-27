using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using cafe.Shared;
using NLog;

namespace cafe
{
    public class FileDownloader : IFileDownloader
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(FileDownloader).FullName);

        public Result Download(Uri downloadLink, string file)
        {
            return DownloadAsync(downloadLink, file).GetAwaiter().GetResult();
        }

        public async Task<Result> DownloadAsync(Uri downloadLink, string file)
        {
            using (var httpClient = new HttpClient {Timeout = TimeSpan.FromMinutes(10)})
            {
                using (
                    var request = new HttpRequestMessage(HttpMethod.Get, downloadLink)
                )
                {
                    const int bufferSize = 4096;
                    var response = (await httpClient.SendAsync(request));
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        Logger.Info($"File at {downloadLink} does not exist");
                        return Result.Failure($"File at {downloadLink} does not exist");
                    }
                    using (
                        Stream contentStream = await response.Content.ReadAsStreamAsync(),
                            stream = new FileStream(file, FileMode.Create, FileAccess.Write,
                                FileShare.None, bufferSize, true))
                    {
                        Logger.Debug("Downloading file");
                        await contentStream.CopyToAsync(stream);
                        Logger.Debug("Finished downloading file");
                    }
                }
            }
            return Result.Successful();
        }
    }
}