namespace FantasyWorld.Core.Quests;

// ---------------------------------------------------------
// シンプルなレコード: QuestStep
// 位置指定レコード（Positional Record）構文
// ---------------------------------------------------------
public record QuestStep(int StepNumber, string Description, bool IsCompleted);