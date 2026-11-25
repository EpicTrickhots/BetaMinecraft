using Minecraft.InputHandling;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;

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

            float u = Input.GetKey(Keys.Space) ? 1f : 0f;
            float d = Input.GetKey(Keys.LeftShift) ? 1f : 0f;

            Vector3 movement = Right * moveDelta.X + Forward * moveDelta.y + Vector3.UnitY * (u - d);
            localPosition += movement * Time.deltaTime * MOVE_SPEED;
        }

        private void HandleMouseLook()
        {
            Vector2 mouseDelta = Input.MouseDelta;

            float x = mouseDelta.Y * MOUSE_SENS_X;
            float y = mouseDelta.X * MOUSE_SENS_Y;

            playerTargetRotation *= Quaternion.FromEulerAngles(0.0f, -y, 0.0f);
            cameraTargetRotation *= Quaternion.FromEulerAngles(-x, 0.0f, 0.0f);

            cameraTargetRotation = ClampRotationAroundAxis(cameraTargetRotation);

            localRotation = playerTargetRotation;
            camera.localRotation = cameraTargetRotation;
        }

        private static Quaternion ClampRotationAroundAxis(Quaternion q)
        {
            q.X /= q.W;
            q.Y /= q.W;
            q.Z /= q.W;
            q.W = 1.0f;

            float angleX = 2.0f * MathHelper.RadToDeg * (float)MathHelper.Atan(q.X);
            angleX = MathHelper.Clamp(angleX, -90.0f, 90.0f);
            q.X = (float)MathHelper.Tan(0.5f * MathHelper.DegToRad * angleX);
            return q;
        }
    }
}
