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

namespace FieldEngineerLite
{
    public class JobService
    {
        // TODO - add Mobile Service client and Jobs table



        public async Task InitializeAsync()
        {
            // TODO - setup local database


        }

        public async Task<IEnumerable<Job>> ReadJobs(string search)
        {
            // TODO - load jobs from local storage

            throw new NotImplementedException();
        }

        public async Task SyncAsync()
        {
            // TODO - add synchronization code


        }     

        public async Task UpdateJobAsync(Job job)
        {
            // TODO - complete the job and update locally


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

        public async Task<IEnumerable<Job>> SearchJobs(string searchInput)
        {
            var query = jobTable.CreateQuery ();

            if (!string.IsNullOrWhiteSpace (searchInput)) {
                query = query.Where (job => 
                                job.JobNumber == searchInput
                                // workaround bug: these are backwards
                || searchInput.ToUpper ().Contains (job.Title.ToUpper ())
                || searchInput.ToUpper ().Contains (job.Status.ToUpper ())
                );
            }

            return await query.ToEnumerableAsync ();
        }

        private async Task<bool> IsAuthenticated()
        {
            return await Task.FromResult(true);
        }   

        private List<Job> GetDummyData()
        {
            return new List<Job> {
                new Job 
                { 
                    AgentId = "agent1",
                    Customer = new Customer { FullName = "Fake Customer" },
                    Equipments = new List<Equipment>
                    {
                        new Equipment { Name = "Some cable", ThumbImage = "Data/EquipmentImages/HDMI_1_Thumb.jpg" }
                    },
                    EtaTime = "08:30 - 09:30",
                    Id = "1",
                    JobNumber = "1",
                    Status = Job.InProgressStatus,
                    Title = "Go fix some broken thing"
                },
                new Job 
                { 
                    AgentId = "agent1", 
                    Customer = new Customer { FullName = "Another Pretend Customer" },
                    Equipments = new List<Equipment>
                    {
                        new Equipment { Name = "Some cable", ThumbImage = "Data/EquipmentImages/HDMI_1_Thumb.jpg" }
                    },
                    EtaTime = "10:30 - 11:30",
                    Id = "2",
                    JobNumber = "2",
                    Status = Job.PendingStatus,
                    Title = "Work work"
                },
                new Job 
                { 
                    AgentId = "agent1", 
                    Customer = new Customer { FullName = "John Smith" },
                    Equipments = new List<Equipment>
                    {
                        new Equipment { Name = "Some cable", ThumbImage = "Data/EquipmentImages/HDMI_1_Thumb.jpg" }
                    },
                    EtaTime = "11:30 - 12:30",
                    Id = "3",
                    JobNumber = "3",
                    Status = Job.CompleteStatus,
                    Title = "setup the new thing"
                }
            };
        }

        private List<Job> jobs;

        public bool Online = false;
        #endregion
    }
}

