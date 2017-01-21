using cafe.Chef;
using cafe.Server.Jobs;
using cafe.Test.Chef;
using cafe.Test.Server.Scheduling;
using FluentAssertions;
using Moq;
using Xunit;

namespace cafe.Test.Server.Jobs
{
    public class RunChefJobTest
    {
        [Fact]
        public void Run_ShouldRunChefNormally()
        {
            var chefRunner = new FakeChefRunner();
            var runChefJob = CreateRunChefJobThatRunsJobsImmediately(chefRunner);

            runChefJob.Run();

            chefRunner.WasRun.Should().BeTrue("because we ran the job it should run the runner");
            chefRunner.Bootstrapper.Should().BeNull("because we only ran it normally");
        }

        private RunChefJob CreateRunChefJobThatRunsJobsImmediately(IChefRunner chefRunner, RunPolicy runPolicy = null)
        {
            var job = CreateRunChefJob(chefRunner, runPolicy);
            job.RunReady += RunJobImmediately;
            return job;
        }

        public static RunChefJob CreateRunChefJob(IChefRunner chefRunner = null, RunPolicy runPolicy = null)
        {
            chefRunner = chefRunner ?? new FakeChefRunner();
            runPolicy = runPolicy ?? new FakeRunPolicy();
            return new RunChefJob(runPolicy, chefRunner, new FakeClock());
        }

        [Fact]
        public void BootstrapPolicy_ShouldRunChefWithBootstrapper()
        {
            var chefRunner = new FakeChefRunner();
            var runChefJob = CreateRunChefJobThatRunsJobsImmediately(chefRunner);
            var bootstrapper = new Mock<IChefBootstrapper>().Object;

            runChefJob.Bootstrap(bootstrapper);

            chefRunner.WasRun.Should().BeTrue("because bootstrap should run chef");
            chefRunner.Bootstrapper.Should().BeSameAs(bootstrapper, "because we are bootstrapping chef");
        }

        private void RunJobImmediately(object sender, JobRun e)
        {
            e.Run();
        }

        [Fact]
        public void RunPolicy_DueEvent_ShouldRunJob()
        {
            var chefRunner = new FakeChefRunner();
            var policy = new FakeRunPolicy();
            CreateRunChefJobThatRunsJobsImmediately(chefRunner, policy);

            policy.FireDue();

            chefRunner.WasRun.Should().BeTrue("because a run of the job was due");
        }

        [Fact]
        public void Pause_ShouldMakeTheTaskNotReadyEvenWhenItIsDue()
        {
            var policy = new FakeRunPolicy();
            var runChefJob = CreateRunChefJob(runPolicy: policy);
            bool wasJobRunRequested = false;
            runChefJob.RunReady += (sender, run) => wasJobRunRequested = true;

            runChefJob.Pause();
            policy.FireDue();

            wasJobRunRequested.Should()
                .BeFalse("because the job is paused, even when the policy is due it shouldn't fire");
        }

        [Fact]
        public void Resume_ShouldMakeTheTaskRunnableAgain()
        {
            var policy = new FakeRunPolicy();
            var runChefJob = CreateRunChefJob(runPolicy: policy);
            bool wasJobRunRequested = false;
            runChefJob.RunReady += (sender, run) => wasJobRunRequested = true;

            runChefJob.Pause();
            runChefJob.Resume();
            policy.FireDue();

            wasJobRunRequested.Should()
                .BeTrue("because the job is resumed it should request to be ran");
        }

        [Fact]
        public void Due_ShouldNotRequestRunWhenPreviousRunIsStillRunning()
        {
            var policy = new FakeRunPolicy();
            var runChefJob = CreateRunChefJob(runPolicy: policy);

            policy.FireDue(); // which creates a job that doesn't run, so it's not done yet

            var wasRunReady = false;
            runChefJob.RunReady += (sender, run) => wasRunReady = true;

            policy.FireDue();

            wasRunReady.Should().BeFalse("because the original job has not finished yet");
        }

        [Fact]
        public void Due_ShouldFireWhenPreviousRunFinishedRunning()
        {
            var policy = new FakeRunPolicy();
            var runChefJob = CreateRunChefJob(runPolicy: policy);
            runChefJob.RunReady += RunJobImmediately;

            policy.FireDue();

            bool wasRunReady = false;
            runChefJob.RunReady += (sender, run) =>  wasRunReady = true;

            policy.FireDue();

            wasRunReady.Should().BeTrue("because the previous run already ran, we're ready to run again");
        }


    }
}