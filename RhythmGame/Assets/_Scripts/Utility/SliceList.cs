using System.Collections.Generic;
using System.Diagnostics;

public struct Slice<T>
{
    public double Time { get; set; }
    public T Value { get; set; }

    public Slice(double time, T value)
    {
        Time = time;
        Value = value;
    }
}

public class SliceList<T>
{
    private const double EPSILON = 0.0001;

    private List<Slice<T>> slices;

    public Slice<T> this[int index]
    {
        get => slices[index];
        set => slices[index] = value;
    }

    public int Count => slices.Count;

    public T GetValueAt(double time)
    {
        if (slices.Count == 0) return default;

        Debug.WriteLine($"Time: {time} {slices[0].Time}");
        if (time < slices[0].Time - EPSILON) return default;

        int index = Search(time);
        if (index < slices.Count)
        {
            return slices[index].Value;
        }

        return slices[^1].Value;
    }

    public SliceList(List<Slice<T>> elements = null)
    {
        // Sort the list if it's not null
        elements?.Sort((a, b) =>
        {
            if (a.Time < b.Time) return -1;
            else if (a.Time > b.Time) return 1;
            else return 0;
        });
        slices = elements ?? new List<Slice<T>>();
    }

    private int Search(double time)
    {
        // lower bound
        int min = slices.Count - 1;
        while (min >= 0 && slices[min].Time - EPSILON >= time) min--;

        return min < 0 ? 0 : min;
    }

    public void Add(Slice<T> slice)
    {
        slices.Add(slice);
        slices.Sort((a, b) =>
        {
            if (a.Time < b.Time) return -1;
            else if (a.Time > b.Time) return 1;
            else return 0;
        });
    }

    public void RemoveAt(int index)
    {
        slices.RemoveAt(index);
    }

    public void Clear()
    {
        slices.Clear();
    }
}