class Place
{
    public string name;
    public List<string> clusterPlaces = new();

    public Place(string name)
    {
        this.name = name;
    }

    public override string ToString()
    {
        string s = name;
        if (clusterPlaces.Count > 0)
            s += clusterPlaces.Select(p => "\n- " + p).Aggregate((a, b) => a + b);
        return s;
    }
}