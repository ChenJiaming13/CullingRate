using System.Linq;
using System.Text;
using Unity.Profiling;
using UnityEngine;

public class FrameStats : MonoBehaviour
{
    private string m_StatsText;
    private ProfilerRecorder m_SetPassCallsRecorder;
    private ProfilerRecorder m_DrawCallsRecorder;
    private ProfilerRecorder m_TrianglesRecorder;
    private ProfilerRecorder m_VerticesRecorder;
    private Renderer[] m_Renderers;
    private int m_TotalTriangleCount;
    public static int ValidObjectCount = -1;
    public static int ValidTriangleCount = -1;

    private void OnEnable()
    {
        m_SetPassCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");
        m_DrawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
        m_TrianglesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");
        m_VerticesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Vertices Count");
        
        m_Renderers = FindObjectsOfType<Renderer>();
        var meshFilters = FindObjectsOfType<MeshFilter>();
        foreach (var meshFilter in meshFilters)
        {
            m_TotalTriangleCount += meshFilter.sharedMesh.triangles.Length / 3;
        }
    }

    private void OnDisable()
    {
        m_SetPassCallsRecorder.Dispose();
        m_DrawCallsRecorder.Dispose();
        m_TrianglesRecorder.Dispose();
        m_VerticesRecorder.Dispose();
    }

    private void Update()
    {
        var sb = new StringBuilder(500);
        if (m_SetPassCallsRecorder.Valid)
            sb.AppendLine($"SetPass Calls: {m_SetPassCallsRecorder.LastValue}");
        if (m_DrawCallsRecorder.Valid)
            sb.AppendLine($"Draw Calls: {m_DrawCallsRecorder.LastValue}");
        if (m_VerticesRecorder.Valid)
            sb.AppendLine($"Vertices: {m_VerticesRecorder.LastValue}");
        if (m_TrianglesRecorder.Valid)
            sb.AppendLine($"Triangles: {m_TrianglesRecorder.LastValue}");

        var currObjectCount = m_Renderers.Count(t => t.isVisible);
        sb.AppendLine(
            $"Visible Objects: {ValidObjectCount} / {currObjectCount} = {ValidObjectCount * 1.0f / currObjectCount:P0} | {m_Renderers.Length}");
        
        var currTriangleCount = m_Renderers.Sum(currRenderer => !currRenderer.isVisible ? 0 :
            currRenderer.gameObject.GetComponent<MeshFilter>().sharedMesh.triangles.Length / 3);
        sb.AppendLine(
            $"Visible Triangles: {ValidTriangleCount} / {currTriangleCount} = {ValidTriangleCount * 1.0f / currTriangleCount:P0} | {m_TotalTriangleCount}");
        
        m_StatsText = sb.ToString();
    }

    private void OnGUI()
    {
        GUI.TextArea(new Rect(10, 30, 300, 100), m_StatsText);
    }
}
