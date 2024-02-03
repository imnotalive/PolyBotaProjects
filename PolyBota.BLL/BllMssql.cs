using PolyBota.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyBota.BLL
{
    public static class BllMssql
    {
        private static SqlConnection SqlConBringBy(DatabaseConnectionRecord db)
        {

            var conStr = string.Format("Server={0}; Database={1};persist security info=True;user id={2};password={3};Integrated Security=false;connection timeout=5", db.IpAdress, db.DatabaseName, db.UserName, db.Password).Replace(@"\\", @"\");

            SqlConnection con = new SqlConnection(conStr);

            return con;
        }
        public static List<Dictionary<string, object>> CustomSql(string query, DatabaseConnectionRecord db)
        {
            DataTable dt = new DataTable();

            SqlConnection con = SqlConBringBy(db);
            con.Open();
            try
            {
                SqlCommand cmd = new SqlCommand(query, con);
               
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dtSchema = dr.GetSchemaTable();

                // You can also use an ArrayList instead of List<>
                List<DataColumn> listCols = new List<DataColumn>();

                if (dtSchema != null)
                {
                    foreach (DataRow drow in dtSchema.Rows)
                    {
                        string columnName = System.Convert.ToString(drow["ColumnName"]);
                        DataColumn column = new DataColumn(columnName, (Type)(drow["DataType"]));
                        column.Unique = (bool)drow["IsUnique"];
                        column.AllowDBNull = (bool)drow["AllowDBNull"];
                        column.AutoIncrement = (bool)drow["IsAutoIncrement"];
                        listCols.Add(column);
                        dt.Columns.Add(column);
                    }
                }

                // Read rows from DataReader and populate the DataTable

                while (dr.Read())
                {
                    DataRow dataRow = dt.NewRow();
                    for (int i = 0; i < listCols.Count; i++)
                    {
                        dataRow[listCols[i]] = dr[i];
                    }
                    dt.Rows.Add(dataRow);
                }
            }
            catch (Exception e)
            {
                var itt = new Dictionary<string, string>();
                itt.Add("hata", e.Message);
            }
            con.Close();
            return dt.AsEnumerable().Select(
                row => dt.Columns.Cast<DataColumn>().ToDictionary(
                    column => column.ColumnName,
                    column => row[column])).ToList();
        }
    }
}
