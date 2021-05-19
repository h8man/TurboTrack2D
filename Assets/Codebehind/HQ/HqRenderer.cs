using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HqRenderer : MonoBehaviour
{
    public RenderWindow Renderer;

    public Camera targetCamera;
    public SpriteRenderer BG;
    public SpriteRenderer Plane;
    public SpriteRenderer FG;
    public Sprite BGSprite;
    public int PPU;

    public TrackObject track;

    public Material grass1;
    public Material grass2;
    public Material rumble1;
    public Material rumble2;
    public Material road1;
    public Material road2;
    public Material dashline;

    public int screenWidthRef = 320;
    public int screenHeightRef = 240;
    public float cameraDepth = 0.84f; //camera depth [0..1]
    public int DravingDistance = 300; //segments
    public int cameraHeight = 1500; //pixels?
    public float centrifugal = 0.1f;
    public float paralaxSpeed = 0.1f;
    public bool drawRoad;
    public bool drawSprites;
    public int rumbleWidth;
    public float SpriteScale;

    [NonSerialized]
    int screenWidth2;
    [NonSerialized]
    int screenHeight2;
    [NonSerialized]
    Mesh[] combined;
    [NonSerialized]
    Dictionary<Material, Quad> dictionary = new Dictionary<Material, Quad>();
    [NonSerialized]
    private Material[] materials;

    //[NonSerialized]
    public int trip = 0; //pixels
    [NonSerialized]
    float playerX = 0;
    [NonSerialized]
    float playerY = 0;
    [NonSerialized]
    float playerZ = 0;

    [NonSerialized]
    private int startPos;
    [NonSerialized]
    private int playerPos;

    [NonSerialized]
    Quad[] quad;
    [NonSerialized]
    private RenderTexture _renderTexture;
    [NonSerialized]
    private int speed;
    [NonSerialized]
    private Vector2 bgOffset;

    private void OnEnable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Awake();
        }
#endif
        Camera.onPostRender += PostRender;
    }
    private void OnDisable()
    {
        Camera.onPostRender -= PostRender;
    }

    void Awake()
    {
        Renderer = new RenderWindow();

        Texture2D tex1 = new Texture2D(screenWidthRef, screenHeightRef, TextureFormat.RGBA32, false);
        tex1.filterMode = FilterMode.Point;
        FG.sprite = Sprite.Create(tex1, new Rect(0, 0, screenWidthRef, screenHeightRef), new Vector2(0.5f,0.5f), PPU);
        FG.sprite.name = "runtimeFG";

        Texture2D tex2 = new Texture2D(screenWidthRef, screenHeightRef, TextureFormat.RGBA32, false);
        tex2.filterMode = FilterMode.Point;
        Plane.sprite = Sprite.Create(tex2, new Rect(0, 0, screenWidthRef, screenHeightRef), new Vector2(0.5f, 0.5f), PPU);
        Plane.sprite.name = "runtimePlane";
        
        Texture2D tex3 = new Texture2D(BGSprite.texture.width,  BGSprite.texture.height, TextureFormat.RGBA32, false);
        tex2.filterMode = FilterMode.Point;
        BG.sprite = Sprite.Create(tex3, BGSprite.rect, new Vector2(0.5f, 0.5f), PPU);
        BG.sprite.name = "runtimeBG";

        quad = new Quad[] { new Quad(), new Quad(), new Quad(), new Quad(), new Quad(), new Quad(), new Quad() };
        combined = new Mesh[] { new Mesh(), new Mesh(),new Mesh(),new Mesh(),new Mesh(),new Mesh(),new Mesh(),new Mesh(),new Mesh()};
        dictionary = new Dictionary<Material, Quad>()
        {
            { grass1, quad[0]},
            { grass2, quad[1]},
            { rumble1, quad[2]},
            { rumble2, quad[3]},
            { road1, quad[4]},
            { road2, quad[5]},
            { dashline, quad[6]},
        };
        materials = new Material[] { grass1, grass2, rumble1, rumble2, road1, road2, dashline };
    }
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

        destX += destW * Mathf.Sign(line.spriteX) / 2; //offsetX
        destY += destH * (-1);    //offsetY

        float clipH = -line.Y + line.clip;
        if (clipH < 0) clipH = 0;

        if (clipH >= destH) return;

        Rect target = new Rect(destX, destY, destW, destH);
        Rect source = new Rect(Vector2Int.zero, new Vector2(1, 1 - clipH / destH));
        Renderer.draw(source, s, target);
    }
    private void addQuad(Material c, float x1, float y1, float w1, float x2, float y2, float w2, float z)
    {
        dictionary[c].SetQuad(x1 / PPU, y1 / PPU, w1 / PPU, x2 / PPU, y2 / PPU, w2 / PPU, z);
    }

    private void DrawObjects()
    {
        ////////draw objects////////
        if (drawSprites)
        {
            _renderTexture = RenderTexture.GetTemporary(screenWidthRef, screenHeightRef);
            RenderTexture currentActiveRT = RenderTexture.active;
            RenderTexture.active = _renderTexture;
            //Work in the pixel matrix of the texture resolution.
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, screenWidthRef, screenHeightRef, 0);
            GL.Clear(false, true, new Color(0, 0, 0, 0));
            for (int n = startPos + DravingDistance; n > startPos; n--)
            {
                drawSprite(ref track.lines[n % track.Length]);
            }
            Graphics.CopyTexture(_renderTexture, FG.sprite.texture);
            //Revert the matrix and active render texture.
            GL.PopMatrix();
            RenderTexture.active = currentActiveRT;
            RenderTexture.ReleaseTemporary(_renderTexture);
        }
    }
    private void DrawRoad()
    {
        if (drawRoad)
        {
            _renderTexture = RenderTexture.GetTemporary(screenWidthRef, screenHeightRef);
            RenderTexture currentActiveRT = RenderTexture.active;
            Graphics.SetRenderTarget(_renderTexture);
            GL.Clear(false, true, new Color(0.0f, 0.0f, 0, 0));
            GL.PushMatrix();
            float refH = targetCamera.orthographicSize * PPU * 2;
            float refHScale = refH / screenHeightRef;
            float HScale = ((float)screenHeightRef) / targetCamera.pixelHeight;
            float unscaledAspectRation = (HScale * targetCamera.pixelWidth) / screenWidthRef;

            var m = Matrix4x4.Scale(new Vector3(unscaledAspectRation * refHScale, refHScale, 1));

            int i = 0;
            foreach (var material in materials)
            {
                Renderer.draw(dictionary[material].ToMesh(combined[i++]), material, m);
            }
            Graphics.CopyTexture(_renderTexture, Plane.sprite.texture);
            GL.PopMatrix();
            Graphics.SetRenderTarget(currentActiveRT);
            RenderTexture.ReleaseTemporary(_renderTexture);
        }
    }

    private void DrawBackground()
    {
        //Good enough
        _renderTexture = RenderTexture.GetTemporary(BG.sprite.texture.width, BG.sprite.texture.height, 0, BG.sprite.texture.graphicsFormat);
        RenderTexture currentActiveRT = RenderTexture.active;
        RenderTexture.active = _renderTexture;
        //Work in the pixel matrix of the texture resolution.
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, screenWidthRef, screenHeightRef, 0);

        bgOffset += new Vector2(paralaxSpeed/PPU * speed * Time.deltaTime * track.lines[playerPos].curve, 0);

        Graphics.Blit(BGSprite.texture, _renderTexture, Vector2.one, bgOffset, 0, 0);

        Graphics.CopyTexture(_renderTexture, BG.sprite.texture);

        GL.PopMatrix();
        Graphics.SetRenderTarget(currentActiveRT);
        RenderTexture.ReleaseTemporary(_renderTexture);
    }

    private void PostRender(Camera cam)
    {
        DrawRoad();
    }

    private void FixedUpdate()
    {
        GetInput();
        CalculateProjection();
    }
    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            CalculateProjection();
        }
#endif
        DrawBackground();
        DrawObjects();
    }

    void GetInput()
    {
        speed = 0;
        if (Input.GetKey(KeyCode.RightArrow)) playerX += 0.1f;
        if (Input.GetKey(KeyCode.LeftArrow)) playerX -= 0.1f;
        if (Input.GetKey(KeyCode.UpArrow)) speed = 200;
        if (Input.GetKey(KeyCode.DownArrow)) speed = -200;
        if (Input.GetKey(KeyCode.Tab)) speed *= 3;
        if (Input.GetKey(KeyCode.W)) cameraHeight += 100;
        if (Input.GetKey(KeyCode.S)) cameraHeight -= 100;

        trip += speed;
        while (trip >= track.Length * track.segmentLength) trip -= track.Length * track.segmentLength;
        while (trip < 0) trip += track.Length * track.segmentLength;
    }

    void CalculateProjection()
    { 
        startPos = trip / track.segmentLength;
        playerZ = trip + cameraHeight * cameraDepth; // car is in front of cammera
        playerPos = (int)(playerZ / track.segmentLength) % track.lines.Length;
        playerY = track.lines[playerPos].y;
        int camH = (int)(playerY + cameraHeight);
        playerX = playerX - track.lines[playerPos].curve * centrifugal * speed * Time.fixedDeltaTime;
        playerX = Mathf.Clamp(playerX, -2, 2);

        screenWidth2 = screenWidthRef / 2;
        screenHeight2 = screenHeightRef / 2;

        float maxy = -screenHeight2;
        int counter = 0;
        float x = 0, dx = 0;
        float res = 1f / PPU;

        foreach (var q in quad) { q.Clear(); }
        foreach (var m in combined) { m.Clear(); }
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

            var z = (float)(n - startPos) / DravingDistance;

            addQuad(grass, 0, p.Y, screenWidth2, 0, l.Y, screenWidth2, z);
            addQuad(rumble, p.X, p.Y, p.W + p.scale * rumbleWidth * screenWidth2, l.X, l.Y, l.W + l.scale * rumbleWidth * screenWidth2, z);
            addQuad(road, p.X, p.Y, p.W, l.X, l.Y, l.W, z);

            if ((n / 3) % 2 == 0)
            {
                addQuad(dashline, p.X, p.Y * 1.1f, p.W * 0.05f, l.X, l.Y * 1.1f, l.W * 0.05f, z);
            }

            counter++;
        }
    }
}
