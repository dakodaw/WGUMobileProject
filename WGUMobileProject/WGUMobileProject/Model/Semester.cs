using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace WGUMobileProject.Model
{
    [Table("semesters")]
    public class Semester
    {
        [PrimaryKey, AutoIncrement]
        public int SemesterId { get; set; }
        [Unique]
        public string SemesterName { get; set; }

        //Start and End Dates
        public DateTime SemesterStart { get; set; }
        public DateTime SemesterEnd { get; set; }

    }
}
