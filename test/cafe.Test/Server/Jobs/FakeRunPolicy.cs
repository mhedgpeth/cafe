using cafe.Server.Jobs;

namespace cafe.Test.Server.Jobs
{
    public class FakeRunPolicy : RunPolicy
    {
        public void FireDue()
        {
            OnDue();
        }
    }
}