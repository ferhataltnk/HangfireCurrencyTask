using Core.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Abstract
{

    public interface ICurrencyService
    {
        public Result<bool> CurrencyReadXmlToWriteSql();
        public Result<DataTable> GetCurrenciesFromAPI();
    }
}
