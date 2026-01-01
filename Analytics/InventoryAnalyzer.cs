namespace FantasyWorld.Analytics;

using FantasyWorld.Core.Items;

public class InventoryAnalyzer
{
    public void Analyze(IEnumerable<Item> items)
    {
        Console.WriteLine("--- インベントリ分析レポート ---");

        // LINQを使ったデータ変換と匿名型の生成
        var report = items
           .Where(i => i.Weight > 0) // 重さが0以上のものを対象
           .Select(i => new 
            {
                // プロパティ名の推論（Name = i.Name）
                i.Name,
                // 計算プロパティ
                IsHeavy = i.Weight > 5.0,
                TypeCategory = i.GetType().Name
            })
           .OrderByDescending(x => x.IsHeavy);

        foreach (var entry in report)
        {
            // コンパイラはentryがName, IsHeavyプロパティを持つことを知っている
            string status = entry.IsHeavy? "[重量級]" : "[軽量]";
            Console.WriteLine($"{status} {entry.Name} ({entry.TypeCategory})");
        }
    }
    
    // 匿名型のwith式（C# 10+）
    // 非破壊的変更のデモ
    public void DemoAnonymousMutation()
    {
        var original = new { Name = "Unknown", Value = 100 };
        var modified = original with { Name = "Identified" };
        
        Console.WriteLine($"Original: {original.Name}, Modified: {modified.Name}");
    }
}