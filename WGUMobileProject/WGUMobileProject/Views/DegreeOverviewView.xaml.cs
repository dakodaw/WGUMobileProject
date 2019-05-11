using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using WGUMobileProject.Model;

namespace WGUMobileProject.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DegreeOverviewView : ContentPage
	{
        public int initialNumOfClassesTotal;

        List<Semester> tempSemesters;
        public DegreeOverviewView()
        {
            var eventArgs = new EventArgs();
            getDataButton(this, eventArgs);

            //Load list of Semesters to a temp variable
            tempSemesters = App.DatabaseData.allSemesters;

            initialNumOfClassesTotal = App.DatabaseData.allCourses.Count();
            //Create the Stack Layout for the whole page
            StackLayout fullPageStack = new StackLayout();
            fullPageStack.BackgroundColor = Color.Orange;

            //Create Header, and Add it to the main stack
            Label wguMainHeader = new Label
            {
                Text = "Degree Overview - WGU",
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            //Add Header to fullPageStack
            fullPageStack.Children.Add(wguMainHeader);

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

        public async void getDataButton(object sender, EventArgs args)
        {
            try
            {
                App.DatabaseData.allCourses = await App.DatabaseData.GetAllCoursesAsync();
                App.DatabaseData.allSemesters = await App.DatabaseData.GetAllSemestersAsync();
            }
            catch
            {
                await DisplayAlert("Data Retreival Failed", "Failed to retreive Courses", "Cancel");
            }
            //Navigation.PushModalAsync(new CourseView(courseName));
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
            try
            {
                foreach (var semester in App.DatabaseData.allSemesters)
                {
                    App.SelectedSemester = semester;
                    var termLayout = createTerm(semester.SemesterName, semester.SemesterStart, semester.SemesterEnd);
                    scrollingPageStack.Children.Add(termLayout);
                }
            }
            catch(Exception ex)
            {
                DisplayAlert("Warning", ex.InnerException.ToString(), "Ok");
            }
            //Add the Add Semester Button to Bottom of scrollingPageStack
            Button addSemesterButton = new Button
            {
                Text = "+ Add a Semester",
                BackgroundColor = Color.Green,
                TextColor = Color.White
            };
            // Event Handler for the Add Semester Button
            addSemesterButton.Clicked += async (sender, args) =>
            {
                //App.IsNewSemester = true;
                App.SelectedSemester = new Semester
                {
                    SemesterName = "New Semester",
                    SemesterStart = DateTime.Now.AddDays(1),
                    SemesterEnd = DateTime.Now.AddMonths(6)
                };
                //Save the new Course in the Database,
                await App.DatabaseData.AddNewSemesterAsync("New Semester", DateTime.Now.AddDays(1), DateTime.Now.AddMonths(6));

                //Retreive the list of Data again
                await App.DatabaseData.GetAllSemestersAsync();

                //Set a bool equal to true to show there is a new semester added, and this page will need to be reopened after close
                App.IsNewSemester = true;

                //Close the current page, and open it up again.
                await Navigation.PopModalAsync();
                //await Navigation.PushModalAsync(new SemesterView("New Semester"));
            };
            //Add Button to scrolling stack
            scrollingPageStack.Children.Add(addSemesterButton);

            //Add A Save Button to save the start and end dates for each semester
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
        /// <param name="termName"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public StackLayout createTerm(string termName, DateTime startDate, DateTime endDate)
        {
            //Find the selected Semester
            var semesterSelected = App.DatabaseData.allSemesters.Where(i => i.SemesterName == termName).FirstOrDefault();
            var semesterSelectedId = semesterSelected.SemesterId;
            //Set an Initial Value to the App Selected Semester Value to use for the label
            if (App.SelectedSemester == null)
                App.SelectedSemester = semesterSelected;

            // Create a tapGestureRecognizer for when the edit Icon has been selected
            var editTappedEventHandler = new TapGestureRecognizer();

            //Create an Event Handler for the Edit image
            editTappedEventHandler.Tapped += async (sender, e) => {
                App.SelectedSemester = semesterSelected;

                await DisplayAlert(termName + " Clicked", termName + " has been selected", "Ok");
                await Navigation.PushModalAsync(new EditTextView(semesterSelected.SemesterName, App.StringOpts.TypeSemesterName));
            };

            //Create Event Handler for Tapping Semester Label
            var termNameTappedEventHandler = new TapGestureRecognizer();
            termNameTappedEventHandler.Tapped += (sender, e) =>
            {
                //if(App.SelectedSemester == null)
                App.SelectedSemester = semesterSelected;
                //App.DatabaseData.allSemesters.Where(i => i.SemesterName == semesterSelected.SemesterName).FirstOrDefault();
                Navigation.PushModalAsync(new SemesterView(App.SelectedSemester.SemesterName));
            };

            //Create DatePickers
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

            //Event Handlers for the DatePickers
            startDatePicker.DateSelected += (sender, args) =>
            {
                if (startDatePicker.Date < endDatePicker.Date)
                {
                    semesterSelected.SemesterStart = startDatePicker.Date;
                    tempSemesters.Where(i => i.SemesterId == semesterSelected.SemesterId)
                    .FirstOrDefault().SemesterStart = semesterSelected.SemesterStart;
                    Resources["DegreeViewButtonVisible"] = true;
                    startDatePicker.BackgroundColor = Color.CornflowerBlue;
                    endDatePicker.BackgroundColor = Color.CornflowerBlue;
                }
                else
                {
                    Resources["DegreeViewButtonVisible"] = false;
                    startDatePicker.BackgroundColor = Color.Red;
                    endDatePicker.BackgroundColor = Color.Red;
                }
            };
            endDatePicker.DateSelected += (sender, args) =>
            {
                if (endDatePicker.Date > startDatePicker.Date)
                {
                    semesterSelected.SemesterEnd = endDatePicker.Date;
                    tempSemesters.Where(i => i.SemesterId == semesterSelected.SemesterId)
                    .FirstOrDefault().SemesterEnd = semesterSelected.SemesterEnd;
                    Resources["DegreeViewButtonVisible"] = true;
                    startDatePicker.BackgroundColor = Color.CornflowerBlue;
                    endDatePicker.BackgroundColor = Color.CornflowerBlue;
                }
                else
                {
                    Resources["DegreeViewButtonVisible"] = false;
                    endDatePicker.BackgroundColor = Color.Red;
                    startDatePicker.BackgroundColor = Color.Red;
                }
            };

            //Create the Label to use the  termname tapped handler
            var termNameLabel = new Label
            {
                Text = App.SelectedSemester.SemesterName
            };
            termNameLabel.GestureRecognizers.Add(termNameTappedEventHandler);

            //Create the View to hold the frame with Simple Course Information
            View NewTermView()
            {
                //Get The Image to display, and add Event Handler
                Image editImg = new Image
                {
                    Source = ImageSource.FromFile("EditIcon.png")
                };
                editImg.GestureRecognizers.Add(editTappedEventHandler);

                //Create a Frame to hold An Edit Symbol, The name of the semester
                //, and the start and end date for each semester
                return new Frame
                {
                    BackgroundColor = Color.CornflowerBlue,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
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
                                    termNameLabel
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
                                }

                            }
                        }
                    }
                };
            };
            var newFrame = NewTermView();


            //Add the Term Frame to the Stack Layout, and return to the sender
            StackLayout termLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children =
                    {
                        newFrame
                    }
            };

            Device.StartTimer(TimeSpan.FromSeconds(1),
            () =>
            {
                try
                {
                    //If the App New Course Variable is set to true, then reopen the semester page
                    //if(App.IsNewCourse == true)
                    //{
                    //    Navigation.PushModalAsync
                    //}

                    //Monitor when the number of courses change. If it changes, then close the semester page to open it up again.
                    var newNumOfCourses = App.DatabaseData.allCourses.Count();
                    if (initialNumOfClassesTotal != newNumOfCourses)
                    {
                        Navigation.PushModalAsync(new SemesterView(App.SelectedSemester.SemesterName));
                        termName = semesterSelected.SemesterName;
                        initialNumOfClassesTotal = newNumOfCourses;

                        //Set the text of the given Label to the edited text of the selected Label
                        if (App.SelectedSemester.SemesterId == semesterSelected.SemesterId)
                            termNameLabel.Text = App.SelectedSemester.SemesterName;
                    }
                    else
                    {
                        //Set the text of the given Label to the edited text of the selected Label
                        if(App.SelectedSemester.SemesterId == semesterSelected.SemesterId)
                            termNameLabel.Text = App.SelectedSemester.SemesterName;
                    }
                    return true;
                }
                catch(Exception ex)
                {
                    DisplayAlert("Error", "An Error has occurred", "Ok");
                    return false;
                }
            });
            
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
                App.DatabaseData.allSemesters = tempSemesters;
                //Save Course Dates
                foreach (var semester in App.DatabaseData.allSemesters)
                {
                    await App.DatabaseData.UpdateSemesterAsync(semester.SemesterId
                        ,semester.SemesterName, semester.SemesterStart, semester.SemesterEnd);

                }
                //Save Semester Information
                await App.DatabaseData.UpdateSemesterAsync(App.SelectedSemester.SemesterId, App.SelectedSemester.SemesterName
                    , App.SelectedSemester.SemesterStart, App.SelectedSemester.SemesterEnd);
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

            saveButton.SetDynamicResource(Button.IsVisibleProperty, "DegreeViewButtonVisible");
            Resources["DegreeViewButtonVisible"] = false;
            //saveButton.GestureRecognizers.Add(saveButtonClicked);
            return saveButton;
        }
    }
}