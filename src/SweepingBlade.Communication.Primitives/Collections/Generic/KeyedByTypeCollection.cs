using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SweepingBlade.Communication.Primitives.Collections.Generic;

public class KeyedByTypeCollection<TItem> : KeyedCollection<Type, TItem>
{
    private const int DictionaryCreationThreshold = 4;

    public KeyedByTypeCollection()
        : base(null, DictionaryCreationThreshold)
    {
    }

    public KeyedByTypeCollection(IEnumerable<TItem> items)
        : base(null, DictionaryCreationThreshold)
    {
        if (items is null) throw new ArgumentNullException(nameof(items));
        foreach (var item in items)
        {
            Add(item);
        }
    }

    protected override Type GetKeyForItem(TItem item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        return item.GetType();
    }

    protected override void InsertItem(int index, TItem item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        if (Contains(item.GetType()))
        {
            var message = $"The value could not be added to the collection, as the collection already contains an item of the same type: '{item.GetType().FullName}'. This collection only supports one instance of each type.";
            throw new ArgumentException(message, nameof(item));
        }

        base.InsertItem(index, item);
    }

    protected override void SetItem(int index, TItem item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));

        base.SetItem(index, item);
    }

    public T Find<T>()
    {
        return Find<T>(false);
    }

    public Collection<T> FindAll<T>()
    {
        return FindAll<T>(false);
    }

    public T Remove<T>()
    {
        return Find<T>(true);
    }

    public Collection<T> RemoveAll<T>()
    {
        return FindAll<T>(true);
    }

    private T Find<T>(bool remove)
    {
        for (var i = 0; i < Count; i++)
        {
            var settings = this[i];
            if (settings is T)
            {
                if (remove)
                {
                    Remove(settings);
                }

                return (T)(object)settings;
            }
        }

        return default;
    }

    private Collection<T> FindAll<T>(bool remove)
    {
        var result = new Collection<T>();
        foreach (var settings in this)
        {
            if (settings is T)
            {
                result.Add((T)(object)settings);
            }
        }

        if (remove)
        {
            foreach (var settings in result)
            {
                Remove((TItem)(object)settings);
            }
        }

        return result;
    }
}