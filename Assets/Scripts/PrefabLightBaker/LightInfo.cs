using System;

namespace CityTransition.Light
{
    [Serializable]
    public struct LightInfo
    {
        public UnityEngine.Light light;
        public int lightmapBakeType;
        public int mixedLightingMode;
    }
}