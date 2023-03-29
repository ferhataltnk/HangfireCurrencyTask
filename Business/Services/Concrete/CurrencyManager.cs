using Business.Services.Abstract;
using Core.Utilities.Results;
using DataAccess.Abstract;
using DataAccess.Concrete;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Business.Services.Concrete
{
    public class CurrencyManager : ICurrencyService
    {
        private readonly ICurrencyDal _currencyDal;

        public CurrencyManager(ICurrencyDal currencyDal)
        {
            _currencyDal = currencyDal;
        }


        public Result<bool> CurrencyReadXmlToWriteSql()
        {
            try
            {
                var getCurrenciesResult = GetCurrenciesFromAPI();

                if (getCurrenciesResult.Success == false)
                {
                    //loglanacak Error Message
                    return new Result<bool>(data: false, success: false, message: getCurrenciesResult.Message);
                }
                else
                {
                    DataTable dataTable = getCurrenciesResult.Data;
                    _currencyDal.BulkInsertToSqlServer(dataTable);
                    return new Result<bool>(data: true, success: true, message: "Döviz kurları veritabanına başarıyla kaydedildi.");
                }
            }
            catch (Exception e)
            {

                return new Result<bool>(data: false, success: false, message: $"Beklenmeyen bir hata oluştu.\n Detay:{e.Message}");

            }
        }
        
        public Result<DataTable> GetCurrenciesFromAPI()
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load("https://www.tcmb.gov.tr/kurlar/today.xml");

                XmlNodeList list = xmlDocument.SelectNodes("//Tarih_Date/Currency");
                DateTime date = Convert.ToDateTime(xmlDocument.SelectSingleNode("Tarih_Date").Attributes["Tarih"].Value);



                DataTable dataTable = new DataTable();

                // Create the columns of the DataTable based on the XML elements
                XmlNodeList xmlNodes = xmlDocument.SelectNodes("//Tarih_Date/Currency");

                DateTime tcmbCurrencyDateValue = Convert.ToDateTime(xmlDocument.SelectSingleNode("Tarih_Date").Attributes["Tarih"].Value);


                DataColumn currencyCode = new DataColumn();
                currencyCode.ColumnName = "CurrencyCode";
                currencyCode.AllowDBNull = false;
                currencyCode.Unique = true;
                currencyCode.DataType = typeof(string);

                DataColumn currencyName = new DataColumn();
                currencyName.ColumnName = "CurrencyName";
                currencyName.AllowDBNull = true;
                currencyName.DataType = typeof(string);

                DataColumn currencyNameTr = new DataColumn();
                currencyNameTr.ColumnName = "CurrencyNameTr";
                currencyNameTr.AllowDBNull = true;
                currencyNameTr.DataType = typeof(string);

                DataColumn forexBuying = new DataColumn();
                forexBuying.ColumnName = "ForexBuying";
                forexBuying.AllowDBNull = true;
                forexBuying.DataType = typeof(float);

                DataColumn forexSelling = new DataColumn();
                forexSelling.ColumnName = "ForexSelling";
                forexSelling.AllowDBNull = true;
                forexSelling.DataType = typeof(float);

                DataColumn tcmbCurrencyDate = new DataColumn();
                tcmbCurrencyDate.ColumnName = "TcmbCurrencyDate";
                tcmbCurrencyDate.AllowDBNull = true;
                tcmbCurrencyDate.DataType = typeof(DateTime);

                DataColumn isActive = new DataColumn();
                isActive.ColumnName = "IsActive";
                isActive.AllowDBNull = false;
                isActive.DataType = typeof(bool);


                DataColumn isDeleted = new DataColumn();
                isDeleted.ColumnName = "IsDeleted";
                isDeleted.AllowDBNull = false;
                isDeleted.DataType = typeof(bool);


                DataColumn createdTime = new DataColumn(); // createdDate --> createdTime
                createdTime.ColumnName = "CreatedTime";
                createdTime.AllowDBNull = false;
                createdTime.DataType = typeof(DateTime);

                DataColumn createdUserId = new DataColumn(); // createdDate --> createdTime
                createdUserId.ColumnName = "CreatedUserId";
                createdUserId.AllowDBNull = false;
                createdUserId.DataType = typeof(int);

                DataColumn modifiedTime = new DataColumn();
                modifiedTime.ColumnName = "ModifiedTime";
                modifiedTime.AllowDBNull = false;
                modifiedTime.DataType = typeof(DateTime);

                DataColumn modifiedUserId = new DataColumn(); // createdDate --> createdTime
                modifiedUserId.ColumnName = "ModifiedUserId";
                modifiedUserId.AllowDBNull = false;
                modifiedUserId.DataType = typeof(int);

                DataColumn deletedTime = new DataColumn();
                deletedTime.ColumnName = "DeletedTime";
                deletedTime.AllowDBNull = true;
                deletedTime.DataType = typeof(DateTime);

                DataColumn deletedUserId = new DataColumn(); // createdDate --> createdTime
                deletedUserId.ColumnName = "DeletedUserId";
                deletedUserId.AllowDBNull = true;
                deletedUserId.DataType = typeof(int);


                dataTable.Columns.Add(currencyCode);
                dataTable.Columns.Add(currencyName);
                dataTable.Columns.Add(currencyNameTr);
                dataTable.Columns.Add(forexBuying);
                dataTable.Columns.Add(forexSelling);
                dataTable.Columns.Add(tcmbCurrencyDate);
                dataTable.Columns.Add(isActive);
                dataTable.Columns.Add(isDeleted);
                dataTable.Columns.Add(createdTime);
                dataTable.Columns.Add(createdUserId);
                dataTable.Columns.Add(modifiedTime);
                dataTable.Columns.Add(modifiedUserId);
                dataTable.Columns.Add(deletedTime);
                dataTable.Columns.Add(deletedUserId);





                // Loop through the XML nodes and add the data to the DataTable
                foreach (XmlNode xmlNode in xmlNodes)
                {
                    DataRow dataRow = dataTable.NewRow();


                    dataRow[xmlNode.Attributes["CurrencyCode"].Name] = xmlNode.Attributes["CurrencyCode"].Value;




                    foreach (XmlNode xmlText in xmlNode.ChildNodes)
                    {
                        if (xmlText.Name == "CurrencyName")
                        {
                            dataRow["CurrencyName"] = xmlText.InnerText;

                        }
                    }

                    foreach (XmlNode xmlText in xmlNode.ChildNodes)
                    {
                        if (xmlText.Name == "Isim")
                        {
                            dataRow["CurrencyNameTr"] = xmlText.InnerText;

                        }
                    }


                    foreach (XmlNode xmlText in xmlNode.ChildNodes)
                    {
                        if (xmlText.Name == "ForexBuying" || xmlText.Name == "ForexSelling")
                        {
                            double decimalC;
                            var value = double.TryParse(xmlText.InnerText, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-GB"), out decimalC);
                            dataRow[xmlText.Name] = decimalC;
                        }
                    }

                    dataRow["TcmbCurrencyDate"] = tcmbCurrencyDateValue;
                    dataRow["IsActive"] = true;
                    dataRow["IsDeleted"] = false;
                    dataRow["CreatedTime"] = DateTime.Now;
                    dataRow["CreatedUserId"] = 1;
                    dataRow["ModifiedTime"] = DateTime.Now;
                    dataRow["ModifiedUserId"] = 1;
                    //dataRow["DeletedTime"] = null;
                    //dataRow["DeletedUserId"] = null;

                    dataTable.Rows.Add(dataRow);
                }

                return new Result<DataTable>(data: dataTable, success: true, message: "Döviz kur bilgileri başarıyla getirildi.");
            }
            catch (Exception e)
            {

                return new Result<DataTable>(data: null, success: false, message: $"Beklenmeyen bir hata oluştu.\n Detay:{e.Message}");
            }
        }


    }
}
