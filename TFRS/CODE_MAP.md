# CODE_MAP.md — Database Communication Code Map

## Files that Communicate with Database

```
TFRS (Root)
│
├── 📄 appsettings.json ⭐ CONNECTION STRING
│   └─ "ConnectionStrings": { "AccessDb": "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\ObsVault\\JapDev\\TFRS\\TFRSdb.accdb" }
│
├── 📄 Program.cs ⭐ DEPENDENCY INJECTION
│   └─ builder.Services.AddSingleton<IMainTableService, MainTableService>();
│
├── Services/ ⭐ DATABASE SERVICE LAYER
│   ├── IMainTableService.cs (Interface: GetAll / Add / GetByFranchiseNumber / Update / Delete)
│   └── MainTableService.cs ⭐⭐ ACTUAL DATABASE CODE
│       ├── Constructor reads ConnectionString from appsettings.json
│       ├── GetAll() → SELECT all 27 columns FROM MAINTABLE
│       ├── Add(record) → INSERT with 27 parameters
│       ├── GetByFranchiseNumber(id) → SELECT WHERE TRICYCLE_FRANCHISE_NUMBER=?
│       ├── Update(record) → UPDATE with 27 parameters WHERE TRICYCLE_FRANCHISE_NUMBER=?
│       └── Delete(franchiseNumber) → DELETE WHERE TRICYCLE_FRANCHISE_NUMBER=?
│
├── Controllers/
│   ├── HomeController.cs → Landing page / masterlist (TFRS.Controllers)
│   ├── ApplicationController.cs → Application form: Create/Index/Save/Edit/Delete
│   ├── RecordsController.cs → Records list + JSON lookup + Edit/Delete
│   ├── FranchiseController.cs → Certificate registration + Edit/Delete
│   ├── DiagnosticsController.cs → Dev-only DB health check
│   └── (Account, Attendance, IDSystem, Search → Stubs)
│
├── Models/
│   └── MainTableRecord.cs → 27 properties mapping MAINTABLE columns
│
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml → Landing page
│   │   ├── Applicationpage.cshtml → Application form (create/edit)
│   │   ├── TFrecords.cshtml → Records list + actions
│   │   ├── certificateRegistration.cshtml → Certificate + record actions
│   │   └── IDsystem.cshtml → ID system view
│   └── Shared/
│       ├── _Layout.cshtml
│       ├── Error.cshtml
│       └── _ValidationScriptsPartial.cshtml
│
└── wwwroot/
    ├── css/
    ├── js/
    └── images/
```

---

## Data Flow

```
Applicationpage.cshtml (form)
    │  POST /Application/Save  OR  POST /Application/Edit
    ▼
ApplicationController.Save/Edit(MainTableRecord)
    │  if ModelState.IsValid
    ▼
MainTableService.Add/Update(record)
    │  OleDbConnection → appsettings.json "AccessDb"
    ▼
TFRSdb.accdb → MAINTABLE (27 columns)

GET /Application/Edit?franchiseNumber=...
    → MainTableService.GetByFranchiseNumber()
    → Applicationpage.cshtml (edit mode)

POST /Application/Delete?franchiseNumber=...
    → MainTableService.Delete()
    → Redirect to Records
```

---

## Database Schema (MAINTABLE — 27 columns)

| # | Column Name | Type | Model Property |
|---|-------------|------|----------------|
| 1 | DATE_SUBMITTED | Text | DateSubmitted |
| 2 | TRICYCLE_FRANCHISE_NUMBER | Text | TricycleFranchiseNumber |
| 3 | NAME_OF_OPERATOR | Text | NameOfOperator |
| 4 | ADDRESS | Text | Address |
| 5 | MAKE | Text | Make |
| 6 | YEAR_MODEL | Number | YearModel |
| 7 | COLOR | Text | Color |
| 8 | ENGINE_NUMBER | Text | EngineNumber |
| 9 | MV_FILE_NO | Text | MVFileNo |
| 10 | CHASSIS_NUMBER | Text | ChassisNumber |
| 11 | PLATE_NUMBER | Text | PlateNumber |
| 12 | DATE_ISSUED | Date | DateIssued |
| 13 | DATE_EXPIRED | Date | DateExpired |
| 14 | TODA | Text | TODA |
| 15 | YEAR_RENEW | Text | YearRenew |
| 16 | YEAR_EXPIRED | Text | YearExpired |
| 17 | STATUS | Text | Status |
| 18 | REMARKS | Text | Remarks |
| 19 | ROUTE_COVERED | Text | RouteCovered |
| 20 | COMMITTEE_REPORT | Text | CommitteeReport |
| 21 | SB_APPROVED_DATE | Text | SBApprovedDate |
| 22 | ContactNo | Text | ContactNo |
| 23 | CommunityTaxNo | Text | CommunityTaxNo |
| 24 | PlaceIssued | Text | PlaceIssued |
| 25 | TypeOfOwnership | Text | TypeOfOwnership |
| 26 | VerifiedBy | Text | VerifiedBy |
| 27 | TYPE_OF_APPLICATION | Text | TypeOfApplication |

---

## Key Changes from Legacy

- **All 27 fields** now persisted (previously 22, missing ContactNo, CommunityTaxNo, PlaceIssued, TypeOfOwnership, VerifiedBy)
- **CRUD Complete**: CREATE ✅, READ ✅, UPDATE ✅, DELETE ✅
- **HomeController namespace fixed**: Changed from `TFRegistration.Controllers` to `TFRS.Controllers`
- **Dead code removed**: `server.js`, `Controllers/Tfrsdb.cs`, `Repositories/ApplicationRepository.cs`
- **All views wired**: Edit/Delete/View buttons functional across all pages

---

## OleDb Classes

| Class | Purpose |
|-------|---------|
| `OleDbConnection` | Opens/closes DB connection |
| `OleDbCommand` | Holds SQL query and parameters |
| `OleDbParameter` | Safe parameter binding |
| `OleDbDataReader` | Reads query results row-by-row |
