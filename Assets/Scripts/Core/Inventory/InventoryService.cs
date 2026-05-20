namespace Core.Inventory
{
    public class InventoryService
    {
        private readonly InventoryModel _model;

        public InventoryService(InventoryModel model)
        {
            _model = model;
        }

        public void AddItem(string itemId, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemId) || quantity <= 0) return;

            if (_model.Items.TryGetValue(itemId, out int currentQuantity))
            {
                _model.Items[itemId] = currentQuantity + quantity;
            }
            else
            {
                _model.Items.Add(itemId, quantity);
            }
        }

        public bool TryRemoveItem(string itemId, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemId) || quantity <= 0) return false;

            if (_model.Items.TryGetValue(itemId, out int currentQuantity))
            {
                if (currentQuantity >= quantity)
                {
                    int newQuantity = currentQuantity - quantity;
                    if (newQuantity == 0)
                    {
                        _model.Items.Remove(itemId);
                    }
                    else
                    {
                        _model.Items[itemId] = newQuantity;
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
