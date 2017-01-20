using cafe.Server.Jobs;

namespace cafe.Test.Server.Jobs
{
    public class SimpleJob : Job
    {
        private readonly JobRun _run;

        public SimpleJob(JobRun run)
        {
            _run = run;
        }

        public void Run()
        {
            OnRunReady(_run);
        }

    }
}