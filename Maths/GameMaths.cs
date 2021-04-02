﻿using System;

namespace Maths
{
    public static class GameMaths
    {
    


        // the angles will be integers representing the the angle in pi representation --> 30 = 30 pi 
        public static Point[] CutsSpaceshipAtPoints( Point a, Point b, Point c,
                                                Point origin, int radius)
        {
            // make a, b be the points of the real intersection
            if (!LineIntersectsCircleIn2Points(LineEquation(a, b), origin.x, origin.y, radius))
            {

            if (LineIntersectsCircleIn2Points(LineEquation(a, c), origin.x, origin.y, radius))
                b = c;
             if (LineIntersectsCircleIn2Points(LineEquation(c, b), origin.x, origin.y, radius))
            {
                a = b;
                b = c;
            }

            else return null; }
                   
            double dx, dy, A, B, C, det, t;

            dx = b.x - a.x;
            dy = b.y - a.y;

            A = dx * dx + dy * dy;
            B = 2 * (dx * (a.x - origin.x) + dy * (a.y - origin.y));
            C = (a.x - origin.x) * (a.x - origin.x) +
                (a.y - origin.y) * (a.y - origin.y) -
                radius * radius;

            det = B * B - 4 * A * C;
            t = (float)((-B + Math.Sqrt(det)) / (2 * A));
            Point inter1 = new Point(a.x + t * dx, a.y + t * dy);
            t = (float)((-B - Math.Sqrt(det)) / (2 * A));
            Point inter2 =  new Point(a.x + t * dx, a.y + t * dy);
            
            return new Point[] { inter1, inter2 };
        }


        public static bool ContainsSpaceship(Point a, Point b, Point c,
                                                Point origin, int radius)
        {
            if (LineIntersectsCircle(LineEquation(a, b), origin.x, origin.y, radius) ||
                LineIntersectsCircle(LineEquation(a, c), origin.x, origin.y, radius) ||
                LineIntersectsCircle(LineEquation(c, b), origin.x, origin.y, radius))
                return true;
            return false;
        }

        public static double NormalizeAngle(double angle)
        {
            if (angle < 0)
                return Math.Abs(angle % 360);
            else
                return angle % 360;
        }

        public static double SmallestDistance(Point a, Point b, Point c,
                                                Point origin, int radius)

        {
            double dist = DistanceToLine(LineEquation(a, b), origin.x, origin.y);
            if (dist <= radius)
                return dist;
            dist = DistanceToLine(LineEquation(a, c), origin.x, origin.y);
            if (dist <= radius)
                return dist;

            dist = DistanceToLine(LineEquation(c, b), origin.x, origin.y);
            if (dist <= radius)
                return dist;
            return 0;
        }

        public static double[] LineEquation(Point a, Point b)
        {
            return new double[] { b.y - a.y, a.x - b.x, b.x * (a.y - b.y) - b.y * (a.x - b.x) };
        }


        public static bool LineIntersectsCircle(double[] line,
                  double x, double y, int radius)
        {
            double a = line[0];
            double b = line[1];
            double c = line[2];

            // Finding the distance of line from center. 
            double dist = Math.Abs(a * x + b * y + c) / Math.Sqrt(a * a + b * b);
            // Checking if the distance is less than,  
            // greater than or equal to radius. 
            if (radius >= dist)
                return true;
            return false;
        }

        public static bool LineIntersectsCircleIn2Points(double[] line,
                  double x, double y, int radius)
        {
            double a = line[0];
            double b = line[1];
            double c = line[2];

            // Finding the distance of line from center. 
            double dist = Math.Abs(a * x + b * y + c) / Math.Sqrt(a * a + b * b);
            // Checking if the distance is less than,  
            // greater than or equal to radius. 
            if (radius > dist)
                return true;
            return false;
        }


        public static double Area(Point a, Point b, Point c)
        {
            return Math.Abs((a.x * (b.y - c.y) +
                             b.x * (c.y - a.y) +
                             c.x * (a.y - b.y)) / 2.0);
        }

        public static bool IsInside(Point a, Point b, Point c, Point p)
        {
            /* Calculate area of triangle ABC */
            double A = Area(a,b,c);

            /* Calculate area of triangle PBC */
            double A1 = Area(p,b,c);

            /* Calculate area of triangle PAC */
            double A2 = Area(a,p,c);

            /* Calculate area of triangle PAB */
            double A3 = Area(a,b,p);

            /* Check if sum of A1, A2 and A3 is same as A */
            return (Math.Round(A, 1) == Math.Round((A1 + A2 + A3), 1));
        }

        public static double DistanceToLine(double[] line,
                  double x, double y)
        {
            double a = line[0];
            double b = line[1];
            double c = line[2];

            // Finding the distance of line from center. 
            return Math.Abs(a * x + b * y + c) / Math.Sqrt(a * a + b * b);

        }

        public static Point LineIntersection(Point a, Point b, Point c, Point d)
        {
            // Line AB represented as a1x + b1y = c1  
            double a1 = b.y- a.y;
            double b1 = a.x - b.x;
            double c1 = a1 * a.x + b1 * a.y;

            // Line CD represented as a2x + b2y = c2  
            double a2 = d.y - c.y;
            double b2 = c.x - d.x;
            double c2 = a2 * c.x + b2 * c.y;

            double determinant = a1 * b2 - a2 * b1;
          

            if (determinant == 0)
            {
                // The lines are parallel. This is simplified  
                // by returning a pair of FLT_MAX  
                return null;
            }
            else
            {
                double x = (b2 * c1 - b1 * c2) / determinant;
                double y = (a1 * c2 - a2 * c1) / determinant;
                return new Point(x,y);
            }
        }

        public static double Distance2Points(Point a, Point b)
        {
            return Math.Sqrt((Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2)));
        }

        // returns the points of tangent to a circle through an external point (a); usually there are 2 of these
        // a - external point, o - center of the circle, r - radius

        public static Point[] PointsOfTangentToCircle (Point a, Point o , int r)
        {
            double m = a.x - o.x;
            double n = a.y - o.y;
            return new Point[]
            {
                new Point (o.x + (m * r * r - r * n * Math.Sqrt( m * m + n * n - r * r))/(m*m+n*n),
                           o.y + (n * r * r + r * m * Math.Sqrt( m * m + n * n - r * r))/(m*m+n*n)),

                new Point (o.x + (m * r * r + r * n * Math.Sqrt( m * m + n * n - r * r))/(m*m+n*n),
                           o.y + (n * r * r - r * m * Math.Sqrt( m * m + n * n - r * r))/(m*m+n*n))};

        }


    }
}
