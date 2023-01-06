class LinearCostDistancePlaneCostTable : UndirectedGraphCostTable
{
    public LinearCostDistancePlaneCostTable(Point[] points) : base(new double[points.Length * (points.Length - 1) >> 1])
    {
        for (int i = 0; i < points.Length - 1; i++)
            for (int j = i + 1; j < points.Length; j++)
                costTable[(((points.Length << 1) - i - 3) * i >> 1) + j - 1] = Point.Distance(points[i], points[j]);
    }
}