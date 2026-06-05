## ADDED Requirements

### Requirement: ShopState holds observable available shop item IDs

The ShopState SHALL expose an `ObservableList<string>` named `AvailableShopItemIds` that represents the currently available items in the shop for purchase.

#### Scenario: Initial state is empty

- **WHEN** a new ShopState instance is created
- **THEN** `AvailableShopItemIds` is an empty observable list

#### Scenario: Items can be set

- **WHEN** the available IDs collection is updated with shop item IDs
- **THEN** subscribers to the observable list receive change notifications

### Requirement: RefreshAvailability sets available items from progression registry

The ShopService SHALL provide a `RefreshAvailability(playerLevel)` method that queries ShopProgressionRegistry for all unlocked shop item IDs up to the given level and assigns them to ShopState.AvailableShopItemIds.

#### Scenario: Refresh at level 1

- **WHEN** `RefreshAvailability(1)` is called and progression registry has items unlocked at level 1
- **THEN** ShopState.AvailableShopItemIds contains exactly those level 1 shop item IDs

#### Scenario: Refresh at higher level accumulates items

- **WHEN** `RefreshAvailability(3)` is called and progression registry has items at levels 1, 2, and 3
- **THEN** ShopState.AvailableShopItemIds contains all shop item IDs from levels 1 through 3

#### Scenario: Refresh does not filter by ownership

- **WHEN** `RefreshAvailability(level)` is called and some unlocked items are already owned by the player
- **THEN** owned items still appear in AvailableShopItemIds (shop has infinite supply)
