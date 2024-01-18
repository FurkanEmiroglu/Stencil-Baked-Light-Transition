using System;
using UnityEngine;

namespace CityTransition.Light
{
    [Serializable]
    public class RendererInfo
    {
        public Renderer renderer;
        public int lightmapIndex;
        public Vector4 lightmapOffsetScale;
    }
}