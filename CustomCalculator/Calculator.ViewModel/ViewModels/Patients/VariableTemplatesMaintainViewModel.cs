using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Calculator.Model.Events;
using Calculator.Model.Models;
using Calculator.Service.Services.App;
using Calculator.Service.Services.Database;
using Calculator.ViewModel.ViewModels.Applications;
using Jg.wpf.core.Command;
using Jg.wpf.core.Notify;
using Jg.wpf.core.Profilers;
using Jg.wpf.core.Service;
using Jg.wpf.core.Service.ThreadService;

namespace Calculator.ViewModel.ViewModels.Patients
{
    public class VariableTemplatesMaintainViewModel : ViewModelBase
    {
        private readonly ISQLiteDataService _dbService;
        private readonly TaskManager _queryManager;
        private readonly IDispatcher _dispatcher;
        private readonly IWindowService _windowService;
        private bool _isCheckedAll;

        public event EventHandler<string> OnError;

        public ObservableCollection<SimpleVariableTemplate> Templates { get; }

        public bool IsCheckedAll
        {
            get => _isCheckedAll;
            set
            {
                if (value == _isCheckedAll) return;
                _isCheckedAll = value;

                foreach (var template in Templates)
                {
                    template.UpdateCheck(_isCheckedAll);
                }

                RaisePropertyChanged(nameof(IsCheckedAll));
                RemoveTemplatesCommand.RaiseCanExecuteChanged();
            }
        }

        public JCommand QueryTemplatesCommand { get; }
        public JCommand RemoveTemplatesCommand { get; }

        public VariableTemplatesMaintainViewModel()
        {
            _dbService = ServiceManager.GetService<ISQLiteDataService>();
            _queryManager = new TaskManager("TemplatesQueryManager");
            _dispatcher = ServiceManager.GetService<IDispatcher>();
            _windowService = ServiceManager.GetService<IWindowService>();

            QueryTemplatesCommand = new JCommand("QueryTemplatesCommand", QueryTemplates);
            RemoveTemplatesCommand = new JCommand("RemoveTemplatesCommand", RemoveTemplates, CanRemove);
            Templates = new ObservableCollection<SimpleVariableTemplate>();
        }

        private void QueryTemplates(object obj)
        {
            _queryManager.StartNewTaskProxy("QueryTemplatesTask", OnQueryTemplates);
        }
        private void OnQueryTemplates(TaskProxy obj)
        {
            var dataTable = _dbService.GetTemplateNames();

            _dispatcher.Invoke(() =>
            {
                foreach (var template in Templates)
                {
                    template.OnCheckedChanged -= OnTemplateItemCheckedChanged;
                    template.OnNameChanged -= OnTemplateItemNameChanged;
                }

                Templates.Clear();
                _isCheckedAll = false;
                RaisePropertyChanged(nameof(IsCheckedAll));

                foreach (DataRow row in dataTable.Rows)
                {
                    var name = row["template_name"].ToString();

                    var newTemplate = new SimpleVariableTemplate(name);
                    newTemplate.OnCheckedChanged += OnTemplateItemCheckedChanged;
                    newTemplate.OnNameChanged += OnTemplateItemNameChanged;
                    Templates.Add(newTemplate);
                }
            });
        }

        private void RemoveTemplates(object obj)
        {
            var removes = Templates.Where(t => t.IsChecked).Select(t => $"\"{t.Name}\"").ToList();
            _windowService.ShowDialog("系统提示",
                new ConfirmViewModel("确定要删除以下模板?\r\n" + $"{string.Join(" \r\n", removes)}"));
            if (_windowService.Result is bool confirm)
            {
                if (confirm)
                {
                    try
                    {
                        foreach (var name in removes)
                        {
                            _dbService.RemoveTemplate(name);
                            Templates.Remove(Templates.FirstOrDefault(t => t.Name == name));
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        OnError?.Invoke(this, e.Message);
                    }
                }
            }
        }
        private bool CanRemove(object arg)
        {
            return Templates.Count(t => t.IsChecked) > 0;
        }

        private void OnTemplateItemNameChanged(object sender, TemplateNameChangedEventArgs e)
        {
            _windowService.ShowDialog("系统提示",
                new ConfirmViewModel($"确定要将模板名字从 \"{e.OldName}\" 改为 \"{e.NewName}\" ?"));
            if (_windowService.Result is bool confirm)
            {
                if (confirm)
                {
                    try
                    {
                        _dbService.ChangeTemplateName(e.OldName, e.NewName);
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine(error);
                        OnError?.Invoke(this, error.Message);
                    }
                }
                else
                {
                    e.Template.RevertName();
                }
            }
        }

        private void OnTemplateItemCheckedChanged(object sender, EventArgs e)
        {
            if (Templates.Count(t => t.IsChecked) == Templates.Count)
            {
                _isCheckedAll = true;
            }
            else
            {
                _isCheckedAll = false;
            }

            RaisePropertyChanged(nameof(IsCheckedAll));
            RemoveTemplatesCommand.RaiseCanExecuteChanged();
        }
    }
}
