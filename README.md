# Content Sharing Platform

A modular **.NET 9 Web API** for managing, rating, and sharing digital media content.  
Built with a **clean architecture** approach separating Domain, Application, Infrastructure, and API layers and featuring **JWT authentication**, **Entity Framework Core with PostgreSQL**, **automated testing** and  full **Docker** support.

---

## 🏗️ Project Architecture

```
ContentShare.sln
├── ContentShare.API             # ASP.NET Core Web API layer (controllers, middlewares, DI)
├── ContentShare.Application     # Business logic (services, DTOs, validators, interfaces)
├── ContentShare.Domain          # Entities and core domain models
├── ContentShare.Infrastructure  # EF Core, Identity, repositories, and external services
└── ContentShare.Tests           # Integration & unit tests (xUnit + WebApplicationFactory)
```

Each layer communicates only through well-defined interfaces  ensuring testability and scalability.

---

## ✨ Features

### 🧠 Content Management
- CRUD endpoints for media content (Create, Read, Update, Delete)
- Pagination, filtering, and sorting support
- FluentValidation for input validation

### 👤 User Management
- User registration and login with ASP.NET Core Identity
- JWT authentication (Bearer tokens)
- Role-based authorization (Admin, User)

### ⭐ Rating System
- Authenticated users can rate media content (1–5 stars)
- Optional comments on reviews
- Average ratings calculated dynamically

### 🚨 Reporting System
- Users can report inappropriate or abusive reviews.
- Supported reasons include:
  - `Spam`
  - `Harassment`
  - `HateSpeech`
  - `Inappropriate`
  - `Other`
- When a review reaches **10 reports**, it is **automatically deleted**.

### 🧠 Validation Layer
All requests are validated through **FluentValidation**.  
Implemented validators include:
- `ContentCreateDtoValidator`
- `ContentUpdateDtoValidator`
- `PagedRequestValidator`
- `RegisterRequestValidator`
- `LoginRequestValidator`
- `RatingCreateDtoValidator`
- `ReportCreateDtoValidator`

### 🧰 Swagger Documentation
- Integrated **Swagger UI** for endpoint testing.
- **Example providers** for realistic request/response payloads under `SwaggerExamples/`.
- Secure endpoints documented with JWT authentication headers.
- Automatically loaded when running the API locally.

### 🧱 Persistence Layer
- **PostgreSQL** for production.
- **InMemoryDatabase** for tests.
- All entities managed with **Entity Framework Core**.
- Automatic migrations and seeding on startup.

### 🧪 Testing
- Fully integrated tests using `WebApplicationFactory` and **xUnit**.
- Test coverage includes:
  - User registration and login (Auth)
  - Content CRUD
  - Rating logic and averages
  - Reporting flow and auto-deletion trigger
- Tests run against an isolated **InMemory EF database**.

### 🧠 Architecture Principles
- **Clean Architecture** (Domain-Driven Design inspired)
- Clear separation between:
  - `API` (Presentation)
  - `Application` (Business Logic & DTOs)
  - `Infrastructure` (Persistence, Repositories, Identity)
  - `Domain` (Entities, Enums)
- **Dependency Injection** for all layers.
- Single responsibility per service and repository.

### ⚙️ Additional
- Centralized exception handling middleware
- Entity Framework Core migrations
- Sentry integration for error tracing
- Full integration testing suite

---

## 🛠️ Tech Stack

| Layer | Technologies |
|-------|---------------|
| API | ASP.NET Core 9, Swagger / OpenAPI |
| Application | FluentValidation, DTOs, Service Interfaces |
| Infrastructure | EF Core, Identity, PostgreSQL, JWT Auth |
| Domain | Clean entities & base abstractions |
| Tests | xUnit, FluentAssertions, WebApplicationFactory |
| DevOps | Docker, Docker Compose, Sentry |

---

## 🚀 Getting Started

### 1. Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/)
- [Docker & Docker Compose](https://www.docker.com/)
- Optional: [pgAdmin](https://www.pgadmin.org/) or any SQL client for PostgreSQL

---

### 2. Environment Configuration

The default configuration lives in `appsettings.json` and `docker-compose.yml`.

**Example (local run):**

```json
"ConnectionStrings": {
  "Default": "Host=localhost;Port=5432;Database=contentshare;Username=postgres;Password=root"
},
"Jwt": {
  "Key": "QpKQq7nQv2E9u5mZcYhT4xJ6Wm9a2s5d8f0r3u6w9z1B4E7H0K3N6Q9T2W5Z8C1",
  "Issuer": "ContentShare",
  "Audience": "ContentShareClients"
}
```

---

### 3. Run Locally (Without Docker)

```bash
# Run EF migrations
dotnet ef database update --project ContentShare.Infrastructure --startup-project ContentShare.API

# Launch the API
dotnet run --project ContentShare.API
```

Navigate to **https://localhost:7016/swagger/index.html** for the Swagger UI.

---

### 4. Run via Docker Compose

```bash
docker compose build
docker compose up -d
```

Containers started:

| Service | Description | Port |
|----------|--------------|------|
| `contentshare_api` | ASP.NET Core API | 8080 |
| `contentshare_db` | PostgreSQL database | 5432 |

Swagger will be available at:  
👉 [https://localhost:7016/swagger/index.html](https://localhost:7016/swagger/index.html)


---

## 🧪 Running Tests

The test suite uses **InMemoryDatabase** and **WebApplicationFactory** for integration testing.

```bash
dotnet test
```

Tests cover:
- Authentication (Register, Login)
- CRUD operations for Content
- Rating flow and average calculation

---

## 🧑‍💻 Seeding

When the API starts, the database is automatically seeded with:

### Roles
- **Admin**
- **User**

### Default Users
| Username | Email | Role | Password |
|-----------|--------|------|-----------|
| joe | joe@test.com | Admin | P@ssw0rd1! |
| alice | alice@test.com | User | P@ssw0rd1 |
| bob | bob@test.com | User | P@ssw0rd1! |

---

## 🔒 Authentication Flow

1. **Register:** `/api/auth/register`  
   Returns a JWT access token.

2. **Login:** `/api/auth/login`  
   Returns a new JWT access token.

3. **Authorize Swagger:**  
   Click the 🔒 **Authorize** button and paste:  
   ```
   Bearer {your_token_here}
   ```

---

## 🧩 Example API Usage

**Register**
```bash
POST /api/auth/register
{
  "username": "john",
  "email": "john@test.com",
  "password": "P@ssw0rd1!"
}
```

**Login**
```bash
POST /api/auth/login
{
  "email": "john@test.com",
  "password": "P@ssw0rd1!"
}
```

**Create Content**
```bash
POST /api/content
Authorization: Bearer <token>

{
  "title": "New Video",
  "description": "My first upload",
  "category": "Video",
  "thumbnailUrl": "https://example.com/thumb.jpg",
  "contentUrl": "https://example.com/video.mp4"
}
```

**Rate Content**
```bash
POST /api/ratings
Authorization: Bearer <token>

{
  "mediaContentId": "GUID_HERE",
  "score": 5,
  "comment": "Amazing work!"
}
```

---

## 🧱 Project Principles

- **Clean Architecture** Separation of concerns between layers.
- **Dependency Injection** All services and repositories registered via `DependencyInjection.cs`.
- **DTOs + FluentValidation** Keep domain models clean and validation externalized.
- **Exception Middleware** Centralized API error handling with consistent JSON responses.
- **Automated Tests** Integration coverage for all critical endpoints.

---

## 🐳 Docker Notes

Check container status:
```bash
docker compose ps
```

Tail logs:
```bash
docker compose logs -f api
```

Clean everything:
```bash
docker compose down -v
```

---

## 🧰 Useful Commands

| Command | Description |
|----------|-------------|
| `dotnet build` | Build the solution |
| `dotnet ef migrations add <Name>` | Add EF migration |
| `dotnet ef database update` | Apply DB migrations |
| `dotnet test` | Run test suite |
| `docker compose up -d` | Run app via Docker |
