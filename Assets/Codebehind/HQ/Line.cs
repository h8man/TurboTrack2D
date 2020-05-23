using UnityEngine;
public struct Line
{
    public float x, y, z, w;//3d center of line
    public float X, Y, W; //screen coord
    public float curve, spriteX, clip, scale;
    public Sprite sprite;

    public void project(int camX, int camY, int camZ, int screenWidth2, int screenHeight2, float cameraDepth)
    {
        scale = cameraDepth / (z - camZ);
        X = scale * (x - camX) * screenWidth2;
        Y = scale * (y - camY) * screenHeight2;
        W = scale * w * screenWidth2;
    }
}
