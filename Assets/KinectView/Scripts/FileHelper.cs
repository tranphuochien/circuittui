using System;
using System.Drawing;
using System.IO;
using UnityEngine;

public static class FileHelper
{
    private static readonly String POST_FIX_PNG = ".png";
    public static readonly int DELAY_INFARED_FRAME_WRITE = 100;
    public static readonly int DELAY_COLOR_FRAME_WRITE = 300;

    public static void WritePNGPicture(byte[] data, string nameFile, bool isOverWrite, ref bool isRenamed)
    {
        MemoryStream ms = new MemoryStream(data);
        Image image = Image.FromStream(ms);
        String name;

        if (isOverWrite)
        {
            name = nameFile + POST_FIX_PNG;
        }
        else
        {
            //String timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            name = nameFile;
        }

        image.Save(name);
        Debug.Log("Finish write " + name);
        isRenamed = false;
    }

    public static void WritePNGPicture(byte[] data, string nameFile)
    {
        MemoryStream ms = new MemoryStream(data);
        Image image = Image.FromStream(ms);
        String name;


        //String timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        name = nameFile;

        image.Save(name);
        Debug.Log("Finish write " + name);
    }
}
