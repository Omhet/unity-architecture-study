using ObservableCollections;

namespace Core.Inventory
{
    public class InventoryModel
    {
        // Key: Item ID (e.g., "sword_1"), Value: Quantity stacked
        public ObservableDictionary<string, int> Items { get; } = new ObservableDictionary<string, int>();
    }
}
