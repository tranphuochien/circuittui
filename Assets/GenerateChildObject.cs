using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using System.Linq;
using System.Threading;

public class GenerateChildObject : MonoBehaviour {

    private Vector2 TopLeftScreen;
    private Vector2 BottomRightScreen;
    private Vector2 ScreenCenter;
    private Vector2 TopLeftObject;
    private Vector2 BottomRightObject;
    private Vector2 ObjectCenter;
    private Vector2 VObjectScale;
    public GameObject ObjFollowToken;
    public static GameObject listCubes;
    private static bool isCalibrated = false;
    public static Vector3 curPos = new Vector3(0, 0, 0);
    private float y0;
    private float x0;
    private float deltaY;
    private float deltaX;
    private readonly string CALIBRATE_PLANE = "CalibratePlane";
    private readonly string MENU = BoardManager.MENU_NAME;

    // Use this for initialization
    void Start () {
        //Find gameobject keep world map 
        listCubes = GameObject.Find("ListCubes");
        listCubes.SetActive(false);

        ObjFollowToken = GameObject.Find(MENU);
        GetCoordinateFromDetection();

        CalibratePlane = GameObject.Find(CALIBRATE_PLANE);
    }
	
	// Update is called once per frame
	void Update () {
        if (SocketClient.IsReceived)
        {
            //Destroy(GameObject.Find("ABCD"));
            if (isCalibrated)
            {
                listCubes.SetActive(true);
                RayCastController.beginRayCasting = true;
                if (GameObject.Find(CALIBRATE_PLANE) != null)
                {
                    GameObject.Find(CALIBRATE_PLANE).SetActive(false);
                    GameObject.Find(MENU).SetActive(true);
                }
                ObjectCenter = new Vector2(SocketClient.xPos, SocketClient.yPos);
                CalculateScale(TopLeftScreen, BottomRightScreen, TopLeftObject, BottomRightObject);
                if (ObjFollowToken == null)
                {
                    GenerateChild();
                }
                else
                {
                    MovePlane();
                }
            } else
            {
                if (GameObject.Find(MENU))
                {
                    GameObject.Find(MENU).SetActive(false);
                    GameObject.Find(CALIBRATE_PLANE).SetActive(true);
                }
                
                Calibrate();
            }
        }
    }

    //At a position, get maxDataCollect entries from detector system
    int maxDataCollect = 90;
    //Number of positions at X(Y) axis need collect data to calibrate
    int maxNumX = 9, maxNumY = 6;
    //Must equal: maxNumX + maxNumY
    int maxData = 15; 
    GameObject CalibratePlane;
    private List<Vector2> listData_temp = new List<Vector2>();
    private List<Vector2> listData = new List<Vector2>();

    private int count = 0;
   
    private void Calibrate()
    {
        if (listData.Count < maxData)
        {
            if (listData_temp.Count < maxDataCollect)
            {
                //Move CalibratePlane to collect raw data position from detetor system
                CalibratePlane.transform.localPosition = count < maxNumX ? new Vector3(count, 0.1f, 0) : new Vector3(0, 0.1f, -((count - maxNumX) % maxNumY));
                //Delay for Detect system track object again after movement
                DelayUIThread(50);
                //Mapping raw coordinate to ScreenCenter coordinate
                listData_temp.Add(new Vector2(SocketClient.xPos - ScreenCenter.x, SocketClient.yPos - ScreenCenter.y));
            }
            else
            {
                listData.Add(new Vector2(listData_temp.Average(point => point.x), listData_temp.Average(point => point.y)));
                listData_temp.Clear();
                count++;
            }
        }
        if (listData.Count == maxData)
        {
            CalibratePlane.SetActive(false);
            listData[maxNumX] = listData[0];
            x0 = listData[0].x;
            y0 = listData[0].y;
            List<float> listX = new List<float>(), listY = new List<float>();
            for (int i = 1; i < maxNumX; i++)
            {
                listX.Add(listData[i].x - listData[i - 1].x);
                if (i < maxNumY)
                {
                    listY.Add(listData[i + maxNumX].y - listData[i + maxNumX - 1].y);
                } 
            }
            deltaX = listX.Average();
            deltaY = listY.Average();
            isCalibrated = true;
            GameObject.Find("Plane").GetComponent<MeshRenderer>().material = Resources.Load("wood", typeof(Material)) as Material;
        }
    }

    private void DelayUIThread(int milliseconds)
    {
        Thread.Sleep(milliseconds);
    }

    private void MovePlane()
    {
        ObjectCenter -= ScreenCenter;
        Vector3 newPos = new Vector3((ObjectCenter.x - x0) / deltaX, 0.1f, (y0 - ObjectCenter.y) / deltaY);
        curPos = newPos;
        ObjFollowToken.transform.localPosition = Vector3.Lerp(ObjFollowToken.transform.localPosition, newPos, Time.deltaTime * 5.0f);
    }

    private void GenerateChild()
    {
        ObjFollowToken = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ObjFollowToken.transform.name = "ABCD";
        ObjFollowToken.transform.SetParent(this.gameObject.transform);
        ObjectCenter -= ScreenCenter;
        ObjFollowToken.transform.localRotation = Quaternion.Euler(0, 0, 0);
        ObjFollowToken.transform.localPosition = new Vector3((ObjectCenter.x - x0) / deltaX, 0.1f, (y0 - ObjectCenter.y) / deltaY);
        ObjFollowToken.transform.localScale = new Vector3(VObjectScale.x, 1, VObjectScale.y);
        ObjFollowToken.GetComponent<MeshRenderer>().material = Resources.Load("white_transparent", typeof(Material)) as Material;
    }

    private void CalculateScale(Vector2 topLeftScreen, Vector2 bottomRightScreen, Vector2 topLeftObject, Vector2 bottomRightObject)
    {
        //Vector2 ScreenSize = bottomRightScreen - topLeftScreen;
        //Vector2 ObjectSize = bottomRightObject - topLeftObject;
        //VObjectScale.x = ObjectSize.x / ScreenSize.x;
        //VObjectScale.y = ObjectSize.y / ScreenSize.y;
        VObjectScale.x = 0.26f;
        VObjectScale.y = 0.38f;
    }

    private Vector2 CalculateCenter(Vector2 topLeftObject, Vector2 bottomRightObject)
    {
        Vector2 objectCenter;
        objectCenter = (topLeftObject + bottomRightObject) / 2;
        return objectCenter;
    }

    private void GetCoordinateFromDetection()
    {
        TopLeftScreen = new Vector2(18, 59);
        BottomRightScreen = new Vector2(564, 433);
        ScreenCenter = CalculateCenter(TopLeftScreen, BottomRightScreen);

        //TopLeftObject = new Vector2(152, 312);
        //BottomRightObject = new Vector2(199, 339);

        //TopLeftObject = new Vector2(424, 344);
        //BottomRightObject = new Vector2(516, 393);
    }
}
