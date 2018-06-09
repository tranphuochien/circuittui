using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

public class CalibrateObject : MonoBehaviour
{
    private Vector2 TopLeftScreen;
    private Vector2 BottomRightScreen;
    private Vector2 ScreenCenter;
    private Vector2 TopLeftObject;
    private Vector2 BottomRightObject;
    private Vector2 ObjectCenter;
    private GameObject ObjFollowToken;
    private GameObject BackgroundCalibrate;
    public static Vector3 ScreenWorldPosition;
    private static bool isCalibrated = false;
    public bool ShouldReadConfig = false;
    public static Vector3 curPos = new Vector3(0, 0, 0);
    private float y0;
    private float x0;
    private float deltaY;
    private float deltaX;
    private readonly string CALIBRATE_PLANE = "CalibratePlane";
    private readonly string CONFIG_FILENAME = "config.txt";
    private readonly string MENU = BoardManager.MENU_NAME;

    // Use this for initialization
    void Start()
    {
        BackgroundCalibrate = GameObject.Find("Plane");
        ScreenWorldPosition = this.transform.position;
        GetCoordinateFromDetection();
        CalibratePlane = GameObject.Find(CALIBRATE_PLANE);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCalibrated)
        {
            Calibrate();
        }
        else
        {
            if (DetetorManager.points.Count > 4)
            {
                RoadCameraManager.beginRayCasting = true;
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
    //int maxDataCollect = 30;
    //Number of positions at X(Y) axis need collect data to calibrate
    int maxNumX = 9, maxNumY = 6;
    //Must equal: maxNumX + maxNumY
    int maxData = 15;
    GameObject CalibratePlane;
    private List<Vector2> listData_temp = new List<Vector2>();
    private List<Vector2> listData = new List<Vector2>();

    private int count = 0;
    private bool shouldCollectData = false;

    public void StartCollectData()
    {
        shouldCollectData = true;
    }

    public void StopCollectData()
    {
        shouldCollectData = false;
        CalculateDeltaCalibrate();
    }

    private void Calibrate()
    {
        if (ShouldReadConfig)
        {
            ReadCalibrateConfig();
            BackgroundCalibrate.SetActive(false);
            isCalibrated = true;
            return;
        }
        if (!shouldCollectData)
        {
            return;
        }
        if (listData_temp.Count < Constant.MAX_ENTRY_DATA_COLLECT)
        {
            //Mapping raw coordinate to ScreenCenter coordinate
            Vector2 tmpPoint = new Vector2(DetetorManager.points[4].x, DetetorManager.points[4].y);
            tmpPoint = CoordinateTransition(tmpPoint);
            listData_temp.Add(new Vector2(tmpPoint.x - ScreenCenter.x, tmpPoint.y - ScreenCenter.y));
        }
        else
        {
            listData.Add(new Vector2(listData_temp.Average(point => point.x), listData_temp.Average(point => point.y)));
            listData_temp.Clear();
            count++;
            //Move CalibratePlane to collect raw data position from detetor system
            CalibratePlane.transform.localPosition = count < maxNumX ? new Vector3(count, 0.1f, 0) : new Vector3(0, 0.1f, -((count - maxNumX) % maxNumY));
            Debug.Log("Finish collect data for point");
            shouldCollectData = false;
        }
    }

    private void CalculateDeltaCalibrate()
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

        WriteCalibrateConfigToFile(x0, y0, deltaX, deltaY);
        //Hide Plane calibrate
        BackgroundCalibrate.SetActive(false);
    }

    private void WriteCalibrateConfigToFile(float x0, float y0, float deltaX, float daltaY)
    {
        ConfigModel configModel = new ConfigModel(x0, y0, deltaX, deltaY);
        string json = JsonUtility.ToJson(configModel);

        using (StreamWriter writetext = new StreamWriter(CONFIG_FILENAME))
        {
            writetext.WriteLine(json);
        }
    }

    private void ReadCalibrateConfig()
    {
        using (StreamReader readText = new StreamReader(CONFIG_FILENAME))
        {
            string s = readText.ReadLine();
            if (s != "")
            {
                ConfigModel configModel = JsonUtility.FromJson<ConfigModel>(s);
                x0 = configModel.x0;
                y0 = configModel.y0;
                deltaX = configModel.deltaX;
                deltaY = configModel.deltaY;
                Debug.Log("Read config successfully: [" + s + "]");
            }
        }
    }

    private void DelayUIThread(int milliseconds)
    {
        Thread.Sleep(milliseconds);
    }

    private void MovePlane()
    {
        ObjectCenter -= ScreenCenter;
        Vector3 newPos = new Vector3((ObjectCenter.x - x0) / deltaX, zPlaneChild, (y0 - ObjectCenter.y) / deltaY);

        //Vector3 newPos = new Vector3((ObjectCenter.x + -3.5186307430267336f) / 4.503023147583008f, zPlaneChild, (2.2618608474731447f - ObjectCenter.y) / 4.2054057121276859f);
        curPos = newPos;
        ObjFollowToken.transform.localPosition = Vector3.Lerp(ObjFollowToken.transform.localPosition, newPos, Time.deltaTime * 5.0f);
    }

    float zPlaneChild = 0f;

    private void GenerateChild()
    {
        ObjFollowToken = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ObjFollowToken.transform.name = "TrackingPlane";
        ObjFollowToken.transform.SetParent(this.gameObject.transform);
        //ObjFollowToken.transform.SetParent(GameObject.Find("Map").transform);
        ObjectCenter -= ScreenCenter;
        ObjFollowToken.transform.localRotation = Quaternion.Euler(0, 0, 0);
        ObjFollowToken.transform.localPosition = new Vector3((ObjectCenter.x - x0) / deltaX, zPlaneChild, -(y0 - ObjectCenter.y) / deltaY);
        ObjFollowToken.transform.localScale = new Vector3(0.2f, 1, 0.2f);
        ObjFollowToken.GetComponent<MeshRenderer>().material = Resources.Load("white 2", typeof(Material)) as Material;
    }

    private Vector2 CalculateCenter(Vector2 topLeftObject, Vector2 bottomRightObject)
    {
        Vector2 objectCenter;
        objectCenter = (topLeftObject + bottomRightObject) / 2;
        return objectCenter;
    }

    private Vector2 CoordinateTransition(Vector2 old)
    {
        return new Vector2(old.x, Constant.FEED_DETECTOR_HEIGHT - old.y);
    }

    private void GetCoordinateFromDetection()
    {
        TopLeftScreen = CoordinateTransition(new Vector2(Constant.TOPLEFT_X_SCREEN_PROJECTOR, Constant.TOPLEFT_Y_SCREEN_PROJECTOR));
        BottomRightScreen = CoordinateTransition(new Vector2(Constant.RIGHTBOTTOM_X_SCREEN_PROJECTOR, Constant.RIGHTBOTTOM_Y_SCREEN_PROJECTOR));
        ScreenCenter = CalculateCenter(TopLeftScreen, BottomRightScreen);
    }
}
