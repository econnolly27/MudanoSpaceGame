using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Generic;

namespace SpaceGame
{
    public static class ElasticSearchCommands
    {
        public static ElasticLowLevelClient create_lowlevelclient() {
            var uris = new[]
            {
                new Uri("https://search-teamse04-f3s7jiwtivrci54ib5b2rm3kl4.eu-west-2.es.amazonaws.com/"),
            };
            var connectionPool = new SniffingConnectionPool(uris);
            var settings = new ConnectionSettings(connectionPool)
                .DefaultIndex("spaceship");

            var client = new ElasticLowLevelClient(settings);
            return client;
        }
        public static ElasticClient create_client()
        {
            var uris = new[]
            {
                new Uri("https://search-teamse04-f3s7jiwtivrci54ib5b2rm3kl4.eu-west-2.es.amazonaws.com/"),
            };
            var connectionPool = new SniffingConnectionPool(uris);
            var settings = new ConnectionSettings(connectionPool)
                .DefaultIndex("spaceship");

            var client = new ElasticClient(settings);
            return client;
        }

        public static void Populate()
        {

            //Only works if instance of elasticsearch is running

            var client = create_lowlevelclient();


            var Spaceships = new object[]
            {
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "0", }},
                new { _username="Tata", _health=100, _capacity=100, _energy=100, _xCoord=-20, _yCoord=0, id="0",code="1234567891234567"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "1", }},
                new { _username="Callum", _health=100, _capacity=100, _energy=100, _xCoord=-85, _yCoord=80, id="1",code="8273781009352487"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "2"  }},
                new { _username="Hugh", _health=100, _capacity=100, _energy=100, _xCoord=80, _yCoord=49, id="2",code="1234566356352487"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "3"  }},
                new { _username="Erin", _health=100, _capacity=100, _energy=100, _xCoord=-10, _yCoord=10, id="3",code="3454356200177235"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "4"  }},
                new { _username="Eli", _health=100, _capacity=100, _energy=100, _xCoord=-85, _yCoord=-45, id="4",code="5219807623512345"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "5"  }},
                new { _username="Jamie", _health=100, _capacity=100, _energy=100, _xCoord=35, _yCoord=-25, id="5",code="6352673625615676"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "6"  }},
                new { _username="Sixx", _health=100, _capacity=100, _energy=100, _xCoord=-70, _yCoord=23, id="6", code="1243567612636476"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "7"  }},
                new { _username="Suzie", _health=100, _capacity=100, _energy=100, _xCoord=50, _yCoord=15, id="7",code="98263546716253676"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "8"  }},
                new { _username="Ronnie", _health=100, _capacity=100, _energy=100, _xCoord=-60, _yCoord=-17, id="8", code="7876253615263780"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "9"  }},
                new { _username="Gill", _health=100, _capacity=100, _energy=100, _xCoord=66, _yCoord=3, id="9",code="1235676152364709"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "10"  }},
                new { _username="Fraser", _health=100, _capacity=100, _energy=100, _xCoord=-20, _yCoord=-55, id="10",code="5672348173647819"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "11"  }},
                new { _username="Owen", _health=100, _capacity=1000, _energy=1000, _xCoord=-37, _yCoord=51, id="11", code="23884311098273647"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "12"  }},
                new { _username="Robert", _health=100, _capacity=100, _energy=100, _xCoord=-18, _yCoord=-75, id="12", code="8745120987345612"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "13"  }},
                new { _username="Buzie", _health=100, _capacity=100, _energy=100, _xCoord=25, _yCoord=78, id="13", code="5467811188267483"},

            };


            var ndexResponse = client.Bulk<StringResponse>(PostData.MultiJson(Spaceships));
            Console.WriteLine("Server populated.");
        }

        public static List<Spaceship> searchUsername(string inputString)
        {
            var client = create_client();

            var searchResponse = client.Search<Spaceship>(s => s
                .Index("spaceships")
                .Query(q => q
                     .Match(m => m
                        .Field(f => f.Name)
                        .Query(inputString)
                     )
                )
            );

            List<Spaceship> spaceship_list = new List<Spaceship>();
            foreach (var spaceship in searchResponse.Documents)
            {
                spaceship_list.Add(spaceship);
            }

            return spaceship_list;
        }

        public static List<Spaceship> searchAll()
        {
            var client = create_client();

            var searchResponse = client.Search<Spaceship>(s => s
                .Index("spaceships")
                .From(0)
                .Size(20)
                .Query(q => q
                     .Range(c => c
                        .Field(f => f.Health)
                        .GreaterThan(0)
                     )
                )
            );

            List<Spaceship> spaceship_list = new List<Spaceship>();
            foreach (var spaceship in searchResponse.Documents)
            {                 
                spaceship_list.Add(spaceship);
            }

            return spaceship_list;
        }

        public static void updateUser(Spaceship ship)
        {
            var client = create_lowlevelclient();
            var ndexResponse = client.Index<BytesResponse>("spaceships", ship.UserId, PostData.Serializable(ship));
        }

        
        public static void UpdateEnergy()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
                foreach (var ship in ElasticSearchCommands.searchAll().ToArray())
                {
                    if (ship.Energy + 2 <= ship.Capacity)
                    {
                        ship.Energy += 2;
                        ElasticSearchCommands.updateUser(ship);
                    }
                }
            }
        }

        static double GenRand(double one, double two)
        {
            Random rand = new Random();
            return one + rand.NextDouble() * (two - one);
        }
        public static Spaceship CreateBotShip(string n)
        {

            var client = create_client();
            Spaceship bot = new Spaceship(n, "bot", 0, 0);
            var indexResponse = client.IndexDocument(bot);

            Console.WriteLine("Bot Created with ID:{0}", n);
            return bot;

        }

        public static void BotBehaviour(Spaceship ship)
        {
            Console.WriteLine("Bot Started");
            while (true)
            {
                System.Threading.Thread.Sleep(2000);
                if (ship.Energy >= 50) { 
                    ship.Move(GenRand(0, 10), 10);
                
                    double x = GenRand(0, 360);
                    double y = GenRand(0, 45);
                    ship.Shoot(x, y, 20, "abcdefghijklmnop");
                    

                }
            }
        }
    }
}
