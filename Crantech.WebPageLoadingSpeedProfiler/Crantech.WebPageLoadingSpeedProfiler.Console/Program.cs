using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace Crantech.WebPageLoadingSpeedProfiler.Main
{
    public class Program
    {
        private const string BrowserAgentHeader = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
        public static string UrlList { get; set; } 

        static Program()
        {
            UrlList = ConfigurationManager.AppSettings["UrlList"];
        }

        private static void Main()
        {
            Introduction();

            ProcessUrls();
            
            Pause("** Hit Enter to continue **");
        }

        private static void ProcessUrls()
        {
            IEnumerable<string> urlsList = LoadUrlStringsFromFile();
            var watch1 = new Stopwatch();
            var watch2 = new Stopwatch();

            foreach (string url in urlsList)
            {
                if(string.IsNullOrEmpty(url))
                    continue;
              
                Console.WriteLine("Reading URL {0}.", url);

                watch1.Start();
                OpenUrl(url);
                watch1.Stop();

                Console.WriteLine("Done in {0} at T + {1}.", watch1.Elapsed.ToString("mm\\:ss\\.ff"), watch2.Elapsed.ToString("mm\\:ss\\.ff"));

                watch1.Reset();
            }

            watch1.Stop();

            watch2.Stop();
        }

        private static void OpenUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return;

            var uriBuilder = new UriBuilder(url);
            if (string.IsNullOrEmpty(uriBuilder.Host))
                uriBuilder.Host = "http://";
            
            var client = new WebClient();
            client.Headers.Add("user-agent", BrowserAgentHeader);

            Stream data = client.OpenRead(uriBuilder.ToString());

            if (data != null)
                new StreamReader(data).ReadToEnd(); // read page
        }

        private static void Introduction()
        {
            Console.WriteLine("Web Page Loading Speed Profiler");
            Console.WriteLine("by Crantech Solutions Ltd");
            Console.WriteLine("");
        }

        private static void Pause(string message)
        {
            Console.WriteLine(message);
            Console.ReadLine();
        }

        private static IEnumerable<string> LoadUrlStringsFromFile()
        {
            Console.WriteLine("Opening URL list {0}.", UrlList);
            string path = "";
            if (UrlList != null)
                path = Path.GetFullPath(UrlList);

            if (string.IsNullOrEmpty(path))
                throw new NullReferenceException(string.Format("Cannot determine path for {0}", UrlList));
            


            return new List<string>(File.ReadAllLines(path, Encoding.UTF8));
        }
    }
}
