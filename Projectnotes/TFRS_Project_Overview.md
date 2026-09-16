# TFRS — Tricycle Franchise Registration System

## Project Overview

**TFRS** is a web-based Tricycle Franchise Registration System built with ASP.NET Core MVC and .NET 10. It manages tricycle franchise application and records information, with Microsoft Access used as the current database.

**Repository:** `DeperaltaJap/TFRS`  
**Branch:** `main`  
**Current status:** Full CRUD implemented, under active development.

---

## Technology Stack

- **Backend:** C# / ASP.NET Core MVC / .NET 10
- **Frontend:** Razor Views (`.cshtml`), HTML, CSS, JavaScript
- **Database:** Microsoft Access (`TFRSdb.accdb`)
- **Database access:** `System.Data.OleDb`
- **Database provider:** Microsoft ACE OLE DB
- **IDE/tooling:** Visual Studio
- **Version control:** Git / GitHub
- **Documentation:** `Projectnotes/project_map.md`, `Projectnotes/CODE_MAP.md`, Obsidian

---

## High-Level Architecture

```text
Browser
   │
   ▼
Razor Views (.cshtml)
   │
   ▼
Controllers
   │
   ▼
IMainTableService
   │
   ▼
MainTableService
   │
   ▼
OleDb
   │
   ▼
TFRSdb.accdb
   │
   ▼
MAINTABLE
```

The active database path is handled through configuration and the authoritative database access layer is `MainTableService`.

---

## Main Project Structure

```text
TFRS/
├── TFRS.slnx
├── TFRS/
│   ├── TFRS.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   │
│   ├── Controllers/
│   │   ├── HomeController.cs
│   │   ├── ApplicationController.cs
│   │   ├── RecordsController.cs
│   │   ├── FranchiseController.cs
│   │   ├── AccountController.cs
│   │   ├── AttendanceController.cs
│   │   ├── IDSystemController.cs
│   │   ├── SearchController.cs
│   │   ├── DiagnosticsController.cs
│   │   └── Tfrsdb.cs
│   │
│   ├── Models/
│   │   ├── MainTableRecord.cs
│   │   ├── TodaList.cs
│   │   └── ErrorViewModel.cs
│   │
│   ├── Services/
│   │   ├── IMainTableService.cs
│   │   └── MainTableService.cs
│   │
│   ├── Repositories/
│   │   └── ApplicationRepository.cs
│   │
│   ├── Views/
│   │   ├── Home/
│   │   │   ├── Index.cshtml
│   │   │   ├── Applicationpage.cshtml
│   │   │   ├── TFrecords.cshtml
│   │   │   ├── IDsystem.cshtml
│   │   │   └── certificateRegistration.cshtml
│   │   └── Shared/
│   │       ├── _Layout.cshtml
│   │       ├── Error.cshtml
│   │       └── _ValidationScriptsPartial.cshtml
│   │
│   └── wwwroot/
│       ├── css/
│       ├── js/
│       ├── images/
│       └── lib/
│
├── CODE_MAP.md
└── project_map.md
```

---

## Startup and Routing

`Program.cs` configures MVC, dependency injection, static files, routing, authorization, and the default MVC route.

Default route:

```text
/{controller=Home}/{action=Index}/{id?}
```

The application registers:

```csharp
IMainTableService -> MainTableService
```

---

## Database Layer

### Database

The current database is:

```text
TFRSdb.accdb
```

Main table:

```text
MAINTABLE
```

Access is performed using `OleDb`.

### Active Service

`MainTableService.cs` is the authoritative database layer currently used by the application.

`IMainTableService` currently exposes:

```csharp
IEnumerable<MainTableRecord> GetAll();
void Add(MainTableRecord record);
MainTableRecord? GetByFranchiseNumber(string franchiseNumber);
```

Therefore, the current backend officially supports:

- Read all records
- Read one record by franchise number
- Create/insert a record

It does **not yet provide backend Update or Delete methods**.

### Current database fields used by the service

The active read/insert path works with these 22 fields:

```text
DATE_SUBMITTED
TRICYCLE_FRANCHISE_NUMBER
NAME_OF_OPERATOR
ADDRESS
MAKE
YEAR_MODEL
COLOR
ENGINE_NUMBER
MV_FILE_NO
CHASSIS_NUMBER
PLATE_NUMBER
DATE_ISSUED
DATE_EXPIRED
TODA
YEAR_RENEW
YEAR_EXPIRED
STATUS
REMARKS
ROUTE_COVERED
COMMITTEE_REPORT
SB_APPROVED_DATE
TYPE_OF_APPLICATION
```

---

## Main Data Model

`MainTableRecord.cs` represents a franchise record.

The model currently includes properties such as:

```text
Id
DateSubmitted
TricycleFranchiseNumber
NameOfOperator
ContactNo
Address
CommunityTaxNo
PlaceIssued
Make
YearModel
Color
EngineNumber
MVFileNo
ChassisNumber
PlateNumber
DateIssued
DateExpired
TODA
YearRenew
YearExpired
Status
Remarks
RouteCovered
CommitteeReport
SBApprovedDate
TypeOfOwnership
TypeOfApplication
VerifiedBy
```

Several properties have validation attributes.

### Important model/database mismatch

These model properties exist but are not currently included in the active 22-field persistence path:

```text
ContactNo
CommunityTaxNo
PlaceIssued
TypeOfOwnership
VerifiedBy
```

This is an important technical debt item to address carefully before expanding record functionality.

---

## Application Module

### Controller

`ApplicationController.cs` handles the application-entry workflow.

Important actions:

- `Create()` — displays a new application form and initializes submission date
- `Index()` — loads records for the application page
- `Save()` — validates and inserts a new record through `IMainTableService`

Current save flow:

```text
POST /Application/Save
        ↓
ModelState validation
        ↓
MainTableService.Add(model)
        ↓
redirect to /Records
```

### View

`Views/Home/Applicationpage.cshtml` is the main data-entry interface.

It contains sections for:

- Operator information
- Motorcycle specifications
- Franchise information
- Dates
- TODA
- Status
- Route
- Application/report/approval information

---

## Records / Masterlist Module

### Controller

`RecordsController.cs` handles the records/masterlist area.

Important behavior:

```text
GET /Records
   ↓
_mainTableService.GetAll()
   ↓
Views/Home/TFrecords.cshtml
```

It also exposes:

```text
GET /Records/GetByFranchise/{id}
```

which retrieves a record by franchise number and returns it as JSON.

### View

`TFrecords.cshtml` is the masterlist/records screen.

It includes the records table and client-side logic for selecting a franchise and loading the record through the JSON endpoint.

Current conceptual flow:

```text
Masterlist row
      ↓
Select franchise
      ↓
JavaScript
      ↓
GET /Records/GetByFranchise
      ↓
JSON record
      ↓
Use/display record in UI
```

---

## Certificate / Franchise Module

`FranchiseController.cs` contains:

- `Index()`
- `Certificate()`

The module uses:

```text
Views/Home/certificateRegistration.cshtml
```

This provides a separate Certificate of Franchise workflow and also loads master records for client-side use.

---

## Other Controllers / Modules

The following currently exist but are incomplete or scaffolded:

- `AccountController`
- `AttendanceController`
- `SearchController`
- `IDSystemController`

`DiagnosticsController` is intended for developer/database diagnostics.

`Tfrsdb.cs` is effectively placeholder/dead code at the moment.

---

## Legacy / Disconnected Code

### `Repositories/ApplicationRepository.cs`

This appears to be an older database approach and is not the current active service architecture.

Do not base new features on it unless we intentionally decide to revive/refactor it.

### `server.js`

There is also an older Node/Express prototype. It is not part of the current active backend.

The current source of truth is:

```text
ASP.NET Core MVC
+
C#
+
MainTableService
+
OleDb
+
Access
```

---

## Current Functional State

### Working

- ASP.NET Core MVC application starts
- Main application form is available
- Model validation exists
- New franchise records can be inserted
- Existing records can be loaded
- Masterlist/records page works
- Franchise lookup endpoint works
- Certificate registration area exists
- **Full CRUD: Create, Read, Update, Delete all operational**
- All 27 model fields persisted to database
- GitHub repository is now established

### Partially Implemented

- Search and filtering (client-side only, no server-side)
- Account/authentication functionality
- Attendance functionality
- Database configuration portability
- ID system

---

## Important Technical Debt / Risks

### 1. Database file

`TFRSdb.accdb` is intentionally excluded from Git because the local database is very large.

It should remain local unless we deliberately design a separate database distribution/backup strategy.

### 2. Machine-specific database path

The connection configuration currently depends on a machine-specific database location.

This should eventually be changed to a portable or environment-based path.

### 3. CRUD is complete

The backend now supports: CREATE, READ, UPDATE, DELETE ✅

### 4. Legacy code removed

`server.js`, `Controllers/Tfrsdb.cs`, `Repositories/ApplicationRepository.cs` have been removed.

---

## Development Direction

The project is currently at the stage where the core registration and records workflow works, but the system is not finished.

A sensible development sequence is:

1. Stabilize the current database/model contract
2. Improve search and filtering
3. Finish the ID system
4. Add authentication/accounts and permissions
5. Complete attendance functionality
6. Improve certificate generation/printing
7. Improve validation and error handling
8. Make database configuration portable
9. Add production-focused logging, backups, and deployment documentation

---

## Git / GitHub

Repository:

```text
https://github.com/DeperaltaJap/TFRS
```

The repository is intended for source control and project history.

The local Access database is ignored by Git:

```gitignore
TFRSdb.accdb
```

Build/user files are also ignored:

```gitignore
bin/
obj/
.vs/
*.user
*.suo
```

---

## Working Rules for Future Development

When working on TFRS:

- Treat the active source code as the source of truth.
- Prefer `MainTableService` over the legacy repository.
- All 27 model properties should be persisted.
- Do not commit `TFRSdb.accdb`.
- Do not commit `bin/`, `obj/`, or `.vs/`.
- Preserve working functionality when adding features.
- Make changes in small, understandable commits.
- Update this document when a major module, architecture decision, database change, or workflow is introduced.

---

## Current Project Snapshot

**Project:** TFRS  
**Type:** Tricycle Franchise Registration System  
**Platform:** Web  
**Framework:** ASP.NET Core MVC  
**Runtime:** .NET 10  
**Database:** Microsoft Access  
**Data Access:** OleDb  
**Primary Table:** MAINTABLE (27 columns)  
**Current backend capability:** CREATE + READ + UPDATE + DELETE  
**Development status:** Working, unfinished  
**Version control:** GitHub  
**Primary repository:** `DeperaltaJap/TFRS`

> **Use this note as the Obsidian project-level reference. Update it whenever a major module, architecture decision, database change, or workflow is introduced.**
