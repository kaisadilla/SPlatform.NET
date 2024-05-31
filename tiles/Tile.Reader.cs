using splatform.animation;
using splatform.assets;
using splatform.entities;

namespace splatform.tiles;
internal partial class Tile {
    private enum Behavior : long {
        AIR = 0,
        BLOCK = 1,
        BACKGROUND = 2,
        QUESTION_BLOCK = 3,
        DONUT_BLOCK = 4,
        PLATFORM_TOP = 5,
        BREAKABLE_BLOCK = 6,
        PIPE_ORIGIN = 7,
        PIPE_COMPONENT = 8,
    }

    private enum AnimType {
        STATIC = 0,
        DYNAMIC = 1,
    }

    private enum ContentType {
        ENTITY = 0,
        TILE = 1,
    }

    public static Tile ReadNext (BinaryReader reader, bool generateCollider) {
        // The position of the cursor before we begin reading this tile.
        long startOffset = reader.BaseStream.Position;
        Tile? tile = null;

        Behavior tileType = (Behavior)reader.ReadUInt32();

        // position
        int xPos = reader.ReadUInt16();
        int yPos = reader.ReadUInt16();
        int spriteIndex = (int)reader.ReadUInt32();

        // animations
        int animationLength = reader.ReadByte();
        List<Animation> animations = new();

        for (int i = 0; i < animationLength; i++) {
            animations.Add(ReadNextAnimation(reader, spriteIndex));
        }

        // TODO: Split into methods.
        if (tileType == Behavior.BLOCK) {
            tile = ReadNextBlock(reader);
        }
        else if (tileType == Behavior.BACKGROUND) {
            tile = new BackgroundTile();
        }
        else if (tileType == Behavior.QUESTION_BLOCK) {
            tile = ReadNextQuestionBlock(reader);
        }
        else if (tileType == Behavior.DONUT_BLOCK) {
            tile = new DonutBlock();
        }
        else if (tileType == Behavior.PLATFORM_TOP) {
            tile = new PlatformTop();
        }
        else if (tileType == Behavior.BREAKABLE_BLOCK) {
            tile = new BreakableBlock();
        }
        else if (tileType == Behavior.PIPE_ORIGIN) {
            tile = ReadNextPipeOrigin(reader);
        }
        else if (tileType == Behavior.PIPE_COMPONENT) {
            tile = new PipeComponent();
        }
        else {
            long currentOffset = reader.BaseStream.Position;
            throw new Exception(
                $"Invalid tile behavior ID: {tileType} " +
                $"(Address: 0x{startOffset:x} to 0x{currentOffset:x})"
            );
        }

        tile.SetGridPosition(new(xPos, yPos));

        foreach (var anim in animations) {
            tile._animations.AddAnimation(anim);
        }

        if (generateCollider) {
            vec2 colPos = new(xPos * PIXELS_PER_TILE, yPos * PIXELS_PER_TILE);
            vec2 colCenter = new(PIXELS_PER_TILE / 2f, PIXELS_PER_TILE / 2f);
            vec2 colDistanceToEdge = new(PIXELS_PER_TILE / 2f, PIXELS_PER_TILE / 2f);

            tile.Collider = new(tile, colPos, colCenter, colDistanceToEdge);
        }

        return tile;
    }

    private static Animation ReadNextAnimation (
        BinaryReader reader, int spriteIndex
    ) {
        AnimType animType = (AnimType)reader.ReadByte();

        if (animType == AnimType.STATIC) {
            return ReadNextStaticAnimation(reader, spriteIndex);
        }
        if (animType == AnimType.DYNAMIC) {
            return ReadNextDynamicAnimation(reader, spriteIndex);
        }

        throw new Exception("Unknown animation type.");
    }

    private static StaticAnimation ReadNextStaticAnimation (
        BinaryReader reader, int spriteIndex
    ) {
        ivec2 slices = new(Assets.TexturesPerRow, Assets.TexturesPerRow);
        vec2 sliceSize = new(Assets.TileSize, Assets.TileSize);

        int sliceCountX = reader.ReadByte();
        int sliceCountY = reader.ReadByte(); // unused for tiles, always equals 1.
        int frame = reader.ReadByte();

        int tileIndex = Assets.TileIndices[spriteIndex][frame];

        return new StaticAnimation(slices, sliceSize, tileIndex);
    }

    private static DynamicAnimation ReadNextDynamicAnimation (
        BinaryReader reader, int spriteIndex
    ) {
        ivec2 slices = new(Assets.TexturesPerRow, Assets.TexturesPerRow);
        vec2 sliceSize = new(Assets.TileSize, Assets.TileSize);

        int sliceCountX = reader.ReadByte();
        int sliceCountY = reader.ReadByte(); // unused for tiles, always equals 1.

        int framesLength = reader.ReadByte();
        List<int> frames = new();

        for (int i = 0; i < framesLength; i++) {
            int frame = reader.ReadByte();
            frames.Add(Assets.TileIndices[spriteIndex][frame]);
        }

        int frameTimesLength = reader.ReadByte();

        // frame is a value
        if (frameTimesLength == 0) {
            float frameTime = reader.ReadSingle();
            return new DynamicAnimation(slices, sliceSize, frameTime, frames);
        }
        // frame is an array of values.
        else {
            List<float> frameTimes = new();

            for (int i = 0; i < frameTimesLength; i++) {
                frameTimes.Add(reader.ReadSingle());
            }

            return new DynamicAnimation(slices, sliceSize, frameTimes, frames);
        }
    }

    private static Block ReadNextBlock (BinaryReader reader) {
        Block block = new() {
            IsHidden = false, // todo: parameterize.
        };

        return block;
    }

    private static QuestionBlock ReadNextQuestionBlock (BinaryReader reader) {
        // defaults for properties that may or may not exist.
        Entity? containedEntity = null;
        //Tile? containedTile = null;
        int maxHitCount = 1;
        bool revertToCoin = false;
        float hitTimer = 1;
        bool persistsUntilHit = true;

        bool isHidden = reader.ReadBoolean();
        var contentType = (QuestionBlock.ContentType)reader.ReadByte();

        if (contentType == QuestionBlock.ContentType.Entity) {
            containedEntity = Entity.ReadNext(reader, false);
        }

        var hitMode = (QuestionBlock.HitMode)reader.ReadByte();

        if (hitMode == QuestionBlock.HitMode.Times) {
            maxHitCount = reader.ReadByte();
            revertToCoin = reader.ReadBoolean();
        }
        else if (hitMode == QuestionBlock.HitMode.Seconds) {
            hitTimer = reader.ReadSingle();
            persistsUntilHit = reader.ReadBoolean();
            revertToCoin = reader.ReadBoolean();
        }

        QuestionBlock block = new() {
            IsHidden = isHidden,
            ContentTypeV = contentType,
            HitModeV = hitMode,
            ContainedEntity = containedEntity,
            ContainedTile = null,
            MaxHitCount = maxHitCount,
            RevertToCoin = revertToCoin,
            HitTimer = hitTimer,
            PersistsUntilHit = persistsUntilHit,
        };

        return block;
    }

    private static PipeOrigin ReadNextPipeOrigin (BinaryReader reader) {
        // TODO: Check when pipes are added.
        // defaults for properties that may or may not exist.
        Entity? containedEntity = null;
        Entity? generatedEntity = null;

        bool containsEntity = reader.ReadBoolean();
        if (containsEntity) {
            containedEntity = Entity.ReadNext(reader, false);
        }

        bool generatesEntity = reader.ReadBoolean();
        if (generatesEntity) {
            generatedEntity = Entity.ReadNext(reader, false);
        }

        bool containsWarp = reader.ReadBoolean();

        PipeOrigin pipe = new() {
            ContainsEntity = containsEntity,
            ContainedEntity = containedEntity,
            GeneratesEntity = generatesEntity,
            GeneratedEntity = generatedEntity,
            MaxGenerationCount = 1, // todo parameterize
            ContainsWarp = containsWarp,
        };

        return pipe;
    }
}
