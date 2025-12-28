// C#では名前空間を使用してクラスを整理する（C++のnamespaceと同様）
using System;
using System. Diagnostics;

namespace Day1_MemoryMonitor
{
    // 値型 (Struct): スタックまたは配列内にインラインで確保される
    // C/C++のstructと似ているが、C#では値型として扱われる（コピーセマンティクス）
    public struct PointStruct
    {
        public int X;
        public int Y;
    }

    // 参照型 (Class): ヒープに確保され、参照経由でアクセスされる
    // C++のポインタやC++11以降のスマートポインタに似ているが、
    // C#では自動的にガベージコレクション（GC）で管理される（deleteは不要）
    public class PointClass
    {
        public int X;
        public int Y;
    }

    class Program
    {
        const int IterationCount = 1_000_000; // 100万回（アンダースコアは数値リテラルの区切り文字で、読みやすさのため）

        // エントリポイント（C/C++のmain関数に相当）
        // C#では必ずMainメソッドがエントリポイントになる
        static void Main(string[] args)
        {
            Console.WriteLine("=== Day 1: C# Memory Management & Type System Deep Dive ===\n");

            // 値型（struct）と参照型（class）のパフォーマンス比較
            RunStructVsClassBenchmark();
            
            // ボクシングのオーバーヘッドをジェネリクスと比較
            RunBoxingBenchmark();
            
            // ガベージコレクションの世代管理を観察
            RunGCObservation();

            Console.WriteLine("\n=== 実験終了 ===");
        }

        // 値型（struct）と参照型（class）のベンチマーク
        static void RunStructVsClassBenchmark()
        {
            Console.WriteLine("1. Struct vs Class (Array Allocation Test)");

            // GC. Collect(): 強制的にガベージコレクションを実行（C++には無い概念）
            // 正確なメモリ測定のため、既存のガベージを回収
            GC.Collect();
            GC.WaitForPendingFinalizers(); // ファイナライザ（デストラクタ相当）の実行を待つ
            long memoryBefore = GC. GetTotalMemory(true); // 現在のマネージドヒープのメモリ使用量を取得

            // --- Structのテスト ---
            // Stopwatch:  高精度な時間計測クラス（C++のstd::chrono相当）
            var sw = Stopwatch.StartNew();

            // 配列型として宣言（C/C++と同様の配列構文）
            // structの配列は全要素が連続したメモリブロックに確保される（C/C++と同様）
            PointStruct[] structArray = new PointStruct[IterationCount];

            for (int i = 0; i < IterationCount; i++)
            {
                // structは値型なので、配列確保時に全要素分の領域が確保済み
                // 個別のnewは不要（C/C++のスタック配列と同様の動作）
                structArray[i]. X = i;
                structArray[i].Y = i;
            }

            sw.Stop();
            long memoryAfterStruct = GC.GetTotalMemory(false);
            Console.WriteLine($"[Struct] Time: {sw.ElapsedMilliseconds}ms, Memory Alloc: {(memoryAfterStruct - memoryBefore) / 1024 / 1024} MB");

            // Array. Empty<T>(): 空の配列を返す（メモリ効率的な方法）
            // 元の配列への参照を切ることで、GCの回収対象にする
            structArray = Array.Empty<PointStruct>();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            memoryBefore = GC.GetTotalMemory(true);

            // --- Classのテスト ---
            sw.Restart();

            // classの配列は「参照の配列」（C++のポインタ配列に相当）
            PointClass[] classArray = new PointClass[IterationCount];

            for (int i = 0; i < IterationCount; i++)
            {
                // classは参照型なので、個別にnewして実体をヒープに作る必要がある
                // C++で言うと:  classArray[i] = new PointClass(); に相当
                // { X = i, Y = i } はオブジェクト初期化子（C++11の初期化リストに似ている）
                classArray[i] = new PointClass { X = i, Y = i };
            }

            sw.Stop();
            long memoryAfterClass = GC.GetTotalMemory(false);
            Console.WriteLine($"[Class ] Time: {sw.ElapsedMilliseconds}ms, Memory Alloc: {(memoryAfterClass - memoryBefore) / 1024 / 1024} MB");

            Console. WriteLine("--------------------------------------------------");
        }

        // ボクシング（値型→参照型への暗黙的変換）のコストを測定
        static void RunBoxingBenchmark()
        {
            Console.WriteLine("2. Boxing vs Generics (Performance Cost)");

            int value = 123;
            var sw = Stopwatch.StartNew();

            for (int i = 0; i < IterationCount; i++)
            {
                // ボクシング:  値型（int）を参照型（object）に変換
                // ヒープにメモリを確保してコピーするため、パフォーマンスコストが高い
                ProcessAsObject(value);
            }
            sw.Stop();
            Console.WriteLine($"[Object ] Time: {sw.ElapsedMilliseconds}ms (Boxing: int -> object)");

            sw.Restart();

            for (int i = 0; i < IterationCount; i++)
            {
                // ジェネリクス: 型パラメータTを使用（C++のテンプレートに相当）
                // ボクシングが発生せず、効率的
                ProcessAsGeneric(value);
            }
            sw.Stop();
            Console.WriteLine($"[Generic] Time: {sw. ElapsedMilliseconds}ms (No Boxing: int -> T)");

            Console.WriteLine("--------------------------------------------------");
        }

        // object型を受け取る関数（値型を渡すとボクシングが発生）
        static void ProcessAsObject(object o) { }

        // ジェネリック関数（C++のテンプレート関数に相当）
        // <T>は型パラメータで、呼び出し時に型が決定される
        static void ProcessAsGeneric<T>(T t) { }

        // ガベージコレクション（GC）の世代管理を観察
        // C#のGCは世代別GCを採用（Gen0, Gen1, Gen2の3世代）
        static void RunGCObservation()
        {
            Console.WriteLine("3. GC Generation Observation");

            // 新しくオブジェクトを生成（最初はGen0に配置される）
            PointClass p = new PointClass();

            // オブジェクトが現在どの世代に属しているかを取得
            int gen = GC.GetGeneration(p);
            Console.WriteLine($"Initial Generation: {gen}");

            // Gen0のガベージコレクションを実行
            // 生き残ったオブジェクトはGen1に昇格する
            GC.Collect(0);

            gen = GC.GetGeneration(p);
            Console.WriteLine($"After GC Collect(0): Generation {gen} (Promoted)");

            // 全世代のガベージコレクションを実行
            // 生き残ったオブジェクトはさらに昇格してGen2になる
            GC.Collect();

            gen = GC. GetGeneration(p);
            Console.WriteLine($"After Full GC      : Generation {gen} (Promoted again)");
        }
    }
}
