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
        if (LocalizationSwitcher.Instance != null)
            LocalizationSwitcher.Instance.OnLanguageChanged += UpdateText;

        UpdateText();
    }

    void OnDisable()
    {
        if (LocalizationSwitcher.Instance != null)
            LocalizationSwitcher.Instance.OnLanguageChanged -= UpdateText;
    }

    void UpdateText()
    {
        if (label == null || LocalizationSwitcher.Instance == null) return;
        label.text = LocalizationSwitcher.Instance.GetText(localizationKey);
    }
}
