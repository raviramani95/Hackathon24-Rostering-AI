namespace eSolver.Entities
{
    public class ScheduleCustomData
    {
        public int ScheduleID { get; set; }
        public int? CustomDataID { get; set; }
        public int? CustomDataLookupID { get; set; }
        public string CustomDataLookupContent { get; set; }
        public double? NumberValue { get; set; }
        public string TextValue { get; set; }
    }
}
