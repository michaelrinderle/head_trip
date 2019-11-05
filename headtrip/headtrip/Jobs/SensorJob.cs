using Shiny.Jobs;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace headtrip.Jobs
{
    public class SensorJob : Shiny.Jobs.IJob
    {
        public SensorJob()
        {
        }

        public async Task<bool> Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
            while (true)
            {
                try
                {
                    Debug.WriteLine("Service");
                }
                catch
                {
                    break;
                }
            }
            return false;
        }
    }
}