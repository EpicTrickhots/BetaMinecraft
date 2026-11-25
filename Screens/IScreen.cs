using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft.Screens
{
    public interface IScreen
    {
        public void Start();
        public void Update();
        public void RenderFrame();
    }
}
