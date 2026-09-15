using System.Data.OleDb;
using Microsoft.AspNetCore.Mvc;
using TFRS.Services;

namespace TFRS.Controllers
{
    public class DiagnosticsController : Controller
    {
        private readonly IMainTableService _mainTableService;
        private readonly IConfiguration _config;

        public DiagnosticsController(IMainTableService mainTableService, IConfiguration config)
        {
            _mainTableService = mainTableService;
            _config = config;
        }

        [HttpGet]
        public IActionResult CheckDatabase()
        {
            var diagnostics = new Dictionary<string, object>();

            try
            {
                // 1. Check connection string
                var connStr = _config.GetConnectionString("AccessDb");
                diagnostics["ConnectionString"] = connStr ?? "NOT FOUND";

                // 2. Check database file exists
                var match = System.Text.RegularExpressions.Regex.Match(connStr ?? "", @"Data Source=([^;]+)");
                var dbPath = match.Groups[1].Value;
                diagnostics["DatabasePath"] = dbPath;
                diagnostics["DatabaseFileExists"] = System.IO.File.Exists(dbPath);

                // 3. Try to connect and check table
                using (var conn = new OleDbConnection(connStr))
                {
                    conn.Open();
                    diagnostics["CanConnect"] = true;

                    // Check if MAINTABLE exists
                    var schema = conn.GetSchema("Tables");
                    var tables = schema.Rows.Cast<System.Data.DataRow>()
                        .Select(r => r["TABLE_NAME"].ToString())
                        .ToList();
                    diagnostics["Tables"] = string.Join(", ", tables ?? new List<string>());

                    // Try to get record count
                    using (var cmd = new OleDbCommand("SELECT COUNT(*) FROM MAINTABLE", conn))
                    {
                        try
                        {
                            var count = (int)cmd.ExecuteScalar();
                            diagnostics["MainTableRowCount"] = count;
                        }
                        catch (Exception ex)
                        {
                            diagnostics["MainTableRowCount_Error"] = ex.Message;
                        }
                    }

                    // Try to get sample data
                    using (var cmd = new OleDbCommand("SELECT TOP 1 * FROM MAINTABLE", conn))
                    {
                        try
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    diagnostics["SampleData_Found"] = true;
                                    var sampleRecord = new Dictionary<string, object>();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        sampleRecord[reader.GetName(i)] = reader[i];
                                    }
                                    diagnostics["SampleData"] = sampleRecord;
                                }
                                else
                                {
                                    diagnostics["SampleData_Found"] = false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            diagnostics["SampleData_Error"] = ex.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                diagnostics["Error"] = ex.Message;
                diagnostics["StackTrace"] = ex.StackTrace;
            }

            // 4. Try service GetAll
            try
            {
                var records = _mainTableService.GetAll();
                diagnostics["ServiceGetAll_Count"] = records.Count();
                if (records.Any())
                {
                    diagnostics["ServiceGetAll_FirstRecord"] = new
                    {
                        records.First().TricycleFranchiseNumber,
                        records.First().NameOfOperator,
                        records.First().Address
                    };
                }
            }
            catch (Exception ex)
            {
                diagnostics["ServiceGetAll_Error"] = ex.Message;
            }

            return Json(diagnostics);
        }
    }
}
