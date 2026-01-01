// ---------------------------------------------------------
// File: FantasyRPG.cs
// 全モジュールを統合した実行可能なサンプル
// ---------------------------------------------------------
using FantasyWorld.Core;
using FantasyWorld.Core.Items;
using FantasyWorld.Core.Combat;
using FantasyWorld.Core.Storage;
using FantasyWorld.Core.Quests;

namespace FantasyWorld.Capstone;

public static class GameRunner
{
    public static void Run()
    {
        Console.WriteLine("=== Fantasy RPG System Initialized ===");

        // 1. インベントリのセットアップ（ジェネリック）
        // あらゆるItemを格納できるバックパック
        var backpack = new Inventory<Item>(GameConstants.MaxInventorySize);

        // 2. アイテムの作成（クラス、継承、プライマリコンストラクタ）
        var potion = new Consumable("Healing Potion", 0.5, 50);
        var sword = new Weapon("Steel Sword", 3.0, 15);
        
        // 3. インターフェースの活用
        // 箱を作成し、IDamageableとしてダメージを与える
        IDamageable targetBox = new DestructibleBox(100);
        targetBox.TakeDamage(sword.Damage); // 剣の攻撃力でダメージ計算

        // 4. インベントリ操作
        backpack.Add(potion);
        backpack.Add(sword);

        // 5. レコードと不変データ
        var questSteps = new List<QuestStep>
        {
            new(1, "装備を整える", true)
        };
        var mainQuest = new QuestLog("魔王討伐", questSteps);

        // クエスト進行（with式による非破壊更新）
        var nextStep = new QuestStep(2, "ダンジョンへ向かう", false);
        // リストへの追加ロジックを含んだ新しいレコードを作成
        var updatedQuest = mainQuest with 
        { 
            Steps = mainQuest.Steps.Append(nextStep).ToList().AsReadOnly() 
        };

        // 6. LINQと匿名型による分析
        var analyzer = new FantasyWorld.Analytics.InventoryAnalyzer();
        analyzer.Analyze(backpack.GetAll());

        // 7. アイテムの使用（ポリモーフィズム）
        Console.WriteLine("\n--- アイテム使用テスト ---");
        var foundItem = backpack.GetById(potion.Id);
        foundItem?.Use(); // Consumable.Use()が呼ばれる
    }
}