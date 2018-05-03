using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager {
    public static string MENU_NAME = "Menu";
    public static int WIDTH_NODES = 30;
    public static int HEIGHT_NODES = 20;

    public static BoardManager _instance;
    private int[,] maxtrixNodes = new int[WIDTH_NODES, HEIGHT_NODES];
    private static Dictionary<int, GameObject> circuitComponent = new Dictionary<int, GameObject>();
    private static Dictionary<int, GameObject> boardWareHouse = new Dictionary<int, GameObject>();
    public bool isConstructing = false;
    public int isClickFinish = 0;
    public Vector3 firstClick = new Vector3();

    public void ClearBoard()
    {
        int count = boardWareHouse.Count;
        for(int i = 0; i < count; i++)
        {
            String name = boardWareHouse.Values.ElementAt(i).gameObject.name;
            GameObject.Destroy(GameObject.Find(name));
        }
        boardWareHouse.Clear();
        ResetParam();
    }

    public Vector3 secondClick = new Vector3();

    private BoardManager() { }


    public void ResetParam()
    {
        isConstructing = false;
        isClickFinish = 0;
        firstClick = new Vector3();
        secondClick = new Vector3();
        circuitComponent.Clear();
    }
    public static BoardManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new BoardManager();
        }
        return _instance;
    }

    public void AddObjectToCircuit(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return;
        }
        int count = circuitComponent.Count;
        circuitComponent.Add(++count, gameObject);
        count = 0;
        count = boardWareHouse.Count;
        boardWareHouse.Add(++count, gameObject);
    }

    public bool HaveColisionOnPathGenerateWire(Vector2 firstClick, Vector2 secondClick, int typeGenerate)
    {
        
        //Type: !(compareX ^ compareY)
        if (typeGenerate == 1)
        {
            //Browse the matrix to check colision
            return FindColisionOnVertical((int)firstClick.y, (int)secondClick.y, (int)firstClick.x)
                || FindColisionOnHorizontal((int)firstClick.x, (int)secondClick.x, (int)secondClick.y);
        }

        //Type: compareX ^ compare
        if (typeGenerate == 2)
        {
            //Browse the matrix to check colision
            return FindColisionOnVertical((int)firstClick.y, (int)secondClick.y, (int)secondClick.x)
                || FindColisionOnHorizontal((int)firstClick.x, (int)secondClick.x, (int)firstClick.y);
        }
       
        
        return true;
    }

    private bool FindColisionOnVertical(int y1, int y2, int x)
    {
        int beginY = y1;
        int endY = y1;
        if (y1 > y2)
        {
            beginY = y2;
        } else
        {
            endY = y2;
        }

        for (int y = beginY + 1; y < endY; y++)
        {
            if (maxtrixNodes[x,y] != 0)
            {
                return true;
            }
        }
        return false;
    }

    private bool FindColisionOnHorizontal(int x1, int x2, int y)
    {
        int beginX = x1;
        int endX = x1;
        if (x1 > x2)
        {
            beginX = x2;
        }
        else
        {
            endX = x2;
        }

        for (int x = beginX + 1; x < endX; x++)
        {
            if (maxtrixNodes[x, y] != 0)
            {
                return true;
            }
        }
        return false;
    }

    public Dictionary<int, GameObject> GetCircuitComponent()
    {
        return circuitComponent;
    }

    public int GetPosition(int x, int y)
    {
        if (x < 0 || x >= WIDTH_NODES)
        {
            return 0;
        }
        if (y < 0 || y >= HEIGHT_NODES)
        {
            return 0;
        }
        return maxtrixNodes[x, y];
    }
    
    public void SetPosition(int x, int y, int value)
    {
        if (x < 0 || x >= WIDTH_NODES || y < 0 || y >= HEIGHT_NODES)
        {
            return;
        }
        maxtrixNodes[x, y] = value;
    }

}
