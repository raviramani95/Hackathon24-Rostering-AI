using eSolver.Entities;
using System;
using System.Collections.Generic;

namespace eSolver.Entities.Interfaces
{
	/// <summary>
	/// Interface that models JobBase
	/// </summary>
	public interface IJobBase
    {
		#region Properties

		/// <summary>
		/// Represents Job Primary Key
		/// </summary>
		long JobID { get; set; }
		/// <summary>
		/// Job StartDateTime
		/// </summary>
		DateTime JobStartDate { get; set; }
		/// <summary>
		/// Job EndDateTime
		/// </summary>
		DateTime JobEndDate { get; set; }
		/// <summary>
		/// Sum of all working hours
		/// </summary>
		TimeSpan HoursT { get; set; }
		/// <summary>
		/// Working Hours on First Day
		/// </summary>
		TimeSpan HoursT1 { get; set; }
		/// <summary>
		/// Working Hours on Second Day
		/// </summary>
		TimeSpan HoursT2 { get; set; }
		/// <summary>
		/// Working Hours on Third Day
		/// </summary>
		TimeSpan HoursT3 { get; set; }
		/// <summary>
		/// Working Hours on Fourth Day
		/// </summary>
		TimeSpan HoursT4 { get; set; }
		/// <summary>
		/// Working Hours on Fifth Day
		/// </summary>
		TimeSpan HoursT5 { get; set; }
		/// <summary>
		/// Working Hours on Sixth Day
		/// </summary>
		TimeSpan HoursT6 { get; set; }
		/// <summary>
		/// Helper data for Sum of Working Hours
		/// </summary>
		long Hours { get; set; }
		/// <summary>
		/// Helper data for Sum of Working Hours on First Day
		/// </summary>
		long Hours1 { get; set; }
		/// <summary>
		/// Helper data for Sum of Working Hours on Second Day
		/// </summary>
		long Hours2 { get; set; }
		/// <summary>
		/// Helper data for Sum of Working Hours on Third Day
		/// </summary>
		long Hours3 { get; set; }
		/// <summary>
		/// Helper data for Sum of Working Hours on Fourth Day
		/// </summary>
		long Hours4 { get; set; }
		/// <summary>
		/// Helper data for Sum of Working Hours on Five Day
		/// </summary>
		long Hours5 { get; set; }
		/// <summary>
		/// Helper data for Sum of Working Hours on Sixth Day
		/// </summary>
		long Hours6 { get; set; }
		/// <summary>
		/// This property is used for dividing seconds
		/// </summary>
		int COUNT_OF_MILISECONDS { get; }
		/// <summary>
		/// Job Custom Data
		/// </summary>
		List<ScheduleCustomData> JobCustomData { get; set; }
		/// <summary>
		/// JobType of current Job
		/// </summary>
		public long JobTypeID { get; set; }
		/// <summary>
		/// SubGroup of Job
		/// </summary>
		public long SubGroupID { get; set; }

		#endregion
	}
}
