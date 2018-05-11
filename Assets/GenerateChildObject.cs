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
    private bool check = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (check)
        {
            GetCoordinateFromDetection();
            ScreenCenter = CalculateCenter(TopLeftScreen, BottomRightScreen);
            ObjectCenter = CalculateCenter(TopLeftObject, BottomRightObject);
            CalculateScale(TopLeftScreen, BottomRightScreen, TopLeftObject, BottomRightObject);
            GenerateChild();
            check = false;
        }
        
    }

    private void GenerateChild()
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.SetParent(this.gameObject.transform);
        ObjectCenter -= ScreenCenter;
        plane.transform.localRotation = Quaternion.Euler(0, 0, 0);
        //plane.transform.localPosition = new Vector3(ObjectCenter.x / 33f, 0.1f, (-ObjectCenter.y) / 18.5f);
        plane.transform.localPosition = new Vector3(ObjectCenter.x / 62f, 0.1f, (-ObjectCenter.y) / 34f);
        plane.transform.localScale = new Vector3(VObjectScale.x, 1, VObjectScale.y);
    }

    private void CalculateScale(Vector2 topLeftScreen, Vector2 bottomRightScreen, Vector2 topLeftObject, Vector2 bottomRightObject)
    {
        Vector2 ScreenSize = bottomRightScreen - topLeftScreen;
        Vector2 ObjectSize = bottomRightObject - topLeftObject;
        VObjectScale.x = ObjectSize.x / ScreenSize.x;
        VObjectScale.y = ObjectSize.y / ScreenSize.y;
    }

    private Vector2 CalculateCenter(Vector2 topLeftObject, Vector2 bottomRightObject)
    {
        Vector2 objectCenter;
        objectCenter = (topLeftObject + bottomRightObject) / 2;
        return objectCenter;
    }

    private void GetCoordinateFromDetection()
    {
        TopLeftScreen = new Vector2(29, 117);
        BottomRightScreen = new Vector2(588, 449);

        //TopLeftObject = new Vector2(152, 312);
        //BottomRightObject = new Vector2(199, 339);

        TopLeftObject = new Vector2(424, 344);
        BottomRightObject = new Vector2(516, 393);
    }
}
