// util/DBConnUtil.cs
using System.Data.SqlClient;

namespace OrderManagementSystem.util
{
    public class DBConnUtil
    {
        public static SqlConnection GetConnection()
        {
            string connString = DBPropertyUtil.GetConnectionString("DbConnection");
            return new SqlConnection(connString);
        }
    }
}
