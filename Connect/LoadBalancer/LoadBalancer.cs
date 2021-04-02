using System.Threading.Tasks;
using System.Collections.Generic;
using Grpc.Core;
using Connect;
using SpaceGame;
using System;
using System.Net;
using System.Net.Sockets;
using System.Linq;

namespace cLoadBalancer
{
    class InteractImpl : Interact.InteractBase
    {
        // Server side handler of the SayHello RPC
        public override Task<Confirmation> UpdateUsers(ServerStatus status, ServerCallContext context)
        {
            string result;
            try
            {
                if (status.InitialConnection == "True")
                {
                    LoadBalancer.InitialiseServer(status.Address);
                }
                result = LoadBalancer.UpdateServers(status.Address, status.Users);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                result = "fail";
            }
            return Task.FromResult(new Confirmation { Received = result});
        }

        public override Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
        {
            string result;
            string address;
            try
            {
                result = LoadBalancer.Login(request.Username);
                if (result=="true")
                {
                    address = LoadBalancer.AssignServer();
                }
                else
                {
                    address = null;
                }
                
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                result = "fail";
                address = "fail";
            }
            return Task.FromResult(new LoginReply { Result = result, Address = address });
        }
    }



    public class LoadBalancer
    {
        
        const int Port = 50050;
        static Dictionary<String, Dictionary<String, Int32>> Servers = new Dictionary<String, Dictionary<String, Int32>>();
        public static void Main()
        {
            IPAddress address = Array.Find(Dns.GetHostEntry(string.Empty).AddressList, a => a.AddressFamily == AddressFamily.InterNetwork);
            Console.WriteLine(address.ToString());
            Server server = new Server
            {
                Services = { Interact.BindService(new InteractImpl()) },
                Ports = 
                { 
                    new ServerPort("localhost", Port, ServerCredentials.Insecure), 
                    new ServerPort(address.ToString(), Port, ServerCredentials.Insecure) 
                }
            };
            server.Start();
            Console.WriteLine("LoadBalancer listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();
            
            server.ShutdownAsync().Wait();
        }

        public static String Login(String username)
        {
            Console.WriteLine("Client attempting Login");
            List<Spaceship> search = ElasticSearchCommands.searchUsername(username);
            if(search.Count == 0)
            {
                return "false";
            }
            else
            {
                return "true";
            }
        }

        public static String AssignServer()
        {
            var keyAndValue = Servers.OrderBy(kvp => kvp.Value["currentUsers"]).First();
            Console.WriteLine("{0} => {1}", keyAndValue.Key, keyAndValue.Value["currentUsers"]);
            Servers[keyAndValue.Key]["connectedUsers"] += 1;
            return (string)keyAndValue.Key + ":50051";
        }


        public static void InitialiseServer(String address)
        {
            Servers[address] = new Dictionary<string, int>();
            Servers[address]["connectedUsers"] = 0;
            Servers[address]["disconnectedUsers"] = 0;
            Servers[address]["currentUsers"] = 0;
        }

        public static String UpdateServers(String address, Int32 users)
        {
            Console.WriteLine("Server {0} is updating with: {1} diconnected users", address, users);
            Servers[address]["disconnectedUsers"] = users;
            Servers[address]["currentUsers"] = Servers[address]["connectedUsers"] - users;
            return "true"; 
        }
    }

}