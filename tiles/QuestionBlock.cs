using Betwixt;
using SFML.Audio;
using splatform.animation;
using splatform.assets;
using splatform.entities;
using splatform.particles;
using splatform.physics;

namespace splatform.tiles;
internal class QuestionBlock : Tile {
    #region Constants
    public enum ContentType {
        Coin = 0,
        Entity = 1,
        Tile = 2,
    }

    public enum HitMode {
        Once = 0,
        Times = 1, // n times
        Seconds = 2, // n seconds
    }

    public enum State {
        Active = 0,
        Empty = 1,
    }

    public enum AnimState {
        ACTIVE = 0,
        EMPTY = 1,
    }
    #endregion

    #region Parameters
    public required bool IsHidden { get; init; }
    public required ContentType ContentTypeV { get; init; }
    public required HitMode HitModeV { get; init; }

    public required Entity? ContainedEntity { get; init; }
    public required Tile? ContainedTile { get; init; }
    public required int MaxHitCount { get; init; } = 1;
    public required bool RevertToCoin { get; init; } = false;
    public required float HitTimer { get; init; } = 5f;
    public required bool PersistsUntilHit { get; init; } = false;
    #endregion

    public State CurrentState { get; private set; } = State.Active;
    public int CurrentHits { get; private set; } = 0;

    private Sound _sound_coin;
    private Sound _sound_item;

    /// <summary>
    /// The vertical movement of the draw position when hit by the player.
    /// </summary>
    private TweenAnimation<float> _anim_hitByPlayer;

    public QuestionBlock () {
        _sound_coin = new(Assets.Sounds!.Coin);
        _sound_item = new(Assets.Sounds!.ItemSpawn);

        _anim_hitByPlayer = new(
            [new(0f, -10f, 0.1f, Ease.Sine.Out), new(-10f, 0f, 0.05f, Ease.Linear)],
            false
        );
    }

    public override void OnUpdate () {
        base.OnUpdate();

        _anim_hitByPlayer.Update(
            yOffset => SetDrawPosition(Position + new vec2(0, (int)yOffset))
        );
    }

    public override void OnCollisionWithPlayer (
        Collision collision, Player player
    ) {
        if (collision.Direction != Direction.Up) return;
        if (CurrentState != State.Active) return;

        CurrentHits++;

        _anim_hitByPlayer.Play();

        if (ContentTypeV == ContentType.Coin) {
            _anim_hitByPlayer.SetCallback(null);
            player.EarnCoin();
            _sound_coin.Play();

            EarnedCoinParticle coin = new(Position + new vec2(0, -16));
            _level!.InstantiateParticle(coin);
        }
        else if (ContentTypeV == ContentType.Entity) {
            if (ContainedEntity != null) {
                _anim_hitByPlayer.SetCallback(() => {
                    ContainedEntity.SetPosition(Position + new vec2(0, 0));
                    ContainedEntity.SetStartingDirectionRight(
                        collision.Intersection.X > PIXELS_PER_TILE / 2f
                    );
                    ContainedEntity.PlayAnim_GetFromBlock();

                    _level!.InstantiateEntity(ContainedEntity); // TODO
                    _sound_item.Play();
                });
            }
        }

        if (CurrentHits >= MaxHitCount) {
            CurrentState = State.Empty;
            _animations.SetState((int)AnimState.EMPTY);
        }
    }
}
