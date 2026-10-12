using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace BCGAdminComplaint.Models
{
    public class DatabaseAccessEngine
    {
        #region Provider (Sql) Specific

        private static SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
        }

        private static SqlDataAdapter GetDataAdapter(IDbCommand command)
        {
            return new SqlDataAdapter((SqlCommand)command);
        }

        #endregion

        #region Transaction Handling

        public static SqlTransaction BeginTransaction()
        {
            SqlConnection connection = GetConnection();
            connection.Open();

            SqlTransaction transaction = connection.BeginTransaction();
            return transaction;
        }

        public static void CommitTransaction(SqlTransaction transaction)
        {
            SqlConnection connection = transaction.Connection;

            try
            {
                transaction.Commit();
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public static void RollbackTransaction(SqlTransaction transaction)
        {
            SqlConnection connection = transaction.Connection;

            try
            {
                transaction.Rollback();
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        #endregion

        #region Execute Stored Procedure

        public static int ExecuteNonQuery(ExecutionUnit unit)
        {
            SqlConnection connection = GetConnection();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = unit.StoredProcedureName;
            SqlParameter[] parameters = unit.Parameters;

            if (parameters != null)
            {
                for (int j = 0; j < parameters.Length; j++)
                {
                    command.Parameters.Add(parameters[j]);
                }
            }

            connection.Open();
            try
            {
                //set time out for the command, if not supplied in the connection then use 2 minutes
                //command.CommandTimeout = connection.ConnectionTimeout <= 0 ? 120 : connection.ConnectionTimeout;
                command.CommandTimeout = 120;

                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                command.Parameters.Clear();
                command.Dispose(); command = null;

                connection.Close();
                connection.Dispose(); connection = null;
            }
        }

        public static int ExecuteNonQuery(ExecutionUnit unit, SqlTransaction transaction)
        {
            SqlCommand command = null;
            try
            {
                SqlConnection connection = transaction.Connection;
                command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = unit.StoredProcedureName;
                SqlParameter[] parameters = unit.Parameters;

                if (parameters != null)
                {
                    for (int j = 0; j < parameters.Length; j++)
                    {
                        command.Parameters.Add(parameters[j]);
                    }
                }

                //set time out for the command, if not supplied in the connection then use 2 minutes
                //command.CommandTimeout = connection.ConnectionTimeout <= 0 ? 120 : connection.ConnectionTimeout;
                command.CommandTimeout = 120;
                return command.ExecuteNonQuery();
            }
            finally
            {
                command.Parameters.Clear();
                command.Dispose(); command = null;
            }
        }

        public static DataSet ExecuteDataSet(ExecutionUnit unit)
        {
            SqlConnection connection = GetConnection();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = unit.StoredProcedureName;
            SqlParameter[] parameters = unit.Parameters;

            for (int i = 0; i < parameters.Length; i++)
            {
                command.Parameters.Add(parameters[i]);
            }

            SqlDataAdapter adapter = GetDataAdapter(command);
            DataSet dataSet = new DataSet();

            connection.Open();
            try
            {
                //set time out for the command, if not supplied in the connection then use 2 minutes
                //command.CommandTimeout = connection.ConnectionTimeout <= 0 ? 120 : connection.ConnectionTimeout;
                command.CommandTimeout = 120;

                adapter.Fill(dataSet);
                return dataSet;
            }
            finally
            {
                command.Parameters.Clear();
                command.Dispose(); command = null;

                connection.Close();
                connection.Dispose(); connection = null;
            }
        }

        public static IDataReader ExecuteDataReader(ExecutionUnit unit)
        {
            SqlConnection connection = GetConnection();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = unit.StoredProcedureName;
            SqlParameter[] parameters = unit.Parameters;

            for (int i = 0; i < parameters.Length; i++)
            {
                command.Parameters.Add(parameters[i]);
            }

            connection.Open();

            try
            {
                //set time out for the command, if not supplied in the connection then use 2 minutes
                //command.CommandTimeout = connection.ConnectionTimeout <= 0 ? 120 : connection.ConnectionTimeout;
                command.CommandTimeout = 120;

                IDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                command.Dispose(); command = null;

                return reader;
            }
            catch (Exception ex)
            {
                connection.Close(); connection = null;
                throw ex;
            }
        }

        #endregion

        #region Parse Reader Values

        public static bool IsNull(IDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName));
        }

        public static string GetString(IDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName)) ? null : reader.GetString(reader.GetOrdinal(columnName));
        }

        public static int? GetNullableInt(IDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName)) ? null : (int?)reader.GetInt32(reader.GetOrdinal(columnName));
        }

        public static int GetInt(IDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName)) ? -1 : (int)reader.GetInt32(reader.GetOrdinal(columnName));
        }

        public static byte GetTinyInt(IDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName)) ? Convert.ToByte(-1) : (byte)reader.GetByte(reader.GetOrdinal(columnName));
        }

        public static byte? GetNullableTinyInt(IDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName)) ? null : (byte?)reader.GetByte(reader.GetOrdinal(columnName));
        }

        public static long GetInt64(IDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName)) ? -1 : reader.GetInt64(reader.GetOrdinal(columnName));
        }

        public static long? GetNullableInt64(IDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName)) ? null : (long?)reader.GetInt64(reader.GetOrdinal(columnName));
        }

        public static bool GetBoolean(IDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName)) ? false : reader.GetBoolean(reader.GetOrdinal(columnName));
        }

        public static bool? GetNullableBoolean(IDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName)) ? null : (bool?)reader.GetBoolean(reader.GetOrdinal(columnName));
        }

        public static double GetDouble(IDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName)) ? -1.0 : reader.GetDouble(reader.GetOrdinal(columnName));
        }

        public static double? GetDouble2(IDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName)) ? null : (double?)reader.GetDouble(reader.GetOrdinal(columnName));
        }

        public static decimal GetDecimal(IDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName)) ? -1 : reader.GetDecimal(reader.GetOrdinal(columnName));
        }

        public static decimal? GetDecimal2(IDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName)) ? null : (decimal?)reader.GetDecimal(reader.GetOrdinal(columnName));
        }

        public static decimal? GetNullableDecimal(IDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName)) ? null : (decimal?)reader.GetDecimal(reader.GetOrdinal(columnName));
        }

        public static DateTime GetDateTime(IDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName)) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal(columnName));
        }

        public static DateTime? GetNullableDateTime(IDataReader reader, string columnName)
        {
            return reader.IsDBNull(reader.GetOrdinal(columnName)) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal(columnName));
        }

        public static TimeSpan? GetNullableTimeSpan(IDataReader reader, string columnName)
        {
            //return reader.IsDBNull(reader.GetOrdinal(columnName)) ? null : (TimeSpan?)(Convert.ToDateTime(reader.GetDateTime(reader.GetOrdinal(columnName))).TimeOfDay);
            return reader.IsDBNull(reader.GetOrdinal(columnName)) ? null : (TimeSpan?)reader[columnName];
        }

        #endregion

    }
}
