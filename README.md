# Financial Entries API

Made with .NET Core 3.0, Google Cloud Firestore and Google App Engine

## Instructions to run locally

- Install and configure [.NET Core SDK](https://dotnet.microsoft.com/download) according your platform
- Download this repository and run in terminal:

```sh
dotnet restore
dotnet run
```

In terminal will be available the API port and location. Normally is ´http://localhost:5000´

## Documentation

While running the project, you can access the Swagger UI at ´/docs´ endpoint.

Insomnia Workspace is available at "Insomnia-Workspace.json" as alternative.
Postman Workspace is available at "Postman-Workspace.json" as alternative.

## See in action inside the Cloud environment

An instance of Google App Engine is running for this project at: 

### Notes

- SSL Certification is disabled to be easier to test, but in real production scenario need to be activated for security reasons.
- "credentials.json" is a GCP limited access to write/read Firestore information. Need to remove from the repository in real scenarios, is here just to made our life easier.