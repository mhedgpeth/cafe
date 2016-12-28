# Near Term

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