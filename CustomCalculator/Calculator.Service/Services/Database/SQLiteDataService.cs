using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Xml.Linq;

namespace Calculator.Service.Services.Database
{
    internal class SQLiteDataService : ISQLiteDataService
    {
        private readonly SQLiteDatabase _currentDb = new SQLiteDatabase();
        public DataTable GetPatients()
        {
            var sql = @"select * from patients";
            var result = _currentDb.ExecuteSelect(sql);
            return result;
        }

        public void AddPatient(string id, string name, DateTime birthday, double weight)
        {
            try
            {
                var sql = @"insert into patients (id,name,birthday,weight)
                    values (:id,:name,:birthday,:weight)";
                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("id", id),
                    new SQLiteParameter("name", name),
                    new SQLiteParameter("birthday", birthday),
                    new SQLiteParameter("weight", weight),
                };

                _currentDb.ExecuteNonQuery(sql, paras);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void UpdatePatientInfo(string patientId, string patientName, DateTime patientBirthday, double patientWeight)
        {
            try
            {
                var sql = @"update patients
                    set name=:name, birthday=:birthday, weight=:weight, 
                        update_time=datetime(CURRENT_TIMESTAMP, 'localtime')
                    where id=:patientId";
                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("name", patientName),
                    new SQLiteParameter("birthday", patientBirthday),
                    new SQLiteParameter("weight", patientWeight),
                    new SQLiteParameter("patientId", patientId),
                };

                _currentDb.ExecuteNonQuery(sql, paras);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
