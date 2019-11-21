using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web;
namespace NTS.Utils.BulkUploads
{
    public class SqlUtil
    {
        public static string GetConStr(string cnName)
        {

            
            return cnName;
        }
        public static SqlCommand GetSqlCommand(SqlConnection connection, string cmdText, object[] inputParams = null)
        {
            var command = new SqlCommand(cmdText, connection);
            command.CommandType = cmdText.Contains(" ") ? System.Data.CommandType.Text : System.Data.CommandType.StoredProcedure;

            if (inputParams != null && inputParams.Length > 0)
                CreateInputParams(command, inputParams);

            return command;
        }
        public static SqlConnection GetSqlConnectionByConnectionString(string connectionString, bool openImmediately = false)
        {
            

            var cn = new SqlConnection(connectionString);

            if (openImmediately)
                cn.Open();

            return cn;
        }
        protected static void CreateInputParams(SqlCommand command, object[] inputParams = null)
        {
            if (inputParams != null)
            {
                string paramName = String.Empty;
                int i = 0;

                foreach (var param in inputParams)
                {
                    i++;
                    if (i % 2 == 1) //odd items are the parameters
                        paramName = param.ToString();
                    else
                        command.Parameters.AddWithValue(paramName, param);
                }

                if (i % 2 == 1)
                    throw new Exception("Error: Parameter pairing mismatch.");
            }
        }
        public static SqlConnection GetSqlConnectionByName(string connectionName, bool openImmediately = false)
        {
            var connectionString = GetConStr(connectionName);
            return GetSqlConnectionByConnectionString(connectionString, openImmediately); 
        }
        
        public static SqlDataReader ExecuteReader(string cnName, string cmdText, params object[] parameters)
        {
            SqlDataReader dr = null;
            SqlConnection cn = GetSqlConnectionByName(cnName);
            try
            {
                cn.Open();
                using (SqlCommand cmd = GetSqlCommand(cn, cmdText, parameters))
                {
                    dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
            }
            catch (SqlException sqlex)
            {
               
                try { dr.Close(); }
                catch { }
                cn.Close();
                throw;
            }
           
            return dr;
        }
    }
}