using cafe.Server;
using cafe.Server.Controllers;
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

            chefController.RunChef();
            const string version = "14.17.44";
            chefController.DownloadChef(version);
            chefController.InstallChef(version);
        }

        [Fact]
        public void JobController_ShouldInstantiateThroughStructureMap()
        {
            var jobController = AssertStructureMapCreatesControllerOfType<JobController>();
            jobController.GetStatus().Should().NotBeNull("because chef should be operational");
        }

        private static T AssertStructureMapCreatesControllerOfType<T>()
        {
            var controller = StructureMapResolver.Container.GetInstance<T>();
            controller.Should().NotBeNull("because structuremap should be configured to create it properly");
            return controller;
        }

        [Fact]
        public void InspecController_ShouldInstantiateThroughStructureMap()
        {
            var inspecController = AssertStructureMapCreatesControllerOfType<InspecController>();
            inspecController.GetStatus().Should().NotBeNull();
        }
    }
}