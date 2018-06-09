using System;

[Serializable]
public class ConfigModel
{
    public float x0;

    public float y0;

    public float deltaX;

    public float deltaY;

    public ConfigModel(float x0, float y0, float deltaX, float deltaY)
    {
        this.x0 = x0;
        this.y0 = y0;
        this.deltaX = deltaX;
        this.deltaY = deltaY;
    }
}
