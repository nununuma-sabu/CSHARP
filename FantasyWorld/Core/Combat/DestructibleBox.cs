namespace FantasyWorld.Core.Combat;

// ---------------------------------------------------------
// 実装クラス: DestructibleBox (破壊可能な箱)
// ---------------------------------------------------------
public class DestructibleBox(int initialHealth) : IDamageable
{
    private int _health = initialHealth;

    // 暗黙的実装: クラスのインスタンスから直接アクセス可能
    public int CurrentHealth => _health;

    public void TakeDamage(int amount)
    {
        _health -= amount;
        if (_health < 0) _health = 0;
        Console.WriteLine($"箱に{amount}のダメージ！ 残りHP: {_health}");
    }
}