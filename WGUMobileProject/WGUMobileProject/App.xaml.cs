using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using WGUMobileProject.Views;
using WGUMobileProject.Model;
using System.Collections.Generic;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace WGUMobileProject
{
    public partial class App : Application
    {
        
        public static DatabaseConnection DatabaseData { get; private set; }
        public static Course SelectedCourse { get; set; }
        public static StringOptions StringOpts { get; set; }
        public static Semester SelectedSemester { get; set; }
        public static bool IsNewCourse { get; set; } = false;
        public static bool IsNewSemester { get; set; } = false;
        public static bool NeedsRefresh { get; set; } = false;

        public App(string dbPath)
        {
            InitializeComponent();

            //Initialize lists
            StringOpts = new StringOptions();

            //Get the Course Data that has already been saved
            DatabaseData = new DatabaseConnection(dbPath);
            var args = new EventArgs();
            getDataButton(this, args);

            //Open the main page
            MainPage = new LoadingView();
        }

        /// <summary>
        /// Puts the Data into the Lists from the Database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public async void getDataButton(object sender, EventArgs args)
        {
            try
            {
                App.DatabaseData.allCourses = await App.DatabaseData.GetAllCoursesAsync();
                App.DatabaseData.allSemesters = await App.DatabaseData.GetAllSemestersAsync();
            }
            catch
            {
                throw new Exception("Failed to retreive Courses");
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
