using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGUMobileProject.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace WGUMobileProject.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CourseView : ContentPage
	{        
        public CourseView(string Course)
        {
            //Select the course that matches the courseName selected
            var courseSelected = App.SelectedCourse;
            //If new course is opened, press the save button
            if (App.IsNewCourse == true)
            {
                EventArgs args = new EventArgs();
                saveButtonEvent(this, args);
            }
            //Create the Stack Layout for the whole page
            StackLayout fullPageStack = new StackLayout();
            fullPageStack.BackgroundColor = Color.Orange;

            //Add Header to fullPageStack
            fullPageStack.Children.Add(createWguHeader(App.SelectedCourse.CourseName));

            //Add Scroll Stack to Full Page
            fullPageStack.Children.Add(createScrollingStack(Course));

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
        public ScrollView createScrollingStack(string courseName)
        {
            //USE THIS LATER TO POPULATE FROM SQLITE
            //Testing Views for Terms with Information
            StackLayout scrollingPageStack = new StackLayout
            {
                Padding = new Thickness(5),
                Spacing = 10
            };

            //Show the progressView
            scrollingPageStack.Children.Add(createProgressSection(App.SelectedCourse.CourseProgress));

            //Show the Notifications Subscription
            scrollingPageStack.Children.Add(createNotificationSection(App.SelectedCourse.NotifyOnOff));

            //Class Overview
            scrollingPageStack.Children.Add(createNotesSection("Class Overview", App.SelectedCourse.CourseDetails));

            //Course Instructor Information
            scrollingPageStack.Children.Add(createCourseInstructorInfo(App.SelectedCourse.InstructorName, App.SelectedCourse.InstructorEmail, App.SelectedCourse.InstructorPhone));

            //Add the Add Objective Assessment Button to the scrollingPageStack
            
            scrollingPageStack.Children.Add(assessmentCreated(App.SelectedCourse.ObjectiveAssessmentAdded, "an Objective", App.SelectedCourse.OAName));

            //Add the Add Performance Assessment Button to the scrollingPageStack
            scrollingPageStack.Children.Add(assessmentCreated(App.SelectedCourse.PerformanceAssessmentAdded, "a Performance", App.SelectedCourse.PAName));

            //Add Optional Notes to edit or share
            scrollingPageStack.Children.Add(createNotesSection("Optional Notes", App.SelectedCourse.OptionalNotes));

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
        /// Creates A Progress Section
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        public StackLayout createProgressSection(string progress)
        {
            List<string> pickerList = new List<string>();
            pickerList.Add("In Progress");
            pickerList.Add("Completed");
            pickerList.Add("Plan to Take");
            pickerList.Add("Dropped");

            var progressPicker = new Picker
            {
                Title = "Progress:",
                ItemsSource = pickerList,
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Picker))
            };
            var newStack = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    progressPicker
                }
            };
            progressPicker.SelectedIndex = pickerList.IndexOf(progress);
            try
            {
                progressPicker.SelectedIndexChanged += (sender, e) =>
                {
                    App.SelectedCourse.CourseProgress = progressPicker.SelectedItem.ToString();
                    Resources["CourseViewSaveButtonVisibility"] = true;
                };
            }
            catch
            {
                Resources["CourseViewSaveButtonVisibility"] = false;
            }

            return newStack;
        }

        /// <summary>
        /// Creates the View for choosing whether or not you want notifications
        /// </summary>
        /// <param name="notify"></param>
        /// <returns></returns>
        public StackLayout createNotificationSection(bool notify)
        {
            View notifyView()
            {
                var notificationLbl = new Label
                {
                    Text = "Toggle on if you want us to notify you of Course and Assessment Dates",
                    TextColor = Color.White,
                    FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label))
                };
                var notifySwitch = new Switch();

                //Toggle the switch based on input
                if (notify == true)
                    notifySwitch.IsToggled = true;
                else
                    notifySwitch.IsToggled = false;

                //Create Event Handler to change the value of the variable when switch is toggled
                notifySwitch.Toggled += (sender, e) =>
                {
                    App.SelectedCourse.NotifyOnOff = e.Value;
                    Resources["CourseViewSaveButtonVisibility"] = true;
                };

                

                return new Frame
                {
                    //This Frame should include a label, and switch. The view should include this frame and a picker
                    BackgroundColor = Color.CornflowerBlue,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Content = new StackLayout
                    {
                        Children =
                        {
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Children =
                                {
                                    notificationLbl,
                                    notifySwitch
                                }
                            }
                        }
                    }
                };
            };
            var newFrame = notifyView();

            //Add the image and the Term Frame to the Stack Layout, and return to the sender
            StackLayout termLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children =
                    {
                        newFrame,
                    }
            };
            return termLayout;
        }

        /// <summary>
        /// Creates the View for Course Instructor Info
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="email"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="photoName"></param>
        /// <returns></returns>
        public StackLayout createCourseInstructorInfo(string Name, string email, string phoneNumber)
        {
            // Create a tapGestureRecognizer for when the edit Icon has been selected
            var editTappedEventHandler = new TapGestureRecognizer();
            editTappedEventHandler.Tapped += (sender, e) => {
                Navigation.PushModalAsync(new EditTextView("Course Instructor", App.StringOpts.TypeCourseInstructor));
                Resources["CourseViewSaveButtonVisibility"] = true;
            };

            View courseInstructorView()
            {
                var nameLabel = new Label
                {
                    FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                    TextColor = Color.White
                };
                nameLabel.SetDynamicResource(Label.TextProperty, "Instructor Name");

                //nameLabel.GestureRecognizers.Add(editTappedEventHandler);
                var emailLabel = new Label
                {
                    Text = "Email: " + email,
                    FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                    TextColor = Color.White
                };
                emailLabel.SetDynamicResource(Label.TextProperty, "Instructor Email");

                var phoneLabel = new Label
                {
                    Text = "Phone: " + phoneNumber,
                    FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                    TextColor = Color.White
                };
                phoneLabel.SetDynamicResource(Label.TextProperty, "Instructor Phone");

                Device.StartTimer(TimeSpan.FromSeconds(1),
                    () =>
                    {
                        try
                        {
                            Resources["Instructor Name"] = "Name: " + App.SelectedCourse.InstructorName;
                            Resources["Instructor Email"] = "Email: " + App.SelectedCourse.InstructorEmail;
                            Resources["Instructor Phone"] = "Phone: " + App.SelectedCourse.InstructorPhone;
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    });
                //Get The Image to display, and add Event Handler
                Image editImg = new Image
                {
                    Source = ImageSource.FromFile("EditIcon.png")
                };
                editImg.GestureRecognizers.Add(editTappedEventHandler);
                return new Frame
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Gray,
                    Content = new StackLayout
                    {
                        Children =
                        {
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Children =
                                {
                                    editImg,
                                    new Label
                                    {
                                        Text = "Course Instructor Info:",
                                        TextColor = Color.White
                                    }
                                }
                            },
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Children =
                                {
                                    new StackLayout
                                    {
                                        Children =
                                        {
                                            nameLabel,
                                            emailLabel,
                                            phoneLabel
                                        }
                                    }
                                },

                            }
                        }
                    }
                };
            };
            var newFrame = courseInstructorView();




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
        /// Creates A Term Object and returns it to add to the page
        /// </summary>
        /// <param name="assessmentName"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public StackLayout assessmentCreated(bool assessmentCreated, string assessmentType, string assessmentName)
        {
                       
            // Create a tapGestureRecognizer for when the edit Icon has been selected
            var editTappedEventHandler = new TapGestureRecognizer();
            editTappedEventHandler.Tapped += async (sender, e) =>
            {
                Resources["CourseViewSaveButtonVisibility"] = true;
                var result = await DisplayAlert(assessmentName + " Clicked", "Would you like to edit or remove " + assessmentName + "?", "Edit", "Remove");
                if(result == true)
                {
                    await Navigation.PushModalAsync(new EditTextView(assessmentType, App.StringOpts.TypeAssessment));
                }
                else
                {
                    if (assessmentType == App.StringOpts.AssessmentTypePerformance)
                    {
                        App.SelectedCourse.PerformanceAssessmentAdded = false;
                    }
                    else
                    {
                        App.SelectedCourse.ObjectiveAssessmentAdded = false;
                    }
                }
            };

            var scheduledDatePicker = new DatePicker
            {
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(DatePicker)),
            };
                

            var scheduledTimePicker = new TimePicker
            {
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(DatePicker)),
            };

            if (assessmentType == App.StringOpts.AssessmentTypePerformance)
            {
                scheduledTimePicker.Time = App.SelectedCourse.ScheduledPAAssessmentTime.TimeOfDay;
                scheduledDatePicker.Date = App.SelectedCourse.ScheduledPAAssessmentTime.Date;
                scheduledDatePicker.DateSelected += (sender, args) =>
                    {
                        if (scheduledDatePicker.Date < DateTime.Now.Date)
                        {
                            DisplayAlert("Alert", "Cannot Schedule exam before today", "Ok");
                            scheduledDatePicker.BackgroundColor = Color.Red;
                            Resources["CourseViewSaveButtonVisibility"] = false;
                        }
                        else
                        {
                            Resources["CourseViewSaveButtonVisibility"] = true;
                            scheduledDatePicker.BackgroundColor = Color.Orange;
                            App.SelectedCourse.ScheduledPAAssessmentTime = new DateTime(scheduledDatePicker.Date.Year
                                , scheduledDatePicker.Date.Month, scheduledDatePicker.Date.Day, scheduledTimePicker.Time.Hours
                                , scheduledTimePicker.Time.Minutes, scheduledTimePicker.Time.Seconds);
                        }
                    };
                scheduledTimePicker.PropertyChanged += (sender, e) =>
                {
                    if(e.PropertyName == TimePicker.TimeProperty.PropertyName)
                    {
                        App.SelectedCourse.ScheduledPAAssessmentTime = new DateTime(scheduledDatePicker.Date.Year
                            , scheduledDatePicker.Date.Month, scheduledDatePicker.Date.Day, scheduledTimePicker.Time.Hours
                            , scheduledTimePicker.Time.Minutes, scheduledTimePicker.Time.Seconds);
                    }
                };
            }
            else
            {
                scheduledTimePicker.Time = App.SelectedCourse.ScheduledOAAssessmentTime.TimeOfDay;
                scheduledDatePicker.Date = App.SelectedCourse.ScheduledOAAssessmentTime.Date;
                scheduledDatePicker.DateSelected += (sender, args) =>
                {
                    if (scheduledDatePicker.Date < DateTime.Now.Date)
                    {
                        DisplayAlert("Alert", "Cannot Schedule exam before today", "Ok");
                        scheduledDatePicker.BackgroundColor = Color.Red;
                        Resources["CourseViewSaveButtonVisibility"] = false;
                    }
                    else
                    {
                        Resources["CourseViewSaveButtonVisibility"] = true;
                        scheduledDatePicker.BackgroundColor = Color.Orange;
                        App.SelectedCourse.ScheduledOAAssessmentTime = new DateTime(scheduledDatePicker.Date.Year
                        , scheduledDatePicker.Date.Month, scheduledDatePicker.Date.Day, scheduledTimePicker.Time.Hours
                        , scheduledTimePicker.Time.Minutes, scheduledTimePicker.Time.Seconds);
                    }
                };
                scheduledTimePicker.PropertyChanged += (sender, e) =>
                {
                    if(e.PropertyName == TimePicker.TimeProperty.PropertyName)
                    {
                        App.SelectedCourse.ScheduledOAAssessmentTime = new DateTime(scheduledDatePicker.Date.Year
                        , scheduledDatePicker.Date.Month, scheduledDatePicker.Date.Day, scheduledTimePicker.Time.Hours
                        , scheduledTimePicker.Time.Minutes, scheduledTimePicker.Time.Seconds);
                    }
                };
            }

            View OptionalNotesView()
            {
                var courseNameLabel = new Label();
                courseNameLabel.TextColor = Color.Black;
                if (assessmentType == App.StringOpts.AssessmentTypePerformance)
                    courseNameLabel.Text = "PA - " + assessmentName;
                else
                    courseNameLabel.Text = "OA - " + assessmentName;
                courseNameLabel.GestureRecognizers.Add(editTappedEventHandler);

                courseNameLabel.SetDynamicResource(Label.TextProperty, assessmentType + " Set Text");

                //Get The Image to display, and add Event Handler
                Image editImg = new Image
                {
                    Source = ImageSource.FromFile(App.StringOpts.EditButton)
                };
                editImg.GestureRecognizers.Add(editTappedEventHandler);

                return new Frame
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Orange,
                    Content = new StackLayout
                    {
                        Children =
                        {
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Children =
                                {
                                    editImg,
                                    courseNameLabel
                                }
                            },
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Children =
                                {
                                    new Label
                                    {
                                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                                        Text = "Assessment Scheduled For: ",
                                    },
                                    scheduledDatePicker,
                                    scheduledTimePicker
                                }
                            }
                        }
                    }
                };
            };
            var newFrame = OptionalNotesView();

            //Add the image and the Term Frame to the Stack Layout, and return to the sender
            StackLayout termLayout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    newFrame
                }
            };
            termLayout.SetDynamicResource(StackLayout.IsVisibleProperty, "Info" + assessmentType);


            //Create Button, and add it to a StackLayout
            var addButton = new Button
            {
                Text = "+ Add " + assessmentType + " Assessment",
                BackgroundColor = Color.Green,
                TextColor = Color.White
            };
            addButton.Clicked += (sender, e) =>
            {
                Navigation.PushModalAsync(new EditTextView(assessmentType, App.StringOpts.TypeAssessment));
            };
            var AddAssessmentButton  = new StackLayout
            {
                Children =
                {
                    addButton
                }
            };

            AddAssessmentButton.SetDynamicResource(StackLayout.IsVisibleProperty, "Button" + assessmentType);
            Device.StartTimer(TimeSpan.FromSeconds(1),
            () =>
            {
                try
                {
                    if (assessmentType == App.StringOpts.AssessmentTypeObjective)
                    {
                        Resources["Button" + assessmentType] = !App.SelectedCourse.ObjectiveAssessmentAdded;
                        Resources["Info" + assessmentType] = App.SelectedCourse.ObjectiveAssessmentAdded;
                        Resources[assessmentType + " Set Text"] = App.SelectedCourse.OAName;
                        return true;
                    }
                    else
                    {
                        Resources["Button" + assessmentType] = !App.SelectedCourse.PerformanceAssessmentAdded;
                        Resources["Info" + assessmentType] = App.SelectedCourse.PerformanceAssessmentAdded;
                        Resources[assessmentType + " Set Text"] = App.SelectedCourse.PAName;
                        return true;
                    }                    
                }
                catch
                {
                    return false;
                }
            });

            return new StackLayout
            {
                Children =
                {
                    termLayout, 
                    AddAssessmentButton
                }
            };
        }

        /// <summary>
        /// Creates A Notes Section and returns it to add to the page
        /// </summary>
        /// <param name="title"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public StackLayout createNotesSection(string title, string notes)
        {
            var courseNotesText = new Label
            {
                //Text = notes,
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                TextColor = Color.White
            };
            if (title == App.StringOpts.TitleCourseOverview)
                courseNotesText.SetDynamicResource(Label.TextProperty, "CourseOverviewNotes");
            else if (title == App.StringOpts.TitleOptionalNotes)
                courseNotesText.SetDynamicResource(Label.TextProperty, "OptionalNotesKey");

            Device.StartTimer(TimeSpan.FromSeconds(1),
                    () =>
                    {
                        try
                        {
                            if (title == App.StringOpts.TitleCourseOverview)
                            {
                                Resources["CourseOverviewNotes"] = App.SelectedCourse.CourseDetails;
                                courseNotesText.Text = App.SelectedCourse.CourseDetails;
                                return true;
                            }
                            else
                            {
                                Resources["OptionalNotesKey"] = App.SelectedCourse.OptionalNotes;
                                courseNotesText.Text = App.SelectedCourse.OptionalNotes;
                                return true;
                            }
                        }
                        catch
                        {
                            return false;
                        }
                    });

            // Create a tapGestureRecognizer for when the edit Icon has been selected
            var editTappedEventHandler = new TapGestureRecognizer();
            editTappedEventHandler.Tapped += (sender, e) => {
                Resources["CourseViewSaveButtonVisibility"] = true;
                Navigation.PushModalAsync(new EditTextView(title, App.StringOpts.TypeNotes));
                if (title == App.StringOpts.TitleCourseOverview)
                    courseNotesText.Text = App.SelectedCourse.CourseDetails;
                else if (title == App.StringOpts.TitleOptionalNotes)
                    courseNotesText.Text = App.SelectedCourse.OptionalNotes;
            };

            //Get The Image to display, and add Event Handler
            Image editImg = new Image
            {
                Source = ImageSource.FromFile(App.StringOpts.EditButton)
            };
            editImg.GestureRecognizers.Add(editTappedEventHandler);

            View OptionalNotesView()
            {
                var courseNameLabel = new Label
                {
                    Text = title,
                    TextColor = Color.White
                };
                var optionalNotesToShare = new Label
                {
                    Text = "Share",
                    TextColor = Color.White,
                    BackgroundColor = Color.Orange
                };
                var shareNotes = new TapGestureRecognizer();
                shareNotes.Tapped += async (sender, args) =>
                {
                    await DisplayAlert("Share?", "Do you want to Share this note?", "Ok");
                    try
                    {
                        var message = new SmsMessage();
                        message.Body = courseNotesText.Text.ToString();
                        await Sms.ComposeAsync(message);
                    }
                    catch (Exception ex)
                    {
                         await DisplayAlert("Error", ex.InnerException.ToString(), "Ok");
                    }
                };
                if (title == App.StringOpts.TitleOptionalNotes)
                {
                    optionalNotesToShare.GestureRecognizers.Add(shareNotes);
                    optionalNotesToShare.IsEnabled = true;
                }
                else
                {
                    optionalNotesToShare.IsVisible = false;
                    optionalNotesToShare.IsEnabled = false;
                }

                return new Frame
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Gray,
                    Content = new StackLayout
                    {
                        Children =
                        {
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Children =
                                {
                                    editImg,
                                    courseNameLabel,
                                    optionalNotesToShare
                                }
                            },
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
        /// Event Handler to save Data to the Database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public async void saveButtonEvent(object sender, EventArgs args)
        {
            try
            {
                App.SelectedCourse.SemesterId = App.SelectedSemester.SemesterId;
                if (App.IsNewCourse == false)
                {
                    //Save Information
                    await App.DatabaseData.UpdateCourseAsync(App.SelectedCourse.CourseId, App.SelectedCourse.CourseName, App.SelectedCourse.CourseProgress
                        , App.SelectedCourse.CourseDetails, App.SelectedCourse.NotifyOnOff, App.SelectedCourse.OptionalNotes
                        , App.SelectedCourse.InstructorName, App.SelectedCourse.SemesterId, App.SelectedCourse.InstructorEmail, App.SelectedCourse.InstructorPhone
                        , App.SelectedCourse.PerformanceAssessmentAdded, App.SelectedCourse.ObjectiveAssessmentAdded, App.SelectedCourse.PAName
                        , App.SelectedCourse.OAName, App.SelectedCourse.StartDate, App.SelectedCourse.EndDate, App.SelectedCourse.ScheduledPAAssessmentTime, App.SelectedCourse.ScheduledOAAssessmentTime);
                    //Notify User it has been saved
                    await DisplayAlert("Success", "Course Successfully Saved", "Ok");
                    //Close the current page
                    await Navigation.PopModalAsync();
                }
                else
                {
                    //Save Information
                    await App.DatabaseData.AddNewCourseAsync(App.SelectedCourse.CourseName, App.SelectedCourse.CourseProgress
                        , App.SelectedCourse.CourseDetails, App.SelectedCourse.NotifyOnOff, App.SelectedCourse.OptionalNotes
                        , App.SelectedCourse.InstructorName, App.SelectedSemester.SemesterId, App.SelectedCourse.InstructorEmail, App.SelectedCourse.InstructorPhone
                        , App.SelectedCourse.PerformanceAssessmentAdded, App.SelectedCourse.ObjectiveAssessmentAdded, App.SelectedCourse.PAName
                        , App.SelectedCourse.OAName, App.SelectedCourse.StartDate, App.SelectedCourse.EndDate, App.SelectedCourse.ScheduledPAAssessmentTime, App.SelectedCourse.ScheduledOAAssessmentTime);
                    //Notify User it has been saved
                    await DisplayAlert("Success", "Course Successfully Saved", "Ok");
                    App.DatabaseData.allCourses = await App.DatabaseData.GetAllCoursesAsync();
                    App.SelectedCourse = App.DatabaseData.allCourses.Where(i => i.CourseName == "New Course").FirstOrDefault();
                    App.IsNewCourse = false;
                    //Close the current page
                    await Navigation.PopModalAsync();
                }
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

            saveButton.SetDynamicResource(Button.IsVisibleProperty, "CourseViewSaveButtonVisibility");
            Resources["CourseViewSaveButtonVisibility"] = false;
            //saveButton.GestureRecognizers.Add(saveButtonClicked);
            return saveButton;
        }
    }
}