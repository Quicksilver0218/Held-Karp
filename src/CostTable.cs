abstract class CostTable
{
    protected readonly double[] costTable;
    public readonly int placeCount;

    protected CostTable(double[] costTable, int placeCount)
    {
        this.costTable = costTable;
        this.placeCount = placeCount;
    }

    public abstract double GetCost(int start, int end);
}