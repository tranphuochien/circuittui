using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Windows.Kinect;
using System.Drawing;
using System.IO;
using System;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

public class DemoDepthScreen : MonoBehaviour {


    private KinectSensor _Sensor;
    private InfraredFrameReader _Reader;
    private ushort[] _Data;
    private byte[] _RawData;
    private byte[] rawByte;
    private Thread mThread;
    private static bool isRenamed = true;
    private const string DATA_FILE = "data.png";
    private const string TEMP_DATA_FILE = "tempdata.png";
    public GameObject plane;

    private int mCount = 0;
    byte[] src;

    // I'm not sure this makes sense for the Kinect APIs
    // Instead, this logic should be in the VIEW
    private Texture2D _Texture;

    public Texture2D GetInfraredTexture()
    {
        return _Texture;
    }

    public byte[] getRawByte()
    {
        return rawByte;
    }

    private Color32[] convertDepthToColor(byte[] depthBuf)
    {
        Color32[] img = new Color32[depthBuf.Length];
        for (int pix = 0; pix < depthBuf.Length; pix++)
        {
            img[pix].r = (byte)(depthBuf[pix] / 32);
            img[pix].g = (byte)(depthBuf[pix] / 32);
            img[pix].b = (byte)(depthBuf[pix] / 32);
        }
        return img;
    }


    void writePicture()
    {
        FileHelper.WritePNGPicture(src, "E:\\" + TEMP_DATA_FILE, false, ref isRenamed);
    }



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
            _Texture = new Texture2D(frameDesc.Width, frameDesc.Height, TextureFormat.BGRA32, false);

            //Global.InfraredSize = new Vector2(frameDesc.Width, frameDesc.Height);
            //Debug.Log("Size " + _Data.Length);

            if (!_Sensor.IsOpen)
            {
                Debug.Log("Kinect is opened");
                _Sensor.Open();
            }
        }
    }
    Boolean trigger = true;
    void Update()
    {
        /*if (mThread != null)
        {
            if (mThread.IsAlive)
            {
            }
            else
            {
                if (!isRenamed)
                {
                    if (File.Exists("E:\\" + DATA_FILE))
                    {
                        System.IO.File.Delete("E:\\" + DATA_FILE);
                    }

                    System.IO.File.Move("E:\\" + TEMP_DATA_FILE, "E:\\" + DATA_FILE);
                    isRenamed = true;
                }
            }
        }*/

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
                _Texture.LoadRawTextureData(_RawData);
                _Texture.Apply();

                plane.GetComponent<Renderer>().material.mainTexture = _Texture;
               

                //Write to file
                /*src = _Texture.EncodeToPNG();

                if (mCount == 50)
                {
                    mThread = new Thread(writePicture);
                    mThread.Start();

                }
                mCount = (++mCount) % FileHelper.DELAY_INFARED_FRAME_WRITE;
                */

                frame.Dispose();
                frame = null;
            }
        }
    }

    void OnApplicationQuit()
    {
        if (_Reader != null)
        {
            _Reader.Dispose();
            _Reader = null;
        }

        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }

            _Sensor = null;
        }
    }
    
}
