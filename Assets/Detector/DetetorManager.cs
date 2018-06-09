using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class DetetorManager : MonoBehaviour
{

    private KeyboardDLL keyboardDLL = KeyboardDLL.getInstance();
    public static List<Vector2> points = new List<Vector2>();
    List<Vector2> pointsInDepth = new List<Vector2>();
    int countDisapear = 0;
    private KinectSensor _Sensor;
    private InfraredFrameReader _Reader;
    private ushort[] _Data;
    private byte[] _RawData;
    private byte[] rawByte;
    public bool a = false;
    public int delayFrame = 5;
    int count = 0;
    SocketV2 socket;
    private GameObject socketController;
    uint nPixelsFrame;

    void Start()
    {
        socketController = GameObject.Find("SocketController");
        socket = socketController.GetComponent<SocketV2>();
        _Sensor = KinectSensor.GetDefault();
        if (_Sensor != null)
        {
            _Reader = _Sensor.InfraredFrameSource.OpenReader();
            var frameDesc = _Sensor.InfraredFrameSource.FrameDescription;
            _Data = new ushort[frameDesc.LengthInPixels];
            _RawData = new byte[frameDesc.LengthInPixels * 4];
            nPixelsFrame = frameDesc.LengthInPixels;
            Constant.KINECT_IR_WIDTH = frameDesc.Width;
            Constant.KINECT_IR_HEIGHT = frameDesc.Height;
            Debug.Log("Width: " + Constant.KINECT_IR_WIDTH + " Height: " + Constant.KINECT_IR_HEIGHT + " " + frameDesc.LengthInPixels);

            if (!_Sensor.IsOpen)
            {
                Debug.Log("Kinect is opened");
                _Sensor.Open();
            }
            rawByte = new byte[Constant.FEED_DETECTOR_WIDTH * Constant.FEED_DETECTOR_HEIGHT];
        }
    }

    public static Vector2 GetPointCenter()
    {
        if (points.Count > 4)
        {
            return new Vector2(points[4].x, points[4].y);
        }
        return new Vector2(0, 0);
    }

    unsafe void Update()
    {
        if (a)
        {
            if (count % delayFrame == 0)
            {

                Vector2 preparedData = new Vector2(CalibrateObject.curPos.x + CalibrateObject.ScreenWorldPosition.x,
                     CalibrateObject.curPos.z + CalibrateObject.ScreenWorldPosition.y);
                socket.SendData(Constant.TOKEN_BEGIN_POSITION + preparedData.x + Constant.TOKEN_SPLIT + preparedData.y + Constant.TOKEN_END);
            }
            count++;
            //a = false;
        }
        if (_Reader != null)
        {
            var frame = _Reader.AcquireLatestFrame();
            if (frame != null)
            {
                frame.CopyFrameDataToArray(_Data);

                int index = 0;
                bool isInRangeX, isInRangeY;
                byte intensity;
                int count = 0;
                foreach (var ir in _Data)
                {
                    count++;
                    intensity = (byte)(ir >> 8);
                    isInRangeX = (count % Constant.KINECT_IR_WIDTH >= Constant.LEFT_X_FEED_DETECTOR &&
                        count % Constant.KINECT_IR_WIDTH < Constant.RIGHT_X_FEED_DETECTOR);
                    isInRangeY = count / Constant.KINECT_IR_WIDTH >= Constant.TOP_Y_FEED_DETECTOR &&
                        count / Constant.KINECT_IR_WIDTH < Constant.BOTTOM_Y_FEED_DETECTOR;

                    if (isInRangeX && isInRangeY)
                    {
                        rawByte[index++] = intensity;
                    }
                }


                fixed (byte* data = rawByte)
                {
                    //process
                    keyboardDLL.feedData(data, Constant.FEED_DETECTOR_WIDTH, Constant.FEED_DETECTOR_HEIGHT);
                }

                if (keyboardDLL.isKeyboardIn())   //has keyboard
                {
                    //keyboard is in, do something, control the yellow box
                    points = keyboardDLL.getKeyboardPos();

                    //float angle = keyboardDLL.getAngle();
                    countDisapear = 0;
                }

                frame.Dispose();
                frame = null;
            }
        }
    }
}
