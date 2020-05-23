using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public partial class HqRenderer : MonoBehaviour
{
    public RenderWindow Renderer;
    public SpriteRenderer BG;
    public SpriteRenderer FG;
    public int PPU;

    public TrackObject track;

    public Material grass1;
    public Material grass2;
    public Material rumble1;
    public Material rumble2;
    public Material road1;
    public Material road2;

    public int screenWidthRef = 320;
    public int screenHeightRef = 240;
    public float cameraDepth = 0.84f; //camera depth [0..1]
    public int DravingDistance = 300; //segments
    public int cameraHeight = 1500; //pixels?
    [NonSerialized]
    int screenWidth2;
    [NonSerialized]
    int screenHeight2;

    public bool drawRoad;
    public bool drawSprites;
    public int rumbleWidth;
    public float SpriteScale;
    public int xf = 2;
    public int yd = 2;

    int trip = 0; //pixels
    float playerX = 0;
    float playerY = 0;
    float playerZ = 0;

    [NonSerialized]
    private Texture2D[] t;
    [NonSerialized]
    Mesh[][] Meshes = new Mesh[4][];

    Quad quad;
    private RenderTexture _renderTexture;

    public void drawSprite(ref Line line)
    {
        if (line.Y < -screenHeight2) { return; }
        Sprite s = line.sprite;
        if (s == null) { return; }
        var w = s.rect.width;
        var h = s.rect.height;

        float destX = line.X + line.W * line.spriteX + screenWidth2;
        float destY = -line.Y + screenHeight2;
        float destW = w * line.scale * screenWidth2 * SpriteScale;
        float destH = h * line.scale * screenWidth2 * SpriteScale;

        destX += destW*Mathf.Sign(line.spriteX)/2; //offsetX
        destY += destH * (-1);    //offsetY

        float clipH = -line.Y + line.clip;
        if (clipH < 0) clipH = 0;

        if (clipH >= destH) return;

        Rect target = new Rect(destX , destY , destW, destH);
        Rect source = new Rect(Vector2Int.zero, new Vector2(1, 1-clipH / destH));
        Renderer.draw(source, s, target);
    }

    void drawQuad(Mesh mesh, Material c, float x1, float y1, float w1, float x2, float y2, float w2)
    {
        quad.SetQuad(x1 / PPU, y1 / PPU, w1 / PPU, x2 / PPU, y2 / PPU, w2 / PPU);
        Renderer.draw(quad.ToMesh(mesh), c);
    }

    private void OnEnable()
    {
        Camera.onPostRender += PostRender;
    }
    private void OnDisable()
    {
        Camera.onPostRender -= PostRender;
    }

    void Awake()
    {
        for (int i = 0; i < Meshes.Length; i++)
        {
            Meshes[i] = new Mesh[DravingDistance];
            for (int j = 0; j < DravingDistance; j++)
            {
                Meshes[i][j] = new Mesh();
            }
        }

        Renderer = new RenderWindow();

        quad = new Quad();

        Texture2D tex = new Texture2D(screenWidthRef, screenHeightRef, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        FG.sprite = Sprite.Create(tex, new Rect(0, 0, screenWidthRef, screenHeightRef), new Vector2(0.5f,0.5f), PPU);
        FG.sprite.name = "runtime";

    }

    private void PostRender(Camera cam)
    {
        //Draw();
    }

    void Update()
    {
        Draw();
    }

    void Draw()
    {

        int speed = 0;
        if (Input.GetKey(KeyCode.RightArrow)) playerX += 0.1f;
        if (Input.GetKey(KeyCode.LeftArrow)) playerX -= 0.1f;
        if (Input.GetKey(KeyCode.UpArrow)) speed = 200;
        if (Input.GetKey(KeyCode.DownArrow)) speed = -200;
        //if (Keyboard::isKeyPressed(Keyboard::Tab)) speed *= 3;
        if (Input.GetKey(KeyCode.W)) cameraHeight += 100;
        if (Input.GetKey(KeyCode.S)) cameraHeight -= 100;

        trip += speed;
        while (trip >= track.Length * track.segmentLength) trip -= track.Length * track.segmentLength;
        while (trip < 0) trip += track.Length * track.segmentLength;

        int startPos = trip / track.segmentLength;
        playerZ = trip + cameraHeight * cameraDepth; // car is in front of cammera
        int playerPos = (int)(playerZ / track.segmentLength) % track.lines.Length;
        playerY = track.lines[playerPos].y;
        int camH = (int)(track.lines[playerPos].y + cameraHeight);

        if (speed > 0) BG.transform.localPosition += new Vector3(-track.lines[startPos].curve/PPU, 0);
        if (speed < 0) BG.transform.localPosition += new Vector3(track.lines[startPos].curve/PPU, 0);

        screenWidth2 = screenWidthRef / 2;
        screenHeight2 = screenHeightRef / 2;

        float maxy = -screenHeight2;
        int counter = 0;
        float x = 0, dx = 0;
        float res = 1f / PPU;

        Debug.DrawLine(new Vector3(-1 * xf, 0 + yd, 0), new Vector3(1 * xf, 0 + yd, 0));
        Debug.DrawLine(new Vector3(1 * xf, 0 + yd, 0), new Vector3(1 * xf, -1 + yd, 0));
        Debug.DrawLine(new Vector3(1 * xf, -1 + yd, 0), new Vector3(-1 * xf, -1 + yd, 0));
        Debug.DrawLine(new Vector3(-1 * xf, -1 + yd, 0), new Vector3(-1 * xf, 0 + yd, 0));
        ///////draw road////////
        for (int n = startPos + 1; n < startPos + DravingDistance; n++)
        {
            ref Line l = ref track.lines[n % track.Length];
            l.project(
                (int)(playerX * track.roadWidth - x),
                camH,
                startPos * track.segmentLength - (n >= track.Length ? track.Length * track.segmentLength : 0),
                screenWidth2,
                screenHeight2,
                cameraDepth);
            x += dx;
            dx += l.curve;

            l.clip = maxy;
            if (l.Y <= maxy)
            {
                continue;
            }
            maxy = l.Y;

            Material grass = (n / 3 / 3) % 2 == 0 ? grass1 : grass2;
            Material rumble = (n / 3) % 2 == 0 ? rumble1 : rumble2;
            Material road = (n / 3 / 2) % 2 == 0 ? road1 : road2;

            ref Line p = ref track.lines[(n - 1) % track.Length]; //previous line

            if (Mathf.Abs(l.Y - p.Y) < res)
            {
                continue;
            }

            if (drawRoad)
            {
                drawQuad(Meshes[0][counter], grass, 0, p.Y, screenWidthRef, 0, l.Y, screenWidthRef);
                drawQuad(Meshes[1][counter], rumble, p.X, p.Y, p.W + p.scale*rumbleWidth*screenWidth2, l.X, l.Y, l.W + l.scale * rumbleWidth * screenWidth2);
                drawQuad(Meshes[2][counter], road, p.X, p.Y, p.W, l.X, l.Y, l.W);
            }
            //if ((n / 3) % 2 == 0)
            //{
            //    drawQuad(Meshes[3][counter], rumble1, p.X, p.Y * 1.1f, p.W * 0.05f, l.X, l.Y * 1.1f, l.W * 0.05f);
            //}

            counter++;
        }
        ////////draw objects////////
        if (drawSprites)
        {
            _renderTexture = RenderTexture.GetTemporary(screenWidthRef, screenHeightRef);
            //Sprite sprite = Sprite.Create(_renderTexture, new Rect(0,0,screenWidth,screenHeight), Vector2.zero, PPU);
            RenderTexture currentActiveRT = RenderTexture.active;
            RenderTexture.active = _renderTexture;
            //Work in the pixel matrix of the texture resolution.
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, screenWidthRef, screenHeightRef, 0);
            GL.Clear(false, true, new Color(0,0,0,0));
            //Graphics.Blit(_renderTexture)
            //Renderer.draw(BG.sprite, new Rect(0, 0, screenWidth, screenHeight));
            for (int n = startPos + DravingDistance; n > startPos; n--)
            {
                drawSprite(ref track.lines[n % track.Length]);
            }
            //textureToSprite(_renderTexture, FG);
            //FG.sprite.texture.ReadPixels(new Rect(0, 0, screenWidthRef, screenHeightRef), 0,0);
            //FG.sprite.texture.Apply();
            Graphics.CopyTexture(_renderTexture, FG.sprite.texture);
            //Revert the matrix and active render texture.
            GL.PopMatrix();
            RenderTexture.active = currentActiveRT;
            RenderTexture.ReleaseTemporary(_renderTexture);
        }
    }
}
