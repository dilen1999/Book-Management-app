# 📚 BookMS Backend

This is the **backend API** for the Book Management System (BookMS), built with **.NET 9**, **Entity Framework Core**, **CQRS (MediatR)**, and **SQL Server**.  
It provides **authentication (JWT, Google, Refresh token)**, **books management**, **categories**, and more.

---

## 🚀 Features

- 🔑 Authentication
  - Register / Login with JWT
  - Google Sign-In
- 📘 Books API  
  - Create, List, Get By Id, Count
- 🏷️ Categories API  
  - List all categories, Get total count
- 🧩 CQRS with MediatR
- 🗄️ EF Core + SQL Server
- 📖 Swagger UI for API testing
- ✅ Unit tests with **xUnit + Moq + FluentAssertions**

---

## 🛠️ Prerequisites

Make sure you have installed:

- [Git](https://git-scm.com/)  
- [Docker Desktop](https://www.docker.com/)  
- [.NET 9 SDK](https://dotnet.microsoft.com/) (only if you want to run tests outside Docker)  

---

## ⚙️ How to Run

1. Clone the repository:
   ```bash
   git clone https://github.com/dilen1999/Book-Management-app.git
   cd Backend
   ```
2. Start Docker Desktop and run:
    ```bash
    docker compose up -d --build
    ```

✅ This will automatically:

Build the backend

Set up SQL Server database

Run migrations

Start the API inside a container

3. Once running, open:
👉 http://localhost:8080/swagger

to explore all available API endpoints.


## Running Tests
Run unit tests with:
```bash
dotnet test
```
