using System;
using UnityEngine;
namespace HQ
{
    class ProjectedBody : MonoBehaviour
    {
        internal float playerX;
        internal int speed;
        public TrackObject track;
        [NonSerialized]
        private int playerPos;
        public float centrifugal = 0.1f;
        public int trip;

        public void FixedUpdate()
        {
            trip += speed;

            while (trip >= track.Length * track.segmentLength) trip -= track.Length * track.segmentLength;
            while (trip < 0) trip += track.Length * track.segmentLength;
            playerPos = trip / track.segmentLength;
            playerX = playerX - track.lines[playerPos].curve * centrifugal * speed * Time.fixedDeltaTime;
            playerX = Mathf.Clamp(playerX, -2, 2);
        }
    }
}
