# FantasyWorld - C# 型システム ハンズオン教材

C#の型システムの主要概念を、ファンタジーRPGの世界観を通じて学ぶためのハンズオン教材です。

## 📚 学習目標

このプロジェクトを通じて、以下のC#の型システム機能を実践的に学びます：

- **クラスと継承** - オブジェクト指向の基礎
- **抽象クラスとインターフェース** - 契約と実装の分離
- **ジェネリクス** - 型安全な汎用コード
- **レコード** - 不変データモデル
- **プライマリコンストラクタ** - 簡潔な初期化構文
- **LINQ と匿名型** - データ変換とクエリ

## 🎮 プロジェクト構成

```
FantasyWorld/
├── Program.cs                  # エントリーポイント
├── GlobalUsings.cs             # グローバル名前空間定義
├── FantasyRPG.cs              # 統合実行サンプル
├── InventoryAnalyzer.cs       # LINQ分析デモ
└── Core/
    ├── Constants.cs           # ゲーム定数
    ├── Items/
    │   ├── Item.cs           # 抽象基底クラス
    │   ├── Consumable.cs     # 消費アイテム
    │   └── Weapon.cs         # 武器
    ├── Storage/
    │   └── Inventory.cs      # ジェネリックインベントリ
    ├── Combat/
    │   ├── IDamageable.cs    # ダメージ受付インターフェース
    │   ├── DestructibleBox.cs
    │   └── CursedShield.cs
    └── Quests/
        ├── QuestStep. cs      # シンプルレコード
        └── QuestLog.cs       # 複雑なレコード
```

## 🔍 学習内容の詳細

### 1. クラスと継承 (`Core/Items/`)

**抽象基底クラス `Item`**
```csharp
public abstract class Item(string name, double weight)
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; } = name;
    public double Weight { get; } = weight;
    
    public abstract void Use(); // 派生クラスで実装必須
}
```

**学習ポイント：**
- プライマリコンストラクタの使い方
- `abstract` による直接インスタンス化の禁止
- 仮想メソッドと抽象メソッドの違い
- ポリモーフィズムの実践

**実装クラス：**
- `Consumable` - 回復ポーションなどの消費アイテム
- `Weapon` - 剣や盾などの装備品

### 2. インターフェース (`Core/Combat/IDamageable.cs`)

```csharp
public interface IDamageable
{
    int CurrentHealth { get; }
    void TakeDamage(int amount);
    
    // C# 8.0+ デフォルト実装
    bool IsAlive => CurrentHealth > 0;
}
```

**学習ポイント：**
- インターフェースの役割と契約設計
- 暗黙的実装 vs 明示的実装
- デフォルト実装による後方互換性

**実装例：**
- `DestructibleBox` - 暗黙的実装（通常の使い方）
- `CursedShield` - 明示的実装（メンバーの隠蔽）

### 3. ジェネリクス (`Core/Storage/Inventory.cs`)

```csharp
public class Inventory<T>(int capacity) where T : Item
{
    private readonly List<T> _items = new();
    
    public IEnumerable<T> Find(Func<T, bool> predicate)
    {
        return _items.Where(predicate);
    }
}
```

**学習ポイント：**
- 型パラメータと制約 (`where T : Item`)
- 型安全性の確保
- `Func<T, bool>` による柔軟な条件検索
- ジェネリックコレクションの活用

### 4. レコード (`Core/Quests/`)

**シンプルなレコード：**
```csharp
public record QuestStep(int StepNumber, string Description, bool IsCompleted);
```

**複雑なレコード：**
```csharp
public record QuestLog
{
    public string QuestName { get; init; }
    public IReadOnlyList<QuestStep> Steps { get; init; }
    
    // カスタム等価判定（リストの中身を比較）
    public virtual bool Equals(QuestLog?  other)
    {
        return QuestName == other. QuestName &&
               Steps.SequenceEqual(other.Steps);
    }
}
```

**学習ポイント：**
- 位置指定レコード (Positional Record)
- `init` による初期化後の不変性
- `with` 式による非破壊的更新
- コレクションを含むレコードの等価判定のカスタマイズ

### 5. LINQ と匿名型 (`InventoryAnalyzer.cs`)

```csharp
var report = items
    .Where(i => i.Weight > 0)
    .Select(i => new 
    {
        i.Name,
        IsHeavy = i.Weight > 5. 0,
        TypeCategory = i.GetType().Name
    })
    .OrderByDescending(x => x.IsHeavy);
```

**学習ポイント：**
- メソッドチェーンによるデータ変換
- 匿名型による一時的なデータ構造
- プロパティ名の推論
- 匿名型での `with` 式（C# 10+）

## 🚀 実行方法

### 前提条件
- . NET 8.0 以上

### ビルドと実行

```bash
# プロジェクトディレクトリに移動
cd FantasyWorld

# ビルド
dotnet build

# 実行
dotnet run
```

### 期待される出力例

```
=== Fantasy RPG System Initialized ===
箱に15のダメージ！ 残りHP: 85
Healing Potionを収納しました。
Steel Swordを収納しました。

--- インベントリ分析レポート ---
[軽量] Healing Potion (Consumable)
[軽量] Steel Sword (Weapon)

--- アイテム使用テスト ---
Healing Potionを使用しました。効果値: 50
```

## 📖 学習の進め方

### ステップ1: コードを読む
各ファイルのコメントを読みながら、C#の機能がどのように使われているかを理解します。

### ステップ2: 実行して動作を確認
`FantasyRPG.cs` の `GameRunner. Run()` を実行し、各機能の動作を確認します。

### ステップ3: カスタマイズして実験
以下のような拡張にチャレンジしてみましょう：

1. **新しいアイテムクラスを作成**
   - 例：`Armor`（防具）、`MagicScroll`（魔法の巻物）

2. **インターフェースを実装した新クラス**
   - 例：`Enemy` クラスに `IDamageable` を実装

3. **ジェネリクスの制約を変更**
   - 例：`Inventory<T>` を `where T :  Consumable` に限定

4. **レコードで新しいデータモデル**
   - 例：`CharacterStatus` レコード

5. **LINQ クエリの拡張**
   - 例：重量の合計、カテゴリ別グループ化

## 🎯 型システムの重要概念マップ

| 概念 | 使用場所 | 目的 |
|------|---------|------|
| 抽象クラス | `Item` | 共通の振る舞いと強制実装の組み合わせ |
| インターフェース | `IDamageable` | 契約の定義、多重継承的な機能付与 |
| ジェネリクス | `Inventory<T>` | 型安全な汎用コンテナ |
| レコード | `QuestStep`, `QuestLog` | 不変データモデル、値ベースの等価性 |
| 匿名型 | `InventoryAnalyzer` | 一時的なデータ変換 |

## 💡 重要な落とし穴と対処法

### ⚠️ レコードとコレクション
```csharp
// ❌ 危険：デフォルトの等価判定は参照比較
public record QuestLog(string Name, List<QuestStep> Steps);

// ✅ 安全：SequenceEqual で中身を比較
public override bool Equals(QuestLog?  other) =>
    Steps.SequenceEqual(other?. Steps ??  []);
```

### ⚠️ ジェネリック制約
```csharp
// ❌ コンパイルエラー：制約違反
var inventory = new Inventory<string>(10); 

// ✅ OK：Item を継承している
var inventory = new Inventory<Consumable>(10);
```

### ⚠️ インターフェースの明示的実装
```csharp
var shield = new CursedShield();
// shield.TakeDamage(10); // ❌ コンパイルエラー

IDamageable damageable = shield;
damageable.TakeDamage(10); // ✅ OK
```

## 🔗 参考リソース

- [C# 言語リファレンス - Microsoft Learn](https://learn.microsoft.com/ja-jp/dotnet/csharp/)
- [C# のパターン マッチング](https://learn.microsoft.com/ja-jp/dotnet/csharp/fundamentals/functional/pattern-matching)
- [レコード型 (C# リファレンス)](https://learn.microsoft.com/ja-jp/dotnet/csharp/language-reference/builtin-types/record)

## 📝 ライセンス

このプロジェクトは教育目的で作成されています。自由に改変・再配布できます
