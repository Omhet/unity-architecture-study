namespace App.Quests.Core
{
    using System;

    [Serializable]
    public class ConditionData
    {
        public string Type;
        public string TargetId;
        public int Threshold;
    }
}
