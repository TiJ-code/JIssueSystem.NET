# Installation
## Beforehand
You need to authenticate yourself to GitHub.
```
dotnet nuget add source \
  --username YOUR_USERNAME \
  --password YOUR_GITHUB_PAT \
  --store-password-in-clear-text \ # optional
  --name github \
  "https://nuget.pkg.github.com/TiJ-code/index.json"
```

## Actual
Install package
```
dotnet add package JIssueSystem.NET --version 0.1.0
```