using UnityEngine;

namespace HQ
{
    [ExecuteInEditMode]
    class HqCameraFollow: MonoBehaviour
    {
        public ProjectedBody body;
        public HqRenderer hQCamera;

        private void Update()
        {
#if UNITY_EDITOR
            Follow();
#else
            Follow();
#endif
        }

        private void Follow()
        {
            hQCamera.cameraOffset = body.playerX;
            hQCamera.trip = body.trip;
        }
    }
}
