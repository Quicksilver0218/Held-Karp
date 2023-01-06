struct Point
{
    public readonly double x, y;

    public Point(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    public static double Distance(Point a, Point b)
    {
        return Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Point) {
            Point p = (Point) obj;
            return p.x == x && p.y == y;
        }
        return false;
    }
}