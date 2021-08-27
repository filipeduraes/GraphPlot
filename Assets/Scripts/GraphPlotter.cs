using TMPro;
using UnityEngine;

public class GraphPlotter : MonoBehaviour
{
    [Header("Configuration")] 
    [SerializeField] private float showedTime = 2f;
    [SerializeField] private int xLabelDivisions = 6;
    [SerializeField] private int yLabelDivisions = 6;
    
    [Header("Anchors")]
    [SerializeField] private Transform bottomLeft;
    [SerializeField] private Transform topRight;

    [Header("Model Labels")] 
    [SerializeField] private SpriteRenderer gridPrefab;
    [SerializeField] private TMP_Text xLabelModel;
    [SerializeField] private TMP_Text yLabelModel;
    [SerializeField] private Transform xLabelContainer;
    [SerializeField] private Transform yLabelContainer;

    private Vector2 minPositions = new Vector2();
    private Vector2 maxPositions = new Vector2();
    private LabelHandler labelHandler;
    
    private float graphHeight = 0f;
    private float graphWidth = 0f;
    
    private void Awake()
    {
        xLabelModel.gameObject.SetActive(false);
        yLabelModel.gameObject.SetActive(false);
        
        Vector3 bottomLeftPosition = bottomLeft.position;
        Vector3 topRightPosition = topRight.position;
        
        graphHeight = topRightPosition.y - bottomLeftPosition.y;
        graphWidth = topRightPosition.x - bottomLeftPosition.x;

        maxPositions.x = showedTime;
        labelHandler = new LabelHandler(xLabelModel, yLabelModel, xLabelContainer, yLabelContainer, bottomLeft, topRight, gridPrefab);
        labelHandler.RecalculateXPositions(new LabelParameters(minPositions.x, maxPositions.x, xLabelDivisions));
    }

    public void AddValue(float value, float time, float timeStamp, Graph graph)
    {
        int positionCount = graph.LineRenderer.positionCount;

        Vector3[] newPositions;
        Vector3[] oldPositions = new Vector3[positionCount];
        graph.LineRenderer.GetPositions(oldPositions);
        
        RecalculateLimits(value, time);
        
        bool screenIsFull = time >= maxPositions.x;
        
        if (screenIsFull)
        {
            newPositions = new Vector3[positionCount];

            for (int i = 1; i < positionCount; i++)
            {
                int newIndex = i - 1;
                graph.Values[newIndex] = graph.Values[i];
                newPositions[newIndex] = oldPositions[i];
                float xValue = minPositions.x + newIndex * timeStamp;
                newPositions[newIndex].x = ConvertToDotValue(xValue,
                    minPositions.x, maxPositions.x, graphWidth, bottomLeft.position.x);
            }
            
            graph.Values[graph.Values.Count - 1] = value;
        }
        else
        {
            newPositions = new Vector3[positionCount + 1];
            oldPositions.CopyTo(newPositions);
            
            for (int i = 0; i < newPositions.Length - 1; i++)
            {
                newPositions[i].y = ConvertToDotValue(graph.Values[i], 
                    minPositions.y, maxPositions.y, graphHeight, bottomLeft.position.y);
            }
            
            graph.LineRenderer.positionCount++;
            graph.Values.Add(value);
        }

        Vector3 dotPosition = GetDotPosition(time, value);
        newPositions[newPositions.Length - 1] = dotPosition;
        graph.LineRenderer.SetPositions(newPositions);
    }

    private void RecalculateLimits(float value, float time)
    {
        if (value < minPositions.y)
        {
            minPositions.y = value;
            labelHandler.RecalculateYPositions(new LabelParameters(minPositions.y, maxPositions.y, yLabelDivisions));
        }
        else if (value > maxPositions.y)
        {
            maxPositions.y = value;
            labelHandler.RecalculateYPositions(new LabelParameters(minPositions.y, maxPositions.y, yLabelDivisions));
        }

        if (time >= maxPositions.x)
        {
            maxPositions.x = time;
            minPositions.x = maxPositions.x - showedTime;
            labelHandler.RecalculateXPositions(new LabelParameters(minPositions.x, maxPositions.x, xLabelDivisions));
        }
    }

    private Vector2 GetDotPosition(float currentX, float currentY)
    {
        Vector3 offset = bottomLeft.position;
        float xPosition = ConvertToDotValue(currentX, minPositions.x, maxPositions.x, graphWidth, offset.x);
        float yPosition = ConvertToDotValue(currentY, minPositions.y, maxPositions.y, graphHeight, offset.y);
        return new Vector2(xPosition, yPosition);
    }

    public static float ConvertToDotValue(float regularValue, float min, float max, float size, float offset)
    {
        float normalized = Mathf.InverseLerp(min, max, regularValue);
        float position = normalized * size + offset;
        return position;
    }
}