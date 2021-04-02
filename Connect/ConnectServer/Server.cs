using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using SpaceGame;
using Connect;


namespace cServer
{
    class InteractImpl : Interact.InteractBase
    {
        // Server side handler of the SayHello RPC
        public override Task<Reply> Interaction(Request request, ServerCallContext context)
        {
            string reply;
            try
            {
                object value = null; // Used to store the return value
                value = ConnectServer.AddCommand(request.Command, request.User);
                reply = (string)value;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("The server may need to be restarted please check active connections");
                reply = "The server has suffered an exception and is trying to process your request";
            }     
            return Task.FromResult(new Reply { Message = reply });
        }
    }

    public class ConnectServer
    {
        const int Port = 50051;
        static int disconnected_users = 0;

        public static string AddCommand(String data, String username)
        {
            List<Spaceship> search = ElasticSearchCommands.searchUsername(username);

            Spaceship user = search[0];
            Console.WriteLine("User {0} connected", username);        
            
            String command = data;

            command = command.Replace(" ", "");
            var command_list = command.Split(new Char[] { '(', ',',')' });

            double angle, polarAngle;
            int energy;
            
            if (command_list[0].Equals("move"))
            {
                angle = float.Parse(command_list[1]);
                energy = int.Parse(command_list[2]);
                user.Move(angle, energy);
            }
            
            else if (command_list[0].Equals("shoot"))
            {
                polarAngle = float.Parse(command_list[1]);
                angle = float.Parse(command_list[2]);
                energy = int.Parse(command_list[3]);
                String code = command_list[4];
                user.Shoot(polarAngle, angle, energy,code);
            }
            
            else if (command_list[0].Equals("scan"))
            {
                polarAngle = float.Parse(command_list[1]);
                angle = float.Parse(command_list[2]);
                energy = int.Parse(command_list[3]);
                
                return user.Scan(polarAngle, angle, energy);
            }

            else if (command_list[0].Equals("GetHealth"))
            {
                string originalmessage = ("Current health: " + user.Health);

                return originalmessage;
            }

            else if (command_list[0].Equals("GetEnergy"))
            {
                string originalmessage = ("Current energy: " + user.Energy);

                return originalmessage;

            }

            else if (command_list[0].Equals("exit"))
            {
                disconnected_users += 1;
                return "Quitting game!";
            }

            else 
            {
                return "Unknown command: (" + command+") \n Please try again." ;
            }
            return "";
        }


        public void UpdateBalancer(string address, string balancerIP)
        {
            balancerIP += ":50050";
            Channel channel = new Channel(balancerIP, ChannelCredentials.Insecure);
            var client = new Interact.InteractClient(channel);
            while(true)
            {
                try
                {
                    var confirmation = client.UpdateUsers(new ServerStatus { Address = address, Users = disconnected_users });
                    if (confirmation.Received == "true")
                    {
                        Console.WriteLine("Successful User Update");
                    }
                    else
                    {
                        Console.WriteLine("User update failed");
                    }
                }
                catch (Exception e)
                {
                    Exception baseException = e.GetBaseException();
                    Console.WriteLine("Could not connect to loadbalancer...\nServer restart may be required\nThe exception encountered is below: ");
                    Console.WriteLine(baseException.ToString());
                }
                Thread.Sleep(30000);
            }
        }


        public bool InitialiseBalancer(string address, string balancerIP)
        {
            balancerIP += ":50050";
            Channel channel = new Channel(balancerIP, ChannelCredentials.Insecure);
            var client = new Interact.InteractClient(channel);
            try
            {
                var confirmation = client.UpdateUsers(new ServerStatus { Address = address, Users = disconnected_users, InitialConnection = "True" });
                
                if (confirmation.Received == "true")
                {
                    Console.WriteLine("Successfully initialised balancer.");
                    return true;
                } 
                else
                {
                    Console.WriteLine("Could not intialise the balancer.");
                    return false;    
                }
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Could not connect to loadbalancer...\nThe exception encountered is below: ");
                Console.WriteLine(baseException.ToString());
                return false;
            }
        }

        public void StartServer()
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
            string balancerIP;
            do
            {
                Console.Write("Enter Balancer ip: ");
                balancerIP = Console.ReadLine();

            } while (!InitialiseBalancer(address.ToString(), balancerIP));
            
            Thread updater = new Thread(() => UpdateBalancer(address.ToString(), balancerIP));
            updater.Start();
            
            Console.WriteLine("Game server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();
            
            server.ShutdownAsync().Wait();
        }
    }
}
