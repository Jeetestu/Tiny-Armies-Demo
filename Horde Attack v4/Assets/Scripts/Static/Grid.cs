using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JUtils;
using System;

public class Grid<TGridObject>
{
    protected int width;
    protected int height;
    protected float cellSize;
    protected TGridObject[,] gridArray;
    protected Vector3 originPosition;
    public TextMesh[,] debugTextArray;
    protected bool showGrid = false;

    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new TGridObject[width, height];

        debugTextArray = new TextMesh[width, height];

        //instantiates all cell values
        for (int x = 0; x < gridArray.GetLength(0); x++)
            for (int y = 0; y < gridArray.GetLength(1); y++)
                gridArray[x, y] = createGridObject(this, x, y);
    }

    public virtual void ShowGrid()
    {
        showGrid = true;
        for (int x = 0; x < gridArray.GetLength(0); x++)
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                if (gridArray[x, y] == null)
                    debugTextArray[x, y] = JUtilsClass.CreateWorldText("null", null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, 20, Color.white, TextAnchor.MiddleCenter);
                else
                    debugTextArray[x, y] = JUtilsClass.CreateWorldText(gridArray[x, y].ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, 20, Color.white, TextAnchor.MiddleCenter);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
            }
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
    }

    public virtual void SetValue(int x, int y, TGridObject value)
    {
        if (isInsideGrid(x, y))
            gridArray[x, y] = value;
        if (showGrid)
            debugTextArray[x, y].text = gridArray[x, y].ToString();
    }

    public void SetValue(Vector3 worldPosition, TGridObject value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    public TGridObject GetValue(int x, int y)
    {
        if (isInsideGrid(x, y))
            return gridArray[x, y];
        else
        {
            Debug.LogWarning("Invalid grid request at coordinates: " + x + "," + y);
            return default(TGridObject);
        }

    }

    public TGridObject GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetValue(x, y);
    }

    public TGridObject GetValue(Vector2Int coords)
    {
        return GetValue(coords.x, coords.y);
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public Vector3 GetWorldPositionCentre(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition + new Vector3(cellSize * 0.5f, cellSize * 0.5f);
    }

    public Vector3 GetWorldPositionCentre(Vector2Int vec)
    {
        return new Vector3(vec.x, vec.y) * cellSize + originPosition + new Vector3(cellSize * 0.5f, cellSize * 0.5f);
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public float getCellSize()
    {
        return cellSize;
    }

    public Vector2Int getXYVector(Vector3 worldPosition)
    {
        GetXY(worldPosition, out int x, out int y);
        return new Vector2Int(x, y);
    }

    public bool isInsideGrid(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }


}
