using cafe.CommandLine.LocalSystem;

namespace cafe.CommandLine.Options
{
    public class ServiceStatusWaiter : StatusWaiter<ServiceStatus>
    {
        private readonly ServiceStatusProvider _serviceStatusProvider;
        private readonly string _serviceName;
        private ServiceStatus _waitingFor;

        public ServiceStatusWaiter(string taskDescription, IAutoResetEvent autoResetEvent, ITimerFactory timerFactory,
            ServiceStatusProvider serviceStatusProvider, string serviceName)
            : base(taskDescription, autoResetEvent, timerFactory)
        {
            _serviceStatusProvider = serviceStatusProvider;
            _serviceName = serviceName;
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
            return _serviceStatusProvider.DetermineStatus(_serviceName);
        }
    }
}