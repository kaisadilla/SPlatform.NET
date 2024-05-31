namespace splatform.physics;

public enum GameObjectType {
    Player = 0,
    Tile = 1,
    Enemy = 2,
    Item = 3,
}

internal interface IGameObject {
    public GameObjectType Type { get; }
}
