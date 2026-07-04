using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    public string localizationKey;

    TextMeshProUGUI label;
    LocalizationSwitcher lastInstance;

    void Awake()
    {
        label = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        LocalizationSwitcher.OnInstanceReady += OnInstanceReady;
        SubscribeTo(LocalizationSwitcher.Instance);
        UpdateText();
    }

    void OnDisable()
    {
        LocalizationSwitcher.OnInstanceReady -= OnInstanceReady;
        SubscribeTo(null);
    }

    void OnInstanceReady()
    {
        SubscribeTo(LocalizationSwitcher.Instance);
        UpdateText();
    }

    void SubscribeTo(LocalizationSwitcher instance)
    {
        if (lastInstance != null)
            lastInstance.OnLanguageChanged -= UpdateText;
        lastInstance = instance;
        if (lastInstance != null)
            lastInstance.OnLanguageChanged += UpdateText;
    }

    void UpdateText()
    {
        if (label == null || LocalizationSwitcher.Instance == null) return;
        string text = LocalizationSwitcher.Instance.GetText(localizationKey);
        if (LocalizationSwitcher.Instance.CurrentLanguage == "ar")
            text = ArabicFixer.Fix(text);
        label.text = text;
    }
}
