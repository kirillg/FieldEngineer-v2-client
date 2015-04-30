using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using FieldEngineerLite.Helpers;
using FieldEngineerLite.Models;
using System.Threading;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Net.Http;
using Xamarin.Forms;
using System.Reflection;

//if false
using Microsoft.Azure.AppService;


namespace FieldEngineerLite
{

    public class JobService
    {
        public MobileServiceClient MobileService = new MobileServiceClient(
            "https://fetechnician-code.azurewebsites.net/",
            "https://fieldengineeref90e9309d7f4a608a99748e0eea69de.azurewebsites.net",
            "OtFsjAFDBBMENsPCBQFJmItwjvAfaX77");

        public AppServiceClient AppService = 
            new AppServiceClient("https://fieldengineeref90e9309d7f4a608a99748e0eea69de.azurewebsites.net");
            
       

        public async Task InitializeAsync()
        {
            

            //2. replace with init
           
            var store = new MobileServiceSQLiteStore("localdb-fabrikam12.db");
            store.DefineTable<Job>();

            await MobileService.SyncContext.InitializeAsync(store);

            jobTable = MobileService.GetSyncTable<Job>();
        }

        public async Task<IEnumerable<Job>> ReadJobs(string search)
        {
            //3. replace with read
            var query = jobTable.CreateQuery();
            if (string.IsNullOrEmpty(search) == false)
            {
                query = query.Where(job => (job.Title == search));
            }
            return await query.ToEnumerableAsync();
        }

        public async Task UpdateJobAsync(Job job)
        {
            job.Status = Job.CompleteStatus;
            //4. add update
            await jobTable.UpdateAsync(job);
        }

        public async Task SyncAsync()
        {
            //6. add auth

            await EnsureLogin();
            //5. add sync
            try
            {
                await this.MobileService.SyncContext.PushAsync();

                var query = jobTable.CreateQuery()
                                .Where(job => job.AgentId == "2");

                await jobTable.PullAsync(null, query);
            }
            catch (Exception)
            { 
            }
        }
        public async Task EnsureLogin()
        {
            LoginInProgress = true;
            while (this.AppService.CurrentUser == null) {
                //await this.AppService.LoginAsync(
                await this.MobileService.LoginAsync (App.UIContext, 
                    MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory);
                this.AppService.SetCurrentUser(this.MobileService.CurrentUser.UserId, this.MobileService.CurrentUser.MobileServiceAuthenticationToken);

            }

            LoginInProgress = false;

        }


        private IMobileServiceSyncTable<Job> jobTable;

        public async Task ClearAllJobs()
        {
            jobs = new List<Job>();
            await Task.FromResult(0);
        }

        public async Task CompleteJobAsync(Job job)
        {
            await UpdateJobAsync(job);

            if (Online)
                await this.SyncAsync();
        }

        private List<Job> GetDummyData()
        {
            return new List<Job> {
                new Job 
                { 
                    AgentId = "agent1",
                    CustomerName = "Lorem Ipsum",

                    StartTime = "08:30",
                    EndTime =  "09:30",
                    Id = "1",
                    JobNumber = "1",
                    Status = Job.InProgressStatus,
                    Title = "Dolor sit amet"
                },
                new Job 
                { 
                    AgentId = "agent1", 
                    CustomerName = "Consectetur Adipiscing",
                    StartTime = "10:30",
                    EndTime = "11:30",
                    Id = "2",
                    JobNumber = "2",
                    Status = Job.PendingStatus,
                    Title = "Incididunt ut labore et dolore"
                },
                new Job 
                { 
                    AgentId = "agent1", 
                    CustomerName = "Magna Aliqua",

                    StartTime = "11:30",
                    EndTime = "12:30",
                    Id = "3",
                    JobNumber = "3",
                    Status = Job.CompleteStatus,
                    Title = "Ut enim ad minim veniam"
                }
            };
        }

        private List<Job> jobs;
        public bool LoginInProgress = false;
        public bool Online = false;

    }
}

//#endif