class HeldKarp
{
    readonly int placeCount;
    readonly CostTable costTable;
    int[]? pathTable;
    long fillCount;
    public readonly bool circular;
    public readonly int startPoint;
    public int[]? Solution { get; private set; }

    HeldKarp(int placeCount, CostTable costTable, bool circular, int startPoint)
    {
        this.placeCount = placeCount;
        this.costTable = costTable;
        this.circular = circular;
        this.startPoint = startPoint;
    }

    public static HeldKarp FromLinearCostDistancePlaneCircular(Point[] points)
    {
        return new HeldKarp(points.Length - 1, new LinearCostDistancePlaneCostTable(points), true, -1);
    }

    public static HeldKarp FromLinearCostDistancePlane(Point[] points, int startPoint = -1)
    {
        if (startPoint >= 0) {
            if (startPoint >= points.Length)
                throw new Exception(string.Format("startPoint ({0}) >= place count ({1})", startPoint, points.Length));
            List<Point> pointList = new(points);
            pointList.RemoveAt(startPoint);
            pointList.Add(points[startPoint]);
            return new HeldKarp(points.Length - 1, new LinearCostDistancePlaneCostTable(pointList.ToArray()), false, startPoint);
        }
        return new HeldKarp(points.Length, new LinearCostDistancePlaneCostTable(points), false, -1);
    }

    public static HeldKarp FromUndirectedGraphCircular(UndirectedGraphCostTable costTable)
    {
        return new HeldKarp(costTable.placeCount - 1, costTable, true, -1);
    }

    public static HeldKarp FromUndirectedGraph(UndirectedGraphCostTable costTable, int startPoint = -1)
    {
        if (startPoint >= 0) {
            if (startPoint >= costTable.placeCount)
                throw new Exception(string.Format("startPoint ({0}) >= place count ({1})", startPoint, costTable.placeCount));
            UndirectedGraphCostTable newCostTable = UndirectedGraphCostTable.ShiftPlaceToEnd(costTable, startPoint);
            return new HeldKarp(costTable.placeCount - 1, newCostTable, false, startPoint);
        }
        return new HeldKarp(costTable.placeCount, costTable, false, -1);
    }

    public static HeldKarp FromDirectedGraphCircular(DirectedGraphCostTable costTable)
    {
        return new HeldKarp(costTable.placeCount - 1, costTable, true, -1);
    }

    public static HeldKarp FromDirectedGraph(DirectedGraphCostTable costTable, int startPoint = -1)
    {
        if (startPoint >= 0) {
            DirectedGraphCostTable newCostTable = DirectedGraphCostTable.ShiftPlaceToEnd(costTable, startPoint);
            if (startPoint >= costTable.placeCount)
                throw new Exception(string.Format("startPoint ({0}) >= place count ({1})", startPoint, costTable.placeCount));
            return new HeldKarp(costTable.placeCount - 1, newCostTable, false, startPoint);
        }
        return new HeldKarp(costTable.placeCount, costTable, false, -1);
    }

    public async Task<int[]> Solve()
    {
        await Task.Run(() =>
        {
            int resultStart = 0;
            int resultEnd = 0;
            double minLength = 0;
            int between;
            if (costTable is DirectedGraphCostTable)
                pathTable = new int[placeCount * (placeCount - 1) * ((1 << placeCount - 2) - placeCount + 1)];
            else
                pathTable = new int[((1 << placeCount - 1) - placeCount - 1) * (placeCount - 2)];
            Parallel.For(0, pathTable.LongLength, i => pathTable[i] = -1);
            if (costTable is DirectedGraphCostTable) {
                for (int i = 0; i < placeCount; i++)
                    for (int j = placeCount - 1; j >= 0; j--)
                        if (i != j) {
                            between = ~(-1 << placeCount) ^ 1 << i ^ 1 << j;
                            double length = circular ? costTable.GetCost(placeCount, i) + costTable.GetCost(j, placeCount) :
                                    (startPoint >= 0 ? costTable.GetCost(placeCount, i) : 0);
                            int start = i, end = j;
                            while (between != 0) {
                                int next = DirectedShortestPathNextPlace(start, end, between);
                                length += costTable.GetCost(start, next);
                                start = next;
                                between ^= 1 << next;
                            }
                            length += costTable.GetCost(start, end);
                            if (resultStart == resultEnd || length < minLength) {
                                resultStart = i;
                                resultEnd = j;
                                minLength = length;
                            }
                        }
                LinkedList<int> path = new();
                path.AddLast(resultStart);
                between = ~(-1 << placeCount) ^ 1 << resultStart ^ 1 << resultEnd;
                while (between != 0) {
                    int next = DirectedShortestPathNextPlace(resultStart, resultEnd, between);
                    resultStart = next;
                    path.AddLast(next);
                    between ^= 1 << next;
                }
                path.AddLast(resultEnd);
                if (circular)
                    path.AddLast(placeCount);
                else if (startPoint >= 0) {
                    for (LinkedListNode<int>? node = path.First; node != null; node = node.Next)
                        if (node.Value >= startPoint)
                            node.Value++;
                    path.AddFirst(startPoint);
                }
                Solution = path.ToArray();
            } else {
                for (int i = 0; i < placeCount - 1; i++)
                    for (int j = placeCount - 1; j > i; j--) {
                        between = ~(-1 << placeCount) ^ 1 << i ^ 1 << j;
                        double length = circular ? costTable.GetCost(i, placeCount) + costTable.GetCost(j, placeCount) :
                                (startPoint >= 0 ? Math.Min(costTable.GetCost(i, placeCount), costTable.GetCost(j, placeCount)) : 0);
                        int start = i, end = j;
                        while (between != 0) {
                            (int next, bool r) = UndirectedShortestPathNextPlace(start, end, between);
                            if (r) {
                                length += costTable.GetCost(end, next);
                                end = next;
                            } else {
                                length += costTable.GetCost(start, next);
                                start = next;
                            }
                            between ^= 1 << next;
                        }
                        length += costTable.GetCost(start, end);
                        if (resultStart == resultEnd || length < minLength) {
                            resultStart = i;
                            resultEnd = j;
                            minLength = length;
                        }
                    }
                LinkedList<int> head = new();
                head.AddLast(resultStart);
                LinkedList<int> tail = new();
                tail.AddLast(resultEnd);
                between = ~(-1 << placeCount) ^ 1 << resultStart ^ 1 << resultEnd;
                while (between != 0) {
                    (int next, bool r) = UndirectedShortestPathNextPlace(resultStart, resultEnd, between);
                    if (r) {
                        resultEnd = next;
                        tail.AddFirst(next);
                    } else {
                        resultStart = next;
                        head.AddLast(next);
                    }
                    between ^= 1 << next;
                }
                LinkedList<int> path = new(head.Concat(tail));
                if (circular)
                    path.AddLast(placeCount);
                else if (startPoint >= 0) {
                    LinkedList<int>.Enumerator enumerator = path.GetEnumerator();
                    for (LinkedListNode<int>? node = path.First; node != null; node = node.Next)
                        if (node.Value >= startPoint)
                            node.Value++;
                    if (costTable.GetCost(head.First(), placeCount) <= costTable.GetCost(head.Last(), placeCount))
                        path.AddFirst(startPoint);
                    else {
                        path.AddLast(startPoint);
                        path = new(path.Reverse());
                    }
                }
                Solution = path.ToArray();
            }
        });
        return Solution!;
    }

    public double GetProgress()
    {
        return pathTable == null ? 0 : (double) fillCount / pathTable.LongLength;
    }

    int DirectedShortestPathNextPlace(int start, int end, int between) {
        if (between == 0)
            return end;
        byte bitCount = CountSetBits(between);
        if (bitCount == 1) {
            int i;
            for (i = 0; between != 1; i++)
                between >>= 1;
            return i;
        }
        long index = start * placeCount + end;
        index -= index / (placeCount + 1) + 1;
        int larger, smaller;
        if (start > end) {
            larger = start;
            smaller = end;
        } else {
            larger = end;
            smaller = start;
        }
        int index2 = (((between & -1 << larger) >> 1 | between & ~(-1 << larger)) & -1 << smaller) >> 1 | between & ~(-1 << smaller);
        index2 -= (int) Math.Log2(index2) + 2;
        index = index * ((1 << placeCount - 2) - placeCount + 1) + index2;
        if (pathTable![index] != -1)
            return pathTable[index];
        int[] betweenPoints = new int[bitCount];
        int count = 0;
        int b = between;
        for (int i = 0; b != 0; i++) {
            if ((b & 1) == 1) {
                betweenPoints[count] = i;
                count++;
            }
            b >>= 1;
        }
        int? result = null;
        double minLength = 0;
        for (int i = 0; i < betweenPoints.Length; i++) {
            b = between ^ 1 << betweenPoints[i];
            double length = costTable.GetCost(start, betweenPoints[i]);
            int s = betweenPoints[i], e = end;
            while (b != 0) {
                int next = DirectedShortestPathNextPlace(s, e, b);
                length += costTable.GetCost(s, next);
                s = next;
                b ^= 1 << next;
            }
            length += costTable.GetCost(s, e);
            if (result == null || length < minLength) {
                result = betweenPoints[i];
                minLength = length;
            }
        }
        WritePathTable(index, (int) result!);
        return (int) result!;
    }

    (int, bool) UndirectedShortestPathNextPlace(int start, int end, int between) {
        if (between == 0)
            return (end, false);
        byte bitCount = CountSetBits(between);
        if (bitCount == 1) {
            int i;
            for (i = 0; between != 1; i++)
                between >>= 1;
            return (i, false);
        }
        bool reversed = start > end;
        if (reversed) {
            int temp = start;
            start = end;
            end = temp;
        }
        long index;
        int size = (1 << placeCount - 2) - placeCount + 1;
        if (end >= placeCount - 2) {
            index = (1 << placeCount - 3) * (placeCount - 4) + 1;
            if (end == placeCount - 2)
                index += ((1 << placeCount - 3) - 1) * (start + 1);
            else
                index += ((1 << placeCount - 3) - 1) * (placeCount - 2) + size * (start + 1);
        } else
            index = (1 << end - 1) * (end + start - 1) + 1;
        int index2 = (((between & -1 << end) >> 1 | between & ~(-1 << end)) & -1 << start) >> 1 | between & ~(-1 << start);
        index2 -= (int) Math.Log2(index2) + 2;
        index += index2 - size;
        if (pathTable![index] != -1)
            return (pathTable[index], reversed);
        int[] betweenPoints = new int[bitCount];
        int count = 0;
        int b = between;
        for (int i = 0; b != 0; i++) {
            if ((b & 1) == 1) {
                betweenPoints[count] = i;
                count++;
            }
            b >>= 1;
        }
        int? result = null;
        double minLength = 0;
        for (int i = 0; i < betweenPoints.Length; i++) {
            b = between ^ 1 << betweenPoints[i];
            double length = costTable.GetCost(start, betweenPoints[i]);
            int s = betweenPoints[i], e = end;
            while (b != 0) {
                (int next, bool r) = UndirectedShortestPathNextPlace(s, e, b);
                if (r) {
                    length += costTable.GetCost(e, next);
                    e = next;
                } else {
                    length += costTable.GetCost(s, next);
                    s = next;
                }
                b ^= 1 << next;
            }
            length += costTable.GetCost(s, e);
            if (result == null || length < minLength) {
                result = betweenPoints[i];
                minLength = length;
            }
        }
        WritePathTable(index, (int) result!);
        return ((int) result!, reversed);
    }

    void WritePathTable(long index, int next)
    {
        pathTable![index] = next;
        fillCount++;
    }

    byte CountSetBits(int i)
    {
        byte c;
        for (c = 0; i != 0; c++)
            i &= i - 1;
        return c;
    }
}