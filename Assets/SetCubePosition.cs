using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCubePosition : MonoBehaviour {
    public GameObject socket;
    SocketClient client;

	// Use this for initialization
	void Start () {
        client = socket.GetComponent<SocketClient>();
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = this.transform.localPosition;
        pos.x = SocketClient.xPos;
        pos.z = SocketClient.yPos;

        this.transform.localPosition = pos;
	}
}
