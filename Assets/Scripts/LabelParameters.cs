public readonly struct LabelParameters
{
    public float Minimum { get; }
    public float Maximum { get; }
    public int Divisions { get; }

    public LabelParameters(float minimum, float maximum, int divisions)
    {
        Minimum = minimum;
        Maximum = maximum;
        Divisions = divisions;
    }
}