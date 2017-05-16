using System.Threading.Tasks;
using cafe.Client;
using cafe.CommandLine;
using cafe.Options.Chef;
using cafe.Server.Jobs;
using cafe.Shared;
using FluentAssertions;
using Moq;
using Xunit;

namespace cafe.Test.Options
{
    public class RunJobOptionTest
    {
        [Fact]
        public void Run_ShouldRunAndWait()
        {
            var chefServer = new MockChefServer();
            var waiter = new FakeScheduleWaiter();
            var runChef = new RunChefOption(() => chefServer, waiter);

            var result = runChef.Run();

            result.IsSuccess.Should().BeTrue();
            waiter.Waited.Should().BeTrue("because the run should have waited");
        }

        [Fact]
        public void Run_ShouldRunAndNotWaitIfReturningImmediately()
        {
            var chefServer = new MockChefServer();
            var waiter = new FakeScheduleWaiter();
            var runChef = new RunChefOption(() => chefServer, waiter);

            var result = runChef.Run(new ValueArgument("return:", "immediately"));

            result.IsSuccess.Should().BeTrue();
            waiter.Waited.Should().BeFalse("because the run should not have waited when it was cvonfigured to return immediately");
        }
    }

    public class FakeScheduleWaiter : ISchedulerWaiter
    {
        public JobRunStatus WaitForTaskToComplete(JobRunStatus status)
        {
            Waited = true;
            return status;
        }

        public bool Waited { get; set; }
    }

    public class MockChefServer : IChefServer
    {
        public Task<ChefStatus> GetStatus()
        {
            return Task<ChefStatus>.FromResult(new ChefStatus() {IsRunning = false});
        }


        public Task<JobRunStatus> Download(string version)
        {
            throw new System.NotImplementedException();
        }

        public Task<JobRunStatus> Install(string version)
        {
            throw new System.NotImplementedException();
        }

        public Task<JobRunStatus> RunChef()
        {
            return Task<JobRunStatus>.FromResult(new JobRunStatus() { Result = Result.Successful() });
        }

        public Task<JobRunStatus> BootstrapChef(string config, string validator, string policyName, string policyGroup)
        {
            throw new System.NotImplementedException();
        }

        public Task<JobRunStatus> BootstrapChef(string config, string validator, string runList)
        {
            throw new System.NotImplementedException();
        }

        public Task<ChefStatus> Pause()
        {
            throw new System.NotImplementedException();
        }

        public Task<ChefStatus> Resume()
        {
            throw new System.NotImplementedException();
        }
    }
}