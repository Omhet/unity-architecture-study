namespace App.Quests.Core
{
    using R3;

    public class QuestProgressData
    {
        public ReactiveProperty<bool> IsClaimable { get; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> IsCompleted { get; } = new ReactiveProperty<bool>(false);
    }
}
