using System;
using cafe.Chef;
using cafe.LocalSystem;
using cafe.Server.Scheduling;
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
                    scanner.AssemblyContainingType(typeof(Scheduler));
                    scanner.WithDefaultConventions();
                });

                config.For<Scheduler>().Singleton();
                config.For<ChefProduct>().Singleton();
                config.For<IEnvironment>().Use<EnvironmentBoundary>();
                config.For<IFileSystemCommands>().Use<FileSystemCommandsBoundary>();
                config.For<IProcess>().Use<ProcessBoundary>();
                config.For<IActionExecutor>().Use<RunInBackgroundActionExecutor>();
            });
            return container;
        }
    }
}