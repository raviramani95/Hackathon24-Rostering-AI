using System;
using System.Collections.Generic;
using System.Text;

namespace eSolver.AutoSolverNew.Test
{
    public class JsonGenerator
    {
        /// <summary>
        /// This method is used in development purpose in order to generate json file with data for autosolve
        /// </summary>
        /// <param name="numberOfEmployees"></param>
        /// <param name="numberOfJobs"></param>
        /// <returns></returns>
        //public void Generate()
        //{
        //    int numberOfEmployees = 200;
        //    int numberOfJobs = 400;

        //    string pathToJson = ".\\..\\..\\..\\AutoSolverNew\\Test\\GeneratedJsons\\Test.json";

        //    List<string> partsOfJsons = TemplateJson(numberOfEmployees, numberOfJobs);

        //    string[] lines = partsOfJsons.ToArray();
        //    System.IO.File.WriteAllLines(pathToJson, lines);       
        //}


        /// <summary>
        /// This method is used in development purpose in order to generate json file with data for autosolve
        /// </summary>
        /// <param name="numberOfEmployees"></param>
        /// <param name="numberOfJobs"></param>
        /// <returns></returns>
//        public List<string> TemplateJson(int numberOfEmployees, int numberOfJobs)
//        {
//            List<string> parts = new List<string>();

//            string startJson = @"{ 
//    ""MaxNumOfJobsAtSameTime"":1,
//    ""MaxTime"":300.0,
//    ""StartDayOfTheWeek"":""Monday"",
//    ""Maximize"":true,
//    ""FinalConsoleOutput"":false,
//    ""PrintSolutions"":false,
//    ""Employees"": [";
//            parts.Add(startJson);

//            Random random = new Random();
//            for (int i = 0; i < numberOfEmployees; i++)
//            {
//                var emoloyeeJson = String.Join(Environment.NewLine,
//                 "\t\t{ ",
//        "\t\t\t \"Id\": " + (i + 1) + ",",
//        "\t\t\t \"JobTypeID\": 2,",
//        "\t\t\t \"StartDate\": \"2020 - 12 - 01T00: 00:00\",",
//        "\t\t\t \"LeaveDate\": null,",
//        "\t\t\t \"DateOfBirth\": null,",
//        "\t\t\t \"EmployeeNumber\": \"1\",",
//        "\t\t\t \"Firstname \": \"1\",",
//        "\t\t\t \"Surname \": \"1\",",
//        "\t\t\t \"Address2 \": \"\",",
//        "\t\t\t \"Address1 \": \"\",",
//        "\t\t\t \"Address3 \": \"\",",
//        "\t\t\t \"CostCode \": null,",
//        "\t\t\t \"Address4 \": \"\",",
//        "\t\t\t \"TelephoneNumber \": \"\",",
//        "\t\t\t \"MobileNumber \": \"\",",
//        "\t\t\t \"TargetRuleGroup \": null,",
//        "\t\t\t \"WorkRules \": null,",
//        "\t\t\t \"Email \": \"1@gmail.com\",",
//        "\t\t\t \"Username \": \"1\",",
//        "\t\t\t \"EmployeeProfile \": null,",
//        "\t\t\t \"Gender \": null,",
//        "\t\t\t \"Location \": null,",
//        "\t\t\t \"Department \": null,",
//        "\t\t\t \"Division \": null,",
//        "\t\t\t \"JobTitle \": null,",
//        "\t\t\t \"Team \": null,",
//        "\t\t\t \"Class \": null,",
//        "\t\t\t \"Notification \": \"Push \",",
//        "\t\t\t \"Manager \": null,",
//        "\t\t\t \"JobType \": \"Lekar \",",
//        "\t\t\t \"PayRate \": null,",
//        "\t\t\t \"MaxHours1 \": null,",
//        "\t\t\t \"MaxHours2 \": null,",
//        "\t\t\t \"MaxHours3 \": null,",
//        "\t\t\t \"MaxHours4 \": null,",
//        "\t\t\t \"MaxHours5 \": null,",
//        "\t\t\t \"AllowedTrades \": false,",
//        "\t\t\t \"Availability \": false,",
//        "\t\t },"

//                    );
//            parts.Add(emoloyeeJson);
//            }
//            string endJsonEmployee = @"
//     ],";
//            parts.Add(endJsonEmployee);
//            parts.Add("\t\t\"JobOutsides\": [],");
//            parts.Add(String.Join(Environment.NewLine, 
//                "\t\t\"ScheduleRankings\": [",
//"\t\t    {",
//"\t\t      \"Id\": 1,",
//"\t\t      \"SortOrder\": 1,",
//"\t\t      \"RankingRuleID\": 1,",
//"\t\t      \"RankingTypeID\": 4,",
//"\t\t      \"ScheduleID\": 1",
//"\t\t    }",
//"\t\t  ],",
//"\t\t\"RankingRules\": [",
//"\t\t    {",
//"\t\t      \"Id\": 1,",
//"\t\t      \"ReverseOrder\": false,",
//"\t\t      \"DateRange\": \"Month\",",
//"\t\t      \"Offset\": 0.0,",
//"\t\t      \"Amount\": 0.0,",
//"\t\t      \"BaseDate\": null,",
//"\t\t      \"BaseDateRange\": 0,",
//"\t\t      \"DayOfWeek\": \"Wednesday\",",
//"\t\t      \"CountOvernights\": false,",
//"\t\t      \"JobStartFrom\": null,",
//"\t\t      \"JobStartTo\": null,",
//"\t\t      \"JobEndFrom\": null,",
//"\t\t      \"JobEndTo\": null,",
//"\t\t      \"ComparisonMode\": null,",
//"\t\t      \"ComparisonValue\": null,",
//"\t\t      \"Operator\": null,",
//"\t\t      \"EmployeeField\": null,",
//"\t\t      \"RestrictedToJobTypeID\": null,",
//"\t\t      \"CustomDataID\": null,",
//"\t\t      \"JobType\": null,",
//"\t\t      \"CustomData\": null",
//"\t\t    },",
//"\t\t    {",
//"\t\t      \"Id\": 2,",
//"\t\t      \"ReverseOrder\": false,",
//"\t\t      \"DateRange\": null,",
//"\t\t      \"Offset\": null,",
//"\t\t      \"Amount\": null,",
//"\t\t      \"BaseDate\": null,",
//"\t\t      \"BaseDateRange\": null,",
//"\t\t      \"DayOfWeek\": null,",
//"\t\t      \"CountOvernights\": false,",
//"\t\t      \"JobStartFrom\": null,",
//"\t\t      \"JobStartTo\": null,",
//"\t\t      \"JobEndFrom\": null,",
//"\t\t      \"JobEndTo\": null,",
//"\t\t      \"ComparisonMode\": null,",
//"\t\t      \"ComparisonValue\": null,",
//"\t\t      \"Operator\": null,",
//"\t\t      \"EmployeeField\": null,",
//"\t\t      \"RestrictedToJobTypeID\": null,",
//"\t\t      \"CustomDataID\": null,",
//"\t\t      \"JobType\": null,",
//"\t\t      \"CustomData\": null", 
//"\t\t    }",
//"\t\t  ],",
//"\t\t\"AllConstraintRules\": [",
//"\t\t    {",
//"\t\t      \"Id\": 1,",
//"\t\t      \"Name\": \"Not Already Assigned\"",
//"\t\t    },",
//"\t\t    {",
//"\t\t                \"Id\": 2,",
//"\t\t      \"Name\": \"Maximum Number Of Hours\"",
//"\t\t    },",
//"\t\t    {",
//"\t\t                \"Id\": 4,",
//"\t\t      \"Name\": \"Maximum Number Of Days Of The Week\"",
//"\t\t    },",
//"\t\t    {",
//"\t\t                \"Id\": 5,",
//"\t\t      \"Name\": \"Employee Must Not Work With Another Employee\"",
//"\t\t    },",
//"\t\t    {",
//"\t\t                \"Id\": 6,",
//"\t\t      \"Name\": \"Employee Must Work With Another Employee\"",
//"\t\t    },",
//"\t\t    {",
//"\t\t                \"Id\": 7,",
//"\t\t      \"Name\": \"Maximum Number Of Job Types\"",
//"\t\t    },",
//"\t\t    {",
//"\t\t                \"Id\": 8,",
//"\t\t      \"Name\": \"Maximum Number Of Job Types Consecutively\"",
//"\t\t    },",
//"\t\t    {",
//"\t\t                \"Id\": 9,",
//"\t\t      \"Name\": \"Employee Field Value\"",
//"\t\t    },",
//"\t\t    {",
//"\t\t                \"Id\": 10,",
//"\t\t      \"Name\": \"Split Shifts\"",
//"\t\t    },",
//"\t\t    {",
//"\t\t               \"Id\": 11,",
//"\t\t      \"Name\": \"Days Off Within A Period\"",
//"\t\t    },",
//"\t\t    {",
//"\t\t                \"Id\": 12,",
//"\t\t      \"Name\": \"Employee Planned Absence\"",
//"\t\t    }",
//"\t\t  ],",
//"\t\t\"RankingTypes\": [",
//"\t\t    {",
//"\t\t                \"Id\": 1,",
//"\t\t      \"Name\": \"Scheduled Hours\"",
//"\t\t    },",
//"\t\t    {",
//"\t\t                \"Id\": 2,",
//"\t\t      \"Name\": \"Default job\"",
//"\t\t    },",
//"\t\t    {",
//"\t\t                \"Id\": 3,",
//"\t\t      \"Name\": \"Job Type Frequency\"",
//"\t\t    },",
//"\t\t    {",
//"\t\t                \"Id\": 4,",
//"\t\t      \"Name\": \"Day of week frequency\"",
//"\t\t    },",
//"\t\t    {",
//"\t\t                \"Id\": 5,",
//"\t\t      \"Name\": \"Employee field value\"",
//"\t\t    }",
//"\t\t  ],",
//"\t\t\"Schedule\": {",
//"\t\t    \"ScheduleID\": 1,",
//"\t\t    \"ScheduleActiveConstraints\": [",
//"\t\t      {",
//"\t\t        \"ConstraintRuleID\": 1,",
//"\t\t        \"ScheduleID\": 1,",
//"\t\t        \"ConstraintName\": \"Not Already Assigned\",",
//"\t\t        \"IsConstraintActive\": true,",
//"\t\t        \"Schedule\": null,",
//"\t\t        \"ConstraintRule\": null,",
//"\t\t        \"ConstraintEMWWAEs\": null,",
//"\t\t        \"ConstraintEMNWWAEs\": null,",
//"\t\t        \"ConstraintSetValues\": null,",
//"\t\t        \"ConstraintMNODOTW\": null,",
//"\t\t        \"ConstraintMNOH\": null,",
//"\t\t        \"ConstraintDOWP\": null,",
//"\t\t        \"ConstraintSS\": null,",
//"\t\t        \"Id\": 1",
//"\t\t      }",
//"\t\t    ],",
//"\t\t    \"ScheduleJobs\": ["
//                ));

//            for (int i = 0; i < numberOfJobs; i++)
//            {
//                long startDayi = (random.Next(27) + 1);
//                long startHouri = (random.Next(20) + 1);
//                long startMinutei = (random.Next(58) + 1);

//                long endDayi = (random.Next(27) + 3);
//                long endHouri = (random.Next((int)(22 - startHouri)) + 1 + startHouri);
//                long endMinutei = (random.Next(58) + 1);

//                string startDay = startDayi < 10 ? "0" + startDayi.ToString() : startDayi.ToString();
//                string startHour = startHouri < 10 ? "0" + startHouri.ToString() : startHouri.ToString();
//                string startMinute = startMinutei < 10 ? "0" + startMinutei.ToString() : startMinutei.ToString();

//                string endDay = endDayi < 10 ? "0" + endDayi.ToString() : endDayi.ToString();
//                string endHour = endHouri < 10 ? "0" + endHouri.ToString() : endHouri.ToString();
//                string endMinute = endMinutei < 10 ? "0" + endMinutei.ToString() : endMinutei.ToString();

//                //Console.WriteLine();
//                //Console.WriteLine(endHouri);
//                //Console.WriteLine(startHouri);
//                //Console.WriteLine(endMinutei);
//                //Console.WriteLine(startMinutei);
//                //Console.WriteLine((endHouri * 60 + endMinutei - startHouri * 60 - startMinutei));

//                //Console.WriteLine((endHouri * 60 + endMinutei - startHouri * 60 - startMinutei) * 60 * 100000000);
//                var emoloyeeJson = String.Join(Environment.NewLine,

                    
// "\t\t     {",
// "\t\t       \"Id\": " + i + ",",
// "\t\t       \"JobStartDateTime\": \"2021-06-" + startDay + "T" + startHour + ":" + startMinute + ":00\",",
// "\t\t       \"JobEndDateTime\": \"2021-06-" + startDay + "T" + endHour + ":" + endMinute + ":00\",",
// "\t\t       \"Hours\": " + (endHouri * 60 + endMinutei - startHouri * 60 - startMinutei) * 60 * 100000000 + ",",
// "\t\t       \"Hours1\": 0,",
// "\t\t       \"Hours2\": 0,",
// "\t\t       \"Hours3\": 0,",
// "\t\t       \"Hours4\": 0,",
// "\t\t       \"Hours5\": 0,",
// "\t\t       \"Hours6\": 0,",
// "\t\t       \"NoOfEmployeesRequired\": " + (random.Next(10) + 1) + ",",
// "\t\t       \"SubgroupID\": 1,",
// "\t\t       \"JobtypeID\": 3,",
// "\t\t       \"JobCustomData\": [],",
// "\t\t       \"UnavailableEmployeesForJob\": []",
// "\t\t     },") ;
//                parts.Add(emoloyeeJson);
//            }

//            parts.Add(String.Join(Environment.NewLine,
//"\t\t                ]",
//"\t\t  },",
//"\t\t\"PayPeriodDTO\": null"
//        ));
//            string endJson = @"}";
//            parts.Add(endJson);
//            return parts;
//        }
    }
}
