using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ContosoAuto.Helpers;
using ContosoAuto.Models;
using ContosoAuto.Views;
using ContosoAuto;

namespace FieldEngineerLite
{

    public class MyNavigationPage :NavigationPage 
    {
        public MyNavigationPage(Page root): base(root)
        {
            NavigationPage.SetHasNavigationBar (this, false);

        }
    }
    public class JobMasterDetailPage : MasterDetailPage
    {

        public JobMasterDetailPage ()
        {
            JobListPage listPage = new JobListPage();
            listPage.JobList.ItemSelected += (sender, e) => 
            {
                var selectedJob = e.SelectedItem as Job;
                if (selectedJob != null)
                {
                    NavigateTo (e.SelectedItem as Job);
                }
            };
            Master = listPage;
            JobDetailsPage details = new JobDetailsPage();

            details.Content.IsVisible = false;
            Detail = new MyNavigationPage(details);



            //this.IsPresented = true;



        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            var jobs = await App.JobService.ReadJobs("");
            if (jobs.Count() > 0)
            {
                Job job = jobs.First();
                NavigateTo(job);
            }

        }
        public void NavigateTo(Job item)
        {
            JobDetailsPage page = new JobDetailsPage();
            page.BindingContext = item;
            Detail = new NavigationPage(page);
            IsPresented = false;
        }
    }
}

