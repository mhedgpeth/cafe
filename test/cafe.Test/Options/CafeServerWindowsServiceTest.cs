using cafe.Options;
using cafe.Test.Server.Scheduling;
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
            var scheduler = SchedulerTest.CreateScheduler();
            CafeServerWindowsService.Initialize(scheduler, interval);

            var recurringTask = scheduler.FindRecurringTaskByName("chef");

            recurringTask.Should().NotBeNull("because initialize should have added it");
            recurringTask.Interval.Should().Be(Duration.FromSeconds(interval));
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
            var scheduler = SchedulerTest.CreateScheduler();

            CafeServerWindowsService.Initialize(scheduler, chefIntervalInSeconds);

            scheduler.FindRecurringTaskByName("chef")
                .Should()
                .BeNull("because no interval was given, it will not be run on an interval");
        }
    }

}