using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft.Core.Player
{
    public class DebugController : PlayerController
    {
        private const float MOUSE_SENS_X = .002f;

        private const float MOUSE_SENS_Y = .002f;

        private const float MOVE_SPEED = 20f;

        Quaternion playerTargetRotation;
        Quaternion cameraTargetRotation;

        public DebugController()
        {
            playerTargetRotation = localRotation;
            cameraTargetRotation = camera.localRotation;
        }

        public override void Update()
        {
            HandleDebugMovement();
            HandleMouseLook();
        }

        private void HandleDebugMovement()
        {
            Vector2 moveDelta = Input.WasdDelta;

            float u = Input.GetKey(Key.Space) ? 1f : 0f;
            float d = Input.GetKey(Key.LeftShift) ? 1f : 0f;

            Vector3 movement = right * moveDelta.x + forward * moveDelta.y + Vector3.up * (u - d);
            localPosition += movement * Time.deltaTime * MOVE_SPEED_DEBUG;
        }

        private void HandleMouseLook()
        {
            Vector2 mouseDelta = Input.MouseDelta;

            float x = mouseDelta.y * MOUSE_SENS_X;
            float y = mouseDelta.x * MOUSE_SENS_Y;

            playerTargetRotation *= Quaternion.Euler(0.0f, -y, 0.0f);
            cameraTargetRotation *= Quaternion.Euler(-x, 0.0f, 0.0f);

            cameraTargetRotation = ClampRotationAroundAxis(cameraTargetRotation);

            localRotation = playerTargetRotation;
            camera.localRotation = cameraTargetRotation;
        }
    }
}
