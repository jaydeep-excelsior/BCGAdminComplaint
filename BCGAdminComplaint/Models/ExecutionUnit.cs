using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCGAdminComplaint.Models
{
    public class ExecutionUnit
    {
        public ExecutionUnit(string storedProcedureName)
        {
            this.storedProcedureName = storedProcedureName;
            this.storedProcedureParameterList = new List<SqlParameter>();
            this.storedProcedureParameterNameList = new List<string>();
        }

        #region Properties

        private string storedProcedureName;
        public string StoredProcedureName
        {
            set
            {
                storedProcedureName = value;
            }
            get
            {
                return storedProcedureName;
            }
        }

        private List<SqlParameter> storedProcedureParameterList;
        private List<string> storedProcedureParameterNameList;

        internal SqlParameter[] Parameters
        {
            get
            {
                return storedProcedureParameterList.ToArray();
            }
        }

        #endregion

        #region Indexer

        public IDataParameter this[int index]
        {
            get
            {
                return storedProcedureParameterList[index];
            }
        }

        public IDataParameter this[string parameterName]
        {
            get
            {
                return storedProcedureParameterList[storedProcedureParameterNameList.IndexOf(parameterName)];
            }
        }

        #endregion

        #region Parameters Handling (Sql Specific)

        public SqlParameter AddParameter(string name, SqlDbType type, ParameterDirection direction, object value)
        {
            SqlParameter parameter = new SqlParameter(name, type);
            parameter.Direction = direction;
            parameter.Value = value == null ? DBNull.Value : value;

            storedProcedureParameterList.Add(parameter);
            storedProcedureParameterNameList.Add(parameter.ParameterName);

            return parameter;
        }

        public SqlParameter AddParameter(string name, SqlDbType type, ParameterDirection direction, object value, string arrayTypeName)
        {
            SqlParameter parameter = new SqlParameter(name, type);
            parameter.Direction = direction;
            parameter.UdtTypeName = arrayTypeName;
            parameter.Value = value == null ? DBNull.Value : value;

            storedProcedureParameterList.Add(parameter);
            storedProcedureParameterNameList.Add(parameter.ParameterName);

            return parameter;
        }

        #endregion
    }
}
