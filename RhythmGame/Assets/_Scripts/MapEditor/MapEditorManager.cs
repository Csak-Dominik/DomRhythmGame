using System;
using UnityEngine;

public class MapEditorManager : MonoBehaviour
{
    public AutoUpdateProperty<double> SongCurrentTimeSeconds;

    public AutoUpdateProperty<double> SongLengthSeconds = new(0.0);

    public SliceList<double> SongBPMList = new();

    public SliceList<int> SnapDivisorList = new();

    public SliceList<double> ScrollSpeedList = new();

    public double CurrentBPM => SongBPMList.GetValueAt(SongCurrentTimeSeconds);
    public double CurrentBeatLength => 60.0 / SongBPMList.GetValueAt(SongCurrentTimeSeconds);

    public int CurrentSnapDivisor => SnapDivisorList.GetValueAt(SongCurrentTimeSeconds);

    public double CurrentScrollSpeed => ScrollSpeedList.GetValueAt(SongCurrentTimeSeconds);

    public double SongTimeOffset = 0.0;

    public event Action<double> SongCurrentTimeChanged;

    private void Awake()
    {
        SongCurrentTimeSeconds = new(0.0, time => SongCurrentTimeChanged?.Invoke(time));

        // test data
        SongBPMList.Add(new(0, 120));
        SnapDivisorList.Add(new(0, 16));

        SongBPMList.Add(new(10, 180));
        SnapDivisorList.Add(new(10, 9));

        SongBPMList.Add(new(20, 240));
        SnapDivisorList.Add(new(20, 7));

        SongBPMList.Add(new(30, 80));
        SnapDivisorList.Add(new(30, 5));

        SongBPMList.Add(new(40, 60));
        SnapDivisorList.Add(new(40, 12));

        SongLengthSeconds = 50;
        //SongCurrentTimeSeconds = 10;
    }

    // Loads all the maps from the maps folder
    public void LoadMaps()
    {
        // find all files in the maps folder with extension
    }

    // Opens a map in the editor
    public void OpenMap(string id)
    {
    }
}