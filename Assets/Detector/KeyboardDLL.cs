using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class KeyboardDLL
{
    static KeyboardDLL _instance;

    private IntPtr helper;

    [DllImport("KeyboardDLL", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr CreateKeyboardHelper();

    [DllImport("KeyboardDLL", CallingConvention = CallingConvention.Cdecl)]
    private unsafe static extern void feedData(IntPtr ptr, byte* data, int width, int height);

    [DllImport("KeyboardDLL", CallingConvention = CallingConvention.Cdecl)]
    private unsafe static extern bool isKeyboardIn(IntPtr ptr);

    [DllImport("KeyboardDLL", CallingConvention = CallingConvention.Cdecl)]
    private unsafe static extern float* getKeyboardPos(IntPtr ptr);

    [DllImport("KeyboardDLL", CallingConvention = CallingConvention.Cdecl)]
    private unsafe static extern float getAngle(IntPtr ptr);

    [DllImport("KeyboardDLL", CallingConvention = CallingConvention.Cdecl)]
    private unsafe static extern void DisposeKeyboardHelper(IntPtr ptr);

    [DllImport("KeyboardDLL", CallingConvention = CallingConvention.Cdecl)]
    private unsafe static extern bool checkErrorTouch(IntPtr ptr, float x, float y);

    [DllImport("KeyboardDLL", CallingConvention = CallingConvention.Cdecl)]
    private unsafe static extern float* getSpeakerPos(IntPtr ptr);

    [DllImport("KeyboardDLL", CallingConvention = CallingConvention.Cdecl)]
    private unsafe static extern float* getKeyboardPosInIR(IntPtr ptr);

    public static KeyboardDLL getInstance()
    {
        if (_instance == null)
            _instance = new KeyboardDLL();
        return _instance;
    }

    public KeyboardDLL()
    {
        helper = CreateKeyboardHelper();
    }

    public unsafe void feedData(byte* data, int width, int height)
    {
        feedData(helper, data, width, height);
    }

    public bool isKeyboardIn()
    {
        return isKeyboardIn(helper);
    }

    unsafe public List<Vector2> getKeyboardPos()
    {
        List<Vector2> res = new List<Vector2>();
        float* tmp = getKeyboardPos(helper);
        for (int i = 0; i < 5; i++)
            res.Add(new Vector2(tmp[i * 2], tmp[i * 2 + 1]));
        return res;
    }

    public bool isErrorTouch(Vector2 posInIR)
    {
        return checkErrorTouch(helper, posInIR.x, posInIR.y);
    }

    public float getAngle()
    {
        return getAngle(helper);
    }

    public unsafe Vector2 getSpeakerPositionInIR()
    {
        float* pos = getSpeakerPos(helper);
        if (pos[0] == 0)
            return new Vector2(-1001, -1001);
        return new Vector2(pos[1], pos[2]);
    }

    public unsafe Vector2 getKeyboardPositionInIR()
    {
        float* pos = getKeyboardPosInIR(helper);

        if (pos[0] == 0)
            return new Vector2(-1001, -1001);
        float z = pos[3];
        return new Vector2(pos[1], pos[2]);
    }

    ~KeyboardDLL()
    {
        DisposeKeyboardHelper(helper);
    }
}
