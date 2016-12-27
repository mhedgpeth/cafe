
# Near Term

## Better view of what is happening
* Tasks show failure with helpful text on what happened in the failure
* Task provides current status - so users can see that something is happening
* Include more detailed scheduler status, migrate it to simply cafe status
* When the server is not running, handle the error gracefully
* When the server is cut off while running, handle the error gracefully

## Make scheduler production ready
* Immediate run of chef run/etc instead of waiting on scheduled interval
* Schedule chef to run every X minutes (RecurringTask implementation) with `cafe schedule chef every: 30 minutes`
* Pause just chef (not other things, like an install/upgrade) with `cafe pause chef`
* Resume just chef (not other things) with `cafe resume chef`

## Ease of use
* When server isn't started, start it in process (to make this easy to adopt) - maybe just focus on making it easy for people to run the server with below service

# Long Term

* Initialize cafe to the path so it can run from anywhere on the CLI
* Register to run server as a service
  - Register with `cafe service register`
  - Start service with `cafe service start`
  - Stop service with `cafe service stop`
* Bootstrap through REST API (after cafe is installed, but before chef is installed)
* Simple web page for when people locally go to localhost to view status
* Update documentation
* Create youtube video