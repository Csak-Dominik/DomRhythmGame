using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TimelineTickType
{
    PhraseStart,
    Primary,
    Secondary,
    Tertiary,
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
    private const int MAX_TICK_HEIGHT = 30;

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

            tickCounter = (tickCounter + 1) % (divisor * 4);
            time += 60d / bpm / divisor;

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
        transform.DestoryAllChildren();

        foreach (var tick in _timelineTicks)
        {
            Vector3 tickPos = new Vector3((float)(tick.Time - _songCurrentTimeSeconds) * _timelineSpacing * _timelineSpacingMult, 0f, 0f);
            Vector3 tickScale = new Vector3(2, 30, 1);
            Vector3 tickColor;
            switch (tick.Type)
            {
                case TimelineTickType.PhraseStart:
                    tickScale.y = 30;
                    tickColor = new Vector3(255, 255, 255);
                    break;

                case TimelineTickType.Primary:
                    tickScale.y = 16;
                    tickColor = new Vector3(255, 255, 255);
                    break;

                case TimelineTickType.Secondary:
                    tickScale.y = 12;
                    tickColor = new Vector3(130, 130, 255);
                    break;

                case TimelineTickType.Tertiary:
                    tickScale.y = 9;
                    tickColor = new Vector3(255, 130, 185);
                    break;

                case TimelineTickType.Quaternary:
                    tickScale.y = 7;
                    tickColor = new Vector3(235, 235, 145);
                    break;

                case TimelineTickType.Quinternary:
                    tickScale.y = 5;
                    tickColor = new Vector3(140, 255, 140);
                    break;

                default:
                    tickColor = new Vector3(255, 0, 255);
                    break;
            }

            // shrink tick height so that it fits in the timeline
            tickScale.y /= MAX_TICK_HEIGHT;

            // offset tick so that the bottoms line up
            tickPos.y -= (MAX_TICK_HEIGHT - tickScale.y) / 2;

            // offset to the bottom of the timeline
            tickPos.y += 20;

            var tickObject = Instantiate(_beatTickPrefab, transform);
            tickObject.transform.localPosition = tickPos;
            tickObject.transform.localScale = tickScale;
            var img = tickObject.GetComponent<Image>();
            Color col = new Color(tickColor.x / 255f, tickColor.y / 255f, tickColor.z / 255f);
            Debug.Log(col);
            img.color = col;
        }
    }

    private TimelineTickType GetTickType(int tickCounter, int snapDivisor)
    {
        Debug.Log($"Tick counter: {tickCounter}, Snap divisor: {snapDivisor}");
        tickCounter %= (snapDivisor * 4);

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
                return DivisorUniversal(tickCounter, snapDivisor);

            case 6:
                return DivisorSixth(tickCounter);

            case 7:
                return DivisorUniversal(tickCounter, snapDivisor);

            case 8:
                return DivisorEight(tickCounter);

            case 9:
                return DivisorNinth(tickCounter);

            case 12:
                return DivisorTwelfth(tickCounter);

            case 16:
                return DivisorSixteenth(tickCounter);

            default:
                {
                    Debug.LogWarning($"Snap divisor {snapDivisor} not found!");
                    return DivisorUniversal(tickCounter, snapDivisor);
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

        if (tickCounter % 3 == 0)
        {
            return TimelineTickType.Primary;
        }

        return TimelineTickType.Secondary;
    }

    private TimelineTickType DivisorFourth(int tickCounter)
    {
        if (tickCounter == 0)
        {
            return TimelineTickType.PhraseStart;
        }

        if (tickCounter % 4 == 0)
        {
            return TimelineTickType.Primary;
        }

        if (tickCounter % 2 == 0)
        {
            return TimelineTickType.Secondary;
        }

        return TimelineTickType.Tertiary;
    }

    private TimelineTickType DivisorSixth(int tickCounter)
    {
        if (tickCounter == 0)
        {
            return TimelineTickType.PhraseStart;
        }

        if (tickCounter % 6 == 0)
        {
            return TimelineTickType.Primary;
        }

        if (tickCounter % 3 == 0)
        {
            return TimelineTickType.Secondary;
        }

        return TimelineTickType.Tertiary;
    }

    private TimelineTickType DivisorEight(int tickCounter)
    {
        if (tickCounter == 0)
        {
            return TimelineTickType.PhraseStart;
        }

        if (tickCounter % 8 == 0)
        {
            return TimelineTickType.Primary;
        }

        if (tickCounter % 4 == 0)
        {
            return TimelineTickType.Secondary;
        }

        if (tickCounter % 2 == 0)
        {
            return TimelineTickType.Tertiary;
        }

        return TimelineTickType.Quaternary;
    }

    private TimelineTickType DivisorNinth(int tickCounter)
    {
        if (tickCounter == 0)
        {
            return TimelineTickType.PhraseStart;
        }

        if (tickCounter % 9 == 0)
        {
            return TimelineTickType.Primary;
        }

        if (tickCounter % 3 == 0)
        {
            return TimelineTickType.Secondary;
        }

        return TimelineTickType.Tertiary;
    }

    private TimelineTickType DivisorTwelfth(int tickCounter)
    {
        if (tickCounter == 0)
        {
            return TimelineTickType.PhraseStart;
        }

        if (tickCounter % 12 == 0)
        {
            return TimelineTickType.Primary;
        }

        if (tickCounter % 6 == 0)
        {
            return TimelineTickType.Secondary;
        }

        if (tickCounter % 3 == 0)
        {
            return TimelineTickType.Tertiary;
        }

        return TimelineTickType.Quaternary;
    }

    private TimelineTickType DivisorSixteenth(int tickCounter)
    {
        if (tickCounter == 0)
        {
            return TimelineTickType.PhraseStart;
        }

        if (tickCounter % 16 == 0)
        {
            return TimelineTickType.Primary;
        }

        if (tickCounter % 8 == 0)
        {
            return TimelineTickType.Secondary;
        }

        if (tickCounter % 4 == 0)
        {
            return TimelineTickType.Tertiary;
        }

        if (tickCounter % 2 == 0)
        {
            return TimelineTickType.Quaternary;
        }

        return TimelineTickType.Quinternary;
    }

    private TimelineTickType DivisorUniversal(int tickCounter, int divisor)
    {
        if (tickCounter == 0)
        {
            return TimelineTickType.PhraseStart;
        }

        if (tickCounter % divisor == 0)
        {
            return TimelineTickType.Primary;
        }

        return TimelineTickType.Secondary;
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