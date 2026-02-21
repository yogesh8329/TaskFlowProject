# TaskFlow API

## Overview
TaskFlow API is a RESTful backend service built using ASP.NET Core 8.  
It provides user authentication and task management functionality with structured architecture, validation, caching, and logging.

---

## Tech Stack

- ASP.NET Core 8
- Entity Framework Core
- SQL Server
- Redis (Caching)
- JWT Authentication
- FluentValidation
- Clean layered architecture

---

## Features

- User Registration & Login
- JWT-based Authentication
- Role-based Authorization
- Task CRUD operations
- Pagination support
- Soft Delete implementation
- Global Exception Handling Middleware
- Redis Caching
- Logging
- Password Reset via Email

---

## Project Structure

- Controllers → Handle HTTP requests
- Services → Business logic layer
- Repositories → Data access abstraction
- Infrastructure → Caching & shared utilities
- Middleware → Exception & request logging
- Validators → Input validation
- Models & DTOs → Data transfer structure

---

## How to Run

1. Clone the repository  
2. Update connection string in `appsettings.json`  
3. Run database migrations:
