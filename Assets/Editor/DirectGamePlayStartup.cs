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

        static DirectGamePlayStartup()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.EnteredPlayMode)
            {
                return;
            }

            if (SceneManager.GetActiveScene().name != GameplaySceneName)
            {
                return;
            }

            UniTask.Void(async () =>
            {
                // Give root scope/router a couple frames to initialize before publishing.
                await UniTask.Yield();
                await UniTask.Yield();

                try
                {
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
