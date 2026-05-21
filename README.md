# WaterTracker

WaterTracker is a full-stack water intake tracking application built with a .NET API backend and a Blazor WebAssembly frontend.

The app allows users to register, log in, and manage their own water intake records. Users can add, view, edit and delete entries, with each entry linked to the authenticated user.

This was built as a take-home technical exercise. The main flow is working, but I have kept the scope realistic and listed the remaining improvements at the bottom of this README.

---

## What has been implemented

- User registration and login
- JWT authentication
- Protected water intake API endpoints
- Create, view, update and delete water intake records
- Blazor login and registration pages
- Blazor authentication state handling
- navigation and logout
- Water intake dashboard
- EF Core migrations
- Unit tests for the domain model
- API integration tests
- GitHub Actions build and test workflow

---

## Tech stack

### Backend

- .NET 10
- ASP.NET Core Web API
- ASP.NET Core Identity
- JWT bearer authentication
- Entity Framework Core
- SQLite

### Frontend

- Blazor WebAssembly
- Blazor forms and validation
- CSS isolation
- Plain CSS

### Testing

- xUnit
- WebApplicationFactory integration tests
- GitHub Actions CI

---

## Solution structure

```text
WaterTracker.Core
```

Contains the core domain entity and interfaces.

```text
WaterTracker.Contracts
```

Contains shared request and response models used by both the API and the Blazor client.

```text
WaterTracker.Infrastructure
```

Contains EF Core, Identity, database configuration, entity configuration and persistence services.

```text
WaterTracker.API
```

Contains the ASP.NET Core Web API, authentication endpoints, water intake endpoints and JWT setup.

```text
WaterTracker.Client
```

Contains the Blazor WebAssembly frontend, login/register pages, auth services, token storage and the water intake dashboard.

```text
WaterTracker.Tests
```

Contains unit tests and API integration tests.

---

## Architecture

The project is split into separate layers so each part has a clear responsibility.

The API handles authentication, authorisation and data access. The Blazor client handles the user interface and calls the API through typed services.

The Blazor pages do not call `HttpClient` directly. Authentication calls go through `IAuthService`, and water intake calls go through `IWaterIntakeClient`.

The API does not accept `UserId` from the client when managing water intake records. Instead, it gets the current user from the JWT claims and scopes all water intake queries to that user.

This prevents users from accessing or changing another user’s records by guessing an ID.

---

## Security considerations

A few security decisions were made during the build:

- ASP.NET Core Identity is used for password handling.
- Passwords are not manually stored, logged or hashed by the application code.
- Login failures return a generic message to avoid revealing whether an email address exists.
- Identity lockout is enabled for repeated failed login attempts.
- JWT issuer, audience and signing key configuration are validated at startup.
- Protected endpoints use `[Authorize]`.
- Water intake records are always checked against the authenticated user ID.
- The API returns `404 Not Found` if a record does not exist or does not belong to the current user.
- The Blazor client does not display the JWT token.
- Tokens are not placed in URLs.
- The bearer token is attached to protected API calls using an HTTP handler.

Several of these decisions were informed by the OWASP Top 10:

- **A01 – Broken Access Control:** all water intake queries are scoped to the authenticated user's ID, preventing a user from reading or modifying another user's records by guessing an ID (insecure direct object reference).
- **A02 – Cryptographic Failures:** ASP.NET Core Identity handles password hashing using a vetted algorithm. Passwords are never stored in plaintext or returned in responses.
- **A03 – Injection:** EF Core uses parameterised queries throughout, and request models are validated with data annotations before reaching any data access code.
- **A07 – Identification and Authentication Failures:** failed login attempts return a generic message so an attacker cannot tell whether an email address is registered. Identity lockout is enabled for repeated failed attempts.

For this exercise, the Blazor client stores the JWT in `localStorage`. This keeps the project simple, but in a production application token storage would need further review because JavaScript-accessible storage can be exposed by XSS. A production system may use secure HttpOnly cookies, refresh tokens, an external identity provider, or another session approach depending on requirements.

---

## Testing

The project includes both unit tests and integration tests.

The unit tests cover the `WaterIntakeEntry` domain entity, including:

- creating a valid entry
- rejecting invalid amounts
- rejecting missing user IDs
- rejecting missing consumed dates
- updating an entry
- setting the updated timestamp

The API integration tests cover:

- user registration
- user login
- invalid login handling
- protected endpoint access without a token
- ensuring one user cannot access another user’s water intake record

Run the tests with:

```bash
dotnet test
```

Build the solution with:

```bash
dotnet build
```

---

## Running the project locally

### 1. Restore packages

From the solution root:

```bash
dotnet restore
```

---

### 2. Apply database migrations

```bash
dotnet ef database update --project WaterTracker.Infrastructure --startup-project WaterTracker.API
```

If `dotnet ef` is not installed, install it with:

```bash
dotnet tool install --global dotnet-ef
```

---

### 3. Run the API

```bash
dotnet run --project WaterTracker.API
```

Make a note of the API URL shown in the terminal.

---

### 4. Check the Blazor client API URL

The Blazor client reads the API base URL from:

```text
WaterTracker.Client/wwwroot/appsettings.json
```

Example:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:5001"
  }
}
```

Make sure this matches the URL used by `WaterTracker.API`.

---

### 5. Run the Blazor client

In a separate terminal:

```bash
dotnet run --project WaterTracker.Client
```

Open the Blazor client URL in a browser.

---

## API endpoints

### Authentication

```http
POST /api/auth/register
POST /api/auth/login
```

### Water intake

```http
GET    /api/water-intake
GET    /api/water-intake/{id}
POST   /api/water-intake
PUT    /api/water-intake/{id}
DELETE /api/water-intake/{id}
```

The water intake endpoints require a valid JWT bearer token.

---

## Frontend routes

```text
/             → water intake dashboard (default, requires login)
/water-intake → water intake dashboard
/login        → sign in
/register     → create account
```

The water intake dashboard is protected. Unauthenticated users are redirected to `/login`.

---

## Known limitations

The project is not fully completed. limitations are:

- JWTs are stored in `localStorage` for simplicity.
- Refresh tokens are not implemented.
- Password reset is not implemented.
- Email confirmation is not implemented.
- Date/time handling is basic and would need more work for a production app.
- Frontend automated tests are not included.
- The dashboard is intentionally simple.
- Deployment configuration has not been added.
- More detailed API error handling could be added in the Blazor client.

---

## Future improvements

Given more time, I would add:

- Email confirmation
- Password reset
- Daily water intake goals
- Date range filtering
- Charts or progress indicators
- Better frontend error handling
- Frontend component tests
- Deployment configuration
- Stronger production security headers
- Content Security Policy
- More modern user design

---

## Development approach

1. Solution structure
2. Domain entity
3. Contracts
4. EF Core, Identity, JWT setup and migrations
5. Authentication API endpoints
6. Protected water intake API endpoints
7. Unit tests and API integration tests
8. Blazor authentication services
9. Login and registration pages
10. Authenticated water intake API client
11. Water intake dashboard

---
