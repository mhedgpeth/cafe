using System.Linq;

namespace cafe.CommandLine
{
    public class Runner
    {
        private readonly Option[] _options;

        public Runner(params Option[] options)
        {
            _options = options;
        }

        public void Run(params string[] args)
        {
            foreach (var option in _options)
            {
                if (option.IsSatisfiedBy(args))
                {
                    option.Run(args);
                }
            }
        }
    }
}