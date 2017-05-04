using System;
using cafe.CommandLine;
using cafe.Shared;

namespace cafe.Chef
{
    public interface IChefProcess
    {
        Result Run(params string[] args);
        event EventHandler<ChefLogEntry> LogEntryReceived;
    }
}