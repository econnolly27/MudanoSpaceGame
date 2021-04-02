namespace Maths
{
    public class Line
    {
        public Point a;
        public Point b;
        public Line(Point a, Point b)
        {
            // place the points in order, lowest first (see compareTo function in Point class)

            if (a.CompareTo(b) < 0)
            {
                this.a = a;
                this.b = b;
            }

            else
            {
                this.a = b;
                this.b = a;
            }

        }

        public override string ToString()
        {
            return this.a.x + " " + this.a.y + " / " + this.b.x + " " + " " + this.b.y + '\n';

        }

    }
}
