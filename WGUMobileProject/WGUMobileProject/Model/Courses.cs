using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace WGUMobileProject.Model
{
    public class Courses
    {
        SQLiteAsyncConnection conn; 
        public string StatusMessage { get; set; }
        public List<Course> allCourses;

        public Courses(string dbPath)
        {
            conn = new SQLiteAsyncConnection(dbPath);
            conn.CreateTableAsync<Course>().Wait();
            allCourses = new List<Course>();
        }
 

        public async Task AddNewCourseAsync(string courseName, string courseProgress, string courseDetails, bool notifyOnOff, string optionalNotes, string pictureName, string instructorName, string instructorEmail, string instructorPhone, bool performanceAssessmentAdded, bool objectiveAssessmentAdded, string paName, string oaName)
        {
            int result = 0;
            try
            {
                //basic validation
                if (string.IsNullOrEmpty(courseName))
                    throw new Exception("Valid name required");

                result = await conn.InsertAsync(new Course
                {
                   CourseName = courseName,
                   CourseProgress = courseProgress,
                   CourseDetails = courseDetails,
                   NotifyOnOff = notifyOnOff,
                   OptionalNotes = optionalNotes,
                   PictureName = pictureName,
                   InstructorName = instructorName,
                   InstructorEmail = instructorEmail,
                   InstructorPhone = instructorPhone,
                   PerformanceAssessmentAdded = performanceAssessmentAdded,
                   ObjectiveAssessmentAdded = objectiveAssessmentAdded,
                   PAName = paName,
                   OAName = oaName
                });

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, courseName);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", courseName, ex.Message);
            }
        }

        public async Task UpdateCourseAsync(int courseId, string courseName, string courseProgress, string courseDetails, bool notifyOnOff, string optionalNotes, string pictureName, string instructorName, string instructorEmail, string instructorPhone, bool performanceAssessmentAdded, bool objectiveAssessmentAdded, string paName, string oaName, DateTime scheduledPAAssessmentTime, DateTime scheduledOAAssessmentTime)
        {
            int result = 0;
            try
            {
                result = await conn.UpdateAsync(new Course{
                    CourseId = courseId,
                    CourseName = courseName,
                    CourseProgress = courseProgress,
                    CourseDetails = courseDetails,
                    NotifyOnOff = notifyOnOff,
                    OptionalNotes = optionalNotes,
                    PictureName = pictureName,
                    InstructorName = instructorName,
                    InstructorEmail = instructorEmail,
                    InstructorPhone = instructorPhone,
                    PerformanceAssessmentAdded = performanceAssessmentAdded,
                    ObjectiveAssessmentAdded = objectiveAssessmentAdded,
                    PAName = paName,
                    OAName = oaName,
                    ScheduledOAAssessmentTime = scheduledOAAssessmentTime,
                    ScheduledPAAssessmentTime = scheduledPAAssessmentTime
                });
                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, courseName);
            }
            catch(Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", courseName, ex.Message);
            }
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            try
            {
                return await conn.Table<Course>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new List<Course>();
        }
    }
}
