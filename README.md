# Employee Management API

A RESTful backend application developed with **ASP.NET Core 10**, **Entity Framework Core 10**, and **SQL Server 2022** as part of the Entersoftone Backend Developer Technical Assessment.

The application provides employee, department, and project management functionality, including CRUD operations, search, pagination, validation, and entity relationships.

## Technology Stack

- **.NET 10** — ASP.NET Core Web API
- **Entity Framework Core 10** — ORM and database migrations
- **SQL Server 2022** — Relational database
- **Swagger UI / OpenAPI** — API documentation and testing
- **xUnit v3** — Automated testing
- **SQLite In-Memory** — Isolated relational integration tests

## Architecture

The solution follows a layered architecture inspired by Clean Architecture, separating domain models, application logic, database infrastructure, and API endpoints.

```text
Entersoftone_Assessment
├── EmployeeManagement.Api
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
│   └── Seeding
│
└── EmployeeManagement.Tests
    ├── Helpers
    ├── Services
    └── Validation
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

### Prerequisites

- .NET 10 SDK
- SQL Server 2022 or another compatible SQL Server instance
- Visual Studio with ASP.NET Core development support, or a compatible IDE
- EF Core tooling for database migrations

### 1. Clone the Repository

```bash
git clone <repository-url>
cd Entersoftone_Assessment
```

Replace `<repository-url>` with the GitHub repository URL.

### 2. Configure the Database Connection

Configure `DefaultConnection` in `EmployeeManagement.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLSERVER2022;Database=EntersoftoneAssessmentDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Update the server name and authentication settings according to your local SQL Server configuration.

For environments requiring credentials, use environment variables or .NET User Secrets rather than committing passwords to source control.

### 3. Apply Database Migrations

From the solution directory, run:

```bash
dotnet ef database update --project EmployeeManagement.Infrastructure --startup-project EmployeeManagement.Api
```

This creates or updates the database using the included EF Core migrations.

### 4. Run the API

```bash
dotnet run --project EmployeeManagement.Api --launch-profile https
```

Open the Swagger UI at the HTTPS address configured in `launchSettings.json`, followed by `/swagger`.

For example:

```text
https://localhost:7020/swagger
```

The Swagger UI and development database seeding are enabled in the Development environment.

### 5. Development Seed Data

On startup in Development, the application initializes sample data if required.

The initial dataset includes:

- 3 departments: IT, Human Resources, and Finance.
- 6 sample employees.
- 3 sample projects.
- Employee-to-project assignments.

Seeding checks existing records to avoid duplicate seed data during subsequent executions.

## Automated Testing

The solution includes automated tests using **xUnit v3** and **SQLite In-Memory**.

Tests focus on business rules, service operations, and relational behavior.

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

From the solution directory:

```bash
dotnet test EmployeeManagement.Tests/EmployeeManagement.Tests.csproj
```

Tests use isolated temporary SQLite databases and do not modify the development SQL Server database.

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
- Docker Compose for simplified database setup.
- Soft delete and audit logging.
- Authentication and role-based authorization.

## Assessment

Developed as a technical assessment for **Entersoftone**, demonstrating REST API design, relational data modeling, Entity Framework Core, layered architecture, validation, and automated testing.