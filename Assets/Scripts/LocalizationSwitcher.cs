using System;
using UnityEngine;

public class LocalizationSwitcher : MonoBehaviour
{
    public static LocalizationSwitcher Instance { get; private set; }

    public string CurrentLanguage { get; private set; } = "en";
    public event Action OnLanguageChanged;

    [Serializable]
    public class Entry
    {
        public string key;
        public string english;
        public string arabic;
    }

    public Entry[] entries = new Entry[0];

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetLanguage(string language)
    {
        if (language != "en" && language != "ar") return;
        CurrentLanguage = language;
        OnLanguageChanged?.Invoke();
    }

    public string GetText(string key)
    {
        foreach (var entry in entries)
        {
            if (entry.key == key)
                return CurrentLanguage == "ar" ? entry.arabic : entry.english;
        }

        return $"[{key}]";
    }

    public void ToggleLanguage()
    {
        SetLanguage(CurrentLanguage == "en" ? "ar" : "en");
    }
}
