﻿using EEMod.Common.IDs;
using EEMod.Tiles;
using EEMod.Tiles.EmptyTileArrays;
using EEMod.Tiles.Furniture;
using EEMod.Tiles.Furniture.Coral;
using EEMod.Tiles.Furniture.Coral.HangingCoral;
using EEMod.Tiles.Furniture.Coral.WallCoral;
using EEMod.Tiles.Ores;
using EEMod.VerletIntegration;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

//using Microsoft.Office.Interop.Excel;

namespace EEMod.EEWorld
{
    public partial class EEWorld
    {
        /*public static void MakeKramkenArena(int xPos, int yPos, int size)
        {
            int maxTiles = (int)(Main.maxTilesX * Main.maxTilesY * 9E-04);
            for (int k = 0; k < maxTiles * 60; k++)
            {
                int x = WorldGen.genRand.Next(xPos - (size * 2), xPos + (size * 2));
                int y = WorldGen.genRand.Next(yPos - (size * 2), yPos + (size * 2));
                if (OvalCheck(xPos, yPos, x, y, size * 2, size))
                {
                    WorldGen.TileRunner(x, y, WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(5, 10), TileID.StoneSlab, true, 0f, 0f, true, true);
                }
            }
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    Tile tile = Framing.GetTileSafely(i, j);
                    if (tile.type == TileID.StoneSlab)
                    {
                        WorldGen.KillTile(i, j);
                    }
                }
            }
        }*/

        public static PerlinNoiseFunction perlinNoise;

        public static bool GemsandCheck(int i, int j)
        {
            int ecksdee = Main.tile[i, j].type;
            return ecksdee == ModContent.TileType<LightGemsandTile>() || ecksdee == ModContent.TileType<LightGemsandstoneTile>() || ecksdee == ModContent.TileType<GemsandTile>() || ecksdee == ModContent.TileType<GemsandstoneTile>() || ecksdee == ModContent.TileType<DarkGemsandTile>() || ecksdee == ModContent.TileType<DarkGemsandstoneTile>();
        }

        public static void MakeCoralRoom(int xPos, int yPos, int size, int type, int minibiome, bool ensureNoise = false)
        {
            int sizeX = size;
            int sizeY = size / 2;
            Vector2 TL = new Vector2(xPos - sizeX / 2f, yPos - sizeY / 2f);
            Vector2 BR = new Vector2(xPos + sizeX / 2f, yPos + sizeY / 2f);

            Vector2 startingPoint = new Vector2(xPos - sizeX, yPos - sizeY);
            int tile2;
            if (TL.Y < Main.maxTilesY * 0.33f)
            {
                tile2 = (ushort)ModContent.TileType<LightGemsandTile>();
            }
            else if (TL.Y < Main.maxTilesY * 0.66f)
            {
                tile2 = (ushort)ModContent.TileType<GemsandTile>();
            }
            else
            {
                tile2 = (ushort)ModContent.TileType<DarkGemsandTile>();
            }
            if (TL.Y < Main.maxTilesY / 10)
            {
                tile2 = (ushort)ModContent.TileType<CoralSandTile>();
            }
            void CreateNoise(bool ensureN, int width, int height, float thresh)
            {
                perlinNoise = new PerlinNoiseFunction(1000, 1000, width, height, thresh);
                int[,] perlinNoiseFunction = perlinNoise.perlinBinary;
                if (ensureN)
                {
                    for (int i = (int)startingPoint.X; i < (int)startingPoint.X + sizeX * 2; i++)
                    {
                        for (int j = (int)startingPoint.Y; j < (int)startingPoint.Y + sizeY * 2; j++)
                        {
                            if (i > 0 && i < Main.maxTilesX && j > 0 && j < Main.maxTilesY)
                            {
                                if (i - (int)startingPoint.X < 1000 && j - (int)startingPoint.Y < 1000)
                                {
                                    if (perlinNoiseFunction[i - (int)startingPoint.X, j - (int)startingPoint.Y] == 1 && OvalCheck(xPos, yPos, i, j, sizeX, sizeY) && WorldGen.InWorld(i, j))
                                    {
                                        Tile tile = Framing.GetTileSafely(i, j);
                                        if (j < Main.maxTilesY * 0.33f)
                                        {
                                            tile.type = (ushort)ModContent.TileType<LightGemsandTile>();
                                        }
                                        else if (j < Main.maxTilesY * 0.66f)
                                        {
                                            tile.type = (ushort)ModContent.TileType<GemsandTile>();
                                        }
                                        else if (j > Main.maxTilesY * 0.66f)
                                        {
                                            tile.type = (ushort)ModContent.TileType<DarkGemsandTile>();
                                        }

                                        if (j < Main.maxTilesY / 10)
                                        {
                                            tile.type = (ushort)ModContent.TileType<CoralSandTile>();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            RemoveStoneSlabs();

            switch (type) //Creating the formation of the room(the shape)
            {
                case -1:
                    MakeJaggedOval(sizeX, sizeY, new Vector2(TL.X, TL.Y), TileID.StoneSlab, true);
                    MakeOvalFlatTop(sizeX / 3, sizeY / 3, new Vector2(xPos + 0, yPos + 0), tile2);
                    MakeOvalFlatTop(sizeX / 3, sizeY / 3, new Vector2(xPos + (-sizeX / 5 - sizeX / 6), yPos + (-sizeY / 5 - sizeY / 6)), tile2);
                    MakeOvalFlatTop(sizeX / 3, sizeY / 3, new Vector2(xPos + (sizeX / 5 - sizeX / 6), yPos + (-sizeY / 5 - sizeY / 6)), tile2);
                    MakeOvalFlatTop(sizeX / 3, sizeY / 3, new Vector2(xPos + (sizeX / 5 - sizeX / 6), yPos + (sizeY / 5 - sizeY / 6)), tile2);
                    MakeOvalFlatTop(sizeX / 3, sizeY / 3, new Vector2(xPos + (-sizeX / 5 - sizeX / 6), yPos + (sizeY / 5 - sizeY / 6)), tile2);
                    break;

                case 0:
                    MakeJaggedOval(sizeX, sizeY, new Vector2(TL.X, TL.Y), TileID.StoneSlab, true);
                    MakeOvalFlatTop(sizeX / 3, sizeY / 3, new Vector2(xPos + 0, yPos + 0), tile2);
                    MakeOvalFlatTop(sizeX / 3, sizeY / 3, new Vector2(xPos + (-sizeX / 5), yPos + (-sizeY / 5)), tile2);
                    MakeOvalFlatTop(sizeX / 3, sizeY / 3, new Vector2(xPos + (sizeX / 5), yPos + (-sizeY / 5)), tile2);
                    MakeOvalFlatTop(sizeX / 3, sizeY / 3, new Vector2(xPos + (sizeX / 5), yPos + (sizeY / 5)), tile2);
                    MakeOvalFlatTop(sizeX / 3, sizeY / 3, new Vector2(xPos + (-sizeX / 5), yPos + (sizeY / 5)), tile2);
                    CreateNoise(!ensureNoise, 100, 10, 0.45f);
                    break;

                case 1:
                    MakeJaggedOval(sizeX, sizeY, new Vector2(TL.X, TL.Y), TileID.StoneSlab, true);
                    CreateNoise(!ensureNoise, 20, 200, 0.5f);
                    break;

                case 2:
                    MakeJaggedOval(sizeX, sizeY, new Vector2(TL.X, TL.Y), TileID.StoneSlab, true);
                    CreateNoise(!ensureNoise, 200, 20, 0.5f);
                    break;

                case 3:
                    MakeJaggedOval(sizeX, sizeY, new Vector2(TL.X, TL.Y), TileID.StoneSlab, true);
                    MakeWavyChasm3(new Vector2(TL.X - 50 + WorldGen.genRand.Next(-30, 30), TL.Y - 10), new Vector2(BR.X - 50 + WorldGen.genRand.Next(-30, 30), BR.Y - 10), tile2, 100, 4, true, new Vector2(10, 13), 50, 20);
                    MakeWavyChasm3(new Vector2(TL.X + 50 + WorldGen.genRand.Next(-30, 30), yPos + 10), new Vector2(BR.X + 50 + WorldGen.genRand.Next(-30, 30), yPos + 10), tile2, 100, 4, true, new Vector2(10, 13), 50, 20);
                    MakeWavyChasm3(new Vector2(TL.X + WorldGen.genRand.Next(-30, 30), TL.Y - 10), new Vector2(BR.X + WorldGen.genRand.Next(-30, 30), BR.Y - 10), tile2, 100, 4, true, new Vector2(10, 13), 50, 20);
                    MakeWavyChasm3(new Vector2(TL.X + WorldGen.genRand.Next(-100, 100), TL.Y - 10), new Vector2(BR.X + WorldGen.genRand.Next(-30, 30), BR.Y - 10), tile2, 100, 4, true, new Vector2(10, 13), 50, 20);
                    MakeWavyChasm3(new Vector2(TL.X + WorldGen.genRand.Next(-100, 100), yPos - 10), new Vector2(BR.X + WorldGen.genRand.Next(-30, 30), BR.Y - 10), tile2, 100, 4, true, new Vector2(10, 13), 50, 20);
                    break;

                case 4:

                    MakeJaggedOval(sizeX, sizeY, new Vector2(TL.X, TL.Y), TileID.StoneSlab, true);
                    for (int i = 0; i < 20; i++)
                    {
                        MakeCircle(WorldGen.genRand.Next(5, 20), new Vector2(TL.X + WorldGen.genRand.Next(sizeX), TL.Y + WorldGen.genRand.Next(sizeY)), tile2, true);
                    }
                    RemoveStoneSlabs();
                    for (int i = (int)startingPoint.X; i < (int)startingPoint.X + sizeX * 2; i++)
                    {
                        for (int j = (int)startingPoint.Y; j < (int)startingPoint.Y + sizeY * 2; j++)
                        {
                            int noOfTiles = 0;
                            for (int k = -5; k < 5; k++)
                            {
                                for (int l = -5; l < 5; l++)
                                {
                                    if (WorldGen.InWorld(i + k, j + l, 10))
                                    {
                                        if (Main.tile[i + k, j + l].active() && Main.tileSolid[Main.tile[i + k, j + l].type])
                                        {
                                            noOfTiles++;
                                        }
                                    }
                                }
                            }
                            if (EESubWorlds.BulbousTreePosition.Count > 0)
                            {
                                for (int m = 0; m < EESubWorlds.BulbousTreePosition.Count; m++)
                                {
                                    if (Vector2.DistanceSquared(new Vector2(i, j), EESubWorlds.BulbousTreePosition[m]) < 45 * 45)
                                    {
                                        noOfTiles += 5;
                                    }
                                }
                            }
                            if (EESubWorlds.OrbPositions.Count > 0)
                            {
                                for (int m = 0; m < EESubWorlds.OrbPositions.Count; m++)
                                {
                                    if (Vector2.DistanceSquared(new Vector2(i, j), EESubWorlds.OrbPositions[m]) < 20 * 20)
                                    {
                                        noOfTiles += 5;
                                    }
                                }
                            }
                            if (noOfTiles < 3)
                            {
                                EESubWorlds.BulbousTreePosition.Add(new Vector2(i, j));
                            }
                        }
                    }
                    break;

                case 5:
                    MakeJaggedOval(sizeX, sizeY * 2, new Vector2(TL.X, yPos - sizeY), TileID.StoneSlab, true);
                    MakeJaggedOval((int)(sizeX * 0.8f), (int)(sizeY * 1.6f), new Vector2(xPos - sizeX * 0.4f, yPos - sizeY * 0.8f), tile2, true);
                    MakeJaggedOval(sizeX / 10, sizeY / 5, new Vector2(xPos - sizeX / 20, yPos - sizeY / 10), TileID.StoneSlab, true);
                    for (int i = 0; i < 30; i++)
                    {
                        MakeCircle(WorldGen.genRand.Next(5, 20), new Vector2(TL.X + WorldGen.genRand.Next(sizeX), yPos - sizeY + WorldGen.genRand.Next(sizeY * 2)), TileID.StoneSlab, true);
                    }

                    break;

                case 6:
                    MakeJaggedOval((int)(sizeX * 1.3f), sizeY, new Vector2(TL.X, TL.Y), TileID.StoneSlab, true);
                    MakeWavyChasm3(new Vector2(TL.X - 50 + WorldGen.genRand.Next(-30, 30), TL.Y - 10), new Vector2(BR.X - 50 + WorldGen.genRand.Next(-30, 30), BR.Y - 10), tile2, 100, 4, true, new Vector2(10, 13), 50, 20);
                    MakeWavyChasm3(new Vector2(TL.X + 50 + WorldGen.genRand.Next(-30, 30), yPos + 10), new Vector2(BR.X + 50 + WorldGen.genRand.Next(-30, 30), yPos + 10), tile2, 100, 4, true, new Vector2(10, 13), 50, 20);
                    MakeWavyChasm3(new Vector2(TL.X + WorldGen.genRand.Next(-30, 30), TL.Y - 10), new Vector2(BR.X + WorldGen.genRand.Next(-30, 30), BR.Y - 10), tile2, 100, 4, true, new Vector2(10, 13), 50, 20);
                    MakeWavyChasm3(new Vector2(TL.X + WorldGen.genRand.Next(-100, 100), TL.Y - 10), new Vector2(BR.X + WorldGen.genRand.Next(-30, 30), BR.Y - 10), tile2, 100, 4, true, new Vector2(10, 13), 50, 20);
                    MakeWavyChasm3(new Vector2(TL.X + WorldGen.genRand.Next(-100, 100), yPos - 10), new Vector2(BR.X + WorldGen.genRand.Next(-30, 30), BR.Y - 10), tile2, 100, 4, true, new Vector2(10, 13), 50, 20);
                    CreateNoise(true, 100, 20, 0.2f);
                    break;
            }

            /*switch (minibiome)
            {
                case MinibiomeID.Anemone:
                /*perlinNoise = new PerlinNoiseFunction(sizeX, sizeY, 50, 50, 0.5f);
                int[,] perlinNoiseFunction = perlinNoise.perlinBinary;
                for (int i = 0; i < sizeX; i++)
                {
                    for (int j = 0; j < sizeY; j++)
                    {
                        if (perlinNoiseFunction[i, j] == 1)
                        {
                            WorldGen.PlaceWall(i + xPos, j + yPos, ModContent.WallType<AnemoneWallTile>());
                        }
                    }
                }
                break;

                case MinibiomeID.CrystallineCaves:
                    break;
            }*/

            CreateNoise(ensureNoise, Main.rand.Next(30, 50), Main.rand.Next(20, 40), Main.rand.NextFloat(0.4f, 0.6f));
        }

        public static void MakeCrystal(int xPos, int yPos, int length, int width, int vertDir, int horDir, int type)
        {
            for (int a = 0; a < length; a++)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (!Main.tile[i, j].active())
                        {
                            WorldGen.TileRunner(i + xPos + (a * horDir), j + yPos + (a * vertDir), Main.rand.Next(2, 3), Main.rand.Next(1, 2), type, true, 0, 0, false, false);
                        }
                    }
                }
            }
        }

        public static void PlaceCoral()
        {
            for (int i = 42; i < Main.maxTilesX - 42; i++)
            {
                for (int j = 42; j < Main.maxTilesY - 42; j++)
                {
                    #region Surface Reefs

                    if (j < Main.maxTilesY / 10)
                    {
                        if (TileCheck2(i, j) == 2 && WorldGen.InWorld(i, j) && !Main.rand.NextBool(6)) //Surface Reefs
                        {
                            int selection = WorldGen.genRand.Next(8);
                            switch (selection)
                            {
                                case 0:
                                    WorldGen.PlaceTile(i, j - 1, 324);
                                    break;

                                case 1:
                                    WorldGen.PlaceTile(i, j - 1, 324, style: 2);
                                    break;

                                case 2:
                                    WorldGen.PlaceTile(i, j - 1, TileID.Coral);
                                    break;

                                case 3:
                                    WorldGen.PlaceTile(i, j - 3, ModContent.TileType<Floor3x3Coral>(), style: WorldGen.genRand.Next(2));
                                    break;

                                case 4:
                                    WorldGen.PlaceTile(i, j - 2, ModContent.TileType<Floor1x2Coral>(), style: WorldGen.genRand.Next(7));
                                    break;

                                case 5:
                                    WorldGen.PlaceTile(i, j - 1, ModContent.TileType<Floor1x1Coral>(), style: WorldGen.genRand.Next(3));
                                    break;

                                case 6:
                                    WorldGen.PlaceTile(i, j - 2, ModContent.TileType<Floor2x2Coral>(), style: WorldGen.genRand.Next(5));
                                    break;

                                case 7:
                                    WorldGen.PlaceTile(i, j - 2, ModContent.TileType<Floor2x1Coral>(), style: WorldGen.genRand.Next(5));
                                    break;
                            }
                        }
                    }

                    #endregion Surface Reefs

                    else
                    {
                        if (WorldGen.InWorld(i, j) && GemsandCheck(i, j))
                        {
                            int minibiome = 0;
                            for (int k = 0; k < EESubWorlds.MinibiomeLocations.Count; k++)
                            {
                                if (Vector2.DistanceSquared(new Vector2(EESubWorlds.MinibiomeLocations[k].X, EESubWorlds.MinibiomeLocations[k].Y), new Vector2(i, j)) < (220 * 220) && EESubWorlds.MinibiomeLocations[k].Z != 0)
                                {
                                    minibiome = (int)EESubWorlds.MinibiomeLocations[k].Z;
                                    break;
                                }
                            }

                            int selection;
                            switch (minibiome)
                            {
                                #region Default

                                case MinibiomeID.None: //Default
                                    if (!WorldGen.genRand.NextBool(6))
                                    {
                                        switch (TileCheck2(i, j))
                                        {
                                            case 1:
                                                selection = WorldGen.genRand.Next(6);
                                                switch (selection)
                                                {
                                                    case 0:
                                                        WorldGen.PlaceTile(i, j + 1, ModContent.TileType<Hanging1x2Coral>());
                                                        break;

                                                    case 1:
                                                        WorldGen.PlaceTile(i, j + 1, ModContent.TileType<Hanging1x3Coral>(), style: WorldGen.genRand.Next(2));
                                                        break;

                                                    case 2:
                                                        WorldGen.PlaceTile(i, j + 1, ModContent.TileType<Hanging2x3Coral>(), style: WorldGen.genRand.Next(2));
                                                        break;

                                                    case 3:
                                                        WorldGen.PlaceTile(i, j + 1, ModContent.TileType<Hanging2x4Coral>(), style: WorldGen.genRand.Next(3));
                                                        break;

                                                    case 4:
                                                        ModContent.GetInstance<GlowHangCoral2TE>().Place(i, j + 1);
                                                        WorldGen.PlaceTile(i, j + 1, ModContent.TileType<GlowHangCoral2>());
                                                        break;

                                                    case 5:
                                                        WorldGen.PlaceTile(i, j + 1, ModContent.TileType<Hanging1x4Coral>());
                                                        break;
                                                }
                                                break;

                                            case 2:
                                            {
                                                selection = WorldGen.genRand.Next(16);
                                                switch (selection)
                                                {
                                                    case 0:
                                                        WorldGen.PlaceTile(i, j - 8, ModContent.TileType<Floor6x8Coral>());
                                                        break;

                                                    case 1:
                                                        WorldGen.PlaceTile(i, j - 8, ModContent.TileType<Floor8x8Coral>());
                                                        break;

                                                    case 2:
                                                        WorldGen.PlaceTile(i, j - 3, ModContent.TileType<Floor3x3Coral>(), style: WorldGen.genRand.Next(2));
                                                        break;

                                                    case 3:
                                                        WorldGen.PlaceTile(i, j - 2, ModContent.TileType<Floor1x2Coral>(), style: WorldGen.genRand.Next(7));
                                                        break;

                                                    case 4:
                                                        WorldGen.PlaceTile(i, j - 1, ModContent.TileType<Floor1x1Coral>(), style: WorldGen.genRand.Next(3));
                                                        break;

                                                    case 5:
                                                        WorldGen.PlaceTile(i, j - 2, ModContent.TileType<Floor2x2Coral>(), style: WorldGen.genRand.Next(5));
                                                        break;

                                                    case 6:
                                                        WorldGen.PlaceTile(i, j - 7, ModContent.TileType<Floor7x7Coral>());
                                                        break;

                                                    case 7:
                                                        WorldGen.PlaceTile(i, j - 8, ModContent.TileType<Floor8x7Coral>());
                                                        break;

                                                    case 8:
                                                        WorldGen.PlaceTile(i, j - 6, ModContent.TileType<Floor4x2Coral>(), style: WorldGen.genRand.Next(3));
                                                        break;

                                                    case 9:
                                                        WorldGen.PlaceTile(i, j - 3, ModContent.TileType<Floor5x3Coral>());
                                                        break;

                                                    case 11:
                                                        WorldGen.PlaceTile(i, j - 1, ModContent.TileType<BlueKelpTile>());
                                                        break;

                                                    case 12:
                                                        switch (WorldGen.genRand.Next(4))
                                                        {
                                                            case 0:
                                                                WorldGen.PlaceTile(i, j - 2, ModContent.TileType<FloorGlow2x2Coral1>());
                                                                break;

                                                            case 1:
                                                                WorldGen.PlaceTile(i, j - 2, ModContent.TileType<FloorGlow2x2Coral2>());
                                                                break;

                                                            case 2:
                                                                WorldGen.PlaceTile(i, j - 2, ModContent.TileType<FloorGlow2x2Coral3>());
                                                                break;

                                                            case 3:
                                                                WorldGen.PlaceTile(i, j - 2, ModContent.TileType<FloorGlow1x2Coral1>());
                                                                break;
                                                        }
                                                        break;

                                                    case 13:
                                                        WorldGen.PlaceTile(i, j - 3, ModContent.TileType<ThermalVent>());
                                                        break;

                                                    case 14:
                                                        WorldGen.PlaceTile(i, j - 1, ModContent.TileType<Floor2x1Coral>(), style: WorldGen.genRand.Next(5));
                                                        break;

                                                    case 15:
                                                        WorldGen.PlaceTile(i, j - 6, ModContent.TileType<Floor2x6Coral>(), style: WorldGen.genRand.Next(2));
                                                        break;
                                                }
                                                break;
                                            }
                                            case 3:
                                                selection = WorldGen.genRand.Next(8);
                                                switch (selection)
                                                {
                                                    case 0:
                                                        WorldGen.PlaceTile(i + 1, j, ModContent.TileType<Wall2x2CoralL>(), style: WorldGen.genRand.Next(3));
                                                        break;

                                                    case 1:
                                                        WorldGen.PlaceTile(i + 1, j, ModContent.TileType<Wall3x2CoralL>(), style: WorldGen.genRand.Next(2));
                                                        break;

                                                    case 2:
                                                        WorldGen.PlaceTile(i + 1, j, ModContent.TileType<Wall4x2CoralL>(), style: WorldGen.genRand.Next(2));
                                                        break;

                                                    case 3:
                                                        WorldGen.PlaceTile(i + 1, j, ModContent.TileType<Wall4x3CoralL>());
                                                        break;

                                                    case 4:
                                                        WorldGen.PlaceTile(i + 1, j, ModContent.TileType<Wall2x2NonsolidCoralL>(), style: WorldGen.genRand.Next(2));
                                                        break;

                                                    case 5:
                                                        WorldGen.PlaceTile(i + 1, j, ModContent.TileType<Wall3x2NonsolidCoralL>());
                                                        break;

                                                    case 6:
                                                        WorldGen.PlaceTile(i + 1, j, ModContent.TileType<Wall5x2NonsolidCoralL>());
                                                        break;

                                                    case 7:
                                                        WorldGen.PlaceTile(i + 1, j, ModContent.TileType<Wall5x3CoralL>());
                                                        break;
                                                }
                                                break;

                                            case 4:
                                                selection = WorldGen.genRand.Next(8);
                                                switch (selection)
                                                {
                                                    case 0:
                                                        WorldGen.PlaceTile(i - 2, j, ModContent.TileType<Wall2x2CoralR>(), style: WorldGen.genRand.Next(3));
                                                        break;

                                                    case 1:
                                                        WorldGen.PlaceTile(i - 3, j, ModContent.TileType<Wall3x2CoralR>(), style: WorldGen.genRand.Next(2));
                                                        break;

                                                    case 2:
                                                        WorldGen.PlaceTile(i - 4, j, ModContent.TileType<Wall4x2CoralR>(), style: WorldGen.genRand.Next(2));
                                                        break;

                                                    case 3:
                                                        WorldGen.PlaceTile(i - 4, j, ModContent.TileType<Wall4x3CoralR>());
                                                        break;

                                                    case 4:
                                                        WorldGen.PlaceTile(i - 2, j, ModContent.TileType<Wall2x2NonsolidCoralR>(), style: WorldGen.genRand.Next(2));
                                                        break;

                                                    case 5:
                                                        WorldGen.PlaceTile(i - 3, j, ModContent.TileType<Wall3x2NonsolidCoralR>());
                                                        break;

                                                    case 6:
                                                        WorldGen.PlaceTile(i - 5, j, ModContent.TileType<Wall5x2NonsolidCoralR>());
                                                        break;

                                                    case 7:
                                                        WorldGen.PlaceTile(i - 5, j, ModContent.TileType<Wall5x3CoralR>());
                                                        break;
                                                }
                                                break;
                                        }
                                    }
                                    break;

                                #endregion Default

                                #region Kelp Forest

                                case MinibiomeID.KelpForest: //Kelp Forest (Glowing Kelp/Greencoral)
                                    if (TileCheck2(i, j) == 2 && !WorldGen.genRand.NextBool(6))
                                    {
                                        if (!WorldGen.genRand.NextBool(4))
                                        {
                                            WorldGen.PlaceTile(i, j - 1, ModContent.TileType<GreenKelpTile>());
                                        }
                                        else if (!Main.rand.NextBool(6))
                                        {
                                            selection = WorldGen.genRand.Next(4);
                                            switch (selection)
                                            {
                                                case 0:
                                                    WorldGen.PlaceTile(i, j - 2, ModContent.TileType<Floor1x2Coral>(), style: WorldGen.genRand.Next(7));
                                                    break;

                                                case 1:
                                                    WorldGen.PlaceTile(i, j - 1, ModContent.TileType<Floor1x1Coral>(), style: WorldGen.genRand.Next(3));
                                                    break;

                                                case 2:
                                                    if (TileCheck2(i, j) == 2 && TileCheck2(i + 1, j) == 2 && TileCheck2(i + 2, j) == 2)
                                                    {
                                                        ModContent.GetInstance<GroundGlowCoralTE>().Place(i, j - 13);
                                                        WorldGen.PlaceTile(i, j - 13, ModContent.TileType<GroundGlowCoral>());
                                                    }
                                                    else
                                                    {
                                                        ModContent.GetInstance<GroundGlowCoral3TE>().Place(i, j - 4);
                                                        WorldGen.PlaceTile(i, j - 4, ModContent.TileType<GroundGlowCoral3>());
                                                    }
                                                    break;

                                                case 3:
                                                    if (TileCheck2(i, j) == 2 && TileCheck2(i + 1, j) == 2 && TileCheck2(i + 2, j) == 2)
                                                    {
                                                        ModContent.GetInstance<GroundGlowCoral2TE>().Place(i, j - 5);
                                                        WorldGen.PlaceTile(i, j - 5, ModContent.TileType<GroundGlowCoral2>());
                                                    }
                                                    else
                                                    {
                                                        ModContent.GetInstance<GroundGlowCoral3TE>().Place(i, j - 4);
                                                        WorldGen.PlaceTile(i, j - 4, ModContent.TileType<GroundGlowCoral3>());
                                                    }
                                                    break;

                                                case 4:
                                                    ModContent.GetInstance<GroundGlowCoral3TE>().Place(i, j - 4);
                                                    WorldGen.PlaceTile(i, j - 4, ModContent.TileType<GroundGlowCoral3>());
                                                    break;
                                            }
                                        }
                                    }
                                    if (TileCheck2(i, j) == 1 && WorldGen.genRand.NextBool(6))
                                    {
                                        ModContent.GetInstance<GlowHangCoral1TE>().Place(i, j + 1);
                                        WorldGen.PlaceTile(i, j + 1, ModContent.TileType<GlowHangCoral1>());
                                    }
                                    else if (TileCheck2(i, j) == 1 && WorldGen.genRand.NextBool(15))
                                    {
                                        VerletHelpers.AddStickChain(ref ModContent.GetInstance<EEMod>().verlet, new Vector2(i * 16, j * 16), Main.rand.Next(5, 15), 27);
                                    }
                                    break;

                                #endregion Kelp Forest

                                #region The Great Anemone

                                case MinibiomeID.Anemone: //Anemone(A massive anemone throughout the minibiome that electrocutes the player on contact, coral fans)
                                    if (!WorldGen.genRand.NextBool(6))
                                    {
                                        switch (TileCheck2(i, j))
                                        {
                                            case 2:
                                                selection = WorldGen.genRand.Next(2); // WorldGen.genRand.Next(1);
                                                switch (selection)
                                                {
                                                    case 0:
                                                        WorldGen.PlaceTile(i, j - 3, ModContent.TileType<Floor4x3Coral>());
                                                        break;

                                                    case 1:
                                                        WorldGen.PlaceTile(i, j - 3, ModContent.TileType<Floor3x3Coral>(), style: 1);
                                                        break;
                                                }
                                                break;

                                            case 3:
                                                selection = WorldGen.genRand.Next(5);
                                                switch (selection)
                                                {
                                                    case 0:
                                                        WorldGen.PlaceTile(i + 1, j, ModContent.TileType<Wall3x2NonsolidCoralL>());
                                                        break;

                                                    case 1:
                                                        WorldGen.PlaceTile(i + 1, j, ModContent.TileType<Wall5x2NonsolidCoralL>());
                                                        break;

                                                    case 2:
                                                        WorldGen.PlaceTile(i + 1, j, ModContent.TileType<Wall2x2CoralL>(), style: 0);
                                                        break;

                                                    case 3:
                                                        WorldGen.PlaceTile(i + 1, j, ModContent.TileType<Wall3x2CoralL>(), style: 0);
                                                        break;

                                                    case 4:
                                                        WorldGen.PlaceTile(i + 1, j, ModContent.TileType<Wall4x2CoralL>(), style: 0);
                                                        break;
                                                }
                                                break;

                                            case 4:
                                                selection = WorldGen.genRand.Next(5);
                                                switch (selection)
                                                {
                                                    case 0:
                                                        WorldGen.PlaceTile(i - 3, j, ModContent.TileType<Wall3x2NonsolidCoralR>());
                                                        break;

                                                    case 1:
                                                        WorldGen.PlaceTile(i - 5, j, ModContent.TileType<Wall5x2NonsolidCoralR>());
                                                        break;

                                                    case 2:
                                                        WorldGen.PlaceTile(i - 2, j, ModContent.TileType<Wall2x2CoralR>(), style: 0);
                                                        break;

                                                    case 3:
                                                        WorldGen.PlaceTile(i - 3, j, ModContent.TileType<Wall3x2CoralR>(), style: 0);
                                                        break;

                                                    case 4:
                                                        WorldGen.PlaceTile(i - 4, j, ModContent.TileType<Wall4x2CoralR>(), style: 0);
                                                        break;
                                                }
                                                break;
                                        }
                                    }
                                    break;

                                #endregion The Great Anemone

                                #region Jellyfish Caverns

                                case MinibiomeID.JellyfishCaverns: //Jellyfish Caverns(More hanging coral/longer hanging coral)
                                    if (!WorldGen.genRand.NextBool(6))
                                    {
                                        switch (TileCheck2(i, j))
                                        {
                                            case 1:
                                                selection = WorldGen.genRand.Next(5);
                                                switch (selection)
                                                {
                                                    case 0:
                                                        WorldGen.PlaceTile(i, j + 1, ModContent.TileType<Hanging1x2Coral>());
                                                        break;

                                                    case 1:
                                                        WorldGen.PlaceTile(i, j + 1, ModContent.TileType<Hanging1x3Coral>(), style: WorldGen.genRand.Next(2));
                                                        break;

                                                    case 2:
                                                        WorldGen.PlaceTile(i, j + 1, ModContent.TileType<Hanging2x3Coral>(), style: WorldGen.genRand.Next(2));
                                                        break;

                                                    case 3:
                                                        WorldGen.PlaceTile(i, j + 1, ModContent.TileType<Hanging2x4Coral>(), style: WorldGen.genRand.Next(2));
                                                        break;

                                                    case 4:
                                                        WorldGen.PlaceTile(i, j + 1, ModContent.TileType<Hanging1x4Coral>());
                                                        break;
                                                }
                                                break;

                                            case 2:
                                                selection = WorldGen.genRand.Next(3);
                                                switch (selection)
                                                {
                                                    case 0:
                                                        WorldGen.PlaceTile(i, j - 2, ModContent.TileType<Floor1x2Coral>(), style: WorldGen.genRand.Next(7));
                                                        break;

                                                    case 1:
                                                        WorldGen.PlaceTile(i, j - 6, ModContent.TileType<Floor2x6Coral>(), style: WorldGen.genRand.Next(2));
                                                        break;

                                                    case 2:
                                                        WorldGen.PlaceTile(i, j - 8, ModContent.TileType<Floor6x8Coral>());
                                                        break;
                                                }
                                                break;

                                            case 3:
                                                WorldGen.PlaceTile(i + 1, j, ModContent.TileType<WallGlow2x3NonsolidCoralL>());
                                                break;

                                            case 4:
                                                WorldGen.PlaceTile(i - 2, j, ModContent.TileType<WallGlow2x3NonsolidCoralR>());
                                                break;
                                        }
                                        break;
                                    }
                                    break;

                                #endregion Jellyfish Caverns

                                #region Bulbous Grove

                                case MinibiomeID.BulbousGrove: //Bulbous Grove(Round/circular/bulbous coral/plants)
                                    if (WorldGen.genRand.NextBool())
                                    {
                                        switch (TileCheck2(i, j))
                                        {
                                            case 1:
                                                selection = WorldGen.genRand.Next(3);
                                                switch (selection)
                                                {
                                                    case 0:
                                                        WorldGen.PlaceTile(i, j + 1, ModContent.TileType<HangingCoral7>());
                                                        break;

                                                    case 1:
                                                        WorldGen.PlaceTile(i, j + 1, ModContent.TileType<HangingGlow2x4Coral>());
                                                        break;

                                                    case 2:
                                                        WorldGen.PlaceTile(i, j + 1, ModContent.TileType<HangingGlow3x2Coral>());
                                                        break;
                                                }
                                                break;

                                            case 2:
                                                selection = WorldGen.genRand.Next(9);
                                                switch (selection)
                                                {
                                                    case 0:
                                                        WorldGen.PlaceTile(i, j - 2, ModContent.TileType<Floor2x2Coral>(), style: 3);
                                                        break;

                                                    case 1:
                                                        WorldGen.PlaceTile(i, j - 2, ModContent.TileType<FloorGlow2x2Coral1>());
                                                        break;

                                                    case 2:
                                                        WorldGen.PlaceTile(i, j - 2, ModContent.TileType<FloorGlow2x2Coral2>());
                                                        break;

                                                    case 3:
                                                        WorldGen.PlaceTile(i, j - 1, ModContent.TileType<Floor1x1Coral>(), style: 0);
                                                        break;

                                                    case 4:
                                                        WorldGen.PlaceTile(i, j - 3, ModContent.TileType<Floor8x3Coral>(), style: WorldGen.genRand.Next(2));
                                                        break;

                                                    case 5:
                                                        WorldGen.PlaceTile(i, j - 3, ModContent.TileType<Floor5x3Coral>());
                                                        break;

                                                    case 6:
                                                        WorldGen.PlaceTile(i, j - 3, ModContent.TileType<WideBulbousCoral>());
                                                        break;

                                                    case 7:
                                                        WorldGen.PlaceTile(i, j - 4, ModContent.TileType<FloorGlow4x4Coral>());
                                                        break;

                                                    case 8:
                                                        WorldGen.PlaceTile(i, j - 1, ModContent.TileType<FloorGlow2x1Coral>());
                                                        break;
                                                }
                                                break;
                                        }
                                    }
                                    break;

                                #endregion Bulbous Grove

                                #region Thermal Vents

                                case MinibiomeID.ThermalVents: //Thermal Vents(Thermal Vents-Thermal Vents and larger coral, more coral stacks)
                                    if (!WorldGen.genRand.NextBool(6))
                                    {
                                        if (WorldGen.genRand.NextBool())
                                        {
                                            WorldGen.PlaceTile(i, j - 3, ModContent.TileType<ThermalVent>());
                                        }
                                        else
                                        {
                                            switch (TileCheck2(i, j))
                                            {
                                                case 1:
                                                    selection = WorldGen.genRand.Next(5);
                                                    switch (selection)
                                                    {
                                                        case 0:
                                                            WorldGen.PlaceTile(i, j + 1, ModContent.TileType<Hanging1x2Coral>());
                                                            break;

                                                        case 1:
                                                            WorldGen.PlaceTile(i, j + 1, ModContent.TileType<Hanging1x3Coral>(), style: WorldGen.genRand.Next(2));
                                                            break;

                                                        case 2:
                                                            WorldGen.PlaceTile(i, j + 1, ModContent.TileType<Hanging2x3Coral>(), style: WorldGen.genRand.Next(2));
                                                            break;

                                                        case 3:
                                                            WorldGen.PlaceTile(i, j + 1, ModContent.TileType<Hanging2x4Coral>(), style: WorldGen.genRand.Next(3));
                                                            break;

                                                        case 4:
                                                            WorldGen.PlaceTile(i, j + 1, ModContent.TileType<Hanging1x4Coral>());
                                                            break;
                                                    }
                                                    break;

                                                case 2:
                                                    selection = WorldGen.genRand.Next(7);
                                                    switch (selection)
                                                    {
                                                        case 0:
                                                            WorldGen.PlaceTile(i, j - 2, ModContent.TileType<Floor1x2Coral>(), style: WorldGen.genRand.Next(7));
                                                            break;

                                                        case 1:
                                                            WorldGen.PlaceTile(i, j - 6, ModContent.TileType<Floor1x1Coral>(), style: WorldGen.genRand.Next(3));
                                                            break;

                                                        case 2:
                                                            WorldGen.PlaceTile(i, j - 8, ModContent.TileType<Floor2x1Coral>(), style: WorldGen.genRand.Next(5));
                                                            break;

                                                        case 3:
                                                            WorldGen.PlaceTile(i, j - 8, ModContent.TileType<Floor6x8Coral>());
                                                            break;

                                                        case 4:
                                                            WorldGen.PlaceTile(i, j - 8, ModContent.TileType<Floor8x8Coral>());
                                                            break;

                                                        case 5:
                                                            WorldGen.PlaceTile(i, j - 8, ModContent.TileType<Floor8x7Coral>());
                                                            break;

                                                        case 6:
                                                            WorldGen.PlaceTile(i, j - 8, ModContent.TileType<Floor7x7Coral>());
                                                            break;
                                                    }
                                                    break;
                                            }
                                        }
                                        break;
                                    }
                                    break;

                                #endregion Thermal Vents

                                #region Crystalline Caves

                                case MinibiomeID.CrystallineCaves: //Crystalline Caves(Thinner, taller coral, crystals)
                                    if (!WorldGen.genRand.NextBool(5))
                                    {
                                        if (WorldGen.genRand.NextBool(200) && Main.tile[i, j].active() && Main.tile[i, j].type != ModContent.TileType<AquamarineTile>())
                                        {
                                            MakeCrystal(i, j, WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(2, 4), WorldGen.genRand.NextBool().ToDirectionInt(), WorldGen.genRand.NextBool().ToDirectionInt(), ModContent.TileType<AquamarineTile>());
                                        }
                                        else
                                        {
                                            if (Main.tileSolid[Framing.GetTileSafely(i, j).type])
                                            {
                                                int width = 18;
                                                int height = 18;
                                                int widthOfLedge = 5;
                                                int heightOfLedge = 6;
                                                int Vert = -5;
                                                int Hori = -6;
                                                int check = 0;
                                                Vector2 TopLeft = new Vector2(i - Hori - width, j - height - Vert);
                                                byte[,,] array = EmptyTileArrays.LuminantCoralCrystalBigTopLeft;
                                                if (CheckRangeRight(i, j, widthOfLedge) && CheckRangeDown(i, j, heightOfLedge))
                                                {
                                                    for (int a = 0; a < array.GetLength(1); a++)
                                                    {
                                                        for (int b = 0; b < array.GetLength(0); b++)
                                                        {
                                                            if (array[b, a, 0] == 1)
                                                            {
                                                                if (Main.tileSolid[Framing.GetTileSafely((int)TopLeft.X + a, (int)TopLeft.Y + b).type] && Framing.GetTileSafely((int)TopLeft.X + a, (int)TopLeft.Y + b).active())
                                                                {
                                                                    check++;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (check <= 11)
                                                    {
                                                        EmptyTileEntityCache.AddPair(new BigCrystal(TopLeft, "Tiles/EmptyTileArrays/LuminantCoralCrystalBigTopLeft", "ShaderAssets/LuminantCoralCrystalBigTopLeftLightMap"), TopLeft, EmptyTileArrays.LuminantCoralCrystalBigTopLeft);
                                                        EESubWorlds.CoralCrystalPosition.Add(TopLeft);
                                                    }
                                                }
                                            }
                                            switch (TileCheck2(i, j))
                                            {
                                                case 0:
                                                case 3:
                                                case 4:
                                                    selection = WorldGen.genRand.Next(2);
                                                    switch (selection)
                                                    {
                                                        case 0:

                                                            break;
                                                    }
                                                    break;

                                                case 1:
                                                    selection = WorldGen.genRand.Next(7);
                                                    switch (selection)
                                                    {
                                                        case 0:
                                                            WorldGen.PlaceTile(i, j + 1, ModContent.TileType<Hanging1x2Coral>());
                                                            break;

                                                        case 1:
                                                            WorldGen.PlaceTile(i, j + 1, ModContent.TileType<Hanging1x3Coral>(), style: WorldGen.genRand.Next(2));
                                                            break;

                                                        case 2:
                                                            WorldGen.PlaceTile(i, j + 1, ModContent.TileType<Hanging2x4Coral>(), style: WorldGen.genRand.Next(3));
                                                            break;

                                                        case 3:
                                                            WorldGen.PlaceTile(i, j + 1, ModContent.TileType<Hanging1x4Coral>());
                                                            break;

                                                        case 4:
                                                            if (CheckRangeRight(i, j, 2))
                                                            {
                                                                if (!Framing.GetTileSafely(i, j + 1).active() && !Framing.GetTileSafely(i + 2, j + 4).active())
                                                                {
                                                                    EmptyTileEntityCache.AddPair(new Crystal(new Vector2(i, j + 1), "Tiles/EmptyTileArrays/LuminantCoralHang1", "ShaderAssets/CrystalLightMapHang2"), new Vector2(i, j + 1), EmptyTileArrays.LuminantCoralCrystalHang1);
                                                                    EESubWorlds.CoralCrystalPosition.Add(new Vector2(i, j + 1));
                                                                }
                                                            }
                                                            break;

                                                        case 5:
                                                            if (CheckRangeRight(i, j, 1))
                                                            {
                                                                if (!Framing.GetTileSafely(i, j + 1).active() && !Framing.GetTileSafely(i + 1, j + 4).active())
                                                                {
                                                                    EmptyTileEntityCache.AddPair(new Crystal(new Vector2(i, j + 1), "Tiles/EmptyTileArrays/LuminantCoralHang2", "ShaderAssets/CrystalLightMapHang2"), new Vector2(i, j + 1), EmptyTileArrays.LuminantCoralCrystalHang2);
                                                                    EESubWorlds.CoralCrystalPosition.Add(new Vector2(i, j + 1));
                                                                }
                                                            }
                                                            break;

                                                        case 6:
                                                            if (CheckRangeRight(i, j, 1))
                                                            {
                                                                if (!Framing.GetTileSafely(i, j + 1).active() && !Framing.GetTileSafely(i + 1, j + 1).active())
                                                                {
                                                                    EmptyTileEntityCache.AddPair(new Crystal(new Vector2(i, j + 1), "Tiles/EmptyTileArrays/LuminantCoralHang3", "ShaderAssets/CrystalLightMapHang3"), new Vector2(i, j + 1), EmptyTileArrays.LuminantCoralCrystalHang3);
                                                                    EESubWorlds.CoralCrystalPosition.Add(new Vector2(i, j + 1));
                                                                }
                                                            }
                                                            break;
                                                    }
                                                    break;

                                                case 2:
                                                    selection = WorldGen.genRand.Next(9);
                                                    switch (selection)
                                                    {
                                                        case 0:
                                                            WorldGen.PlaceTile(i, j - 2, ModContent.TileType<Floor1x2Coral>(), style: WorldGen.genRand.Next(7));
                                                            break;

                                                        case 1:
                                                            WorldGen.PlaceTile(i, j - 8, ModContent.TileType<Floor2x6Coral>(), style: WorldGen.genRand.Next(2));
                                                            break;

                                                        case 2:
                                                            WorldGen.PlaceTile(i, j - 8, ModContent.TileType<Floor6x8Coral>());
                                                            break;

                                                        case 3:
                                                            if (CheckRangeRight(i, j, 4))
                                                            {
                                                                if (!Framing.GetTileSafely(i, j - 7).active() && !Framing.GetTileSafely(i + 4, j - 1).active())
                                                                {
                                                                    EmptyTileEntityCache.AddPair(new Crystal(new Vector2(i, j - 7), "Tiles/EmptyTileArrays/CoralCrystal", "ShaderAssets/CrystalLightMapGround1"), new Vector2(i, j - 7), EmptyTileArrays.CoralCrystal);
                                                                    EESubWorlds.CoralCrystalPosition.Add(new Vector2(i, j - 7));
                                                                }
                                                            }
                                                            break;

                                                        case 4:
                                                            int width = 5;
                                                            int height = 7;
                                                            if (CheckRangeRight(i, j, width))
                                                            {
                                                                if (!Framing.GetTileSafely(i, j - height).active() && !Framing.GetTileSafely(i + width - 1, j - 1).active())
                                                                {
                                                                    EmptyTileEntityCache.AddPair(new Crystal(new Vector2(i, j - height), "Tiles/EmptyTileArrays/LuminantCoralGround1", "ShaderAssets/CrystalLightMapGround1"), new Vector2(i, j - height), EmptyTileArrays.LuminantCoralCrystalGround);
                                                                    EESubWorlds.CoralCrystalPosition.Add(new Vector2(i, j - height));
                                                                }
                                                            }
                                                            break;

                                                        case 5:
                                                            width = 2;
                                                            height = 2;
                                                            if (CheckRangeRight(i, j, width))
                                                            {
                                                                if (!Framing.GetTileSafely(i, j - height).active() && !Framing.GetTileSafely(i + width - 1, j - 1).active())
                                                                {
                                                                    EmptyTileEntityCache.AddPair(new Crystal(new Vector2(i, j - height), "Tiles/EmptyTileArrays/LuminantCoralGround3", "ShaderAssets/CrystalLightMapGround2"), new Vector2(i, j - height), EmptyTileArrays.LuminantCoralCrystalGround2);
                                                                    EESubWorlds.CoralCrystalPosition.Add(new Vector2(i, j - height));
                                                                }
                                                            }
                                                            break;

                                                        case 6:
                                                            width = 2;
                                                            height = 3;
                                                            if (CheckRangeRight(i, j, width))
                                                            {
                                                                if (!Framing.GetTileSafely(i, j - height).active() && !Framing.GetTileSafely(i + width - 1, j - 1).active())
                                                                {
                                                                    EmptyTileEntityCache.AddPair(new Crystal(new Vector2(i, j - height), "Tiles/EmptyTileArrays/LuminantCoralGround2", "ShaderAssets/CrystalLightMapGround3"), new Vector2(i, j - height), EmptyTileArrays.LuminantCoralCrystalGround3);
                                                                    EESubWorlds.CoralCrystalPosition.Add(new Vector2(i, j - height));
                                                                }
                                                            }
                                                            break;

                                                        case 7:
                                                            width = 4;
                                                            height = 3;
                                                            if (CheckRangeRight(i, j, width))
                                                            {
                                                                if (!Framing.GetTileSafely(i, j - height).active() && !Framing.GetTileSafely(i + width - 1, j - 1).active())
                                                                {
                                                                    EmptyTileEntityCache.AddPair(new Crystal(new Vector2(i, j - height), "Tiles/EmptyTileArrays/LuminantCoralGround4", "ShaderAssets/CrystalLightMapGround4"), new Vector2(i, j - height), EmptyTileArrays.LuminantCoralCrystalGround4);
                                                                    EESubWorlds.CoralCrystalPosition.Add(new Vector2(i, j - height));
                                                                }
                                                            }
                                                            break;
                                                    }
                                                    break;
                                            }
                                        }
                                        break;
                                    }
                                    break;

                                    #endregion Crystalline Caves
                            }
                        }
                    }
                }
            }
        }
    }
}