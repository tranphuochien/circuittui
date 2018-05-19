using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using System.Linq;

public class GenerateChildObject : MonoBehaviour {

    private Vector2 TopLeftScreen;
    private Vector2 BottomRightScreen;
    private Vector2 ScreenCenter;
    private Vector2 TopLeftObject;
    private Vector2 BottomRightObject;
    private Vector2 ObjectCenter;
    private Vector2 VObjectScale;
    public GameObject plane;
    public static GameObject listCubes;
    private static bool isCalibrated = false;
    public static Vector3 curPos = new Vector3(0, 0, 0);
    private float y0;
    private float x0;
    private float deltaY;
    private float deltaX;

    // Use this for initialization
    void Start () {
        listCubes = GameObject.Find("ListCubes");
        listCubes.SetActive(false);
        plane = GameObject.Find("Menu");
        GetCoordinateFromDetection();
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
                if (GameObject.Find("Phone") != null)
                {
                    GameObject.Find("Phone").SetActive(false);
                    GameObject.Find("Menu").SetActive(true);
                }
                //ObjectCenter = CalculateCenter(TopLeftObject, BottomRightObject);
                ObjectCenter = new Vector2(SocketClient.xPos, SocketClient.yPos);
                CalculateScale(TopLeftScreen, BottomRightScreen, TopLeftObject, BottomRightObject);
                if (plane == null)
                {
                    GenerateChild();
                }
                else
                {
                    MovePlane();
                }
            } else
            {
                if (GameObject.Find("Menu"))
                {
                    GameObject.Find("Menu").SetActive(false);
                    GameObject.Find("Phone").SetActive(true);
                }
                phone = GameObject.Find("Phone");
                //CalibrateCoordination();
                Calibrate();
            }
        }
        
    }

    int maxData_temp = 90;
    int maxData = 12;
    GameObject phone;
    private List<Vector2> listData_temp = new List<Vector2>();
    private List<Vector2> listDataY_temp = new List<Vector2>();
    private List<Vector2> listData = new List<Vector2>();
    private List<Vector2> listDataY = new List<Vector2>();
    private int a = 0;
   
    private void Calibrate()
    {
        if (listData.Count < maxData)
        {
            if (listData_temp.Count < maxData_temp)
            {
                phone.transform.localPosition = a < 6 ? new Vector3(a, 0.1f, 0) : new Vector3(0, 0.1f, -(a % 6));
                for (int temp = 0; temp < 500; temp++) ;
                listData_temp.Add(new Vector2(SocketClient.xPos - ScreenCenter.x, SocketClient.yPos - ScreenCenter.y));
            }
            else
            {
                listData.Add(new Vector2(listData_temp.Average(point => point.x), listData_temp.Average(point => point.y)));
                listData_temp.Clear();
                a++;
            }
        }
        if (listData.Count == maxData)
        {
            phone.SetActive(false);
            listData[6] = listData[0];
            x0 = listData[0].x;
            y0 = listData[0].y;
            List<float> listX = new List<float>(), listY = new List<float>();
            for (int i = 1; i <= 5; i++)
            {
                listX.Add(listData[i].x - listData[i - 1].x);
                listY.Add(listData[i + 6].y - listData[i + 5].y);
            }
            deltaX = listX.Average(); //(listData[3].x - listData[0].x) / 3.0f;
            deltaY = listY.Average(); //(listData[7].y - listData[4].y) / 3.0f;
            isCalibrated = true;
        }
    }

    private void MovePlane()
    {
        ObjectCenter -= ScreenCenter;
        Vector3 newPos = new Vector3((ObjectCenter.x - x0) / deltaX, 0.1f, (y0 - ObjectCenter.y) / deltaY);
        curPos = newPos;
        plane.transform.localPosition = Vector3.Lerp(plane.transform.localPosition, newPos, Time.deltaTime * 5.0f);
    }

    private void GenerateChild()
    {
        plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.name = "ABCD";
        plane.transform.SetParent(this.gameObject.transform);
        ObjectCenter -= ScreenCenter;
        plane.transform.localRotation = Quaternion.Euler(0, 0, 0);
        plane.transform.localPosition = new Vector3((ObjectCenter.x - x0) / deltaX, 0.1f, (y0 - ObjectCenter.y) / deltaY);
        plane.transform.localScale = new Vector3(VObjectScale.x, 1, VObjectScale.y);
        plane.GetComponent<MeshRenderer>().material = Resources.Load("white", typeof(Material)) as Material;
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
