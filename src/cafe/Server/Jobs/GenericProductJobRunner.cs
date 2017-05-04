using cafe.Shared;

namespace cafe.Server.Jobs
{
    public class GenericProductJobRunner : ProductJobRunner<ProductStatus>
    {
        public GenericProductJobRunner(JobRunner runner, DownloadJob downloadJob, InstallJob installJob) : base(runner,
            downloadJob, installJob)
        {
        }

        public override ProductStatus ToStatus()
        {
            return new ProductStatus
            {
                Version = InstallJob.CurrentVersion?.ToString()
            };
        }
    }
}