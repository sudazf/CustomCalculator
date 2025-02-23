using System;
using Jg.wpf.core.Extensions.Types.Pages;
using Jg.wpf.core.Notify;

namespace Calculator.ViewModel.ViewModels.Patients
{
    public class PatientPagingViewModel : ViewModelBase
    {
        private int _recordCount = 0;
        private int _pageSize = 10;
        private int _currentPage = 1;

        public event EventHandler<PageChangedEventArgs> OnPageChanged;

        public int RecordCount
        {
            get => _recordCount;
            set
            {
                if (value == _recordCount) return;
                _recordCount = value;

                var pageCount = (_recordCount - 1) / _pageSize + 1; //计算总页数PageCount

                if (_currentPage > pageCount)
                {
                    CurrentPage = pageCount;
                }
                RaisePropertyChanged(nameof(RecordCount));
            }
        }
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value == _pageSize) return;
                _pageSize = value;
                RaisePropertyChanged(nameof(PageSize));
            }
        }
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (value == _currentPage) return;
                _currentPage = value;
                OnPageChanged?.Invoke(this, new PageChangedEventArgs(_currentPage));
                RaisePropertyChanged(nameof(CurrentPage));
            }
        }

        public PatientPagingViewModel()
        {
        }

        public void SetCurrentPage(int page)
        {
            _currentPage = page;
            RaisePropertyChanged(nameof(CurrentPage));
        }
    }
}
