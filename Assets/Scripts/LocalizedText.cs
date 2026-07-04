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
        if (LocalizationSwitcher.Instance == null)
        {
            LocalizationSwitcher.OnInstanceReady -= OnInstanceReady;
            LocalizationSwitcher.OnInstanceReady += OnInstanceReady;
        }
        else
        {
            LocalizationSwitcher.Instance.OnLanguageChanged -= UpdateText;
            LocalizationSwitcher.Instance.OnLanguageChanged += UpdateText;
        }

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
        LocalizationSwitcher.OnInstanceReady -= OnInstanceReady;

        if (LocalizationSwitcher.Instance != null)
        {
            LocalizationSwitcher.Instance.OnLanguageChanged -= UpdateText;
            LocalizationSwitcher.Instance.OnLanguageChanged += UpdateText;
        }

        UpdateText();
    }

    void UpdateText()
    {
        if (label == null || LocalizationSwitcher.Instance == null) return;

        string text = LocalizationSwitcher.Instance.GetText(localizationKey);
        label.text = text ?? $"[{localizationKey}]";
    }
}
