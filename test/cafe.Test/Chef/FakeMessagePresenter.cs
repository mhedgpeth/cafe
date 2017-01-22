using cafe.Shared;

namespace cafe.Test.Chef
{
    public class FakeMessagePresenter : IMessagePresenter
    {
        public void ShowMessage(string message)
        {
            MessageShown = message;
            WasMessageShown = true;
        }

        public string MessageShown { get; set; }

        public void Clear()
        {
            WasMessageShown = false;
        }

        public bool WasMessageShown { get; private set; }
    }
}