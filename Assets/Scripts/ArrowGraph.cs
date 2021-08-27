using UnityEngine;

public class ArrowGraph
{
    public SpriteRenderer ArrowRenderer { get; }
    
    public float Value { get; set; }

    public float Angle
    {
        get => angle;
        set => angle = NormalizeAngle(value);
    }

    private float angle;
    
    public ArrowGraph(SpriteRenderer arrowRenderer, Color color)
    {
        ArrowRenderer = arrowRenderer;
        arrowRenderer.color = color;
    }
    
    private static float NormalizeAngle(float value)
    {
        float divisions;
        
        if (value > 360f)
        {
            divisions = Mathf.Round(360f / value);
            return value - 360f * divisions;
        }

        if (value >= 0f) return value;
        
        divisions = Mathf.Abs(Mathf.Round(360f / value));
        return 360f * divisions - value;
    }
}