﻿using System.Globalization;
using System.Linq;

// ReSharper disable MemberCanBePrivate.Global

namespace HanumanInstitute.Avalonia;

/// <summary>
/// Represents the generic collection view for collections of items that implement <see cref="ListItem{T}"/>.
/// </summary>
/// <typeparam name="T">The type of list.</typeparam>
public class ListItemCollectionView<T> : CollectionView<ListItem<T>>
{
    public ListItemCollectionView() : base(new List<ListItem<T>>())
    {
    }

    public ListItemCollectionView(IList<ListItem<T>> list) : base(list)
    {
    }

    /// <summary>
    /// Adds a new ListItem to the list. This can be used for easy one-line initialization of the collection.
    /// </summary>
    /// <param name="value">The typed value to add.</param>
    /// <param name="text">The text to display for the item.</param>
    /// <returns>The newly-added item.</returns>
    public ListItem<T> Add(T value, string text)
    {
        var item = new ListItem<T>(text, value);
        Source.Add(item);
        return item;
    }

    /// <summary>
    /// Adds a new ListItem to the list. This can be used for easy one-line initialization of the collection.
    /// </summary>
    /// <param name="value">The typed value to add.</param>
    /// <returns>The newly-added item.</returns>
    public ListItem<T> Add(T value) => Add(value, Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty);

    /// <summary>
    /// Gets or sets the currently selected value.
    /// </summary>
    public T? SelectedValue
    {
        get => CurrentItem != null ? CurrentItem.Value : default;
        set => CurrentItem = value != null ? 
            Source.FirstOrDefault(x => x.Value?.Equals(value) == true) :
            Source.FirstOrDefault(x => x.Value == null);
    }

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        
        if (propertyName == nameof(CurrentItem))
        {
            OnPropertyChanged(nameof(SelectedValue));
        }
    }
}
