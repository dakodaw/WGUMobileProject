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
	public partial class AddCourseView : ContentPage
	{
        public AddCourseView()
        {
            //Create the Stack Layout for the whole page
            StackLayout fullPageStack = new StackLayout();
            fullPageStack.BackgroundColor = Color.Orange;

            //Add Header to fullPageStack
            fullPageStack.Children.Add(createWguHeader("Add A Course"));

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

            Button saveCourseButton = new Button
            {
                Text = "Save New Course",
                BackgroundColor = Color.Green,
                TextColor = Color.White
            };
            scrollingPageStack.Children.Add(saveCourseButton);

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
    }
}