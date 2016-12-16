using System.Collections.Generic;
using cafe.LocalSystem;

namespace cafe.Test.Chef
{
    public class FakeEnvironment : IEnvironment
    {
        public string GetEnvironmentVariable(string key)
        {
            return EnvironmentVariables[key];
        }

        public IDictionary<string, string> EnvironmentVariables { get; }
        = new Dictionary<string, string>();
    }
}