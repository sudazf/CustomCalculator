using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Calculator.Model.Models;
using SQLiteException = Calculator.Model.Models.Exceptions.SQLiteException;

namespace Calculator.Service.Services.Database
{
    internal class SQLiteDataService : ISQLiteDataService
    {
        private readonly SQLiteDatabase _currentDb = new SQLiteDatabase();

        public DataTable GetPatientsCount()
        {
            try
            {
                var sql = $@"select count(*) count from patients;";
                var result = _currentDb.ExecuteSelect(sql);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public DataTable GetPatient(string id)
        {
            try
            {
                var sql = $@"select * from patients where id=:id;";
                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("id", id),
                };
                var result = _currentDb.ExecuteSelect(sql, paras);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public DataTable GetPatients(int from, int to)
        {
            try
            {
                var sql = $@"select * from patients
                    limit {from},{to};";
                var result = _currentDb.ExecuteSelect(sql);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new SQLiteException(e);
            }
        }

        public DataTable GetPatientsCount(string patientName)
        {
            try
            {
                var searchKeyword = $"%{patientName}%";
                var sql = @"SELECT count(*) count from patients t 
                    WHERE t.name LIKE :searchKeyword";

                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter(":searchKeyword", searchKeyword),
                };

                var result = _currentDb.ExecuteSelect(sql, paras);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public DataTable GetPatients(string patientName, int from, int to)
        {
            try
            {
                var searchKeyword = $"%{patientName}%";
                var sql = $@"SELECT * from patients t 
                    WHERE t.name LIKE :searchKeyword limit {from},{to}";

                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter(":searchKeyword", searchKeyword),
                };

                var result = _currentDb.ExecuteSelect(sql, paras);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public DataTable GetPatientNames(string patientName)
        {
            try
            {
                var searchKeyword = $"%{patientName}%";
                var sql = $@"SELECT * from patients t 
                    WHERE t.name LIKE :searchKeyword;";

                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter(":searchKeyword", searchKeyword),
                };

                var result = _currentDb.ExecuteSelect(sql, paras);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void AddPatient(string id, string bedNumber, string name, DateTime birthday, 
            double weight, double height, string sex, string sd)
        {
            try
            {
                var sql = @"insert into patients (id, bed_number,name,birthday,weight, height, sex, SD)
                    values (:id,:bed_number,:name,:birthday,:weight,:height,:sex, :sd)";
                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("id", id),
                    new SQLiteParameter("bed_number", bedNumber),
                    new SQLiteParameter("name", name),
                    new SQLiteParameter("birthday", birthday),
                    new SQLiteParameter("weight", weight),
                    new SQLiteParameter("height", height),
                    new SQLiteParameter("sex", sex),
                    new SQLiteParameter("sd", sd),
                };

                _currentDb.ExecuteNonQuery(sql, paras);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public void UpdatePatientInfo(string patientId, string bedNumber, string patientName, 
            DateTime patientBirthday, double patientWeight, string diagnosis, double height, string sex, string sd)
        {
            try
            {
                var sql = @"update patients
                    set bed_number=:bedNumber, name=:name, birthday=:birthday, 
                        weight=:weight, diagnosis=:diagnosis, height=:height, sex=:sex, SD=:sd,
                        update_time=datetime(CURRENT_TIMESTAMP, 'localtime')
                    where id=:patientId";
                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("bedNumber", bedNumber),
                    new SQLiteParameter("name", patientName),
                    new SQLiteParameter("birthday", patientBirthday),
                    new SQLiteParameter("weight", patientWeight),
                    new SQLiteParameter("height", height),
                    new SQLiteParameter("sex", sex),
                    new SQLiteParameter("sd", sd),
                    new SQLiteParameter("patientId", patientId),
                    new SQLiteParameter("diagnosis", diagnosis),
                };

                _currentDb.ExecuteNonQuery(sql, paras);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public void DeletePatient(string id)
        {
            try
            {
                var sql = @"delete from patients
                    where id=:patientId";
                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("patientId", id),
                };

                _currentDb.ExecuteNonQuery(sql, paras);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public DataTable GetPatientDays(string id)
        {
            try
            {
                var sql = @"select * from patients_variables 
                            where patient_id=:patient_id
                            Order by create_day";
                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("patient_id", id),
                };
                var result = _currentDb.ExecuteSelect(sql, paras);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void DeletePatientDailyVariables(string patientId, string day)
        {
            try
            {
                var sql = @"delete from patients_variables
                where patient_id=:patientId AND create_day=:day";
                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("patientId", patientId),
                    new SQLiteParameter("day", day),
                };

                _currentDb.ExecuteNonQuery(sql, paras);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void InsertPatientDailyVariable(string patientId, string day, Variable variable)
        {
            try
            {
                var sql = @"insert into patients_variables 
                    (id,isChecked,isSetResult,patient_id,variable_name,variable_value,
                    variable_min,variable_max,variable_unit,variable_expression, FollowVariables,create_day)
                    values (:id, :isChecked, :isSetResult, :patient_id, :variable_name, :variable_value,
                    :variable_min, :variable_max, :variable_unit, :variable_expression, :FollowVariables, :day)";

                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("id", variable.Id),
                    new SQLiteParameter("isChecked", variable.IsChecked ? 1 : 0),
                    new SQLiteParameter("isSetResult", variable.ShowAsResult ? 1 : 0),
                    new SQLiteParameter("patient_id", patientId),
                    new SQLiteParameter("variable_name", variable.Name),
                    new SQLiteParameter("variable_value", variable.Value),
                    new SQLiteParameter("variable_min", variable.Min),
                    new SQLiteParameter("variable_max", variable.Max),
                    new SQLiteParameter("variable_unit", variable.Unit),
                    new SQLiteParameter("variable_expression", variable.Formula.MetaExpression),
                    new SQLiteParameter("FollowVariables", string.Join(",", variable.FollowVariables)),
                    new SQLiteParameter("day", day)
                };

                _currentDb.ExecuteNonQuery(sql, paras);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void InsertVariableTemplate(VariableTemplate template)
        {
            try
            {
                var sql = @"insert into template_variables 
                    (id,template_name,isChecked,isSetResult,variable_name,variable_value,
                    variable_min,variable_max,variable_unit,variable_expression, FollowVariables)
                    values (:id, :template_name, :isChecked,:isSetResult, :variable_name, :variable_value,
                    :variable_min, :variable_max, :variable_unit, :variable_expression, :FollowVariables)";

                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("id", template.Id),
                    new SQLiteParameter("template_name", template.Name),
                    new SQLiteParameter("isChecked", template.Variable.IsChecked),
                    new SQLiteParameter("isSetResult", template.Variable.ShowAsResult),
                    new SQLiteParameter("variable_name", template.Variable.Name),
                    new SQLiteParameter("variable_value",  template.Variable.Value),
                    new SQLiteParameter("variable_min",  template.Variable.Min),
                    new SQLiteParameter("variable_max",  template.Variable.Max),
                    new SQLiteParameter("variable_unit",  template.Variable.Unit),
                    new SQLiteParameter("variable_expression",  template.Variable.Formula.MetaExpression),
                    new SQLiteParameter("FollowVariables",  string.Join(",",template.Variable.FollowVariables)),
                };

                _currentDb.ExecuteNonQuery(sql, paras);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public DataTable GetTemplateNames()
        {
            try
            {
                var sql = @"select distinct(template_name) template_name 
                        from template_variables t;";
                var result = _currentDb.ExecuteSelect(sql);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public DataTable GetVariableTemplates(string templateName)
        {
            try
            {
                var sql = @"select * from template_variables 
                            where template_name=:templateName";
                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("templateName", templateName),
                };
                var result = _currentDb.ExecuteSelect(sql, paras);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public void DeletePatientDays(string id)
        {
            try
            {
                var sql = @"delete from patients_variables
                    where patient_id=:patientId";
                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("patientId", id),
                };

                _currentDb.ExecuteNonQuery(sql, paras);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void RemoveTemplate(string name)
        {
            try
            {
                var sql = @"delete from template_variables
                        where template_name=:name";
                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("name", name)
                };

                _currentDb.ExecuteNonQuery(sql, paras);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void ChangeTemplateName(string oldName, string newName)
        {
            try
            {
                var sql = @"update template_variables
                        set template_name=:newName
                        where template_name=:oldName";
                var paras = new List<SQLiteParameter>
                {
                    new SQLiteParameter("newName", newName),
                    new SQLiteParameter("oldName", oldName),
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
