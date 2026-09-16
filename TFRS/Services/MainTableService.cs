using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
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

        private static readonly string[] AllColumns = {
            "[DATE_SUBMITTED]", "[TRICYCLE_FRANCHISE_NUMBER]", "[NAME_OF_OPERATOR]", "[ADDRESS]", "[MAKE]",
            "[YEAR_MODEL]", "[COLOR]", "[ENGINE_NUMBER]", "[MV_FILE_NO]", "[CHASSIS_NUMBER]",
            "[PLATE_NUMBER]", "[DATE_ISSUED]", "[DATE_EXPIRED]", "[TODA]", "[YEAR_RENEW]",
            "[YEAR_EXPIRED]", "[STATUS]", "[REMARKS]", "[ROUTE_COVERED]", "[COMMITTEE_REPORT]",
            "[SB_APPROVED_DATE]", "[ContactNo]", "[CommunityTaxNo]", "[PlaceIssued]",
            "[TypeOfOwnership]", "[VerifiedBy]", "[TYPE_OF_APPLICATION]"
        };

        private static readonly string ColumnList = string.Join(", ", AllColumns);

        private MainTableRecord MapReader(OleDbDataReader reader)
        {
            return new MainTableRecord
            {
                DateSubmitted = reader["DATE_SUBMITTED"]?.ToString(),
                TricycleFranchiseNumber = reader["TRICYCLE_FRANCHISE_NUMBER"]?.ToString(),
                NameOfOperator = reader["NAME_OF_OPERATOR"]?.ToString(),
                Address = reader["ADDRESS"]?.ToString(),
                Make = reader["MAKE"]?.ToString(),
                YearModel = reader.IsDBNull(reader.GetOrdinal("YEAR_MODEL")) ? null : (int?)Convert.ToInt32(reader["YEAR_MODEL"]),
                Color = reader["COLOR"]?.ToString(),
                EngineNumber = reader["ENGINE_NUMBER"]?.ToString(),
                MVFileNo = reader["MV_FILE_NO"]?.ToString(),
                ChassisNumber = reader["CHASSIS_NUMBER"]?.ToString(),
                PlateNumber = reader["PLATE_NUMBER"]?.ToString(),
                DateIssued = reader.IsDBNull(reader.GetOrdinal("DATE_ISSUED")) ? null : (DateTime?)Convert.ToDateTime(reader["DATE_ISSUED"]),
                DateExpired = reader.IsDBNull(reader.GetOrdinal("DATE_EXPIRED")) ? null : (DateTime?)Convert.ToDateTime(reader["DATE_EXPIRED"]),
                TODA = reader["TODA"]?.ToString(),
                YearRenew = reader["YEAR_RENEW"]?.ToString(),
                YearExpired = reader["YEAR_EXPIRED"]?.ToString(),
                Status = reader["STATUS"]?.ToString(),
                Remarks = reader["REMARKS"]?.ToString(),
                RouteCovered = reader["ROUTE_COVERED"]?.ToString(),
                CommitteeReport = reader["COMMITTEE_REPORT"]?.ToString(),
                SBApprovedDate = reader["SB_APPROVED_DATE"]?.ToString(),
                ContactNo = reader["ContactNo"]?.ToString(),
                CommunityTaxNo = reader["CommunityTaxNo"]?.ToString(),
                PlaceIssued = reader["PlaceIssued"]?.ToString(),
                TypeOfOwnership = reader["TypeOfOwnership"]?.ToString(),
                VerifiedBy = reader["VerifiedBy"]?.ToString(),
                TypeOfApplication = reader["TYPE_OF_APPLICATION"]?.ToString()
            };
        }

        public MainTableRecord? GetByFranchiseNumber(string franchiseNumber)
        {
            using var conn = new OleDbConnection(_connectionString);
            conn.Open();

            using var cmd = new OleDbCommand("SELECT " + ColumnList + " FROM MAINTABLE WHERE [TRICYCLE_FRANCHISE_NUMBER]=?", conn);
            cmd.Parameters.AddWithValue("@p1", franchiseNumber ?? (object)DBNull.Value);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return MapReader(reader);
            }

            return null;
        }

        public void Add(MainTableRecord record)
        {
            using var conn = new OleDbConnection(_connectionString);
            conn.Open();

            var placeholders = string.Join(",", AllColumns.Select((_, i) => $"@p{i + 1}"));
            var sql = $"INSERT INTO MAINTABLE ({ColumnList}) VALUES ({placeholders})";

            using var cmd = new OleDbCommand(sql, conn);
            AddParameters(cmd, record);
            cmd.ExecuteNonQuery();
        }

        public void Update(MainTableRecord record)
        {
            using var conn = new OleDbConnection(_connectionString);
            conn.Open();

            var setParts = new List<string>();
            int idx = 1;
            for (int i = 0; i < AllColumns.Length; i++)
            {
                if (AllColumns[i] == "[TRICYCLE_FRANCHISE_NUMBER]") { idx++; continue; }
                setParts.Add($"{AllColumns[i]}=@p{idx}");
                idx++;
            }
            var sql = $"UPDATE MAINTABLE SET {string.Join(", ", setParts)} WHERE [TRICYCLE_FRANCHISE_NUMBER]=@p{idx}";

            using var cmd = new OleDbCommand(sql, conn);
            int pIdx = 1;
            for (int i = 0; i < AllColumns.Length; i++)
            {
                if (AllColumns[i] == "[TRICYCLE_FRANCHISE_NUMBER]") continue;
                cmd.Parameters.AddWithValue($"@p{pIdx}", GetParamValue(record, i));
                pIdx++;
            }
            cmd.Parameters.AddWithValue($"@p{pIdx}", record.TricycleFranchiseNumber ?? (object)DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        private object GetParamValue(MainTableRecord record, int columnIndex)
        {
            return columnIndex switch
            {
                0 => record.DateSubmitted ?? (object)DBNull.Value,
                1 => record.TricycleFranchiseNumber ?? (object)DBNull.Value,
                2 => record.NameOfOperator ?? (object)DBNull.Value,
                3 => record.Address ?? (object)DBNull.Value,
                4 => record.Make ?? (object)DBNull.Value,
                5 => record.YearModel.HasValue ? (object)record.YearModel.Value : DBNull.Value,
                6 => record.Color ?? (object)DBNull.Value,
                7 => record.EngineNumber ?? (object)DBNull.Value,
                8 => record.MVFileNo ?? (object)DBNull.Value,
                9 => record.ChassisNumber ?? (object)DBNull.Value,
                10 => record.PlateNumber ?? (object)DBNull.Value,
                11 => record.DateIssued.HasValue ? (object)record.DateIssued.Value : DBNull.Value,
                12 => record.DateExpired.HasValue ? (object)record.DateExpired.Value : DBNull.Value,
                13 => record.TODA ?? (object)DBNull.Value,
                14 => record.YearRenew ?? (object)DBNull.Value,
                15 => record.YearExpired ?? (object)DBNull.Value,
                16 => record.Status ?? (object)DBNull.Value,
                17 => record.Remarks ?? (object)DBNull.Value,
                18 => record.RouteCovered ?? (object)DBNull.Value,
                19 => record.CommitteeReport ?? (object)DBNull.Value,
                20 => record.SBApprovedDate ?? (object)DBNull.Value,
                21 => record.ContactNo ?? (object)DBNull.Value,
                22 => record.CommunityTaxNo ?? (object)DBNull.Value,
                23 => record.PlaceIssued ?? (object)DBNull.Value,
                24 => record.TypeOfOwnership ?? (object)DBNull.Value,
                25 => record.VerifiedBy ?? (object)DBNull.Value,
                26 => record.TypeOfApplication ?? (object)DBNull.Value,
                _ => DBNull.Value
            };
        }

        public void Delete(string franchiseNumber)
        {
            using var conn = new OleDbConnection(_connectionString);
            conn.Open();

            using var cmd = new OleDbCommand("DELETE FROM MAINTABLE WHERE [TRICYCLE_FRANCHISE_NUMBER]=?", conn);
            cmd.Parameters.AddWithValue("@p1", franchiseNumber ?? (object)DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        public IEnumerable<MainTableRecord> GetAll()
        {
            var list = new List<MainTableRecord>();

            using var conn = new OleDbConnection(_connectionString);
            conn.Open();

            using var cmd = new OleDbCommand("SELECT " + ColumnList + " FROM MAINTABLE", conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(MapReader(reader));
            }

            return list;
        }

        private void AddParameters(OleDbCommand cmd, MainTableRecord record)
        {
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
            cmd.Parameters.AddWithValue("@p22", record.ContactNo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p23", record.CommunityTaxNo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p24", record.PlaceIssued ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p25", record.TypeOfOwnership ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p26", record.VerifiedBy ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@p27", record.TypeOfApplication ?? (object)DBNull.Value);
        }
    }
}
