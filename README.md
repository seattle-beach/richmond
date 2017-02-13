# Richmond
https://richmond.cfapps.io/

[Pivotal Tracker](https://www.pivotaltracker.com/n/projects/1948575)

[Google Drive](https://drive.google.com/drive/folders/0Bz7GxM1Uu1OyLTNYT19IRWU5VGM)

# Installation
[Install](https://www.microsoft.com/net/core) .NET Core.

For development, I would recommend [Visual Studio Code](https://code.visualstudio.com/) with the [C# plugin](https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp).

# Build and run locally
### Environment variables
Our workstation image includes [direnv](https://direnv.net/), so you should just be able to add these to a file named `.envrc` in the root of the project.

```bash
FOODTRUCK_PATH=http://www.seattlefoodtruck.com/schedule/occidental-park-food-truck-pod/
```

### Populate local dependency cache
`richmond> dotnet restore`

### Run backend tests:
```
richmond> dotnet test test-backend
```

### Run frontend tests:
With PhantomJS on the commandline: (if phantomJs is not installed first run `brew install phantomjs`)
```
richmond> cd test-frontend
richmond/test-frontend> phantomjs run-jasmine.js SpecRunner.html
```

Or, open `richmond/test-frontend/SpecRunner.html` in a JS-enabled web browser.

### Run server:
The default location is http://localhost:5000
```
richmond> cd src
richmond/src> dotnet run
```

### Custom scripts
We also include some custom scripts at the root, which you can install with:
```
richmond> ./scripts/install.sh
```

# Deployment
The app is deployed in the `seattle-beach` PWS org, space name `richmond`. We have a deployment service account `alidaka+richmond+cf@pivotal.io`; ask another beach person for the password.

To `cf push` or update the CI pipeline, environment variables `CF_PASSWORD` and `CF_USERNAME` must be set. We're using `direnv` to manage these, but anything is fine.

### CI
We're currently using the monitor between Lovelace and Hopper as our Concourse CI server.
In order to view the CI pipeline simply visit [http://10.37.2.27:8080](http://10.37.2.27:8080) click the login button in the upper right corner and select the 'main' pipeline.

Updating CI:

1. Connect to the machine: `vnc://10.37.2.27` or `ci1.local`
2. See the [vagrant installation instructions](https://concourse.ci/vagrant.html)
3. On the Concourse host, uncomment (or add a line to) the Vagrantfile like:
```ruby
config.vm.network "forwarded_port", guest: 8080, host: 8080
```
4. Visit [http://10.37.2.27:8080](http://10.37.2.27:8080) and login, then download `fly`
5. `richmond/concourse> ./update-concourse.sh`

Optionally, start vagrant on Mac reboot (note: may depend on VB/vagrant version? Currently does not work...):
```bash
richmond/concourse> ln -s vagrant.startup.plist ~/Library/LaunchAgents/vagrant.startup.plist
richmond/concourse> launchctl load -w ~/Library/LaunchAgents/vagrant.startup.plist
```

In order to update the concourse pipelines, you'll need to set the `TRACKER_API_TOKEN` environment variable. You can get it from the Profile page on the seattle-beach@pivotal.io user on [pivotaltracker.com](https://www.pivotaltracker.com)

# dotnet core resources
- [project.json](https://docs.microsoft.com/en-us/dotnet/articles/core/tools/project-json) (similar to package.json, Gemfile, Cargo.toml, build.gradle, *.csproj, ...)
- [.NET Core](https://docs.microsoft.com/en-us/dotnet)
