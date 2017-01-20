# Cafe Demonstration

## Starting point

* Local VM running Windows 2016
* chrome, log4view, code installed
* Zip file of cafe build
* Chef Server account (manage.chef.io), client.rb, and validator for bootstrapping

## Bootstrapping

### Install Cafe on the node

Let's get it installed:

  - Copy files to a folder
  - No ruby, no .NET Framework, **no prerequisites**, can be targeted to any of [these targets](https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog#using-rids).
  - Run `cafe init` and reboot to add to PATH variable
  - Run `cafe service register` which adds cafe to services
  - Run `cafe service start` which starts the Server
  - Run `cafe status` to see output
  - View `http://localhost:59320/api/scheduler/status` - it's the same thing

### Control your node remotely
  * Won't demonstrate, but talk about:
  - update `client.json` on another machine to point at your node over port `59320`

### Install chef-client on the node 
  - Run `cafe chef version` and notice that it is not installed
  - Run `cafe chef download 12.16.42` - places it in the staging folder (this could be done by a chef cookbook as well)
  - Run `cafe chef install 12.16.42`
  - Run `cafe chef version` and notice that it is installed, show add/remove programs

### Bootstrap the node

On windows you have to have winrm open when you need just one thing. It's like opening the bank vault for a $20 withdrawal. This is better:

  - Run `cafe chef bootstrap policy: cafe-demo group: qa config: C:\temp\client.rb validator: C:\temp\validator.pem`
  - Run `cafe chef run`
    - view the chef-specific run on the Server

## Logging

We want to operate chef with a windows mindset for easier adoption:

* View `nlog-server.config` as context
* Chef logging to its own file, rolled
* NLog controlling the level, pattern, etc.
* Rest of server that is consumable by a log4X log viewer, like Log4View
* Run `cafe status` and notice that the runs are in the history

## Scheduling

* Schedule Chef to run every 30 seconds, defined in `server.json`
* Run `cafe service stop`
* Run `cafe service start`
* Easily view the schedule through the `cafe status` command
* Everything scheduled, including the ad-hoc things, are run **serially**. That means no conflicts!
* Keep running `cafe status` and notice what's happening. View the logs for a deeper view.

## Pausing

People are going to pause Chef. Why not give them an easy way to do it so you can track it?

* Pause chef with `cafe chef pause`
* Run `cafe chef run`
* Run `cafe status`. Notice that the task is queued but not run yet since it is paused.
* Go to `http://localhost:59320` and see that it is paused
* Run `cafe chef resume` and see that chef starts running

## Upgrading

We want to upgrade chef outside of running chef, or ruby for that matter. Upgrade should be something separate:

* Run `cafe chef download 12.17.44`
* Run `cafe chef upgrade 12.17.44`

We know that chef won't be running, because if it was, the upgrade would wait! This is a much safer upgrade process.
