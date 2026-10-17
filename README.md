# RaceDay - Event Management System

## Student Information
- **Name**: Sipho Swartbooi
- **Student Number**: ST10467895
- **Assignment**: Part 2 - RESTful API Development

## System Overview
RaceDay is a full-stack web-based event management system designed for the South African road running, walking, and cycling community. The platform allows Event Organisers to create and manage events, categories, and participant results, while Participants can browse upcoming events, enter events, track their personal performance history, and prepare for race day.

## User Roles

### Organiser
- Create, edit, and delete events
- Manage event categories
- Capture participant results
- View all event enrolments for their events

### Participant
- Create an account and manage profile
- Browse upcoming events
- Enter events by selecting categories
- View personal enrolments
- Track personal results and performance history

## Technology Stack
- **Backend**: ASP.NET Core Web API (.NET 8.0)
- **Database**: SQL Server with Entity Framework Core (Code-First)
- **Authentication**: JWT with BCrypt password hashing
- **API Documentation**: Swagger UI
- **Testing**: xUnit with InMemory database
- **CI/CD**: GitHub Actions
- **Version Control**: GitHub

## Project Structure
RaceDay/
├── docs/
│ ├── ERD.pdf
│ ├── APIEndpointPlan.docx
│ ├── RaceDayDatabase.sql
│ ├── green-build.png
│ ├── part2-green-build.png
│ ├── RD DB OUTPUT I.png
│ └── RD DB OUPUT II.png
├── RaceDayAPI/ ← Part 2 API
│ ├── Controllers/
│ │ ├── AuthController.cs
│ │ ├── EventsController.cs
│ │ ├── CategoriesController.cs
│ │ ├── EnrolmentsController.cs
│ │ ├── ResultsController.cs
│ │ └── UsersController.cs
│ ├── Models/
│ │ ├── User.cs
│ │ ├── Organiser.cs
│ │ ├── Participant.cs
│ │ ├── Event.cs
│ │ ├── Category.cs
│ │ ├── Enrolment.cs
│ │ └── Result.cs
│ ├── DTOs/
│ │ ├── RegisterDto.cs
│ │ ├── LoginDto.cs
│ │ ├── AuthResponseDto.cs
│ │ ├── CreateEventDto.cs
│ │ ├── UpdateEventDto.cs
│ │ ├── EventResponseDto.cs
│ │ ├── CreateCategoryDto.cs
│ │ ├── UpdateCategoryDto.cs
│ │ ├── CategoryResponseDto.cs
│ │ ├── CreateEnrolmentDto.cs
│ │ ├── EnrolmentResponseDto.cs
│ │ ├── CreateResultDto.cs
│ │ ├── UpdateResultDto.cs
│ │ ├── ResultResponseDto.cs
│ │ ├── UserProfileDto.cs
│ │ └── UpdateProfileDto.cs
│ ├── Data/
│ │ └── RaceDayDbContext.cs
│ ├── Services/
│ │ ├── IAuthService.cs
│ │ └── AuthService.cs
│ ├── Helpers/
│ ├── Program.cs
│ └── appsettings.json
├── RaceDayAPI.Tests/ ← Unit tests
│ ├── TestHelper.cs
│ ├── AuthServiceTests.cs
│ ├── EventsControllerTests.cs
│ ├── RoleBasedAccessTests.cs
│ └── EnrolmentTests.cs
├── .github/workflows/
│ └── validate.yml
└── README.md


## API Endpoints Implemented

### Authentication
| Method | Route | Description | Role Required |
|--------|-------|-------------|---------------|
| POST | `/api/Auth/register` | Register new user (Organiser or Participant) | None |
| POST | `/api/Auth/login` | Login and get JWT token | None |

### Events
| Method | Route | Description | Role Required |
|--------|-------|-------------|---------------|
| GET | `/api/Events` | Get all events | Any authenticated |
| GET | `/api/Events/{id}` | Get event by ID | Any authenticated |
| GET | `/api/Events/organiser` | Get organiser's own events | Organiser |
| POST | `/api/Events` | Create new event | Organiser |
| PUT | `/api/Events/{id}` | Update event | Organiser (owner) |
| DELETE | `/api/Events/{id}` | Delete event | Organiser (owner) |

### Categories
| Method | Route | Description | Role Required |
|--------|-------|-------------|---------------|
| GET | `/api/Categories/event/{eventId}` | Get categories for an event | Any authenticated |
| GET | `/api/Categories/{id}` | Get category by ID | Any authenticated |
| POST | `/api/Categories` | Create new category | Organiser (event owner) |
| PUT | `/api/Categories/{id}` | Update category | Organiser (event owner) |
| DELETE | `/api/Categories/{id}` | Delete category | Organiser (event owner) |

### Enrolments
| Method | Route | Description | Role Required |
|--------|-------|-------------|---------------|
| GET | `/api/Enrolments` | Get my enrolments | Participant |
| GET | `/api/Enrolments/event/{eventId}` | Get all enrolments for an event | Organiser (event owner) |
| GET | `/api/Enrolments/{id}` | Get enrolment by ID | Participant (own) or Organiser (event owner) |
| POST | `/api/Enrolments` | Enrol in an event category | Participant |
| DELETE | `/api/Enrolments/{id}` | Cancel enrolment | Participant (own) |

### Results
| Method | Route | Description | Role Required |
|--------|-------|-------------|---------------|
| GET | `/api/Results` | Get my personal results | Participant |
| GET | `/api/Results/event/{eventId}` | Get all results for an event | Any authenticated |
| GET | `/api/Results/{id}` | Get result by ID | Participant (own) or Organiser (event owner) |
| POST | `/api/Results` | Capture participant result | Organiser (event owner) |
| PUT | `/api/Results/{id}` | Update result | Organiser (event owner) |

### Users
| Method | Route | Description | Role Required |
|--------|-------|-------------|---------------|
| GET | `/api/Users/profile` | Get current user's profile | Any authenticated |
| PUT | `/api/Users/profile` | Update current user's profile | Any authenticated |

## Database Schema

The database uses Entity Framework Core Code-First with the following entities:

1. **Users** - Base authentication table (UserID, Username, Email, PasswordHash, Role)
2. **Organisers** - Organiser profiles (OrganiserID, UserID, OrganisationName, ContactNumber)
3. **Participants** - Participant profiles (ParticipantID, UserID, FullName, DateOfBirth, Gender, ContactNumber, EmergencyContact)
4. **Events** - Events (EventID, OrganiserID, EventName, EventDate, Location, Description, MaxParticipants, RegistrationDeadline, EventStatus, Distance, EventType)
5. **Categories** - Event categories (CategoryID, EventID, CategoryName, Distance, AgeGroup, GenderGroup, EntryFee, MaxParticipants)
6. **Enrolments** - Participant registrations (EnrolmentID, ParticipantID, CategoryID, RegistrationDate, PaymentStatus, StartNumber)
7. **Results** - Race results (ResultID, EnrolmentID, FinishTime, OverallPosition, CategoryPosition, Pace, Status)

## Setup Instructions

### Prerequisites
- Visual Studio 2022
- .NET 8.0 SDK
- SQL Server (LocalDB or Express)

### Database Setup
1. Open SQL Server Management Studio (SSMS)
2. Connect to `(localdb)\mssqllocaldb`
3. Run `RaceDayDatabase.sql` from `/docs` folder OR use EF Core migrations:
   ```powershell
   cd RaceDayAPI
   Update-Database

   Run the API
Open RaceDayAPI.sln in Visual Studio

Press F5 to run

Swagger UI opens at https://localhost:7xxx/swagger

Run Unit Tests
Open Test Explorer (Test → Test Explorer)

Click "Run All Tests"

All tests should pass

Authentication
Register a new user
POST /api/Auth/register
{
  "username": "testuser",
  "email": "test@test.com",
  "password": "password123",
  "role": "Organiser",
  "organisationName": "Test Events",
  "contactNumber": "0821234567"
}

Login
POST /api/Auth/login
{
  "username": "testuser",
  "password": "password123"
}

Response:
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "username": "testuser",
  "role": "Organiser",
  "userID": 1
}

Using the token in Swagger
Click the "Authorize" button (lock icon)

Enter: Bearer YOUR_TOKEN

Click "Authorize"

All subsequent requests will include the token

CI/CD



---

## 🎯 NAVIGATION TO UPDATE README

### Step 1: Open the file
1. **Open File Explorer**
2. Navigate to: `C:\Users\YourName\Documents\GitHub\RaceDay\`
3. **Right-click** on `README.md`
4. Select **"Open with"** → **"Notepad"**

### Step 2: Replace content
1. Press `Ctrl + A` (select all)
2. Press `Delete`
3. **Paste the entire content above**
4. **Save** (Ctrl+S)

### Step 3: Update video link
1. Find: `[Watch the Part 2 presentation on YouTube](YOUR_YOUTUBE_LINK_HERE)`
2. Replace `YOUR_YOUTUBE_LINK_HERE` with your actual YouTube link
3. **Save**

### Step 4: Commit
1. Open **GitHub Desktop**
2. Write commit message:
