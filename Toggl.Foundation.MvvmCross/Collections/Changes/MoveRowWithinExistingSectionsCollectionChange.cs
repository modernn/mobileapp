﻿namespace Toggl.Foundation.MvvmCross.Collections.Changes
{
    public struct MoveRowWithinExistingSectionsCollectionChange<T> : ICollectionChange
    {
        public SectionedIndex Index { get; }

        public SectionedIndex OldIndex { get; }

        public T Item { get; }

        public bool MoveToDifferentSection { get; }

        public MoveRowWithinExistingSectionsCollectionChange(
            SectionedIndex oldIndex,
            SectionedIndex index,
            T item,
            bool moveToDifferentSection)
        {
            OldIndex = oldIndex;
            Index = index;
            Item = item;
            MoveToDifferentSection = moveToDifferentSection;
        }

        public override string ToString() => $"MoveRow row: {OldIndex} -> {Index} ({Item})";
    }
}
