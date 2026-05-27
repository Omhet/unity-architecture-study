namespace App.View
{
    public readonly struct GameplaySectionDefinition
    {
        public readonly string Id;
        public readonly string TabTitle;
        public readonly int TabOrder;

        public GameplaySectionDefinition(string id, string tabTitle, int tabOrder)
        {
            Id = id;
            TabTitle = tabTitle;
            TabOrder = tabOrder;
        }
    }
}