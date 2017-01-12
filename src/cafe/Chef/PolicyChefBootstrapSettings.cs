using Newtonsoft.Json;

namespace cafe.Chef
{
    public class PolicyChefBootstrapSettings : BootstrapSettings
    {
        [JsonProperty("policy_name")]
        public string PolicyName { get; set; }
        [JsonProperty("policy_group")]
        public string PolicyGroup { get; set; }

        public override string ToString()
        {
            return $"policy #{PolicyName} and group #{PolicyGroup}";
        }
    }
}