using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            p.FileDBExchangeTEST();
            p.DataTableCollectionOfObjectsExchangeTEST();
        }
        public void FileDBExchangeTEST()
        {
            //System.Web.HttpPostedFileBase ExcelFile

            //DataTable dt= FileDBExchange.ExcelPackageExtensions.ExcelFileToDatatable(ExcelFile);

            //bool status=FileDBExchange.ExcelPackageExtensions.LoadDatatableToDatabase(ExcelFile, "", "", "", "", "");

            //==============
            //FileDBExchange.ExcelUtlity obj = new FileDBExchange.ExcelUtlity();
            //bool status= obj.WriteDataTableToExcel(dt, "Person Details", "D:\\testPersonExceldata.xlsx", "Details");
        }

        public void DataTableCollectionOfObjectsExchangeTEST()
        {
            DataTableCollectionOfObjectsExchange.DataTableCollectionOfObjectsExchange E = new DataTableCollectionOfObjectsExchange.DataTableCollectionOfObjectsExchange();

           //T t= E.BindData<T>(DataTable dt);
           //DataTable dt= E.ToDataTable<T>(List<T> t)
                
        }
    }
}
