using Business.Abstract;
using Core.Utilities.Results;
using DataAccess.Abstract;
using DataAccess.Concrete;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class CurrencyManager:ICurrencyService
    {
        private readonly ICurrencyDal _currencyDal;

        public CurrencyManager(ICurrencyDal currencyDal)
        {
            _currencyDal = currencyDal;
        }


        public Result<bool> CurrencyReadXmlWriteSql()
        {
            try
            {
                
                DataTable dataTable = _currencyDal.GetCurrenciesFromAPI().Data;
                _currencyDal.BulkInsertToSqlServer(dataTable);
                return new Result<bool>(data: true, success: true, message: "Döviz kurları veritabanına başarıyla kaydedildi.");

            }
            catch (Exception e)
            {

                return new Result<bool>(data: false, success: false, message: $"Beklenmeyen bir hata oluştu.\n Detay:{e.Message}");

            }
        }
    }
}
