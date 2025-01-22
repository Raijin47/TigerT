using System;
using UnityEngine;

public class Health
{
    public event Action OnChange;
    public event Action OnDie;

    private readonly IDamageable Damageable;

    private int _maxHealth;
    private int _health;

    private bool _isDie;

    public Health(IDamageable idamageable)
    {
        Damageable = idamageable;
        Damageable.OnTakeDamage += TakeDamage;
    }

    public int MaxHealh 
    { 
        get => _maxHealth;
        set
        {
            _maxHealth = value;
            _health = value;
            OnChange?.Invoke();
            _isDie = false;
        }
    }

    public int Current 
    { 
        get => _health; 
        set
        {
            if (_isDie) return;

            _health = Mathf.Clamp(value, 0, MaxHealh);
            OnChange?.Invoke();

            if (_health != 0) return;

            OnDie?.Invoke();
            _isDie = true;
        } 
    }

    public void AddListaner() => Damageable.OnTakeDamage += TakeDamage;
    public void RemoveListaner() => Damageable.OnTakeDamage -= TakeDamage;
    private void TakeDamage(int value) => Current -= value;
}