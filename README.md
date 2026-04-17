# 🚀 TaskFlow API (ASP.NET Core Backend)

A production-style backend API built using ASP.NET Core, implementing secure JWT authentication, user-based task management, and clean architecture principles.

This project demonstrates real-world backend development practices including authentication, authorization, validation, rate limiting, and structured code organization.

---

## 🔥 Features

* 🔐 JWT Authentication (Register & Login)
* 🧑 User-based Authorization (Protected APIs)
* 📋 Task Management (Create, Fetch user-specific tasks)
* ⚡ Rate Limiting (API protection)
* 🛡️ Global Exception Handling (Middleware)
* 🧪 Input Validation using FluentValidation
* 🧱 Clean Architecture (Controllers → Services → Repositories)
* 🧾 Structured Logging (Serilog)

---

## 🛠️ Tech Stack

* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* JWT (JSON Web Token)
* FluentValidation
* Serilog

---

## 📂 Project Structure

* **Controllers** → Handle HTTP requests
* **Services** → Business logic
* **Repositories** → Data access layer
* **Models** → Entities & DTOs
* **Middleware** → Exception & logging
* **Validators** → Request validation

---

## 🚀 How to Run Locally

### 1. Clone the repository

```bash
git clone https://github.com/YOUR_USERNAME/TaskFlowProject.git
```

### 2. Navigate to project

```bash
cd TaskFlowProject
```

### 3. Configure settings

Update `appsettings.Development.json`:

* Add SQL Server connection string
* Add JWT secret key

Example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=TaskFlowDB;Trusted_Connection=True;TrustServerCertificate=True;"
},
"Jwt": {
  "Key": "YOUR_SECRET_KEY",
  "Issuer": "TaskFlow",
  "Audience": "TaskFlowUsers"
}
```

---

### 4. Run the project

```bash
dotnet run
```

---

### 5. Open Swagger UI

```
https://localhost:7077/swagger
```

---

## 🔑 API Usage Flow

1. Register a new user
2. Login → Receive JWT token
3. Click **Authorize** in Swagger
4. Enter:

```
Bearer YOUR_TOKEN
```

5. Access protected APIs
6. Create tasks
7. Fetch user-specific tasks

---

## 📌 Example APIs

* `POST /api/v1/Auth/register`
* `POST /api/v1/Auth/login`
* `GET /api/v1/Task/my`
* `POST /api/v1/Task`

---

## ⚠️ Important Notes

* Email functionality is mocked using `DummyEmailService`
* This project is designed for local/demo purposes
* No sensitive data is stored in the repository

---

## 👨‍💻 Author

**Yogesh**
ASP.NET Core Backend Developer
