# Game Platform RWA - Web Application

## Overview

## 🎮 Overview

**Game Platform RWA** is a full-stack ASP.NET Core web application built to allow users to explore, review, and manage video games. The platform follows a **clean layered architecture**, separating responsibilities across a Web API, MVC frontend, business logic layer, and shared data models.

The application supports two primary user roles:

- **Regular Users** can:

  - Register and authenticate using a secure JWT-backed cookie session
  - Browse and filter games by type and genre
  - Leave and manage reviews for games
  - View their profile and update personal information

- **Administrators** have access to:
  - Full CRUD operations for games, game types, and genres
  - User management (including role promotion)
  - Review moderation (approve/disapprove/delete)
  - A secure admin panel and logging system

---

### 🔐 Security Model

- **Hybrid authentication**: JWT token for API + cookie-based authentication in WebApp
- **Role-based authorization** enforced via ASP.NET Core Identity claims and `[Authorize(Roles = "...")]` attributes
- **Static HTML admin tools** (e.g., log viewer) use localStorage-based JWT handling for secure direct API access

---

### 🧱 Technical Foundation

- **Frontend**: ASP.NET Core MVC with Razor views, ViewModels, and dynamic routing
- **Backend**: ASP.NET Core Web API with attribute-based routing and DTO handling
- **Persistence**: Entity Framework Core with SQL Server
- **Data Layer**: Repository + Interface abstraction per entity
- **Business Logic**: Clean separation of DTOs, ViewModels, and AutoMapper profiles

---

### ⚙️ Design Principles

- 🧩 **Modular CRUD**: Easily extendable and testable per entity (Game, Genre, etc.)
- 🔁 **Generic Repositories**: Interfaces and services encapsulate logic
- 📦 **DTO Mappers**: Automated translation from EF Core models to transport-safe objects
- ✅ **Validation**: Attributes applied at ViewModel and DTO level for both client and server consistency

---

## Architecture Summary

### 1. **Solution Structure**

- **Backend API** (ASP.NET Core Web API)

  - Handles all business logic and data access
  - Exposes endpoints for users, games, genres, game types, reviews, and authentication

- **Frontend WebApp** (ASP.NET Core MVC)

  - Provides views and user interactions
  - Communicates with the backend via the `ApiService` class

- **Shared Models & DTOs**

  - Used to transfer data between client and server
  - Separate ViewModels for frontend and DTOs for API

### 2. **Main Features**

## 🔐 Authentication & Authorization

The application uses a **hybrid authentication approach** that combines **JWT tokens** and **cookie-based sessions** to secure both API access and user sessions.

---

### ✅ API Authentication (JWT)

- The WebAPI issues a **JWT token** upon successful login via `/api/User/Login`.
- Token is generated using a secure symmetric key (`JWT:SecureKey`) defined in `appsettings.json`.
- JWTs are validated in the WebAPI using `JwtBearerDefaults.AuthenticationScheme`.
- Protected API routes use `[Authorize]` and optionally `[Authorize(Roles = "...")]`.
- Swagger UI is configured with a Bearer token field for API testing.

---

### ✅ MVC WebApp Authentication (Cookies + JWT)

- The `LoginController` in the MVC frontend sends credentials to the WebAPI login endpoint.
- Upon success, the API returns:
  - A `JWT` token
  - The user's role
- The WebApp stores the JWT in a custom claim and issues a **cookie-based identity** using:
  - `ClaimTypes.Name` for the username
  - `ClaimTypes.Role` for role-based routing
  - `"JwtToken"` as a custom claim to allow secure API calls using `ApiService`
- Authentication is maintained via `HttpContext.SignInAsync` and `CookieAuthenticationDefaults.AuthenticationScheme`.

---

### 🔓 Logout Handling

- User is signed out using `HttpContext.SignOutAsync`.
- Upon logout, they are redirected to the login page (`/Login`).

---

### 🛡️ Security Practices

- CORS is configured in the WebAPI to allow AJAX requests from the frontend (`http://localhost:5196`) and support `Authorization` headers and cookies.
- Static files such as `log-list.html` use JWT from `localStorage` to authenticate directly with the API.

---

### 🔐 Claims in Use

| Claim Type | Description                    |
| ---------- | ------------------------------ |
| `Name`     | The username                   |
| `Role`     | The role assigned by the API   |
| `JwtToken` | The raw token for API requests |

This setup allows seamless interaction between the ASP.NET Core MVC frontend and the secure Web API backend.

---

## 🗂️ Interfaces & Repository Pattern

The application implements a **clean separation of concerns** using the **Repository Pattern** and **interface abstraction** for all entity-related operations.

This design ensures:

- Centralized data access logic
- Easier unit testing and mocking
- Reduced duplication across entities

---

### ✅ Structure

For each domain entity (e.g., `Game`, `Genre`, `GameType`, `User`), the following components are implemented:

| Component             | Description                                                             |
| --------------------- | ----------------------------------------------------------------------- |
| `I<Entity>Repository` | Interface defining CRUD methods (`GetAll`, `GetById`, `Create`, etc.)   |
| `<Entity>Repository`  | Concrete implementation using Entity Framework Core                     |
| `DTO Generator`       | Converts EF Core entities into lightweight data transfer objects (DTOs) |

#### Game Management

- CRUD operations for games
- Each game has:

  - A `GameType` (1\:N relationship)
  - Multiple `Genres` (M\:N relationship)
  - Multiple `Reviews` (M\:N between users and games)

#### Admin Features

- **GameType Management**

  - View, create, update, delete game types

- **Genre Management**

  - Same CRUD pattern as GameType

- **User Management**

  - View all users
  - Promote/demote user roles
  - Update/delete user profiles

- **Review Moderation**

  - View all reviews
  - Approve/disapprove reviews
  - Delete reviews

### 3. **Entities and Relationships**

- **User**

  - `Id`, `Username`, `Email`, `Phone`, `RoleId`, `PwdHash`, `PwdSalt`
  - Relationships: `Role`, `Reviews`

- **Game**

  - `Id`, `Name`, `Description`, `GameTypeId`, `ImageUrl`, `ReleaseDate`, `MetacriticScore`, etc.
  - Relationships: `GameType`, `Genres`, `Reviews`

- **GameType**

  - Simple entity with `Id`, `Name`

- **Genre**

  - `Id`, `Name`

- **Review**

  - Composite key (`UserId`, `GameId`)
  - `Rating`, `ReviewText`, `Approved`, `CreatedAt`

---

## Frontend (WebApp) Highlights

### Views

- Razor views under `/Views/Admin` or `/Views/User`
- Strongly-typed views using ViewModels

### Controllers

- `AdminGameController`, `AdminGameTypeController`, `AdminGenreController`, `AdminUserController`, `AdminUserReviewController`
- Each controller wraps around corresponding API routes

### `ApiService`

- Centralized HTTP client wrapper
- Handles GET, POST, PUT, DELETE calls
- Injected into all admin controllers

### Forms & Bindings

- All forms use `asp-for` to bind ViewModels
- Smart conditional rendering (e.g., Create vs. Edit form title)
- Approve/disapprove reviews via form post (mapped to `PUT` method in API)

---

## Backend (Web API) Highlights

### Routing & Controllers

- Attribute-based routing (e.g., `[HttpPut("Update/{id}")]`)
- `Authorize` attributes for protected endpoints
- Roles (`Admin`, `User`) enforced where applicable

### Security

- JWT token system with claims: `Name`, `Role`, and `UserId`
- Token used to authorize API calls and check identity (`whoami` endpoint)

### Logging

- `LogService` logs successful and failed operations
- Categorized logs ("Success", "Error", "No Results")

### DTO Handling

- DTOs used to decouple models from frontend:

  - `UserDto`, `EditUserDto`, `GameCreateDto`, `UserGameReviewDto`, etc.

- `GameDTOGenerator` used to convert EF entities to DTOs

---

## Development and Customization

### How to Run:

1. Run `GamePlatformRwa.API` project (backend)
2. Run `WebApp` project (frontend)
3. Make sure the base URL in `ApiService.cs` matches your API project address

### Adding New Admin Feature:

1. Add new Web API controller method (GET/POST/PUT/DELETE)
2. Create ViewModel for frontend
3. Add new controller and views in WebApp
4. Wire up `ApiService` calls

---

## Summary

This project is a full-featured ASP.NET Core MVC + Web API application with:

- Clean separation between backend and frontend
- Strong admin feature set
- Secure user authentication
- Robust review moderation system
- Expandable, modular code structure
