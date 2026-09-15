using System;
using System.Collections.Generic;
using System.Data.OleDb;
using Microsoft.Extensions.Configuration;
using TFRS.Models;

namespace TFRS.Services
{
    public class MainTableService : IMainTableService
    {
        private readonly string _connectionString;

        public MainTableService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("AccessDb");
        }

        public MainTableRecord? GetByFranchiseNumber(string franchiseNumber)
        {
            using var conn = new OleDbConnection(_connectionString);
            conn.Open();

            using var cmd = new OleDbCommand("SELECT [DATE_SUBMITTED], [TRICYCLE_FRANCHISE_NUMBER], [NAME_OF_OPERATOR], [ADDRESS], [MAKE], [YEAR_MODEL], [COLOR], [ENGINE_NUMBER], [MV_FILE_NO], [CHASSIS_NUMBER], [PLATE_NUMBER], [DATE_ISSUED], [DATE_EXPIRED], [TODA], [YEAR_RENEW], [YEAR_EXPIRED], [STATUS], [REMARKS], [ROUTE_COVERED], [COMMITTEE_REPORT], [SB_APPROVED_DATE], [TYPE_OF_APPLICATION] FROM MAINTABLE WHERE [TRICYCLE_FRANCHISE_NUMBER]=?", conn);
            cmd.Parameters.AddWithValue("@p1", franchiseNumber ?? (object)DBNull.Value);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new MainTableRecord
                {
                    DateSubmitted = reader[0]?.ToString(),
                    TricycleFranchiseNumber = reader[1]?.ToString(),
                    NameOfOperator = reader[2]?.ToString(),
                    Address = reader[3]?.ToString(),
                    Make = reader[4]?.ToString(),
                    YearModel = reader.IsDBNull(5) ? null : (int?)Convert.ToInt32(reader[5]),
                    Color = reader[6]?.ToString(),
                    EngineNumber = reader[7]?.ToString(),
                    MVFileNo = reader[8]?.ToString(),
                    ChassisNumber = reader[9]?.ToString(),
                    PlateNumber = reader[10]?.ToString(),
                    DateIssued = reader.IsDBNull(11) ? null : (DateTime?)Convert.ToDateTime(reader[11]),
                    DateExpired = reader.IsDBNull(12) ? null : (DateTime?)Convert.ToDateTime(reader[12]),
                    TODA = reader[13]?.ToString(),
                    YearRenew = reader[14]?.ToString(),
                    YearExpired = reader[15]?.ToString(),
                    Status = reader[16]?.ToString(),
                    Remarks = reader[17]?.ToString(),
                    RouteCovered = reader[18]?.ToString(),
                    CommitteeReport = reader[19]?.ToString(),
                    SBApprovedDate = reader[20]?.ToString(),
                    TypeOfApplication = reader[21]?.ToString()
                };
            }

            return null;
        }

        public void Add(MainTableRecord record)
        {
            using var conn = new OleDbConnection(_connectionString);
            conn.Open();

            var sql = @"INSERT INTO MAINTABLE ([DATE_SUBMITTED], [TRICYCLE_FRANCHISE_NUMBER], [NAME_OF_OPERATOR], [ADDRESS], [MAKE], [YEAR_MODEL], [COLOR], [ENGINE_NUMBER], [MV_FILE_NO], [CHASSIS_NUMBER], [PLATE_NUMBER], [DATE_ISSUED], [DATE_EXPIRED], [TODA], [YEAR_RENEW], [YEAR_EXPIRED], [STATUS], [REMARKS], [ROUTE_COVERED], [COMMITTEE_REPORT], [SB_APPROVED_DATE], [TYPE_OF_APPLICATION]) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";

            using var cmd = new OleDbCommand(sql, conn);
            cmd.Parameters.AddWithValue("@p1", record.DateSubmitted ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p2", record.TricycleFranchiseNumber ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p3", record.NameOfOperator ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p4", record.Address ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p5", record.Make ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p6", record.YearModel.HasValue ? (object)record.YearModel.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@p7", record.Color ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p8", record.EngineNumber ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p9", record.MVFileNo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p10", record.ChassisNumber ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p11", record.PlateNumber ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p12", record.DateIssued.HasValue ? (object)record.DateIssued.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@p13", record.DateExpired.HasValue ? (object)record.DateExpired.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@p14", record.TODA ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p15", record.YearRenew ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p16", record.YearExpired ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p17", record.Status ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p18", record.Remarks ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p19", record.RouteCovered ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p20", record.CommitteeReport ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p21", record.SBApprovedDate ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p22", record.TypeOfApplication ?? (object)DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        public IEnumerable<MainTableRecord> GetAll()
        {
            var list = new List<MainTableRecord>();

            using var conn = new OleDbConnection(_connectionString);
            conn.Open();

            using var cmd = new OleDbCommand("SELECT [DATE_SUBMITTED], [TRICYCLE_FRANCHISE_NUMBER], [NAME_OF_OPERATOR], [ADDRESS], [MAKE], [YEAR_MODEL], [COLOR], [ENGINE_NUMBER], [MV_FILE_NO], [CHASSIS_NUMBER], [PLATE_NUMBER], [DATE_ISSUED], [DATE_EXPIRED], [TODA], [YEAR_RENEW], [YEAR_EXPIRED], [STATUS], [REMARKS], [ROUTE_COVERED], [COMMITTEE_REPORT], [SB_APPROVED_DATE], [TYPE_OF_APPLICATION] FROM MAINTABLE", conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var r = new MainTableRecord
                {
                    DateSubmitted = reader[0]?.ToString(),
                    TricycleFranchiseNumber = reader[1]?.ToString(),
                    NameOfOperator = reader[2]?.ToString(),
                    Address = reader[3]?.ToString(),
                    Make = reader[4]?.ToString(),
                    YearModel = reader.IsDBNull(5) ? null : (int?)Convert.ToInt32(reader[5]),
                    Color = reader[6]?.ToString(),
                    EngineNumber = reader[7]?.ToString(),
                    MVFileNo = reader[8]?.ToString(),
                    ChassisNumber = reader[9]?.ToString(),
                    PlateNumber = reader[10]?.ToString(),
                    DateIssued = reader.IsDBNull(11) ? null : (DateTime?)Convert.ToDateTime(reader[11]),
                    DateExpired = reader.IsDBNull(12) ? null : (DateTime?)Convert.ToDateTime(reader[12]),
                    TODA = reader[13]?.ToString(),
                    YearRenew = reader[14]?.ToString(),
                    YearExpired = reader[15]?.ToString(),
                    Status = reader[16]?.ToString(),
                    Remarks = reader[17]?.ToString(),
                    RouteCovered = reader[18]?.ToString(),
                    CommitteeReport = reader[19]?.ToString(),
                    SBApprovedDate = reader[20]?.ToString(),
                    TypeOfApplication = reader[21]?.ToString()
                };

                list.Add(r);
            }

            return list;
        }
    }
}
