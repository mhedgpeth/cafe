# cafe

Chef is a world-class configuration management platform that, for many of us, is a gateway into automation that was previously unimaginable. However, like many of its siblings, it started out within a linux environment. As a result, running and maintaining it is not straightforward when you come from a Windows background.

Cafe exists to make managing Chef runs in a Windows environment extremely easy.

Here are some of the things that Cafe does or will do soon:

* Download the `chef-client` installer for a particular version
* Install `chef-client` on an existing package
* Run `chef-client` and reroute logging to a logging mechanism that is controllable by NLog
* (Coming Soon) Schedule `chef-client` to run on an interval
* (Coming Soon) Pause Chef runs on a node, because of emergency manual fixes or maintenance of policies
* (Coming Soon) Run as a service and receive operational commands through a REST API, thus enabling remote control
* (Coming Soon) Provide an easy way to see a simple status of what is happening with Chef on the machine (including history)

# Technology

cafe is written in C# with the .NET Core framework, which means it compiles into one folder and eventually will be available on any platform without any prerequisite installers.

This is an important distinction of cafe: there isn't a lot of fanfare with getting it installed. That means that since it's super simple to install, it can be easily copied into a machine with System Center or whatever you think of, and then it does the rest of the hard work.

It's also important that cafe not be written in ruby and be self-contained, because ruby itself is updated while chef is updated. Cafe, as a design constraint, must be completely separated from the applications it manages.

# Usage

## Run chef on demand

Run `chef-client`, redirecting logging to `cafe`. This currently only works on windows and assumes that `chef-client` is on your `PATH` environment variable.

```
cafe chef run
```

## View chef version

View the version of `chef-client` currently running:

```
cafe chef version
chef-client version: 12.17.44
```

## Download chef-client

Downloads chef client installer for the specified version to a staging directory.

```
cafe chef download 12.17.44
```

# Coming Soon
## Initialize

Adds cafe to the PATH environment variable, letting it run from anywhere.

```
cafe init
```

## Install or Upgrade chef

While chef isn't running, upgrade it using a staged or downloaded MSI.:

```
cafe chef install 12.17.44
```

or 

```
cafe chef upgrade 12.17.44
```

Both commands do the same thing.

# Future Use Cases

## Run cafe in server mode

```
cafe server
```

All other commands will call the server, which will keep things from running into each other. The server will listen on a port and have a REST API.

Server mode can output in JSON or in an easy to understand result

## View status of Chef runs on a machine

I want to run a simple command that will view the status of chef runs:

```
cafe status

cafe version 0.1
Running Chef every 30 minutes
Last Run: Failed - short error here
Last 10 Runs: 50% failed
Fully idempotent? No
```

## Roll logging

I want to log to a file and roll it every X days. To do this I should be able to provide an NLog config file:

```
cafe config logging with: loggingConfiguration.xml
```

## Pause chef on a machine

Lots of times, especially early on, a team wants to 'pause' chef. Instead of killing the chef infrastructure, let's do this explicitly:

```
cafe pause chef
```

This will mean the service still runs, but is in a paused state

## Unpause chef on a machine

When we're ready to go again, let's unpause it:

```
cafe unpause chef
```

## Schedule chef-client

I want to be able to easily schedule `chef-client` to run on a machine:

```
cafe schedule chef every: 30 minutes
```

## Register cafe to run as a service so it will run chef

In order for cafe to run chef on a scheduled interval, I want to be able to run it as a service:

```
cafe service register
cafe service start
cafe service stop
```

This will use the windows services mechanism in Windows, and linux service mechanism in linux.

# Other ideas

* include ETW on chef events
* Trigger chef to run remotely (lightweight push job), and with orchestration events
* All agents listen to a central server that provides direction on what to do
* When chef crashes, let's retry running it to avoid downtime
* Also run chef as a scheduled task (but still through cafe so you have the safety) - not sure about this one, should discuss more
* Register an event with the process to say shut down if Azure needs to reconfigure the box - needs more discussion
