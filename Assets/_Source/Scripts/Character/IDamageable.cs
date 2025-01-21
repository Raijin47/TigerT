using System;

public interface IDamageable
{
    public event Action<int> OnTakeDamage;
    public void ApplyDamage(int value);
}