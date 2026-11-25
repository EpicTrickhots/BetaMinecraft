using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft.Core.Player
{
    public interface IController
    {
        public Matrix4 GetViewMatrix();
    }
}
