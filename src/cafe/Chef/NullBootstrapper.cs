using System.Collections.Generic;

namespace cafe.Chef
{
    public class RunChefPolicy : IRunChefPolicy
    {
        protected static readonly string ChefInstallDirectory = $@"{ServerSettings.Instance.InstallRoot}\chef";
        protected static readonly string ClientConfigPath = $@"{ChefInstallDirectory}\client.rb";


        public virtual void PrepareEnvironmentForChefRun()
        {
            // nothing to do here
        }

        public string[] ArgumentsForChefRun()
        {
            var arguments = new List<string> {"-c", ClientConfigPath};
            arguments.AddRange(AdditionalArgumentsForChefRun());
            return arguments.ToArray();
        }

        protected virtual string[] AdditionalArgumentsForChefRun()
        {
            return new string[0];
        }

        public override string ToString()
        {
            return "normally";
        }
    }
}