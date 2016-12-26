
# Near Term

* Better UI on the client side (not using logger)
* Immediate run of chef run/etc instead of waiting on scheduled interval
* Log to file on the server side
* Chef run should log to its own file (with own logger)
* When server isn't started, start it in process (easy to adopt)
* Convert all client-facing Options to talk to REST API
* Schedule chef to run every X minutes (RecurringTask implementation) with `cafe schedule chef every: 30 minutes`
* Live view of tasks, with descriptive names, and guids for up to date status as they work through the scheduler
* Pause just chef (not other things, like an install/upgrade) with `cafe pause chef`
* Resume just chef (not other things) with `cafe resume chef`

# Long Term

* Initialize cafe to the path so it can run from anywhere on the CLI
* Register to run server as a service
  - Register with `cafe service register`
  - Start service with `cafe service start`
  - Stop service with `cafe service stop`