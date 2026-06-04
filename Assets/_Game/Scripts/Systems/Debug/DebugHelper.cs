namespace App.Systems.Debug
{
    using App.Progression.Core;
    using UnityEngine;
    using VContainer;
    using NaughtyAttributes;

    public class DebugHelper : MonoBehaviour
    {
        [Inject]
        public ProgressionService _progressionService;

        [Button("Add 50 XP")]
        private void AddXP()
        {
            _progressionService.AddXp(50);
        }
    }
}