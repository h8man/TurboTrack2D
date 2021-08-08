using UnityEngine;

namespace HQ
{
    class PlayerController: MonoBehaviour
    {
        public HqRenderer hQcamera;
        public ProjectedBody body;
        private void FixedUpdate()
        {
            body.speed = 0;
            if (Input.GetKey(KeyCode.RightArrow)) body.playerX += 0.1f;
            if (Input.GetKey(KeyCode.LeftArrow)) body.playerX -= 0.1f;
            if (Input.GetKey(KeyCode.UpArrow)) body.speed = 200;
            if (Input.GetKey(KeyCode.DownArrow)) body.speed = -200;
            if (Input.GetKey(KeyCode.Tab)) body.speed *= 3;
            if (Input.GetKey(KeyCode.W)) hQcamera.cameraHeight += 100;
            if (Input.GetKey(KeyCode.S)) hQcamera.cameraHeight -= 100;
        }
    }
}
