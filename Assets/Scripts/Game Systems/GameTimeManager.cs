using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimeManager : MonoBehaviour
{
    private GameTimeManager _instance;
    public GameTimeManager instance => _instance;

    private class TimeScaleMultiplier
    {
        private float _timeScale;
        private float _lifetime;

        private float _timeCounter;

        public float timeScale => _timeScale;

        public TimeScaleMultiplier(float timeScale, float lifetime)
        {
            _timeScale = timeScale;
            _lifetime = lifetime;
            _timeCounter = 0;
        }

        public void UpdateTimeCounter(float deltaTime, out bool hasEnded)
        {
            if (_lifetime == 0)
            {
                hasEnded = false;
                return;
            }
            _timeCounter += deltaTime;
            hasEnded = _timeCounter >= _lifetime;
        }

        public void AddTime(float timeToAdd)
        {
            _lifetime += timeToAdd;
        }

        public void UpdateTimeScale(float newTimeScale)
        {
            _timeScale = newTimeScale;
        }

        public float TimeToEnd()
        {
            return _lifetime - _timeCounter;
        }
    }

    private static Dictionary<object, TimeScaleMultiplier> _timeScaleMultipliers = new Dictionary<object, TimeScaleMultiplier>();
    private static List<object> _finishedTimeScaleMultipliers = new List<object>();

    public enum OverrideMethod
    {
        Additive,
        OverrideShorter,
        OverrideAlways,
        Never
    }

    private void Update()
    {
        UpdateTimeScaleMultipliers();

        float totalTimeScale = 1;
        foreach (KeyValuePair<object, TimeScaleMultiplier> timeScaleMultiplier in _timeScaleMultipliers)
        {
            totalTimeScale *= timeScaleMultiplier.Value.timeScale;
        }

        Time.timeScale = totalTimeScale;
    }

    public static void CreateTimeScaleMultiplier(object creator, float timeScaleMultiplier, float lifetime = 0, OverrideMethod overrideMethod = OverrideMethod.OverrideAlways)
    {
        if (!_timeScaleMultipliers.ContainsKey(creator))
        {
            _timeScaleMultipliers.Add(creator, new TimeScaleMultiplier(timeScaleMultiplier, lifetime));
            return;
        }

        switch (overrideMethod)
        {
            case OverrideMethod.Never:
                return;
            case OverrideMethod.Additive:
                _timeScaleMultipliers[creator].AddTime(lifetime);
                _timeScaleMultipliers[creator].UpdateTimeScale(timeScaleMultiplier);
                break;
            case OverrideMethod.OverrideShorter:
                if (_timeScaleMultipliers[creator].TimeToEnd() <= lifetime)
                {
                    _timeScaleMultipliers[creator] = new TimeScaleMultiplier(timeScaleMultiplier, lifetime);
                }
                break;
            case OverrideMethod.OverrideAlways:
                _timeScaleMultipliers[creator] = new TimeScaleMultiplier(timeScaleMultiplier, lifetime);
                break;
        }
    }

    public static void DestroyTimeScaleMultiplier(object creator)
    {
        _timeScaleMultipliers.Remove(creator);
    }

    private void UpdateTimeScaleMultipliers()
    {
        
        foreach (KeyValuePair<object, TimeScaleMultiplier> timeScaleMultiplier in _timeScaleMultipliers)
        {
            timeScaleMultiplier.Value.UpdateTimeCounter(Time.unscaledDeltaTime, out bool hasEnded);
            if (hasEnded)
            {
                _finishedTimeScaleMultipliers.Add(timeScaleMultiplier.Key);
            }
        }

        foreach (object creator in _finishedTimeScaleMultipliers)
        {
            _timeScaleMultipliers.Remove(creator);
        }
        _finishedTimeScaleMultipliers.Clear();
    }
}
