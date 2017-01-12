using cafe.Chef;
using FluentAssertions;
using Xunit;

namespace cafe.Test.Chef
{
    public class PolicyChefBootstrapSettingsTest
    {
        [Fact]
        public void ToJson_ShouldBeReadableByChefClient()
        {
            var settings = new PolicyChefBootstrapSettings() { PolicyGroup = "group", PolicyName = "name"};

            settings.ToJson().Should().Be("{\r\n  \"policy_name\": \"name\",\r\n  \"policy_group\": \"group\"\r\n}");
        }
    }
}