using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGUMobileProject.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingView : ContentPage
    {
        public bool datareturnedbool { get; set; } = false;
        public bool stillLoading { get; set; }
        public LoadingView()
        {
            InitializeComponent();

            var mainStack = new StackLayout
            {
                BackgroundColor = Color.CornflowerBlue
            };

            mainStack.Children.Add(createMainStack());
            Content = mainStack;
        }

        /// <summary>
        /// Creates a Stack Layout for the buttons when loading
        /// </summary>
        /// <returns></returns>
        public StackLayout createMainStack()
        {
            retreiveInfoAgain();

            var loadingLabel = new Label
            {
                Text = "Welcome to my WGU Mobile App",
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Color.White,
                BackgroundColor = Color.CornflowerBlue
            };
            loadingLabel.SetDynamicResource(Label.TextProperty, "loadingKey");
            //Create Buttons
            var nextPageButton = new Button
            {
                Text = "Continue",
                BackgroundColor = Color.Green
            };
            var clearDataButton = new Button
            {
                Text = "Clear Previous Data",
                BackgroundColor = Color.Red
            };
            var loadExampleDataButton = new Button
            {
                Text = "Load Example Data",
                BackgroundColor = Color.Orange
            };

            //Visibility Setters for Buttons
            nextPageButton.SetDynamicResource(Button.IsVisibleProperty, "LoadingNextPageButtonVisible");
            clearDataButton.SetDynamicResource(IsVisibleProperty, "LoadingClearDataButtonVisible");
            loadExampleDataButton.SetDynamicResource(IsVisibleProperty, "LoadingLoadExampleButtonVisible");

            //Button Event Handlers
            nextPageButton.Clicked += (sender, args) =>
            {
                EventArgs eventArgs = new EventArgs();
                nextPageButtonEvent(this, eventArgs);
            };
            clearDataButton.Clicked += (sender, args) =>
            {
                EventArgs eventArgs = new EventArgs();
                clearDataButtonEvent(this, eventArgs);
            };
            loadExampleDataButton.Clicked += (sender, args) =>
            {
                EventArgs eventArgs = new EventArgs();
                loadExampleDataButtonEvent(this, eventArgs);
            };


            retreiveInfoAgain();

            if (App.DatabaseData.allSemesters.Count() > 0)
            {
                Resources["LoadingNextPageButtonVisible"] = true;
                Resources["LoadingClearDataButtonVisible"] = true;
                Resources["LoadingLoadExampleButtonVisible"] = false;
            }
            else
            {
                Resources["LoadingNextPageButtonVisible"] = false;
                Resources["LoadingClearDataButtonVisible"] = false;
                Resources["LoadingLoadExampleButtonVisible"] = true;
            }

            if (App.IsNewSemester == true)
            {
                retreiveInfoAgain();
                App.IsNewSemester = false;
                var eventArgs = new EventArgs();
                

                nextPageButtonEvent(this, eventArgs);
            }
            return new StackLayout
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                BackgroundColor = Color.CornflowerBlue,
                Children =
                {
                    loadingLabel,
                    nextPageButton,
                    clearDataButton,
                    loadExampleDataButton
                }
            };
        }


        /// <summary>
        /// Creates an event for when the next page buttonn is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public async void nextPageButtonEvent(Object sender, EventArgs args)
        {
            if (App.DatabaseData.allSemesters.Count() > 0)
            {
                //Set the selected Semester
                App.SelectedSemester = App.DatabaseData.allSemesters.Last();
                await Navigation.PushModalAsync(new NotificationView());
                await Navigation.PushModalAsync(new DegreeOverviewView());
            }
            else
            {
                retreiveInfoAgain();
                await DisplayAlert("Loading", "Please wait a moment, Semesters are still Loading", "Ok");
            }
        }

        /// <summary>
        /// Creates an event for when the clear data button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public async void clearDataButtonEvent(Object sender, EventArgs args)
        {
            var checkResult = await DisplayAlert("Are You Sure?", "Do you really want to delete all course information?", "Yes", "No");
            if(checkResult == true)
            {
                try
                {
                    //Remove Data from Database
                    await App.DatabaseData.RemoveAllSemesterData();
                    await App.DatabaseData.RemoveAllCourseData();

                    //Remove Data from Lists
                    App.DatabaseData.allSemesters.Clear();
                    App.DatabaseData.allCourses.Clear();

                    //Change visibility of buttons
                    Resources["LoadingNextPageButtonVisible"] = false;
                    Resources["LoadingClearDataButtonVisible"] = false;
                    Resources["LoadingLoadExampleButtonVisible"] = true;
                }
                catch
                {
                    await DisplayAlert("Error Occurred", App.DatabaseData.StatusMessage, "Ok");
                }
            }
        }

        /// <summary>
        /// Creates an event for when the load example button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public async void loadExampleDataButtonEvent(Object sender, EventArgs args)
        {
            //Set DataBase Data, and load list for An example Semester
            await App.DatabaseData.AddNewSemesterAsync("Semester 1", DateTime.Now.AddDays(1)
                , DateTime.Now.AddMonths(6));
            App.DatabaseData.allSemesters = await App.DatabaseData.GetAllSemestersAsync();

            //Set DataBase Data, and load list for An example Course
            await App.DatabaseData.AddNewCourseAsync("Course 1 - C111"
                , "In Progress", "This is the first of all of your development Classes", true, ""
                , "Dakoda Willden", App.DatabaseData.allSemesters.FirstOrDefault().SemesterId
                , "dwilld1@wgu.edu", "8013729899", true, true, "P112", "O212"
                , DateTime.Now.AddDays(1), DateTime.Now.AddMonths(1), DateTime.Now.AddDays(14), DateTime.Now.AddDays(28));

            await DisplayAlert("Success", "Database Data successfully Added", "Ok");
            
            App.DatabaseData.allCourses = await App.DatabaseData.GetAllCoursesAsync();

            //Change Button Visibility
            Resources["LoadingNextPageButtonVisible"] = true;
            Resources["LoadingClearDataButtonVisible"] = true;
            Resources["LoadingLoadExampleButtonVisible"] = false;

            //Show Success of Data Retreived
            await DisplayAlert("Success", "Database Data successfully Retreived", "Ok");
        }

        /// <summary>
        /// Attempts to repopulate the data in the App List of Courses and Semester
        /// </summary>
        public async void retreiveInfoAgain()
        {
            App.DatabaseData.allSemesters = await App.DatabaseData.GetAllSemestersAsync();
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                try
                {
                    if(App.IsNewSemester == true)
                    {
                        var mainStack = new StackLayout
                        {
                            BackgroundColor = Color.CornflowerBlue
                        };
                        mainStack.Children.Add(createMainStack());
                        Content = mainStack;
                    }
                    //Keep the Timer going
                    return true;
                }
                catch
                {
                    //Stop the Timer
                    return false;
                }
            });
        }
    }
}