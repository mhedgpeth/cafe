using System;

namespace cafe.Chef
{
    public interface IChefProcess
    {
        void Run(params string[] args);
        event EventHandler<ChefLogEntry> LogEntryReceived;
    }
}