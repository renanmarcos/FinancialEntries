# Financial Entries API

Made with .NET Core 3.0, Google Cloud Firestore and Google App Engine

## Instructions to run locally

- Install and configure [.NET Core SDK](https://dotnet.microsoft.com/download) according your platform
- Download this repository and run in terminal:

```sh
cd Src/
dotnet restore
dotnet run
```

In terminal will be available the API port and location (in general is `http://localhost:5000`)

### Optionally, run with Docker

If you prefer, you can run this project in Docker container.

- Install and configure [Docker Engine Community](https://docs.docker.com/install/) according your platform
- Download this repository and run in terminal:

```sh
cd Src/
docker build -t app .
docker run -d -p 8080:8080 --rm --name api app
```

Server will be running at `http://localhost:8080`

## Running tests

```sh
cd Tests/
dotnet restore
dotnet test
```

## Documentation

While running the project, you can access the Swagger UI at `/docs` endpoint. This will show all API resources, properties and responses.

- Insomnia Workspace is available at `Src/insomnia.json` as alternative.

## See in action running into Google Cloud Platform

An instance of Google App Engine is running for this project at: http://financial-entries-259514.appspot.com/
- Documentation: http://financial-entries-259514.appspot.com/docs

### Notes

- SSL Certification is disabled to be easier to test, but in real production scenario need to be activated for security reasons.
- `credentials.json` is a GCP limited access to write/read Firestore information. Need to remove from the repository in real scenarios, it's here just to made our life easier.