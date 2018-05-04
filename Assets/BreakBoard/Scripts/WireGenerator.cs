using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VolumetricLines;

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

        if (Input.GetMouseButtonDown(0) && !boardManager.isConstructing)
        {
            boardManager.isConstructing = true;
            AutoCompleteWire();
            replaceWire();
            boardManager.ResetParam();
        }
    }

    private void replaceWire()
    {
        Dictionary<int, GameObject> currentCircuit = boardManager.GetCircuitComponent();
        List<GameObject> lines = new List<GameObject>();
        GameObject newLine;
        Vector3 a, b, c;
        foreach (KeyValuePair<int, GameObject> item in currentCircuit)
        {
            string prefix = item.Value.gameObject.name.Substring(0, 4);
            if (prefix == "line")
            {
                lines.Add(item.Value);
            }
        }
        lines.ForEach(line =>
        {
            LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
            if (lineRenderer.positionCount == 3)
            {
                a = lineRenderer.GetPosition(0);
                b = lineRenderer.GetPosition(1);
                c = lineRenderer.GetPosition(2);
                newLine = (GameObject)Instantiate(Resources.Load("VolumetricLine"));
                newLine.transform.localPosition = a;
                newLine.transform.rotation = Quaternion.Euler(0, 0, 0);
                newLine.GetComponent<VolumetricLineBehavior>().StartPos = a;
                newLine.GetComponent<VolumetricLineBehavior>().EndPos = b;
                newLine = (GameObject)Instantiate(Resources.Load("VolumetricLine"));
                newLine.transform.localPosition = b;
                newLine.transform.rotation = Quaternion.Euler(0, 0, 0);
                newLine.GetComponent<VolumetricLineBehavior>().StartPos = b;
                newLine.GetComponent<VolumetricLineBehavior>().EndPos = c;
            }
            else
            {
                a = lineRenderer.GetPosition(0);
                b = lineRenderer.GetPosition(1);
                newLine = (GameObject)Instantiate(Resources.Load("VolumetricLine"));
                newLine.transform.localPosition = a;
                newLine.transform.rotation = Quaternion.Euler(0, 0, 0);
                newLine.GetComponent<VolumetricLineBehavior>().StartPos = a;
                newLine.GetComponent<VolumetricLineBehavior>().EndPos = b;
            }
        });
    }

    private void AutoCompleteWire()
    {
        Dictionary<int, GameObject> circuit = boardManager.GetCircuitComponent();
        if (circuit.Count <= 0)
        {
            return;
        }
        Vector3 secondPos = circuit.Values.ElementAt(0).transform.position;
        Vector3 firstPos = circuit.Values.ElementAt(circuit.Count - 1).transform.position;

        DrawWire(firstPos, secondPos);
    }

    private void GenerateWire(Vector3 finalPosition)
    {
        //Set  position in matrix
        boardManager.SetPosition((int)finalPosition.x, (int)finalPosition.y, 1);

        if (boardManager.isClickFinish == 1)
        {
            if (!boardManager.isConstructing)
            {
                boardManager.isClickFinish = 0;
                return;
            }

            boardManager.secondClick = new Vector3(finalPosition.x, finalPosition.y, lineZ);
            DrawWire(boardManager.firstClick, boardManager.secondClick);

            //Swap data to prepare generate wire with next node
            boardManager.firstClick = boardManager.secondClick;
        } else
        {
            boardManager.firstClick = new Vector3(finalPosition.x, finalPosition.y, lineZ);
            boardManager.isConstructing = true;
            boardManager.isClickFinish = 1;
        }
    }

    private void DrawWire(Vector3 firstClick, Vector3 secondClick)
    {
        List<Vector3> listPoints = new List<Vector3>();
        listPoints.Add(new Vector3(firstClick.x, firstClick.y, lineZ));
        if (firstClick.x == secondClick.x || firstClick.y == secondClick.y)
        {
            listPoints.Add(new Vector3(secondClick.x, secondClick.y, lineZ));
        }
        else
        {
            bool compareX = firstClick.x < secondClick.x;
            bool compareY = firstClick.y < secondClick.y;
            if (!(compareX ^ compareY))
            {
                if (!boardManager.HaveColisionOnPathGenerateWire(firstClick, secondClick, 1))
                {
                    listPoints.Add(new Vector3(firstClick.x, secondClick.y, lineZ));
                } else
                {
                    listPoints.Add(new Vector3(secondClick.x, firstClick.y, lineZ));
                }
            }
            if (compareX ^ compareY)
            {
                if (!boardManager.HaveColisionOnPathGenerateWire(firstClick, secondClick, 2))
                {
                    listPoints.Add(new Vector3(secondClick.x, firstClick.y, lineZ));
                }
                else
                {
                    listPoints.Add(new Vector3(firstClick.x, secondClick.y, lineZ));
                }
            }
           
            listPoints.Add(new Vector3(secondClick.x, secondClick.y, lineZ));
        }
        CreateLine(listPoints);
    }

    private void CreateLine(List<Vector3> listPoints)
    {
        //Create Line object
        var gameObject = new GameObject();
        gameObject.name = "line_" + DateTime.Now.ToString();
        var lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.positionCount = listPoints.Count;
        lineRenderer.SetPositions(listPoints.ToArray());
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        boardManager.AddObjectToCircuit(gameObject);
    }

    private Vector3 PlaceCubeNear(Vector3 clickPoint)
    {
        var finalPosition = grid.GetNearestPointOnGrid(clickPoint);
        RadialMenuController menu = Resources.FindObjectsOfTypeAll<RadialMenuController>()[0];
        menu.ZoomOutWhenAppear(new Vector3(finalPosition.x, finalPosition.y, 2 * lineZ));
        
        return finalPosition;
    }
}