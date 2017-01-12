using Newtonsoft.Json;

namespace cafe.Chef
{
    public abstract class BootstrapSettings
    {
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}