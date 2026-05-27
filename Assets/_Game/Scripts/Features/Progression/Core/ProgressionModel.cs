namespace App.Progression.Core
{
    using R3;

    public class ProgressionModel
    {
        public ReactiveProperty<int> Level { get; } = new ReactiveProperty<int>(1);
        public ReactiveProperty<int> Experience { get; } = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> TalentPoints { get; } = new ReactiveProperty<int>(0);
    }
}
