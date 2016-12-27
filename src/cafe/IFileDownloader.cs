using System;
using System.Threading.Tasks;
using cafe.Shared;

namespace cafe
{
    public interface IFileDownloader
    {
        Result Download(Uri downloadLink, string file);
    }
}