## 1. Core — Shop domain models and registry

- [ ] 1.1 Create `ShopItemType` enum (Recipe, Generator) in `Core/Shop/`
- [ ] 1.2 Create `ShopItemDefinition` struct with `Id`, `ItemId`, `Price` fields
- [ ] 1.3 Create `ShopCatalogConfig` serializable class with `Items` containing `RecipeShopItems[]` and `GeneratorShopItems[]`
- [ ] 1.4 Create `ShopProgressionEntry` struct with `Level` (int) and `ShopItemIds` (string[])
- [ ] 1.5 Create `ShopCatalogConfig` to also include `Progression` array of `ShopProgressionEntry`
- [ ] 1.6 Create `ShopRegistry` with `Load(config)`, `TryGetAny(id, out type, out definition)`, and iteration support

## 2. Core — ShopProgressionRegistry

- [ ] 2.1 Create `ShopProgressionRegistry` with `Load(entries)` method
- [ ] 2.2 Implement `GetUnlockedUpToLevel(level)` returning cumulative shop item IDs from all levels ≤ given level
- [ ] 2.3 Implement `GetNewAtLevel(level)` returning only items unlocked at exactly that level

## 3. Core — ShopState

- [ ] 3.1 Create `ShopState` with `ObservableList<string> AvailableShopItemIds` property

## 4. Core — ShopService

- [ ] 4.1 Create `ShopService` constructor accepting ShopRegistry, ShopState, ShopProgressionRegistry, EconomyService, RecipeState, GeneratorState
- [ ] 4.2 Implement `RefreshAvailability(playerLevel)` — query progression registry, set AvailableShopItemIds
- [ ] 4.3 Implement `Buy(shopItemId)` — lookup item type/definition, TrySpend price, grant ownership to RecipeState or GeneratorState

## 5. Boot — ShopConfigModule

- [ ] 5.1 Create `ShopConfigModule` implementing `IConfigModule` with Key = "shop"
- [ ] 5.2 Implement `Deserialize` — JSON deserialize into ShopCatalogConfig
- [ ] 5.3 Implement `Validate` — check unique IDs across both arrays, validate recipe itemIds against RecipeRegistry, generator itemIds against GeneratorRegistry, progression references against shop items
- [ ] 5.4 Implement `Hydrate` — load ShopRegistry and ShopProgressionRegistry from bundle

## 6. Flow — BuyShopItemEvent and ShopFlowHandler

- [ ] 6.1 Add `BuyShopItemEvent` struct to `Events.cs` implementing `ICommand` with `ShopItemId` field
- [ ] 6.2 Create `ShopFlowHandler` with `[Route]` handler for `BuyShopItemEvent` calling `ShopService.Buy()`

## 7. View — ShopSectionView

- [ ] 7.1 Create `ShopSectionView` extending `GameplaySectionViewBase` with definition "shop" / "Shop"
- [ ] 7.2 Implement `BuildContent` — create scrollable list container with CSS class "shop-section"
- [ ] 7.3 Implement `Bind` — subscribe to ShopState.AvailableShopItemIds changes via R3 Observable.Merge pattern
- [ ] 7.4 Implement `RebuildRows` — clear list, iterate available IDs, build shop item cards with name, price, buy button
- [ ] 7.5 Implement `BuildShopItemRow` — lookup definition from ShopRegistry, render label with price, create Buy button publishing `BuyShopItemEvent`
- [ ] 7.6 Implement `Unbind` — dispose R3 subscriptions

## 8. Integration — Wire up in lifetime scope and factory

- [ ] 8.1 Register ShopState, ShopRegistry, ShopProgressionRegistry, ShopService as `Lifetime.Singleton` in `RootLifetimeScope`, alongside other registries/states/services
- [ ] 8.2 Register ShopConfigModule as `IConfigModule` in `RootLifetimeScope`
- [ ] 8.3 Map `ShopFlowHandler` in VitalRouter routing in `GameLifetimeScope`
- [ ] 8.4 Update `HudSectionFactory.Create("shop")` to return `ShopSectionView` instead of `PlaceholderSectionView`
- [ ] 8.5 Add "shop" catalog entry to `_game_manifest.json`

## 9. Config — Create shop_catalog.json

- [ ] 9.1 Create `Assets/_Game/Configs/shop_catalog.json` with sample recipe and generator shop items plus progression entries

## 10. Styles — Shop USS

- [ ] 10.1 Create `Assets/_Game/Styles/shop.uss` with styles for `.shop-section`, `.shop-item-card`, `.shop-item-name`, `.shop-item-price`, `.shop-buy-button`
