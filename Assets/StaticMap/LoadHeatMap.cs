using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class LoadHeatMap : MonoBehaviour
{

    Text HeatMapBtn;
    GameObject Lighting;
    private bool shouldShowHeatmap = false;
    private readonly static string POPULATION = "Textures/heatmap/population/";
    private readonly static string POLUTION = "Textures/heatmap/polution/";
    // Use this for initialization
    void Start()
    {
        HeatMapBtn = GameObject.Find("HeatmapBtn").GetComponentInChildren<Text>();
        Lighting = GameObject.Find("Directional Light");
        gameObject.SetActive(false);
        GenerateDummy16bitImage();
    }

   
    Bitmap b16bpp;
    int IMAGE_WIDTH = 1024;
    int IMAGE_HEIGHT = 768;
    private void GenerateDummy16bitImage()
    {

        b16bpp = new Bitmap(IMAGE_WIDTH, IMAGE_HEIGHT, PixelFormat.Format16bppRgb565);

        var rect = new Rectangle(0, 0, IMAGE_WIDTH, IMAGE_HEIGHT);
        var bitmapData = b16bpp.LockBits(rect, ImageLockMode.WriteOnly, b16bpp.PixelFormat);
        // Calculate the number of bytes required and allocate them.
        var numberOfBytes = bitmapData.Stride * IMAGE_HEIGHT;
        var bitmapBytes = new short[IMAGE_WIDTH * IMAGE_HEIGHT];
        // Fill the bitmap bytes with random data.
        var random = new Random();
        for (int x = 0; x < IMAGE_WIDTH; x++)
        {
            for (int y = 0; y < IMAGE_HEIGHT; y++)
            {

                var i = ((y * IMAGE_WIDTH) + x); // 16bpp

                // Generate the next random pixel color value.
                var value = (short)Random.Range(0, 65536);

                bitmapBytes[i] = value;         // GRAY
            }
        }
        // Copy the randomized bits to the bitmap pointer.
        var ptr = bitmapData.Scan0;
        Marshal.Copy(bitmapBytes, 0, ptr, bitmapBytes.Length);

        // Unlock the bitmap, we're all done.
        b16bpp.UnlockBits(bitmapData);

        b16bpp.Save("random.jpeg", ImageFormat.Jpeg);

        //FileHelper.WritePNGPicture(bitmapBytes, "E:\\" + "tempdata.png");
        Debug.Log("saved");
    }

    int count = 0;
    int INTERVAL = 100;
    int index = 1;
    int MAX_POOL = 6;
    // Update is called once per frame
    void Update()
    {

        if (!shouldShowHeatmap)
        {
            return;
        }
        if (count % INTERVAL == 0)
        {
            index++;
            if (index > MAX_POOL)
            {
                index = 1;
            }

        }
        count++;
        Texture2D texture = Resources.Load(POLUTION + index) as Texture2D; //No need to specify extension.

        this.GetComponent<Renderer>().material.mainTexture = texture;

    }

    public void HeatMapBtnTrigger()
    {
        shouldShowHeatmap = !shouldShowHeatmap;

        if (shouldShowHeatmap)
        {
            gameObject.SetActive(true);
            Lighting.GetComponent<Light>().intensity = 0.5f;
            HeatMapBtn.text = "Hide Heatmap";
        }
        else
        {
            gameObject.SetActive(false);
            Lighting.GetComponent<Light>().intensity = 1.1f;
            index = 0;
            count = 0;
            HeatMapBtn.text = "Show Heatmap";
        }

    }
}
