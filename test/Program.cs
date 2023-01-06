Point[] points = {
    new(0, 0),
    new(1, 0),
    new(1, 2),
    new(2, 0)
};
HeldKarp heldKarp = HeldKarp.FromLinearCostDistancePlane(points);
int[] expected = { 0, 1, 3, 2 };
Console.WriteLine("Expected: {0}", string.Join(" -> ", expected));
int[] actual = await heldKarp.Solve();
Console.WriteLine("Actual  : {0}", string.Join(" -> ", actual));
Console.WriteLine(Enumerable.SequenceEqual(expected, actual) ? "Passed" : "Failed");

Console.WriteLine();

points = new Point[] {
    new(0, 0),
    new(1, 0),
    new(1, 2),
    new(2, 0),
    new(1, -2)
};
heldKarp = HeldKarp.FromLinearCostDistancePlane(points);
expected = new int[] { 2, 0, 1, 3, 4 };
Console.WriteLine("Expected: {0}", string.Join(" -> ", expected));
actual = await heldKarp.Solve();
Console.WriteLine("Actual  : {0}", string.Join(" -> ", actual));
Console.WriteLine(Enumerable.SequenceEqual(expected, actual) ? "Passed" : "Failed");

Console.WriteLine();

points = new Point[] {
    new(0, 0),
    new(1, 0),
    new(1, 2),
    new(2, 0),
    new(1, -2)
};
heldKarp = HeldKarp.FromLinearCostDistancePlane(points, 4);
expected = new int[] { 4, 0, 1, 3, 2 };
Console.WriteLine("Expected: {0}", string.Join(" -> ", expected));
actual = await heldKarp.Solve();
Console.WriteLine("Actual  : {0}", string.Join(" -> ", actual));
Console.WriteLine(Enumerable.SequenceEqual(expected, actual) ? "Passed" : "Failed");

Console.WriteLine();

points = new Point[] {
    new(0, 0),
    new(1, 0),
    new(1, 2),
    new(2, 0),
    new(1, -2)
};
heldKarp = HeldKarp.FromLinearCostDistancePlaneCircular(points);
expected = new int[] { 0, 1, 2, 3, 4 };
Console.WriteLine("Expected: {0}", string.Join(" -> ", expected));
actual = await heldKarp.Solve();
Console.WriteLine("Actual  : {0}", string.Join(" -> ", actual));
Console.WriteLine(Enumerable.SequenceEqual(expected, actual) ? "Passed" : "Failed");

Console.WriteLine();

DirectedGraphCostTable costTable = new(new double[] {
    1, 5, 4,
    1, 4, 1,
    5, 4, 5,
    4, 1, double.PositiveInfinity
});
heldKarp = HeldKarp.FromDirectedGraph(costTable);
expected = new int[] { 2, 0, 1, 3 };
Console.WriteLine("Expected: {0}", string.Join(" -> ", expected));
actual = await heldKarp.Solve();
Console.WriteLine("Actual  : {0}", string.Join(" -> ", actual));
Console.WriteLine(Enumerable.SequenceEqual(expected, actual) ? "Passed" : "Failed");

Console.WriteLine();

costTable = new(new double[] {
    1, 5, 4, 5,
    1, 4, 1, 4,
    5, 4, 5, 16,
    4, 1, double.PositiveInfinity, double.PositiveInfinity,
    5, 4, 16, 5
});
heldKarp = HeldKarp.FromDirectedGraph(costTable);
expected = new int[] { 2, 3, 1, 0, 4 };
Console.WriteLine("Expected: {0}", string.Join(" -> ", expected));
actual = await heldKarp.Solve();
Console.WriteLine("Actual  : {0}", string.Join(" -> ", actual));
Console.WriteLine(Enumerable.SequenceEqual(expected, actual) ? "Passed" : "Failed");

Console.WriteLine();

costTable = new(new double[] {
    1, 5, 4, 5,
    1, 4, 1, 4,
    5, 4, 5, 16,
    4, 1, double.PositiveInfinity, double.PositiveInfinity,
    5, 4, 16, 5
});
heldKarp = HeldKarp.FromDirectedGraph(costTable, 4);
expected = new int[] { 4, 3, 1, 0, 2 };
Console.WriteLine("Expected: {0}", string.Join(" -> ", expected));
actual = await heldKarp.Solve();
Console.WriteLine("Actual  : {0}", string.Join(" -> ", actual));
Console.WriteLine(Enumerable.SequenceEqual(expected, actual) ? "Passed" : "Failed");

Console.WriteLine();

costTable = new(new double[] {
    1, 5, 4, 5,
    1, 4, 1, 4,
    5, 4, 5, 16,
    4, 1, double.PositiveInfinity, double.PositiveInfinity,
    5, 4, 16, 5
});
heldKarp = HeldKarp.FromDirectedGraphCircular(costTable);
expected = new int[] { 0, 2, 3, 1, 4 };
Console.WriteLine("Expected: {0}", string.Join(" -> ", expected));
actual = await heldKarp.Solve();
Console.WriteLine("Actual  : {0}", string.Join(" -> ", actual));
Console.WriteLine(Enumerable.SequenceEqual(expected, actual) ? "Passed" : "Failed");