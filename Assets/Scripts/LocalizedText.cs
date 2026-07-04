using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    public string localizationKey;

    TextMeshProUGUI label;

    void Awake()
    {
        label = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        LocalizationSwitcher.OnInstanceReady += OnInstanceReady;
        if (LocalizationSwitcher.Instance != null)
            LocalizationSwitcher.Instance.OnLanguageChanged += UpdateText;
        UpdateText();
    }

    void OnDisable()
    {
        LocalizationSwitcher.OnInstanceReady -= OnInstanceReady;
        if (LocalizationSwitcher.Instance != null)
            LocalizationSwitcher.Instance.OnLanguageChanged -= UpdateText;
    }

    void OnInstanceReady()
    {
        if (LocalizationSwitcher.Instance != null)
            LocalizationSwitcher.Instance.OnLanguageChanged += UpdateText;
        UpdateText();
    }

    void UpdateText()
    {
        if (label == null || LocalizationSwitcher.Instance == null) return;
        label.text = LocalizationSwitcher.Instance.GetText(localizationKey);
    }
}
