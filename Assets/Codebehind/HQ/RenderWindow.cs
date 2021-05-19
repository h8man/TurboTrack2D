using System;
using UnityEngine;

public class RenderWindow
{

    internal void draw(Mesh mesh, Material mat, Matrix4x4 fixAspect)
    {

        //Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, mat, layer);
        if (mat.SetPass(0))
        {
            Graphics.DrawMeshNow(mesh, fixAspect);
        }
    }

    internal void clear(Color color)
    {
        throw new NotImplementedException();
    }

    internal void draw(Rect offsetSource, Sprite sprite, Rect taret, bool flip)
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
        var sign = flip ? -1: 1;

        Graphics.DrawTexture(
            new Rect(
                taret.x,
                taret.y,
                taret.width * sign,
                taret.height * offsetSource.height
            ),
            sprite.texture,
            new Rect(
                sprite.rect.x / sprite.texture.width,
                sprite.rect.y / sprite.texture.height + (1 - offsetSource.height) * sprite.rect.height / sprite.texture.height,
                sprite.rect.width / sprite.texture.width,
                sprite.rect.height / sprite.texture.height * offsetSource.height)
            , 0, 0, 0, 0);

    }
}