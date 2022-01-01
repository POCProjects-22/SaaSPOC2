using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SaaSPOCServices.Helper
{
    public static class Utilities
    {

        public static string GetNewDBName()
        {
            var dbName = Guid.NewGuid().ToString();
            return dbName;
        }
        public static string GetDBNameFromUserEmail(string email)
        {
            var dbName = email.Split('@')[0];
            return dbName;
        }
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static DataSet ExecuteDataSet(string connectionString,string sqlCommand, List<SqlParameter> sqlParameterlist = null, string DSName = "Table1")
        {
            //var connectionString = userRepository.GetConnectionString();

            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            var _dbCommand   = new SqlCommand(sqlCommand, sqlConnection);


            _dbCommand.Parameters.Clear();
            if (sqlParameterlist != null)
                _dbCommand.Parameters.AddRange(sqlParameterlist.ToArray());

            _dbCommand.CommandText = sqlCommand;

            DataSet dataSet = new DataSet();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            _dbCommand.CommandText = sqlCommand;
            dataAdapter.SelectCommand = _dbCommand;
            dataAdapter.Fill(dataSet, DSName);
            return dataSet;
        }



        public static string BasePath { get;   set; }
    }

    
}
