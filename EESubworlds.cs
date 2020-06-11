using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.World.Generation;
using Terraria.ModLoader;
using EEMod.Walls;
using EEMod.Tiles;
using EEMod.EEWorld;
namespace EEMod
{

    public class EESubWorlds
    {
       
      public static void Pyramids(int seed, GenerationProgress customProgressObject = null)
      {
           Main.maxTilesX = 400;
           Main.maxTilesY = 400;
           Main.spawnTileX = 234;
           Main.spawnTileY = 92;
           SubworldManager.Reset(seed, customProgressObject);
           EEWorld.EEWorld.FillRegion(400, 400, new Vector2(0, 0), TileID.SandstoneBrick);
           EEWorld.EEWorld.Pyramid(63, 42);
           EEMod.isSaving = false;
        }
        public static void Sea(int seed, GenerationProgress customProgressObject = null)
        {
            Main.maxTilesX = 400;
            Main.maxTilesY = 400;
            Main.spawnTileX = 234;
            Main.spawnTileY = 92;
            SubworldManager.Reset(seed, customProgressObject);
            EEWorld.EEWorld.FillWall(400, 400, new Vector2(0, 0), WallID.Waterfall);
            EEMod.isSaving = false;
        }
        public static void CoralReefs(int seed, GenerationProgress customProgressObject = null)
        {
            Main.maxTilesX = 1000;
            Main.maxTilesY = 2000;
            int boatPos = Main.maxTilesX / 2;
            SubworldManager.Reset(seed, customProgressObject);
            EEWorld.EEWorld.FillRegion(1000, 2000, new Vector2(0, 0), ModContent.TileType<HardenedGemsandTile>());
            EEWorld.EEWorld.ClearRegion(1000, 100, Vector2.Zero);
            for (int i = 0; i < 10; i++)
            {
                for (int j = -5; j < 5; j++)
                    WorldGen.TileRunner(300 + (i * 170) + (j * 10), 100, 10, 10, ModContent.TileType<HardenedGemsandTile>(), true, 0, 0, true, true);
            }
            EEWorld.EEWorld.FillRegionNoEdit(1000, 100, new Vector2(0, 50), ModContent.TileType<CoralSand>());
            EEWorld.EEWorld.CoralReef();
            EEWorld.EEWorld.fillRegionWithWater(1000, 1930, new Vector2(0,70));
            EEWorld.EEWorld.PlaceShip(boatPos, EEWorld.EEWorld.TileCheck(boatPos) - 22, EEWorld.EEWorld.ShipTiles);
            EEWorld.EEWorld.PlaceShipWalls(boatPos, EEWorld.EEWorld.TileCheck(boatPos) - 22, EEWorld.EEWorld.ShipWalls);
            Main.spawnTileX = boatPos;
            Main.spawnTileY = EEWorld.EEWorld.TileCheck(boatPos) - 22;
            EEMod.isSaving = false;
        }
    }
}