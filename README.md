# Game Platform RWA - Web Application

## Overview

Game Platform RWA is a web-based application designed to allow users to explore, review, and manage video games. The application supports two major user roles: regular users and administrators. Regular users can register, log in, browse games, leave reviews, and manage their profiles. Administrators have access to additional management tools, such as user management, game and genre CRUD operations, and moderation of user reviews.

This solution follows a layered architecture based on ASP.NET Core MVC, Entity Framework Core, and RESTful Web API communication.

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

#### User Authentication & Authorization

- JWT-based login
- `UserController` handles:

  - Registration
  - Login
  - Password change
  - Role promotion
  - Identity check (`whoami`)

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

This application is ideal for showcasing ASP.NET Core development best practices, authentication, and full CRUD capabilities.
