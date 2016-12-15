# cafe

Running and managing chef, especially within a windows machine is difficult. In order to get up and running, you have to install the chef client application. Doing so with the chef-client application doesn't work. 

In addition to this, you have to manage the schedule of the chef client. Also, many people in the windows world want to run chef as a service, but it's not clear how to do so and maintain the schedule of it.

Upgrading chef with the chef-client way is also difficult in windows; it's not natural for an application to upgrade itself in windows due to how windows manages its files.

Finally getting status from chef isn't straightforward from a windows perspective. People want to check something for status, but that's not available. In addition, the logs don't rotate like they would on a normal windows/.NET application.

So I propose a new application: cafe. This application will be a self-contained executable written in C# against the .NET Core and will provide easy maintenance of chef.

# Usage

## Run chef on demand

Run `chef-client`, redirecting logging to `cafe`. This currently only works on windows and assumes that `chef-client` is on your `PATH` environment variable.

```
cafe chef run
```

# Coming Soon

## View chef version

View the version of `chef-client` currently running:

```
cafe chef version
Chef: 12.17.44
```

## Upgrade chef

While chef isn't running, upgrade it:

```
cafe chef upgrade with: C:\temp\chef-client-12.6.9.msi
```

# Future Use Cases

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



# Other ideas

* include ETW on chef events
* Trigger chef to run remotely (lightweight push job), and with orchestration events
* When chef crashes, let's retry running it to avoid downtime
* Also run chef as a scheduled task (but still through cafe so you have the safety) - not sure about this one, should discuss more
* Register an event with the process to say shut down if Azure needs to reconfigure the box - needs more discussion
