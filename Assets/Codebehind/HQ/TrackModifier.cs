using System;
using UnityEngine;

namespace HQ
{
    [Serializable]
    public class TrackModifier
    {
        public string label;
        public bool disabled;
        public float curve;
        public float h;
        public float spriteX;
        public bool flipX;
        public Sprite sprite;
        public Vector2Int Segments;
        public int frequency;
    }
}
