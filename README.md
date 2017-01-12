# Installation
[Install](https://www.microsoft.com/net/core) .NET Core.

For development, I would recommend [Visual Studio Code](https://code.visualstudio.com/) with the [C# plugin](https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp).

# Build and run
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
CI is a work-in-progress...deploy directly to PWS:
```
richmond> cd backend/src
richmond/backend/src> dotnet restore
richmond/backend/src> dotnet publish
richmond/backend/src> cd ../..
richmond> cf login # target the 'richmond' space
richmond> cf push
```

# dotnet core resources
- [project.json](https://docs.microsoft.com/en-us/dotnet/articles/core/tools/project-json) (similar to package.json, Gemfile, Cargo.toml, build.gradle, *.csproj, ...)
- [.NET Core](https://docs.microsoft.com/en-us/dotnet/)
