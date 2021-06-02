﻿namespace EEMod.Systems.Structurizer.PlacementActions
{
    public abstract class BaseRepeatedPlacementActionWithEntry<TPlacementAction> : BaseRepeatedPlacementAction<TPlacementAction> 
        where TPlacementAction : IPlacementAction
    {
        public override ushort EntryData { get; }

        protected BaseRepeatedPlacementActionWithEntry(ushort repetitionCount, ushort entryData) : base(repetitionCount)
        {
            EntryData = entryData;
        }
    }
}