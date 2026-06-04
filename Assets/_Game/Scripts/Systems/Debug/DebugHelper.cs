namespace App.Systems.Debug
{
    using App.Progression.Core;
    using UnityEngine;
    using VContainer;

    public class DebugHelper : MonoBehaviour
    {
        [Inject]
        public ProgressionService _progressionService;

        [ContextMenu("Add 50 XP")]
        private void AddXP()
        {
            _progressionService.AddXp(50);
        }
    }
}