using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class DetetorManager : MonoBehaviour {


 
    private KeyboardDLL keyboardDLL = KeyboardDLL.getInstance();
    List<Vector2> pointsInDepth = new List<Vector2>();
    
 
    List<GameObject> spots = new List<GameObject>();

    int countDisapear = 0;


    private KinectSensor _Sensor;
    private InfraredFrameReader _Reader;
    private ushort[] _Data;
    private byte[] _RawData;
    private byte[] rawByte;

 
    int width, height;
    void Start()
    {
        _Sensor = KinectSensor.GetDefault();
        if (_Sensor != null)
        {
            _Reader = _Sensor.InfraredFrameSource.OpenReader();
            var frameDesc = _Sensor.InfraredFrameSource.FrameDescription;
            _Data = new ushort[frameDesc.LengthInPixels];
            _RawData = new byte[frameDesc.LengthInPixels * 4];
            rawByte = new byte[frameDesc.LengthInPixels];
            width = frameDesc.Width;
            height = frameDesc.Height;
            Debug.Log(width + "  " + height);
            //Global.InfraredSize = new Vector2(frameDesc.Width, frameDesc.Height);
            //Debug.Log("Size " + _Data.Length);

            if (!_Sensor.IsOpen)
            {
                Debug.Log("Kinect is opened");
                _Sensor.Open();
            }
        }
    }

    // Update is called once per frame
    unsafe void Update()
    {

        if (_Reader != null)
        {
            var frame = _Reader.AcquireLatestFrame();
            if (frame != null)
            {
                frame.CopyFrameDataToArray(_Data);

                int index = 0;
                int index2 = 0;
                foreach (var ir in _Data)
                {
                    byte intensity = (byte)(ir >> 8);
                    _RawData[index++] = intensity;
                    _RawData[index++] = intensity;
                    _RawData[index++] = intensity;
                    _RawData[index++] = 255; // Alpha
                    rawByte[index2++] = intensity;
                }
                fixed (byte* data = rawByte)
                {
                    //process
                    keyboardDLL.feedData(data, width, height);
                }

                if (keyboardDLL.isKeyboardIn())   //has keyboard
                {
                    //keyboard is in, do something, control the yellow box
                    List<Vector2> points = keyboardDLL.getKeyboardPos();
                   

                    Vector2 center = points[4];
                    Debug.Log(center.x + "  " + center.y);

                    float angle = keyboardDLL.getAngle();



                    countDisapear = 0;
                }

                frame.Dispose();
                frame = null;
            }
        }




       
    }
}
