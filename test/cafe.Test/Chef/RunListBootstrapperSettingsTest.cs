using cafe.Chef;
using FluentAssertions;
using Xunit;

namespace cafe.Test.Chef
{
    public class RunListBootstrapperSettingsTest
    {
        [Fact]
        public void ToJson_ShouldBeReadableByChefClient()
        {
            ChefRunner.ParseRunList("recipe[base],recipe[foo],recipe[bar],role[webserver]").ToJson()
                .Should().Be("{\r\n  \"run_list\": [\r\n    \"recipe[base]\",\r\n    \"recipe[foo]\",\r\n    \"recipe[bar]\",\r\n    \"role[webserver]\"\r\n  ]\r\n}");
        }
    }
}