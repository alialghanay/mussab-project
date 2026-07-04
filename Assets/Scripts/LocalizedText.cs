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

    void Start()
    {
        Subscribe();
        UpdateText();
    }

    void OnEnable()
    {
        Subscribe();
        UpdateText();
    }

    void OnDisable()
    {
        Unsubscribe();
    }

    void Subscribe()
    {
        if (LocalizationSwitcher.Instance == null) return;
        LocalizationSwitcher.Instance.OnLanguageChanged -= UpdateText;
        LocalizationSwitcher.Instance.OnLanguageChanged += UpdateText;
    }

    void Unsubscribe()
    {
        if (LocalizationSwitcher.Instance == null) return;
        LocalizationSwitcher.Instance.OnLanguageChanged -= UpdateText;
    }

    void UpdateText()
    {
        if (label == null || LocalizationSwitcher.Instance == null) return;

        string text = LocalizationSwitcher.Instance.GetText(localizationKey);
        label.text = text ?? $"[{localizationKey}]";
    }
}
