using System.Configuration;

namespace OrderManagementSystem.util
{
    public class DBPropertyUtil
    {
        public static string GetConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}
