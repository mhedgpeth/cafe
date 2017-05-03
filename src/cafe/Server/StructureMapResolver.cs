using System;
using cafe.Chef;
using cafe.CommandLine.LocalSystem;
using cafe.LocalSystem;
using cafe.Server.Jobs;
using NodaTime;
using StructureMap;

namespace cafe.Server
{
    public static class StructureMapResolver
    {
        public static readonly IContainer Container = CreateContainer();

        public static Container CreateContainer()
        {
            var container = new Container();
            container.Configure(config =>
            {
                config.Scan(scanner =>
                {
                    scanner.AssemblyContainingType(typeof(ChefJobRunner));
                    scanner.AssemblyContainingType<TimerFactory>();
                    scanner.WithDefaultConventions();
                });

                config.For<ChefJobRunner>().Singleton();

                config.For<RunChefJob>().Singleton();
                config.For<ChefProduct>().Singleton();
                config.For<JobRunner>().Singleton();
                config.For<IEnvironment>().Use<EnvironmentBoundary>();
                config.For<IFileSystemCommands>().Use<FileSystemCommandsBoundary>();
                config.For<IProcess>().Use<ProcessBoundary>();
                config.For<IActionExecutor>().Use<RunInBackgroundActionExecutor>();
                config.For<IClock>().Use(SystemClock.Instance);
                config.For<IInstaller>().Use<ChefProduct>();
                config.For<RunPolicy>().Use(context => RunPolicy.OnDemand());
            });
            return container;
        }
    }
}