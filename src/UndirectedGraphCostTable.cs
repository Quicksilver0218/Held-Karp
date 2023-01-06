class UndirectedGraphCostTable : CostTable
{
    public UndirectedGraphCostTable(double[] costTable) : base(costTable, (int) Math.Sqrt(costTable.Length * 2) + 1)
    {
        if (costTable.Length != placeCount * (placeCount - 1) >> 1)
            throw new Exception("Invalid cost table.");
    }

    public override double GetCost(int pId1, int pId2)
    {
        if (pId1 > pId2) {
            int temp = pId1;
            pId1 = pId2;
            pId2 = temp;
        }
        return costTable[(((placeCount << 1) - pId1 - 3) * pId1 >> 1) + pId2 - 1];
    }

    public static UndirectedGraphCostTable ShiftPlaceToEnd(UndirectedGraphCostTable costTable, int pId)
    {
        if (pId < 0 || pId >= costTable.placeCount)
            throw new Exception("Place ID out of bounds.");
        double[] table = new double[costTable.costTable.Length];
        for (int i = 0; i < costTable.placeCount - 1; i++)
            for (int j = i + 1; j < costTable.placeCount; j++) {
                int index = (((costTable.placeCount << 1) - i - 3) * i >> 1) + j - 1;
                if (i < pId)
                    if (j < pId)
                        table[index] = costTable.costTable[index];
                    else if (j == costTable.placeCount - 1)
                        table[index] = costTable.costTable[index - i];
                    else
                        table[index] = costTable.costTable[index + 1];
                else if (j < costTable.placeCount - 1)
                    table[index] = costTable.costTable[index + costTable.placeCount - i];
                else
                    table[index] = costTable.costTable[(((costTable.placeCount << 1) - pId - 3) * pId >> 1) + i];
            }
        return new(table);
    }
}