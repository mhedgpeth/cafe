
# Near Term

## Better Presentation of the Basics
* Better UI on the client side (not using logger)
* Log to file and console on the server side
* Chef run should log to its own file (with own logger)

## Finish client/server transition
* Convert all client-facing Options to talk to REST API
* Live view of tasks, with descriptive names, and guids for up to date status as they work through the scheduler

## Make scheduler production ready
* Immediate run of chef run/etc instead of waiting on scheduled interval
* Schedule chef to run every X minutes (RecurringTask implementation) with `cafe schedule chef every: 30 minutes`
* Pause just chef (not other things, like an install/upgrade) with `cafe pause chef`
* Resume just chef (not other things) with `cafe resume chef`

## Ease of use
* When server isn't started, start it in process (to make this easy to adopt)

# Long Term

* Initialize cafe to the path so it can run from anywhere on the CLI
* Register to run server as a service
  - Register with `cafe service register`
  - Start service with `cafe service start`
  - Stop service with `cafe service stop`
* Bootstrap through REST API (after cafe is installed, but before chef is installed)
* Update documentation
* Create youtube video