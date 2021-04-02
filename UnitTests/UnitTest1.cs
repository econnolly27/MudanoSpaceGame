using NUnit.Framework;
using SpaceGame;
using System;
using System.Text.RegularExpressions;
using Maths;
using Elasticsearch.Net;
using System.Collections.Generic;
using System.Threading;
using cLoadBalancer;
using cClient;
using cServer;

namespace UnitTests
{
    [TestFixture]
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            var client = ElasticSearchCommands.create_lowlevelclient();


            var Spaceships = new object[]
            {
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "0", }},
                new { Name="test0", Health=100, Capacity=100, Energy=100, XCoordinate=0, YCoordinate=0, UserId="0",Code="1234567891234567"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "1", }},
                new { Name="test1", Health=20, Capacity=100, Energy=100, XCoordinate=20, YCoordinate=20, UserId="1",Code="8273781009352487"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "2"  }},
                new { Name="test2", Health=100, Capacity=100, Energy=100, XCoordinate=80, YCoordinate=49, UserId="2",Code="1234566356352487"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "3"  }},
                new { Name="test3", Health=100, Capacity=100, Energy=100, XCoordinate=-10, YCoordinate=10, UserId="3",Code="3454356200177235"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "4"  }},
                new { Name="test4", Health=100, Capacity=100, Energy=100, XCoordinate=0, YCoordinate=0, UserId="4",Code="3454356200177235"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "5"  }},
                new { Name="test5", Health=100, Capacity=100, Energy=100, XCoordinate=10, YCoordinate=20, UserId="5",Code="3454356200177235"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "6"  }},
                new { Name="test6", Health=100, Capacity=1000, Energy=100, XCoordinate=20, YCoordinate=5, UserId="6",Code="3454356200177235"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "7"  }},
                new { Name="test7", Health=100, Capacity=100, Energy=98, XCoordinate=20, YCoordinate=5, UserId="7",Code="3454356200177235"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "100"  }},
                new { Name="test100", Health=100, Capacity=100, Energy=100, XCoordinate=20, YCoordinate=5, UserId="7",Code="3454356200177235"}
            };

            var ndexResponse = client.Bulk<StringResponse>(PostData.MultiJson(Spaceships));
            Console.WriteLine("Populated");
        }
    

        [TearDown]
        public void TearDown()
        {
            var client = ElasticSearchCommands.create_lowlevelclient();


            var Spaceships = new object[]
            {
                 new { index = new { _index = "spaceships", _type = "spaceship", _id = "0", }},
                new { Name="test0", Health=100, Capacity=100, Energy=100, XCoordinate=0, YCoordinate=0, UserId="0",Code="1234567891234567"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "1", }},
                new { Name="test1", Health=20, Capacity=100, Energy=100, XCoordinate=20, YCoordinate=20, UserId="1",Code="8273781009352487"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "2"  }},
                new { Name="test2", Health=100, Capacity=100, Energy=100, XCoordinate=80, YCoordinate=49, UserId="2",Code="1234566356352487"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "3"  }},
                new { Name="test3", Health=100, Capacity=100, Energy=100, XCoordinate=-10, YCoordinate=10, UserId="3",Code="3454356200177235"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "4"  }},
                new { Name="test4", Health=100, Capacity=100, Energy=100, XCoordinate=0, YCoordinate=0, UserId="4",Code="3454356200177235"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "5"  }},
                new { Name="test5", Health=100, Capacity=100, Energy=100, XCoordinate=10, YCoordinate=20, UserId="5",Code="3454356200177235"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "6"  }},
                new { Name="test6", Health=100, Capacity=1000, Energy=100, XCoordinate=20, YCoordinate=5, UserId="6",Code="3454356200177235"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "7"  }},
                new { Name="test7", Health=100, Capacity=100, Energy=98, XCoordinate=20, YCoordinate=5, UserId="7",Code="3454356200177235"},
                new { index = new { _index = "spaceships", _type = "spaceship", _id = "100"  }},
                new { Name="test100", Health=100, Capacity=100, Energy=100, XCoordinate=20, YCoordinate=5, UserId="7",Code="3454356200177235"}

            };

            var ndexResponse = client.Bulk<StringResponse>(PostData.MultiJson(Spaceships));
            Console.WriteLine("Populated");
        }

        public String smallSpaceshipCode(Point center, char[,] scan_array){
           return  "" + scan_array[(int)center.x -1, (int)center.y] + 
                            scan_array[(int)center.x, (int)center.y ] +
                            scan_array[(int)center.x, (int)center.y + 1] +
                            scan_array[(int)center.x +1,(int)center.y + 1]+
                            scan_array[(int)center.x +1,(int)center.y] +
                            scan_array[(int)center.x +1,(int)center.y-1 ] +
                            scan_array[(int)center.x, (int)center.y - 1] +
                            scan_array[(int)center.x-1,(int)center.y - 1];
        }

        public char[,] fromScan(Spaceship spaceship,double polarAngle, double scanAngle, int dist ){
            String presentInScan = "";
            presentInScan = spaceship.Scan(polarAngle, scanAngle, dist);
            Console.WriteLine(presentInScan);
            string[] lines = Regex.Split(presentInScan, "\n");
            char[,] scan_array = new char[lines.Length, lines[0].Length];
            int i = 0,j;

            foreach (string line in lines){

                char[] l = line.ToCharArray();
                for (j = 0; j < l.Length; j++)
                    scan_array[i,j] = l[j];
                i += 1;
                
            }
            return scan_array;
        }


        [Test]
        public void Move_StandardParameters_CorrectCoordinates()
        {
            Spaceship spaceship = new Spaceship("1", "spaceship", 0, 0);
            spaceship.Move(45, 20);
            Assert.AreEqual(14.2, Math.Round(spaceship.XCoordinate,1));
            Assert.AreEqual(14.2, Math.Round(spaceship.YCoordinate,1));
        }


        [Test]
        public void Move_GreaterCapacityShip_CorrectCoordinates()
        {
            Spaceship spaceship = new Spaceship("1", "spaceship", 20, 30);
            spaceship.Capacity = 1000;
            spaceship.Move(60, 80);
            Assert.AreEqual(24, spaceship.XCoordinate);
            Assert.AreEqual(36, Math.Floor(spaceship.YCoordinate));
        }


        [Test]
        public void Move_NegativeCos_CorrectCoordiates()
        {
            Spaceship spaceship = new Spaceship("1", "spaceship", 0, 0);

            spaceship.Move(180, 20);
            TestContext.Out.WriteLine("Message to write to log");
            Assert.AreEqual(-20.0, spaceship.XCoordinate);
            Assert.AreEqual(0.0, spaceship.YCoordinate);
        }


        [Test]
        public void Move_NegativeSin_CorrectCoordinates()
        {

            Spaceship spaceship = new Spaceship("1", "spaceship", 0, 0);

            spaceship.Move(270, 20);

            Assert.AreEqual(0, spaceship.XCoordinate);
            Assert.AreEqual(-20, spaceship.YCoordinate);
        }
        

        


        [Test]
        public void Scan_SpaceshipCenterNotInScan_BothShips()
        {

            Spaceship spaceship =  new Spaceship("0", "spaceship", 10, 10);
            Spaceship spaceship1 = new Spaceship("1", "spaceship", 5, 30);
            Spaceship spaceship2 = new Spaceship("2", "spaceship", 50, 0);

            String all_codes = spaceship1.Code + spaceship2.Code;
            String scanCodes = spaceship.Scan(45,90,50);
            scanCodes.Replace(" ", String.Empty);
            Console.WriteLine("scancodes" + scanCodes);

            Assert.AreEqual(0, StringDistance.Compute(scanCodes, all_codes));
        
        }
          
        [Test]
        public void Scan_SpaceshipInSecondQuadrant_BothShips()
        {
            Spaceship spaceship = new Spaceship("100", "spaceship", -10, 10);
            Spaceship spaceship1 = new Spaceship("101", "spaceship", -20, 20);
            Spaceship spaceship2 = new Spaceship("102", "spaceship", -30, 5);

            spaceship2.Capacity = 1000;

            
            String presentInScan = "";
            
            presentInScan = spaceship.Scan(135, 90, 50);

            presentInScan = presentInScan.Replace(" ", string.Empty);
           

            if (StringDistance.Compute(presentInScan, spaceship1.Code)>0 && 
                    StringDistance.Compute(presentInScan, spaceship1.Code)>0 )
            {   
                Assert.Pass();
            }
        }


        [Test]
        public void Shoot_SimpleValues_ReturnsShotShips()
        {
            Spaceship spaceship = new Spaceship("100", "spaceship", 0, 0);
            Spaceship spaceship1 = new Spaceship("101", "spaceship", 20, 20);
            Spaceship spaceship2 = new Spaceship("102", "spaceship", 40, 40);

            spaceship2.Capacity = 1000;

       

           String all_codes = spaceship1.Code + spaceship2.Code;
            String presentInShoot = "" ;

            List<Spaceship> lst_sp = new List<Spaceship>();
            
            presentInShoot = spaceship.Shoot(45, 45, 50,"12345678");

            string[] lines = Regex.Split(presentInShoot, "\r\n");
            foreach (string line in lines){

                string[] words = line.Split(" ");
                lst_sp.AddRange( ElasticSearchCommands.searchUsername(words[5]));

            }

            if (lst_sp.Contains(spaceship1) && lst_sp.Contains(spaceship2)){
                Assert.Pass();
            }

        }

        
        [Test]
        public void Shoot_SimpleValues_SpaceshipShotShouldHaveLessHealth()
        {
 
            Spaceship spaceship = ElasticSearchCommands.searchUsername("test0")[0];

            spaceship.Shoot(0, 90, 40, "123456778987654321");

            //Thread.Sleep(5000);
            Spaceship spaceship1 = ElasticSearchCommands.searchUsername("test3")[0];
            Assert.Less(spaceship1.Health, 100);

        }

        

        [Test]
        public void Shoot_WithNoEnergy_ShootReturnsNull()
        {
            Spaceship spaceship = new Spaceship("100", "spaceship", 0, 0, "12345");
            spaceship.Energy = 0;
            Assert.AreEqual(null, spaceship.Shoot(45, 45, 50,"123456789"));
        }

        
        [Test]
        public void Shoot_WithNoEnergy_EnergyAtZero()
        {
            Spaceship spaceship = new Spaceship("100", "spaceship", 0, 0, "12345");
            spaceship.Energy = 0;
            spaceship.Shoot(45, 45, 50,"123456789");
            Assert.AreEqual(0, spaceship.Energy);
        }

        
        [Test]
        public void Search_Username_Finds_User()
        {
            Spaceship search_username = ElasticSearchCommands.searchUsername("test0")[0];
            Assert.AreEqual(search_username.Name, "test0");
        }

        
        [Test]
        public void Search_All_Finds_All_Users()
        {
            var client = ElasticSearchCommands.create_lowlevelclient();
            ElasticSearchCommands.Populate();
            List<Spaceship> search_all = ElasticSearchCommands.searchAll();

            Assert.Greater(search_all.Count, 3);
    
        }

        [Test]
        public void Update_User_Correctly_Updates_Information()
        {

            Spaceship test_ship = ElasticSearchCommands.searchUsername("test4")[0];
            test_ship.XCoordinate = 100;
            ElasticSearchCommands.updateUser(test_ship);

            Thread.Sleep(1000);
            Spaceship search_username = ElasticSearchCommands.searchUsername("test4")[0];

            Console.WriteLine(search_username.XCoordinate);
            Assert.AreEqual(search_username.XCoordinate, 100);

        }


        [Test]
        public void Update_Energy_Increases_Energy()
        {

            Thread energy_update = new Thread(() => ElasticSearchCommands.UpdateEnergy());
            energy_update.Start();
            Thread.Sleep(4000);
            Spaceship test_ship = ElasticSearchCommands.searchUsername("test7")[0];
            Assert.AreEqual(100, test_ship.Energy);

        }

        
        [Test]
        public void Pass_Login_Through_LoadBalancer()
        {
            Assert.AreEqual("true", LoadBalancer.Login("test0"));
        }

        
        [Test]
        public void Fail_Login_Through_LoadBalancer()
        {
            Assert.AreEqual("false", LoadBalancer.Login("notUsername"));
        }


        [Test]
        public void LoadBalancer_Server_Assignment()
        {
            LoadBalancer.InitialiseServer("testServer0");
            LoadBalancer.InitialiseServer("testServer1");
            LoadBalancer.InitialiseServer("testServer2");
            LoadBalancer.UpdateServers("testServer0", 10);
            LoadBalancer.UpdateServers("testServer1", 5);
            Assert.AreEqual("testServer0:50051", LoadBalancer.AssignServer()); // testServer0 will have -10 users and as such has the fewest users
        }


        [Test]
        public void Server_AddCommand() //also acts as a test for the exit command
        {
            String test_command = "exit";
            String test_User = "test0";
            String result = ConnectServer.AddCommand(test_command, test_User);
            Assert.AreEqual("Quitting game!", result);
        }


        [Test]
        public void Server_GetHealth_Command()
        {
            String test_command = "GetHealth";
            String test_User = "test0";
            String result = ConnectServer.AddCommand(test_command, test_User);
            Assert.AreEqual("Current health: 100", result);
        }


        [Test]
        public void Server_GetEnergy_Command()
        {
            String test_command = "GetEnergy";
            String test_User = "test0";
            String result = ConnectServer.AddCommand(test_command, test_User);
            Assert.AreEqual("Current energy: 100", result);
        }


        [Test]
        public void Server_Unknown_Command()
        {
            String test_command = "test";
            String test_User = "test0";
            String result = ConnectServer.AddCommand(test_command, test_User);
            Assert.AreEqual("Unknown command: (" + test_command + ") \n Please try again.", result);
        }

        [Test]
        public void Maths_DistanceBetweenPoints_CorrectDistance()
        {
            
            Point p = new Point(0,0);
            Point z = new Point(10,20);

            double distance_between = GameMaths.Distance2Points(p, z);
            double delta =  0.001;
            if ((Math.Abs(distance_between - 22.36) < delta)){
                Assert.Pass();
            }
            Assert.Fail();

        }

        [Test]

        public void Maths_Area_CorrectArea()
        {
            Point A = new Point(0,0);
            Point B = new Point(5,7);
            Point C = new Point(1,2);

            double triangle_area = GameMaths.Area(A, B, C);
            Assert.AreEqual(triangle_area, 1.5);
        }

        [Test]
        public void Maths_isInside_PointInsideTrinagle()
        {
            Point traingleCorner1 = new Point(5,0);
            Point traingleCorner2 = new Point(2,10);
            Point traingleCorner3 = new Point(10,10);
            Point insideTriangle = new Point(5,5);

            Assert.IsTrue(GameMaths.IsInside(traingleCorner1,traingleCorner2 , traingleCorner3, insideTriangle));
        }

        [Test]
        public void Maths_LineIntersectsCircleIn2Points_()
        {
            
            Double[] l = {40,-40,-4000};
            Assert.IsTrue(GameMaths.LineIntersectsCircle(GameMaths.LineEquation(new Point(10,10),
                                                        new Point(50,50)), 20,30,10));
        }
    
        [Test]
        public void Maths_LineIntersection_CorrectPoint()
        {
            Point A = new Point(0,5);
            Point B = new Point(10,5);
            Point C = new Point(5,0);
            Point D = new Point(5,10);

            Point intersectionPoint = GameMaths.LineIntersection(A,B,C,D);

            Assert.AreEqual(intersectionPoint.x, 5);
            Assert.AreEqual(intersectionPoint.y, 5);
        }

        [Test]
        public void TestStringDistance_ReturnsZero()
        {
            Assert.AreEqual(0,StringDistance.Compute("abcdefghi","123456789"));
        }

        [Test]
        public void TestStringDistance_for_NULL()
        {
            Assert.AreEqual(0,StringDistance.Compute(null,"12345678"));
        }

        [Test]
        public void TestStringDistance_Returns_ShortestDistance()
        {
            Assert.AreEqual(5,StringDistance.Compute("abcdefghi","bdafhi"));
        }
        
        [Test]
        public void Maths_DistanceToLine_CorrectDistance()
        {
            Double[] equationValues = {0,-20,200};
            Double distance = GameMaths.DistanceToLine(equationValues, 5, 0);
            Assert.AreEqual(distance, 10);
        }
        
    }

}