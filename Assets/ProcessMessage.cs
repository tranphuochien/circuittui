using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProcessMessage : MonoBehaviour {

    private static GameObject socketController;
    static SocketV2 socket;
    public static DetetorManager detetorManager;
    public static FlagController flagController;
    public static float DefaultFOV = -1;
    static int currentImageNumber = -1;
    static string currentMsg = "";
    static List<Texture> TextureList = new List<Texture>();
    string[] FileNameList = { "boku_no_hero", "gintama", "kuroko", "nanatsu_taizai", "one_punch_man" };

    void Start()
    {
        socketController = GameObject.Find("SocketController");
        socket = socketController.GetComponent<SocketV2>();
        LoadTexture();
    }

    private void LoadTexture()
    {
        for (int i = 0; i < FileNameList.Length; i++)
        {
            TextureList.Add(Resources.Load(FileNameList[i]) as Texture);
        }
    }

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
                UnityMainThreadDispatcher.Instance().Enqueue(changeMapType());
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
            case Constant.TOKEN_BEGIN_DROP:
                currentImageNumber = GetCurrentImageFromMsg(msgContent[0]);
                currentMsg = GetCurrentImageMessage(msgContent[0]);
                UnityMainThreadDispatcher.Instance().Enqueue(DisplayImage());
                break;
            case Constant.TOKEN_BEGIN_SET_FLAG:
                flagController.AddFlag(CalibrateObject.curPos);
                break;
            case Constant.TOKEN_BEGIN_ANALYZE:
                break;
            case Constant.TOKEN_BEGIN_GET:
                SendMessageToClient();
                break;
        }
        if (msgCode == Constant.TOKEN_BEGIN_SHAKE)
        {

            return;
        }
    }

    private static void SendMessageToClient()
    {
        socket.SendDataAll(Constant.TOKEN_BEGIN_GET + ":" + currentImageNumber + Constant.TOKEN_END);
    }

    private static IEnumerator DisplayImage()
    {
        Debug.Log("Display Image");
        GameObject.Find("ImageContent").GetComponent<RawImage>().texture = TextureList[currentImageNumber];
        GameObject.Find("Message").GetComponent<InputField>().text = currentMsg;
        yield return null;
    }

    private static int GetCurrentImageFromMsg(string v)
    {
        int val = int.Parse(v.Substring(5, v.Length - 5).Split('|')[0]);
        return val;
    }

    private static string GetCurrentImageMessage(string v)
    {
        string val = v.Substring(5, v.Length - 5).Split('|')[1];
        return val;
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

    public static IEnumerator changeMapType()
    {
        Debug.Log("Changemaptype");
        GameObject map = GameObject.Find("Map");
        map.SendMessage("ChangeMapType");
        yield return null;
    }
}