using System.ServiceProcess;

namespace NRedisApi.RedisWindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if (!DEBUG)
            var servicesToRun = new ServiceBase[] 
                { 
                    new RedisService() 
                };
            ServiceBase.Run(servicesToRun);

#else
            // Debug code: this allows the process to run as a non-service.
            // It will kick off the service start point, but never kill it.
            // Shut down the debugger to exit
            var service = new RedisService();
            service.RunService();
#endif



        }
    }
}
