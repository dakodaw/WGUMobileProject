using System;
using System.Collections.Generic;
using System.Text;
using WGUMobileProject.Views;

namespace WGUMobileProject.Model
{
    public class StringOptions
    {
        public string PageCourseView { get; set; } = "Course View";
        public string TitleCourseOverview { get; set; } = "Class Overview";
        public string TitleOptionalNotes { get; set; } = "Optional Notes";

        public string TypeName { get; set; } = "Name";
        public string TypeEmail { get; set; } = "Email";
        public string TypePhone { get; set; } = "Phone";
        public string TypeCourseName { get; set; } = "Course Name";
        public string TypeSemesterName { get; set; } = "Semester Name";
        public string TypeCourseInstructor { get; set; } = "Course Instructor";
        public string TypeAssessment { get; set; } = "Assessment";
        public string TypeNotes { get; set; } = "Notes";
        public string TypeChangeSemester { get; set; } = "Change Semester";

        public string AssessmentTypePerformance { get; set; } = "a Performance";
        public string AssessmentTypeObjective { get; set; } = "an Objective";
        public string BackButton { get; set; } = "BackArrowSmall.png";
        public string EditButton { get; set; } = "EditIcon.png";

        //Dropdown Picker Options
        public string PickerInProgress { get; set; } = "In Progress";
        public string PickerComplete { get; set; } = "Completed";
        public string PickerPlannedToTake { get; set; } = "Plan to Take";
        public string PickerDrop { get; set; } = "Dropped";
        public string EditTextViewTypeS { get; set; } = "WGUMobileProject.Views.EditTextView";

        public string Test { get; set; } = "Test";
        public string Start { get; set; } = "Start Date";
        public string End { get; set; } = "End Date";
    }
}
