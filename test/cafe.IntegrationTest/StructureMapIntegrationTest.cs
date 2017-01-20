using cafe.Server;
using cafe.Server.Controllers;
using cafe.Server.Jobs;
using cafe.Server.Scheduling;
using FluentAssertions;
using Xunit;

namespace cafe.IntegrationTest
{
    public class StructureMapIntegrationTest
    {
        [Fact]
        public void ChefController_ShouldInsantiateThroughStructureMap()
        {
            var chefController = AssertStructureMapCreatesControllerOfType<ChefController>();
            var scheduler = StructureMapResolver.Container.GetInstance<ChefJobRunner>();
            scheduler.RunChefJob.Pause();

            chefController.RunChef();
            const string version = "14.17.44";
            chefController.DownloadChef(version);
            chefController.InstallChef(version);
            chefController.GetChefStatus().Should().NotBeNull("because chef should be operational");
        }

        private static T AssertStructureMapCreatesControllerOfType<T>()
        {
            var controller = StructureMapResolver.Container.GetInstance<T>();
            controller.Should().NotBeNull("because structuremap should be configured to create it properly");
            return controller;
        }

        [Fact]
        public void Scheduler_ShouldBeSingleton()
        {
            var one = StructureMapResolver.Container.GetInstance<ChefJobRunner>();
            var another = StructureMapResolver.Container.GetInstance<ChefJobRunner>();

            one.Should().BeSameAs(another, "because chef job runner should be a singleton");
        }

        [Fact]
        public void SchedulerController_ShouldInstantiateThroughStructureMap()
        {
            AssertStructureMapCreatesControllerOfType<SchedulerController>();
        }
    }
}