class DirectedGraphCostTable : CostTable
{
    public DirectedGraphCostTable(double[] costTable) : base(costTable, (int) Math.Sqrt(costTable.Length) + 1)
    {
        if (placeCount * (placeCount - 1) != costTable.Length)
            throw new Exception("Invalid cost table.");
    }

    public override double GetCost(int start, int end)
    {
        int i = start * placeCount + end;
        return costTable[i - i / (placeCount + 1) - 1];
    }

    public static DirectedGraphCostTable ShiftPlaceToEnd(DirectedGraphCostTable costTable, int pId)
    {
        if (pId < 0 || pId >= costTable.placeCount)
            throw new Exception("Place ID out of bounds.");
        double[] table = new double[costTable.costTable.Length];
        for (int i = 0; i < costTable.placeCount; i++)
            for (int j = 0; j < costTable.placeCount - 1; j++) {
                int index = i * (costTable.placeCount - 1) + j;
                if (i < pId)
                    if (j < pId - 1)
                        table[index] = costTable.costTable[index];
                    else if (j == costTable.placeCount - 2)
                        table[index] = costTable.costTable[index - costTable.placeCount + pId];
                    else
                        table[index] = costTable.costTable[index + 1];
                else if (i == costTable.placeCount - 1)
                    table[index] = costTable.costTable[pId * (costTable.placeCount - 1) + j];
                else if (j < pId)
                    table[index] = costTable.costTable[index + costTable.placeCount - 1];
                else if (j == costTable.placeCount - 2)
                    table[index] = costTable.costTable[index + pId];
                else
                    table[index] = costTable.costTable[index + costTable.placeCount];
            }
        return new(table);
    }
}