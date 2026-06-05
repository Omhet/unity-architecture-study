## ADDED Requirements

### Requirement: ShopSectionView extends GameplaySectionViewBase pattern

The ShopSectionView SHALL extend `GameplaySectionViewBase` with a `GameplaySectionDefinition` for the "shop" section, implementing `BuildContent`, `Bind`, and `Unbind` following the existing section view pattern.

#### Scenario: View builds content

- **WHEN** `BuildContent()` is called on ShopSectionView
- **THEN** a scrollable list container is created with class name "shop-section"

### Requirement: ShopSectionView subscribes to available shop items

The ShopSectionView SHALL subscribe to changes in `ShopState.AvailableShopItemIds` using R3 Observable patterns (ObserveAdd, ObserveRemove, ObserveReplace, ObserveReset) and rebuild the item list on any change.

#### Scenario: Initial render

- **WHEN** the view is mounted and ShopState has 2 available items
- **THEN** two shop item cards are rendered in the list

#### Scenario: Reactive update on availability change

- **WHEN** a new shop item ID is added to ShopState.AvailableShopItemIds
- **THEN** the view rebuilds rows to include the new item card

### Requirement: ShopSectionView renders item cards with price and buy button

Each available shop item SHALL be rendered as a card showing the item name (from registry lookup), its price, and a "Buy" button. Clicking the button SHALL publish a `BuyShopItemEvent` via `ICommandPublisher`.

#### Scenario: Buy button publishes event

- **WHEN** the user clicks the "Buy" button on a shop item card for "shop_recipe_bread"
- **THEN** `ICommandPublisher.PublishAsync(new BuyShopItemEvent("shop_recipe_bread"))` is called

### Requirement: ShopSectionView disposes subscriptions on unmount

The ShopSectionView SHALL dispose all R3 subscriptions in the `Unbind()` method to prevent memory leaks.

#### Scenario: Unbind cleans up

- **WHEN** `Unmount()` is called on the view
- **THEN** all observable subscriptions are disposed and no further updates trigger rebuilds
