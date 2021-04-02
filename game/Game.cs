using System;
using System.Threading;
using System.Collections.Generic;
using cServer;

namespace SpaceGame
{
    public static class Game
    {
        static void Main()
        {
            ElasticSearchCommands.Populate();
            
            ConnectServer c = new ConnectServer();
            Thread cServer = new Thread(()=>c.StartServer());
            cServer.Start();



            Thread energy_update = new Thread(() => ElasticSearchCommands.UpdateEnergy());

            //Set boolean to true to run bot clients
            bool bot_test = false;
            if (bot_test) {
                List<Thread> thread_list = new List<Thread>();

                for (int i = 0; i < 1; i++) {
                    thread_list.Add(new Thread(() => ElasticSearchCommands.BotBehaviour(ElasticSearchCommands.CreateBotShip((i + 20).ToString()))));
                    thread_list[i].Start();
                    Thread.Sleep(500);
                }
            }

            energy_update.Start();
            Console.WriteLine("Pass");
        }
    }

}