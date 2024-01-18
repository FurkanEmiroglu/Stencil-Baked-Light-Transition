using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace CityTransition.Light
{
    [ExecuteAlways]
    public class Prerendered : MonoBehaviour
    {
        [SerializeField] private RendererInfo[] rendererInfos;

        [SerializeField] private Texture2D[] lightmaps;

        [SerializeField] private Texture2D[] lightmapDirections;

        [SerializeField] private Texture2D[] shadowMasks;

        [SerializeField] private LightInfo[] lightInfos;

        private void Awake()
        {
            Initialize();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Initialize();
        }

        private void Initialize()
        {
            if (rendererInfos == null || rendererInfos.Length == 0)
                return;

            LightmapData[] lightmapData = LightmapSettings.lightmaps;

            int[] offsetIndices = new int[lightmaps.Length];
            int totalLightmapCount = lightmapData.Length;

            List<LightmapData> combinedLightmaps = new List<LightmapData>();

            for (int i = 0; i < lightmaps.Length; i++)
            {
                bool exists = false;
                
                for (int j = 0; j < lightmapData.Length; j++)
                {
                    if (lightmaps[i] == lightmapData[j].lightmapColor)
                    {
                        exists = true;
                        offsetIndices[i] = j;
                    }
                }

                if (exists) continue;

                offsetIndices[i] = totalLightmapCount;

                LightmapData newLightmapData = new LightmapData
                {
                    lightmapColor = lightmaps[i],
                    lightmapDir = lightmapDirections.Length == lightmaps.Length
                        ? lightmapDirections[i]
                        : default(Texture2D),
                    shadowMask = shadowMasks.Length == this.lightmaps.Length ? shadowMasks[i] : default(Texture2D),
                };

                combinedLightmaps.Add(newLightmapData);

                totalLightmapCount += 1;
            }

            LightmapData[] combinedLightmaps2 = new LightmapData[totalLightmapCount];

            lightmapData.CopyTo(combinedLightmaps2, 0);
            combinedLightmaps.ToArray().CopyTo(combinedLightmaps2, lightmapData.Length);

            bool directional = lightmapDirections.All(t => t != null);

            LightmapSettings.lightmapsMode = (lightmapDirections.Length == this.lightmaps.Length && directional)
                ? LightmapsMode.CombinedDirectional
                : LightmapsMode.NonDirectional;
            
            ApplyRendererInfo(rendererInfos, offsetIndices, lightInfos);
            LightmapSettings.lightmaps = combinedLightmaps2;
        }

        private static void ApplyRendererInfo(RendererInfo[] infos, int[] lightmapOffsetIndex, LightInfo[] lightsInfo)
        {
            for (int i = 0; i < infos.Length; i++)
            {
                var info = infos[i];

                if (!info.renderer.isPartOfStaticBatch)
                {
                    info.renderer.lightmapIndex = lightmapOffsetIndex[info.lightmapIndex];
                    info.renderer.lightmapScaleOffset = info.lightmapOffsetScale;   
                }
                
                Material[] mat = info.renderer.sharedMaterials;
                for (int j = 0; j < mat.Length; j++)
                {
                    if (mat[j] != null && Shader.Find(mat[j].shader.name) != null)
                        mat[j].shader = Shader.Find(mat[j].shader.name);
                }

            }

            for (int i = 0; i < lightsInfo.Length; i++)
            {
                LightBakingOutput bakingOutput = new LightBakingOutput();
                bakingOutput.isBaked = true;
                bakingOutput.lightmapBakeType = (LightmapBakeType)lightsInfo[i].lightmapBakeType;
                bakingOutput.mixedLightingMode = (MixedLightingMode)lightsInfo[i].mixedLightingMode;
                lightsInfo[i].light.bakingOutput = bakingOutput;
            }
        }
    }
}
