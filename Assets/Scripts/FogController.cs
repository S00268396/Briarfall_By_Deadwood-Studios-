using UnityEngine;

public class FogController : MonoBehaviour
{
    [Header("Fog Settings")]
    public Color fogColor = new Color(0.55f, 0.6f, 0.65f);
    [Range(0.001f, 0.1f)]
    public float fogDensity = 0.02f;

    void Start()
    {
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
    }
}
