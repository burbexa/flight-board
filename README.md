# ✈️ Real-Time Flight Board Management System

## Overview
A professional full-stack flight board management system featuring real-time updates, clean architecture, and test-driven development. The system includes a responsive React frontend and an ASP.NET Core backend powered by SignalR and SQLite.

---

## 🧠 Features

### 🔧 Backend (ASP.NET Core)
- Real-time updates with SignalR (`Microsoft.AspNetCore.SignalR`)
- EF Core with SQLite persistence
- Clean Architecture with layered separation: Domain, Application, Infrastructure, API
- Server-side validation and business logic
- Server-side status calculation for flights:
  - **Scheduled**: More than 30 minutes before departure
  - **Boarding**: 0–30 minutes before departure
  - **Departed**: 0–60 minutes after departure
  - **Landed**: 60+ minutes after departure
- API Endpoints:
  - `GET /api/flights` – Get all flights with status
  - `POST /api/flights` – Add new flight with validation
  - `DELETE /api/flights/{id}` – Delete by ID
  - `GET /api/flights/search?status=...&destination=...` – Search by filters

### 🧪 Testing (TDD)
- xUnit test project with Moq mocking
- Full unit test coverage of all critical backend logic:
  - `AddFlightAsync` (validations)
  - `DeleteFlightAsync` (with hub broadcast)
  - `CalculateStatus`
  - `GetAllWithStatusAsync`
  - Edge-case validation (e.g. missing fields, invalid departure time)
- Test Structure:
  - `Arrange` – setup mocks and inputs
  - `Act` – call the tested method
  - `Assert` – validate expected result or exception

### 📁 Backend Libraries Used
- `Microsoft.AspNetCore.SignalR`
- `Microsoft.EntityFrameworkCore.Sqlite`
- `Swashbuckle.AspNetCore` (Swagger)
- `Serilog.Sinks.File` (Structured Logging)
- `xUnit`, `Moq`

---

## 🎨 Frontend (React + TypeScript)

### Setup & Run Instructions
```bash
# Install dependencies
npm install

# Start development server
npm run dev

# Visit http://localhost:3000
```

### Key Frontend Features
- React with TypeScript for safety & scalability
- Redux Toolkit for UI state (e.g., filters)
- React Query (@tanstack/react-query) for backend communication and caching
- CSS Modules + Material UI for clean styling
- SignalR integration for real-time updates
- Form with validation and filterable/searchable table view
- Delete and Add operations with optimistic UI

### Frontend Libraries Used
- `react`, `react-dom`
- `@reduxjs/toolkit`
- `@tanstack/react-query`
- `@microsoft/signalr`
- `@mui/material`, `@emotion/react`, `@emotion/styled`
- `notistack` (for snackbars)
- `jest`, `@testing-library/react`


---

## 📁 Project Structure
```
FlightBoardAPI/
├── Application/
│   ├── Interfaces/
│   └── Services/
├── Domain/
├── Infrastructure/
│   └── Repositories/
├── API/
│   ├── Endpoints/
│   └── Hubs/
├── Migrations/
├── Program.cs
├── appsettings.json

FlightBoard.Tests/
├── FlightServiceTests.cs
```

---

## 👨‍💻 Author
**Oleg Burbyga**

---

## 🧪 How TDD Was Applied
Tests were written **before** implementing key logic like flight validation and status calculation. Mocks were used to isolate dependencies, and unit tests drove the service implementation.

Example:
```csharp
[Fact]
public void CalculateStatus_ShouldReturnScheduled_WhenDepartureIsFarAway()
{
    var departure = DateTime.Now.AddMinutes(90);
    var result = _service.CalculateStatus(departure);
    Assert.Equal(FlightStatus.Scheduled, result);
}
```

---

## 📸 Optional Demo
Include a screen recording or animated GIF of the running project to show off the UI and SignalR updates in action!
