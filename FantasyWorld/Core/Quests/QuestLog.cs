namespace FantasyWorld.Core.Quests;

// ---------------------------------------------------------
// 複雑なレコード: QuestLog
// コレクションを含むため、等価判定をカスタマイズする必要がある
// ---------------------------------------------------------
public record QuestLog
{
    public string QuestName { get; init; }
    
    // 不変性を保つため、IReadOnlyListとして公開
    // 読み取り専用のプロパティ
    public IReadOnlyList<QuestStep> Steps { get; init; }

    // コンストラクタ
    public QuestLog(string name, IEnumerable<QuestStep> steps)
    {
        QuestName = name;
        // リストをコピーして外部からの変更を防ぐ
        Steps = steps.ToList().AsReadOnly();
    }

    // -----------------------------------------------------
    // 落とし穴の回避: Equalsのオーバーライド
    // デフォルトの挙動ではリストの参照比較になってしまうため、
    // SequenceEqualを使って中身の比較を行うように修正する。
    // -----------------------------------------------------
    public virtual bool Equals(QuestLog? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        // 名前の一致 && リストの中身の完全一致
        return QuestName == other.QuestName &&
               Steps.SequenceEqual(other.Steps);
    }

    // EqualsをオーバーライドしたらGetHashCodeも必ずオーバーライドする
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(QuestName);
        foreach (var step in Steps)
        {
            hash.Add(step);
        }
        return hash.ToHashCode();
    }
}