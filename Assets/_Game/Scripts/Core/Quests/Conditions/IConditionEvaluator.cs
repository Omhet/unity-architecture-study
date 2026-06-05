namespace App.Quests.Core.Conditions
{
    using System;
    using R3;

    public interface IConditionEvaluator
    {
        bool IsMet();
        Observable<bool> Observe();
    }
}
