using System.Collections.Generic;
using TFRS.Models;

namespace TFRS.Services
{
    public interface IMainTableService
    {
        IEnumerable<MainTableRecord> GetAll();
        void Add(MainTableRecord record);
        MainTableRecord? GetByFranchiseNumber(string franchiseNumber);
    }
}
