namespace eSolver.Entities
{
    public class ScheduleRanking
    {
        public int Id { get; set; }
        public int? SortOrder { get; set; }
        public int? RankingRuleID { get; set; }
        public int? RankingTypeID { get; set; }
        public int? ScheduleID { get; set; }
    }
}
