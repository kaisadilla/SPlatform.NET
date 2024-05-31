namespace splatform.assets.data;
// TODO: this is for JSON files only, maybe has a purpose in testing stuff.
internal class LevelData {
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required string Background { get; init; }
    public required string Music { get; init; }
    public required EnemyData[] Enemies { get; init; }
    public required GridData Grids { get; init; }
}

internal class EnemyData {
    public required int Id { get; init; }
    public required string Behavior { get; init; }
    public required Dictionary<string, object> BehaviorProperties { get; init; }
    public required string Sprite { get; init; }
    public required Dictionary<string, int[]> Dimensions { get; init; }
    public required int[] Position { get; init; }
}

internal class GridData {
    public required Dictionary<string, string> Background { get; init; }
    public required Dictionary<string, string> Foreground { get; init; }
}