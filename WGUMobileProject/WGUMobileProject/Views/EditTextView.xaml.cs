using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGUMobileProject.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WGUMobileProject.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditTextView : ContentPage
    {
        public Course tempCourseInfo;
        public Semester tempSemesterInfo;
        public string textSenderType;
        public int tempSemesterId = 0;
        public EditTextView(string title, string type)
        {
            if (type != App.StringOpts.TypeSemesterName)
            {
                //Select the course that matches the courseName selected
                tempCourseInfo = App.SelectedCourse;
                tempCourseInfo.SemesterId = App.SelectedSemester.SemesterId;
                if (type == App.StringOpts.TypeChangeSemester)
                {
                    tempSemesterInfo = App.SelectedSemester;
                    textSenderType = type;
                }
                else
                    textSenderType = title;
            }
            else
            {
                tempSemesterInfo = App.SelectedSemester;
                textSenderType = type;
            }

            //Create the Stack Layout for the whole page
            StackLayout fullPageStack = new StackLayout();
            fullPageStack.BackgroundColor = Color.Orange;

            //Add Header to fullPageStack
            fullPageStack.Children.Add(createWguHeader(title));
            if (type == App.StringOpts.TypeChangeSemester)
            {
                fullPageStack.Children.Add(createChangeSemesterLayout(title));
            }
            else if(type == App.StringOpts.TypeSemesterName)
            {
                fullPageStack.Children.Add(createSemesterNameSection(tempSemesterInfo.SemesterName));
            }
            else
            {
                //Add Scroll Stack to Full Page
                fullPageStack.Children.Add(createScrollingStack(title, type));
            }

            //Create a GridView for the bottom buttons, and add that to the  grid
            Grid grid = createWGUFooter();
            fullPageStack.Children.Add(grid);

            //Set the main Stack as the content for the page
            Content = fullPageStack;

            //Add Padding to not interfere with the clock
            Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);
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
                Source = ImageSource.FromFile(App.StringOpts.BackButton)
            };
            var backPageEventHandler = new TapGestureRecognizer();
            backPageEventHandler.Tapped += (sender, e) =>
            {
                Navigation.PopModalAsync();
            };
            backImg.GestureRecognizers.Add(backPageEventHandler);

            if (input == App.StringOpts.AssessmentTypeObjective)
                input = "Objective Assessment";
            else if (input == App.StringOpts.AssessmentTypePerformance)
                input = "Performance Assessment";
            Label wguMainHeaderTitle = new Label
            {
                Text = "Edit " + input,
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
        public ScrollView createScrollingStack(string title, string type)
        {
            StackLayout scrollingPageStack = new StackLayout
            {
                Padding = new Thickness(5),
                Spacing = 10
            };

            //Class Overview
            if (title == App.StringOpts.TitleCourseOverview)
                scrollingPageStack.Children.Add(createNotesSection(title, App.SelectedCourse.CourseDetails, title));
            else if (title == App.StringOpts.TitleOptionalNotes)
                scrollingPageStack.Children.Add(createNotesSection(title, App.SelectedCourse.OptionalNotes, title));
            else if (title == App.StringOpts.AssessmentTypeObjective)
                scrollingPageStack.Children.Add(createNotesSection(App.StringOpts.AssessmentTypeObjective, App.SelectedCourse.OAName != null ? App.SelectedCourse.OAName : "Enter Assessment Name Here", "Objective Assessment"));
            else if (title == App.StringOpts.AssessmentTypePerformance)
                scrollingPageStack.Children.Add(createNotesSection(App.StringOpts.AssessmentTypePerformance, App.SelectedCourse.PAName != null?App.SelectedCourse.PAName:"Enter Assessment Name Here", "Performance Assessment"));
            else
            {
                if (type == App.StringOpts.TypeCourseName)
                {
                    scrollingPageStack.Children.Add(createNotesSection(type, title, type));
                }
                else
                {
                    scrollingPageStack.Children.Add(createNotesSection(title, App.SelectedCourse.InstructorName, App.StringOpts.TypeName));
                    scrollingPageStack.Children.Add(createNotesSection(title, App.SelectedCourse.InstructorEmail, App.StringOpts.TypeEmail));
                    scrollingPageStack.Children.Add(createNotesSection(title, App.SelectedCourse.InstructorPhone, App.StringOpts.TypePhone));
                }
            }


            //Add a Save Button
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
        /// Creates A Notes Section and returns it to add to the page
        /// </summary>
        /// <param name="title"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public StackLayout createNotesSection(string title, string notes, string type)
        {
            View OptionalNotesView()
            {
                var courseNameLabel = new Label
                {
                    Text = type,
                    TextColor = Color.White
                };
                var courseNotesText = new Editor
                {
                    
                    Text = notes,
                    FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                    TextColor = Color.White
                };
                if (type == App.StringOpts.TypeName)
                {
                    courseNameLabel.Text = " Name";
                }
                else if (type == App.StringOpts.TypeEmail)
                {
                    courseNameLabel.Text = " Email";
                }
                else if (type == App.StringOpts.TypePhone)
                {
                    courseNameLabel.Text = " Phone";
                }
                else if (title == App.StringOpts.AssessmentTypeObjective)
                {
                    courseNameLabel.Text = "Edit Objective Assessment";
                }
                else if (title == App.StringOpts.AssessmentTypePerformance)
                {
                    courseNameLabel.Text = "Edit Performance Assessment";
                }
                else if(type == App.StringOpts.TypeCourseName)
                {
                    courseNameLabel.Text = type;
                    courseNotesText.Text = notes;
                }
                else if (type == App.StringOpts.TypeSemesterName)
                {
                    courseNameLabel.Text = "Edit Semester Name";
                    courseNotesText.Text = notes;
                }
                courseNotesText.Unfocused += (sender, e) =>
                {
                    if (courseNotesText.Text.Count() > 0)
                    {
                        //Class Overview
                        Resources["EditTextViewSaveButtonVisible"] = true;
                        if (title == App.StringOpts.TitleCourseOverview)
                        {
                            tempCourseInfo.CourseDetails = courseNotesText.Text;
                        }
                        else if (title == App.StringOpts.TitleOptionalNotes)
                        {
                            tempCourseInfo.OptionalNotes = courseNotesText.Text;
                        }
                        else if (type == App.StringOpts.TypeName)
                        {
                            tempCourseInfo.InstructorName = courseNotesText.Text;
                            courseNameLabel.Text = " Name";
                        }
                        else if (type == App.StringOpts.TypeEmail)
                        {
                            if(courseNotesText.Text.Contains('@') && courseNotesText.Text.Contains('.'))
                                Resources["EditTextViewSaveButtonVisible"] = true;
                            else
                                Resources["EditTextViewSaveButtonVisible"] = false;
                            tempCourseInfo.InstructorEmail = courseNotesText.Text;
                            courseNameLabel.Text = " Email";
                        }
                        else if (type == App.StringOpts.TypePhone)
                        {
                            string textToDisplay = courseNotesText.Text;
                            string textTemp = "";
                            //With each keypress, check if it is a number. If it is, add the character to the string
                            foreach(char c in textToDisplay)
                            {
                                if (char.IsDigit(c))
                                {
                                    textTemp += c;
                                }
                                else
                                {
                                    DisplayAlert("Alert", "Only Numbers allowed in this field", "Ok");
                                }
                            }
                            courseNotesText.Text = textTemp;
                            tempCourseInfo.InstructorPhone = courseNotesText.Text;
                            courseNameLabel.Text = " Phone";
                        }
                        else if (title == App.StringOpts.AssessmentTypeObjective)
                        {
                            tempCourseInfo.OAName = courseNotesText.Text;
                            tempCourseInfo.ObjectiveAssessmentAdded = true;
                            courseNameLabel.Text = "Edit Objective Assessment";
                        }
                        else if (title == App.StringOpts.AssessmentTypePerformance)
                        {
                            tempCourseInfo.PAName = courseNotesText.Text;
                            tempCourseInfo.PerformanceAssessmentAdded = true;
                            courseNameLabel.Text = "Edit Performance Assessment";
                        }
                        else if (type == App.StringOpts.TypeCourseName)
                        {
                            tempCourseInfo.CourseName = courseNotesText.Text;
                        }
                        else if (type == App.StringOpts.TypeSemesterName)
                        {
                            tempSemesterInfo.SemesterName = courseNotesText.Text;
                        }
                    }
                    else
                    {
                        Resources["EditTextViewSaveButtonVisible"] = false;
                        DisplayAlert("Alert", "Cannot enter blank text fields", "Ok");
                    }
                };
                return new Frame
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Gray,
                    Content = new StackLayout
                    {
                        Children =
                        {
                            courseNameLabel,
                            courseNotesText
                        }
                    }
                };
            };
            var newFrame = OptionalNotesView();


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
        /// Creates a Section to change the semester of a course
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public ScrollView createChangeSemesterLayout(string title)
        {
            StackLayout scrollingPageStack = new StackLayout
            {
                Padding = new Thickness(5),
                Spacing = 10
            };



            List<string> semesterPickerList = new List<string>();
            foreach (var semester in App.DatabaseData.allSemesters)
            {
                semesterPickerList.Add(semester.SemesterName);
            }
            var semesterPicker = new Picker
            {
                Title = "Please Select a Semester"
            };
            semesterPicker.ItemsSource = semesterPickerList;

            semesterPicker.SelectedIndexChanged += (sender, args) =>
            {
                try
                {
                    tempSemesterId = App.DatabaseData.allSemesters.Where(i => i.SemesterName == semesterPicker.SelectedItem.ToString()).FirstOrDefault().SemesterId;
                    //tempSemesterInfo.SemesterId = App.DatabaseData.allSemesters.Where(i => i.SemesterName == semesterPicker.SelectedItem.ToString()).FirstOrDefault().SemesterId;
                    var Semester = App.SelectedSemester;
                    Resources["EditTextViewSaveButtonVisible"] = true;
                }
                catch
                {
                    DisplayAlert("Alert", "Please Select a Semester", "Ok");
                    Resources["EditTextViewSaveButtonVisible"] = false;
                }
            };

            var semesterPickerPart = new StackLayout
            {
                Children =
                {
                    semesterPicker
                }
            };
            scrollingPageStack.Children.Add(semesterPickerPart);

            //Add a Save Button
            scrollingPageStack.Children.Add(createSaveButton());

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
        /// Creates a section to edit the semester name
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public ScrollView createSemesterNameSection(string title)
        {
            StackLayout scrollingPageStack = new StackLayout
            {
                Padding = new Thickness(5),
                Spacing = 10
            };

            scrollingPageStack.Children.Add(createNotesSection("Edit Semester Name", tempSemesterInfo.SemesterName, App.StringOpts.TypeSemesterName));
            
            //Add a Save Button
            scrollingPageStack.Children.Add(createSaveButton());

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
        /// Event Handler to save Data to the Database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public async void saveButtonEvent(object sender, EventArgs args)
        {
            try
            {
                //If it is an Assessment being added, set the assessmentadded variable equal to true
                if (textSenderType == App.StringOpts.AssessmentTypeObjective)
                {
                    tempCourseInfo.SemesterId = App.SelectedSemester.SemesterId;

                    tempCourseInfo.ObjectiveAssessmentAdded = true;
                    tempCourseInfo.ScheduledOAAssessmentTime = DateTime.Now;
                    //Save course to App Data
                    App.SelectedCourse = tempCourseInfo;

                    //Save Information to Database
                    if (App.IsNewCourse == false)
                    {
                        await App.DatabaseData.UpdateCourseAsync(App.SelectedCourse.CourseId, App.SelectedCourse.CourseName, App.SelectedCourse.CourseProgress
                             , App.SelectedCourse.CourseDetails, App.SelectedCourse.NotifyOnOff, App.SelectedCourse.OptionalNotes
                             , App.SelectedCourse.InstructorName, App.SelectedCourse.SemesterId, App.SelectedCourse.InstructorEmail, App.SelectedCourse.InstructorPhone
                             , App.SelectedCourse.PerformanceAssessmentAdded, App.SelectedCourse.ObjectiveAssessmentAdded, App.SelectedCourse.PAName
                             , App.SelectedCourse.OAName, App.SelectedCourse.StartDate, App.SelectedCourse.EndDate, App.SelectedCourse.ScheduledPAAssessmentTime, App.SelectedCourse.ScheduledOAAssessmentTime);
                        //Notify User it has been saved
                    }
                    else
                    {

                    }
                }
                else if (textSenderType == App.StringOpts.AssessmentTypePerformance || textSenderType == App.StringOpts.TypeChangeSemester)
                {
                    if (textSenderType == App.StringOpts.AssessmentTypePerformance)
                    {
                        tempCourseInfo.SemesterId = App.SelectedSemester.SemesterId;

                        tempCourseInfo.PerformanceAssessmentAdded = true;
                        tempCourseInfo.ScheduledPAAssessmentTime = DateTime.Now;
                        //Save course to App Data
                        App.SelectedCourse = tempCourseInfo;
                    }

                    //Save Information to Database
                    if (App.IsNewCourse == false)
                    {
                        //Update Selected Course
                        App.SelectedCourse.SemesterId = tempSemesterId;

                        //Update List
                        App.DatabaseData.allCourses.Where(i => i.CourseId == App.SelectedCourse.CourseId).FirstOrDefault().SemesterId = App.SelectedCourse.SemesterId;

                        //Update DataBase
                        await App.DatabaseData.UpdateCourseAsync(App.SelectedCourse.CourseId, App.SelectedCourse.CourseName, App.SelectedCourse.CourseProgress
                             , App.SelectedCourse.CourseDetails, App.SelectedCourse.NotifyOnOff, App.SelectedCourse.OptionalNotes
                             , App.SelectedCourse.InstructorName, App.SelectedCourse.SemesterId, App.SelectedCourse.InstructorEmail, App.SelectedCourse.InstructorPhone
                             , App.SelectedCourse.PerformanceAssessmentAdded, App.SelectedCourse.ObjectiveAssessmentAdded, App.SelectedCourse.PAName
                             , App.SelectedCourse.OAName, App.SelectedCourse.StartDate, App.SelectedCourse.EndDate, App.SelectedCourse.ScheduledPAAssessmentTime, App.SelectedCourse.ScheduledOAAssessmentTime);
                        //Notify User it has been saved
                    }
                    else
                    {

                    }
                }
                else if (textSenderType == App.StringOpts.TypeSemesterName)
                {
                    App.SelectedSemester = tempSemesterInfo;
                    await App.DatabaseData.UpdateSemesterAsync(tempSemesterInfo.SemesterId
                        , tempSemesterInfo.SemesterName, tempSemesterInfo.SemesterStart
                        , tempSemesterInfo.SemesterEnd);
                }

                
                await DisplayAlert("Success", "Save Successful", "Ok");
                //Close the current page
                await Navigation.PopModalAsync();
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

            saveButton.SetDynamicResource(Button.IsVisibleProperty, "EditTextViewSaveButtonVisible");
            Resources["EditTextViewSaveButtonVisible"] = false;
            //saveButton.GestureRecognizers.Add(saveButtonClicked);
            return saveButton;
        }
    }
}