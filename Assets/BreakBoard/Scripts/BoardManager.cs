using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager {
    public static string MENU_NAME = "Menu";
    public static int WIDTH_NODES = 30;
    public static int HEIGHT_NODES = 20;

    public static BoardManager _instance;
    private int[,] maxtrixNodes = new int[WIDTH_NODES, HEIGHT_NODES];

    public bool isConstructing = false;
    public int isClickFinish = 0;
    public Vector3 firstClick = new Vector3();
    public Vector3 secondClick = new Vector3();

    private BoardManager() { }

    public static BoardManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new BoardManager();
        }
        return _instance;
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
