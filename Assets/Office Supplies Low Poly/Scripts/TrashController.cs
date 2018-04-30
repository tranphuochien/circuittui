using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashController : MonoBehaviour {

    private BoardManager boardManager;
    public int speed = 20;
    Vector3 vectorRotate = new Vector3(0, 0, 1);

    void Start()
    {
        boardManager = BoardManager.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(vectorRotate * Time.deltaTime * speed);
    }

    private void OnMouseDown()
    {
        Debug.Log("Trash clicked");
        boardManager.ClearBoard();
        
    }
}
