# Database Communication Code Map

## Files that Communicate with Database

```
TFRS (Root)
│
├── 📄 appsettings.json ⭐ CONNECTION STRING
│   │
│   └─ "ConnectionStrings": {
│      "AccessDb": "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\jap\\backend\\TFRSdb.accdb"
│      }
│
├── 📄 Program.cs ⭐ DEPENDENCY INJECTION
│   │
│   └─ builder.Services.AddSingleton<IMainTableService, MainTableService>();
│
├── Services/ ⭐ DATABASE SERVICE LAYER
│   │
│   ├── IMainTableService.cs (Interface)
│   │   ├─ GetAll()
│   │   ├─ Add(record)
│   │   └─ GetByFranchiseNumber(id)
│   │
│   └── MainTableService.cs (Implementation) ⭐ ACTUAL DATABASE CODE
│       │
│       ├─ Constructor
│       │   └─ Reads ConnectionString from appsettings.json
│       │
│       ├─ GetAll()
│       │   ├─ Creates OleDbConnection
│       │   ├─ Executes: SELECT * FROM MAINTABLE
│       │   ├─ Reads rows with OleDbDataReader
│       │   └─ Maps to MainTableRecord objects
│       │
│       ├─ Add(MainTableRecord)
│       │   ├─ Creates OleDbConnection
│       │   ├─ Executes: INSERT INTO MAINTABLE (...) VALUES (...)
│       │   ├─ Uses OleDbParameter for 21 columns
│       │   └─ Commits to database
│       │
│       └─ GetByFranchiseNumber(franchiseNumber)
│           ├─ Creates OleDbConnection
│           ├─ Executes: SELECT * FROM MAINTABLE WHERE FRANCHISE#=?
│           ├─ Reads single row
│           └─ Returns MainTableRecord
│
├── Controllers/ ⭐ CALLS DATABASE SERVICE
│   │
│   └── ApplicationController.cs
│       │
│       ├─ Index() [HttpGet]
│       │   ├─ Calls: _mainTableService.GetAll() ⭐ DATABASE QUERY
│       │   ├─ Gets list of all records
│       │   └─ Passes to View (Applicationpage.cshtml)
│       │
│       └─ Save(MainTableRecord model) [HttpPost]
│           ├─ Validates ModelState
│           ├─ Calls: _mainTableService.Add(model) ⭐ DATABASE INSERT
│           ├─ Saves record to database
│           └─ Redirects to Records page
│
├── Models/ ⭐ DATA STRUCTURE
│   │
│   └── MainTableRecord.cs
│       ├─ Maps to database columns
│       ├─ Properties match MAINTABLE fields
│       └─ Used by controller and service
│
├── Repositories/ (Alternative approach - not currently used)
│   │
│   └── ApplicationRepository.cs
│       └─ Alternative way to communicate with database
│
└── Views/ ⭐ USER INTERFACE
	│
	└── Home/
		└── Applicationpage.cshtml
			├─ Form submission POST to /Application/Save
			└─ Form data binds to MainTableRecord model

═══════════════════════════════════════════════════════════════
DATABASE
═══════════════════════════════════════════════════════════════
C:\jap\backend\TFRSdb.accdb
	│
	└── MAINTABLE (Database Table)
		├─ DATE SUBMITTED (column)
		├─ TRICYCLE FRANCHISE NUMBER (column)
		├─ NAME OF OPERATOR (column)
		├─ ... (18 more columns)
		└─ [Rows of data]
```

---

## Step-by-Step Code Execution

### 1️⃣ User Submits Form (Applicationpage.cshtml)

```html
<form id="application-form" 
	  asp-controller="Application" 
	  asp-action="Save" 
	  method="post">

	<input asp-for="NameOfOperator" />
	<input asp-for="Address" />
	<!-- ... more fields ... -->

	<button id="btn-save-record" type="submit">
		Save Record
	</button>
</form>
```

**What Happens:**
- User fills form → Clicks button → Form POSTs to `/Application/Save`

---

### 2️⃣ Controller Receives Request (ApplicationController.cs)

```csharp
[HttpPost]
public IActionResult Save(MainTableRecord model)
{
	// ASP.NET Model Binding automatically maps form data
	// to MainTableRecord properties

	if (ModelState.IsValid)
	{
		try
		{
			// ⭐ THIS IS WHERE DATABASE COMMUNICATION HAPPENS
			_mainTableService.Add(model);

			return RedirectToAction("Index", "Records");
		}
		catch
		{
			ModelState.AddModelError("", "Unable to save record.");
		}
	}

	return View("~/Views/Home/Applicationpage.cshtml", model);
}
```

**What Happens:**
- Controller validates form data
- Calls service method to save to database

---

### 3️⃣ Service Communicates with Database (MainTableService.cs)

```csharp
public class MainTableService : IMainTableService
{
	private readonly string _connectionString;

	// Constructor - called once at startup
	public MainTableService(IConfiguration config)
	{
		// ⭐ Gets connection string from appsettings.json
		_connectionString = config.GetConnectionString("AccessDb");
		// Result: "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\jap\\backend\\TFRSdb.accdb"
	}

	public void Add(MainTableRecord record)
	{
		// ⭐ OPENS CONNECTION
		using var conn = new OleDbConnection(_connectionString);
		conn.Open();  // Connects to TFRSdb.accdb

		// ⭐ BUILDS SQL QUERY
		var sql = @"INSERT INTO MAINTABLE 
				   ([DATE SUBMITTED], [TRICYCLE FRANCHISE NUMBER], ...) 
				   VALUES (?,?,?,...)";

		// ⭐ CREATES COMMAND
		using var cmd = new OleDbCommand(sql, conn);

		// ⭐ ADDS PARAMETERS (SAFE FROM SQL INJECTION)
		cmd.Parameters.AddWithValue("@p1", record.DateSubmitted ?? (object)DBNull.Value);
		cmd.Parameters.AddWithValue("@p2", record.TricycleFranchiseNumber ?? (object)DBNull.Value);
		cmd.Parameters.AddWithValue("@p3", record.NameOfOperator ?? (object)DBNull.Value);
		cmd.Parameters.AddWithValue("@p4", record.Address ?? (object)DBNull.Value);
		// ... 17 more parameters ...

		// ⭐ EXECUTES QUERY
		cmd.ExecuteNonQuery();  // INSERT happens here!

		// Connection closes automatically with 'using'
	}
}
```

**What Happens:**
- Gets connection string from appsettings.json
- Creates OleDbConnection to Access database
- Builds INSERT SQL query
- Safely adds all 21 form field values as parameters
- Executes query
- Record is inserted into database

---

### 4️⃣ Database Stores Data (TFRSdb.accdb)

```
MAINTABLE (Access Table)
═══════════════════════════════════════════════════
[ID] [DATE SUBMITTED] [FRANCHISE#] [OPERATOR] [ADDRESS] ... [APPROVAL DATE]
─────────────────────────────────────────────────────
 1   2024-01-15       TF-2024-001  Juan Dela Cruz  Barangay A  ... 2024-02-01
 2   2024-01-16       TF-2024-002  Maria Santos    Barangay B  ... 2024-02-02
 3   2024-01-17       TF-2024-003  Pedro Garcia    Barangay C  ... 2024-02-03
 ↑
 └─ NEW ROW INSERTED BY OleDbCommand.ExecuteNonQuery()
```

---

## Key Code Sections Explained

### A) Connection String (appsettings.json)
```json
"ConnectionStrings": {
  "AccessDb": "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\jap\\backend\\TFRSdb.accdb;Persist Security Info=False;"
}
```
**Explanation:**
- `Provider=Microsoft.ACE.OLEDB.12.0` = Use Access Database Engine
- `Data Source=C:\\jap\\...\\TFRSdb.accdb` = Path to database file
- `Persist Security Info=False` = Don't save login credentials

---

### B) Service Registration (Program.cs)
```csharp
builder.Services.AddSingleton<TFRS.Services.IMainTableService, TFRS.Services.MainTableService>();
```
**Explanation:**
- When code requests `IMainTableService`, ASP.NET provides `MainTableService`
- `AddSingleton` means one instance for entire application lifetime
- Automatically passes `IConfiguration` to constructor

---

### C) Database Operations

#### READ (GetAll)
```csharp
using var cmd = new OleDbCommand("SELECT ... FROM MAINTABLE", conn);
using var reader = cmd.ExecuteReader();  // Executes SELECT
while (reader.Read())
{
	// reader[0] = first column value
	// reader[1] = second column value
	// etc.
	var record = new MainTableRecord { ... };
}
```

#### INSERT (Add)
```csharp
using var cmd = new OleDbCommand(
	"INSERT INTO MAINTABLE (...) VALUES (?,?,...)", conn);
cmd.Parameters.AddWithValue("@p1", value1);
cmd.Parameters.AddWithValue("@p2", value2);
cmd.ExecuteNonQuery();  // Executes INSERT
```

#### UPDATE (Not currently implemented but would be)
```csharp
using var cmd = new OleDbCommand(
	"UPDATE MAINTABLE SET [COL1]=? WHERE [ID]=?", conn);
cmd.Parameters.AddWithValue("@p1", newValue);
cmd.Parameters.AddWithValue("@p2", id);
cmd.ExecuteNonQuery();  // Executes UPDATE
```

---

## OleDb Classes Explained

| Class | What It Does | Example |
|-------|-------------|---------|
| `OleDbConnection` | Opens/closes DB connection | `var conn = new OleDbConnection(connString);` |
| `OleDbCommand` | Holds SQL query and parameters | `new OleDbCommand(sql, conn)` |
| `OleDbParameter` | Safe parameter binding | `cmd.Parameters.AddWithValue("@p1", value)` |
| `OleDbDataReader` | Reads query results row-by-row | `while (reader.Read()) { ... }` |

---

## Common Database Operations in This App

### 1. Load All Records (Masterlist)
```csharp
// Called in: ApplicationController.Index()
var records = _mainTableService.GetAll();
// Executes: SELECT [...all columns...] FROM MAINTABLE
// Returns: List<MainTableRecord>
```

### 2. Save New Record (Form Submission)
```csharp
// Called in: ApplicationController.Save()
_mainTableService.Add(model);
// Executes: INSERT INTO MAINTABLE (...) VALUES (...)
// Updates: Database with new row
```

### 3. Load Single Record (View/Edit)
```csharp
// Called in: JavaScript in Applicationpage.cshtml
var record = _mainTableService.GetByFranchiseNumber(franchiseNumber);
// Executes: SELECT [...] FROM MAINTABLE WHERE FRANCHISE# = ?
// Returns: MainTableRecord or null
```

---

## Summary Table

| Location | Component | Purpose |
|----------|-----------|---------|
| `appsettings.json` | 🔐 Connection String | Tells app where database is |
| `Program.cs` | 📦 Service Registration | Makes service available to controllers |
| `ApplicationController.cs` | 🎮 Controller | Receives form data, calls service |
| `MainTableService.cs` | 💾 Service | Creates connections and executes SQL |
| `IMainTableService.cs` | 📋 Interface | Defines what service methods exist |
| `MainTableRecord.cs` | 📊 Model | Maps database columns to C# properties |
| `TFRSdb.accdb` | 🗄️ Database | Actually stores the data |

