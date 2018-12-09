using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataTableCollectionOfObjectsExchange
{
    public class DataTableCollectionOfObjectsExchange
    {
        //Convert List of objects to Datatable
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }

            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }

                dataTable.Rows.Add(values);

            }
            //put a breakpoint here and check datatable
            return dataTable;
        }


        //Using reflection to construct single C# object from Datatable having single record 
        //Need to test it for constructing collection of C# object from Datatable 
        public T BindData<T>(DataTable dt)
        {
            DataRow dr = dt.Rows[0];

            List<string> columns = new List<string>();
            foreach (DataColumn dc in dt.Columns)
            {
                columns.Add(dc.ColumnName);
            }

            var ob = Activator.CreateInstance<T>();

            var fields = typeof(T).GetFields();
            foreach (var fieldInfo in fields)
            {
                if (columns.Contains(fieldInfo.Name))
                {
                    fieldInfo.SetValue(ob, dr[fieldInfo.Name]);
                }
            }

            var properties = typeof(T).GetProperties();
            foreach (var propertyInfo in properties)
            {
                if (columns.Contains(propertyInfo.Name))
                {
                    // Fill the data into the property
                    //Below line is to avoid exception for case - 'Object of type 'System.DBNull' cannot be converted to type 'System.Nullable`1[System.Decimal]'.'

                    var propval = dr[propertyInfo.Name] == DBNull.Value ? null : dr[propertyInfo.Name];
                    //logging LWS = new logging();
                    //Encrypting the the Token(bigint) from dataset and assigning the encrypted string to TokenId property
                    //propval = propertyInfo.Name == "TokenId" ? LWS.Encrypt(propval.ToString()) : propval;
                    //if(propertyInfo.Name == "Amount")
                    //{
                    //    Convert.ToDecimal(propval) + 0.210
                    //}
                    //Below line to retain decimal value as string (18,3).. Normal decimal property round off it and removes unneccesary trailing zeroes
                    //propval = propertyInfo.Name == "Amount" ? propval.ToString() : propval;
                    propertyInfo.SetValue(ob, propval, null);
                }
            }

            return ob;
        }
    }
}
