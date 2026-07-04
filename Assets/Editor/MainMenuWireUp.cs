using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public static class MainMenuWireUp
{
    [MenuItem("Tools/Neighborhood/Wire Up Main Menu")]
    public static void Wire()
    {
        string scenePath = "Assets/Scenes/MainMenu.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        var director = GameObject.Find("MainMenuDirector");
        var menu = director != null ? director.GetComponent<MainMenuController>() : null;
        var settingsPanel = GameObject.Find("SettingsPanel")?.GetComponent<SettingsPanel>();

        if (menu == null)
        {
            Debug.LogError("[MainMenuWireUp] MainMenuController not found.");
            return;
        }

        // assign settings panel to menu if missing
        if (menu.settingsPanel == null && settingsPanel != null)
        {
            menu.settingsPanel = settingsPanel;
            EditorUtility.SetDirty(menu);
        }

        // assign font asset
        TMP_FontAsset fontAsset = Resources.GetBuiltinResource<TMP_FontAsset>("LegacyRuntime.ttf");
        if (fontAsset == null)
        {
            var guids = AssetDatabase.FindAssets("t:TMP_FontAsset");
            if (guids.Length > 0)
                fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        if (fontAsset == null)
        {
            Debug.LogWarning("[MainMenuWireUp] No TMP_FontAsset found in project. Text will not render until a font is assigned.");
        }

        foreach (var tmp in Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None))
        {
            if (tmp.font == null && fontAsset != null)
            {
                tmp.font = fontAsset;
                EditorUtility.SetDirty(tmp);
            }
        }

        // wire buttons
        WireButton("StartButton", menu.OnStartDemo);
        WireButton("ContinueButton", menu.OnContinue);
        WireButton("SettingsButton", menu.OnSettings);
        WireButton("QuitButton", menu.OnQuit);
        WireButton("LanguageButton", menu.OnToggleLanguage);

        // wire close settings button
        var closeButton = GameObject.Find("CloseSettingsButton")?.GetComponent<Button>();
        if (closeButton != null && settingsPanel != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(settingsPanel.Hide);
            EditorUtility.SetDirty(closeButton);
        }

        // fix slider rects
        var sliderGO = GameObject.Find("VolumeSlider");
        if (sliderGO != null)
        {
            var slider = sliderGO.GetComponent<Slider>();
            if (slider != null)
            {
                var sliderRect = sliderGO.GetComponent<RectTransform>();
                if (sliderRect == null) sliderRect = sliderGO.AddComponent<RectTransform>();
                if (sliderRect.sizeDelta == Vector2.zero)
                    sliderRect.sizeDelta = new Vector2(400, 30);

                if (slider.fillRect == null)
                {
                    var fillArea = new GameObject("Fill Area", typeof(RectTransform));
                    fillArea.transform.SetParent(sliderGO.transform, false);
                    var fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
                    fill.transform.SetParent(fillArea.transform, false);
                    var fillRectTransform = fill.GetComponent<RectTransform>();
                    fillRectTransform.anchorMin = Vector2.zero;
                    fillRectTransform.anchorMax = Vector2.one;
                    fillRectTransform.sizeDelta = Vector2.zero;
                    fill.GetComponent<Image>().color = Color.white;
                    slider.fillRect = fillRectTransform;
                }

                if (slider.handleRect == null)
                {
                    var handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
                    handleArea.transform.SetParent(sliderGO.transform, false);
                    var handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
                    handle.transform.SetParent(handleArea.transform, false);
                    var handleRectTransform = handle.GetComponent<RectTransform>();
                    handleRectTransform.sizeDelta = new Vector2(30, 30);
                    handle.GetComponent<Image>().color = Color.white;
                    slider.handleRect = handleRectTransform;
                }

                EditorUtility.SetDirty(slider);
            }
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[MainMenuWireUp] MainMenu scene wired up.");
    }

    static void WireButton(string name, UnityAction action)
    {
        var buttonGO = GameObject.Find(name);
        if (buttonGO == null) return;
        var button = buttonGO.GetComponent<Button>();
        if (button == null) return;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
        EditorUtility.SetDirty(button);
    }
}
