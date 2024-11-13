using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]
public class ValidDrawStatsSettings
{
    public bool statObjects = true;
    public bool statTriangles = true;
}

public class ValidDrawStatsRendererFeature : ScriptableRendererFeature
{
    public ValidDrawStatsSettings settings = new();
    
    private class ValidDrawStatsPass : ScriptableRenderPass
    {
        private readonly RenderTexture m_ColorBuffer = new(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32,
            RenderTextureReadWrite.Linear);
        private readonly RenderTexture m_DepthBuffer = new(Screen.width, Screen.height, 24, RenderTextureFormat.Depth,
            RenderTextureReadWrite.Linear);

        public ValidDrawStatsSettings settings
        {
            get;
            set;
        }
        
        private void ReadBack()
        {
            if (!settings.statObjects && !settings.statTriangles) return;
            var width = m_ColorBuffer.width;
            var height = m_ColorBuffer.height;
            var t2d = new Texture2D(width, height, TextureFormat.ARGB32, false);
            var oldRT = RenderTexture.active;
            RenderTexture.active = m_ColorBuffer;
            t2d.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
            t2d.Apply();
            RenderTexture.active = oldRT;
            var pixels = t2d.GetPixels(0, 0, width, height);
            var objectIds = new HashSet<float>();
            var triangles = new HashSet<Color>();
            if (settings.statObjects)
            {
                foreach (var pixel in pixels)
                {
                    if (pixel.r == 0.0f) continue;
                    objectIds.Add(pixel.r);
                }
            }
            if (settings.statTriangles)
            {
                foreach (var pixel in pixels)
                {
                    if (pixel.r == 0.0f) continue;
                    triangles.Add(pixel);
                }
            }
            FrameStats.ValidObjectCount = objectIds.Count;
            FrameStats.ValidTriangleCount = triangles.Count;
        }
        
        // private uint DecodeColorToUint(Color color)
        // {
        //     // 将 g, b, a 分量从 [0, 1] 映射回 [0, 255]
        //     var g = (uint)(color.g * 255.0f) & 0xFF;
        //     var b = (uint)(color.b * 255.0f) & 0xFF;
        //     var a = (uint)(color.a * 255.0f) & 0xFF;
        //     // 重组为 uint，按位移位
        //     return (g) | (b << 8) | (a << 16);
        // }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get();
            context.SetupCameraProperties(renderingData.cameraData.camera);
            cmd.SetRenderTarget(m_ColorBuffer, m_DepthBuffer);
            cmd.ClearRenderTarget(true, true, Color.clear);
            context.ExecuteCommandBuffer(cmd);

            var drawingSettings = new DrawingSettings(new ShaderTagId("StatObjectRenderPass"),
                new SortingSettings(renderingData.cameraData.camera));
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
            
            CommandBufferPool.Release(cmd);
            ReadBack();
        }
    }

    private ValidDrawStatsPass m_ValidDrawStatsPass;

    public override void Create()
    {
        m_ValidDrawStatsPass = new ValidDrawStatsPass
        {
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques
        };
    }
    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_ValidDrawStatsPass.settings = settings;
        renderer.EnqueuePass(m_ValidDrawStatsPass);
    }
}


