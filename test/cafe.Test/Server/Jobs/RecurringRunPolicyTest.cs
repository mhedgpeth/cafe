using cafe.Server.Jobs;
using FluentAssertions;
using NodaTime;
using Xunit;

namespace cafe.Test.Server.Jobs
{
    public class RecurringRunPolicyTest
    {
        private static readonly Duration FiveMinutes = Duration.FromMinutes(5);

        [Fact]
        public void TimerFired_ShouldFireJobReady()
        {
            var timerFactory = new FakeTimerFactory();

            var recurringPolicy = new RecurringRunPolicy(Duration.FromMinutes(30), timerFactory, new FakeClock());
            bool isReady = false;
            recurringPolicy.Due += (sender, args) =>  isReady = true;

            timerFactory.FireTimerAction();

            timerFactory.Every.Should().Be(recurringPolicy.Every, "timer should be on duration of the recurrence");
            isReady.Should().BeTrue("because the timer fired");
        }

        [Fact]
        public void ExpectedNextRun_ShouldBeCreatedDatePlusDurationOnInitialRun()
        {
            var clock = new FakeClock();
            var created = clock.CurrentInstant;
            var interval = FiveMinutes;
            var policy = new RecurringRunPolicy(interval, new FakeTimerFactory(), clock);

            policy.ExpectedNextRun.Should().Be(created.Plus(interval));
        }

        [Fact]
        public void ExpectedNextRun_ShouldBeLastRunDatePlusIntervalAfterItRuns()
        {
            var clock = new FakeClock();
            var interval = FiveMinutes;
            var timer = new FakeTimerFactory();

            var policy = new RecurringRunPolicy(interval, timer, clock);
            clock.AddToCurrentInstant(FiveMinutes);
            var timeAtTimerAction = clock.CurrentInstant;
            timer.FireTimerAction();
            clock.AddToCurrentInstant(Duration.FromMinutes(1));


            policy.ExpectedNextRun.Should().Be(timeAtTimerAction.Plus(interval));
        }

    }
}