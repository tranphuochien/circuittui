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
    //public static GameObject listCubes;
    private static bool isCalibrated = false;
    public static Vector3 curPos = new Vector3(0, 0, 0);
    private float y0;
    private float x0;
    private float deltaY;
    private float deltaX;
    private readonly string CALIBRATE_PLANE = "CalibratePlane";
    private readonly string MENU = BoardManager.MENU_NAME;
    private const int KINECTX = 512;
    private const int KINECTY = 424;

    // Use this for initialization
    void Start () {
        //Find gameobject keep world map 
        //listCubes = GameObject.Find("ListCubes");
        //listCubes.SetActive(false);

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
                //listCubes.SetActive(true);
                //RayCastController.beginRayCasting = true;
                RoadCameraManager.beginRayCasting = true;
                if (GameObject.Find(CALIBRATE_PLANE) != null)
                {
                    GameObject.Find(CALIBRATE_PLANE).SetActive(false);
                    GameObject.Find(MENU).SetActive(true);
                }
                //ObjectCenter = new Vector2(SocketClient.xPos, SocketClient.yPos);
                
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
        } else // test calibrate kinect
        {
            if (DetetorManager.points.Count > 4)
            {
                ObjectCenter = new Vector2(DetetorManager.points[4].x, DetetorManager.points[4].y);
                ObjectCenter = CoordinateTransition(ObjectCenter);
                if (ObjFollowToken == null)
                {
                    GenerateChild();
                }
                else
                {
                    MovePlane();
                }
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
                DelayUIThread(30);
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
            GameObject.Find("Map").transform.position = new Vector3(13.57f, 9.94f, -2.71f);
        }
    }

    private void DelayUIThread(int milliseconds)
    {
        Thread.Sleep(milliseconds);
    }

    private void MovePlane()
    {
        ObjectCenter -= ScreenCenter;
        //Vector3 newPos = new Vector3((ObjectCenter.x - x0) / deltaX, zPlaneChild, (y0 - ObjectCenter.y) / deltaY);
        Vector3 newPos = new Vector3((ObjectCenter.x + 2.5f) / 25, zPlaneChild, (0.5f - ObjectCenter.y) / 19);
        curPos = newPos;
        ObjFollowToken.transform.localPosition = Vector3.Lerp(ObjFollowToken.transform.localPosition, newPos, Time.deltaTime * 5.0f);
    }

    float zPlaneChild = 2.75f;

    private void GenerateChild()
    {
        ObjFollowToken = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ObjFollowToken.transform.name = "ABCD";
        ObjFollowToken.transform.SetParent(this.gameObject.transform);
        //ObjFollowToken.transform.SetParent(GameObject.Find("Map").transform);
        ObjectCenter -= ScreenCenter;
        ObjFollowToken.transform.localRotation = Quaternion.Euler(0, 0, 0);
        //ObjFollowToken.transform.localPosition = new Vector3((ObjectCenter.x - x0) / deltaX, zPlaneChild, -(y0 - ObjectCenter.y) / deltaY);
        ObjFollowToken.transform.localPosition = new Vector3((ObjectCenter.x + 2.5f) / 25, zPlaneChild, (0.5f - ObjectCenter.y) / 19);
        ObjFollowToken.transform.localScale = new Vector3(VObjectScale.x, 1, VObjectScale.y);
        ObjFollowToken.GetComponent<MeshRenderer>().material = Resources.Load("white 2", typeof(Material)) as Material;
    }

    private void CalculateScale(Vector2 topLeftScreen, Vector2 bottomRightScreen, Vector2 topLeftObject, Vector2 bottomRightObject)
    {
        //Vector2 ScreenSize = bottomRightScreen - topLeftScreen;
        //Vector2 ObjectSize = bottomRightObject - topLeftObject;
        //VObjectScale.x = ObjectSize.x / ScreenSize.x;
        //VObjectScale.y = ObjectSize.y / ScreenSize.y;
        VObjectScale.x = 0.1f;
        VObjectScale.y = 0.1f;
    }

    private Vector2 CalculateCenter(Vector2 topLeftObject, Vector2 bottomRightObject)
    {
        Vector2 objectCenter;
        objectCenter = (topLeftObject + bottomRightObject) / 2;
        return objectCenter;
    }

    private Vector2 CoordinateTransition(Vector2 old)
    {
        return new Vector2(old.x, KINECTY - old.y);
    }

    private void GetCoordinateFromDetection()
    {
        //TopLeftScreen = new Vector2(97, 60);
        //BottomRightScreen = new Vector2(595, 400);
        TopLeftScreen = CoordinateTransition(new Vector2(136, 250));
        BottomRightScreen = CoordinateTransition(new Vector2(373, 87));
        ScreenCenter = CalculateCenter(TopLeftScreen, BottomRightScreen);

        //TopLeftObject = new Vector2(152, 312);
        //BottomRightObject = new Vector2(199, 339);

        //TopLeftObject = new Vector2(424, 344);
        //BottomRightObject = new Vector2(516, 393);
    }
}
