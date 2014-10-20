using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using FieldEngineerLite.Helpers;
using FieldEngineerLite.Models;

namespace FieldEngineerLite.Views
{
    public class JobListPage : ContentPage
    {
        private SearchBar searchBar;
        private ListView jobList;
        private JobDetailsPage detailPage; 

        public JobListPage()
        {
            detailPage = new JobDetailsPage();

            jobList = new ListView
            {
                HasUnevenRows = true,
                IsGroupingEnabled = true,
                GroupDisplayBinding = new Binding("Key"),
                GroupHeaderTemplate = new DataTemplate(typeof(JobGroupingHeaderCell)),
                ItemTemplate = new DataTemplate(typeof(JobCell))
            };
            
            jobList.ItemTapped += async (sender, e) =>
            {                
                var selectedJob = e.Item as Job;
                if (selectedJob != null)
                    await ShowJobDetailsAsync((Job)e.Item);                    
            };


            searchBar = new SearchBar { Placeholder = "Enter search" };
            searchBar.SearchButtonPressed += async (object sender, EventArgs e) => 
            {
                await RefreshAsync();
            };

            var onlineLabel = new Label { Text = "Online", Font = AppStyle.DefaultFont, YAlign = TextAlignment.Center };
            var onlineSwitch = new Switch { IsToggled = true, VerticalOptions = LayoutOptions.Center };

            onlineSwitch.Toggled += async (sender, e) => 
            {
                App.JobService.Online = onlineSwitch.IsToggled;

                if(onlineSwitch.IsToggled)
                {
                    await App.JobService.SyncAsync();
                    await RefreshAsync();
                } 
            };

            var syncButton = new Button
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Font = AppStyle.DefaultFont,
                Text = "Refresh",
                WidthRequest = 100,
            };

            syncButton.Clicked += async (object sender, EventArgs e) =>
            {
                try
                {
                    syncButton.Text = "Refreshing..";
                    await App.JobService.SyncAsync();
                    await this.RefreshAsync();
                }
                finally
                {
                    syncButton.Text = "Refresh";
                }
            };

            var clearButton = new Button
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Font = AppStyle.DefaultFont,
                Text = "Clear",
                WidthRequest = 100
            };

            clearButton.Clicked += async (object sender, EventArgs e) =>
            {
                await App.JobService.ClearAllJobs();
                await this.RefreshAsync();
            };

            this.Title = "All Jobs";
            this.Content = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Children = {
                    searchBar,
                    new StackLayout {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        Children = {
                            syncButton, new Label { Text = "   "}, onlineLabel, onlineSwitch
                        }
                    },                                        
					jobList
				}
            };
        }

        public async Task FakeIt()
        {
            while(true)
            {
                if(App.JobService.Online)
                {
                    await App.JobService.SyncAsync();
                    await Task.Delay(3000);
                    await RefreshAsync();
                }
                await Task.Delay(3000);
            }
        }

        Task FakeItTask;
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await this.RefreshAsync();

            FakeItTask = FakeIt();
            System.Diagnostics.Debug.WriteLine(FakeItTask);
        }

        private async Task ShowJobDetailsAsync(Job selectedJob)
        {                        
            detailPage.BindingContext = selectedJob;
            await this.Navigation.PushAsync(detailPage);
        }
            
        private async Task RefreshAsync()
        {        
            var groups = from job in await App.JobService.SearchJobs (searchBar.Text)
                         group job by job.Status into jobGroup                        
                         select jobGroup;

            jobList.ItemsSource = groups.OrderBy(group => GetJobSortOrder(group.Key));     
        }

        private int GetJobSortOrder(string status)
        {
            switch(status) {
                case Job.InProgressStatus: return 0;
                case Job.PendingStatus: return 1;
                case Job.CompleteStatus: return 2;
                default: return -1;
            }                
        }           
    }
}
