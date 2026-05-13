using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Diagnostics;

namespace NetworkScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            string baseIp = "192.168.0."; 
            
            Console.WriteLine($"Починаю сканування мережі {baseIp}0/24...\n");
            Stopwatch sw = Stopwatch.StartNew();

            
            Parallel.For(1, 255, i =>
            {
                string ip = baseIp + i;
                ScanHost(ip);
            });

            sw.Stop();
            Console.WriteLine($"\nСканування завершено за {sw.Elapsed.TotalSeconds:F1} сек.");
            Console.ReadKey();
        }

        static void ScanHost(string ip)
        {
            using (Ping pinger = new Ping())
            {
                try
                {
                    
                    PingReply reply = pinger.Send(ip, 500);

                    if (reply.Status == IPStatus.Success)
                    {
                        string hostName = "Незвісно";
                        try
                        {
                           
                            hostName = Dns.GetHostEntry(ip).HostName;
                        }
                        catch { }

                        Console.WriteLine($"[+] {ip,-15} | Відгук: {reply.RoundtripTime,3}ms | Ім'я: {hostName}");
                    }
                }
                catch {  }
            }
        }
    }
}