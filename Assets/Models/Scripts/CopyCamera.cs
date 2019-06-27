using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Camera))]
    public class CopyCamera : MonoBehaviour
    {
        private Camera _ThisCam;
        public Camera TargetCam;

        void Start()
        {
            _ThisCam = GetComponent<Camera>();
            _ThisCam.nearClipPlane = TargetCam.nearClipPlane;
            _ThisCam.farClipPlane = TargetCam.farClipPlane;
        }

        void LateUpdate()
        {
            _ThisCam.projectionMatrix = TargetCam.projectionMatrix;
        }
    }
}