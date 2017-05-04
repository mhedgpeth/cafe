using cafe.Options;
using cafe.Options.Server;
using cafe.Server.Jobs;
using cafe.Test.Server.Jobs;
using FluentAssertions;
using NodaTime;
using Xunit;

namespace cafe.Test.Options
{
    public class CafeServerWindowsServiceTest
    {
        [Fact]
        public void Initialize_ShouldInitializeChefRecurringTaskIfSettingExists()
        {
            const int interval = 1800;
            var runChefJob = RunChefJobTest.CreateRunChefJob();
            var timerFactory = new FakeTimerFactory();
            var clock = new FakeClock();
            CafeServerWindowsService.Initialize(runChefJob, interval, timerFactory, clock);

            bool wasRunReady = false;
            runChefJob.RunReady += (sender, run) => wasRunReady = true;

            timerFactory.FireTimerAction();

            wasRunReady.Should()
                .BeTrue(
                    "because there was a policy created by the initialize that tied the timer to when runs were ready");
        }

        [Fact]
        public void Initialize_ShouldDoNothingIfIntervalIsZero()
        {
            AssertInitializeDoesNothingWhenIntervalIs(0);
        }

        [Fact]
        public void Initialize_ShouldDoNothingIfIntervalIsNegative()
        {
            AssertInitializeDoesNothingWhenIntervalIs(-1);
        }

        private static void AssertInitializeDoesNothingWhenIntervalIs(int chefIntervalInSeconds)
        {
            var runChefJob = RunChefJobTest.CreateRunChefJob();

            CafeServerWindowsService.Initialize(runChefJob, chefIntervalInSeconds, new FakeTimerFactory(), new FakeClock());

            runChefJob.RunPolicy.Should().NotBeOfType<RecurringRunPolicy>("because no interval was given, it will not be run on an interval");
        }
    }

}