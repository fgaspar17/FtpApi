# FtpApi

**FtpApi** is a .NET 9 Web API for securely uploading, downloading, and deleting files via an FTP server. 
It leverages modern .NET features like Minimal APIs, JWT authentication, and integration with SQLite, 
FluentValidation, and Serilog. The project is fully containerized with Docker and demonstrates robust 
practices in error handling, testing, and file management.

## Tech Stack

- **.NET 9 Minimal API**
- **Entity Framework Core** (with SQLite)
- **Docker & Docker Compose**
- **FluentFTP** (FTP client)
- **FluentValidation** (input validation)
- **JWT Authentication**
- **Serilog** (structured logging)
- **TestContainers** (for integration testing)
- **Scalar** (OpenAPI documentation)

## Goals

  - [x] Learn how to work with the FTP protocol in .NET  
  - [x] Gain experience with Docker and container networking  
  - [x] Implement JWT authentication and secure APIs  
  - [x] Explore error handling and validation techniques  
  - [x] Write integration and unit tests for a file-based API  

## Features

- File management via FTP

  - Upload, download, and delete files from an FTP server.
  - Files are physically stored on an FTP server via `FluentFTP`.
  - Metadata (filename, description, hash, owner) is stored in a **SQLite** database.

- Authorization & Authentication

  - All file endpoints (`/api/file`) are secured using **JWT tokens**.
  - Login via `/api/login` returns a signed token after validating user credentials.

- Validation

  - Inputs are validated using **FluentValidation**.
  - Validation includes file extension, size (max 5MB), and file name constraints.

 - Error Handling

  - A centralized error endpoint returns structured `ProblemDetails` objects.
  - Custom exceptions (e.g., `CustomValidationException`) are handled gracefully.

- Containerized Environment

  - A custom FTP server is built using **Alpine Linux** in Docker.
  - The API and FTP server run in isolated containers with network access.

- Scalar Documentation

  - ![image](https://github.com/user-attachments/assets/4dab6fbe-30d1-4f9d-a78e-9765e6b1df4d)

## Testing

- Integration testing with **TestContainers** for dynamic FTP server.
- Unit testing with **xUnit**.
- FluentValidation is decoupled from Entity Framework, allowing isolated unit tests.

## Challenges

- Managing FTP server setup and teardown for tests.
- Managing authentication using JWT.
- Handling file stream uploads and downloads properly.
- Validate data. I decided to use FluentValidation for not database related data, so I can unit test this part.
- Launch a FTP Server, I decided to create my own Dockerfile using Alpine Linux.
- Designing a custom error response structure.
- Coordinating Docker networking between API and FTP containers.

## Lessons Learned

- Writing Dockerfiles, managing volumes and networks.
- Using `.HasQueryFilter()` for soft deletes in EF Core.
- Structuring logging via Serilog and reading config from `appsettings.json`.
- Implementing a custom `Middleware` and `ExceptionHandler`.
- Understanding file streaming in .NET via `Stream`.

## Areas to Improve

- Improve REST API design consistency (e.g., status codes and route naming).
- Expand test coverage (unit tests and edge case validations).
- Explore file storage alternatives like AWS S3 or Azure Blob Storage.
- Enhance Serilog configuration (e.g., file logging, enrichment, sinks).
- Investigate container orchestration with Docker Compose or Kubernetes.

## Resources used

- StackOverflow posts
- ChatGPT
- [Global Error Handling Video](https://www.youtube.com/watch?v=B5NsgtdwOlg&list=WL)
- [Serilog Logging Video](https://www.youtube.com/watch?v=nVAkSBpsuTk)
- [Docker Documentation](https://docs.docker.com/manuals/)
- [TestContainers .NET Documentation](https://dotnet.testcontainers.org/)
- [Rethrowing Exceptions Video](https://www.youtube.com/watch?v=TCiL3y2P4rA)
- [FluentFTP examples](https://github.com/robinrodricks/FluentFTP/tree/master/FluentFTP.CSharpExamples)
