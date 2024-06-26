﻿using SFML.Graphics;
using SFML.Window;
using splatform.entities;
using splatform.particles;
using splatform.player;
using splatform.tiles;

namespace splatform.game.scenes;
internal partial class LevelScene : Scene {
    public int Width { get; private set; }
    public int Height { get; private set; }
    public float TimeLeft { get; private set; }

    private Background _background;

    private List<Tile> _backgroundLayer = new();
    private List<Tile> _foregroundLayer = new();
    private List<Tile> _detailLayer = new();
    private List<Tile> _frontLayer = new();

    // TODO: split this into two lists: one with enemies that haven't been
    // spawned yet, and another with active entities. Enemies spawn when
    // mario gets close enough to their initial location (in the x coordinate)
    private List<Entity> _entities = new();
    private List<Particle> _particles = new();

    private Player _player = new();
    private Camera _camera = new(new(1, 1), new(1, 1));
    private vec2 __temp_playerPos = new(0, 400);

    public int GridWidth => Width;
    public int GridHeight => Height;
    public int PixelWidth => Width * PIXELS_PER_TILE;
    public int PixelHeight => Height * PIXELS_PER_TILE;

    /// <summary>
    /// The size, in pixels, of the view. This is the amount of in-game pixels
    /// that can be seen in the window, not the amount of screen pixels. For
    /// example, if zoom is equal to 2, then each in-game pixel takes 4 screen
    /// pixels. In practice, this is equal to WindowSize / WindowZoom.
    /// </summary>
    public ivec2 ViewSize => new(
        (int)(WindowSize.X / WindowZoom),
        (int)(WindowSize.Y / WindowZoom)
    );

    public LevelScene () {

    }

    public override void Init (Game game, RenderWindow window) {
        _game = game;

        __TEMP_initialize_player();

        _background.SetWindowContext(WindowSize, WindowZoom);
        _camera = new(new(PixelWidth, PixelHeight), ViewSize);
    }

    public override void Start () {

        foreach (var tile in _backgroundLayer) {
            tile.OnStart();
        }
        foreach (var tile in _foregroundLayer) {
            tile.OnStart();
        }
        foreach (var tile in _detailLayer) {
            tile.OnStart();
        }
        foreach (var tile in _frontLayer) {
            tile.OnStart();
        }
        foreach (var entity in _entities) {
            entity.Start();
        }

        _background.PlayMusic();
    }

    public override void Update () {
        TimeLeft -= Time.DeltaTime;

        foreach (var tile in _backgroundLayer) {
            tile.OnUpdate();
        }
        foreach (var tile in _foregroundLayer) {
            tile.OnUpdate();
        }
        foreach (var tile in _detailLayer) {
            tile.OnUpdate();
        }
        foreach (var tile in _frontLayer) {
            tile.OnUpdate();
        }
        foreach (var entity in _entities) {
            entity.Update();
        }
        foreach (var particle in _particles) {
            particle.Update();
        }

        _player.Update();
        _camera.UpdatePosition(_player.PixelPosition);
        _background.Update(_camera.TopLeft);

        DeleteDisposedObjects();
    }

    public override void FixedUpdate () {
        foreach (var entity in _entities) {
            entity.FixedUpdate();
        }
        for (int i = 0; i < _entities.Count; i++) {
            _entities[i].CheckCollisionWithTiles(_foregroundLayer);
            _entities[i].CheckCollisionWithEntities(_entities, i + 1);
        }

        _player.FixedUpdate();
        _player.CheckCollisionWithTiles(_foregroundLayer);
        _player.CheckCollisionWithEntities(_entities);
    }

    public override void LateUpdate () {
        foreach (var entity in _entities) {
            entity.LateUpdate();
        }
    }

    public override void DrawToWindow (RenderWindow window) {
        _background.Draw(window);

        window.SetView(_camera.View);
        DrawLayer(window, _backgroundLayer);
        DrawEntities(window, true);
        DrawLayer(window, _foregroundLayer);
        DrawEntities(window, false);
        DrawLayer(window, _detailLayer);
        DrawLayer(window, _frontLayer);
        DrawPlayer(window);
        DrawParticles(window);

        if (Debug.DrawColliders) {
            DrawColliders(window);
        }
        if (Debug.DrawVisualInfo) {
            DrawDebugInfo(window);
        }

        window.SetView(window.DefaultView);
    }

    public override void ProcessKeyboardEvents (KeyEventArgs evt) {

    }

    public override void Close (RenderWindow window) {
        _background.StopMusic();
    }

    public void InstantiateEntity (Entity entity) {
        entity.SetLevel(this);
        entity.Start();
        _entities.Add(entity);
    }

    public void InstantiateParticle (Particle particle) {
        particle.Start();
        _particles.Add(particle);
    }

    public void AddCoins (int amount) {
        _game.Context.Coins += amount;
    }

    public void AddScore (int amount) {
        _game.Context.Score += amount;
    }

    private void DeleteDisposedObjects () {
        for (int i = _entities.Count - 1; i >= 0; i--) {
            if (_entities[i].DisposePending) {
                _entities.RemoveAt(i);
            }
        }

        for (int i = _particles.Count - 1; i >= 0; i--) {
            if (_particles[i].DisposePending) {
                _particles.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Draws one layer of tiles to the window given.
    /// </summary>
    /// <param name="window">The window in which to draw each tile.</param>
    /// <param name="layer">The tile layer to draw.</param>
    private void DrawLayer (RenderWindow window, List<Tile> layer) {
        foreach (var tile in layer) {
            tile.Draw(window);
        }
    }

    /// <summary>
    /// Draws the entities given, if they meet the criteria given.
    /// </summary>
    /// <param name="window">The window in which to draw each entity.</param>
    /// <param name="beforeForeground">If true, only entities that are drawn
    /// before the foreground are drawn. If false, all other entities are drawn.
    /// </param>
    private void DrawEntities (RenderWindow window, bool beforeForeground) {
        foreach (var entity in _entities) {
            if (entity.DrawBeforeForeground == beforeForeground) {
                entity.Draw(window);
            }
        }
    }

    private void DrawPlayer (RenderWindow window) {
        _player.Draw(window);
    }

    private void DrawParticles (RenderWindow window) {
        foreach (var particle in _particles) {
            particle.Draw(window);
        }
    }

    private void DrawColliders (RenderWindow window) {
        foreach (var tile in _foregroundLayer) {
            tile.Collider?.DrawColliderBounds(window);
        }
        foreach (var entity in _entities) {
            entity.Collider.DrawColliderBounds(window);
        }

        _player.Collider.DrawColliderBounds(window);
    }

    private void DrawDebugInfo (RenderWindow window) {
        foreach (var entity in _entities) {
            entity.DrawDebugInfo(window);
        }
    }

    private void __TEMP_initialize_player () {
        _player.SetDefaultSizes(new(32f, 32f), new(32f, 32f), new(11, 18, 10, 14));
        _player.SetGridPosition(new(0, 22));
        _player.__TEMP_set_sprite_by_filename("mario", new(32f, 32f));
        _player.__TEMP_initialize_animations();
        _player.SetLevel(this);
    }
}
