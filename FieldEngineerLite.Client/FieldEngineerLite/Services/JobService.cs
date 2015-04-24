using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using ContosoAuto.Helpers;
using ContosoAuto.Models;
using System.Threading;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Net.Http;
using Xamarin.Forms;
using System.Reflection;

namespace ContosoAuto
{
    public class JobService
    {
        // TODO - add Mobile Service client and Jobs table
        //1. connect
        public MobileServiceClient MobileService = 
            new MobileServiceClient (
                "https://techmobileapp-code.azurewebsites.net/",
                "cnhfyetgdhfjjg736dhfgjhdfg3"
            );
        
        public async Task InitializeAsync()
        {
        //2. replace with init
            var store = new MobileServiceSQLiteStore ("fabrikam1.db");
            store.DefineTable<Job> ();

            await MobileService.SyncContext.InitializeAsync (store);

            jobTable = MobileService.GetSyncTable<Job> ();

        }

        public async Task<IEnumerable<Job>> ReadJobs(string search)
        {
        //3. replace with read
            var query = jobTable.CreateQuery ();
            if (string.IsNullOrEmpty (search) == false) {
                query = query.Where (job => (job.Title == search));
            }
            return await query.ToEnumerableAsync ();
        }

        public async Task UpdateJobAsync(Job job)
        {
            job.Status = Job.CompleteStatus;
        //4. add update
            await jobTable.UpdateAsync (job);
        }

        public async Task SyncAsync()
        {
        //6. add auth


        //5. add sync
            try
            {
                await this.MobileService.SyncContext.PushAsync ();

                var query = jobTable.CreateQuery ()
                                            .Where (job => job.AgentId == "2");

                await jobTable.PullAsync (null, query);
            }
            catch(Exception ex)
            {}
        }   
               



















        #region Other Client Code

        private IMobileServiceSyncTable<Job> jobTable;

        public async Task ClearAllJobs()
        {
            jobs = new List<Job> ();
            await Task.FromResult(0);
        }

        public async Task CompleteJobAsync(Job job)
        {
            await UpdateJobAsync(job);

            if(Online)
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
        public bool LoginInProgress=false;
        public bool Online = false;

        public void InitMobileService()
        {
            var mobileServiceProperty = this.GetType().GetField("MobileService", BindingFlags.Instance | BindingFlags.Public );
            if (mobileServiceProperty != null)
            {
                var mobileServicePropertyValue = mobileServiceProperty.GetValue(this);
                var uriProperty = mobileServicePropertyValue.GetType().GetProperty("ApplicationUri");
                string uri = uriProperty.GetValue(mobileServicePropertyValue).ToString();

                var keyProperty = mobileServicePropertyValue.GetType().GetProperty("ApplicationKey");
                string key = keyProperty.GetValue(mobileServicePropertyValue).ToString();

                mobileServiceProperty.SetValue(this,
                    new MobileServiceClient("https://servicedepartmentproxysite.azurewebsites.net/", key,
                        new MobileServiceHandler("code-microsoft-mobilee641d4eef5514ead9d4889e8a273528e")));
            }

        }

        public class MobileServiceHandler : DelegatingHandler
        {
            private string userSiteName;

            public MobileServiceHandler(string userSiteName)
            {
                this.userSiteName = userSiteName;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                bool isLoginRequest = request.RequestUri.AbsolutePath.EndsWith("/login");
                if (!isLoginRequest)
                {
                    int firstIndex = request.RequestUri.Host.IndexOf('.');
                    string domain = request.RequestUri.Host.Substring(firstIndex);
                    request.RequestUri = new Uri(string.Format("{0}://{1}{2}/{3}",
                        request.RequestUri.Scheme, this.userSiteName, domain, request.RequestUri.PathAndQuery));
                }
                return await base.SendAsync(request, cancellationToken);
            }
        }  


        #endregion
    }
}

