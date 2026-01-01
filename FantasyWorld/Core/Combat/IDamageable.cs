namespace FantasyWorld.Core.Combat;

// ---------------------------------------------------------
// インターフェース: IDamageable
// ダメージを受けることができるすべてのエンティティが実装する
// ---------------------------------------------------------
public interface IDamageable
{
    int CurrentHealth { get; }
    void TakeDamage(int amount);

    // デフォルト実装（C# 8.0+）
    // 必須ではないが、共通のロジックを提供できる
    bool IsAlive => CurrentHealth > 0;

    // 新機能の追加例（既存の実装クラスを壊さない）
    void LogStatus()
    {
        Console.WriteLine(IsAlive? "Target is functional." : "Target is down.");
    }
}