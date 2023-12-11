using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TimelineTickType
{
    PhraseStart,
    Primary,
    Secondary,
    Teritary,
    Quaternary,
    Quinternary,
}

public struct TimelineTick
{
    public double Time { get; set; }
    public TimelineTickType Type { get; set; }

    public TimelineTick(double time, TimelineTickType type)
    {
        Time = time;
        Type = type;
    }
}

public class EditorTimeline : MonoBehaviour
{
    [SerializeField]
    private MapEditorManager _mapEditorManager;

    [SerializeField]
    private GameObject _beatTickPrefab;

    [SerializeField]
    private float _timelineSpacingMult = 1f;

    [SerializeField]
    private float _timelineSpacing = 20;

    private double _songCurrentTimeSeconds = 0.0;

    private List<TimelineTick> _timelineTicks = new();

    // Event handlers
    private Action<double> _songCurrentTimeSecondsAction;

    private void Awake()
    {
        _songCurrentTimeSecondsAction = time => _songCurrentTimeSeconds = time;
    }

    private void Start()
    {
        GenerateTickData();
        UpdateTicks();
    }

    private void Update()
    {
    }

    public void GenerateTickData()
    {
        _timelineTicks.Clear();

        double time = _mapEditorManager.SongTimeOffset;
        int tickCounter = 0;
        double songLength = _mapEditorManager.SongLengthSeconds.Value;
        while (time <= songLength)
        {
            double bpm = _mapEditorManager.SongBPMList.GetValueAt(time);
            int divisor = _mapEditorManager.SnapDivisorList.GetValueAt(time);
            Debug.Log($"Time: {time}, BPM: {bpm}, Divisor: {divisor}");

            var tickType = GetTickType(tickCounter, divisor);
            _timelineTicks.Add(new(time, tickType));

            tickCounter = (tickCounter + 1) % divisor;
            time += 60d / bpm;

            // check if divisor or bpm changed
            if (divisor != _mapEditorManager.SnapDivisorList.GetValueAt(time) || bpm != _mapEditorManager.SongBPMList.GetValueAt(time))
            {
                tickCounter = 0;
            }
        }
    }

    public void UpdateTicks()
    {
        // Update the beat ticks
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        foreach (var tick in _timelineTicks)
        {
            Vector3 tickPos = new Vector3((float)(tick.Time - _songCurrentTimeSeconds) * _timelineSpacing * _timelineSpacingMult, 0f, 0f);
            Vector3 tickScale = new Vector3(2, 30, 1);
            Color tickColor = Color.white;
            switch (tick.Type)
            {
                case TimelineTickType.PhraseStart:
                    tickScale.y = 30;
                    tickColor = Color.white;
                    break;

                case TimelineTickType.Primary:
                    tickScale.y = 16;
                    tickColor = Color.white;
                    break;

                case TimelineTickType.Secondary:
                    tickScale.y = 12;
                    tickColor = new Color(220, 220, 255);
                    break;

                case TimelineTickType.Teritary:
                    tickScale.y = 9;
                    tickColor = new Color(255, 220, 235);
                    break;

                case TimelineTickType.Quaternary:
                    tickScale.y = 7;
                    tickColor = new Color(245, 245, 235);
                    break;

                case TimelineTickType.Quinternary:
                    tickScale.y = 5;
                    tickColor = new Color(235, 255, 235);
                    break;

                default:
                    break;
            }

            // offset tick so that the bottoms line up
            tickPos.y -= (30 - tickScale.y) / 2;

            var tickObject = Instantiate(_beatTickPrefab, transform);
            tickObject.transform.localPosition = tickPos;
            tickObject.transform.localScale = tickScale;
            var img = tickObject.GetComponent<Image>();
            img.color = tickColor;
        }
    }

    private TimelineTickType GetTickType(int tickCounter, int snapDivisor)
    {
        Debug.Log($"Tick counter: {tickCounter}, Snap divisor: {snapDivisor}");
        tickCounter %= snapDivisor;

        switch (snapDivisor)
        {
            case 1:
                return DivisorOne(tickCounter);

            case 2:
                return DivisorHalf(tickCounter);

            case 3:
                return DivisorThird(tickCounter);

            case 4:
                return DivisorFourth(tickCounter);

            case 5:
                return DivisorUniversal(tickCounter);

            case 6:
                return DivisorSixth(tickCounter);

            case 7:
                return DivisorUniversal(tickCounter);

            case 8:
                return DivisorEight(tickCounter);

            default:
                {
                    Debug.LogWarning($"Snap divisor {snapDivisor} not found!");
                    return DivisorUniversal(tickCounter);
                }
        }
    }

    #region SnapDivisor Functions

    private TimelineTickType DivisorOne(int tickCounter)
    {
        if (tickCounter == 0)
        {
            return TimelineTickType.PhraseStart;
        }
        return TimelineTickType.Primary;
    }

    private TimelineTickType DivisorHalf(int tickCounter)
    {
        if (tickCounter == 0)
        {
            return TimelineTickType.PhraseStart;
        }

        if (tickCounter % 2 == 0)
        {
            return TimelineTickType.Primary;
        }

        return TimelineTickType.Secondary;
    }

    private TimelineTickType DivisorThird(int tickCounter)
    {
        if (tickCounter == 0)
        {
            return TimelineTickType.PhraseStart;
        }

        return TimelineTickType.Primary;
    }

    private TimelineTickType DivisorFourth(int tickCounter)
    {
        if (tickCounter == 0)
        {
            return TimelineTickType.PhraseStart;
        }

        if (tickCounter % 2 == 0)
        {
            return TimelineTickType.Primary;
        }

        if (tickCounter % 4 == 0)
        {
            return TimelineTickType.Secondary;
        }

        return TimelineTickType.Teritary;
    }

    private TimelineTickType DivisorSixth(int tickCounter)
    {
        if (tickCounter == 0)
        {
            return TimelineTickType.PhraseStart;
        }

        if (tickCounter % 3 == 0)
        {
            return TimelineTickType.Primary;
        }

        return TimelineTickType.Secondary;
    }

    private TimelineTickType DivisorEight(int tickCounter)
    {
        if (tickCounter == 0)
        {
            return TimelineTickType.PhraseStart;
        }

        if (tickCounter % 2 == 0)
        {
            return TimelineTickType.Primary;
        }

        if (tickCounter % 4 == 0)
        {
            return TimelineTickType.Secondary;
        }

        if (tickCounter % 8 == 0)
        {
            return TimelineTickType.Teritary;
        }

        return TimelineTickType.Quaternary;
    }

    private TimelineTickType DivisorUniversal(int tickCounter)
    {
        if (tickCounter == 0)
        {
            return TimelineTickType.PhraseStart;
        }
        return TimelineTickType.Primary;
    }

    #endregion SnapDivisor Functions

    private void OnEnable()
    {
        _mapEditorManager.SongCurrentTimeSeconds.OnValueChanged += _songCurrentTimeSecondsAction;
    }

    private void OnDisable()
    {
        _mapEditorManager.SongCurrentTimeSeconds.OnValueChanged -= _songCurrentTimeSecondsAction;
    }
}