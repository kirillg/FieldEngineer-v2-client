using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices;
//using Android.Provider;
using Newtonsoft.Json;


namespace ContosoAuto.Models
{
    public class Job
    {
        public Job()
        {
            Items = new List<WorkItem> 
            {
                new WorkItem {Name = "Change Oil", Completed = false},
                new WorkItem {Name = "Fix Brakes", Completed = false},
                new WorkItem {Name = "Change Filter", Completed = false}
            };

        }
        public const string CompleteStatus = "Completed";
        public const string InProgressStatus = "In Progress";
        public const string PendingStatus = "Not Started";

        public string Id { get; set; }
        public string AgentId { get; set; }
        public string JobNumber { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string WorkPerformed { get; set; }
        [JsonIgnore]
        public List<WorkItem> Items{get; set;}




        [Version]
        public string Version { get; set; }

    }
}
