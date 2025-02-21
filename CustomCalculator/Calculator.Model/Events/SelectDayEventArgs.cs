using Calculator.Model.Models;

namespace Calculator.Model.Events
{
    public class SelectDayEventArgs
    {
        public DailyInfo OldItem { get; }
        public DailyInfo NewItem { get; }
        public SelectDayEventArgs(DailyInfo oldItem, DailyInfo newItem)
        {
            OldItem = oldItem;
            NewItem = newItem;
        }
    }
}
