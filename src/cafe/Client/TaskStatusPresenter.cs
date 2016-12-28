using System;
using cafe.Shared;

namespace cafe.Client
{
    public class TaskStatusPresenter
    {
        private readonly IMessagePresenter _messagePresenter;
        private ScheduledTaskStatus _lastStatus;

        public TaskStatusPresenter(IMessagePresenter messagePresenter)
        {
            _messagePresenter = messagePresenter;
        }

        public void PresentAnyChangesTo(ScheduledTaskStatus status)
        {
            if (status == null)
            {
                return;
            }
            if (_lastStatus == null)
            {
                throw new InvalidOperationException("Expecting last status to be populated");
            }
            if (_lastStatus.IsNotRun && status.IsRunning)
            {
                _messagePresenter.ShowMessage($"Server started running {status.Description} at {status.StartTime}");
            }
            if (!string.Equals(_lastStatus.CurrentMessage, status.CurrentMessage))
            {
                _messagePresenter.ShowMessage($"Latest: {status.CurrentMessage}");
            }
            _lastStatus = status;
        }

        public void BeginPresenting(ScheduledTaskStatus status)
        {
            if (status == null)
            {
                throw new ArgumentNullException(nameof(status));
            }
            _lastStatus = status;
            if (status.IsNotRun)
            {
                _messagePresenter.ShowMessage($"Server waiting to start {status.Description}");
            }
        }
    }
}