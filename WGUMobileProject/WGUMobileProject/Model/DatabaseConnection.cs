using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace WGUMobileProject.Model
{
    public class DatabaseConnection
    {
        SQLiteAsyncConnection conn; 
        public string StatusMessage { get; set; }
        public List<Course> allCourses;
        public List<Semester> allSemesters;
        public TaskStatus checkStatus;

        public DatabaseConnection(string dbPath)
        {
            conn = new SQLiteAsyncConnection(dbPath);
            conn.CreateTableAsync<Course>().Wait();
            conn.CreateTableAsync<Semester>().Wait();
            allSemesters = new List<Semester>();
            allCourses = new List<Course>();
        }
 
        //Course Data Add, Updates, and Retreival
        public async Task AddNewCourseAsync(string courseName, string courseProgress, string courseDetails
            , bool notifyOnOff, string optionalNotes, string instructorName, int semesterId
            , string instructorEmail, string instructorPhone, bool performanceAssessmentAdded
            , bool objectiveAssessmentAdded, string paName, string oaName, DateTime startDate, DateTime endDate
            , DateTime oaScheduledTime, DateTime paScheduledTime)
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
                   InstructorName = instructorName,
                   SemesterId = semesterId,
                   InstructorEmail = instructorEmail,
                   InstructorPhone = instructorPhone,
                   PerformanceAssessmentAdded = performanceAssessmentAdded,
                   ObjectiveAssessmentAdded = objectiveAssessmentAdded,
                   PAName = paName,
                   OAName = oaName,
                   ScheduledPAAssessmentTime = paScheduledTime,
                   ScheduledOAAssessmentTime = oaScheduledTime,
                   StartDate = startDate,
                   EndDate = endDate
                });

                StatusMessage = result.ToString() + " record(s) added [Name: " + courseName + "]";
            }
            catch (Exception ex)
            {
                StatusMessage = "Failed to add data. " + ex.Message.ToString();
            }
        }

        public async Task UpdateCourseAsync(int courseId, string courseName, string courseProgress, string courseDetails
            , bool notifyOnOff, string optionalNotes, string instructorName, int semesterId
            , string instructorEmail, string instructorPhone, bool performanceAssessmentAdded
            , bool objectiveAssessmentAdded, string paName, string oaName, DateTime startDate, DateTime endDate
            , DateTime oaScheduledTime, DateTime paScheduledTime)
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
                    InstructorName = instructorName,
                    InstructorEmail = instructorEmail,
                    InstructorPhone = instructorPhone,
                    PerformanceAssessmentAdded = performanceAssessmentAdded,
                    ObjectiveAssessmentAdded = objectiveAssessmentAdded,
                    PAName = paName,
                    OAName = oaName,
                    ScheduledPAAssessmentTime = paScheduledTime,
                    ScheduledOAAssessmentTime = oaScheduledTime,
                    StartDate = startDate,
                    EndDate = endDate,
                    SemesterId = semesterId
                });
                StatusMessage = result.ToString() + " record(s) updated [Name: " + courseName + "]";
            }
            catch(Exception ex)
            {
                StatusMessage = "Failed to update data. " + ex.Message.ToString();
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
                StatusMessage = "Failed to retreive Data. " + ex.Message.ToString();
            }

            return new List<Course>();
        }

        public async Task RemoveAllCourseData()
        {
            try
            {
                int result;
                result = await conn.DeleteAllAsync<Course>();
                StatusMessage = "Courses successfully deleted";
            }
            catch(Exception ex)
            {
                StatusMessage = "Failed to delete Data. " + ex.Message.ToString();
            }
        }

        //Semester Data Add, Updates, and Retreival
        public async Task AddNewSemesterAsync(string semesterName, DateTime semesterStart, DateTime semesterEnd)
        {
            int result = 0;
            try
            {
                //basic validation
                if (string.IsNullOrEmpty(semesterName))
                    throw new Exception("Valid name required");

                result = await conn.InsertAsync(new Semester
                {
                    SemesterName = semesterName,
                    SemesterStart = semesterStart,
                    SemesterEnd = semesterEnd
                });

                StatusMessage = result.ToString() + " record(s) added [Name: " + semesterName + "]";
            }
            catch (Exception ex)
            {
                StatusMessage = "Failed to add data. " + ex.Message.ToString();
            }
        }

        public async Task UpdateSemesterAsync(int semesterId, string semesterName, DateTime semesterStart, DateTime semesterEnd)
        {
            int result = 0;
            try
            {
                result = await conn.UpdateAsync(new Semester
                {
                    SemesterId = semesterId,
                    SemesterName = semesterName,
                    SemesterStart = semesterStart,
                    SemesterEnd = semesterEnd
                });
                StatusMessage = result.ToString() + " record(s) added [Name: "+ semesterName + "]";
            }
            catch (Exception ex)
            {
                StatusMessage = "Failed to update data. " + ex.Message.ToString();
            }
        }

        public async Task<List<Semester>> GetAllSemestersAsync()
        {
            try
            {
                return await conn.Table<Semester>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = "Failed to retreive Data. " + ex.Message.ToString();
            }
            return new List<Semester>();
        }

        public async Task RemoveAllSemesterData()
        {
            try
            {
                int result;
                result = await conn.DeleteAllAsync<Semester>();
                StatusMessage = "Semesters successfully deleted";
            }
            catch(Exception ex)
            {
                StatusMessage = "Failed to delete data. " + ex.Message.ToString();
            }
        }
     }
}
