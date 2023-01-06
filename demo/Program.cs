List<Point> pointList = new();
Dictionary<Point, Place> dict = new();
double clusterRange = 0;
Console.Write("Cluster Range (Default = 0): ");
string? line = Console.ReadLine();
if (line != null && line.Length > 0)
    clusterRange = Math.Pow(double.Parse(line), 2);
Console.WriteLine("Coordinates of Points (Format: <x> <y> [name], separated into lines):");
while ((line = Console.ReadLine()) != null && line.Length > 0) {
    string[] coors = line.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    double x = double.Parse(coors[0]);
    double y = double.Parse(coors[1]);
    string? name = null;
    if (coors.Length > 2)
        name = coors[2];
    Point point = new(x, y);
    if (dict.ContainsKey(point)) {
        if (name != null)
            dict[point].name += ", " + name;
    } else {
        name = "(" + x + ", " + y + ")" + (name == null ? "" : ": " + name);
        Point? cluster = null;
        foreach (Point p in pointList)
            if (Point.Distance(point, p) <= clusterRange) {
                cluster = p;
                break;
            }
        if (cluster == null) {
            pointList.Add(point);
        } else
            dict[cluster].clusterPlaces.Add(name);
        dict.Add(point, new(name));
    }
}
Point[] points = pointList.ToArray();
Console.WriteLine("Cluster Count: " + points.Length);
if (points.Length < 3) {
    Console.WriteLine("Cluster Count should be >= 3.");
    return;
}
if (points.Length > 33) {
    Console.WriteLine("Cluster Count should be <= 33.");
    return;
}

HeldKarp heldKarp = HeldKarp.FromLinearCostDistancePlane(points);

Task<int[]> t = heldKarp.Solve();
using (ProgressBar progressBar = new())
    while (!t.IsCompleted) {
        progressBar.Report(heldKarp.GetProgress());
        Thread.Sleep(500);
    }
foreach (int i in await t)
    Console.WriteLine(dict[points[i]]);
