using SFML.Graphics;
using splatform.animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splatform.entities;
internal abstract partial class Entity {
    const long BEHAVIOR_ENEMY_START_INDEX = 1_000;

    private enum Behavior : long {
        VOID = 0,
        SUPER_MUSHROOM = 1,
        GOOMBA = BEHAVIOR_ENEMY_START_INDEX,
        KOOPA,
        VENUS_FIRE_TRAP,
    }

    private enum AnimType {
        STATIC = 0,
        DYNAMIC = 1,
    }

    public static Entity ReadNext (BinaryReader reader, bool hasLevelSettings) {
        // The position of the cursor before we begin reading this tile.
        long startOffset = reader.BaseStream.Position;
        Entity? entity = null;

        Behavior behavior = (Behavior)reader.ReadUInt32();

        // sprite
        int entitySizeX = reader.ReadByte();
        int entitySizeY = reader.ReadByte();
        int spriteSizeX = reader.ReadByte();
        int spriteSizeY = reader.ReadByte();

        int colliderTop = reader.ReadByte();
        int colliderLeft = reader.ReadByte();
        int colliderWidth = reader.ReadByte();
        int colliderHeight = reader.ReadByte();

        vec2 entitySize = new(entitySizeX, entitySizeY);
        vec2 spriteSize = new(spriteSizeX, spriteSizeY);
        IntRect collider = new(
            colliderTop, colliderLeft, colliderWidth, colliderHeight
        );

        long spriteIndex = reader.ReadUInt32();

        // animations
        int animationLength = reader.ReadByte();
        List<Animation> animations = new(animationLength);

        for (int i = 0; i < animationLength; i++) {
            animations.Add(ReadNextAnimation(reader, spriteSize));
        }

        if (behavior == Behavior.SUPER_MUSHROOM) {
            entity = ReadNextSuperMushroom(reader);
        }
        else if (behavior == Behavior.GOOMBA) {
            entity = ReadNextGoomba(reader);
        }
        else if (behavior == Behavior.KOOPA) {
            entity = ReadNextKoopa(reader);
        }
        else if (behavior == Behavior.VENUS_FIRE_TRAP) {
            entity = ReadNextVenusFireTrap(reader);
        }
        else {
            long currentOffset = reader.BaseStream.Position;
            throw new Exception(
                $"Invalid entity behavior ID: {behavior} " +
                $"(Address: 0x{startOffset:x} to 0x{currentOffset:x})"
            );
        }

        // TODO: contents of if (entity != nullptr)

        if (hasLevelSettings) {
            int xPos = reader.ReadUInt16();
            int yPos = reader.ReadUInt16();

            bool startingDirectionRight = reader.ReadBoolean();
        }

        return entity;
    }

    private static Animation ReadNextAnimation (
        BinaryReader reader, vec2 spriteSize
    ) {
        AnimType animType = (AnimType)reader.ReadByte();

        if (animType == AnimType.STATIC) {
            return ReadNextStaticAnimation(reader, spriteSize);
        }
        if (animType == AnimType.DYNAMIC) {
            return ReadNextDynamicAnimation(reader, spriteSize);
        }

        throw new Exception("Unknown animation type.");
    }

    private static StaticAnimation ReadNextStaticAnimation (
        BinaryReader reader, vec2 spriteSize
    ) {
        int slicesX = reader.ReadByte();
        int slicesY = reader.ReadByte();
        int frame = reader.ReadByte();

        return new StaticAnimation(new(slicesX, slicesY), spriteSize, frame);
    }

    private static DynamicAnimation ReadNextDynamicAnimation (
        BinaryReader reader, vec2 spriteSize
    ) {
        int slicesX = reader.ReadByte();
        int slicesY = reader.ReadByte();

        int framesLength = reader.ReadByte();
        List<int> frames = new(framesLength);

        for (int i = 0; i < framesLength; i++) {
            frames.Add(reader.ReadByte());
        }

        int frameTimesLength = reader.ReadByte();

        // frame is a value
        if (frameTimesLength == 0) {
            float frameTime = reader.ReadSingle();
            return new DynamicAnimation(
                new(slicesX, slicesY), spriteSize, frameTime, frames
            );
        }
        // frame is an array of values.
        else {
            List<float> frameTimes = new();

            for (int i = 0; i < frameTimesLength; i++) {
                frameTimes.Add(reader.ReadSingle());
            }

            return new DynamicAnimation(
                new(slicesX, slicesY), spriteSize, frameTimes, frames
            );
        }
    }

    private static Entity ReadNextSuperMushroom (BinaryReader reader) {
        // TODO

        int effectOnPlayer = reader.ReadByte(); // discarded for now.

        return new();
    }

    private static Entity ReadNextGoomba (BinaryReader reader) {
        // TODO

        bool avoidsCliffs = reader.ReadBoolean();

        return new();
    }

    private static Entity ReadNextKoopa (BinaryReader reader) {
        // TODO

        int shellColliderTop = reader.ReadByte();
        int shellColliderLeft = reader.ReadByte();
        int shellColliderWidth = reader.ReadByte();
        int shellColliderHeight = reader.ReadByte();

        bool avoidsCliffs = reader.ReadBoolean();
        bool canRevive = reader.ReadBoolean();
        bool playerCanGrabShell = reader.ReadBoolean();

        return new();
    }

    private static Entity ReadNextVenusFireTrap (BinaryReader reader) {
        int projectileCount = reader.ReadByte();
        bool canBeStomped = reader.ReadBoolean();

        return new();
    }
}
