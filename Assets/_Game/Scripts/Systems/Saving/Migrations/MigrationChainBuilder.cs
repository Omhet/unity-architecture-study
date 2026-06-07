namespace App.Systems.Saving.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Collects all registered ISaveMigration instances, orders them by version,
    /// validates there are no gaps in the chain, and returns an ordered sequence.
    /// </summary>
    public class MigrationChainBuilder
    {
        private readonly IEnumerable<ISaveMigration> _migrations;

        public MigrationChainBuilder(IEnumerable<ISaveMigration> migrations)
        {
            _migrations = migrations;
        }

        /// <summary>
        /// Build an ordered chain of migrations to transform save data from fileVersion to current version.
        /// Returns empty sequence if fileVersion is already current or newer.
        /// </summary>
        public ISaveMigration[] BuildChain(int fileVersion)
        {
            if (fileVersion >= SaveSchemaVersion.Current)
                return Array.Empty<ISaveMigration>();

            var ordered = _migrations.OrderBy(m => m.FromVersion).ToList();

            var chain = new List<ISaveMigration>();
            int currentVersion = fileVersion;

            while (currentVersion < SaveSchemaVersion.Current)
            {
                var migration = ordered.FirstOrDefault(m => m.FromVersion == currentVersion && m.ToVersion == currentVersion + 1);

                if (migration == null)
                {
                    var list = string.Join(", ", ordered.Select(m => $"{m.FromVersion}->{m.ToVersion}"));
                    throw new InvalidOperationException(
                        $"Save migration gap: no migration exists from version {currentVersion} to {currentVersion + 1}. " +
                        $"Available migrations: {list}");
                }

                chain.Add(migration);
                currentVersion = migration.ToVersion;
            }

            return chain.ToArray();
        }
    }
}
