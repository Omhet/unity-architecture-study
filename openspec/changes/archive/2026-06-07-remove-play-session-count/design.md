## Context

The save system currently tracks `playSessionCount` — a counter incremented on every load and save operation. This requires two extra storage I/O operations per cycle: a post-load write (to bump the counter after deserializing) and a pre-save read (to fetch the existing count before writing). The counter is stored in the `metadata` section of save files, exposed through `SlotDescriptor`, and displayed in the slot summary string.

## Goals / Non-Goals

**Goals:**

- Eliminate unnecessary storage I/O from load and save pipelines
- Make `LoadSlotAsync()` a pure read operation with no side effects
- Make `SaveSlotAsync()` a pure write operation without pre-read
- Remove dead code paths across 4 files

**Non-Goals:**

- Adding alternative metrics (playtime, progress percentage) — out of scope
- Migration of existing save files to strip the key — orphaned keys are harmless and will naturally disappear

## Decisions

### No migration needed for existing save files

Existing save files on disk will retain `"playSessionCount"` in their metadata JSON. Since we use a raw `Dictionary<string, object>` during deserialization, extra keys are silently ignored. The key will be overwritten when the slot is next saved (new saves won't include it). This avoids adding a permanent migration to the chain for a trivial cleanup.

**Alternatives considered:**

- _Add a migration step_ — Would guarantee clean files but adds complexity to the migration chain that lives forever in the codebase. Not worth ~30 bytes of orphaned JSON per file.

### Keep `lastPlayed` in metadata

Only `playSessionCount` is removed; `lastPlayed` remains as it provides genuine value (showing when a slot was last touched). The metadata section and its parsing logic stay intact.

## Risks / Trade-offs

- [Existing save files retain the key] → No impact; extra keys in JSON are ignored by Newtonsoft.Json during dictionary deserialization
- [Slot UI summary changes slightly] → Minor cosmetic change; session count wasn't a meaningful metric for players anyway
