using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;

namespace Tests
{
    public enum Status
    {
        Pending = 0,
        Running = 1,
        Complete = 2
    }

    public class Job
    {
        public Guid guid;
        public Status status;
        public object argument;
    }

    public class WorkerPool : IDisposable
    {
        private Timer timer;
        private List<BackgroundWorker> workers;
        private List<Job> jobList;

        public event EventHandler WorkDone;

        public WorkerPool()
        {
            timer = new Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            workers = new List<BackgroundWorker>();
            jobList = new List<Job>();
        }

        public void InstantiateWorkers(int workerNum, DoWorkEventHandler method)
        {
            for (int i = 0; i < workerNum; i++)
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerSupportsCancellation = true;
                worker.DoWork += method;
                worker.RunWorkerCompleted += WorkComplete;
                workers.Add(worker);
            }
        }

        private void WorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            Job completedJob = (Job)e.Result;
            completedJob.status = Status.Complete;
        }

        public void AddJob(object job)
        {
            jobList.Add(new Job()
            {
                guid = Guid.NewGuid(),
                status = Status.Pending,
                argument = job
            });
        }

        public void StartWorking()
        {
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var pendingJobs = jobList.Where(j => j.status == Status.Pending).ToList();

            if (pendingJobs.Count == 0)
            {
                timer.Stop();
                WorkDone(this, null);
                return;
            }

            foreach (Job job in pendingJobs)
            {
                BackgroundWorker worker = workers.FirstOrDefault(w => !w.IsBusy);
                if (worker != null)
                {
                    worker.RunWorkerAsync(job);
                }
            }
        }

        public void Dispose()
        {
            timer.Stop();
            timer.Dispose();

            workers.ForEach(w =>
            {
                if (w.WorkerSupportsCancellation)
                {
                    w.CancelAsync();
                }
            });
            while (!workers.All(w => !w.IsBusy));
            workers.ForEach(w => w.Dispose());
        }
    }
}
