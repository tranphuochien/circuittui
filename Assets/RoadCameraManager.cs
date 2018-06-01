using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadCameraManager : MonoBehaviour {

    public string URL;
    public static bool beginRayCasting = false;
    private GameObject socketController;
    public int x = 12;
    public int y = 7;
    public float z = -1f;
    public Vector3 directionZ = new Vector3(0, 0, -1);
    private GameObject screenObject;
    private readonly string TAG = "road_camera";
    private bool isRaycasting = false;
    private bool isSent = false;

    // Use this for initialization
    void Start () {
        socketController = GameObject.Find("SocketController");
        screenObject = GameObject.Find("Screen");
    }

    private void OnMouseDown()
    {
        SocketV2 socket = socketController.GetComponent<SocketV2>();
        socket.SendData("url:" + URL + "@");
    }

    // Update is called once per frame
    void Update () {
        if (!beginRayCasting)
        {
            return;
        }
        RaycastHit hit;

        Vector3 test = new Vector3(GenerateChildObject.curPos.x + screenObject.transform.position.x, GenerateChildObject.curPos.z + screenObject.transform.position.y, z);
        if (Physics.Raycast(test, directionZ, out hit, 100))
        {
            if (hit.collider.gameObject.tag == TAG && !isSent)
            {
                //hit.collider.gameObject.GetComponent<Renderer>().material.color = Color.red;
                isRaycasting = true;
                Debug.Log("pang pang");
                SocketV2 socket = socketController.GetComponent<SocketV2>();
                socket.SendData("url:" + URL + "@");
                isSent = true;
            }
        } else
        {
            isRaycasting = false;
        }

        if (!isRaycasting)
        {
            isSent = false;
        }
    }
}
