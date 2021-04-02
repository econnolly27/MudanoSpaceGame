using Maths;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SpaceGame
{
    public class Spaceship
    {

        private int _health;
        private int _capacity;
        private int _energy;
        private String _username;
        private double _xCoord;
        private double _yCoord;
        private String code;


        private string id;



        public Spaceship(String user_id, String name, double xCoord, double yCoord, string code = "")
        {
            id = user_id;
            _username = name;
            _health = 100;
            _capacity = 100;
            _energy = 100;
            _xCoord = xCoord;
            _yCoord = yCoord;
            if (this.code == "")
                this.code = GenRand(100000000,999999999).ToString() + GenRand(100000000,999999999).ToString();
            //GenRand(-Map.MapSize, Map.MapSize);
        }

        static int GenRand(int one, int two)
        {

            Random rand = new Random();
            return rand.Next(one, two);
        }

        public String Code
        {
            get { return this.code; }
            set { this.code = value; }
        }

        public int Health
        {
            get { return _health; }
            set { _health = value; }
        }
        public string Name
        {
            get { return _username; }
            set { _username = value; }
        }

        public int Capacity
        {
            get { return _capacity; }
            set { _capacity = value; }
        }

        public int Energy
        {
            get { return _energy; }
            set { _energy = value; }
        }

        public String UserId
        {
            get { return id; }
            set { id = value; }
        }
        
        public double XCoordinate
        {
            get { return _xCoord; }
            set { _xCoord = value; }
        }

        public double YCoordinate
        {
            get { return _yCoord; }
            set { _yCoord = value; }
        }

        public String Move(double angle, int energy)
        {
                  int distance = energy / (this.Capacity / 100);


            //updateSelf();
            if (this._energy > energy) {
                for ( int i = 1; i <= distance; i++)
                {
                    this.XCoordinate = Math.Round(this.XCoordinate +  Math.Cos(angle * Math.PI / 180.0), 2);
                    this.YCoordinate = Math.Round(this.YCoordinate + Math.Sin(angle * Math.PI / 180.0), 2);
                    this.Energy -= energy / distance;

                    System.Threading.Thread.Sleep(100);
                    Console.WriteLine(this.ToString());
                    ElasticSearchCommands.updateUser(this);

                }
            } 
            else 
            {
                Console.WriteLine("Not enough energy to move");
            }
            return this.ToString();
            
        }


        /*
         * 
         *  Helper function to compute the not shadowed spaceships after a scan or attack 
         * 
         *  returns a dictionary mapping the spaceship to a real ratio (the relative portion of the spaceship that is visible in the scan, shoot
         * 
         */

        public Dictionary<Spaceship, double> SpaceshipsInsideWithShadow(Point a, Point b, Point c)
        {

            Spaceship[] sp = ElasticSearchCommands.searchAll().ToArray();

            // double represents the shadows

            Dictionary<Spaceship, double> list = new Dictionary<Spaceship, double>();

            var lines = new ArrayList();

            lines.Add(new Line(b, c));
            var x = this.XCoordinate;
            var y = this.YCoordinate;

            var dict = new SortedDictionary<double, Spaceship>();

            for (int i = 0; i < sp.Length; i++)
            {
                double x_coord = sp[i].XCoordinate;
                double y_coord = sp[i].YCoordinate;

                if ((GameMaths.ContainsSpaceship(a, b, c, new Point(x_coord, y_coord), sp[i].Capacity / 100)
                    || GameMaths.IsInside(a, b, c, new Point(x_coord, y_coord))) && this.Name != sp[i].Name )//&& sp[i].Name!="Owen")
                {
                    double dist = GameMaths.Distance2Points(new Point(x, y), new Point(x_coord, y_coord));
                    while (dict.ContainsKey(dist))
                        dist += 0.2;
                    dict.Add(dist, sp[i]);
                }
            }

            // find the true non-shadowed spaceships


            foreach (KeyValuePair<double, Spaceship> kvp in dict)
            {
                Spaceship ship = kvp.Value;

                Point p1, p2; // projections of the tangents to the circle onto BC line
                Point[] points = GameMaths.PointsOfTangentToCircle(a, new Point(ship.XCoordinate, ship.YCoordinate), ship.Capacity / 100);

                p1 = points[0];
                p2 = points[1];

                // get intersection between AP1 and BC (projection of A on BC through a point tangent to the circle/spaceship)
                points[0] = GameMaths.LineIntersection(a, points[0], b, c);
                points[1] = GameMaths.LineIntersection(a, points[1], b, c);

                Array.Sort(points);
                p1 = points[0];
                p2 = points[1];

                if (p1 == null || p2 == null)
                    continue;

                int i = 0, j = 0;
                Boolean found1 = false;
                Boolean found2 = false;

                //find where the projections of the points on BC found would sit on the line BC
                // the points l1,l2 are looking for where p1 sits
                // the poiints n1, n2 are looking for where p2 sits
                // modifications in the intervals is necessary
                // delete all lines between p1 and p2 


                while (!(found1 && found2))
                {
                    // fetch the ith line
                    if (!found1 && i < lines.Count)
                    {
                        Line line = (Line)lines[i];
                        Point l1 = line.a, l2 = line.b;

                        if (p1.CompareTo(l1) > 0)
                            if (p1.CompareTo(l2) < 0)
                            {
                                // l1 < p1 < l2
                                // found the interval where p1 lies, it is not shadowed, so add the spacehsip
                                found1 = true;



                                // check if p1 and p2 are on the same interval

                                if (p2.CompareTo(l1) > 0 && p2.CompareTo(l2) < 0)
                                {
                                    list.Add(ship, GameMaths.Distance2Points(p1, p2));

                                    found2 = true;
                                    Point aux = ((Line)lines[i]).b;
                                    ((Line)lines[i]).b = p1;
                                    lines.Insert(i + 1, new Line(p2, aux));
                                }

                                else
                                {
                                    list.Add(ship, GameMaths.Distance2Points(p1, ((Line)lines[i]).b));
                                    ((Line)lines[i]).b = p1;
                                    j = i + 1;

                                }
                            }

                            else
                            {
                                // p1 will sit in the next intervals
                                i++;
                            }
                        else
                        {
                            // p1 lies in an area outside of the lines array, which means it is shadowed
                            // mark l1 as found, but don't add this spaceship to the scanned ones
                            found1 = true;

                        }
                    }

                    if (!found2 && j < lines.Count)
                    {

                        Line line = (Line)lines[j];
                        Point n1 = line.a, n2 = line.b;

                        if (p2.CompareTo(n1) > 0)
                        {
                            if (p2.CompareTo(n2) < 0)
                            {
                                // n1 < p2 < n2
                                // found the interval where p2 lies
                                found2 = true;
                                if (list.ContainsKey(ship))
                                    list[ship] += GameMaths.Distance2Points(((Line)lines[j]).a, p2);
                                else
                                    list.Add(ship, GameMaths.Distance2Points(((Line)lines[j]).a, p2));

                                // restrict the interval for future lookups
                                ((Line)lines[i]).a = p2;
                            }

                            else
                            {
                                // p2 will sit in the next intervals

                                if (found1)
                                {

                                    if (list.ContainsKey(ship))
                                        list[ship] += GameMaths.Distance2Points(((Line)lines[j]).a, ((Line)lines[j]).b);
                                    else
                                        list.Add(ship, GameMaths.Distance2Points(((Line)lines[j]).a, ((Line)lines[j]).b));

                                    lines.RemoveAt(j);              // delete all intervals in between p1 and p2

                                }

                                else
                                    j++;
                            }
                        }

                        else
                        {
                            // p2 lies in an area outside of the lines array, which means it is shadowed
                            // mark l1 as found, but don't add this spaceship to the scanned ones
                            found2 = true;
                        }
                    }

                    if (i >= lines.Count)
                        found1 = true;

                    if (j >= lines.Count)
                        found2 = true;
                }

                if (list.ContainsKey(ship))
                {

                    list[ship] = list[ship] / GameMaths.Distance2Points(p1, p2);

                }

            }


            return list;
        }

        public String Scan(double polarAngle, double scanAngle, int energy)
        {
            String bigoutput = "";

            //updateSelf();
            // cordinates of the spaceship
            ArrayList myList = new ArrayList();
            if (this._energy >= energy)
            {
                double x = this.XCoordinate;
                double y = this.YCoordinate;

                int r = this.Capacity / 100;

                Point a, b, c;

                int dist = (int)Math.Round(energy / Math.Abs(Math.Sin((Math.PI - scanAngle) / 2 * Math.PI / 180.0)), 0);

                double xA = Math.Round(x + r * Math.Cos(polarAngle * Math.PI / 180.0), 2);
                double yA = Math.Round(y + r * Math.Sin(polarAngle * Math.PI / 180.0), 2);
                // B and C are points of the scan extremes

                double xB = Math.Round(xA + dist * Math.Cos((polarAngle - scanAngle * 0.5) * Math.PI / 180.0), 2);
                double yB = Math.Round(yA + dist * Math.Sin((polarAngle - scanAngle * 0.5) * Math.PI / 180.0), 2);


                double xC = Math.Round(xA + dist * Math.Cos((polarAngle + scanAngle * 0.5) * Math.PI / 180.0), 2);
                double yC = Math.Round(yA + dist * Math.Sin((polarAngle + scanAngle * 0.5) * Math.PI / 180.0), 2);

                a = new Point(xA, yA);
                b = new Point(xB, yB);
                c = new Point(xC, yC);


                

                var dict = this.SpaceshipsInsideWithShadow(a, b, c);

                foreach (KeyValuePair<Spaceship, double> kvp in dict)
                {
                    Console.WriteLine(((Spaceship)(kvp.Key)).Name + " with ratio " + kvp.Value);
                    myList.Add(kvp.Key);
                }



                /*
                    * Printing as 2D array
                    *  
               */

                bool pretty_print = true;

                if (pretty_print)
                {
                    int xmax, ymax;

                    xmax = (int)(Math.Max(xC, Math.Max(xA, xB)) - Math.Min(xC, Math.Min(xA, xB)));
                    ymax = (int)(Math.Max(yC, Math.Max(yA, yB)) - Math.Min(yC, Math.Min(yA, yB)));
                    Console.WriteLine(xmax + " " + ymax);
                    int[,] toPrint = new int[xmax, ymax];

                    for (int i = 0; i < xmax; i++)
                        for (int j = 0; j < ymax; j++)
                            toPrint[i, j] = -1;

                    Point new_a = new Point(a.x - Math.Min(xC, Math.Min(xA, xB)), a.y - Math.Min(yC, Math.Min(yA, yB))),
                    new_b = new Point(b.x - Math.Min(xC, Math.Min(xA, xB)), b.y - Math.Min(yC, Math.Min(yA, yB))),
                    new_c = new Point(c.x - Math.Min(xC, Math.Min(xA, xB)), c.y - Math.Min(yC, Math.Min(yA, yB)));

                    foreach (KeyValuePair<Spaceship, double> kvp in dict)
                    {
                        Random random = new Random();
                        Spaceship sp = kvp.Key;
                        double ratio = kvp.Value;
                        int chnum = (int)Math.Ceiling(16 * ratio);
                        Console.WriteLine(sp.code.Substring(0, chnum));

                        String substring = sp.code.Substring(0, chnum);
                        Point translocated = new Point(sp.XCoordinate - Math.Min(xC, Math.Min(xA, xB)),
                                                        sp.YCoordinate - Math.Min(yC, Math.Min(yA, yB)));

                        if ((int)translocated.x < xmax && (int)translocated.x >= 0 && (int)translocated.y >= 0 && (int)translocated.y < ymax)
                            toPrint[(int)translocated.x, (int)translocated.y] = -1;

                        int stringIndex = 0;

                        int startPoint = -90;

                        Point inter1, inter2;
                        Point[] aux_point_array = GameMaths.CutsSpaceshipAtPoints(a, b, c, new Point(sp.XCoordinate, sp.YCoordinate), sp._capacity / 100);
                        if (aux_point_array != null)
                        {
                            inter1 = aux_point_array[0];
                            inter2 = aux_point_array[1];
                            inter1.x += 1;
                            inter1.y += 1;
                            if (GameMaths.IsInside(a, b, c, inter1))
                            {
                                inter1.x -= 1;
                                inter2.y -= 1;
                                startPoint = (int)((180.0 / Math.PI) * Math.Atan2(inter1.y - sp.XCoordinate, inter1.x - sp.YCoordinate));
                                Console.WriteLine(startPoint);
                            }
                            else
                            {
                                startPoint = (int)((180.0 / Math.PI) * Math.Atan2(inter2.y - sp.YCoordinate, inter2.x - sp.XCoordinate));
                                Console.WriteLine(startPoint);

                            }
                        }

                        for (double i = startPoint; i >= startPoint - 360; i -= 0.5)
                        {
                            int xcircle, ycircle;
                            xcircle = (int)(translocated.x + sp._capacity / 100 * Math.Cos(i * Math.PI / 180.0));
                            ycircle = (int)(translocated.y + sp._capacity / 100 * Math.Sin(i * Math.PI / 180.0));
                            if (GameMaths.IsInside(new_a, new_b, new_c, new Point(ycircle, xcircle)))

                                if (xcircle < xmax && xcircle >= 0 && ycircle < ymax && ycircle >= 0 && toPrint[xcircle, ycircle] == -1)

                                {
                                    toPrint[xcircle, ycircle] = int.Parse("" + substring[stringIndex % chnum]);
                                    stringIndex++;
                                }



                        }
                    

                        for (int i = ymax - 1; i >= 0; i--)
                        {
                            for (int j = 0; j < xmax; j++)
                            {
                                if (toPrint[j, i] <=9 && toPrint[j,i]>=0 && GameMaths.IsInside(new_a, new_b, new_c, new Point(j, i)))
                                    bigoutput+= (toPrint[j,i]).ToString();
                                else
                                    bigoutput += " ";
                            }

                            bigoutput += "\n";
                        }

                   
                    /*
                    * end of printing 
                    */
                    }

                    return bigoutput;
                }
            }

            return bigoutput;
                
        }

        public String Shoot(double polarAngle, double scanAngle, int energy, String attackCode)
        {
            string bigoutput = "";
            if (this.Energy < energy)
            {
                Console.WriteLine("You do not have enough energy to shoot");
                return null;
            }
            this.Energy -= energy;
            // cordinates of the spaceship
            // cordinates of the spaceship
            double x = this.XCoordinate;
            double y = this.YCoordinate;

            int r = this.Capacity / 100;

            ArrayList list = new ArrayList();

            int dist = (int)Math.Round(energy / Math.Abs(Math.Sin((Math.PI - scanAngle) / 2 * Math.PI / 180.0)), 0);

            double xA = Math.Round(x + r * Math.Cos(polarAngle * Math.PI / 180.0), 2);
            double yA = Math.Round(y + r * Math.Sin(polarAngle * Math.PI / 180.0), 2);
            // B and C are points of the scan extremes

            double xB = Math.Round(xA + dist * Math.Cos((polarAngle - scanAngle * 0.5) * Math.PI / 180.0), 2);
            double yB = Math.Round(yA + dist * Math.Sin((polarAngle - scanAngle * 0.5) * Math.PI / 180.0), 2);


            double xC = Math.Round(xA + dist * Math.Cos((polarAngle + scanAngle * 0.5) * Math.PI / 180.0), 2);
            double yC = Math.Round(yA + dist * Math.Sin((polarAngle + scanAngle * 0.5) * Math.PI / 180.0), 2);

            Point a, b, c;
            a = new Point(xA, yA);
            b = new Point(xB, yB);
            c = new Point(xC, yC);

            // get a list of the planets and spacehsips
            // check if they are inside the triangle ABC
            // or if the sides of the triangle cu the spaceships (as a circle)

            var dict = this.SpaceshipsInsideWithShadow(a, b, c);


            foreach (KeyValuePair<Spaceship, double> kvp in dict)
            {
                Console.WriteLine("You attacked " + ((Spaceship)(kvp.Key)).Name + " with ratio " + kvp.Value);
            }

            foreach (KeyValuePair<Spaceship, double> kvp in dict)
            {
                Console.WriteLine("You attacked " + ((Spaceship)(kvp.Key)).Name + " with ratio " + kvp.Value);
                Spaceship ship = kvp.Key;
                list.Add(ship);

                double x_coord = ship.XCoordinate;
                double y_coord = ship.YCoordinate;
                int other_radius = ship.Capacity / 100;

                double ratio;   // ratio of the energy with which we are attacking

                // if the spaceship is inside the shoot array'

                if (GameMaths.IsInside(a, b, c, new Point(x_coord, y_coord)))
                {

                    // B'C' is parallel to BC, we need the the slope and the other spaceship coordinates to find the equation and distance of B'C'

                    double slope = (yB - yC) / (xB - xC);

                    // find an equation of the type y = slope * x + b_
                    // aim: find out another point on B'C'

                    double b_ = y_coord - slope * x_coord;
                    double x_new_point = 1; // random, doesn't matter
                    double y_new_point = slope * x_new_point + b_;

                    Point b_prime = GameMaths.LineIntersection(a, b, new Point(x_coord, y_coord), new Point(x_new_point, y_new_point));
                    Point c_prime = GameMaths.LineIntersection(a, c, new Point(x_coord, y_coord), new Point(x_new_point, y_new_point));

                    double dist_b_c_prime = GameMaths.Distance2Points(b_prime, c_prime);

                    // multiple cases depending on where the spaceship lies on the line

                    if (GameMaths.DistanceToLine(GameMaths.LineEquation(a, b), x_coord, y_coord) < other_radius)  // other spaceship is closer to AB, and extends outside
                    {
                        double distance_to_ab = GameMaths.DistanceToLine(GameMaths.LineEquation(a, b), x_coord, y_coord);


                        double dist_to_add;
                        double b_o_dist = distance_to_ab * Math.Sin(((Math.PI - scanAngle) / 2) * Math.PI / 180.0);

                        if (dist_b_c_prime - b_o_dist >= other_radius)
                            dist_to_add = other_radius;
                        else
                            dist_to_add = dist_b_c_prime - b_o_dist;


                        ratio = (dist_to_add + b_o_dist) / dist_b_c_prime;
                    }

                    else if (GameMaths.DistanceToLine(GameMaths.LineEquation(a, c), x_coord, y_coord) < other_radius)
                    {
                        double distance_to_ac = GameMaths.DistanceToLine(GameMaths.LineEquation(a, c), x_coord, y_coord);


                        double dist_to_add;
                        double c_o_dist = distance_to_ac * Math.Sin(((Math.PI - scanAngle) / 2) * Math.PI / 180.0);

                        if (dist_b_c_prime - c_o_dist >= other_radius)
                            dist_to_add = other_radius;
                        else
                            dist_to_add = dist_b_c_prime - c_o_dist;


                        ratio = (dist_to_add + c_o_dist) / dist_b_c_prime;
                    }

                    else
                    {
                        // spaceship is on the line equally distributed
                        if (2 * other_radius < dist_b_c_prime)
                            ratio = (2 * other_radius) / dist_b_c_prime;
                        else
                            ratio = 1;

                    }

                    ratio += dict[ship];
                    ratio /= 2;
                    ratio *= (StringDistance.Compute(ship.Code, attackCode)/ 16.0 + 1)/2.0;
                    bigoutput += this.Attack(ship, (int)(Math.Round((energy * ratio), 0)));

                }


                // if the other spacehsip center is outside of the shoot triangle

                else if (GameMaths.ContainsSpaceship(a, b, c, new Point(x_coord, y_coord), other_radius))
                                      //   && this.UserId != ship.UserId)
                {
                    // B'C' is parallel to BC, we need the the slope and the other spaceship coordinates to find the equation and distance of B'C'

                    double slope = (yB - yC) / (xB - xC);

                    // find an equation of the type y = slope * x + b
                    // aim: find out another point on B'C'

                    double b_ = y_coord - slope * x_coord;
                    double x_new_point = 1; // random, doesn't matter
                    double y_new_point = slope * x_new_point + b_;

                    Point b_prime = GameMaths.LineIntersection(a, b, new Point(x_coord, y_coord), new Point(x_new_point, y_new_point));
                    Point c_prime = GameMaths.LineIntersection(a, c, new Point(x_coord, y_coord), new Point(x_new_point, y_new_point));

                    double dist_b_c_prime = GameMaths.Distance2Points(b_prime, c_prime);

                    ratio = (other_radius - GameMaths.SmallestDistance(a, b, c, new Point(x_coord, y_coord), other_radius)) / dist_b_c_prime;
                    
                    
                    ratio += dict[ship];
                    ratio /= 2;
                    ratio *= (StringDistance.Compute(ship.Code, attackCode)/ 16.0 + 1)/2.0;
                    bigoutput += this.Attack(ship, (int)(Math.Round((energy * ratio), 0)));

                }
            }

            return bigoutput;
        }

        public String Attack(Spaceship ship, int energy)
        {
           
            int beforeHealth = ship.Health;
            ship.Health -= energy;
            if (ship.Health <= 0)
            {
                ship.Health = 0;
                this.Capacity += ship.Capacity;  

            }

            ElasticSearchCommands.updateUser(ship);
            ElasticSearchCommands.updateUser(this);

            // update health and energy
            return "You just attacked the spaceship " + ship.Name + " . Damage caused: " +  (double)ship.Health/beforeHealth *100
            + "% !\n";

        }

        /*
        public void updateSelf()
        {
            Spaceship ship = ElasticSearchCommands.searchUsername(this.Name)[0];
            this.Health = ship.Health; 
            this.Energy = ship.Energy;
            this.XCoordinate = ship.XCoordinate;
            this.YCoordinate = ship.YCoordinate;
    
        }
    */
        public override string ToString()
        {
            string location = String.Format("({0},{1})", this.XCoordinate, this.YCoordinate);
            return "ID: " + this.id + " | Name: " + this.Name +
            " | HP: " + this.Health + " | Energy: " +
            this.Energy + " | Capacity: " + Capacity + " | Location: " + location;
        }


    }
}
