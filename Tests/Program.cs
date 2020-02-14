using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            using (WorkerPool pool = new WorkerPool())
            {
                pool.InstantiateWorkers(10, BackgroundWork);
                for (int i = 0; i <= 300; i++)
                {
                    pool.AddJob(new Pen()
                    {
                        color = "blue",
                        price = 1.5f,
                        brand = "brand " + i.ToString()
                    });
                }
                pool.WorkDone += Pool_WorkDone;
                pool.StartWorking();
                Console.ReadKey();
            }
        }

        private static void Pool_WorkDone(object sender, EventArgs e)
        {
            Console.WriteLine("All done");
        }

        public static void BackgroundWork(object sender, DoWorkEventArgs args)
        {
            Job job = (Job)args.Argument;
            job.status = Status.Running;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Pen));
                using (TextWriter writer = new StreamWriter(job.guid + ".xml"))
                {
                    serializer.Serialize(writer, job.argument);
                }

                using (Stream fs = new FileStream(job.guid + ".xml", FileMode.Open))
                {
                    Pen theSamePen = (Pen)serializer.Deserialize(fs);
                    Console.WriteLine(theSamePen.ToString());
                }

            }
            finally
            {
                File.Delete(job.guid + ".xml");
            }
            args.Result = job;
        }
    }
}
