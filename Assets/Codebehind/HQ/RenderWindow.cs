using System;
using UnityEngine;

public class RenderWindow
{
    public  Material sharedMaterial;


    internal void draw(Mesh mesh, Material mat)
    {

        Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, mat, 8);
        //mat.SetPass(0);
        //Graphics.DrawMeshNow(mesh, Vector3.zero, Quaternion.identity, 0);
    }

    internal void clear(Color color)
    {
        throw new NotImplementedException();
    }

    internal void draw(Rect Source, Sprite s, Rect taret)
    {
        //Graphics.DrawTexture(
        //    new Rect(
        //        taret.x + taret.x * Source.x,
        //        taret.y + taret.y * Source.y,
        //        taret.width * Source.width,
        //        taret.height * Source.height
        //    ),
        //    s.texture,
        //    new Rect(
        //        s.rect.x / s.texture.width + Source.x,
        //        s.rect.y / s.texture.height + Source.y,
        //        s.rect.width / s.texture.width  * Source.width,
        //        s.rect.height / s.texture.height * Source.height)
        //    , 0, 0, 0, 0);
        Graphics.DrawTexture(
            new Rect(
                taret.x,
                taret.y,
                taret.width,
                taret.height * Source.height
            ),
            s.texture,
            new Rect(
                s.rect.x / s.texture.width,
                s.rect.y / s.texture.height + 1 - s.rect.height / s.texture.height * Source.height, //WHY?!
                s.rect.width / s.texture.width,
                s.rect.height / s.texture.height * Source.height)
            , 0, 0, 0, 0);

    }
}