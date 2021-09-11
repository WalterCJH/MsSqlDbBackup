using System;
using System.Data;
using System.Data.SqlClient;

namespace MsSqlDbBackup
{
    class Program
    {
        static void Main(string[] args)
        {
            //NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();


            try
            {
                string isIntegratedSecurity = args[0];
                string instance = args[1];
                string database = args[2];
                string conString = $"Data Source ={instance}; Initial Catalog = {database};";
                if (isIntegratedSecurity == "1")
                {
                    conString += $" Integrated Security = True;";
                }
                else
                {
                    string userName = args[3];
                    string password = args[4];
                    conString += $" Integrated Security = False; uid = {userName}; password = {password};";
                }
                string path = $@"C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\Backup\{database}_{DateTime.Now:yyyyMMdd}.bak";
                SqlConnection con = new SqlConnection(conString);
                con.Open();

                string sqlStmt2 = $"ALTER DATABASE [{database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                SqlCommand bu2 = new SqlCommand(sqlStmt2, con);
                bu2.ExecuteNonQuery();

                string sqlStmt3 = $"BACKUP DATABASE [{database}] TO DISK='{path}' WITH INIT;";
                SqlCommand bu3 = new SqlCommand(sqlStmt3, con);
                bu3.ExecuteNonQuery();

                string sqlStmt4 = $"ALTER DATABASE [{database}] SET MULTI_USER";
                SqlCommand bu4 = new SqlCommand(sqlStmt4, con);
                bu4.ExecuteNonQuery();

                //MessageBox.Show("database restoration done successefully");
                con.Close();

            }
            catch (Exception ex)
            {
                //logger.Error(ex);
            }

            //Console.WriteLine("Hello World!");
        }
    }
}
