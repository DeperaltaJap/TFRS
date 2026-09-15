# 🗺️ TFRS Project Map

> Compiled by Claude on 2026-09-15 from the existing notes in `JapDev/TFRS/TFRS/*.md` **and verified directly against the current source files**, since several existing docs turned out to be stale (see ⚠️ Known Discrepancies below). Treat the "Verified" column as ground truth; treat prose in the old `*_FIX*`/`DATABASE_*` notes as historical context only.

---

## 1. What this project is

**TFRS** = a small ASP.NET Core MVC web app (.NET 10) for managing tricycle franchise records ("Tricycle Franchise Registration System"), backed by a Microsoft Access database (`TFRSdb.accdb`). There's also an abandoned Node/Express prototype (`server.js`) sitting in the same folder that is **not** part of the running app.

- **Solution file:** `JapDev/TFRS/TFRS.slnx`
- **Project file:** `JapDev/TFRS/TFRS/TFRS.csproj`
- **Entry point:** `JapDev/TFRS/TFRS/Program.cs`
- **Database file:** `JapDev/TFRS/TFRSdb.accdb` (Access, table `MAINTABLE`)

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
| `ApplicationController` | `/Application` | ✅ via `IMainTableService` | `Applicationpage.cshtml` | ⭐ Core — form create/save |
| `RecordsController` | `/Records` | ✅ via `IMainTableService` | `TFrecords.cshtml` | ⭐ Core — list + JSON lookup |
| `HomeController` | `/` (default) | ✅ via `IMainTableService` | `Index.cshtml` | ⭐ Core — landing/masterlist |
| `FranchiseController` | `/Franchise` | ✅ via `IMainTableService` | `certificateRegistration.cshtml` | Working |
| `DiagnosticsController` | `/Diagnostics` | ✅ (raw OleDb, for debugging) | JSON only | Dev tool, not user-facing |
| `AccountController` | `/Account` | ❌ | empty View() | Stub / unimplemented |
| `AttendanceController` | `/Attendance` | ❌ | empty View() | Stub / unimplemented |
| `SearchController` | `/Search` | ❌ | empty View() | Stub / unimplemented |
| `IDSystemController` | `/IDSystem` | ❌ | empty View() | Stub — ⚠️ see discrepancy below |
| `Tfrsdb.cs` (`tfrsdb` class) | n/a | ❌ | n/a | Dead placeholder, not a controller in practice |

---

## 5. Which file is "the right one" (services vs repositories)

There are **two** pieces of code that know how to write to the database, and only one is real:

- **`Services/MainTableService.cs`** — ✅ **This is the one that's actually used.** Registered in `Program.cs`, injected into every controller that needs data, used for both reads and writes.
- **`Repositories/ApplicationRepository.cs`** — ⚠️ **Not used anywhere.** It's never registered in DI and no controller references it. It's also internally inconsistent with the live schema (see below), so even if someone wired it up today it would fail. Treat this file as legacy/reference only, not something to edit expecting it to run.

If you're debugging "why didn't my data save," look at `MainTableService.cs`, not `ApplicationRepository.cs`.

---

## 6. ⚠️ Known discrepancies (docs vs. actual code)

The existing `*.md` notes in this folder (`CODE_MAP.md`, `DATABASE_CODE_REFERENCE.md`, `DOCUMENTATION_INDEX.md`, `START_HERE.md`, `ANSWER_DIRECT.md`, `VISUAL_GUIDE.md`, `6_FILES_VISUAL_OVERVIEW.md`, `DATABASE_HIGHLIGHTED_CODE.md`, `DATABASE_FINAL_SUMMARY.md`, `DATABASE_QUICK_INDEX.md`, `DATABASE_COMMUNICATION.md`) all describe the architecture correctly at a high level (form → controller → service → Access DB), but several **specifics no longer match the live code** — likely written before a later edit pass:

1. **Column naming.** The docs consistently show bracketed column names *with spaces*, e.g. `[DATE SUBMITTED]`, `[TRICYCLE FRANCHISE NUMBER]`. The **actual, current** `MainTableService.cs` uses **underscored** column names instead: `[DATE_SUBMITTED]`, `[TRICYCLE_FRANCHISE_NUMBER]`, `[NAME_OF_OPERATOR]`, etc. Only `ApplicationRepository.cs` (the dead file) still uses the space-style names and table name `[MAIN TABLE]` (with a space) instead of `MAINTABLE`.
2. **Database path.** The docs say the DB lives at `C:\jap\backend\TFRSdb.accdb`. The **actual** `appsettings.json` connection string points to `C:\ObsVault\JapDev\TFRS\TFRSdb.accdb`.
3. **Field count.** Docs describe "21 columns / 21 parameters." The live `MainTableService.Add()` actually binds **22 parameters** (`@p1`–`@p22`), including `TYPE_OF_APPLICATION`, which the docs don't mention.
4. **Model vs. persisted columns mismatch.** `MainTableRecord.cs` has `ContactNo`, `CommunityTaxNo`, `PlaceIssued`, `TypeOfOwnership`, and `VerifiedBy` properties (added per `SAVE_BUTTON_FIX.md`, presumably for the form), but `MainTableService.cs`'s SQL (`GetAll`, `Add`, `GetByFranchiseNumber`) **does not read or write any of these five fields**. If the form captures them, they're silently dropped before reaching the database today.
5. **`IDSystemController.Index()`** calls the default `return View()`, which by convention looks for `Views/IDSystem/Index.cshtml` — but no such file exists; only `Views/Home/IDsystem.cshtml` exists (different folder, different casing). As written, hitting `/IDSystem` will most likely throw a view-not-found error rather than render `IDsystem.cshtml`.
6. **`HomeController.cs`** is declared under namespace `TFRegistration.Controllers`, while every other controller uses `TFRS.Controllers`. Cosmetic inconsistency, but worth knowing if you're searching by namespace.
7. **`server.js`** is a separate, disconnected Node/Express + `node-adodb` prototype. It's broken as written (`dbPath` is `'C:\jap\backend\.accdb'` — missing the actual filename) and builds SQL via raw string interpolation (`WHERE [TRICYCLE FRANCHISE NUMBER] = '${franchiseId}'`), which is a SQL-injection pattern. It is not referenced by the .NET app and nothing currently runs it.

**Bottom line:** for anything about *current* behavior, trust `MainTableService.cs`, `appsettings.json`, and the controllers directly over the prose in the `DATABASE_*`/`MASTER_LIST_*`/`*_FIX*` markdown notes — those were accurate snapshots at the time they were written but have drifted.

---

## 7. The `MASTER_LIST_*` and other fix-log notes (historical)

These read as a running diary from an earlier debugging session, roughly in this order:

`MASTER_LIST_DIAGNOSTIC.md` → `MASTER_LIST_EMPTY_FIX.md` → `MASTER_LIST_FIXED.md` → `MASTER_LIST_FIX_COMPLETE.md` → `MASTER_LIST_NOW_FIXED.md` → `MASTER_LIST_SOLUTION_COMPLETE.md` → `MASTER_LIST_VISUAL_SUMMARY.md`

The end state they converge on — `HomeController` calling `_mainTableService.GetAll()` and populating `ViewBag.MainRecords` — **is** what's in the code today, confirmed directly. So this particular fix stuck. `FIX_COMPLETE_READY_TO_RUN.md` and `SAVE_BUTTON_FIX.md` describe the form-field-binding work, which also matches current `Applicationpage.cshtml`/`MainTableRecord.cs` (modulo the 5 unused model properties noted in §6.4). You likely don't need to open all seven Master List files — `MASTER_LIST_SOLUTION_COMPLETE.md` alone has the final diagnosis, fix, and data-flow diagram.

---

## 8. Existing documentation inventory (in `JapDev/TFRS/TFRS/`)

| File | Covers | Reliability |
|---|---|---|
| `DOCUMENTATION_INDEX.md` | Index/table of contents for all the `DATABASE_*` docs | Accurate as an index; underlying details have drifted (§6) |
| `START_HERE.md` | Same content as above, reading-path oriented | Same caveats |
| `ANSWER_DIRECT.md` | One-page "which file talks to the DB" answer | Correct at the architecture level |
| `CODE_MAP.md` | Directory tree + step-by-step execution trace | Correct structurally; column names/DB path stale (§6.1–6.2) |
| `DATABASE_COMMUNICATION.md` | Full guide, same territory as CODE_MAP | Same caveats |
| `DATABASE_CODE_REFERENCE.md` | Code excerpts with annotations | Excerpts are stale versions of the code, not current |
| `DATABASE_HIGHLIGHTED_CODE.md` | Same, with highlighting | Same caveats |
| `DATABASE_FINAL_SUMMARY.md` | Executive summary | Correct at a high level |
| `DATABASE_QUICK_INDEX.md` | FAQ-style quick answers | Correct at a high level |
| `VISUAL_GUIDE.md` / `6_FILES_VISUAL_OVERVIEW.md` | ASCII architecture diagrams | Correct at a high level |
| `MASTER_LIST_*.md` (7 files) | Debug log for the empty-masterlist bug | Final state matches current code (§7) |
| `SAVE_BUTTON_FIX.md` | Form field binding fix | Matches current code, except 5 fields never made it into the SQL layer (§6.4) |
| `FIX_COMPLETE_READY_TO_RUN.md` | Wrap-up note, "ready to run" status | Historical status note |

---

## 9. If you're about to make a change

- **Add a field end-to-end:** touch `MainTableRecord.cs` (model) → `Applicationpage.cshtml` (form) → `MainTableService.cs` SQL in **all three** methods (`GetAll`, `Add`, `GetByFranchiseNumber`) → confirm the Access column name (underscored, e.g. `[NEW_FIELD]`) actually exists in `TFRSdb.accdb`.
- **Debug "data isn't saving/showing":** check `DiagnosticsController` (`/Diagnostics/CheckDatabase`) first, then `MainTableService.cs`, then the connection string in `appsettings.json`. Don't waste time in `ApplicationRepository.cs` — it isn't wired up.
- **Ignore/delete candidates (with care):** `server.js`, `Controllers/Tfrsdb.cs`, `Repositories/ApplicationRepository.cs`, `Models/TodaList.cs` all appear to be dead weight — confirm with the project owner before removing.
