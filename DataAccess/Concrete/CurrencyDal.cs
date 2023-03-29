using Core.Utilities.Results;
using DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace DataAccess.Concrete
{
    public class CurrencyDal : ICurrencyDal
    {
      
        public Result<bool> BulkInsertToSqlServer(DataTable dataTable)
        {
            try
            {
                using (var connection = new SqlConnection("Server=NB317493;Database=CURRENCY;Trusted_Connection=True"))
                {
                    connection.Open();
                    using (SqlBulkCopy sqlBulkCopy = new(connection))
                    {
                        sqlBulkCopy.DestinationTableName = "Currencies";
                        sqlBulkCopy.BulkCopyTimeout = 0;
                        dataTable.Columns.Cast<DataColumn>().ToList().ForEach(p => sqlBulkCopy.ColumnMappings.Add(p.ColumnName, destinationColumn: p.ColumnName));                  

                        sqlBulkCopy.WriteToServer(dataTable);
                    }
                    connection.Close();
                }
                return new Result<bool>(data: true, success: true, message: "Döviz kurları veritabanına başarıyla kaydedildi.");
            }
            catch (Exception e)
            {

                return new Result<bool>(data: false, success: false, message: $"Beklenmeyen bir hata oluştu.\n Detay:{e.Message}");

            }
        }
    }
}
