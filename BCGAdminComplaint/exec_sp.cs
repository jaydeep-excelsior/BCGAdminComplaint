using System;
using System.Data.SqlClient;
using System.IO;

class Program
{
    static void Main()
    {
        string connStr = "Data Source=tcp:68.178.173.156,55866;Initial Catalog=Barcouncil_27082026; User ID=sa; Password=Adminbcg@272707071";
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();
            string script = File.ReadAllText("SP_Get_ComplaintList.sql");
            
            // split by GO if needed, but since it's just ALTER PROCEDURE, we can execute directly if we drop GOs
            string[] batches = script.Split(new string[] { "\r\nGO", "\nGO" }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (string batch in batches)
            {
                if (string.IsNullOrWhiteSpace(batch)) continue;
                using (SqlCommand cmd = new SqlCommand(batch, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        Console.WriteLine("Done");
    }
}
