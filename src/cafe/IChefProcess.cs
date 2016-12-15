using System;

namespace cafe
{
    public interface IChefProcess
    {
        void Run(params string[] args);
        event EventHandler<ChefLogEntry> LogEntryReceived;
    }
}