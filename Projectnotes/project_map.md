# 🗺️ TFRS Project Map

> Compiled by Claude on 2026-09-15 from the existing notes in `JapDev/TFRS/TFRS/*.md` **and verified directly against the current source files**, since several existing docs turned out to be stale (see ⚠️ Known Discrepancies below). Treat the "Verified" column as ground truth; treat prose in the old `*_FIX*`/`DATABASE_*` notes as historical context only.

---

## 1. What this project is

**TFRS** = a small ASP.NET Core MVC web app (.NET 10) for managing tricycle franchise records ("Tricycle Franchise Registration System"), backed by a Microsoft Access database (`TFRSdb.accdb`). Dead code (`server.js`, `Controllers/Tfrsdb.cs`, `Repositories/ApplicationRepository.cs`) has been removed.

- **Solution file:** `TFRS.slnx`
- **Project file:** `TFRS/TFRS.csproj`
- **Entry point:** `TFRS/Program.cs`
- **Database file:** `TFRSdb.accdb` (Access, table `MAINTABLE`, 27 columns)

---

## 2. Real source tree (noise removed)

`bin/`, `obj/`, and everything under them are **compiler/build output** — never edit these, they're regenerated from the files below. I've excluded them from this map. The only two non-source files worth knowing about there: `bin/Debug/net10.0/TFRS.exe` (the built app) and `obj/.../ApiEndpoints.json` (generated).

```
JapDev/
├── TFRS.slnx                         Solution file
├── TFRSdb.accdb                      ⭐ The actual database
├── Untitled.base / Untitled.canvas   Obsidian scratch files, unrelated to code
└── TFRS/                             (project root)
    ├── TFRS.csproj                   Project file (.NET 10, ASP.NET Core MVC)
    ├── Program.cs                    ⭐ App startup / DI / routing
    ├── appsettings.json              ⭐ Connection string lives here
    ├── appsettings.Development.json
    ├── server.js                     ⚠️ Dead Node/Express prototype — not wired into the app, don't confuse with the real backend
    │
    ├── Controllers/
    │   ├── HomeController.cs         Home page — loads all records into ViewBag
    │   ├── ApplicationController.cs  ⭐ Application form: Create/Index/Save (Save = the INSERT path)
    │   ├── RecordsController.cs      Records list page + JSON lookup by franchise #
    │   ├── FranchiseController.cs    Certificate registration page
    │   ├── AccountController.cs      Stub — Index() returns empty View()
    │   ├── AttendanceController.cs   Stub — Index() returns empty View()
    │   ├── IDSystemController.cs     Stub — Index() returns empty View() (see ⚠️ below, view mismatch)
    │   ├── SearchController.cs       Stub — Index() returns empty View()
    │   ├── DiagnosticsController.cs  Dev-only DB health check endpoint (/Diagnostics/CheckDatabase)
    │   └── Tfrsdb.cs                 ⚠️ Empty placeholder class `tfrsdb` — dead code, does nothing
    │
    ├── Models/
    │   ├── MainTableRecord.cs        ⭐ The record model (see field notes below)
    │   ├── TodaList.cs               Empty stub model (just an Id)
    │   └── ErrorViewModel.cs         Standard ASP.NET error page model
    │
    ├── Services/
    │   ├── IMainTableService.cs      Interface: GetAll / Add / GetByFranchiseNumber
    │   └── MainTableService.cs       ⭐⭐ THE file that actually talks to the database (OleDb)
    │
    ├── Repositories/
    │   └── ApplicationRepository.cs  ⚠️ DEAD CODE — alternate/older save path, not registered in DI, not called anywhere, and it's broken (see below)
    │
    ├── Views/
    │   ├── _ViewImports.cshtml / _ViewStart.cshtml
    │   ├── Home/
    │   │   ├── Index.cshtml               Home page — renders ViewBag.MainRecords from HomeController
    │   │   ├── Applicationpage.cshtml      ⭐ The application form (posts to Application/Save)
    │   │   ├── TFrecords.cshtml            Records list (rendered by RecordsController.Index)
    │   │   ├── IDsystem.cshtml             View for an ID system page — note casing, see ⚠️ below
    │   │   └── certificateRegistration.cshtml  Rendered by FranchiseController.Certificate()
    │   └── Shared/
    │       ├── _Layout.cshtml (+ .css)
    │       ├── Error.cshtml
    │       └── _ValidationScriptsPartial.cshtml
    │
    └── wwwroot/                      Static assets: css/, js/site.js, lib/ (bootstrap, jquery, jquery-validation), favicon.ico
```

---

## 3. The actual data flow (verified against current code, not the old docs)

```
Applicationpage.cshtml (form)
        │  POST /Application/Save
        ▼
ApplicationController.Save(MainTableRecord model)
        │  if ModelState.IsValid
        ▼
MainTableService.Add(record)          ← Services/MainTableService.cs
        │  OleDbConnection using appsettings.json "AccessDb" connection string
        ▼
TFRSdb.accdb → table MAINTABLE
```

Reads work the same way through `MainTableService.GetAll()` / `GetByFranchiseNumber()`, called from `HomeController`, `ApplicationController.Index()`, `RecordsController.Index()`, and `FranchiseController.Certificate()`.

**Registration (Program.cs, line 5):**
```csharp
builder.Services.AddSingleton<TFRS.Services.IMainTableService, TFRS.Services.MainTableService>();
```

---

## 4. Controllers — quick reference

| Controller | Route base | Real DB access? | Renders | Status |
|---|---|---|---|---|
| `ApplicationController` | `/Application` | ✅ via `IMainTableService` | `Applicationpage.cshtml` | ⭐ Core — form create/save/edit/delete |
| `RecordsController` | `/Records` | ✅ via `IMainTableService` | `TFrecords.cshtml` | ⭐ Core — list + JSON lookup + edit/delete |
| `HomeController` | `/` (default) | ✅ via `IMainTableService` | `Index.cshtml` | ⭐ Core — landing/masterlist |
| `FranchiseController` | `/Franchise` | ✅ via `IMainTableService` | `certificateRegistration.cshtml` | Working — certificate + edit/delete |
| `DiagnosticsController` | `/Diagnostics` | ✅ (raw OleDb, for debugging) | JSON only | Dev tool, not user-facing |
| `AccountController` | `/Account` | ❌ | empty View() | Stub / unimplemented |
| `AttendanceController` | `/Attendance` | ❌ | empty View() | Stub / unimplemented |
| `SearchController` | `/Search` | ❌ | empty View() | Stub / unimplemented |
| `IDSystemController` | `/IDSystem` | ❌ | empty View() | Stub |
| `Tfrsdb.cs` | n/a | ❌ | n/a | ❌ REMOVED — dead code |

---

## 5. Database access

`MainTableService.cs` is the sole database access layer. It is registered in `Program.cs` as a singleton and injected into all controllers that need data.

The legacy `Repositories/ApplicationRepository.cs` has been removed entirely.

---

## 6. ⚠️ Known discrepancies (docs vs. actual code)

### Fixed in latest update (2026-09-15)

1. **Column naming.** All column names now use underscores consistently. No more bracketed spaces.
2. **Database path.** `appsettings.json` correctly points to `C:\ObsVault\JapDev\TFRS\TFRSdb.accdb`.
3. **Field count.** All 27 columns are now persisted (was 22, missing 5 fields).
4. **Model vs. persisted columns mismatch.** FIXED — `ContactNo`, `CommunityTaxNo`, `PlaceIssued`, `TypeOfOwnership`, `VerifiedBy` are now all persisted in the database and in `MainTableService` SQL.
5. **`HomeController.cs` namespace.** FIXED — Changed from `TFRegistration.Controllers` to `TFRS.Controllers`.
6. **`Tfrsdb.cs` and `Repositories/ApplicationRepository.cs`.** REMOVED — Dead code cleaned up.
7. **`server.js`.** REMOVOVED — Dead Node/Express prototype deleted.
8. **CRUD completeness.** FIXED — Update and Delete methods added to `IMainTableService` and `MainTableService`.
9. **Controller actions.** All controllers now have proper Edit/Delete actions wired up.
10. **Views.** All Edit/Delete/View buttons are now functional across all pages.

### Remaining issues (low priority)

- Search is client-side only (no server-side search endpoint)
- `IDSystemController`, `SearchController`, `AccountController`, `AttendanceController` are still stubs
- Database path in `appsettings.json` is still machine-specific

---

## 7. The `MASTER_LIST_*` and other fix-log notes (historical)

These read as a running diary from an earlier debugging session, roughly in this order:

`MASTER_LIST_DIAGNOSTIC.md` → `MASTER_LIST_EMPTY_FIX.md` → `MASTER_LIST_FIXED.md` → `MASTER_LIST_FIX_COMPLETE.md` → `MASTER_LIST_NOW_FIXED.md` → `MASTER_LIST_SOLUTION_COMPLETE.md` → `MASTER_LIST_VISUAL_SUMMARY.md`

The end state they converge on — `HomeController` calling `_mainTableService.GetAll()` and populating `ViewBag.MainRecords` — **is** what's in the code today, confirmed directly. So this particular fix stuck. `FIX_COMPLETE_READY_TO_RUN.md` and `SAVE_BUTTON_FIX.md` describe the form-field-binding work, which also matches current `Applicationpage.cshtml`/`MainTableRecord.cs` (modulo the 5 unused model properties noted in §6.4). You likely don't need to open all seven Master List files — `MASTER_LIST_SOLUTION_COMPLETE.md` alone has the final diagnosis, fix, and data-flow diagram.

---

## 8. Current documentation inventory

| File | Covers | Status |
|---|---|---|
| `Projectnotes/TFRS_Project_Overview.md` | Full project overview, architecture, technical debt | Updated — CRUD complete |
| `Projectnotes/project_map.md` | Verified source tree, data flow, controller reference | Updated — all fixes documented |
| `TFRS/CODE_MAP.md` | Directory tree, execution trace, database schema | Updated — 27 columns, full CRUD |

### Old documentation (outdated, not updated)

The following files in `TFRS/` are stale and should not be relied upon:
- `DATABASE_CODE_REFERENCE.md`, `DATABASE_HIGHLIGHTED_CODE.md`, `DATABASE_FINAL_SUMMARY.md`, `DATABASE_QUICK_INDEX.md`, `DATABASE_COMMUNICATION.md`
- `START_HERE.md`, `ANSWER_DIRECT.md`, `VISUAL_GUIDE.md`, `6_FILES_VISUAL_OVERVIEW.md`
- `MASTER_LIST_*.md` (7 files), `SAVE_BUTTON_FIX.md`, `FIX_COMPLETE_READY_TO_RUN.md`
- `DOCUMENTATION_INDEX.md`

---

## 9. If you're about to make a change

- **Add a field end-to-end:** touch `MainTableRecord.cs` (model) → `Applicationpage.cshtml` / `certificateRegistration.cshtml` (forms) → `MainTableService.cs` SQL in **all three** methods (`GetAll`, `Add`, `GetByFranchiseNumber`, `Update`) → confirm the Access column name (underscored, e.g. `[NEW_FIELD]`) actually exists in `TFRSdb.accdb`.
- **Debug "data isn't saving/showing":** check `DiagnosticsController` (`/Diagnostics/CheckDatabase`) first, then `MainTableService.cs`, then the connection string in `appsettings.json`.
- **Ignore/delete candidates (with care):** `Models/TodaList.cs` appears to be dead weight — confirm with the project owner before removing. It is already excluded from all active SQL queries.
