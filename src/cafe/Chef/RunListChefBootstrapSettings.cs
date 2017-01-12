using Newtonsoft.Json;

namespace cafe.Chef
{
    public class RunListChefBootstrapSettings : BootstrapSettings
    {
        [JsonProperty("run_list")]
        public string[] RunList { get; set; }

        public override string ToString()
        {
            return $"run list: #{RunList}";
        }
    }
}