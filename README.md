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

After installation, let's work on getting chef bootstrapped on the machine. The first step is to download the [Chef Client](https://docs.chef.io/ctl_chef_client.html):

```
cafe chef download 12.16.42
```

Once the Chef Client is downloaded, let's install it:

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

# Other ideas

Here are some other ideas about what can be done with cafe from demos and discussions:

* working with proxy servers during download
* overridable download location for internal networks
* include ETW on chef events
* Trigger chef to run remotely (lightweight push job), and with orchestration events
* All agents listen to a central server that provides direction on what to do
* When chef crashes, let's retry running it to avoid downtime
* Register an event with the process to say shut down if Azure needs to reconfigure the box - needs more discussion
