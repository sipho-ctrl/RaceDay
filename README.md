# RaceDay - Event Management System

## Student Information
- **Name**: Sipho Swartbooi
- **Student Number**: ST10467895
- **Assignment**: Part 3 - Automation of Web Application

## System Overview
RaceDay is a full-stack event management platform for the South African road running, walking, and cycling community. It consists of:
- **Part 1**: ERD, API endpoint plan, SQL database
- **Part 2**: ASP.NET Core Web API with JWT authentication and role-based access
- **Part 3**: ASP.NET Core MVC frontend that consumes the API, with distinct Organiser and Participant interfaces

## User Roles

### Organiser
- Dashboard with event and enrolment statistics
- Create, edit, and delete events
- Manage event categories
- View all event enrolments
- Capture and edit race results

### Participant
- Browse upcoming events with filters
- View event details and enrol
- View personal enrolments and start numbers
- View personal race history and results
- Manage personal profile

## Technology Stack
- **Backend**: ASP.NET Core Web API (.NET 8.0)
- **Frontend**: ASP.NET Core MVC (.NET 8.0)
- **Database**: SQL Server with Entity Framework Core (Code-First)
- **Authentication**: JWT with BCrypt password hashing
- **API Documentation**: Swagger UI
- **Testing**: xUnit with InMemory database
- **Containerisation**: Docker
- **CI/CD**: GitHub Actions
- **Version Control**: GitHub

RaceDay/
docs/ (Part 1 planning documents)
RaceDayAPI/ (Part 2 RESTful API)
RaceDayAPI.Tests/ (Part 2 unit tests)
RaceDayMVC/ (Part 3 MVC frontend)
.github/workflows/ (CI/CD pipeline)
Dockerfile (MVC container definition)
RaceDayAPI.sln
README.md


## Setup Instructions (Local Development)

### Prerequisites
- Visual Studio 2022
- .NET 8.0 SDK
- SQL Server LocalDB
- Docker Desktop (for container run)

### 1. Database Setup
1. Open SQL Server Management Studio
2. Connect to `(localdb)\mssqllocaldb`
3. Run `RaceDayDatabase.sql` from `/docs`, OR use EF Core migrations:

cd RaceDayAPI
Update-Database



### 2. Run the API
1. Open `RaceDayAPI.sln` in Visual Studio
2. Right-click `RaceDayAPI` → **Set as Startup Project**
3. Press `F5` — Swagger opens at `https://localhost:7162/swagger`

### 3. Run the MVC App
1. Right-click `RaceDayMVC` → **Set as Startup Project**
2. Press `F5` — MVC opens at `https://localhost:7xxx`

**Tip:** Set both projects as startup — right-click the solution → **Properties** → **Multiple startup projects** → set `RaceDayAPI` and `RaceDayMVC` to **Start**.

### 4. Run Unit Tests
1. Open **Test Explorer** (Test → Test Explorer)
2. Click **"Run All Tests"**
3. All tests should pass

## Docker Setup

### Build the image
```bash
docker build -t racedaymvc:latest .
``` 

docker run -d -p 8080:8080 --name racedaymvc racedaymvc:latest

Access the app
Open browser → http://localhost:8080

docker stop racedaymvc
docker rm racedaymvc

Note: The containerised MVC app requires the API to be running and reachable. When running the container locally, the API should be accessible from the container's network. For local development and demonstration, run the API outside Docker and point the MVC container at it via the ApiSettings:BaseUrl environment variable:

docker run -d -p 8080:8080 -e ApiSettings__BaseUrl=https://host.docker.internal:7162 --name racedaymvc racedaymvc:latest

API Endpoints (Part 2)
Authentication
POST /api/Auth/register — Register (Organiser or Participant)

POST /api/Auth/login — Login and return JWT token

Events
GET /api/Events — List all events

GET /api/Events/{id} — Event details

GET /api/Events/organiser — Organiser's own events

POST /api/Events — Create event (Organiser)

PUT /api/Events/{id} — Update event (Organiser, owner)

DELETE /api/Events/{id} — Delete event (Organiser, owner)

Categories
GET /api/Categories/event/{eventId} — Categories for an event

GET /api/Categories/{id} — Category details

POST /api/Categories — Create (Organiser, event owner)

PUT /api/Categories/{id} — Update (Organiser, event owner)

DELETE /api/Categories/{id} — Delete (Organiser, event owner)

Enrolments
GET /api/Enrolments — My enrolments (Participant)

GET /api/Enrolments/event/{eventId} — Event enrolments (Organiser, owner)

GET /api/Enrolments/{id} — Enrolment details

POST /api/Enrolments — Enrol (Participant)

DELETE /api/Enrolments/{id} — Cancel (Participant, own)

Results
GET /api/Results — My results (Participant)

GET /api/Results/event/{eventId} — Event results

GET /api/Results/{id} — Result details

POST /api/Results — Capture (Organiser, event owner)

PUT /api/Results/{id} — Update (Organiser, event owner)

Users
GET /api/Users/profile — Get current user's profile

PUT /api/Users/profile — Update profile

