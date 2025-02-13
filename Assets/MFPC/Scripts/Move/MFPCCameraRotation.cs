using System.Collections.Generic;
using MFPC.Camera;
using UnityEngine;
using MFPC.Input.PlayerInput;
using MFPC.Utils;

namespace MFPC
{
    /// <summary>
    /// Allows you to rotate the camera vertically
    /// </summary>
    public class MFPCCameraRotation : MonoBehaviour
    {
        [SerializeField] 
        private PlayerInputTuner inputTuner;

        [Utils.CenterHeader("Angle Range"), SerializeField]
        private Vector2 rangeCameraRotation = new Vector2(-90, 90);

        private float _lookDirection;
        private List<ICameraModule> _cameraModules = new List<ICameraModule>();

        private void Update()
        {
            _lookDirection += inputTuner.CurrentPlayerInputHandler.CalculatedVerticalLookDirection;
            _lookDirection = Mathf.Clamp(_lookDirection, rangeCameraRotation.x, rangeCameraRotation.y);

            this.transform.localRotation = RotateHelper.SmoothRotateVertical(this.transform.localRotation,
                inputTuner.SensitiveData.RotateSpeedSmoothVertical, _lookDirection);

            foreach (var module in _cameraModules) module.Update();
        }

        public void AddModule(ICameraModule newModule)
        {
            newModule.SetCameraTransform(this.transform);
            _cameraModules.Add(newModule);
        }

        public void SetRotation(float angle)
        {
            _lookDirection = Mathf.Clamp(angle, rangeCameraRotation.x, rangeCameraRotation.y);
        }
    }
}