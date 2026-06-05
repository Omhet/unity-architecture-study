namespace App.Quests.Core
{
    using R3;

    public class ActiveQuest
    {
        public string Id { get; }
        public int XpReward { get; }
        public ReactiveProperty<bool> IsClaimable { get; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> IsCompleted { get; } = new ReactiveProperty<bool>(false);

        public ActiveQuest(string id, int xpReward)
        {
            Id = id;
            XpReward = xpReward;
        }
    }
}
