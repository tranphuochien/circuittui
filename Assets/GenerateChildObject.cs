using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateChildObject : MonoBehaviour {

    private Vector2 TopLeftScreen;
    private Vector2 BottomRightScreen;
    private Vector2 ScreenCenter;
    private Vector2 TopLeftObject;
    private Vector2 BottomRightObject;
    private Vector2 ObjectCenter;
    private Vector2 VObjectScale;
    public GameObject plane;

	// Use this for initialization
	void Start () {
        plane = GameObject.Find("Menu");
    }
	
	// Update is called once per frame
	void Update () {
        if (SocketClient.IsReceived)
        {
            //Destroy(GameObject.Find("ABCD"));
            
            GetCoordinateFromDetection();
            ScreenCenter = CalculateCenter(TopLeftScreen, BottomRightScreen);
            //ObjectCenter = CalculateCenter(TopLeftObject, BottomRightObject);
            ObjectCenter = new Vector2(SocketClient.xPos, SocketClient.yPos);
            CalculateScale(TopLeftScreen, BottomRightScreen, TopLeftObject, BottomRightObject);
            if (plane == null)
            {
                //GenerateChild();
            } else
            {
                MovePlane();
            }
            
        }
        
    }

    private void MovePlane()
    {
        ObjectCenter -= ScreenCenter;
        plane.transform.localPosition = new Vector3((ObjectCenter.x - 2) / 40f, 0.1f, (7 - ObjectCenter.y) / 21f);
    }

    private void GenerateChild()
    {
        plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.name = "ABCD";
        plane.transform.SetParent(this.gameObject.transform);
        ObjectCenter -= ScreenCenter;
        plane.transform.localRotation = Quaternion.Euler(0, 0, 0);
        plane.transform.localPosition = new Vector3((ObjectCenter.x - 2) / 40f, 0.1f, (7 - ObjectCenter.y) / 21f);
        plane.transform.localScale = new Vector3(VObjectScale.x, 1, VObjectScale.y);
        plane.GetComponent<MeshRenderer>().material = Resources.Load("white", typeof(Material)) as Material;
    }

    private void CalculateScale(Vector2 topLeftScreen, Vector2 bottomRightScreen, Vector2 topLeftObject, Vector2 bottomRightObject)
    {
        //Vector2 ScreenSize = bottomRightScreen - topLeftScreen;
        //Vector2 ObjectSize = bottomRightObject - topLeftObject;
        //VObjectScale.x = ObjectSize.x / ScreenSize.x;
        //VObjectScale.y = ObjectSize.y / ScreenSize.y;
        VObjectScale.x = 0.22f;
        VObjectScale.y = 0.33f;
    }

    private Vector2 CalculateCenter(Vector2 topLeftObject, Vector2 bottomRightObject)
    {
        Vector2 objectCenter;
        objectCenter = (topLeftObject + bottomRightObject) / 2;
        return objectCenter;
    }

    private void GetCoordinateFromDetection()
    {
        TopLeftScreen = new Vector2(38, 56);
        BottomRightScreen = new Vector2(585, 432);

        //TopLeftObject = new Vector2(152, 312);
        //BottomRightObject = new Vector2(199, 339);

        //TopLeftObject = new Vector2(424, 344);
        //BottomRightObject = new Vector2(516, 393);
    }
}
