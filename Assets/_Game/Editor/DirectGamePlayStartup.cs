namespace App.Editor
{
    using App.Flow.Events;
    using Cysharp.Threading.Tasks;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using VitalRouter;

    [InitializeOnLoad]
    public static class DirectGamePlayStartup
    {
        private const string GameplaySceneName = "Game";
        private const string MenuSceneName = "Menu";

        static DirectGamePlayStartup()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.EnteredPlayMode) return;

            // If the Game scene is open, redirect to Menu first so the normal flow runs.
            // This avoids stale default state — config and save data load before views bind.
            if (SceneManager.GetActiveScene().name == GameplaySceneName)
            {
                UniTask.Void(async () =>
                {
                    await UniTask.Yield();
                    await UniTask.Yield();

                    try
                    {
                        // Load Menu scene so RootLifetimeScope is ready, then trigger normal flow.
                        await SceneManager.LoadSceneAsync(MenuSceneName, LoadSceneMode.Single).ToUniTask();
                        await UniTask.Yield(); // Let the new scene settle

                        await Router.Default.PublishAsync(new PlayGameEvent());
                    }
                    catch (System.Exception exception)
                    {
                        Debug.LogException(exception);
                    }
                });
            }
        }
    }
}
