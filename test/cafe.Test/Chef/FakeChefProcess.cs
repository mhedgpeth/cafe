using System;
using System.Collections.Generic;
using cafe.Chef;
using cafe.Shared;

namespace cafe.Test.Chef
{
    public class FakeChefProcess : IChefProcess
    {
        public Result Run(params string[] args)
        {
            LogEntriesToReceiveDuringRun.ForEach(OnLogEntryReceived);
            return Result.Successful();
        }

        public List<ChefLogEntry> LogEntriesToReceiveDuringRun { get; } = new List<ChefLogEntry>();

        public event EventHandler<ChefLogEntry> LogEntryReceived;

        protected virtual void OnLogEntryReceived(ChefLogEntry e)
        {
            LogEntryReceived?.Invoke(this, e);
        }
    }
}