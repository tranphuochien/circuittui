using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessMessage : MonoBehaviour {

    public static DetetorManager detetorManager;
    public static float DefaultFOV = -1;

    public static void processMessage(string msg)
    {
        string[] msgContent = msg.Split(Constant.TOKEN_END.ToCharArray());

        string msgCode = msgContent[0].Substring(0, 4);

        switch (msgCode)
        {
            case Constant.TOKEN_BEGIN_SHAKE:
                detetorManager.shouldSendPosition = true;
                break;
            case Constant.TOKEN_BEGIN_FREEZE:
                detetorManager.shouldSendPosition = false;
                break;
            case Constant.TOKEN_BEGIN_FLIP:
                changeMapType();
                break;
            case Constant.TOKEN_BEGIN_ZOOM:
                if (DefaultFOV < 0)
                {
                    DefaultFOV = Camera.main.fieldOfView;
                }
                float zoomVal = getZoomValFromMsg(msgContent[0]);
                float DestFov = Camera.main.fieldOfView + zoomVal;
                zoomMap(DestFov);
                break;
            case Constant.TOKEN_BEGIN_ZOOMDEFAULT:
                if (DefaultFOV >= 0)
                {
                    zoomMap(DefaultFOV);
                }
                break;
        }
        if (msgCode == Constant.TOKEN_BEGIN_SHAKE)
        {

            return;
        }
    }

    private static void zoomMap(float DestFov)
    {
        float speed = 5.0f;
        Camera.main.fieldOfView = Mathf.MoveTowards(Camera.main.fieldOfView, DestFov, Time.deltaTime * speed);
    }

    private static float getZoomValFromMsg(string content)
    {
        float val = float.Parse(content.Substring(5, content.Length - 5));
        return val; 
    }

    private void Update()
    {
        changeMapType();
    }

    private static void changeMapType()
    {
        Debug.Log("Changemaptype");
        GameObject map = GameObject.Find("Map");
        map.SendMessage("changeMapType");
    }
}