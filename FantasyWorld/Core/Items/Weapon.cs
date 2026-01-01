namespace FantasyWorld.Core.Items;
// ---------------------------------------------------------
// 派生クラス: Weapon (装備品)
// ---------------------------------------------------------
public class Weapon(string name, double weight, int damage) : Item(name, weight)
{
    // 攻撃力をプロパティに割り当てる
    public int Damage { get; } = damage;

    // 抽象メソッドをオーバーライドして実装
    public override void Use()
    {
        Console.WriteLine($"{Name}を装備しました。攻撃力: {Damage}");
    }
}