using System.Collections.Generic;
using UnityEngine;

/// Drives the day/night cycle: skybox blending, sun/moon light, ambient
/// intensity and street lamps. 10 real minutes = 24 game hours by default.
/// Scrub timeOfDay in the Inspector to preview any hour in the editor.
[ExecuteAlways]
public class DayNightCycle : MonoBehaviour
{
    [Header("Time")]
    [Range(0f, 24f)] public float timeOfDay = 7f;
    [Tooltip("Real minutes for a full 24h game day.")]
    public float dayLengthMinutes = 10f;

    [Header("Sky")]
    public Material skyboxMaterial;
    public Texture sunriseSky;
    public Texture daySky;
    public Texture sunsetSky;
    public Texture nightSky;

    [Header("Sun / Moon")]
    public Light sunMoonLight;
    public float dayIntensity = 1.1f;
    public float moonIntensity = 0.35f;

    [Header("Street Lights")]
    public Transform streetLightsRoot;
    public float lampIntensity = 1.2f;

    [Header("Fog")]
    public bool controlFog = true;
    public Color dayFogColor = new Color(0.72f, 0.76f, 0.80f);
    public Color nightFogColor = new Color(0.015f, 0.025f, 0.045f);
    public float dayFogDensity = 0.0035f;
    public float nightFogDensity = 0.014f;

    /// Current hour (0-24) for other systems (e.g. GameDirector).
    public float Hour { get { return timeOfDay; } }
    /// True from dusk (18:30) until dawn (06:00).
    public bool IsNight { get { return timeOfDay >= 18.5f || timeOfDay < 6f; } }

    static readonly Color MoonColor = new Color(0.659f, 0.733f, 0.831f);
    static readonly Color HorizonColor = new Color(1f, 0.62f, 0.38f);
    static readonly Color DayColor = new Color(1f, 0.96f, 0.88f);

    static readonly int TexAId = Shader.PropertyToID("_TexA");
    static readonly int TexBId = Shader.PropertyToID("_TexB");
    static readonly int BlendId = Shader.PropertyToID("_Blend");

    readonly List<Light> lamps = new List<Light>();
    float giTimer;

    void OnEnable()
    {
        CacheLamps();
        if (skyboxMaterial != null && RenderSettings.skybox != skyboxMaterial)
            RenderSettings.skybox = skyboxMaterial;
        ApplyTimeOfDay();
    }

    void OnValidate()
    {
        ApplyTimeOfDay();
    }

    void Update()
    {
        if (!Application.isPlaying)
            return;

        timeOfDay += (24f / (dayLengthMinutes * 60f)) * Time.deltaTime;
        if (timeOfDay >= 24f) timeOfDay -= 24f;
        ApplyTimeOfDay();

        giTimer += Time.deltaTime;
        if (giTimer >= 5f)
        {
            giTimer = 0f;
            DynamicGI.UpdateEnvironment();
        }
    }

    public void ApplyTimeOfDay()
    {
        ApplySky();
        ApplySunMoon();
        ApplyLamps();
        ApplyFog();
    }

    void ApplyFog()
    {
        if (!controlFog)
            return;
        float sunAngle = (timeOfDay / 24f) * 360f - 90f;
        float e = Mathf.Clamp01(Mathf.Sin(sunAngle * Mathf.Deg2Rad));
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = Color.Lerp(nightFogColor, dayFogColor, e);
        RenderSettings.fogDensity = Mathf.Lerp(nightFogDensity, dayFogDensity, e);
    }

    void ApplySky()
    {
        if (skyboxMaterial == null)
            return;

        Texture a, b;
        float t;
        GetSkySegment(timeOfDay, out a, out b, out t);
        t = t * t * (3f - 2f * t); // smoothstep
        skyboxMaterial.SetTexture(TexAId, a);
        skyboxMaterial.SetTexture(TexBId, b);
        skyboxMaterial.SetFloat(BlendId, t);
    }

    void GetSkySegment(float h, out Texture a, out Texture b, out float t)
    {
        if (h < 4f)       { a = nightSky;   b = nightSky;   t = 0f; }
        else if (h < 6f)  { a = nightSky;   b = sunriseSky; t = (h - 4f) / 2f; }
        else if (h < 8f)  { a = sunriseSky; b = daySky;     t = (h - 6f) / 2f; }
        else if (h < 16f) { a = daySky;     b = daySky;     t = 0f; }
        else if (h < 18f) { a = daySky;     b = sunsetSky;  t = (h - 16f) / 2f; }
        else if (h < 20f) { a = sunsetSky;  b = nightSky;   t = (h - 18f) / 2f; }
        else              { a = nightSky;   b = nightSky;   t = 0f; }
    }

    void ApplySunMoon()
    {
        if (sunMoonLight == null)
            return;

        // Sun crosses the horizon at 06:00 and 18:00, peaks at noon.
        float sunAngle = (timeOfDay / 24f) * 360f - 90f;
        float elevation = Mathf.Sin(sunAngle * Mathf.Deg2Rad);

        if (elevation > 0f)
        {
            sunMoonLight.transform.rotation = Quaternion.Euler(sunAngle, 215f, 0f);
            sunMoonLight.color = Color.Lerp(HorizonColor, DayColor, Mathf.Clamp01(elevation * 2f));
            sunMoonLight.intensity = Mathf.Lerp(0.05f, dayIntensity, Mathf.Pow(elevation, 0.6f));
        }
        else
        {
            sunMoonLight.transform.rotation = Quaternion.Euler(sunAngle + 180f, 215f, 0f);
            sunMoonLight.color = MoonColor;
            sunMoonLight.intensity = Mathf.Lerp(0.05f, moonIntensity, Mathf.Pow(-elevation, 0.6f));
        }

        RenderSettings.ambientIntensity = Mathf.Lerp(0.35f, 1f, Mathf.Clamp01(elevation));
    }

    void ApplyLamps()
    {
        if (lamps.Count == 0)
            CacheLamps();

        // Fade on 18:00-18:30, off 06:00-06:30.
        float factor;
        if (timeOfDay >= 18f)      factor = Mathf.Clamp01((timeOfDay - 18f) / 0.5f);
        else if (timeOfDay < 6f)   factor = 1f;
        else if (timeOfDay < 6.5f) factor = 1f - (timeOfDay - 6f) / 0.5f;
        else                       factor = 0f;

        for (int i = 0; i < lamps.Count; i++)
        {
            var lamp = lamps[i];
            if (lamp == null) continue;
            lamp.enabled = factor > 0f;
            lamp.intensity = lampIntensity * factor;
        }
    }

    void CacheLamps()
    {
        lamps.Clear();
        if (streetLightsRoot == null)
            return;
        streetLightsRoot.GetComponentsInChildren(true, lamps);
    }
}
