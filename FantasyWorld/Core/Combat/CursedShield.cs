namespace FantasyWorld.Core.Combat;

// ---------------------------------------------------------
// 実装クラス: CursedShield (呪われた盾)
// 盾として使えるが、ダメージを受ける機能は隠蔽したい
// ---------------------------------------------------------
public class CursedShield : IDamageable
{
    private int _durability = 100;

    // プロパティの明示的実装
    int IDamageable.CurrentHealth => _durability;

    // メソッドの明示的実装
    // CursedShieldインスタンスからは直接呼び出せない
    void IDamageable.TakeDamage(int amount)
    {
        _durability -= amount / 2; // 盾なのでダメージ軽減
        Console.WriteLine($"盾が攻撃を受け止めた。耐久値: {_durability}");
    }
}