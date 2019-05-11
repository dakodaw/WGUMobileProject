using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.LocalNotifications;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGUMobileProject.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationView : ContentPage
    {
        public NotificationView()
        {
            //Create the Stack Layout for the whole page
            StackLayout fullPageStack = new StackLayout();
            fullPageStack.BackgroundColor = Color.Orange;

            //Add Header to fullPageStack
            fullPageStack.Children.Add(createWguHeader("Notifications "));

            //Go through a loop to create Notification sections that set notifications by course and Semester
            fullPageStack.Children.Add(createScrollingStack());

            //Create a GridView for the bottom buttons, and add that to the  grid
            Grid grid = createWGUFooter();
            fullPageStack.Children.Add(grid);

            //Set the main Stack as the content for the page
            Content = fullPageStack;
        }


        /// <summary>
        /// Creates the WGU Header with the BackButton
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public StackLayout createWguHeader(string input)
        {
            //Create Header, and Add it to the main stack
            Image backImg = new Image
            {
                Source = ImageSource.FromFile("BackArrowSmall.png")
            };
            var backPageEventHandler = new TapGestureRecognizer();
            backPageEventHandler.Tapped += (sender, e) =>
            {
                Navigation.PopModalAsync();
                App.SelectedCourse = null;
            };
            backImg.GestureRecognizers.Add(backPageEventHandler);
            Label wguMainHeaderTitle = new Label
            {
                Text = input + " Overview - WGU",
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            StackLayout header = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    backImg,
                    wguMainHeaderTitle
                }
            };
            return header;
        }

        /// <summary>
        /// Creates the Scrolling Stack: Not Complete
        /// </summary>
        /// <returns></returns>
        public ScrollView createScrollingStack()
        {
            //USE THIS LATER TO POPULATE FROM SQLITE
            //Testing Views for Terms with Information
            StackLayout scrollingPageStack = new StackLayout
            {
                Padding = new Thickness(5),
                Spacing = 10
            };

            //Add a Notification for each course.
            foreach (var course in App.DatabaseData.allCourses)
            {
                //Check if the notifications are subscribed for a given course
                if(course.NotifyOnOff == true)
                {
                    //If a course has been subscribed to, create a notification alerts and sections
                    //for each one, create an ID comprised of Semester, courseId, and number of 
                    //notification for each course

                    //Course Start
                    string courseStartId = course.SemesterId.ToString() + course.CourseId.ToString() + "1";
                    scrollingPageStack.Children.Add(createScheduledNotifications(course.CourseName  
                        + " Starting", course.CourseName + " Start Date Approaching", Convert.ToInt32(courseStartId)
                        , course.StartDate, App.StringOpts.Start));
                    //If Objective Assessment is set, set a notification for it
                    if (course.ObjectiveAssessmentAdded == true)
                    {
                        string courseOAId = course.SemesterId.ToString() + course.CourseId.ToString() + "2";
                        scrollingPageStack.Children.Add(createScheduledNotifications(course.OAName
                            + " Scheduled", course.CourseName + " Test Date Approaching", Convert.ToInt32(courseOAId)
                            , course.ScheduledOAAssessmentTime, App.StringOpts.Test));
                    }
                    //If Performace Assessment is set, set a notification for it
                    if (course.PerformanceAssessmentAdded == true)
                    {
                        string coursePAId = course.SemesterId.ToString() + course.CourseId.ToString() + "3";
                        scrollingPageStack.Children.Add(createScheduledNotifications(course.PAName
                            + " Scheduled", course.PAName + " Test Date Approaching", Convert.ToInt32(coursePAId)
                            , course.ScheduledPAAssessmentTime, App.StringOpts.Test));
                    }
                    //Course End
                    string courseEndId = course.SemesterId.ToString() + course.CourseId.ToString() + "1";
                    scrollingPageStack.Children.Add(createScheduledNotifications(course.CourseName
                        + " Ending", course.CourseName + " End Date Approaching", Convert.ToInt32(courseEndId)
                        , course.EndDate, App.StringOpts.End));
                }
            }

            //Add the scrollable part to a scroll view, then add that to the fullPageStack
            ScrollView scrollView = new ScrollView
            {
                BackgroundColor = Color.LightGray,
                Content = scrollingPageStack,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(5, 0),
            };
            return scrollView;
        }

        /// <summary>
        /// Creates the Footer for the App
        /// </summary>
        /// <returns></returns>
        public Grid createWGUFooter()
        {
            //Create Buttons
            var degreeOverviewButton = new Button
            {
                BackgroundColor = Color.Blue,
                Text = "Degree",
                TextColor = Color.White,
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button))
            };
            degreeOverviewButton.Clicked += (sender, args) =>
            {
                Navigation.PushModalAsync(new DegreeOverviewView());
            };

            var notificationsButton = new Button
            {
                BackgroundColor = Color.Blue,
                Text = "Notifications",
                TextColor = Color.White,
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button))
            };
            notificationsButton.Clicked += (sender, args) =>
            {
                Navigation.PushModalAsync(new NotificationView());
            };

            //Create a grid with two equally divided portions for degree overview and notifications
            var grid = new Grid();
            grid.BackgroundColor = Color.Orange;
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            //Add buttons to grid
            grid.Children.Add(degreeOverviewButton, 0, 0);
            grid.Children.Add(notificationsButton, 1, 0);

            return grid;
        }

        /// <summary>
        /// Creates a stacklayout for Scheduling Notifications
        /// </summary>
        /// <param name="title"></param>
        /// <param name="notes"></param>
        /// <param name="id"></param>
        /// <param name="dateOccurring"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public StackLayout createScheduledNotifications(string title, string notes, int id, DateTime dateOccurring, string type)
        {
            //Create a timeSpan to subract for a notification 1 day previous
            var dayPreviousAlertTime = dateOccurring.AddDays(-1);
            //Create another timespan to subract for a notification 15 minutes previous
            var sameDayAlertTime = dateOccurring.AddMinutes(-15);

            //Create the alerts for both one day previous, and 15 minutes previous
            CrossLocalNotifications.Current.Show(title, notes, id, dayPreviousAlertTime);
            CrossLocalNotifications.Current.Show(title, notes, id, sameDayAlertTime);

            string scheduleFormattedString = "";
            if (type == App.StringOpts.Start)
                scheduleFormattedString = "Scheduled to begin " + dateOccurring.Date.ToString("MM/dd/yyyy");
            else if (type == App.StringOpts.Test)
                scheduleFormattedString = "Scheduled for: " + dateOccurring.Date.ToString("MM/dd/yyyy")
                    + " at " + dateOccurring.ToString("HH:mm");
            else
                scheduleFormattedString = "Scheduled to end: " + dateOccurring.Date.ToString("MM/dd/yyyy");
            var timeScheduledLabel = new Label
            {
                Text = scheduleFormattedString,
                TextColor = Color.White
            };
            try
            {
                View courseInstructorView()
                {
                    return new Frame
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        BackgroundColor = Color.CornflowerBlue,
                        Content = new StackLayout
                        {
                            Children =
                            {
                                new Label
                                {
                                    Text = title,
                                    TextColor = Color.White,
                                    FontAttributes = FontAttributes.Bold
                                },
                                timeScheduledLabel,
                                new Label
                                {
                                    Text = id + " - " + notes,
                                    TextColor = Color.White
                                }
                            }
                        }
                    };
                };
                var newFrame = courseInstructorView();

                return new StackLayout
                {
                    BackgroundColor = Color.CornflowerBlue,
                    Children =
                    {
                        newFrame
                    }
                };
            }
            catch(Exception ex)
            {
                return new StackLayout
                {
                    Children =
                    {
                        new Label
                        {
                            Text = "Error Occurred - " + ex.InnerException.ToString()
                        }
                    }
                };
            }
        }
    }
}