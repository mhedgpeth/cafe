using cafe.Server.Jobs;

namespace cafe.Test.Server.Jobs
{
    public class FakeRunPolicy : RunPolicy
    {
        public FakeRunPolicy() : base(null)
        {

        }

        public void FireDue()
        {
            OnDue();
        }
    }
}