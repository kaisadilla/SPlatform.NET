using Newtonsoft.Json;
using splatform.assets;
using splatform.assets.data;
using splatform.entities;
using splatform.tiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.game.scenes;
internal partial class LevelScene {
    /// <summary>
    /// Creates a LevelScene from the binary data given by the reader.
    /// </summary>
    /// <param name="reader"></param>
    public LevelScene (BinaryReader reader) {
        int fileType = reader.ReadByte();
        int versionMajor = reader.ReadByte();
        int versionMinor = reader.ReadByte();
        int versionRevision = reader.ReadByte();

        int width = reader.ReadUInt16();
        int height = reader.ReadUInt16();

        int background = reader.ReadUInt16();
        int music = reader.ReadUInt16();
        int time = reader.ReadUInt16();

        _backgroundLayer = ReadLayer(reader, false);
        _foregroundLayer = ReadLayer(reader, true);
        _detailLayer = ReadLayer(reader, false);
        _frontLayer = ReadLayer(reader, false);

        int entityCount = reader.ReadUInt16();

        for (int i = 0; i < entityCount; i++) {
            var entity = Entity.ReadNext(reader, true);
            entity.SetLevel(this);
            _entities.Add(entity);
        }

        Width = width;
        Height = height;
        TimeLeft = time;

        _background = new(
            Assets.GetBackgroundImageAt(background),
            Assets.GetMusicAt(music),
            new(PixelWidth, PixelHeight)
        );

    }

    public static LevelScene FromBinary (string fileName) {
        //#if DEBUG
        Stopwatch s = Stopwatch.StartNew();
        //#endif

        using FileStream stream = File.OpenRead(
            PATH_LEVELS + "/" + fileName + ".sm-binl"
        );
        using BinaryReader reader = new(stream);

        LevelScene level = new(reader);

        //#if DEBUG
        s.Stop();
        Console.WriteLine($"Level loaded in {s.ElapsedMilliseconds} ms");
        //#endif

        return level;
    }

    public static LevelScene FromJson (string json) {
        throw new NotImplementedException();
        //LevelData? data = JsonConvert.DeserializeObject<LevelData>(json);
        //
        //if (data == null) {
        //    throw new Exception("Couldn't read json file.");
        //}
        //
        //LevelScene scene = new() {
        //    Width = data.Width,
        //    Height = data.Height,
        //    _backgroundLayer = ReadJsonLayer(
        //        data.Grids.Background, data.Width, data.Height, false
        //    ),
        //    _foregroundLayer = ReadJsonLayer(
        //        data.Grids.Foreground, data.Width, data.Height, false
        //    ),
        //};
        //
        //scene.LoadBackground(data.Background);
        //scene.LoadMusic(data.Music);
        //
        //return scene;
    }

    private static List<Tile> ReadLayer (
        BinaryReader reader, bool generateColliders
    ) {
        uint tileCount = reader.ReadUInt32();
        List<Tile> tiles = new();

        for (int i = 0; i < tileCount; i++) {
            Tile tile = Tile.ReadNext(reader, generateColliders);
            tiles.Add(tile);
        }

        return tiles;
    }

    private static List<Tile> ReadJsonLayer (
        Dictionary<string, string> layer,
        int width,
        int height,
        bool generateColliders
    ) {
        List<Tile> tiles = new();

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                string sprite = layer[$"{x},{y}"];

                if (sprite == "air") continue;

                int spriteIndex = Assets.Registry!.TileSpriteIndices[sprite];
                int tileIndex = Assets.TileIndices[spriteIndex][0];

                int xStart = tileIndex % Assets.TexturesPerRow;
                int yStart = tileIndex / Assets.TexturesPerRow;
                
                //Tile tile = new() {
                //    __debug_Slice = new(
                //        xStart * Assets.TileSize,
                //        yStart * Assets.TileSize,
                //        Assets.TileSize,
                //        Assets.TileSize
                //    )
                //}; 
                Tile tile = new(); // TODO: Fix!!!
                tile.SetGridPosition(new(x, y));

                tiles.Add(tile);
            }
        }

        return tiles;
    }
}
