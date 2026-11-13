# Employee Management API

A RESTful backend application developed with **ASP.NET Core 10**, **Entity Framework Core 10**, and **SQL Server 2022** as part of the Entersoftone Backend Developer Technical Assessment.

The application provides employee, department, and project management functionality, including CRUD operations, search, pagination, validation, and entity relationships.

## Technology Stack

- **ASP.NET Core 10** — ASP.NET Core Web API
- **Entity Framework Core 10** — ORM and database migrations
- **SQL Server 2022** — Relational database
- **Swagger UI / OpenAPI** — API documentation and testing
- **xUnit v3** — Automated testing
- **SQLite In-Memory** — Isolated relational integration tests

## Architecture

The solution follows a layered architecture inspired by Clean Architecture, separating domain models, application logic, database infrastructure, and API endpoints. The Application layer uses EF Core abstractions through `IApplicationDbContext`; it does not depend on the Infrastructure project.

```text
Entersoftone_Assessment
├── Entersoftone_Assessment.Api
│   ├── Controllers
│   ├── ExceptionHandling
│   └── Program.cs
│
├── EmployeeManagement.Application
│   ├── DTOs
│   ├── Exceptions
│   ├── Interfaces
│   └── Services
│
├── EmployeeManagement.Domain
│   ├── Entities
│   └── Enums
│
├── EmployeeManagement.Infrastructure
│   ├── Configurations
│   ├── Persistence
│   ├── Migrations
│   └── Seeder
│
└── EmployeeManagement.Tests
    ├── Helpers
    └── Services
```

### Layer Responsibilities

**Domain**
- Defines core business entities and enumerations.
- Contains no direct dependency on database infrastructure.

**Application**
- Implements employee, department, and project management services.
- Defines request and response DTOs.
- Contains application interfaces, validation, and business exceptions.

**Infrastructure**
- Implements database persistence using Entity Framework Core.
- Defines entity relationships and constraints through Fluent API.
- Manages database migrations and development seed data.

**API**
- Exposes RESTful endpoints through ASP.NET Core Controllers.
- Configures dependency injection and middleware.
- Provides OpenAPI documentation and centralized exception handling.

## Domain Model

The application manages three core entities:

### Employee

Represents an employee with personal information, employment status, hire date, and optional notes.

Each employee belongs to one department and may participate in multiple projects.

### Department

Represents an organizational department containing multiple employees.

A department cannot be deleted while employees are assigned to it.

### Project

Represents a project with a name, optional description, start date, and optional end date.

Projects can have multiple assigned employees.

### Entity Relationships

- **Department → Employees:** One-to-Many
- **Employee ↔ Project:** Many-to-Many

The many-to-many relationship is implemented through the `EmployeeProjects` join table.

Deleting an employee or project removes the corresponding relationship records without deleting unrelated entities.

## API Endpoints

### Employees

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Employees` | Retrieve employees with search and pagination |
| GET | `/api/Employees/{id}` | Retrieve an employee by ID |
| POST | `/api/Employees` | Create an employee |
| PUT | `/api/Employees/{id}` | Update an employee |
| DELETE | `/api/Employees/{id}` | Delete an employee |

**Employee search parameters**

- `search` — Optional search across first name, last name, and email.
- `page` — Page number, starting from 1.
- `pageSize` — Number of records per page, up to 100.

### Departments

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Departments` | Retrieve all departments |
| GET | `/api/Departments/{id}` | Retrieve a department by ID |
| POST | `/api/Departments` | Create a department |
| PUT | `/api/Departments/{id}` | Update a department |
| DELETE | `/api/Departments/{id}` | Delete a department |

Department responses also include the number of assigned employees.

### Projects

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Projects` | Retrieve all projects |
| GET | `/api/Projects/{id}` | Retrieve a project by ID |
| POST | `/api/Projects` | Create a project |
| PUT | `/api/Projects/{id}` | Update a project |
| DELETE | `/api/Projects/{id}` | Delete a project |

Project responses include the number of assigned employees.

## Validation and Error Handling

Request validation is implemented using Data Annotations and `IValidatableObject`.

Validation rules include:

- Required employee names and valid email addresses.
- Unique employee email addresses.
- Valid department and project references.
- Unique department names.
- Valid project date ranges.
- Field length constraints.

The API uses centralized exception handling with ASP.NET Core `IExceptionHandler` and standardized Problem Details responses.

| HTTP Status | Description |
|---|---|
| 200 OK | Successful retrieval |
| 201 Created | Resource created |
| 204 No Content | Resource updated or deleted |
| 400 Bad Request | Request validation failed |
| 404 Not Found | Resource not found |
| 409 Conflict | Business constraint violation |
| 500 Internal Server Error | Unexpected server error |

## Getting Started

### Quick Start with Docker (recommended)

Docker Compose runs the **.NET 10 API** and **SQL Server 2022** together. You do not need to install SQL Server or configure a database connection manually.

Prerequisite: Docker Desktop (or an equivalent Docker Engine with Compose support).

```bash
git clone https://github.com/xanthosgeorge/Entersoftone_Assessment.git
cd Entersoftone_Assessment
cp .env.example .env
```

On Windows PowerShell, use `Copy-Item .env.example .env` instead of `cp`.

Open the new `.env` file and replace the example value of `MSSQL_SA_PASSWORD` with a **strong, unique password for your local development database**. Never commit `.env`; it is excluded by `.gitignore`. The password is used by Docker Compose to configure both SQL Server and the API, so there is no need to share credentials.

```bash
docker compose up --build -d
docker compose ps
```

When the database is healthy and the API is running, open **http://localhost:8080/swagger**. In the Development environment, the application applies the included EF Core migrations and seeds sample records automatically.

Useful commands:

```bash
docker compose logs --tail=100 api
docker compose down
```

`docker compose down` preserves the database volume. `docker compose down -v` **deletes the development database volume and its data**.

**Apple Silicon:** The official SQL Server 2022 container is an x64 image and may require amd64 emulation; that setup is not officially supported by Microsoft. It was tested for this assessment but is not guaranteed to work on every ARM machine. See [Docker setup details](DOCKER_SETUP.md).

### Run Locally without Docker

Prerequisites: .NET 10 SDK, a compatible SQL Server instance, and EF Core CLI tooling (if running the migration command manually).

Configure the `DefaultConnection` setting for your **own local SQL Server instance**, preferably through `.NET User Secrets` or an environment variable instead of committing machine-specific settings or credentials. Database connection details are intentionally not included in this README.

From the solution directory:

```bash
dotnet ef database update --project EmployeeManagement.Infrastructure --startup-project Entersoftone_Assessment.Api
dotnet run --project Entersoftone_Assessment.Api --launch-profile https
```

The Development environment also applies migrations automatically during application startup. Swagger UI is available at **https://localhost:7020/swagger** when using the included HTTPS launch profile.

### Development Seed Data

The first Development startup initializes sample data:

- 3 departments: IT, Human Resources, and Finance
- 6 sample employees
- 3 sample projects and their employee assignments

Seeding checks existing records to prevent duplicates. Deleted sample records may be recreated on subsequent Development startups.

## Postman Collection

Import [`postman/Entersoftone_Assessment.postman_collection.json`](postman/Entersoftone_Assessment.postman_collection.json) into Postman.

Set the collection-level `baseUrl` variable to `http://localhost:8080` for Docker, or `https://localhost:7020` when using the local HTTPS launch profile.

The collection contains **15 CRUD requests** and **3 additional negative test cases**. Run the folders in order: **01 - Setup / Create**, **02 - Employees**, **03 - Projects**, **04 - Departments**, then optionally **05 - Negative Tests**.

The create requests save `departmentId`, `projectId`, and `employeeId` to collection variables. Subsequent requests reuse those IDs for GET, PUT, and DELETE. The sample workflow operates on records created by the collection rather than deleting seeded records.


## Automated Testing

The solution includes 8 automated tests using **xUnit v3** and **SQLite In-Memory**, covering business rules, service operations, and relational database behavior.

### Test Runner

The project uses **Microsoft.Testing.Platform**, configured through `global.json`, to support test execution with the .NET 10 SDK.

Tests can be executed directly from the solution directory:

```bash
dotnet test
```

All 8 tests run against isolated SQLite in-memory databases without requiring a SQL Server instance.

### Covered Scenarios

- Preventing duplicate department names.
- Preventing deletion of departments with assigned employees.
- Allowing deletion of departments without employees.
- Preventing duplicate employee emails.
- Rejecting nonexistent department references.
- Updating employee-to-project relationships.
- Rejecting invalid project date ranges.
- Preserving employees when assigned projects are deleted.

### Run Tests

From the solution directory, `dotnet test` runs the suite using the `global.json` Microsoft.Testing.Platform configuration. Tests use isolated temporary SQLite databases and do not modify the development SQL Server database.

## Design Decisions

**Fluent API Configuration**

Entity mappings are organized in dedicated `IEntityTypeConfiguration<T>` implementations to keep the DbContext focused and maintainable.

**Dependency Injection**

Application services depend on interfaces rather than concrete infrastructure implementations.

**DTO Separation**

Request and response DTOs prevent direct exposure of persistence entities through the API.

**EF Core Query Optimization**

Read operations use `AsNoTracking()` and DTO projections to avoid unnecessary entity tracking and data loading.

**Centralized Exception Handling**

Application exceptions are translated into consistent HTTP Problem Details responses.

**Relational Integration Testing**

SQLite In-Memory provides relational behavior during automated tests without requiring an external database server.

## Future Improvements

Potential enhancements include:

- Enhanced pagination responses with total record counts.
- Additional automated test coverage.
- Soft delete and audit logging.
- Authentication and role-based authorization.

## Assessment

Developed as a technical assessment for **Entersoftone**, demonstrating REST API design, relational data modeling, Entity Framework Core, layered architecture, validation, and automated testing.