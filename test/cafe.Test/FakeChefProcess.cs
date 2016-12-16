using System;
using System.Collections.Generic;
using cafe.Chef;

namespace cafe.Test
{
    public class FakeChefProcess : IChefProcess
    {
        public void Run(params string[] args)
        {
            LogEntriesToReceiveDuringRun.ForEach(OnLogEntryReceived);
        }

        public List<ChefLogEntry> LogEntriesToReceiveDuringRun { get; } = new List<ChefLogEntry>();

        public event EventHandler<ChefLogEntry> LogEntryReceived;

        protected virtual void OnLogEntryReceived(ChefLogEntry e)
        {
            LogEntryReceived?.Invoke(this, e);
        }
    }
}