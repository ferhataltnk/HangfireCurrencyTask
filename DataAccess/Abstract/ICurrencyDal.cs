using Core.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstract
{
    public interface ICurrencyDal
    {
        public Result<DataTable> GetCurrenciesFromAPI();

        public Result<bool> BulkInsertToSqlServer(DataTable dataTable);
    }
}
