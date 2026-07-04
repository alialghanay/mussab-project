using UnityEngine;

public class PhoneController : MonoBehaviour
{
    [Header("Light")]
    public Light phoneLight;
    public bool lightOnAtStart = false;

    [Header("UI")]
    public GameObject phoneScreen;
    public TMPro.TextMeshProUGUI timeText;
    public float messageDisplaySeconds = 4f;

    [Header("Audio")]
    public AudioSource quranSource;

    bool lightOn;
    float messageTimer;

    void Start()
    {
        SetLight(lightOnAtStart);
        if (phoneScreen != null)
            phoneScreen.SetActive(false);
    }

    public void ToggleLight()
    {
        SetLight(!lightOn);
    }

    void SetLight(bool on)
    {
        lightOn = on;
        if (phoneLight != null)
            phoneLight.enabled = on;
    }

    public void ShowMessage(string text)
    {
        // TODO Phase 2: assign text to a UI Text/TMP_Text element.
        Debug.Log("[Phone] " + text);
        if (phoneScreen != null)
            phoneScreen.SetActive(true);
        messageTimer = messageDisplaySeconds;
    }

    public void ToggleQuran()
    {
        if (quranSource == null) return;
        if (quranSource.isPlaying)
            quranSource.Stop();
        else
            quranSource.Play();
    }

    public void Vibrate()
    {
        #if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
        #endif
    }

    void Update()
    {
        if (timeText != null)
            timeText.text = System.DateTime.Now.ToString("HH:mm");

        if (messageTimer > 0f)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0f && phoneScreen != null)
                phoneScreen.SetActive(false);
        }
    }
}
