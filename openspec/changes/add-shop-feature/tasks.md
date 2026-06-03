## 1. Core — Shop domain models and registry

- [x] 1.1 Create `ShopItemType` enum (Recipe, Generator) in `Core/Shop/`
- [x] 1.2 Create `ShopItemDefinition` struct with `Id`, `ItemId`, `Price` fields
- [x] 1.3 Create `ShopCatalogConfig` serializable class with `Items` containing `RecipeShopItems[]` and `GeneratorShopItems[]`
- [x] 1.4 Create `ShopProgressionEntry` struct with `Level` (int) and `ShopItemIds` (string[])
- [x] 1.5 Create `ShopCatalogConfig` to also include `Progression` array of `ShopProgressionEntry`
- [x] 1.6 Create `ShopRegistry` with `Load(config)`, `TryGetAny(id, out type, out definition)`, and iteration support

## 2. Core — ShopProgressionRegistry

- [x] 2.1 Create `ShopProgressionRegistry` with `Load(entries)` method
- [x] 2.2 Implement `GetUnlockedUpToLevel(level)` returning cumulative shop item IDs from all levels ≤ given level
- [x] 2.3 Implement `GetNewAtLevel(level)` returning only items unlocked at exactly that level

## 3. Core — ShopState

- [x] 3.1 Create `ShopState` with `ObservableList<string> AvailableShopItemIds` property

## 4. Core — ShopService

- [x] 4.1 Create `ShopService` constructor accepting ShopRegistry, ShopState, ShopProgressionRegistry, EconomyService, RecipeState, GeneratorState
- [x] 4.2 Implement `RefreshAvailability(playerLevel)` — query progression registry, set AvailableShopItemIds
- [x] 4.3 Implement `Buy(shopItemId)` — lookup item type/definition, TrySpend price, grant ownership to RecipeState or GeneratorState

## 5. Boot — ShopConfigModule

- [x] 5.1 Create `ShopConfigModule` implementing `IConfigModule` with Key = "shop"
- [x] 5.2 Implement `Deserialize` — JSON deserialize into ShopCatalogConfig
- [x] 5.3 Implement `Validate` — check unique IDs across both arrays, validate recipe itemIds against RecipeRegistry, generator itemIds against GeneratorRegistry, progression references against shop items
- [x] 5.4 Implement `Hydrate` — load ShopRegistry and ShopProgressionRegistry from bundle

## 6. Flow — BuyShopItemEvent and ShopFlowHandler

- [x] 6.1 Add `BuyShopItemEvent` struct to `Events.cs` implementing `ICommand` with `ShopItemId` field
- [x] 6.2 Create `ShopFlowHandler` with `[Route]` handler for `BuyShopItemEvent` calling `ShopService.Buy()`

## 7. View — ShopSectionView

- [x] 7.1 Create `ShopSectionView` extending `GameplaySectionViewBase` with definition "shop" / "Shop"
- [x] 7.2 Implement `BuildContent` — create scrollable list container with CSS class "shop-section"
- [x] 7.3 Implement `Bind` — subscribe to ShopState.AvailableShopItemIds changes via R3 Observable.Merge pattern
- [x] 7.4 Implement `RebuildRows` — clear list, iterate available IDs, build shop item cards with name, price, buy button
- [x] 7.5 Implement `BuildShopItemRow` — lookup definition from ShopRegistry, render label with price, create Buy button publishing `BuyShopItemEvent`
- [x] 7.6 Implement `Unbind` — dispose R3 subscriptions

## 8. Integration — Wire up in lifetime scope and factory

- [x] 8.1 Register ShopState, ShopRegistry, ShopProgressionRegistry, ShopService as `Lifetime.Singleton` in `RootLifetimeScope`, alongside other registries/states/services
- [x] 8.2 Register ShopConfigModule as `IConfigModule` in `RootLifetimeScope`
- [x] 8.3 Map `ShopFlowHandler` in VitalRouter routing in `GameLifetimeScope`
- [x] 8.4 Update `HudSectionFactory.Create("shop")` to return `ShopSectionView` instead of `PlaceholderSectionView`
- [x] 8.5 Add "shop" catalog entry to `_game_manifest.json`

## 9. Config — Create shop_catalog.json

- [x] 9.1 Create `Assets/_Game/Configs/shop_catalog.json` with sample recipe and generator shop items plus progression entries

## 10. Styles — Shop USS

- [x] 10.1 Create `Assets/_Game/Styles/shop.uss` with styles for `.shop-section`, `.shop-item-card`, `.shop-item-name`, `.shop-item-price`, `.shop-buy-button`
