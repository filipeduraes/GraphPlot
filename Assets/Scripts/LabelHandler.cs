using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LabelHandler
{
    private readonly List<SpriteRenderer> xGrids = new List<SpriteRenderer>();
    private readonly List<SpriteRenderer> yGrids = new List<SpriteRenderer>();
    private readonly List<TMP_Text> xLabels = new List<TMP_Text>();
    private readonly List<TMP_Text> yLabels = new List<TMP_Text>();
    
    private readonly TMP_Text xLabelModel;
    private readonly TMP_Text yLabelModel;
    private readonly Transform xLabelContainer;
    private readonly Transform yLabelContainer;
    private readonly Transform bottomLeft;
    private readonly SpriteRenderer gridPrefab;
    private readonly float width;
    private readonly float height;

    public LabelHandler(TMP_Text xLabelModel, TMP_Text yLabelModel, Transform xLabelContainer,
        Transform yLabelContainer, Transform bottomLeft, Transform topRight, SpriteRenderer gridPrefab)
    {
        this.xLabelModel = xLabelModel;
        this.yLabelModel = yLabelModel;
        this.xLabelContainer = xLabelContainer;
        this.yLabelContainer = yLabelContainer;
        this.bottomLeft = bottomLeft;
        this.gridPrefab = gridPrefab;

        Vector3 topRightPosition = topRight.position;
        Vector3 bottomLeftPosition = bottomLeft.position;
        width = topRightPosition.x - bottomLeftPosition.x;
        height = topRightPosition.y - bottomLeftPosition.y;
    }

    public void RecalculateXPositions(LabelParameters xParameters)
    {
        float xMin = xParameters.Minimum;
        float xMax = xParameters.Maximum;
        float xSpacing = (xMax - xMin) / xParameters.Divisions;
        int xLabelsCount = xLabels.Count;
        
        for (int xIndex = 0; xIndex <= xParameters.Divisions; xIndex++)
        {
            float value = xMin + xIndex * xSpacing;
            float xPos = GraphPlotter.ConvertToDotValue(value, xMin, xMax, width, bottomLeft.position.x);
            
            if (xLabelsCount > 0)
            {
                RectTransform rectTransform = (RectTransform) xLabels[xIndex].transform;
                CopyRectTransform(rectTransform, xLabelModel.transform as RectTransform);
                rectTransform.position = new Vector2(xPos, rectTransform.position.y);

                Vector2 gridPosition = xGrids[xIndex].transform.position;
                gridPosition.x = xPos;
                xGrids[xIndex].transform.position = gridPosition;
                    
                xLabels[xIndex].SetText($"{value:0.#}");
                xLabelsCount--;
            }
            else
            {
                CreateXLabel(xPos, value);
            }
        }
    }
    
    public void RecalculateYPositions(LabelParameters yParameters)
    {
        float yMin = yParameters.Minimum;
        float yMax = yParameters.Maximum;
        float ySpacing = (yMax - yMin) / yParameters.Divisions;
        int yLabelsCount = yLabels.Count;

        for (int yIndex = 0; yIndex <= yParameters.Divisions; yIndex++)
        {
            float value = yMin + yIndex * ySpacing;
            float yPos = GraphPlotter.ConvertToDotValue(value, yMin, yMax, height, bottomLeft.position.y);
            
            if (yLabelsCount > 0)
            {
                RectTransform rectTransform = (RectTransform) yLabels[yIndex].transform;
                CopyRectTransform(rectTransform, yLabelModel.transform as RectTransform);
                rectTransform.position = new Vector2(rectTransform.position.x, yPos);
                
                Vector2 gridPosition = yGrids[yIndex].transform.position;
                gridPosition.y = yPos;
                yGrids[yIndex].transform.position = gridPosition;
                
                yLabels[yIndex].SetText($"{value:0.#}");
                yLabelsCount--;
            }
            else
            {
                CreateYLabel(yPos, value);
            }
        }
    }

    private void CreateXLabel(float xPosition, float xValue)
    {
        RectTransform xLabelRect = CreateSingleLabel(xLabelModel, xLabelContainer, xValue, out TMP_Text xLabel);
        CreateXGrid(xPosition);

        Vector2 labelPosition = xLabelRect.position;
        labelPosition.x = xPosition;
        xLabelRect.position = labelPosition;
        xLabels.Add(xLabel);
    }

    private void CreateYLabel(float yPosition, float yValue)
    {
        RectTransform yLabelRect = CreateSingleLabel(yLabelModel, yLabelContainer, yValue, out TMP_Text yLabel);
        CreateYGrid(yPosition);
        
        Vector2 labelPosition = yLabelRect.position;
        labelPosition.y = yPosition;
        yLabelRect.position = labelPosition;
        yLabels.Add(yLabel);
    }
    
    private void CreateXGrid(float xPosition)
    {
        SpriteRenderer xGrid = Object.Instantiate(gridPrefab);
        Transform xGridTransform = xGrid.transform;
        xGridTransform.Rotate(Vector3.forward, 90f);
        Vector2 gridPosition = xGridTransform.position;
        gridPosition.x = xPosition;
        gridPosition.y = bottomLeft.position.y;
        xGridTransform.position = gridPosition;
        xGrid.size = new Vector2(height, xGrid.size.y);
        xGrids.Add(xGrid);
    }

    private void CreateYGrid(float yPosition)
    {
        SpriteRenderer yGrid = Object.Instantiate(gridPrefab);
        Transform yGridTransform = yGrid.transform;
        Vector2 gridPosition = yGridTransform.position;
        gridPosition.x = bottomLeft.position.x;
        gridPosition.y = yPosition;
        yGridTransform.position = gridPosition;
        yGrid.size = new Vector2(width, yGrid.size.y);
        yGrids.Add(yGrid);
    }

    private static RectTransform CreateSingleLabel(TMP_Text original, Transform container, float value, out TMP_Text label)
    {
        TMP_Text instance = Object.Instantiate(original, container, true);
        instance.gameObject.SetActive(true);
        
        RectTransform instanceRect = instance.GetComponent<RectTransform>();
        RectTransform originalRect = original.GetComponent<RectTransform>();
        CopyRectTransform(instanceRect, originalRect);
        instance.SetText($"{value:0.#}");
        label = instance;

        return instanceRect;
    }

    private static void CopyRectTransform(RectTransform original, RectTransform copied)
    {
        original.position = copied.position;
        original.sizeDelta = copied.sizeDelta;
    }
}
