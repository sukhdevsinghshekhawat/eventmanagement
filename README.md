# College Event Management

Role based college event management app. The React frontend lives in `frontend/`; the ASP.NET Core API and MySQL data layer live in `MyMvcApp/`.

The API separates `DTOs/Requests` and `DTOs/Responses`, service contracts in `Interfaces/Services`, implementations in `Services`, mapping helpers in `Mapping`, and centralized API error handling in `Middleware`. Controllers handle HTTP routing and role authorization; services own Identity, EF Core, and business rules. API endpoints return DTOs rather than database entities.

## Roles and access

- **Admin:** sees every event and application, creates or removes teacher accounts, reviews applications, and enables or disables student accounts.
- **Teacher:** an admin creates the account and supplies its login credentials. A teacher creates events for one or more departments and reviews applications for events they created or events targeting their department.
- **Student:** registers as a student, sees events for their department, applies with email and phone number, and tracks application status.

The API enforces the role and department rules. Frontend route guards only control navigation and are not used as the security boundary.

## Run locally

1. Configure the API's MySQL connection, JWT signing key, and seed admin through ASP.NET configuration or environment variables. Use a strong private JWT key and keep credentials out of source control.
2. Apply the checked-in EF Core migrations to the configured database, then start the API from `MyMvcApp/` with `dotnet run` (default API URL: `http://localhost:5297`).
3. Start the frontend with `cd frontend && npm install && npm run dev`. Vite proxies `/api` to the local API.
4. Sign in with the seeded admin, create teacher accounts from **Manage Teachers**, and share those credentials with the intended teachers. Students can register from the login screen.

## Deployment configuration

- Set `VITE_API_BASE_URL` at frontend build time to the API base URL including `/api` (for example, `https://api.example.edu/api`). Leave it unset when the frontend and API share an origin and `/api` is routed to the API.
- For local development, set `BACKEND_URL` in `frontend/.env` to the API origin (for example, `http://localhost:5297`). Vite forwards all `/api` requests to this URL; restart Vite after changing it.
- Set ASP.NET `Frontend:Origin` to the exact deployed frontend origin when the API is hosted on a different origin.
- Configure `ConnectionStrings__DefaultConnection`, `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience`, `SeedAdmin__Email`, and `SeedAdmin__Password` through the deployment secret store. Do not use development credentials for deployment.

## Main API areas

- `api/auth`: student registration and login
- `api/admin`: teacher accounts, student list and account status
- `api/events`: department filtered event listing and event management
- `api/applications`: student applications, status tracking and reviews
# eventmanagement
