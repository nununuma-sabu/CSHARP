namespace FantasyWorld.Core.Items;
// Itemクラスはabstractとして定義されており、
// 具体的なアイテム（ポーションや剣）の共通概念を表す。
// ConsumableやWeaponはそれを継承し、
// Useメソッドの具体的な振る舞いを定義している。
// これにより、インベントリシステムは個別の型を知らなくても、
// Item型として統一的に扱うことができる（ポリモーフィズム）。


// ---------------------------------------------------------
// 抽象基底クラス: Item
// すべてのアイテムの共通定義。直接インスタンス化は不可。
// ---------------------------------------------------------
public abstract class Item(string name, double weight)
{
    // プライマリコンストラクタの引数をプロパティに割り当てる
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; } = name;
    public double Weight { get; } = weight;

    // 仮想メソッド: 派生クラスでオーバーライド可能
    public virtual string GetDescription()
    {
        return $"{Name} (Weight: {Weight}kg)";
    }

    // 抽象メソッド: 派生クラスで必ず実装しなければならない
    public abstract void Use();
}


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