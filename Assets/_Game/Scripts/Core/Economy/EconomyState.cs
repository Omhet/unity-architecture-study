using R3;

namespace App.Economy.Core
{
    public class EconomyState
    {
        public ReactiveProperty<int> Balance { get; } = new ReactiveProperty<int>(0);
    }
}
