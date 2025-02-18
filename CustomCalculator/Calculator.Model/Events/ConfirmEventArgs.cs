namespace Calculator.Model.Events
{
    public class ConfirmEventArgs
    {
        public bool IsCancel { get; }
        public ConfirmEventArgs(bool isCancel)
        {
            IsCancel = isCancel;
        }
    }
}
