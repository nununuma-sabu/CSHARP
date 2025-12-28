# Day 2: リソース管理とオブジェクト指向機能

## 📋 概要

Day 2のテーマは**「C++のRAIIパターンをC#でどう実現するか」**です。

C++ではデストラクタ (`~Class`) でリソース解放を行いますが、C#のデストラクタ（ファイナライザ）はGC任せでいつ呼ばれるか分かりません。ファイルハンドルやデータベース接続、アンマネージドメモリなどの「非メモリリソース」を扱う場合、C#では**Disposeパターン**を使用します。

---

## 🎯 学習のポイント

### 1. **プロパティ (Property)**
- get/set メソッドをカプセル化し、フィールドのように見せる機能
- C++のゲッター/セッターメソッドをよりシンプルに記述可能

### 2. **インデクサ (Indexer)**
- `operator[]` のオーバーロードに相当
- オブジェクトを配列のように扱える:  `obj[index]`

### 3. **IDisposableとusing文**
- C++の `std::lock_guard` やスマートポインタのように、スコープを抜けた瞬間に確実なクリーンアップを行う仕組み
- `using` ブロックを抜けると自動的に `Dispose()` が呼ばれる

---

## 🛠️ 実装課題:  アンマネージドメモリ・ラッパー

C#からC言語の `malloc`/`free` に相当する機能 (`Marshal` クラス) を使い、GCの管理外にあるメモリを扱うクラスを作成します。

### 主要な実装内容

#### **UnmanagedArrayクラス**
- アンマネージドメモリを確保・管理するクラス
- `IDisposable` インターフェースを実装
- インデクサを使った配列風アクセスをサポート

```csharp
public class UnmanagedArray : IDisposable
{
    private IntPtr _memoryPtr;    // C++の void* に相当
    private int _length;
    private bool _disposed = false;
    
    public int Length { get; private set; }  // プロパティ
    public byte this[int index] { get; set; } // インデクサ
    
    // ...  Disposeパターンの実装 ...
}
```

---

## 🔍 Disposeパターンの実装

### 3つの重要な要素

#### 1. **Dispose()メソッド**
ユーザーが明示的にリソースを解放するためのメソッド

```csharp
public void Dispose()
{
    Dispose(true);
    GC.SuppressFinalize(this); // ファイナライザ呼び出しを抑制
}
```

#### 2. **Dispose(bool)メソッド**
実際の解放ロジックを実装

```csharp
protected virtual void Dispose(bool disposing)
{
    if (_disposed) return;
    
    if (disposing)
    {
        // マネージドリソースの解放
    }
    
    // アンマネージドリソースの解放
    if (_memoryPtr != IntPtr.Zero)
    {
        Marshal. FreeHGlobal(_memoryPtr); // free()相当
        _memoryPtr = IntPtr.Zero;
    }
    
    _disposed = true;
}
```

#### 3. **ファイナライザ (~クラス名)**
`Dispose()` を呼び忘れた場合の安全装置

```csharp
~UnmanagedArray()
{
    Dispose(false);
    Console.WriteLine("[GC] Finalizer called.  (You forgot to Dispose!)");
}
```

---

## 🚀 実行方法

```bash
cd Day2
dotnet run
```

### 期待される出力

```
=== Day 2: Resource Management & IDisposable ===

--- Test A: using block ---
[Alloc] Memory allocated:  5 bytes at 0x... 
Index 0: 10
Index 1: 20
[Free ] Memory freed at 0x... 
Outside using block.

--- Test B: Forgetting Dispose (Triggering GC) ---
[Alloc] Memory allocated: 10 bytes at 0x... 
Forcing GC to collect the leaked object...
[Free ] Memory freed at 0x... 
[GC   ] Finalizer called. (You forgot to Dispose!)
GC done.
```

---

## 📖 コードの詳細解説

### **パターンA: using文 (推奨)**
```csharp
using (var array = new UnmanagedArray(5))
{
    array[0] = 10;
    array[1] = 20;
    Console.WriteLine($"Index 0: {array[0]}");
} // ここで自動的にDispose()が呼ばれる
```

- `using` ブロックを抜けると自動的に `Dispose()` が呼ばれる
- C++のスタック巻き戻しによるデストラクタ呼び出しに近い動作

### **パターンB: Dispose呼び忘れ (非推奨)**
```csharp
void CreateGarbage()
{
    var leaked = new UnmanagedArray(10);
    leaked[0] = 99;
    // Dispose()を呼ばずに終了 → メモリリーク
}

GC.Collect(); // GCを強制実行してファイナライザを確認
```

- `Dispose()` を呼ばないと、GCが回収するまでメモリが解放されない
- ファイナライザが呼ばれるが、タイミングは不定

---

## 実行結果
```bash
=== Day 2: Resource Management & IDisposable ===

--- Test A: using block ---
[Alloc] Memory allocated:  5 bytes at 0x291927AE490
Index 0: 10
Index 1: 20
[Free ] Memory freed at 0x291927AE490
Outside using block.

--- Test B: Forgetting Dispose (Triggering GC) ---
[Alloc] Memory allocated:  10 bytes at 0x291AA431780
Forcing GC to collect the leaked object...
[Free ] Memory freed at 0x291AA431780
[GC   ] Finalizer called.  (You forgot to Dispose!)
GC done.
```


## 🔑 重要なポイント

### C++との比較

| C++ | C# |
|-----|-----|
| デストラクタ `~Class()` | ファイナライザ `~Class()` (GC依存) |
| スタック上のオブジェクトは自動解放 | `using` 文で明示的にスコープ管理 |
| `std::unique_ptr`, `std::shared_ptr` | `IDisposable` + `using` |
| RAII (Resource Acquisition Is Initialization) | Dispose パターン |

### ベストプラクティス

1. ✅ **アンマネージドリソースを使う場合は必ず `IDisposable` を実装**
2. ✅ **使用側は `using` 文を使う**
3. ✅ **ファイナライザはあくまで安全装置（呼ばれないのが理想）**
4. ⚠️ **ファイナライザ内でマネージドオブジェクトにアクセスしない**

---

## 📚 関連リソース

- [IDisposable インターフェイス (Microsoft Docs)](https://docs.microsoft.com/ja-jp/dotnet/api/system.idisposable)
- [Dispose パターン](https://docs.microsoft.com/ja-jp/dotnet/standard/design-guidelines/dispose-pattern)
- [Marshal クラス](https://docs.microsoft.com/ja-jp/dotnet/api/system.runtime.interopservices.marshal)

---

## 🎓 学習の成果

このDay 2を通じて、以下を習得できます: 

- ✨ C#でのリソース管理の正しい方法
- ✨ プロパティとインデクサの使い方
- ✨ Disposeパターンの実装方法
- ✨ C++のRAIIとC#のusing文の対応関係
- ✨ アンマネージドメモリの安全な扱い方
