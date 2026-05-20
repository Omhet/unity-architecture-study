using UnityEngine;

namespace Flow.Events
{
    public readonly struct CoinEarnedEvent
    {
        public readonly Vector2 ScreenPosition;

        public CoinEarnedEvent(Vector2 screenPosition)
        {
            ScreenPosition = screenPosition;
        }
    }

    public readonly struct ItemBoughtEvent
    {
        public readonly string ItemId;

        public ItemBoughtEvent(string itemId)
        {
            ItemId = itemId;
        }
    }
}
