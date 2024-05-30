using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.entities.traits;
// TODO: Make them all inherit from Trait or ITrait.
internal class AvoidCliffsTrait : Trait {
    public bool IsActive { get; private set; }

    private Enemy _owner;
    private int _offsetDown;
    private ivec2 _checkedCliffTile = new(-2, 0);

    /// <summary>
    /// A rectangle that represents the tile that is currently being checked
    /// as a potential cliff.
    /// </summary>
    private RectangleShape _dbg_targetTile;

    public AvoidCliffsTrait (Enemy owner, bool isActive, float mobHeight) {
        _owner = owner;
        IsActive = isActive;
        _offsetDown = (int)MathF.Ceiling(mobHeight / 16f);

        _dbg_targetTile = new() {
            FillColor = new(255, 0, 0, 128),
            Size = new(16f, 16f),
        };
    }

    public override void FixedUpdate () {
        if (IsActive == false) return;

        var gridPos = _owner.GridPosition;

        if (_owner.Velocity.X < 0f) {
            _checkedCliffTile = new(gridPos.X, gridPos.Y + _offsetDown);
        }
        else if (_owner.Velocity.X > 0f) {
            _checkedCliffTile = new(gridPos.X + 1, gridPos.Y + _offsetDown);
        }
    }

    public override void DrawDebugInfo (RenderWindow window) {
        if (IsActive == false) return;

        _dbg_targetTile.Position = (vec2)(_checkedCliffTile * 16);
        window.Draw(_dbg_targetTile);
    }
}
