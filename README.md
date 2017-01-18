# Richmond
https://richmond.cfapps.io/

[Pivotal Tracker](https://www.pivotaltracker.com/n/projects/1948575)

[Google Drive](https://drive.google.com/drive/folders/0Bz7GxM1Uu1OyLTNYT19IRWU5VGM)

# Installation
[Install](https://www.microsoft.com/net/core) .NET Core.

For development, I would recommend [Visual Studio Code](https://code.visualstudio.com/) with the [C# plugin](https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp).

# Build and run locally
### Populate local dependency cache
`richmond/backend> dotnet restore`

### Run tests:
```
richmond/backend> cd test
richmond/backend/test> dotnet test
```

### Run server:
The default location is http://localhost:5000
```
richmond/backend> cd src
richmond/backend/src> dotnet run
```

# Deployment
The app is deployed in the `seattle-beach` PWS org, space name `richmond`. We have a deployment service account `alidaka+richmond+cf@pivotal.io`; ask another beach person for the password.

To `cf push` or update the CI pipeline, environment variables `CF_PASSWORD` and `CF_USERNAME` must be set. We're using `direnv` to manage these, but anything is fine.

### CI
We're currently using the monitor between Lovelace and Hopper as our Concourse CI server.

1. Connect to the machine: `vnc://10.37.2.27` or `ci1.local`
2. See the vagrant installation instructions [here](https://concourse.ci/vagrant.html)
3. On the Concourse host, uncomment (or add a line to) the Vagrantfile like:
```
config.vm.network "forwarded_port", guest: 8080, host: 8080
```
4. Visit http://10.37.2.27:8080, download `fly`
5. `richmond/concourse> ./update-concourse.sh`

# dotnet core resources
- [project.json](https://docs.microsoft.com/en-us/dotnet/articles/core/tools/project-json) (similar to package.json, Gemfile, Cargo.toml, build.gradle, *.csproj, ...)
- [.NET Core](https://docs.microsoft.com/en-us/dotnet)
