namespace App.Systems.Saving.Options
{
    /// <summary>
    /// Configuration for the save system at boot time.
    /// </summary>
    public class SaveBootstrapOptions
    {
        public int SlotCount = 4;
        public int ActiveSlotIndex = 0;
    }
}
