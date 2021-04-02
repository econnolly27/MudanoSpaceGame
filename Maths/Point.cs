using System;
namespace Maths
{
    public class Point : IComparable<Point>
    {
        public double x;
        public double y;

        public Point(double x, double y)
        {
            this.x = Math.Round(x, 2);
            this.y = Math.Round(y, 2);
        }

        // compare the points as in projections on the ox axis then if they are the same, then according to the y axis
        // helps in dealing with points situated on the same line

        public int CompareTo(Point other)
        {
            if (this.x == other.x)
                return this.y.CompareTo(other.y);
            else
                return this.x.CompareTo(other.x);
        }

        public override string ToString()
        {
            return this.x + " " + this.y + " ";

        }
    }
}
