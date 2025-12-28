using System;
using System.Runtime.InteropServices; // Marshalクラスを使用するために必要

namespace Day2_ResourceManagement
{
    // C++のスマートポインタや独自コンテナのようなクラスを作ります
    // IDisposableインターフェースを実装することで、using文での自動解放に対応します
    public class UnmanagedArray : IDisposable
    {
        // ポインタ保持用（C++の void* / byte* に相当）
        private IntPtr _memoryPtr;
        private int _length;
        private bool _disposed = false;

        // プロパティ: 配列の長さを取得 (C++の getLength() 相当)
        // 外部からは読み取り専用、内部でのみセット可能
        public int Length { get; private set; }

        // コンストラクタ
        public UnmanagedArray(int length)
        {
            if (length <= 0) throw new ArgumentException("Length must be positive");

            Length = length;
            // Marshal.AllocHGlobal は C言語の malloc と同じ働き (GC管理外のヒープを確保)
            _memoryPtr = Marshal.AllocHGlobal(length * sizeof(byte));
            
            // メモリをゼロクリア (memset相当)
            // Span<T>を使うとより安全に書けますが、ここでは基本的なポインタ操作に近いAPIを使います
            for (int i = 0; i < length; i++)
            {
                Marshal.WriteByte(_memoryPtr, i, 0);
            }
            
            Console.WriteLine($"[Alloc] Memory allocated: {length} bytes at 0x{_memoryPtr:X}");
        }

        // インデクサ: オブジェクトを配列のように扱えるようにする (C++の operator)
        // obj[i] = value; のように書けるようになる
        public byte this[int index]
        {
            get
            {
                CheckBounds(index);
                // 指定オフセットのバイトを読み取る
                return Marshal.ReadByte(_memoryPtr, index);
            }
            set
            {
                CheckBounds(index);
                // 指定オフセットにバイトを書き込む
                Marshal.WriteByte(_memoryPtr, index, value);
            }
        }

        private void CheckBounds(int index)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(UnmanagedArray));
            if (index < 0 || index >= Length) throw new IndexOutOfRangeException();
        }

        // --- Dispose Pattern の実装 (重要) ---

        // 1. ユーザーが明示的にリソースを解放するためのメソッド
        public void Dispose()
        {
            Dispose(true);
            // GCに対して「もうファイナライザを呼ぶ必要はない」と通知する（パフォーマンス向上）
            GC.SuppressFinalize(this);
        }

        // 2. 実際の解放ロジック
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // マネージドリソース（他のC#クラスなど）の解放はここで行う
            }

            // アンマネージドリソース（IntPtrなど）の解放はここで行う
            if (_memoryPtr!= IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_memoryPtr); // free()
                Console.WriteLine($"[Free ] Memory freed at 0x{_memoryPtr:X}");
                _memoryPtr = IntPtr.Zero;
            }

            _disposed = true;
        }

        // 3. ファイナライザ (C++のデストラクタ ~UnmanagedArray)
        // ユーザーが Dispose() を呼び忘れた場合の「安全装置」としてGCから呼ばれる
        ~UnmanagedArray()
        {
            // falseを指定: GCスレッドから呼ばれているため、他のマネージドオブジェクトには触ってはいけない
            Dispose(false);
            Console.WriteLine("[GC   ] Finalizer called. (You forgot to Dispose!)");
        }
    }

    class Program
    {
        static void Main(string args)
        {
            Console.WriteLine("=== Day 2: Resource Management & IDisposable ===\n");

            // パターンA: usingステートメント (推奨)
            // ブロックを抜けると自動的に Dispose() が呼ばれる (C++のスタック巻き戻しによるデストラクタ呼び出しに近い)
            Console.WriteLine("--- Test A: using block ---");
            using (var array = new UnmanagedArray(5))
            {
                array = 10;
                array[1] = 20;
                Console.WriteLine($"Index 0: {array}");
                Console.WriteLine($"Index 4: {array[1]}");
                // array[2] = 30; // IndexOutOfRangeException
            } // ここで "Memory freed" が表示されるはず
            Console.WriteLine("Outside using block.\n");


            // パターンB: Dispose呼び忘れ (非推奨だが挙動確認用)
            Console.WriteLine("--- Test B: Forgetting Dispose (Triggering GC) ---");
            CreateGarbage();
            
            Console.WriteLine("Forcing GC to collect the leaked object...");
            GC.Collect();
            GC.WaitForPendingFinalizers(); // ファイナライザが終わるのを待つ
            Console.WriteLine("GC done.\n");
        }

        static void CreateGarbage()
        {
            var leaked = new UnmanagedArray(10);
            leaked = 99;
            // Dispose() を呼ばずにメソッドを抜ける -> メモリリーク（GC回収まで解放されない）
        }
    }
}
