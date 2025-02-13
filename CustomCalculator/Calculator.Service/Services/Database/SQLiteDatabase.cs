using System.Collections.Generic;
using System.Data;
using System.IO;
using System;
using System.Data.SQLite;

namespace Calculator.Service.Services.Database
{
    internal class SQLiteDatabase
    {
        private const string DataBaseFileName = @"CustomCalculator\Data\data.db";
        private readonly string _connectionString =
            $@"Data Source={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), DataBaseFileName)};Pooling=true;";

        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        public DataTable ExecuteSelect(string sql)
        {
            DataTable dt = null;
            SQLiteConnection cnn = GetConnection();
            cnn.Open();
            dt = ExecuteSelect(cnn, sql, new List<SQLiteParameter>());
            cnn.Close();
            return dt;
        }

        public DataTable ExecuteSelect(string sql, List<SQLiteParameter> paras)
        {
            DataTable dt = null;
            SQLiteConnection cnn = GetConnection();
            cnn.Open();
            dt = ExecuteSelect(cnn, sql, paras);
            cnn.Close();
            return dt;
        }

        public DataTable ExecuteSelect(SQLiteConnection cnn, string sql, List<SQLiteParameter> paras)
        {
            SQLiteCommand command = new SQLiteCommand(cnn);
            command.CommandText = sql;
            if (null != paras)
                command.Parameters.AddRange(paras.ToArray());
            SQLiteDataAdapter da = new SQLiteDataAdapter(command);
            try
            {
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds.Tables.Count > 0 ? ds.Tables[0] : null;
            }
            catch { }
            finally
            {
                da.Dispose();
            }
            return null;
        }

        public void ExecuteNonQuery(string sql, List<SQLiteParameter> paras)
        {
            SQLiteConnection cnn = GetConnection();
            cnn.Open();
            ExecuteNonQuery(cnn, sql, paras);
            cnn.Close();
        }

        public void ExecuteNonQuery(SQLiteConnection cnn, string sql, List<SQLiteParameter> paras)
        {
            SQLiteCommand command = new SQLiteCommand(sql, cnn);
            if (null != paras)
                command.Parameters.AddRange(paras.ToArray());
            command.ExecuteNonQuery();
            command.Dispose();
        }
    }
}
