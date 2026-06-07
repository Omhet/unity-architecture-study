## REMOVED Requirements

### Requirement: Save File Session Count Metadata

**Reason**: `playSessionCount` added storage I/O overhead (extra read on save, extra write on load) for minimal value — it was a load counter with a misleading name, not playtime or progress.

**Migration**: Existing save files may retain the orphaned key in JSON metadata; it will be ignored during deserialization and naturally disappear when slots are overwritten by new saves. No code migration needed.
