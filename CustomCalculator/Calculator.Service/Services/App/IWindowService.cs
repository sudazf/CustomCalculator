namespace Calculator.Service.Services.App
{
    public interface IWindowService
    {
        void ShowDialog(string title, object viewModel);
        void Close();
        object Result { get; }
    }
}
