using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoundGraphPlotter : MonoBehaviour
{
    [Header("Components")] 
    [SerializeField] private SpriteRenderer arrowPrefab;
    [SerializeField] private Color sumVectorColor;
    
    [Header("Settings")]
    [SerializeField] private float maxModuleSize;
    [SerializeField] private float minArrowHeight;
    [SerializeField] private float maxArrowHeight;
    
    private readonly List<ArrowGraph> graphs = new List<ArrowGraph>();

    public ArrowGraph SpawnGraph(Color color)
    {
        Transform thisTransform = transform;
        SpriteRenderer vectorRenderer = Instantiate(arrowPrefab, thisTransform.position, Quaternion.identity, thisTransform);
        return new ArrowGraph(vectorRenderer, color);
    }
    
    public void ChangeValue(ArrowGraph arrowGraph, float angle, float module)
    {
        arrowGraph.Angle = angle;
        arrowGraph.Value = module;

        Transform arrowTransform = arrowGraph.ArrowRenderer.transform;
        Vector3 arrowRotation = arrowTransform.rotation.eulerAngles;
        arrowRotation.z = arrowGraph.Angle;
        arrowTransform.rotation = Quaternion.Euler(arrowRotation);

        if (!graphs.Contains(arrowGraph))
            graphs.Add(arrowGraph);

        RecalculateMax();
    }

    private void RecalculateMax()
    {
        float maxModule = graphs.Max(v => v.Value);
        float minModule = graphs.Min(v => v.Value);

        foreach (ArrowGraph arrowGraph in graphs)
        {
            float height = CalculateHeight(minModule, maxModule, arrowGraph.Value);
            Vector2 arrowRendererSize = arrowGraph.ArrowRenderer.size;
            arrowRendererSize.y = height;
            arrowGraph.ArrowRenderer.size = arrowRendererSize;
        }
    }

    private float CalculateHeight(float minModule, float maxModule, float value)
    {
        float t = Mathf.InverseLerp(minModule, maxModule, value);
        float height = Mathf.Lerp(minArrowHeight, maxArrowHeight, t);
        return height;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 graphPosition = transform.position;
        Gizmos.DrawLine(graphPosition, graphPosition + Vector3.right * maxModuleSize);
    }
}