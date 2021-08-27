using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    public LineRenderer LineRenderer { get; }
    public List<float> Values { get; }
        
    public Graph(LineRenderer lineRenderer, Color graphColor)
    {
        LineRenderer = lineRenderer;
        lineRenderer.startColor = graphColor;
        lineRenderer.endColor = graphColor;
        Values = new List<float>();
    }
}