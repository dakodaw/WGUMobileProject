using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace WGUMobileProject
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
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
            Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);
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
            for (int i = 1; i < 7; i++)
            {
                var termLayout = createTerm("Term " + i.ToString(), DateTime.Parse("2012-01-01"), DateTime.Now);
                scrollingPageStack.Children.Add(termLayout);
            }
            //Add the Add Semester Button to Bottom of scrollingPageStack
            Button addSemesterButton = new Button
            {
                Text = "+ Add a Semester",
                BackgroundColor = Color.Green,
                TextColor = Color.White
            };
            scrollingPageStack.Children.Add(addSemesterButton);

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
            var grid = new Grid();
            grid.BackgroundColor = Color.Orange;
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.Children.Add(new Button
            {
                BackgroundColor = Color.Blue,
                Text = "Degree",
                TextColor = Color.White,
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button))
            }, 0, 0);
            grid.Children.Add(new Button
            {
                BackgroundColor = Color.Blue,
                Text = "Notifications",
                TextColor = Color.White,
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button))
            }, 1, 0);

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
            // Create a tapGestureRecognizer for when the edit Icon has been selected
            var editTappedEventHandler = new TapGestureRecognizer();
            editTappedEventHandler.Tapped += (sender, e) => {
                DisplayAlert(termName + " Clicked", "Hola, " + termName + " has been selected", "Ok");
                // handle the tap
            };

            View NewTermView()
            {
                var termNameLabel = new Label
                {
                    Text = termName
                };
                termNameLabel.GestureRecognizers.Add(editTappedEventHandler);

                return new Frame
                {
                    BackgroundColor = Color.CornflowerBlue,
                    Content = new StackLayout
                    {
                        Children =
                        {
                            termNameLabel,
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Children =
                                {
                                    new DatePicker
                                    {
                                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(DatePicker)),
                                        Date = startDate
                                    },
                                    new DatePicker
                                    {
                                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(DatePicker)),
                                        Date = DateTime.Parse("1900-01-01")
                                    }
                                },

                            }
                        }
                    }
                };
            };
            var newFrame = NewTermView();


            //Get The Image to display, and add Event Handler
            Image editImg = new Image
            {
                Source = ImageSource.FromFile("EditIcon.png")
            };
            editImg.GestureRecognizers.Add(editTappedEventHandler);

            //Add the image and the Term Frame to the Stack Layout, and return to the sender
            StackLayout termLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children =
                    {
                        editImg,
                        newFrame
                    }
            };
            return termLayout;
        }
    }
}
