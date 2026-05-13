using System;
using System.Net;
using System.Linq;

namespace IpParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            try
            {
                Console.Write("Введіть IP-адресу: ");
                string ipInput = Console.ReadLine();
                Console.Write("Введіть маску підмережі (CIDR, наприклад 24): ");
                int cidr = int.Parse(Console.ReadLine());
                
                IPAddress ip = IPAddress.Parse(ipInput);
                byte[] ipBytes = ip.GetAddressBytes();
                
                uint maskUint = uint.MaxValue << (32 - cidr);
                byte[] maskBytes = BitConverter.GetBytes(maskUint).Reverse().ToArray();
                IPAddress mask = new IPAddress(maskBytes);
                byte[] networkBytes = new byte[4];
                byte[] broadcastBytes = new byte[4];

                for (int i = 0; i < 4; i++)
                {
                    networkBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
                    broadcastBytes[i] = (byte)(ipBytes[i] | ~maskBytes[i]);
                }

                IPAddress network = new IPAddress(networkBytes);
                IPAddress broadcast = new IPAddress(broadcastBytes);
                
                byte[] firstHostBytes = (byte[])networkBytes.Clone();
                firstHostBytes[3]++;
                
                byte[] lastHostBytes = (byte[])broadcastBytes.Clone();
                lastHostBytes[3]--;
                
                double hostCount = Math.Pow(2, 32 - cidr) - 2;
                if (hostCount < 0) hostCount = 0;
                
                string netClass = GetNetworkClass(ipBytes[0]);
                
                Console.WriteLine("\nРезультати:");
                Console.WriteLine($"-----------------------------------");
                Console.WriteLine($"IP-адреса:         {ip}");
                Console.WriteLine($"Маска підмережі:   {mask} (/{cidr})");
                Console.WriteLine($"Мережева адреса:   {network}");
                Console.WriteLine($"Broadcast адреса:  {broadcast}");
                Console.WriteLine($"Перший хост:       {new IPAddress(firstHostBytes)}");
                Console.WriteLine($"Останній хост:     {new IPAddress(lastHostBytes)}");
                Console.WriteLine($"Кількість хостів:  {hostCount}");
                Console.WriteLine($"Клас мережі:       {netClass}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
            
            Console.ReadKey();
        }

        static string GetNetworkClass(byte firstByte)
        {
            if (firstByte >= 1 && firstByte <= 126) return "A";
            if (firstByte >= 128 && firstByte <= 191) return "B";
            if (firstByte >= 192 && firstByte <= 223) return "C";
            return "Інший (D/E або службовий)";
        }
    }
}