using System;
using System.Collections.Generic;

public class AutoUpdateProperty<T>
{
    // Event that is called when the value is changed
    public event Action<T> OnValueChanged;

    // Function that is called when the value is changed
    private Action<T> _callback;

    private T _value;

    public T Value
    {
        get => _value;
        set
        {
            if (!EqualityComparer<T>.Default.Equals(_value, value))
            {
                _value = value;
                OnValueChanged?.Invoke(_value);
                _callback?.Invoke(_value);
            }
        }
    }

    public AutoUpdateProperty(T initialValue, Action<T> callback = null)
    {
        _value = initialValue;
        _callback = callback;
    }

    // implicit conversion operators
    public static implicit operator T(AutoUpdateProperty<T> property) => property.Value;

    public static implicit operator AutoUpdateProperty<T>(T value) => new AutoUpdateProperty<T>(value);
}