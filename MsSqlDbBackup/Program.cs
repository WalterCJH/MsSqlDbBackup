using System;
using System.Data;
using System.Data.SqlClient;
//using System.IO.Compression;
using Ionic.Zip;

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
                string dirPath = args[3];
                if (isIntegratedSecurity == "1")
                {
                    conString += $" Integrated Security = True;";
                }
                else
                {
                    string userName = args[4];
                    string password = args[5];
                    conString += $" Integrated Security = False; uid = {userName}; password = {password};";
                }
                string fileName = $"{database}_{DateTime.Now:yyyyMMdd}";
                string bakPath = $@"{dirPath}\{fileName}.bak";
                SqlConnection con = new SqlConnection(conString);
                con.Open();

                string sqlStmt2 = $"ALTER DATABASE [{database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                SqlCommand bu2 = new SqlCommand(sqlStmt2, con);
                bu2.ExecuteNonQuery();

                string sqlStmt3 = $"BACKUP DATABASE [{database}] TO DISK='{bakPath}' WITH INIT;";
                SqlCommand bu3 = new SqlCommand(sqlStmt3, con);
                bu3.ExecuteNonQuery();

                string sqlStmt4 = $"ALTER DATABASE [{database}] SET MULTI_USER";
                SqlCommand bu4 = new SqlCommand(sqlStmt4, con);
                bu4.ExecuteNonQuery();

                //MessageBox.Show("database restoration done successefully");
                con.Close();

                using (var zip = new ZipFile())
                {
                    //    zip.Password = "P@ssW0rd";
                    zip.AddFile(bakPath, "");
                    zip.Save($@"{dirPath}\{fileName}.zip");
                }

            }
            catch (Exception ex)
            {
                //logger.Error(ex);
            }

            //Console.WriteLine("Hello World!");
        }
    }
}
