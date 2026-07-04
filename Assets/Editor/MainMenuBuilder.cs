using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public static class MainMenuBuilder
{
    [MenuItem("Tools/Neighborhood/Build Main Menu Scene")]
    public static void Build()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // event system
        var eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();

        // camera
        var cameraGO = new GameObject("Main Camera");
        cameraGO.tag = "MainCamera";
        var cam = cameraGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        cam.orthographic = true;
        cam.nearClipPlane = 0.01f;
        cam.farClipPlane = 10f;

        // menu director + localization switcher
        var directorGO = new GameObject("MainMenuDirector");
        directorGO.AddComponent<AudioListener>();
        var menu = directorGO.AddComponent<MainMenuController>();
        var localization = directorGO.AddComponent<LocalizationSwitcher>();
        localization.entries = new LocalizationSwitcher.Entry[]
        {
            new LocalizationSwitcher.Entry { key = "start", english = "Start Demo", arabic = "بدء التجربة" },
            new LocalizationSwitcher.Entry { key = "continue", english = "Continue", arabic = "استمرار" },
            new LocalizationSwitcher.Entry { key = "settings", english = "Settings", arabic = "الإعدادات" },
            new LocalizationSwitcher.Entry { key = "quit", english = "Quit", arabic = "خروج" },
            new LocalizationSwitcher.Entry { key = "language", english = "العربية", arabic = "English" },
            new LocalizationSwitcher.Entry { key = "volume", english = "Master Volume", arabic = "مستوى الصوت" }
        };

        // canvas
        var canvasGO = new GameObject("MainCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        // background panel
        var bg = CreatePanel("Background", canvasGO.transform, Vector2.zero, new Vector2(1920, 1080), new Color(0.05f, 0.05f, 0.05f, 1f));
        bg.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        bg.GetComponent<RectTransform>().anchorMax = Vector2.one;
        bg.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        bg.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

        // menu panel
        var menuPanel = CreatePanel("MenuPanel", canvasGO.transform, Vector2.zero, new Vector2(500, 500), new Color(0.1f, 0.1f, 0.1f, 0.9f));
        var menuRect = menuPanel.GetComponent<RectTransform>();
        menuRect.anchorMin = new Vector2(0.5f, 0.5f);
        menuRect.anchorMax = new Vector2(0.5f, 0.5f);
        menuRect.anchoredPosition = new Vector2(0, 0);

        var vlg = menuPanel.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 16;
        vlg.padding = new RectOffset(32, 32, 32, 32);
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        // title
        var titleGO = new GameObject("TitleText");
        titleGO.transform.SetParent(menuPanel.transform, false);
        var title = titleGO.AddComponent<TextMeshProUGUI>();
        title.text = "4th Day of Eid";
        title.fontSize = 48;
        title.alignment = TextAlignmentOptions.Center;
        title.color = Color.white;

        // buttons
        var startBtn = CreateButton("StartButton", menuPanel.transform, "start", () => menu.OnStartDemo());
        var continueBtn = CreateButton("ContinueButton", menuPanel.transform, "continue", () => menu.OnContinue());
        continueBtn.interactable = false;
        var settingsBtn = CreateButton("SettingsButton", menuPanel.transform, "settings", () => menu.OnSettings());
        var quitBtn = CreateButton("QuitButton", menuPanel.transform, "quit", () => menu.OnQuit());
        var langBtn = CreateButton("LanguageButton", menuPanel.transform, "language", () => menu.OnToggleLanguage());

        // settings panel
        var settingsGO = CreatePanel("SettingsPanel", canvasGO.transform, Vector2.zero, new Vector2(500, 200), new Color(0.12f, 0.12f, 0.12f, 1f));
        var settingsRect = settingsGO.GetComponent<RectTransform>();
        settingsRect.anchorMin = new Vector2(0.5f, 0.5f);
        settingsRect.anchorMax = new Vector2(0.5f, 0.5f);
        settingsRect.anchoredPosition = new Vector2(0, 0);
        settingsGO.SetActive(false);

        var settingsVlg = settingsGO.AddComponent<VerticalLayoutGroup>();
        settingsVlg.spacing = 16;
        settingsVlg.padding = new RectOffset(24, 24, 24, 24);
        settingsVlg.childAlignment = TextAnchor.UpperCenter;
        settingsVlg.childControlWidth = true;
        settingsVlg.childControlHeight = false;

        var volumeLabelGO = new GameObject("VolumeLabel");
        volumeLabelGO.transform.SetParent(settingsGO.transform, false);
        var volumeLabel = volumeLabelGO.AddComponent<TextMeshProUGUI>();
        volumeLabel.text = "Master Volume";
        volumeLabel.fontSize = 24;
        volumeLabel.alignment = TextAlignmentOptions.Center;
        volumeLabel.color = Color.white;
        var volumeLabelLocalized = volumeLabelGO.AddComponent<LocalizedText>();
        volumeLabelLocalized.localizationKey = "volume";

        var sliderGO = new GameObject("VolumeSlider");
        sliderGO.transform.SetParent(settingsGO.transform, false);
        var slider = sliderGO.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;

        var settingsPanel = settingsGO.AddComponent<SettingsPanel>();
        settingsPanel.panelRoot = settingsGO;
        settingsPanel.volumeSlider = slider;
        menu.settingsPanel = settingsPanel;

        // close settings button
        var closeBtn = CreateButton("CloseSettingsButton", settingsGO.transform, "quit", () => settingsPanel.Hide());
        var closeLocalized = closeBtn.GetComponentInChildren<LocalizedText>();
        if (closeLocalized != null)
        {
            closeLocalized.localizationKey = "quit";
            closeLocalized.enabled = false;
            closeLocalized.enabled = true;
        }

        var sliderRect = sliderGO.GetComponent<RectTransform>();
        if (sliderRect == null)
        {
            sliderRect = sliderGO.AddComponent<RectTransform>();
            sliderRect.sizeDelta = new Vector2(400, 30);
        }

        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            AssetDatabase.CreateFolder("Assets", "Scenes");

        string scenePath = "Assets/Scenes/MainMenu.unity";
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scenePath);
        AssetDatabase.SaveAssets();

        Debug.Log("[MainMenuBuilder] MainMenu scene saved to " + scenePath);
    }

    static GameObject CreatePanel(string name, Transform parent, Vector2 position, Vector2 size, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        var image = go.AddComponent<Image>();
        image.color = color;
        return go;
    }

    static Button CreateButton(string name, Transform parent, string localizationKey, UnityEngine.Events.UnityAction onClick)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 60);

        var image = go.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        var button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);

        var labelGO = new GameObject("Text");
        labelGO.transform.SetParent(go.transform, false);
        var labelRect = labelGO.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        var label = labelGO.AddComponent<TextMeshProUGUI>();
        label.fontSize = 24;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;

        var localized = labelGO.AddComponent<LocalizedText>();
        localized.localizationKey = localizationKey;

        return button;
    }
}
