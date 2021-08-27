using UnityEngine;

public class EngineCalculator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GraphPlotter graphPlotter;
    [SerializeField] private RoundGraphPlotter roundGraphPlotter;
    [SerializeField] private LineRenderer baseLineRenderer;

    [Header("Settings")]
    [SerializeField] private float timeStamp = 0.2f;
    [SerializeField] private float maxIntensity = 1f;
    [SerializeField] private float angularVelocity = 1f;
    
    [Header("Colors")]
    [SerializeField] private Color sumVectorColor;
    [SerializeField] private Color aGraphColor;
    [SerializeField] private Color bGraphColor;
    [SerializeField] private Color cGraphColor;
    
    [Header("Time")]
    [SerializeField]private float time = 0.5f;

    private ArrowGraph summedVectorGraph;
    private ArrowGraph aArrow;
    private ArrowGraph bArrow;
    private ArrowGraph cArrow;
    
    private Graph aGraph;
    private Graph bGraph;
    private Graph cGraph;
    
    private float timer = 0f;
    private float currentTime = 0f;

    private void Awake()
    {
        summedVectorGraph = roundGraphPlotter.SpawnGraph(sumVectorColor);

        aGraph = new Graph(Instantiate(baseLineRenderer), aGraphColor);
        bGraph = new Graph(Instantiate(baseLineRenderer), bGraphColor);
        cGraph = new Graph(Instantiate(baseLineRenderer), cGraphColor);
        
        aArrow = roundGraphPlotter.SpawnGraph(aGraphColor);
        bArrow = roundGraphPlotter.SpawnGraph(bGraphColor);
        cArrow = roundGraphPlotter.SpawnGraph(cGraphColor);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer < time) return;
        timer = 0f;

        float wt = angularVelocity * currentTime;
        float aAngle = wt;
        float bAngle = wt - 2f * Mathf.PI / 3f;
        float cAngle = wt + 2f * Mathf.PI / 3f;
        
        float aH = 1.5f * maxIntensity * Mathf.Sin(aAngle);
        float bH = 1.5f * maxIntensity * Mathf.Sin(bAngle);
        float cH = 1.5f * maxIntensity * Mathf.Sin(cAngle);
        
        graphPlotter.AddValue(aH, currentTime, timeStamp, aGraph);
        graphPlotter.AddValue(bH, currentTime, timeStamp, bGraph);
        graphPlotter.AddValue(cH, currentTime, timeStamp, cGraph);

        roundGraphPlotter.ChangeValue(aArrow, aAngle * Mathf.Rad2Deg, aH);
        roundGraphPlotter.ChangeValue(bArrow, bAngle * Mathf.Rad2Deg, bH);
        roundGraphPlotter.ChangeValue(cArrow, cAngle * Mathf.Rad2Deg, cH);
        RecalculateSummedVector(out float sumModule, out float sumAngle, aArrow, bArrow, cArrow);
        
        roundGraphPlotter.ChangeValue(summedVectorGraph, sumAngle + wt * Mathf.Rad2Deg, sumModule);
        
        currentTime += timeStamp;
    }

    private static void RecalculateSummedVector(out float sumModule, out float sumAngle, params ArrowGraph[] graphs)
    {
        Vector2 summedVector = Vector2.zero;
        
        foreach (ArrowGraph arrowGraph in graphs)
        {
            float module = arrowGraph.Value;
            float angle = arrowGraph.Angle;

            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * module;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * module;
            
            summedVector += new Vector2(x, y);
        }

        float summedModule = Mathf.Sqrt(Mathf.Pow(summedVector.x, 2f) 
                                        + Mathf.Pow(summedVector.y, 2f));
        float summedAngle = Mathf.Atan2(summedVector.y, summedVector.x);
        
        sumModule = summedModule;
        sumAngle = summedAngle * Mathf.Rad2Deg;
    }
}