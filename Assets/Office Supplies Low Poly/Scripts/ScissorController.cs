using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScissorController : MonoBehaviour {

    private BoardManager boardManager;
    public int speed = 20;
	// Use this for initialization
	void Start () {
        boardManager = BoardManager.GetInstance();
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.right * Time.deltaTime * speed);

        // ...also rotate around the World's Y axis
        transform.Rotate(Vector3.up * Time.deltaTime * speed, Space.World);
    }

    private void OnMouseDown()
    {
        Debug.Log("Scissor clicked");
        boardManager.isConstructing = false;
        boardManager.isClickFinish = 0;
    }
}
