﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadCameraManager : MonoBehaviour {

    public string URL;
    public static bool beginRayCasting = false;
    private GameObject socketController;

    // Use this for initialization
    void Start () {
        socketController = GameObject.Find("SocketController");
	}

    private void OnMouseDown()
    {
        SocketV2 socket = socketController.GetComponent<SocketV2>();
        socket.SendData("url:" + URL + "@");
    }

    // Update is called once per frame
    void Update () {
        /*if (!beginRayCasting)
        {
            return;
        }
        RaycastHit hit;

        Vector3 test = new Vector3(GenerateChildObject.curPos.x + 13.57f, GenerateChildObject.curPos.z + 9.94f, z);
        if (Physics.Raycast(test, directionZ, out hit, 100))
        {
            if (hit.collider.gameObject.tag == "cube")
            {
                hit.collider.gameObject.GetComponent<Renderer>().material.color = Color.red;
            }
        }*/

    }
}