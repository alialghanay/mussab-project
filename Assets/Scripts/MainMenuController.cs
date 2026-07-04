using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene")]
    public string demoSceneName = "Neighborhood";

    [Header("UI Panels")]
    public SettingsPanel settingsPanel;

    [Header("Button Labels")]
    public LocalizedText startButtonText;
    public LocalizedText continueButtonText;
    public LocalizedText settingsButtonText;
    public LocalizedText quitButtonText;
    public LocalizedText languageButtonText;

    private void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.escapeKey.wasPressedThisFrame && settingsPanel != null)
            settingsPanel.Hide();
    }

    public void OnStartDemo()
    {
        SceneLoader.Load(demoSceneName);
    }

    public void OnContinue()
    {
        Debug.Log("[MainMenu] Continue is not implemented in the demo.");
    }

    public void OnSettings()
    {
        if (settingsPanel != null)
            settingsPanel.Show();
    }

    public void OnQuit()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif
        Debug.Log("[MainMenu] Quit requested.");
    }

    public void OnToggleLanguage()
    {
        if (LocalizationSwitcher.Instance != null)
            LocalizationSwitcher.Instance.ToggleLanguage();
    }
}
