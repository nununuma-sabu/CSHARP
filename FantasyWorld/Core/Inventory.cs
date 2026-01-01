namespace FantasyWorld.Core.Storage;

using FantasyWorld.Core.Items;
using System.Linq.Expressions;

// ---------------------------------------------------------
// ジェネリッククラス: Inventory<T>
// 制約: TはItemクラスを継承していなければならない
// ---------------------------------------------------------
public class Inventory<T>(int capacity) where T : Item
{
    private readonly List<T> _items = new();
    public int Capacity { get; } = capacity;

    public void Add(T item)
    {
        if (_items.Count >= Capacity)
        {
            Console.WriteLine("インベントリが一杯です！");
            return;
        }
        _items.Add(item);
        Console.WriteLine($"{item.Name}を収納しました。");
    }

    // 条件検索メソッド
    // Func<T, bool>を使用し、任意の条件でアイテムを検索可能にする
    public IEnumerable<T> Find(Func<T, bool> predicate)
    {
        return _items.Where(predicate);
    }

    public T? GetById(Guid id)
    {
        return _items.FirstOrDefault(i => i.Id == id);
    }
    
    public IEnumerable<T> GetAll() => _items;
}