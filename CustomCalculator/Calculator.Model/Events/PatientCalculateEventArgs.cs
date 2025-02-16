namespace Calculator.Model.Events
{
    public class PatientCalculateEventArgs
    {
        public double Property1 { get;}
        public double Property2 { get;}
        public double Property3 { get;}
        public double Property4 { get;}

        public bool IsCancel { get; }


        public PatientCalculateEventArgs(double property1, double property2, double property3, double property4, bool isCancel)
        {
            Property1 = property1;
            Property2 = property2;
            Property3 = property3;
            Property4 = property4;
            IsCancel = isCancel;
        }
    }
}
