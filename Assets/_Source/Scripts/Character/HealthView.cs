using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthView : MonoBehaviour, IDamageable
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _image;
    private Health _health;
    private Sequence _sequence;

    public event Action<int> OnTakeDamage;

    private void Start()
    {
        _health = new(this);
        _health.OnChange += UpdateUI;
        _health.MaxHealh = 100;
    }

    private void UpdateUI()
    {
        _sequence?.Kill();

        _sequence = DOTween.Sequence();

        _sequence.Append(_slider.DOValue(_health.Current, .5f));
    }

    public void ApplyDamage(int value) => OnTakeDamage?.Invoke(value);
}