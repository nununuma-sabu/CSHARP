// ---------------------------------------------------------
// 派生クラス: Consumable (消費アイテム)
// ---------------------------------------------------------
public class Consumable(string name, double weight, int effectValue) : Item(name, weight)
{
    // 効果値をプロパティに割り当てる
    public int EffectValue { get; } = effectValue;

    // 抽象メソッドをオーバーライドして実装
    public override void Use()
    {
        Console.WriteLine($"{Name}を使用しました。効果値: {EffectValue}");
    }

    // 基底クラスのメソッドをオーバーライド
    public override string GetDescription()
    {
        return $"{base.GetDescription()} - 効果: {EffectValue}";
    }
}
