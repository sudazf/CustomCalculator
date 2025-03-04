﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Calculator.Model.Models;
using Calculator.Service.Services.Database;
using Jg.wpf.core.Service;

namespace Calculator.ViewModel.Helpers
{
    public class PatientDataHelper
    {
        private readonly ISQLiteDataService _dbService;

        public PatientDataHelper()
        {
            _dbService = ServiceManager.GetService<ISQLiteDataService>();

        }

        public IEnumerable<Patient> GetAllPatients(int page, int pageSize, int recordCount)
        {
            try
            {
                int offset = (page - 1) * pageSize;
                var dataTable = _dbService.GetPatients(offset, pageSize);
                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    return null;
                }

                var patients = new List<Patient>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var id = row["id"].ToString();
                    var bedNumber = row["bed_number"].ToString();
                    var name = row["name"].ToString();
                    var birthday = row["birthday"].ToString();
                    var weight = row["weight"].ToString();
                    var height = row["height"].ToString();
                    var sex = row["sex"].ToString();
                    var sd = row["SD"].ToString();
                    var diagnosis = row["diagnosis"].ToString();

                    patients.Add(new Patient(id, bedNumber, name, 
                        DateTime.Parse(birthday), double.Parse(weight),
                        height == "" ? 0 : double.Parse(height),
                        sex, sd,
                        diagnosis));
                }

                return patients;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public IEnumerable<Patient> GetSearchPatients(string patientName, int page, int pageSize, int recordCount)
        {
            try
            {
                int offset = (page - 1) * pageSize;
                var dataTable = _dbService.GetPatients(patientName, offset, pageSize);
                if (dataTable == null)
                {
                    return null;
                }

                var patients = new List<Patient>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var id = row["id"].ToString();
                    var bedNumber = row["bed_number"].ToString();
                    var name = row["name"].ToString();
                    var birthday = row["birthday"].ToString();
                    var weight = row["weight"].ToString();
                    var height = row["height"].ToString();
                    var sex = row["sex"].ToString();
                    var sd = row["SD"].ToString();
                    var diagnosis = row["diagnosis"].ToString();

                    patients.Add(new Patient(id, bedNumber, name, DateTime.Parse(birthday), 
                        double.Parse(weight), double.Parse(height), sex, sd, diagnosis));
                }

                return patients;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public IEnumerable<DailyInfo> GetPatientDays(string patientId)
        {
            try
            {
                var dataTable = _dbService.GetPatientDays(patientId);
                if (dataTable == null)
                {
                    return null;
                }

                var dailyInfos = new List<DailyInfo>();
                var days = new List<string>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var day = row["create_day"].ToString();
                    if (!days.Contains(day))
                    {
                        days.Add(day);
                    }
                }

                foreach (var day in days)
                {
                    var daily = new DailyInfo() { Day = DateTime.Parse(day).ToString("yyyy-MM-dd") };
                    var variables = new ObservableCollection<Variable>();
                    var dayRows = dataTable.AsEnumerable().Where(row=> row["create_day"].ToString() == day);
                    foreach (var row in dayRows)
                    {
                        var id = row["id"].ToString();
                        var isChecked = row["isChecked"].ToString();
                        var isSetResult = row["isSetResult"].ToString();
                        var name = row["variable_name"].ToString();
                        var value = row["variable_value"].ToString();
                        var min = row["variable_min"].ToString();
                        var max = row["variable_max"].ToString();
                        var unit = row["variable_unit"].ToString();
                        var metaExpression = row["variable_expression"].ToString();
                        var follows = row["FollowVariables"].ToString();

                        variables.Add(new Variable(id, int.Parse(isChecked) == 1, int.Parse(isSetResult) == 1, 
                            name, value, unit, min, max, new Formula(metaExpression), !string.IsNullOrEmpty(follows) ? new List<string>(follows.Split(',')) : new List<string>()));
                    }

                    foreach (var variable in variables)
                    {
                        var expressionItems = variable.Formula.ExpressionItems;
                        var names = variable.Formula.MetaExpression.Split(',');
                        foreach (var name in names)
                        {
                            if (name == "无公式")
                            {
                                break;
                            }
                            var existVariable = variables.FirstOrDefault(v => v.Name == name);
                            expressionItems.Add(new ExpressionItem(name, existVariable == null ? name : existVariable.Value));
                        }
                    }

                    daily.Variables = variables;
                    dailyInfos.Add(daily);
                }

                return dailyInfos;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void SavePatientDailyVariables(string patientId, DailyInfo daily)
        {
            try
            {
                //先删除
                _dbService.DeletePatientDailyVariables(patientId, daily.Day);

                //后添加
                foreach (var variable in daily.Variables)
                {
                    _dbService.InsertPatientDailyVariable(patientId, daily.Day, variable);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
