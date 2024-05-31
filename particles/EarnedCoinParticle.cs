using Betwixt;
using splatform.animation;
using splatform.assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.particles;
internal class EarnedCoinParticle : Particle {
    const string PARTICLE_NAME = "volatile_coin";

    private vec2 _initialPosition;
    private TweenAnimation<float> _anim_height;

    public EarnedCoinParticle (vec2 initialPos) {
        _sprite = new(Assets.ParticleTextures[PARTICLE_NAME]);
        _spriteAnim = new DynamicAnimation(
            new(4, 1),
            new(16f, 16f),
            0.1f,
            [0, 1, 2, 3]
        ); // TODO: Do not hardcode.

        _anim_height = new(
            [
                new(0, -64, 0.4, Ease.Sine.Out),
                new(-64, -18, 0.25, Ease.Sine.In)
            ],
            false,
            Destroy
        );

        _position = initialPos;
        _initialPosition = initialPos;
    }

    protected override void OnStart () {
        _anim_height.Play();
    }

    protected override void OnUpdate () {
        _anim_height.Update(
            v => SetPosition(_initialPosition + new vec2(0, v))
        );
    }
}
