using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WGUMobileProject.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGUMobileProject.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SemesterView : ContentPage
	{
        public List<Course> semesterCourses;
        public int initialNumOfCourses;
        public bool maxCoursesNotReached { get; set; } = true;

        public SemesterView(string term)
        {
            try
            {
                semesterCourses = App.DatabaseData.allCourses.Where(i => i.SemesterId == App.SelectedSemester.SemesterId).ToList();
                initialNumOfCourses = semesterCourses.Count();
            }
            catch
            {
                DisplayAlert("Warning", "No Courses Found in Semester", "Ok");
            }
                //Create the Stack Layout for the whole page
            StackLayout fullPageStack = new StackLayout();
            fullPageStack.BackgroundColor = Color.Orange;

            //Add Header to fullPageStack
            fullPageStack.Children.Add(createWguHeader(term));

            //Add Scroll Stack to Full Page
            fullPageStack.Children.Add(createScrollingStack());

            //Create a GridView for the bottom buttons, and add that to the  grid
            Grid grid = createWGUFooter();
            fullPageStack.Children.Add(grid);

            //Set the main Stack as the content for the page
            Content = fullPageStack;

            //Add Padding to not interfere with the clock
            //Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);
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
            };
            backImg.GestureRecognizers.Add(backPageEventHandler);
            var editTappedEventHandler = new TapGestureRecognizer();
            
            Label wguMainHeaderTitle = new Label
            {
                Text = input + " Overview - WGU",
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            StackLayout header = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
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
            foreach(var course in semesterCourses)
            {
                if(semesterCourses.Count() < 7)
                {
                    var termLayout = createCourse(course.CourseName, course.StartDate, course.EndDate);
                    scrollingPageStack.Children.Add(termLayout);
                }
                
            }

            //Add the Add Semester Button to Bottom of scrollingPageStack
            Button addCourseButton = new Button
            {
                Text = "+ Add a Course",
                BackgroundColor = Color.Green,
                TextColor = Color.White
            };
            addCourseButton.SetDynamicResource(Button.IsVisibleProperty, "View Add Course Button");
            addCourseButton.Clicked += async (sender, args) =>
            {
                if (semesterCourses.Count() < 6)
                {
                    App.SelectedCourse = new Course
                    {
                        CourseName = "New Course",
                        CourseProgress = App.StringOpts.PickerPlannedToTake,
                        CourseDetails = "Course Details Go Here",
                        StartDate = App.SelectedSemester.SemesterStart.AddDays(1),
                        EndDate = App.SelectedSemester.SemesterEnd,
                        InstructorEmail = "Email",
                        InstructorName = "Instructor Name",
                        InstructorPhone = "",
                        NotifyOnOff = false,
                        OAName = "Objective Assessment Name",
                        PAName = "Performance Assessment Name",
                        ObjectiveAssessmentAdded = false,
                        PerformanceAssessmentAdded = false,
                        OptionalNotes = "Optional",
                        ScheduledOAAssessmentTime = App.SelectedSemester.SemesterEnd,
                        ScheduledPAAssessmentTime = App.SelectedSemester.SemesterEnd,
                        SemesterId = App.SelectedSemester.SemesterId
                    };
                    //Save the new Course in the Database,
                    await App.DatabaseData.AddNewCourseAsync(App.SelectedCourse.CourseName, App.SelectedCourse.CourseProgress, App.SelectedCourse.CourseDetails
                        , App.SelectedCourse.NotifyOnOff, App.SelectedCourse.OptionalNotes, App.SelectedCourse.InstructorName, App.SelectedCourse.SemesterId
                        , App.SelectedCourse.InstructorEmail, App.SelectedCourse.InstructorPhone, App.SelectedCourse.PerformanceAssessmentAdded, App.SelectedCourse.ObjectiveAssessmentAdded
                        , App.SelectedCourse.PAName, App.SelectedCourse.OAName, App.SelectedCourse.StartDate, App.SelectedCourse.EndDate
                        , App.SelectedCourse.ScheduledOAAssessmentTime, App.SelectedCourse.ScheduledPAAssessmentTime);
                    //Retreive the list of Data again
                    App.DatabaseData.allCourses = await App.DatabaseData.GetAllCoursesAsync();
                    //Close the current page, and open it up again.
                    App.IsNewCourse = true;
                    await Navigation.PopModalAsync();   
                //await Navigation.PushModalAsync(new CourseView("New Course"));
                }
                else
                {
                    await DisplayAlert("Warning", "Only 6 courses are allowed per semester", "Ok");
                }
            };
            scrollingPageStack.Children.Add(addCourseButton);
            scrollingPageStack.Children.Add(createSaveButton());
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
        /// Creates A Term Object and returns it to add to the page
        /// </summary>
        /// <param name="courseName"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public StackLayout createCourse(string courseName, DateTime startDate, DateTime endDate)
        {
            var courseSelected = App.DatabaseData.allCourses.Where(i => i.CourseName == courseName).FirstOrDefault();
            // Create a tapGestureRecognizer for when the edit Icon has been selected
            var editTappedEventHandler = new TapGestureRecognizer();
            editTappedEventHandler.Tapped += async (sender, e) => {
                    var result = await DisplayAlert(courseName + " Clicked", "Would you like to edit or change Semester for " + courseName + "?", "Edit", "Change");
                    if (result == true)
                    {
                        App.SelectedCourse = courseSelected;
                        App.SelectedSemester = App.DatabaseData.allSemesters.Where(i => i.SemesterId == courseSelected.SemesterId).FirstOrDefault();
                        await Navigation.PushModalAsync(new EditTextView(courseSelected.CourseName, App.StringOpts.TypeCourseName));
                    }
                    else
                    {
                        App.SelectedCourse = courseSelected;
                        App.SelectedSemester = App.DatabaseData.allSemesters.Where(i => i.SemesterId == courseSelected.SemesterId).FirstOrDefault();
                        await Navigation.PushModalAsync(new EditTextView(courseSelected.CourseName, App.StringOpts.TypeChangeSemester));
                    }
            };

            var courseSelectedEventHandler = new TapGestureRecognizer();
            courseSelectedEventHandler.Tapped += (sender, e) =>
            {
                if (App.DatabaseData.allCourses.Count() != 0)
                {
                    App.SelectedCourse = App.DatabaseData.allCourses.Where(i => i.CourseId == courseSelected.CourseId).FirstOrDefault();
                    Navigation.PushModalAsync(new CourseView(courseName));
                }
                else
                    DisplayAlert("Error", "Could not find selected course", "Cancel");
            };

            //Date Pickers
            var startDatePicker = new DatePicker
            {
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(DatePicker)),
                Date = startDate
            };
            var endDatePicker = new DatePicker
            {
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(DatePicker)),
                Date = endDate
            };
            //Date Picker Event handlers
            startDatePicker.DateSelected += (sender, args) =>
            {
                if (startDatePicker.Date < endDatePicker.Date)
                {
                    courseSelected.StartDate = startDatePicker.Date;
                    semesterCourses.Where(i => i.CourseId == courseSelected.CourseId)
                    .FirstOrDefault().StartDate = courseSelected.StartDate;
                    Resources["SemesterViewSaveVisible"] = true;
                    startDatePicker.BackgroundColor = Color.CornflowerBlue;
                    endDatePicker.BackgroundColor = Color.CornflowerBlue;
                }
                else
                {
                    Resources["SemesterViewSaveVisible"] = false;
                    startDatePicker.BackgroundColor = Color.Red;
                    endDatePicker.BackgroundColor = Color.Red;
                }
            };
            endDatePicker.DateSelected += (sender, args) =>
            {
                if (endDatePicker.Date > startDatePicker.Date)
                {
                    courseSelected.EndDate = endDatePicker.Date;
                    semesterCourses.Where(i => i.CourseId == courseSelected.CourseId)
                     .FirstOrDefault().EndDate = courseSelected.EndDate;
                    Resources["SemesterViewSaveVisible"] = true;
                    startDatePicker.BackgroundColor = Color.CornflowerBlue;
                    endDatePicker.BackgroundColor = Color.CornflowerBlue;
                }
                else
                {
                    Resources["SemesterViewSaveVisible"] = false;
                    endDatePicker.BackgroundColor = Color.Red;
                    startDatePicker.BackgroundColor = Color.Red;
                }
            };

            //Get The Image to display, and add Event Handler
            Image editImg = new Image
            {
                Source = ImageSource.FromFile("EditIcon.png")
            };
            editImg.GestureRecognizers.Add(editTappedEventHandler);

            var courseNameLabel = new Label
            {
                Text = courseName
            };
            courseNameLabel.GestureRecognizers.Add(courseSelectedEventHandler);
            View NewCourseView()
            {
                return new Frame
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.CornflowerBlue,
                    Content = new StackLayout
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Children =
                        {
                            new StackLayout
                            {
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                Orientation = StackOrientation.Horizontal,
                                Children =
                                {
                                    editImg,
                                    courseNameLabel
                                }
                            },
                            new StackLayout
                            {
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                Orientation = StackOrientation.Horizontal,
                                Children =
                                {
                                    startDatePicker,
                                    endDatePicker
                                },

                            }
                        }
                    }
                };
            };
            var newFrame = NewCourseView();

            Device.StartTimer(TimeSpan.FromSeconds(1),
            () =>
            {
                try
                {
                    courseNameLabel.Text = App.DatabaseData.allCourses.Where(i => i.CourseId == courseSelected.CourseId).FirstOrDefault().CourseName;
                    
                    //If the number of courses is equal to 6, make the add course button disappear
                    if(semesterCourses.Count() == 6)
                    {
                        maxCoursesNotReached = false;
                        Resources["View Add Course Button"] = maxCoursesNotReached;
                    }
                    var stack = Navigation.ModalStack;
                    Type LastPageType;
                    if (Navigation.ModalStack.Count() > 0)
                    {
                        LastPageType = stack.Last().GetType();


                        //Monitor when the number of courses change. If it changes, then close the semester page to open it up again.
                        string lastPageType = LastPageType.ToString();
                        if (lastPageType.Trim() != App.StringOpts.EditTextViewTypeS.Trim())
                        {
                            var newNumOfCourses = App.DatabaseData.allCourses.Where(i => i.SemesterId == App.SelectedSemester.SemesterId).Count();
                            if (initialNumOfCourses != newNumOfCourses)
                            {
                                initialNumOfCourses = newNumOfCourses;
                                Navigation.PopModalAsync();
                            }
                        }
                    }
                    else
                    {
                        //Navigation.PushModalAsync(new DegreeOverviewView());
                    }
                    return true;
                }
                catch(Exception ex)
                {
                    DisplayAlert("Warning", "Something went wrong" + ex.InnerException.ToString() , "Ok");
                    return false;
                }
            });

           

            //Add the image and the Term Frame to the Stack Layout, and return to the sender
            StackLayout termLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children =
                    {
                        
                        newFrame
                    }
            };
            return termLayout;
        }


        /// <summary>
        /// Event Handler to save Data to the Database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public async void saveButtonEvent(object sender, EventArgs args)
        {
            try
            {
                //Save Course Dates
                foreach(var course in semesterCourses)
                {
                    await App.DatabaseData.UpdateCourseAsync(course.CourseId, course.CourseName, course.CourseProgress
                    , course.CourseDetails, course.NotifyOnOff, course.OptionalNotes
                    , course.InstructorName, course.SemesterId, course.InstructorEmail, course.InstructorPhone
                    , course.PerformanceAssessmentAdded, course.ObjectiveAssessmentAdded, course.PAName
                    , course.OAName, course.StartDate, course.EndDate, course.ScheduledPAAssessmentTime, course.ScheduledOAAssessmentTime);

                }
                //Save Semester Information
                await App.DatabaseData.UpdateSemesterAsync(App.SelectedSemester.SemesterId,App.SelectedSemester.SemesterName
                    ,App.SelectedSemester.SemesterStart, App.SelectedSemester.SemesterEnd);
                //Notify User it has been saved
                await DisplayAlert("Success", "Semester Successfully Saved", "Ok");
            }
            catch (Exception ee)
            {
                await DisplayAlert("Operation Failed", "Failed to save information. /r/n"
                    + ee.InnerException.ToString(), "Ok");
            }
        }

        /// <summary>
        /// Creates the Async Save Button so it can save to the Database
        /// </summary>
        /// <returns></returns>
        public Button createSaveButton()
        {
            var saveButtonClicked = new TapGestureRecognizer();

            var saveButton = new Button
            {
                Text = "Save",
                TextColor = Color.White,
                BackgroundColor = Color.Green
            };
            saveButton.Clicked += (sender, e) =>
            {
                saveButtonEvent(sender, e);
            };
            saveButton.SetDynamicResource(Button.IsVisibleProperty, "SemesterViewSaveVisible");
            Resources["SemesterViewSaveVisible"] = false;
            //saveButton.GestureRecognizers.Add(saveButtonClicked);
            return saveButton;
        }
    }
}