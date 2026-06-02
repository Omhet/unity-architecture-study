namespace App.Systems.Configuration
{
    using System;
    using System.Collections.Generic;

    public class GameConfigValidator
    {
        private readonly IEnumerable<IConfigModule> _modules;

        public GameConfigValidator(IEnumerable<IConfigModule> modules)
        {
            _modules = modules;
        }

        public void ValidateOrThrow(GameCatalogBundle bundle)
        {
            var errors = new List<string>();

            if (bundle == null)
            {
                errors.Add("Config bundle is null.");
                ThrowIfInvalid(errors);
                return;
            }

            foreach (var module in _modules)
            {
                module.Validate(bundle, errors);
            }

            ThrowIfInvalid(errors);
        }

        private static void ThrowIfInvalid(List<string> errors)
        {
            if (errors.Count == 0)
            {
                return;
            }

            throw new InvalidOperationException("Game config validation failed:\n- " + string.Join("\n- ", errors));
        }
    }
}
