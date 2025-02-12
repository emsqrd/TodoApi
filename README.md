# 📝 TodoApi

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![API](https://img.shields.io/badge/API-REST-009688?style=flat-square&logo=fastapi)](https://learn.microsoft.com/aspnet/core/web-api/)

> A minimal ASP.NET Core API for managing tasks/todos, built with .NET 9.0. This API serves as the backend for [todo-app](https://github.com/emsqrd/todo-app).

## ✨ Features

- 🔄 RESTful API endpoints for CRUD operations
- 💾 In-memory task storage
- ⚡ Fast and lightweight
- 🔒 HTTPS support

## 🚀 Prerequisites

- .NET 9.0 SDK

## 🏃‍♂️ Getting Started

1. Clone the repository
2. Navigate to the project directory
3. Run the application:

```bash
dotnet run
```

### 🌐 Access Points

| Protocol | URL                      |
| -------- | ------------------------ |
| HTTP     | `http://localhost:5080`  |
| HTTPS    | `https://localhost:7124` |

## 🛠️ API Endpoints

| Method | Endpoint          | Description       |
| ------ | ----------------- | ----------------- |
| GET    | `/api/tasks`      | Get all tasks     |
| POST   | `/api/tasks`      | Create a new task |
| PUT    | `/api/tasks/{id}` | Update a task     |
| DELETE | `/api/tasks/{id}` | Delete a task     |

---

<div align="center">

Made with ❤️ using ASP.NET Core

</div>
