using System;
using cafe.Client;
using cafe.Shared;
using cafe.Test.Chef;
using FluentAssertions;
using Xunit;

namespace cafe.Test.CommandLine
{
    public class TaskStatusPresenterTest
    {
        [Fact]
        public void SameMessage_ShouldNotLogAnything()
        {
            var presenter = new FakeMessagePresenter();
            var status = ScheduledTaskStatus.Create("task");

            var statusPresenter = new TaskStatusPresenter(presenter);
            statusPresenter.BeginPresenting(status);
            presenter.Clear();

            statusPresenter.PresentAnyChangesTo(status);

            presenter.WasMessageShown.Should().BeFalse("because no changes have been made yet");
        }

        private static readonly DateTime StartTime = new DateTime(2016, 12, 27, 12, 34, 0);

        [Fact]
        public void TaskStart_ShouldLogMessage()
        {
            var presenter = new FakeMessagePresenter();
            var status = ScheduledTaskStatus.Create("task");

            var statusPresenter = new TaskStatusPresenter(presenter);
            statusPresenter.BeginPresenting(status);

            presenter.Clear();

            var copy = status.ToRunningState(StartTime);

            statusPresenter.PresentAnyChangesTo(copy);

            presenter.WasMessageShown.Should().BeTrue("because the task started");
        }

        [Fact]
        public void DifferentCurrentMessage_ShouldLogMessage()
        {
            var presenter = new FakeMessagePresenter();
            var status = ScheduledTaskStatus.Create("task");

            var statusPresenter = new TaskStatusPresenter(presenter);
            statusPresenter.BeginPresenting(status);

            presenter.Clear();

            var copy = status.ToRunningState(StartTime);
            copy.CurrentMessage = "different message";
            statusPresenter.PresentAnyChangesTo(copy);

            presenter.WasMessageShown.Should().BeTrue("because the message changed");
            presenter.MessageShown.Should().Be($"Latest: {copy.CurrentMessage}");
        }

        [Fact]
        public void NullStatus_ShouldBeIgnored()
        {
            var presenter = new FakeMessagePresenter();
            var statusPresenter = new TaskStatusPresenter(presenter);
            statusPresenter.BeginPresenting(ScheduledTaskStatus.Create("task"));
            presenter.Clear();

            statusPresenter.PresentAnyChangesTo(null);

            presenter.WasMessageShown.Should().BeFalse("because nothing was given to the status presenter");
        }

        [Fact]
        public void MessageChange_ShouldLogMessageAfterFirstTime()
        {
            var presenter = new FakeMessagePresenter();
            var status = ScheduledTaskStatus.Create("task");

            var statusPresenter = new TaskStatusPresenter(presenter);
            statusPresenter.BeginPresenting(status);
            statusPresenter.PresentAnyChangesTo(status.WithDifferentMessage("another"));
            presenter.Clear();

            statusPresenter.PresentAnyChangesTo(status.WithDifferentMessage("third"));

            presenter.WasMessageShown.Should().BeTrue("because the message changed");
        }

        [Fact]
        public void TwoMessagesInARow_ShouldNotBePresented()
        {
            var presenter = new FakeMessagePresenter();
            var status = ScheduledTaskStatus.Create("task");

            var statusPresenter = new TaskStatusPresenter(presenter);
            statusPresenter.BeginPresenting(status);
            var another = status.WithDifferentMessage("another");
            statusPresenter.PresentAnyChangesTo(another);
            presenter.Clear();

            statusPresenter.PresentAnyChangesTo(another);

            presenter.WasMessageShown.Should().BeFalse("because the message did not change");
        }

    }
}