﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using Calculator.Model.Events;
using Calculator.Model.Models;
using Calculator.Model.Models.Exceptions;
using Calculator.Service.Services.App;
using Calculator.Service.Services.Database;
using Calculator.Service.Services.Parser;
using Calculator.ViewModel.Helpers;
using Calculator.ViewModel.ViewModels.Patients;
using Jg.wpf.core.Command;
using Jg.wpf.core.Notify;
using Jg.wpf.core.Profilers;
using Jg.wpf.core.Service;
using Jg.wpf.core.Service.ThreadService;

namespace Calculator.ViewModel.ViewModels.Applications
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ISQLiteDataService _dbService;
        private readonly IParser _parser;
        private readonly IDispatcher _dispatcher;
        private readonly IWindowService _windowService;
        private readonly PatientDataHelper _dataHelper;
        private readonly TaskManager _taskManager;

        private string _searchPatientName;
        private string _selectedSuggestPatientName = "";
        private Patient _selectPatient;
        private object _dialogViewModel;
        private bool _isDialogOpen;

        public object DialogViewModel
        {
            get => _dialogViewModel;
            set
            {
                _dialogViewModel = value;
                IsDialogOpen = true;
                RaisePropertyChanged(nameof(DialogViewModel));
            }
        }
        public bool IsDialogOpen
        {
            get => _isDialogOpen;
            set
            {
                if (value == _isDialogOpen) return;
                _isDialogOpen = value;
                RaisePropertyChanged(nameof(IsDialogOpen));
            }
        }
        public bool PatientChanged { get; set; }
        public ObservableCollection<Patient> Patients { get; }
        public Patient SelectPatient
        {
            get => _selectPatient;
            set
            {
                if (Equals(value, _selectPatient)) return;
                _selectPatient = value;
                PatientChanged = true;
                RaisePropertyChanged(nameof(SelectPatient));
                RemovePatientCommand.RaiseCanExecuteChanged();
            }
        }
        public string SearchPatientName
        {
            get => _searchPatientName;
            set
            {
                if (value == _searchPatientName) return;
                _searchPatientName = value;
                RaisePropertyChanged(nameof(SearchPatientName));
            }
        }
        public string SelectedSuggestPatientName
        {
            get => _selectedSuggestPatientName;
            set
            {
                if (value == _selectedSuggestPatientName) return;
                _selectedSuggestPatientName = value;
                SearchSelectedPatientName(_selectedSuggestPatientName);
                RaisePropertyChanged(nameof(SelectedSuggestPatientName));
            }
        }

        public JCommand AddPatientCommand { get; }
        public JCommand EditPatientCommand { get; }
        public JCommand RemovePatientCommand { get; }
        public JCommand EditVariableExpressionCommand { get;  }
        public JCommand SaveVariablesCommand { get; }
        public JCommand CancelSaveVariablesCommand { get; }
        public JCommand CalculateCommand { get; }
        public JCommand CalcSingleCommand { get; }
        public JCommand AddVariableCommand { get; }
        public JCommand RemoveVariableCommand { get; }
        public JCommand ExportVariablesToTemplateCommand { get; }
        public JCommand ImportVariablesFromTemplateCommand { get; }
        public JCommand AddPreviewDayCommand { get; }
        public JCommand AddCurrentDayCommand { get; }
        public JCommand RemoveDayCommand { get; }
        public JCommand VariableFollowsSettingCommand { get; }
        
        public AddPatientViewModel AddPatientViewModel { get; }
        public EditPatientViewModel EditPatientViewModel { get; }
        public PatientPagingViewModel PatientPagingViewModel { get; }
        public VariableExpressionViewModel VariableExpressionViewModel { get; }
        public FollowVariablesSettingViewModel FollowVariablesSettingViewModel { get; }
        public VariableTemplatesMaintainViewModel VariableTemplatesMaintainViewModel { get; }
        public MessageViewModel MessageViewModel { get; }
        
        public MainWindowViewModel()
        {
            _parser = ServiceManager.GetService<IParser>();
            _dbService = ServiceManager.GetService<ISQLiteDataService>();
            _dispatcher = ServiceManager.GetService<IDispatcher>();
            _windowService = ServiceManager.GetService<IWindowService>();
            
            _dataHelper = new PatientDataHelper();
            _taskManager = new TaskManager("MainQueryTaskManager");

            AddPatientCommand = new JCommand("AddPatientCommand", OnAddPatient);
            EditPatientCommand = new JCommand("EditPatientCommand", OnEditPatient);
            RemovePatientCommand = new JCommand("RemovePatientCommand", OnRemovePatient, CanRemovePatient);
            EditVariableExpressionCommand = new JCommand("EditVariableExpressionCommand", OnEditVariableExpression);
            SaveVariablesCommand = new JCommand("SaveVariablesCommand", OnSaveVariables);
            CancelSaveVariablesCommand = new JCommand("CancelSaveVariablesCommand", OnCancelSaveVariables);
            AddVariableCommand = new JCommand("AddVariableCommand", OnAddVariable);
            RemoveVariableCommand = new JCommand("RemoveVariableCommand", OnRemoveVariable, CanRemoveVariable);
            ExportVariablesToTemplateCommand = new JCommand("ExportVariablesToTemplateCommand", OnExportVariablesToTemplate);
            ImportVariablesFromTemplateCommand = new JCommand("ImportVariablesFromTemplateCommand", OnImportVariablesFromTemplate);
            AddPreviewDayCommand = new JCommand("AddPreviewDayCommand", OnAddPreviewDay);
            AddCurrentDayCommand = new JCommand("AddCurrentDayCommand", OnAddCurrentDay);
            RemoveDayCommand = new JCommand("RemoveDayCommand", OnRemoveDay);
            VariableFollowsSettingCommand = new JCommand("VariableFollowsSettingCommand", OnVariableFollowsSetting);
            
            CalculateCommand = new JCommand("CalculateCommand", OnCalc);
            CalcSingleCommand = new JCommand("", OnCalcSingle);
            AddPatientViewModel = new AddPatientViewModel();
            AddPatientViewModel.OnPatientAdded += OnPatientAdded;

            EditPatientViewModel = new EditPatientViewModel();
            EditPatientViewModel.OnPatientEdited += OnPatientEdited;

            VariableExpressionViewModel = new VariableExpressionViewModel();
            VariableExpressionViewModel.OnExpressionItemsEditCompleted += OnExpressionItemsEditCompleted;

            MessageViewModel = new MessageViewModel();
            MessageViewModel.OnMessageClosed += OnMessageClosed;

            PatientPagingViewModel = new PatientPagingViewModel();
            PatientPagingViewModel.OnPageChanged += PatientPagingViewModel_OnPageChanged;

            FollowVariablesSettingViewModel = new FollowVariablesSettingViewModel();
            FollowVariablesSettingViewModel.OnFollowsSettingCompleted += OnFollowsSettingCompleted;

            VariableTemplatesMaintainViewModel = new VariableTemplatesMaintainViewModel();
            VariableTemplatesMaintainViewModel.OnError += VariableTemplatesMaintainViewModelOnOnError;

            Patients = new ObservableCollection<Patient>();

            SdHelper.Init();
        }

        //初始化
        public void InitPatients()
        {
            _taskManager.StartNewTaskProxy("InitPatients", OnInitPatients);
        }
        public void InitSelectPatientDays()
        {
            _taskManager.StartNewTaskProxy("InitSelectPatientDays", OnInitSelectPatientDays);
        }
        private void OnInitPatients(TaskProxy obj)
        {
            try
            {
                _selectedSuggestPatientName = "";
                UpdatePagingRecordCount();
                var patients = _dataHelper.GetAllPatients(PatientPagingViewModel.CurrentPage,
                    PatientPagingViewModel.PageSize, PatientPagingViewModel.RecordCount);
                if (patients == null)
                {
                    //判断数据文件是否损坏
                    JudgeAndBackupDbFile("病人数据为空");
                    return;
                }

                //备份数据库文件
                BackupDbFile();

                foreach (var patient in Patients)
                {
                    patient.OnSelectedDailyVariableChanged -= OnSelectedDailyVariableChanged;
                    patient.OnSelectedDailyAllVariableChanged -= OnSelectedDailyAllVariableChanged;
                    patient.OnCommonInfoChanged -= OnCommonInfoChanged;
                    
                    if (patient.Days == null)
                    {
                        continue;
                    }

                    foreach (var day in patient.Days)
                    {
                        foreach (var variable in day.Variables)
                        {
                            variable.OnPropertyChanged -= PatientVariable_OnPropertyChanged;
                        }
                    }
                }

                _dispatcher.Invoke(() => { Patients.Clear(); });

                foreach (var patient in patients)
                {
                    var id = patient.Id;
                    var bedNumber = patient.BedNumber;
                    var name = patient.Name;
                    var birthday = patient.Birthday;
                    var weight = patient.Weight;
                    var height = patient.Height;
                    var sex = patient.Sex;
                    var sd = patient.SD;
                    var diagnosis = patient.Diagnosis;

                    _dispatcher.Invoke(() =>
                    {
                        var newPatient = new Patient(id, bedNumber, name, birthday, weight, height, sex, sd,
                            diagnosis);
                            newPatient.CalcBMI();
                        newPatient.OnSelectedDailyVariableChanged += OnSelectedDailyVariableChanged;
                        newPatient.OnSelectedDailyAllVariableChanged += OnSelectedDailyAllVariableChanged;
                        newPatient.OnCommonInfoChanged += OnCommonInfoChanged;
                        Patients.Add(newPatient);
                    });
                }
            }
            catch (SQLiteException sqliteError)
            {
                JudgeAndBackupDbFile(sqliteError.Exception.Message);
            }
            catch (Exception e)
            {
                MessageViewModel.SetMessage($"{e.Message} \r\n {e.StackTrace}");
                DialogViewModel = MessageViewModel;
            }
        }

        private void JudgeAndBackupDbFile(string message = "")
        {
            var backup = @"CustomCalculator\Data\data_backup.db";
            var backupFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                backup);
            //无病人数据，但有备份文件，可能就是数据文件损坏
            if (File.Exists(backupFile))
            {
                var source = @"CustomCalculator\Data\data.db";
                var sourceFile =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), source);

                _dispatcher.Invoke(() =>
                {
                    //将备份文件，按照时间再次备份起来（防止数据源损坏，又新增了病人导致备份文件也只剩1个病人）
                    var newBackup = $@"CustomCalculator\Data\data_backup_{DateTime.Now:yyyy-MM-dd HH_mm_ss}.db";
                    var newBackupFile =
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            newBackup);
                    File.Copy(backupFile, newBackupFile, overwrite: true);

                    ShowMessage(
                        $"检测到源数据可能发生丢失，异常信息：{message} \r\n 请不要新增任何病人，并立刻手动拷贝文件：\r\n \"{backupFile}\"  \r\n 到 \r\n \"{sourceFile}\"");
                });
            }
        }

        private void OnInitSelectPatientDays(TaskProxy obj)
        {
            try
            {
                if (SelectPatient != null)
                {
                    if (SelectPatient.Days != null)
                    {
                        foreach (var day in SelectPatient.Days)
                        {
                            foreach (var variable in day.Variables)
                            {
                                variable.OnPropertyChanged -= PatientVariable_OnPropertyChanged;
                            }

                            //day.Variables = null;
                        }
                        SelectPatient.Days = null;
                    }

                    var patientInfo = _dbService.GetPatient(SelectPatient.Id);
                    var days = _dataHelper.GetPatientDays(SelectPatient.Id);

                    _dispatcher.Invoke(() =>
                    {
                        var diagnosis = patientInfo.Rows[0]["diagnosis"].ToString();
                        SelectPatient.ResetDiagnosis(diagnosis);
                        OnInitDays(days);
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        private void OnInitDays(IEnumerable<DailyInfo> queryDays)
        {
            var days = new List<DailyInfo>(queryDays);
            if (days.Any())
            {
                SelectPatient.Days = new ObservableCollection<DailyInfo>(days);
            }
            else
            {
                SelectPatient.GenerateDefaultDays();
            }

            if (SelectPatient.Days != null)
            {
                foreach (var day in SelectPatient.Days)
                {
                    foreach (var variable in day.Variables)
                    {
                        variable.OnPropertyChanged += PatientVariable_OnPropertyChanged;
                    }
                }

                //显示最新日期
                if (SelectPatient.SelectedDay != null)
                {
                    var selectedDayString = SelectPatient.SelectedDay.Day;
                    var selectedDay = SelectPatient.Days.FirstOrDefault(d => d.Day == selectedDayString);
                    if (selectedDay != null)
                    {
                        SelectPatient.SelectedDay = selectedDay;
                    }
                    else
                    {
                        SelectPatient.SelectedDay = SelectPatient.Days.Last();
                    }
                }
                else
                {
                    SelectPatient.SelectedDay = SelectPatient.Days.Last();
                }
                SelectPatient.UpdateSelect();
            }

            PatientChanged = false;
        }

        //增、改、删人
        private void OnAddPatient(object obj)
        {
            AddPatientViewModel.SetPatient();
            DialogViewModel = AddPatientViewModel;
        }
        private void OnEditPatient(object obj)
        {
            if (SelectPatient == null)
            {
                MessageViewModel.SetMessage("请先选择一个病人信息");
                DialogViewModel = MessageViewModel;

                return;
            }

            EditPatientViewModel.SetPatient((Patient)SelectPatient.Clone());
            DialogViewModel = EditPatientViewModel;
        }
        private void OnRemovePatient(object obj)
        {
            try
            {
                if (SelectPatient != null)
                {
                    _windowService.ShowDialog("系统提示", new ConfirmViewModel($"确定要删除病人记录：{SelectPatient.Name} ?"));
                    if (_windowService.Result is bool confirm)
                    {
                        if (confirm)
                        {
                            _dbService.DeletePatient(SelectPatient.Id);
                            _dbService.DeletePatientDays(SelectPatient.Id);

                            UpdatePagingRecordCount(SearchPatientName);

                            SelectPatient.OnSelectedDailyVariableChanged -= OnSelectedDailyVariableChanged;
                            SelectPatient.OnSelectedDailyAllVariableChanged -= OnSelectedDailyAllVariableChanged;
                            SelectPatient.OnCommonInfoChanged -= OnCommonInfoChanged;

                            if (SelectPatient.Days != null)
                            {
                                foreach (var day in SelectPatient.Days)
                                {
                                    foreach (var variable in day.Variables)
                                    {
                                        variable.OnPropertyChanged -= PatientVariable_OnPropertyChanged;
                                    }
                                }
                            }

                            Patients.Remove(SelectPatient);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ShowMessage(e.Message);
            }
        }
        private bool CanRemovePatient(object arg)
        {
            return SelectPatient != null;
        }

        private void OnPatientAdded(object sender, PatientAddOrEditEventArgs args)
        {
            if (!args.IsCancel)
            {
                var sd = SdHelper.GetSd(args.Patient.Sex == "男", args.Patient.Birthday,
                    args.Patient.Weight, args.Patient.Height);

                _dbService.AddPatient(args.Patient.Id, args.Patient.BedNumber,
                    args.Patient.Name, args.Patient.Birthday, args.Patient.Weight,
                    args.Patient.Height, args.Patient.Sex, sd);

                //清除筛选条件
                _selectedSuggestPatientName = "";
                //获取总数
                UpdatePagingRecordCount();
                //计算总页数PageCount
                var pageCount = (PatientPagingViewModel.RecordCount - 1) / PatientPagingViewModel.PageSize + 1;
                //跳转到最后一页
                PatientPagingViewModel.SetCurrentPage(pageCount);
                //加载页数据
                InitPatients();
            }

            IsDialogOpen = false;
        }
        private void OnPatientEdited(object sender, PatientAddOrEditEventArgs args)
        {
            if (!args.IsCancel)
            {
                var patient = Patients.FirstOrDefault(p => p.Id == args.Patient.Id);
                if (patient != null)
                {
                    patient.BedNumber = args.Patient.BedNumber;
                    patient.Name = args.Patient.Name;
                    patient.Birthday = args.Patient.Birthday;
                    patient.Weight = args.Patient.Weight;
                    patient.Height = args.Patient.Height;
                    patient.Sex = args.Patient.Sex;
                    patient.Diagnosis = args.Patient.Diagnosis;

                    //计算sd
                    var sd = SdHelper.GetSd(args.Patient.Sex == "男", args.Patient.Birthday,
                        args.Patient.Weight, args.Patient.Height);
                    patient.SD = sd;
                    //计算BMI
                    patient.CalcBMI();

                    _dbService.UpdatePatientInfo(patient.Id, patient.BedNumber, patient.Name,
                        patient.Birthday, patient.Weight, patient.Diagnosis, patient.Height, 
                        patient.Sex, patient.SD);
                }
            }

            IsDialogOpen = false;
        }

        //联动
        private void OnVariableFollowsSetting(object obj)
        {
            try
            {
                if (SelectPatient.SelectedDay.SelectedVariable == null)
                {
                    return;
                }

                var variable = (Variable)SelectPatient.SelectedDay.SelectedVariable.Clone();
                var day = (DailyInfo)SelectPatient.SelectedDay.Clone();
                FollowVariablesSettingViewModel.SetVariable(variable, day);
                _windowService.ShowDialog("变量联动设置", FollowVariablesSettingViewModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ShowMessage(e.Message);
            }
        }
        private void OnFollowsSettingCompleted(object sender, VariableFollowsSettingEventArgs e)
        {
            if (!e.IsCancel)
            {
                var id = e.Variable.Id;
                var variable = SelectPatient.SelectedDay.Variables.FirstOrDefault(v => v.Id == id);
                if (variable != null)
                {
                    variable.FollowVariables = new List<string>();
                    foreach (var followVariable in e.Variable.FollowVariables)
                    {
                        variable.FollowVariables.Add(string.Copy(followVariable));
                    }
                }
            }
        }

        //日数据
        private void AddDayContent(string day)
        {
            var property1 = new Variable(Guid.NewGuid().ToString(), false, false,"P1", "0", "", "", "", new Formula("无公式"));
            var property2 = new Variable(Guid.NewGuid().ToString(), false, false,"P2", "0", "", "", "", new Formula("无公式"));
            var property3 = new Variable(Guid.NewGuid().ToString(), false, false,"P3", "0", "", "", "", new Formula("无公式"));
            var property4 = new Variable(Guid.NewGuid().ToString(), false, false, "P4", "0", "", "", "", new Formula("无公式"));

            var dailyInfo = new DailyInfo()
            {
                Day = day,
                Variables =
                {
                    property1, property2, property3, property4,
                },
            };

            foreach (var variable in dailyInfo.Variables)
            {
                variable.OnPropertyChanged += PatientVariable_OnPropertyChanged;
                _dbService.InsertPatientDailyVariable(SelectPatient.Id, day, variable);
            }

            SelectPatient.Days.Add(dailyInfo);
            SelectPatient.SelectedDay = SelectPatient.Days.Last();
            SelectPatient.UpdateSelect();

            SelectPatient.SelectedDay.IsDirty = true;
            SelectPatient.IsDirty = true;
        }
        private void OnAddCurrentDay(object obj)
        {
            try
            {
                var newDay = DateTime.Now.ToString("yyyy-MM-dd");
                var exist = SelectPatient.Days.FirstOrDefault(d => d.Day == newDay);
                if (exist != null)
                {
                    ShowMessage($"已存在 {newDay} 数据");
                    return;
                }
                
                AddDayContent(newDay);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        private void OnAddPreviewDay(object obj)
        {
            try
            {
                var lastDay = SelectPatient.Days.Last();
                var newDayTime = DateTime.Parse(lastDay.Day).AddDays(1);
                var newDay = newDayTime.ToString("yyyy-MM-dd");
                var exist = SelectPatient.Days.FirstOrDefault(d => d.Day == newDay);
                while (exist != null)
                {
                    newDayTime = newDayTime.AddDays(1);
                    newDay = newDayTime.ToString("yyyy-MM-dd");
                    exist = SelectPatient.Days.FirstOrDefault(d => d.Day == newDay);
                }

                AddDayContent(newDay);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        private void OnRemoveDay(object obj)
        {
            if (SelectPatient.SelectedDay != null)
            {
                if (SelectPatient.Days.Count == 1 && SelectPatient.SelectedDay == SelectPatient.Days[0])
                {
                    ShowMessage("不能删除，至少需要保留1天的数据");
                    return;
                }

                _windowService.ShowDialog("系统提示", new ConfirmViewModel($"确定要删除日期记录：{SelectPatient.SelectedDay.Day} ?"));
                if (_windowService.Result is bool confirm)
                {
                    if (confirm)
                    {
                        _dbService.DeletePatientDailyVariables(SelectPatient.Id, SelectPatient.SelectedDay.Day);

                        foreach (var variable in SelectPatient.SelectedDay.Variables)
                        {
                            variable.OnPropertyChanged -= PatientVariable_OnPropertyChanged;
                        }

                        SelectPatient.Days.Remove(SelectPatient.SelectedDay);
                    }
                }
            }
        }

        //模板
        private void OnExportVariablesToTemplate(object obj)
        {
            try
            {
                _windowService.ShowDialog("模板导出", new GetTemplateNameViewModel());
                if (_windowService.Result is string templateName)
                {
                    if (!string.IsNullOrEmpty(templateName))
                    {
                        var dataTable = _dbService.GetTemplateNames();
                        var names = new List<string>();
                        foreach (DataRow row in dataTable.Rows)
                        {
                            names.Add(row["template_name"].ToString());
                        }

                        if (names.Contains(templateName))
                        {
                            ShowMessage($"已存在名为 {templateName} 的模板，请更换模板名称并重新保存");
                            return;
                        }

                        foreach (var variable in SelectPatient.SelectedDay.Variables)
                        {
                            _dbService.InsertVariableTemplate(new VariableTemplate(templateName, variable));
                        }

                        ShowMessage("保存成功");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ShowMessage(e.Message);
            }
        }
        private void OnImportVariablesFromTemplate(object obj)
        {
            try
            {
                var dataTable = _dbService.GetTemplateNames();
                var names = new List<string>();
                foreach (DataRow row in dataTable.Rows)
                {
                    names.Add(row["template_name"].ToString());
                }

                if (names.Count == 0)
                {
                    ShowMessage("没有可用模板，请先导出一个模板。");
                    return;
                }

                _windowService.ShowDialog("模板导入", new SelectTemplateNameViewModel(names));
                if (_windowService.Result is string selectedName)
                {
                    if (!string.IsNullOrEmpty(selectedName))
                    {
                        if (!names.Contains(selectedName))
                        {
                            ShowMessage($"不存在模板名：\"{selectedName}\"，请确认。");
                            return;
                        }
                        ////删除对应日期数据库数据
                        //_dbService.DeletePatientDailyVariables(SelectPatient.Id, SelectPatient.SelectedDay.Day);

                        //清空
                        foreach (var variable in SelectPatient.SelectedDay.Variables)
                        {
                            variable.OnPropertyChanged -= PatientVariable_OnPropertyChanged;
                        }
                        SelectPatient.SelectedDay.Variables.Clear();

                        var templates = _dbService.GetVariableTemplates(selectedName);
                        foreach (DataRow row in templates.Rows)
                        {
                            var id = Guid.NewGuid().ToString();
                            var isChecked = row["isChecked"].ToString();
                            var isSetResult = row["isSetResult"].ToString();
                            var name = row["variable_name"].ToString();
                            var value = row["variable_value"].ToString();
                            var min = row["variable_min"].ToString();
                            var max = row["variable_max"].ToString();
                            var unit = row["variable_unit"].ToString();
                            var metaExpression = row["variable_expression"].ToString();
                            var follows = row["FollowVariables"].ToString();

                            SelectPatient.SelectedDay.Variables.Add(new Variable(id, int.Parse(isChecked) == 1, 
                                int.Parse(isSetResult) == 1, name, value, unit, min, max, 
                                new Formula(metaExpression), 
                                !string.IsNullOrEmpty(follows) ? new List<string>(follows.Split(',')) : new List<string>()));
                        }

                        SelectPatient.SelectedDay.RaiseCheckedAll();

                        foreach (var variable in SelectPatient.SelectedDay.Variables)
                        {
                            variable.OnPropertyChanged += PatientVariable_OnPropertyChanged;

                            var expressionItems = variable.Formula.ExpressionItems;
                            var expressionNames = variable.Formula.MetaExpression.Split(',');
                            foreach (var name in expressionNames)
                            {
                                var existVariable = SelectPatient.SelectedDay.Variables.FirstOrDefault(v => v.Name == name);
                                expressionItems.Add(new ExpressionItem(name, existVariable == null ? name : existVariable.Value));
                            }
                        }

                        SelectPatient.SelectedDay.IsDirty = true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //分页
        private void UpdatePagingRecordCount(string patientName = "")
        {
            try
            {
                var recordCountTable = patientName == "" ? _dbService.GetPatientsCount() : _dbService.GetPatientsCount(patientName);
                if (recordCountTable.Rows.Count == 1)
                {
                    PatientPagingViewModel.RecordCount = int.Parse(recordCountTable.Rows[0]["count"].ToString());
                }
            }
            catch (Exception e)
            {
                ShowMessage("更新页码出错");
            }
        }
        private void PatientPagingViewModel_OnPageChanged(object sender, Jg.wpf.core.Extensions.Types.Pages.PageChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SearchPatientName))
            {
                SearchSelectedPatientName(SearchPatientName);
            }
            else
            {
                InitPatients();
            }
        }

        //查询人
        private void SearchSelectedPatientName(string selectedSuggestPatientName)
        {
            _taskManager.StartNewTaskProxy("SearchSelectedPatientName", OnSearchSelectedPatientName, null, selectedSuggestPatientName);
        }
        private void OnSearchSelectedPatientName(TaskProxy args)
        {
            try
            {
                if (args.Tag is string selectedSuggestPatientName)
                {
                    UpdatePagingRecordCount(selectedSuggestPatientName);
                    var patients = _dataHelper.GetSearchPatients(selectedSuggestPatientName,
                        PatientPagingViewModel.CurrentPage, PatientPagingViewModel.PageSize, PatientPagingViewModel.RecordCount);
                    if (patients == null)
                    {
                        return;
                    }

                    foreach (var patient in Patients)
                    {
                        patient.OnSelectedDailyVariableChanged -= OnSelectedDailyVariableChanged;
                        patient.OnSelectedDailyAllVariableChanged -= OnSelectedDailyAllVariableChanged;
                        patient.OnCommonInfoChanged -= OnCommonInfoChanged;

                        if (patient.Days == null)
                        {
                            continue;
                        }
                        foreach (var day in patient.Days)
                        {
                            foreach (var variable in day.Variables)
                            {
                                variable.OnPropertyChanged -= PatientVariable_OnPropertyChanged;
                            }
                        }
                    }

                    if (_dispatcher.CheckAccess())
                    {
                        Patients.Clear();
                    }
                    else
                    {
                        _dispatcher.Invoke(() =>
                        {
                            Patients.Clear();
                        });
                    }

                    foreach (var patient in patients)
                    {
                        var id = patient.Id;
                        var bedNumber = patient.BedNumber;
                        var name = patient.Name;
                        var birthday = patient.Birthday;
                        var weight = patient.Weight;
                        var height = patient.Height;
                        var sex = patient.Sex;
                        var sd = patient.SD;
                        var diagnosis = patient.Diagnosis;

                        _dispatcher.Invoke(() =>
                        {
                            var newPatient = new Patient(id, bedNumber, name, birthday, weight, height, sex, sd, diagnosis);
                            newPatient.OnSelectedDailyVariableChanged += OnSelectedDailyVariableChanged;
                            newPatient.OnSelectedDailyAllVariableChanged += OnSelectedDailyAllVariableChanged;
                            newPatient.OnCommonInfoChanged += OnCommonInfoChanged;
                            Patients.Add(newPatient);
                        });
                    }
                }
            }
            catch (Exception e)
            {
                MessageViewModel.SetMessage($"{e.Message} \r\n {e.StackTrace}");
                DialogViewModel = MessageViewModel;
            }
        }

        //计算
        private void OnCalcSingle(object obj)
        {
            try
            {
                var expression = string.Join("", SelectPatient.SelectedDay.SelectedVariable.Formula.ExpressionItems.Select(a => a.Value));
                if (string.IsNullOrEmpty(expression))
                {
                    ShowMessage("无公式不支持计算");
                    return;
                }

                var result = _parser.Parse(expression).ToString(CultureInfo.InvariantCulture);
                result = Math.Round(double.Parse(result), 4).ToString(CultureInfo.InvariantCulture);
                SelectPatient.SelectedDay.SelectedVariable.Value = result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ShowMessage($"计算出错：{e.Message}");
            }
        }
        private void OnCalc(object obj)
        {
            try
            {
                foreach (var variable in SelectPatient.SelectedDay.Variables)
                {
                    if (!variable.IsChecked)
                    {
                        continue;
                    }
                    var expression = string.Join("", variable.Formula.ExpressionItems.Select(a => a.Value));
                    if (expression.Contains("无公式") || string.IsNullOrEmpty(expression))
                    {
                        continue;
                    }

                    var result = _parser.Parse(expression).ToString(CultureInfo.InvariantCulture);
                    variable.Value = Math.Round(double.Parse(result), 4).ToString(CultureInfo.InvariantCulture);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ShowMessage($"Calc failed: {e.Message}");
            }
        }

        //增、删变量
        private void OnAddVariable(object obj)
        {
            int suffix = 1;
            var newName = $"data{suffix}";
            while (SelectPatient.SelectedDay.Variables.FirstOrDefault(v => v.Name == newName) != null)
            {
                newName = $"data{++suffix}";
            }

            var newVariable = new Variable(Guid.NewGuid().ToString(), false, false, newName, "0", "", "", "", new Formula("无公式"));
            newVariable.OnPropertyChanged += PatientVariable_OnPropertyChanged;

            SelectPatient.SelectedDay.Variables.Add(newVariable);

            SelectPatient.SelectedDay.IsDirty = true;
            SelectPatient.IsDirty = true;
        }
        private void OnRemoveVariable(object obj)
        {
            var removeIds = new List<string>();
            foreach (var variable in SelectPatient.SelectedDay.Variables)
            {
                if (variable.IsChecked)
                {
                    removeIds.Add(variable.Id);
                }
            }

            foreach (var removeId in removeIds)
            {
                var removedVariable = SelectPatient.SelectedDay.Variables.FirstOrDefault(r => r.Id == removeId);
                if (removedVariable != null)
                {
                    removedVariable.OnPropertyChanged -= PatientVariable_OnPropertyChanged;
                    SelectPatient.SelectedDay.Variables.Remove(removedVariable);
                }
            }

            SelectPatient.SelectedDay.IsDirty = true;
            SelectPatient.IsDirty = true;
        }
        private bool CanRemoveVariable(object arg)
        {
            if (SelectPatient != null && SelectPatient.SelectedDay != null)
            {
                if (SelectPatient.SelectedDay.Variables.Count(v => v.IsChecked) > 0)
                {
                    return true;
                }
            }

            return false;
        }

        //保存变量
        private void OnSaveVariables(object obj)
        {
            _taskManager.StartNewTaskProxy("SaveVariables", () =>
            {
                try
                {
                    _dbService.UpdatePatientInfo(SelectPatient.Id, SelectPatient.BedNumber, SelectPatient.Name,
                        SelectPatient.Birthday, SelectPatient.Weight, SelectPatient.Diagnosis, 
                        SelectPatient.Height, SelectPatient.Sex, SelectPatient.SD);

                    foreach (var day in SelectPatient.Days)
                    {
                        if (day.IsDirty)
                        {
                            _dataHelper.SavePatientDailyVariables(SelectPatient.Id, day);
                            day.IsDirty = false;
                        }
                    }

                    SelectPatient.IsDirty = false;

                    ShowMessage("保存成功");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });
        }
        private void OnCancelSaveVariables(object obj)
        {
            InitSelectPatientDays();
        }

        //编辑公式
        private void OnEditVariableExpression(object obj)
        {
            if (SelectPatient.SelectedDay.SelectedVariable == null)
            {
                MessageViewModel.SetMessage("请先选择一项数据");
                DialogViewModel = MessageViewModel;
                return;
            }

            VariableExpressionViewModel.SetPatient((Patient)SelectPatient.Clone());
            _windowService.ShowDialog("公式设置", VariableExpressionViewModel);
        }
        private void OnExpressionItemsEditCompleted(object sender, VariableEditEventArgs e)
        {
            if (!e.IsCancel)
            {
                if (e.ExpressionItems.Count > 0)
                {
                    //更新公式
                    var noneExpression = e.ExpressionItems.FirstOrDefault(a => a.Name == "无公式");
                    if (noneExpression != null && e.ExpressionItems.Count > 1)
                    {
                        e.ExpressionItems.Remove(noneExpression);
                    }

                    SelectPatient.SelectedDay.SelectedVariable.Formula.ExpressionItems.Clear();
                    foreach (var item in e.ExpressionItems)
                    {
                        SelectPatient.SelectedDay.SelectedVariable.Formula.ExpressionItems.Add(new ExpressionItem(item.Name, item.Value));
                    }

                    SelectPatient.SelectedDay.SelectedVariable.Formula.Expression = string.Join("",
                        e.ExpressionItems.Select(item => item.Name));
                    SelectPatient.SelectedDay.SelectedVariable.Formula.MetaExpression = string.Join(",",
                        e.ExpressionItems.Select(item => item.Name));
                }
                else
                {
                    SelectPatient.SelectedDay.SelectedVariable.Formula.ExpressionItems.Clear();
                    SelectPatient.SelectedDay.SelectedVariable.Formula.Expression = "无公式";
                    SelectPatient.SelectedDay.SelectedVariable.Formula.MetaExpression = "无公式";
                }

                SelectPatient.SelectedDay.IsDirty = true;
                SelectPatient.IsDirty = true;
            }

            IsDialogOpen = false;
        }

        //变量属性更改
        private void PatientVariable_OnPropertyChanged(object sender, VariablePropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsChecked":
                    SelectPatient.SelectedDay.RaiseCheckedAll();
                    RemoveVariableCommand.RaiseCanExecuteChanged();
                    break;
                case "Name":
                    var matches = SelectPatient.SelectedDay.Variables.Where(v => v.Name == e.NewValue);
                    if (matches.Count() > 1)
                    {
                        MessageViewModel.Message = $"已存在名为 {e.NewValue} 的数据";
                        DialogViewModel = MessageViewModel;
                        e.Variable.RevertName(e.OldValue);
                    }
                    else
                    {
                        var list = new List<string>()
                        {
                            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                            "+","-","*","/","(",")",
                        };
                        if (list.Contains(e.NewValue))
                        {
                            MessageViewModel.Message = $"不能以 {e.NewValue} 命名";
                            DialogViewModel = MessageViewModel;
                            e.Variable.RevertName(e.OldValue);
                            return;
                        }

                        //更新所有公式里的变量
                        foreach (var patientVariable in SelectPatient.SelectedDay.Variables)
                        {
                            var formula = patientVariable.Formula;
                            foreach (var expressionItem in formula.ExpressionItems)
                            {
                                if (expressionItem.Name == e.OldValue)
                                {
                                    expressionItem.Name = e.NewValue;
                                }
                            }

                            formula.Expression = string.Join("", formula.ExpressionItems.Select(c => c.Name));
                            formula.MetaExpression = string.Join(",", formula.ExpressionItems.Select(c => c.Name));

                            if (string.IsNullOrEmpty(formula.Expression))
                            {
                                formula.Expression = "无公式";
                                formula.MetaExpression = "无公式";
                            }

                            //更新联动设置
                            for (var i = 0; i < patientVariable.FollowVariables.Count; i++)
                            {
                                if (patientVariable.FollowVariables[i] == e.OldValue)
                                {
                                    patientVariable.FollowVariables[i] = e.NewValue;
                                }
                            }
                        }
                    }
                    break;
                case "Value":
                    //更新所有公式里的变量
                    foreach (var patientVariable in SelectPatient.SelectedDay.Variables)
                    {
                        var formula = patientVariable.Formula;
                        foreach (var expressionItem in formula.ExpressionItems)
                        {
                            if (expressionItem.Name == e.Variable.Name)
                            {
                                expressionItem.Value = e.NewValue;
                            }
                        }
                    }

                    //联动计算
                    foreach (var variableName in e.Variable.FollowVariables)
                    {
                        var next = SelectPatient.SelectedDay.Variables.FirstOrDefault(a => a.Name == variableName);
                        if (next != null)
                        {
                            var expression = string.Join("", next.Formula.ExpressionItems.Select(a => a.Value));
                            if (string.IsNullOrEmpty(expression))
                            {
                                continue;
                            }

                            var result = _parser.Parse(expression).ToString(CultureInfo.InvariantCulture);
                            result = Math.Round(double.Parse(result), 4).ToString(CultureInfo.InvariantCulture);
                            next.Value = result;
                        }
                    }
                    break;
                case "Min":
                    break;
                case "Max":
                    break;
                case "Unit":
                    break;
            }

            SelectPatient.SelectedDay.IsDirty = true;
        }
        
        //动画消息弹窗
        private void ShowMessage(string message)
        {
            MessageViewModel.SetMessage(message);
            DialogViewModel = MessageViewModel;
        }
        private void OnMessageClosed(object sender, EventArgs e)
        {
            IsDialogOpen = false;
        }

        //限制
        private void OnSelectedDailyVariableChanged(object sender, EventArgs e)
        {
            RemoveVariableCommand.RaiseCanExecuteChanged();
        }
        private void OnSelectedDailyAllVariableChanged(object sender, EventArgs e)
        {
            RemoveVariableCommand.RaiseCanExecuteChanged();
        }
        private void OnCommonInfoChanged(object sender, EventArgs e)
        {
            SelectPatient.SelectedDay.IsDirty = true;
        }

        //其他
        private void VariableTemplatesMaintainViewModelOnOnError(object sender, string error)
        {
            ShowMessage(error);
        }
        private void BackupDbFile()
        {
            var source = @"CustomCalculator\Data\data.db";
            var backup = @"CustomCalculator\Data\data_backup.db";

            var sourceFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), source);
            var backupFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), backup);
            //备份数据库文件
            File.Copy(sourceFile, backupFile, overwrite: true);
        }
    }
}
