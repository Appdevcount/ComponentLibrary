using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

//HttpPostedFileBase Class -- to use this Upload type below assembly refrence added manually
//Namespace:System.Web
//Assemblies:System.Web.dll, System.Web.Abstractions.dll

//The nuget package name is EPPlus, 

namespace FileDBExchange //https://www.mikesdotnetting.com/article/277/reading-excel-files-without-saving-to-disk-in-asp-net
{
    public static class ExcelPackageExtensions
    {

        //Excel to Datatable  //I declared this as private to make it not accesible publicly because it needs Excelpackage as parameter, so created another public method with stream of excel file as parameter to return DataTable
        private static DataTable ExcelPackageToDataTable(this ExcelPackage package)
        {
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            DataTable table = new DataTable();
            foreach (var firstRowCell in workSheet.Cells[1, 1, 1, workSheet.Dimension.End.Column])
            {
                table.Columns.Add(firstRowCell.Text);
            }

            for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
            {
                var row = workSheet.Cells[rowNumber, 1, rowNumber, workSheet.Dimension.End.Column];
                var newRow = table.NewRow();
                foreach (var cell in row)
                {
                    newRow[cell.Start.Column - 1] = cell.Text;
                }
                table.Rows.Add(newRow);
            }
            return table;
        }
        public static DataTable ExcelFileToDatatable(HttpPostedFileBase OnlyExcelFile)
        {
            ExcelPackage excel = new ExcelPackage(OnlyExcelFile.InputStream);

            DataTable dt = excel.ExcelPackageToDataTable();

            return dt;
        }

        ////Using below way we can load the data from excel file to DataTable and can use that
        public static bool LoadDatatableToDatabase(HttpPostedFileBase File, string DBTable, string DBServer, string DBName, string DBUser, string DBPwd)
        {
            try
            {
              
                ExcelPackage excel = new ExcelPackage(File.InputStream);

                var dt = excel.ExcelPackageToDataTable();
                //var table = "Contacts";
                //using (var conn = new SqlConnection("Server=.;Database=test;Integrated Security=SSPI"))
                using (var conn = new SqlConnection("Data Source=" + DBServer + ";user id=" + DBUser + ";password=" + DBPwd + ";initial catalog=" + DBName + ";Connect Timeout=1200"))

                {
                    var bulkCopy = new SqlBulkCopy(conn);
                    bulkCopy.DestinationTableName = DBTable;
                    conn.Open();
                    var schema = conn.GetSchema("Columns", new[] { null, null, DBTable, null });
                    foreach (DataColumn sourceColumn in dt.Columns)
                    {
                        foreach (DataRow row in schema.Rows)
                        {
                            if (string.Equals(sourceColumn.ColumnName, (string)row["COLUMN_NAME"], StringComparison.OrdinalIgnoreCase))
                            {
                                bulkCopy.ColumnMappings.Add(sourceColumn.ColumnName, (string)row["COLUMN_NAME"]);
                                break;
                            }
                        }
                    }
                    bulkCopy.WriteToServer(dt);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        
    }

}
