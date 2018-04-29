using System;
using System.Collections.Generic;
using UnityEngine;

public class WireGenerator : MonoBehaviour
{
    private Grid grid;
    private BoardManager boardManager;
    public Material lineMaterial;
    public float lineWidth;
    private float lineZ = -0.3f;

    private void Start()
    {
        boardManager =  BoardManager.GetInstance();    
    }

    private void Awake()
    {
        grid = FindObjectOfType<Grid>();
    }

    private void Update()
    {
        //Ctrl + click
        if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo))
            {
                Vector3 finalPosition = PlaceCubeNear(hitInfo.point);
                GenerateWire(finalPosition);
            }
        }
    }

    private void GenerateWire(Vector3 finalPosition)
    {
        //Set  position in matrix
        boardManager.SetPosition((int)finalPosition.x, (int)finalPosition.y, 1);

        if (boardManager.isClickFinish == 1)
        {
            //boardManager.isClickFinish = 0;
            boardManager.secondClick = new Vector3(finalPosition.x, finalPosition.y, lineZ);
            DrawWire(boardManager.firstClick, boardManager.secondClick);

            //Swap data to prepare generate wire with next node
            boardManager.firstClick = boardManager.secondClick;
        } else
        {
            boardManager.firstClick = new Vector3(finalPosition.x, finalPosition.y, lineZ);
            boardManager.isClickFinish = 1;
        }
    }

    private void DrawWire(Vector3 firstClick, Vector3 secondClick)
    {
        List<Vector3> listPoints = new List<Vector3>();
        listPoints.Add(new Vector3(firstClick.x, firstClick.y, lineZ));
        if(firstClick.x == secondClick.x || firstClick.y == secondClick.y)
        {
            listPoints.Add(new Vector3(secondClick.x, secondClick.y, lineZ));            
        } else
        {
            bool compareX = firstClick.x < secondClick.x;
            bool compareY = firstClick.y < secondClick.y;
            if (!(compareX ^ compareY)) {
                listPoints.Add(new Vector3(firstClick.x, secondClick.y, lineZ));
            }
            if (compareX ^ compareY)
            {
                listPoints.Add(new Vector3(secondClick.x, firstClick.y, lineZ));
            }
            listPoints.Add(secondClick);
        }
        CreateLine(listPoints);
    }

    private void CreateLine(List<Vector3> listPoints)
    {
        //Create Line object
        var gameObject = new GameObject();
        var lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.positionCount = listPoints.Count;
        lineRenderer.SetPositions(listPoints.ToArray());
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }

    private Vector3 PlaceCubeNear(Vector3 clickPoint)
    {
        var finalPosition = grid.GetNearestPointOnGrid(clickPoint);
        RadialMenuController menu = Resources.FindObjectsOfTypeAll<RadialMenuController>()[0];
        menu.ZoomOutWhenAppear(new Vector3(finalPosition.x, finalPosition.y, 2 * lineZ));
        
        return finalPosition;
    }
}