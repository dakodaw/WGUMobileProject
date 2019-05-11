using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace WGUMobileProject.Model
{
    [Table("courses")]
    public class Course
    {
        [PrimaryKey, AutoIncrement]
        public int CourseId { get; set; }

        [MaxLength(250), Unique]
        public string CourseName { get; set; }
        public string CourseProgress { get; set; }
        public string CourseDetails { get; set; }
        public bool NotifyOnOff { get; set; }
        public string OptionalNotes { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        //Semester Info
        public int SemesterId { get; set; }

        //Instructor Info
        public string InstructorName { get; set; }
        public string InstructorEmail { get; set; }
        public string InstructorPhone { get; set; }
        
        //Assessment Information
        public bool PerformanceAssessmentAdded { get; set; }
        public bool ObjectiveAssessmentAdded { get; set; }
        public string PAName { get; set; }
        public string OAName { get; set; }
        public DateTime ScheduledOAAssessmentTime { get; set; }
        public DateTime ScheduledPAAssessmentTime { get; set; }

    }
}
