using cafe.Chef;
using cafe.CommandLine;

namespace cafe.Options
{
    public class DownloadChefOption : Option
    {
        private readonly ChefDownloader _chefDownloader;

        public DownloadChefOption(ChefDownloader chefDownloader)
            : base(
                new OptionSpecification(OptionValueSpecification.ForExactValue("chef"),
                    OptionValueSpecification.ForExactValue("download"),
                    OptionValueSpecification.ForVersion()),
                "downloads the provided version of chef")
        {
            _chefDownloader = chefDownloader;
        }

        protected override void RunCore(string[] args)
        {
            _chefDownloader.Download(args[2]);
        }
    }
}