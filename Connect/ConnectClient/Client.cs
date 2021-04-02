using System;
using Grpc.Core;
using Connect;

namespace cClient
{
    public class Client
    {
        static string givenIp;
        static string username;
        public static void Main(string[] args)
        {
            Channel channelLoadBalancer = Connect();

            var clientLoadBalancer = new Interact.InteractClient(channelLoadBalancer);
            
            Boolean loggedIn = false;
            while(!loggedIn)
            {
                Console.Write("Username: ");
                username = Console.ReadLine();
                loggedIn = Login(username, clientLoadBalancer);
            }
            
            Channel channelServer = new Channel(givenIp, ChannelCredentials.Insecure);
            var clientServer = new Interact.InteractClient(channelServer);
            SendCommand(clientServer);
            channelServer.ShutdownAsync().Wait();
        }

        
        public static Channel Connect()
        {
            String localIp = "127.0.0.1:50050";

            Console.Write("Enter the IP you wish to connect to\nor leave blank for the localhost: ");
            givenIp = Console.ReadLine();

            if (givenIp=="")
            {
                givenIp = localIp;
            } 
            else
            {
                givenIp = givenIp + ":50051";
            }

            Console.WriteLine(givenIp);
            
            return new Channel(givenIp, ChannelCredentials.Insecure);
        }
        
        public static bool Login(string username, Interact.InteractClient clientLoadBalancer)
        {
            try
            {
                var reply = clientLoadBalancer.Login(new LoginRequest { Username = username });
                if (reply.Result == "true")
                {
                    givenIp = reply.Address;
                    Console.WriteLine(givenIp);
                    Console.WriteLine("Login successful");
                    return true;
                }
                else  
                {
                    Console.WriteLine("Login unsuccessful, please try again.");
                    return false;
                }
            }
            catch (Exception e)
            {
                //Causes build warning as the Exception e is not used, this is because a full stack trace would be
                //excessive for a simple user and the error message below is shown instead
                Console.WriteLine("Could not connect to the server.\nYou may need to restart the client.\nPlease check the IP address you are connecting to.");
                return false;
            }
        }

        public static void SendCommand(Interact.InteractClient clientServer)
        {
            string command = "";
            while (command != "exit")
            {
                Console.Write("Command: ");
                command = Console.ReadLine();
                if (command == "exit" )
                {
                    ExitCheck: // Start of goto loop
                    Console.Write("Are you sure you want to quit? Y/N: ");
                    
                    String yesNo = Console.ReadLine();
                    
                    if (yesNo.ToLower() == "y") // command stays as exit and is sent in the finalreply interaction
                    {
                        break;
                    }
                    else if(yesNo.ToLower() == "n") 
                    {
                        command = "";
                        continue;
                    }
                    else //if the response is neither yes or no
                    {
                        goto ExitCheck; // End of goto loop
                    }
                }
                
                try
                {
                    var reply = clientServer.Interaction(new Request { User = username, Command = command });
                    Console.WriteLine(reply.Message);
                }
                catch (Exception e)
                {
                    //Causes build warning as the Exception e is not used, this is because a full stack trace would be
                    //excessive for a simple user and the error message below is shown instead
                    Console.WriteLine("The connection could not reach the server.\nThe server may have crashed.");
                }
            }
        }
    }
}
