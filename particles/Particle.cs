using SFML.Graphics;
using splatform.animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.particles;
internal abstract class Particle {
    protected vec2 _position = new(0, 0);

    protected Sprite _sprite;
    /// <summary>
    /// The sprite's animation. Not related to the animations by this particle,
    /// such as its position.
    /// </summary>
    protected Animation _spriteAnim;
    protected float _animSpeed = 1f;

    public bool DisposePending { get; private set; } = false;
    public vec2 CurrentPosition => _position;

    #region Setters
    public void SetPosition (vec2 position) {
        _position = position;
        _sprite.Position = position;
    }
    #endregion

    public void Start () {
        _sprite.TextureRect = _spriteAnim.GetCurrentSlice(false);
        OnStart();
    }

    public void Update () {
        _spriteAnim.Update(Time.DeltaTime, _animSpeed);
        _sprite.TextureRect = _spriteAnim.GetCurrentSlice(false);
        OnUpdate();
    }

    public void Draw (RenderWindow window) {
        window.Draw(_sprite);
    }

    public void Destroy() {
        OnDestroy();
        DisposePending = true;
    }

    protected virtual void OnStart () { }
    protected virtual void OnUpdate () { }
    protected virtual void OnDestroy () { }
}
