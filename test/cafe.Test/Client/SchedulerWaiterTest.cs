using System;
using System.Net.Http;
using cafe.Client;
using cafe.CommandLine.LocalSystem;
using cafe.Shared;
using cafe.Test.Chef;
using cafe.Test.Server.Jobs;
using FluentAssertions;
using Moq;
using Xunit;

namespace cafe.Test.Client
{
    public class SchedulerWaiterTest
    {
        [Fact]
        public void Exception_ShouldSetAutoresetEventAndFail()
        {
            var jobServer = new Mock<IJobServer>();
            jobServer.Setup(s => s.GetJobRunStatus(It.IsAny<Guid>(), It.IsAny<int>())).Throws<HttpRequestException>();
            var autoResetEvent = new FakeAutoResetEvent();
            var fakeTimerFactory = new FakeTimerFactory();

            var waiter = new SchedulerWaiter(() => jobServer.Object, autoResetEvent, fakeTimerFactory, new FakeMessagePresenter());
            autoResetEvent.WhatToDoDuringWaitOne = () =>
            {
                fakeTimerFactory.FireTimerAction();
            };

            var result = waiter.WaitForTaskToComplete(JobRunStatus.Create("sample task"));

            result.Result.IsSuccess.Should().BeFalse("because an exception was thrown by the scheduler");
            result.Result.FailureDescription.Should()
                .NotBeEmpty("because it should contain a description of what happened");
        }
    }

    public class FakeAutoResetEvent : IAutoResetEvent
    {
        public FakeAutoResetEvent()
        {
            WhatToDoDuringWaitOne = () => { };
        }

        public void WaitOne(TimeSpan timeout)
        {
            WhatToDoDuringWaitOne();
        }

        public Action WhatToDoDuringWaitOne { get; set; }

        public void Set()
        {
        }
    }
}