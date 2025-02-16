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

        public void AddCalculation(string calcId, string patientId, string p1, string p2, string p3, string p4, string formula)
        {
            try
            {
                var sql = @"insert into patients_calculates (id, patient_id, Property1, Property2, Property3, Property4, formula)
                        values (:id, :patientId, :p1, :p2, :p3, :p4, :formula)";

                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("id", calcId),
                    new SQLiteParameter("patientId", patientId),
                    new SQLiteParameter("p1", p1),
                    new SQLiteParameter("p2", p2),
                    new SQLiteParameter("p3", p3),
                    new SQLiteParameter("p4", p4),
                    new SQLiteParameter("formula", formula),
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
