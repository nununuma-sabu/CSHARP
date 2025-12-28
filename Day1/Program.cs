using System;
using System.Diagnostics;

namespace Day1_MemoryMonitor
{
    // 値型 (Struct): スタックまたは配列内にインラインで確保される
    public struct PointStruct
    {
        public int X;
        public int Y;
    }

    // 参照型 (Class): ヒープに確保され、参照経由でアクセスされる
    public class PointClass
    {
        public int X;
        public int Y;
    }

    class Program
    {
        const int IterationCount = 1_000_000; // 100万回

        // エントリポイントは string[] args
        static void Main(string[] args)
        {
            Console.WriteLine("=== Day 1: C# Memory Management & Type System Deep Dive ===\n");

            RunStructVsClassBenchmark();
            RunBoxingBenchmark();
            RunGCObservation();

            Console.WriteLine("\n=== 実験終了 ===");
        }

        static void RunStructVsClassBenchmark()
        {
            Console.WriteLine("1. Struct vs Class (Array Allocation Test)");

            GC.Collect();
            GC.WaitForPendingFinalizers();
            long memoryBefore = GC.GetTotalMemory(true);

            // --- Structのテスト ---
            var sw = Stopwatch.StartNew();

            // 配列型として宣言する
            PointStruct[] structArray = new PointStruct[IterationCount];

            for (int i = 0; i < IterationCount; i++)
            {
                // structは配列確保時に要素分の領域が確保済みなので、個別のnewは不要
                structArray[i].X = i;
                structArray[i].Y = i;
            }

            sw.Stop();
            long memoryAfterStruct = GC.GetTotalMemory(false);
            Console.WriteLine($"[Struct] Time: {sw.ElapsedMilliseconds}ms, Memory Alloc: {(memoryAfterStruct - memoryBefore) / 1024 / 1024} MB");

            // structArrayを破棄対象にする
            structArray = Array.Empty<PointStruct>();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            memoryBefore = GC.GetTotalMemory(true);

            // --- Classのテスト ---
            sw.Restart();

            // こちらも配列型として宣言する
            PointClass[] classArray = new PointClass[IterationCount];

            for (int i = 0; i < IterationCount; i++)
            {
                // classは参照の配列なので、個別にnewして実体をヒープに作る必要がある
                classArray[i] = new PointClass { X = i, Y = i };
            }

            sw.Stop();
            long memoryAfterClass = GC.GetTotalMemory(false);
            Console.WriteLine($"[Class ] Time: {sw.ElapsedMilliseconds}ms, Memory Alloc: {(memoryAfterClass - memoryBefore) / 1024 / 1024} MB");

            Console.WriteLine("--------------------------------------------------");
        }

        static void RunBoxingBenchmark()
        {
            Console.WriteLine("2. Boxing vs Generics (Performance Cost)");

            int value = 123;
            var sw = Stopwatch.StartNew();

            for (int i = 0; i < IterationCount; i++)
            {
                ProcessAsObject(value); // boxing
            }
            sw.Stop();
            Console.WriteLine($"[Object ] Time: {sw.ElapsedMilliseconds}ms (Boxing: int -> object)");

            sw.Restart();

            for (int i = 0; i < IterationCount; i++)
            {
                ProcessAsGeneric(value); // no boxing
            }
            sw.Stop();
            Console.WriteLine($"[Generic] Time: {sw.ElapsedMilliseconds}ms (No Boxing: int -> T)");

            Console.WriteLine("--------------------------------------------------");
        }

        static void ProcessAsObject(object o) { }

        static void ProcessAsGeneric<T>(T t) { }

        static void RunGCObservation()
        {
            Console.WriteLine("3. GC Generation Observation");

            PointClass p = new PointClass();

            int gen = GC.GetGeneration(p);
            Console.WriteLine($"Initial Generation: {gen}");

            GC.Collect(0);

            gen = GC.GetGeneration(p);
            Console.WriteLine($"After GC Collect(0): Generation {gen} (Promoted)");

            GC.Collect();

            gen = GC.GetGeneration(p);
            Console.WriteLine($"After Full GC      : Generation {gen} (Promoted again)");
        }
    }
}
