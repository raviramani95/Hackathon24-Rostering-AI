using System;
using System.Collections.Generic;

namespace eSolver.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string EmployeeNumber { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public long? JobTypeID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? LeaveDate { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string TelephoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public int? MaxHours1 { get; set; }
        public int? MaxHours2 { get; set; }
        public int? MaxHours3 { get; set; }
        public int? MaxHours4 { get; set; }
        public int? MaxHours5 { get; set; }
        public bool? AllowedTrades { get; set; }
        public bool? Availability { get; set; }
        public bool? NightWorker { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string Profile { get; set; }
        public string Jobtype { get; set; }
        public string Manager { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }

        public string PayRate { get; set; }
        public string TargetRuleGroup { get; set; }
        public string Gender { get; set; }
        public string Location { get; set; }
        public List<string> Locations { get; set; }
        public string Department { get; set; }
        public string Division { get; set; }
        public string JobTitle { get; set; }
        public string Class { get; set; }
        public string Notification { get; set; }
        public string Team { get; set; }
        public string CostCode { get; set; }

    }
}
