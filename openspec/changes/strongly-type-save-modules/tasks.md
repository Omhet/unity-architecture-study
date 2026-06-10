## 1. Create SaveDataBundle Infrastructure

- [x] 1.1 Create SaveDataBundle.cs with generic SetData and GetData methods
- [x] 1.2 Add HasData method to check key existence
- [x] 1.3 Ensure GetData throws exception with clear message when key not found

## 2. Update Save Module Interface

- [x] 2.1 Update ISaveModule.cs to replace object parameters with bundle-based methods
- [x] 2.2 Add Serialize(SaveDataBundle bundle) method signature
- [x] 2.3 Add Deserialize(JToken section, SaveDataBundle bundle) method signature
- [x] 2.4 Update Validate signature to Validate(SaveDataBundle bundle, List<string> errors)
- [x] 2.5 Add Apply(SaveDataBundle bundle) method signature
- [x] 2.6 Remove old Serialize() and Deserialize(object) method signatures

## 3. Update Migration Interface

- [x] 3.1 Update ISaveMigration.cs to change Migrate parameter from Dictionary<string, object> to JObject
- [x] 3.2 Update MigrationChainBuilder to work with JObject if needed

## 4. Refactor SaveLoadSystem Orchestration

- [x] 4.1 Change LoadSlotAsync to parse JSON as JObject instead of Dictionary<string, object>
- [x] 4.2 Update version extraction to use JObject token access
- [x] 4.3 Update migration chain to pass JObject to ISaveMigration.Migrate
- [x] 4.4 Create SaveDataBundle instance for load pipeline
- [x] 4.5 Update module deserialization loop to extract JSON sections and call module.Deserialize
- [x] 4.6 Update validation loop to call module.Validate with bundle
- [x] 4.7 Add transaction check - only proceed to Apply if validation succeeds
- [x] 4.8 Update apply loop to call module.Apply with bundle
- [x] 4.9 Change SaveSlotAsync to create SaveDataBundle for save pipeline
- [x] 4.10 Update module serialization loop to call module.Serialize with bundle
- [x] 4.11 Update save document creation to build JObject from bundle data
- [x] 4.12 Ensure version and metadata are added to JObject before writing

## 5. Refactor ResourceSaveModule

- [x] 5.1 Create ResourceSaveData DTO class in ResourceSaveModule.cs
- [x] 5.2 Add Balances property to ResourceSaveData
- [x] 5.3 Implement Serialize method to create ResourceSaveData and store in bundle
- [x] 5.4 Implement Deserialize method to convert section JToken to ResourceSaveData and store in bundle
- [x] 5.5 Update Validate method to use bundle.GetData<ResourceSaveData>
- [x] 5.6 Implement Apply method to read ResourceSaveData from bundle and update ResourceState

## 6. Refactor EconomySaveModule

- [x] 6.1 Create EconomySaveData DTO class in EconomySaveModule.cs
- [x] 6.2 Add Balance property to EconomySaveData
- [x] 6.3 Implement Serialize method to create EconomySaveData and store in bundle
- [x] 6.4 Implement Deserialize method to convert section JToken to EconomySaveData and store in bundle
- [x] 6.5 Update Validate method to use bundle.GetData<EconomySaveData>
- [x] 6.6 Implement Apply method to read EconomySaveData from bundle and update EconomyState
- [x] 6.7 Remove JObject coupling from validation logic

## 7. Testing and Verification

- [x] 7.1 Test round-trip save/load with ResourceSaveModule produces identical state
- [x] 7.2 Test round-trip save/load with EconomySaveModule produces identical state
- [x] 7.3 Test validation errors prevent Apply from being called
- [x] 7.4 Test missing save section is skipped (module not invoked)
- [x] 7.5 Test corrupted JSON structure throws exception with clear error
- [x] 7.6 Test migration with JObject structure changes works correctly
- [x] 7.7 Verify Bundle.GetData throws exception for missing key
- [x] 7.8 Test transaction semantics - all modules validated before any Applied
- [x] 7.9 Verify module token-to-DTO conversion fails fast with clear errors on shape/type mismatch
