namespace App.Resources.Core
{
    using System.Collections.Generic;
    using App.GameConfig.Core;

    public class ResourceRegistry
    {
        private readonly List<ResourceDefinition> _resources = new List<ResourceDefinition>();

        public void Load(ResourceCatalogConfig config)
        {
            _resources.Clear();

            if (config?.Resources == null)
            {
                return;
            }

            for (int i = 0; i < config.Resources.Length; i++)
            {
                var resource = config.Resources[i];
                if (resource == null || string.IsNullOrWhiteSpace(resource.Id))
                {
                    continue;
                }

                _resources.Add(resource);
            }
        }

        public bool TryGetById(string resourceId, out ResourceDefinition resource)
        {
            resource = null;
            if (string.IsNullOrWhiteSpace(resourceId))
            {
                return false;
            }

            for (int i = 0; i < _resources.Count; i++)
            {
                var current = _resources[i];
                if (current != null && current.Id == resourceId)
                {
                    resource = current;
                    return true;
                }
            }

            return false;
        }

        public int Count => _resources.Count;

        public ResourceDefinition this[int index] => _resources[index];
    }
}