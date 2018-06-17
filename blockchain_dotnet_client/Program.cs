using System;
using System.IO;
using System.Net;
using System.Text;

namespace blockchain_dotnet_client
{
    class Program
    {
        static void Main(string[] args)
        {
            Client chainClient = new Client();

            string command = "";

            while (command != "exit")
            {
                Console.WriteLine("Waiting for a new command; \n'exit = exit' \n'getbc = get blockchain' \n'postdm = post diamond to blockchain' \n");
                command = Console.ReadLine();

                switch (command)
                {
                    case "getbc":
                        Console.Clear();
                        Console.WriteLine("\nHTTP server response:");
                        Console.WriteLine(chainClient.Get("http://localhost:5000/Blockchain/chain") + "\n");
                        Console.WriteLine("press any key to continue...");
                        Console.ReadKey();
                        Console.Clear();
                        break;

                    case "postdm":
                        Console.Clear();
                        Console.WriteLine("\nHTTP server response:");
                        //Console.WriteLine(chainClient.Post("http://localhost:5000/Blockchain/chain") + "\n");
                        Console.WriteLine("Body is yeat to be created...");
                        Console.WriteLine("press any key to continue...");
                        Console.ReadKey();
                        Console.Clear();
                        break;
                }
            }

            Console.WriteLine("Hello World!");
        }
    }
}
