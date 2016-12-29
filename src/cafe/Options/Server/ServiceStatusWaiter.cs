using cafe.Client;
using cafe.Server.Scheduling;

namespace cafe.Options.Server
{
    public class ServiceStatusWaiter : StatusWaiter<ServiceStatus>
    {
        private readonly ServiceStatusProvider _serviceStatusProvider;
        private ServiceStatus _waitingFor;

        public ServiceStatusWaiter(string taskDescription, IAutoResetEvent autoResetEvent, ITimerFactory timerFactory,
            ServiceStatusProvider serviceStatusProvider)
            : base(taskDescription, autoResetEvent, timerFactory)
        {
            _serviceStatusProvider = serviceStatusProvider;
        }

        public ServiceStatus WaitFor(ServiceStatus serviceStatus)
        {
            _waitingFor = serviceStatus;
            return Wait();
        }

        protected override bool IsCurrentStatusCompleted(ServiceStatus currentStatus)
        {
            return currentStatus == _waitingFor;
        }

        protected override ServiceStatus RetrieveCurrentStatus()
        {
            return _serviceStatusProvider.DetermineStatus(CafeServerWindowsServiceOptions.ServiceName);
        }
    }
}