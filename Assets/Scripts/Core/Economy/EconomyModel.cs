using R3;

namespace Core.Economy
{
    public class EconomyModel
    {
        public ReactiveProperty<int> Balance { get; } = new ReactiveProperty<int>(0);
    }
}
