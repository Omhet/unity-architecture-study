using R3;

namespace App.Economy.Core
{
    public class EconomyModel
    {
        public ReactiveProperty<int> Balance { get; } = new ReactiveProperty<int>(0);
    }
}
