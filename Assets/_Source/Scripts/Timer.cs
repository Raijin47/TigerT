using System.Collections;
using UnityEngine;
using System;
using TMPro;
using DG.Tweening;

public class Timer : MonoBehaviour
{
    public event Action<bool> OnChangeState;

    [SerializeField] private GameObject _icon;
    [SerializeField] private RectTransform _glowImg;
    private TextMeshProUGUI _text;
    private Tween _tween;

    private const float RequiredTime = 120f;
    private float _currentTime;

    private float CurrentTime
    {
        get => _currentTime;
        set
        {
            _currentTime = Mathf.Clamp(value, 0, RequiredTime);
            _text.text = TextUtility.FormatSeconds(_currentTime);
        }
    }

    private Coroutine _coroutine;

    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        Game.Action.OnPause += Action_OnPause;
        Game.Action.OnEnter += Action_OnEnter;
        Game.Action.OnStart += Action_OnStart;
        Game.Action.OnRestart += Action_OnEnter;
        Game.Locator.Change.OnPickUpItem += AddTime;
    }

    private void Action_OnEnter()
    {
        CurrentTime = RequiredTime;
        _icon.SetActive(false);
    }

    private void AddTime()
    {
        CurrentTime += Game.Locator.Karma.Karma;

        Release();
        _coroutine = StartCoroutine(UpdateTimer());
    }

    private void Action_OnStart()
    {
        Release();
        _coroutine = StartCoroutine(UpdateTimer());
    }

    private void Action_OnPause(bool onPause)
    {
        Release();
        if (!onPause) _coroutine = StartCoroutine(UpdateTimer()); 
    }

    private IEnumerator UpdateTimer()
    {
        OnChangeState?.Invoke(false);
        _icon.SetActive(false);
        _tween?.Kill();

        while (CurrentTime > 0)
        {
            CurrentTime -= Time.deltaTime;
            yield return null;
        }

        OnChangeState?.Invoke(true);
        _icon.SetActive(true);
        StartTween();
        
    }

    private void StartTween()
    {
        _tween?.Kill();
        _tween = _glowImg.DOScale(0.5f, 2).From(1).SetLoops(-1);
    }

    private void Release()
    {
        if(_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }
}