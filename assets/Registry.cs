using Newtonsoft.Json.Linq;

namespace splatform.assets;
internal class Registry {
    public string[] BackgroundImages { get; private set; }
    public string[] Music { get; private set; }
    public int TileSize { get; private set; }
    public string[] TileSprites { get; private set; }
    public Dictionary<string, int> TileSpriteIndices { get; private set; }
    public string[] EntitySprites { get; private set; }
    public string[] ParticleSprites { get; private set; }

    public Registry (string path) {
        Console.WriteLine("=== LOADING ASSET REGISTRY ===");

        string str_registry = ReadRegistry(path);
        JObject json_registry = JObject.Parse(str_registry);

        // TODO: Complex mode with error handling and stuff.
        BackgroundImages = json_registry["backgrounds"]!.ToObject<string[]>()!;
        Music = json_registry["music"]!.ToObject<string[]>()!;
        TileSize = json_registry["sprites"]!["tile_size"]!.ToObject<int>()!;
        TileSprites = json_registry["sprites"]!["tiles"]!.ToObject<string[]>()!;
        EntitySprites = json_registry["sprites"]!["entities"]!.ToObject<string[]>()!;
        ParticleSprites = json_registry["sprites"]!["particles"]!.ToObject<string[]>()!;

        TileSpriteIndices = new();

        for (int i = 0; i < TileSprites.Length; i++) {
            TileSpriteIndices[TileSprites[i]] = i;
        }
    }

    private string ReadRegistry (string path) {
        using StreamReader reader = new(path);

        try {
            var str = reader.ReadToEnd();
            return str;
        }
        catch (Exception) {
            Console.WriteLine($"Couldn't read file at path '{path}'");
            throw;
        }
    }
}
