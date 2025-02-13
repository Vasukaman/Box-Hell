using UnityEngine;

namespace MFPC.Camera
{
    public interface ICameraModule
    {
        void SetCameraTransform(Transform transform);
        
        void Update();
    }
}