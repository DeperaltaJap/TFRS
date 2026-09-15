using System.Data.OleDb;
using TFRS.Models;

public class ApplicationRepository
{
    private readonly string _connectionString;

    public ApplicationRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("AccessConnection");
    }

    public void Save(MainTableRecord record)
    {
        using var conn = new OleDbConnection(_connectionString);

        string sql = @"
        INSERT INTO [MAIN TABLE]
        (
            [DATE SUBMITTED],
            [TRICYCLE FRANCHISE NUMBER],
            [NAME OF OPERATOR],
            [ADDRESS],
            [MAKE],
            [YEAR MODEL],
            [COLOR],
            [ENGINE NUMBER],
            [MV FILE NO],
            [CHASSIS NUMBER],
            [PLATE NUMBER],
            [DATE ISSUED],
            [DATE EXPIRED],
            [TODA],
            [YEAR RENEW],
            [YEAR EXPIRED],
            [STATUS],
            [REMARKS],
            [ROUTE COVERED],
            [COMMITTEE REPORT],
            [SB APPROVED DATE]
        )
        VALUES
        (
            ?,?,?,?,?,?,?,?,?,?,
            ?,?,?,?,?,?,?,?,?,?,
            ?
        )";

        using var cmd = new OleDbCommand(sql, conn);

        cmd.Parameters.AddWithValue("@p1", record.DateSubmitted);
        cmd.Parameters.AddWithValue("@p2", record.TricycleFranchiseNumber);
        cmd.Parameters.AddWithValue("@p3", record.NameOfOperator);
        cmd.Parameters.AddWithValue("@p4", record.Address);
        cmd.Parameters.AddWithValue("@p5", record.Make);
        cmd.Parameters.AddWithValue("@p6", record.YearModel);
        cmd.Parameters.AddWithValue("@p7", record.Color);
        cmd.Parameters.AddWithValue("@p8", record.EngineNumber);
        cmd.Parameters.AddWithValue("@p9", record.MVFileNo);
        cmd.Parameters.AddWithValue("@p10", record.ChassisNumber);
        cmd.Parameters.AddWithValue("@p11", record.PlateNumber);
        cmd.Parameters.AddWithValue("@p12", record.DateIssued);
        cmd.Parameters.AddWithValue("@p13", record.DateExpired);
        cmd.Parameters.AddWithValue("@p14", record.TODA);
        cmd.Parameters.AddWithValue("@p15", record.YearRenew);
        cmd.Parameters.AddWithValue("@p16", record.YearExpired);
        cmd.Parameters.AddWithValue("@p17", record.Status);
        cmd.Parameters.AddWithValue("@p18", record.Remarks);
        cmd.Parameters.AddWithValue("@p19", record.RouteCovered);
        cmd.Parameters.AddWithValue("@p20", record.CommitteeReport);
        cmd.Parameters.AddWithValue("@p21", record.SBApprovedDate);

        conn.Open();
        cmd.ExecuteNonQuery();
    }
}