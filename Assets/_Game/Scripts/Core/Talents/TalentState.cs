namespace App.Talents.Core
{
    using System.Collections.Generic;
    using R3;

    public class TalentState
    {
        public ReactiveProperty<int> AvailablePoints { get; } = new ReactiveProperty<int>(0);
        public Dictionary<string, int> PointsSpent { get; } = new Dictionary<string, int>();
    }
}
