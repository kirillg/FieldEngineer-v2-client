using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FieldEngineerLite.Models;

namespace FieldEngineerLite
{
    public class JobService
    {
        private List<Job> jobs;
        public bool LoginInProgress = false;
        public bool Online = false;            

        public async Task InitializeAsync()
        {
            jobs = GetDummyData();
        }

        public async Task<IEnumerable<Job>> ReadJobs(string search)
        {
            return jobs;
        }

        public async Task UpdateJobAsync(Job job)
        {
            job.Status = Job.CompleteStatus;
        }

        public async Task SyncAsync()
        {
            await Task.FromResult(0);
        }

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

    }
}
