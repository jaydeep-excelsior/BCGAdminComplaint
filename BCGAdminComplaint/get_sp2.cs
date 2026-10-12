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
            using (SqlCommand cmd = new SqlCommand("sp_helptext", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@objname", "SP_DcCommittee_Mst");
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    using (StreamWriter sw = new StreamWriter("SP_DcCommittee_Mst.sql"))
                    {
                        while (reader.Read())
                        {
                            sw.Write(reader[0].ToString());
                        }
                    }
                }
            }
        }
        Console.WriteLine("Done");
    }
}
