using System;
using System.Threading.Tasks;

namespace cafe
{
    public interface IFileDownloader
    {
        bool Download(Uri downloadLink, string file);
    }
}