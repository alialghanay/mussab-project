using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static void Load(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[SceneLoader] sceneName is null or empty.");
            return;
        }

        if (SceneUtility.GetBuildIndexByScenePath($"Assets/Scenes/{sceneName}.unity") < 0)
        {
            Debug.LogError($"[SceneLoader] '{sceneName}' is not in Build Settings. Add it via File > Build Settings.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}
