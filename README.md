# cafe

Cafe makes operating chef easy and delightful.

Chef is a world-class configuration management platform that, for many of us, is a gateway into automation that was previously unimaginable. However, like many of its siblings, it started out within a linux environment. As a result, running and maintaining it is not straightforward when you come from a Windows background.

# Installation

Cafe is a standalone program that is fully operational by unzipping files into a folder and running `cafe.exe`. No ruby or .NET dependencies. It just works.

To install:

1. Unzip the installation package into a folder
2. Run `cafe init` if you want it added to the path (you'll need to reboot)
3. Run `cafe service register` to have the cafe server run in the background so it can do things for you

# Runtime

Cafe is lightweight. To run the service it takes 17.8MB of memory and no CPU. This means that you can put cafe on all your nodes, then install and run chef as you want to.

# Walkthrough

After installation, let's work on getting chef bootstrapped on the machine. 

The first step is to download and install [inspec](http://inspec.io/):

```
cafe inspec download 1.7.1
```

Once the inspec installer is downloaded, let's install it:

```
cafe inspec install 1.7.1
```

Next we will do the same with the [Chef Client](https://docs.chef.io/ctl_chef_client.html):

```
cafe chef download 12.16.42
```

And then install it:

```
cafe chef install 12.16.42
```

Now that we've installed Chef, let's bootstrap it. You can do this two ways:

1. [The Policyfile](http://hedge-ops.com/policyfiles/) way:

```
cafe chef bootstrap policy: webserver group: qa config: C:\Users\mhedg\client.rb validator: C:\Users\mhedg\my-validator.pem
```

2. The Run List Way:

```
cafe chef bootstrap run-list: "[chocolatey::default]" config: C:\Users\mhedg\client.rb validator: C:\Users\mhedg\my-validator.pem
```

Both ways ask for a config file that will be your `client.rb` on the machine and a validator used to ask the chef server for validation.

Now that we've bootstrapped Chef, we can run it again on demand if we want to:

```
cafe chef run
```

We can even look at the `logs` directory and see that we have a rolling log that only has our chef-client runs in it. We can also see specific logging for our client and server.

We probably want to schedule Chef to run every 30 minutes or so. To do this we edit our `server.json`:

```json
{
    "ChefInterval": 1800,
    "Port": 59320
}
```   

And restart the cafe service:

```
cafe service restart
```

At some point you may even want to pause chef on the node so you can manually check a node's state without fear of Chef changing anything. To do this, run:

```
cafe chef pause
```

And then when you're ready to rejoin the land of sanity, you can simply run:

```
cafe chef resume
```
# Running Remotely

You can either run cafe remotely through Powershell Remoting (where you invoke cafe as a locally run process) or by using the `on: servername` syntax at the end of commands above. You'll need to make sure you have networking set up for it if you go the latter route.
# Other ideas

Here are some other ideas about what can be done with cafe from demos and discussions:

* working with proxy servers during download
* overridable download location for internal networks
* include ETW on chef events
* Trigger chef to run remotely (lightweight push job), and with orchestration events
* All agents listen to a central server that provides direction on what to do
* When chef crashes, let's retry running it to avoid downtime
* Register an event with the process to say shut down if Azure needs to reconfigure the box - needs more discussion

# Upgrading Cafe

## The Simple Way

To upgrade cafe, simply stop the service with `cafe service stop`, copy all binaries into your cafe installation, then start the service with `cafe service start`.

## The Complicated Way

The problem with this is that it's difficult to automate. If you try to automate this process from within cafe itself, it can't because it can't stop itself.

Enter the `cafe.Updater`. This application is completely separate from `cafe` and so can update it without endangering the cafe application itself.

Here's how it works:

1. `cafe.Updater` is installed with `cafe` itself in a folder called `updater`.
2. Run `cafe.Updater` as a service by running the commands `cafe.Updater service register` and `cafe.Updater service start`
3. Download a particular update of `cafe` by running `cafe download 0.8.0`. This will stage your cafe zip file in your local `staging` folder
4. Now run `cafe install 0.8.0`. This will:
  * Copy the cafe zip install file to the `updater/staging` folder
  * `cafe.Updater` will notice this file arrived and will start its update
  * `cafe.Updater` will wait 30 seconds so the install can finish replying back to its client
  * `cafe.Updater` stops the `cafe` service
  * `cafe.Updater` unzips the file to the parent directory (the `cafe` installation directory)
  * `cafe.Updater` starts the `cafe` service

[Note: this is not implemented yet.]
To update the `cafe.Updater` itself, you'll need to follow these steps:

1. Run `cafe updater download 0.8.0`. This will be the `cafe` version and will stage the same exact file. In fact `cafe download 0.8.0` and `cafe updater 0.8.0` should do the same thing
2. Run `cafe updater install 0.8.0`. This will:
  * Unzip the cafe installation to a temporary location
  * Stop the `cafe.Updater` service by running `cafe.Updater service stop`
  * Copy all files from `updater` inside of the install package into the `updater` folder
  * Start the `cafe.Updater` service by running `cafe.Updater service start`

## The Easy Way

[Note that this is currently in progress]

The `cafe` [cookbook](https://github.com/mhedgpeth/cafe-cookbook) will handle all of this for you. In fact, this complexity exists so that you can manage cafe during a chef run while also avoiding updating yourself while you're running.

# Development

Cafe is built with .NET Core SDK 1.0.3 using cake.