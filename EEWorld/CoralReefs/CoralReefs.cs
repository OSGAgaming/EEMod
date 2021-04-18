﻿using EEMod.ID;
using EEMod.Tiles;
using EEMod.Tiles.EmptyTileArrays;
using EEMod.Tiles.Foliage;
using EEMod.Tiles.Foliage.Coral;
using EEMod.Tiles.Foliage.Coral.HangingCoral;
using EEMod.Tiles.Foliage.Coral.WallCoral;
using EEMod.Tiles.Ores;
using EEMod.Tiles.Walls;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using EEMod.Tiles.Furniture;
using EEMod.Tiles.Foliage.ThermalVents;
using EEMod.Tiles.Foliage.KelpForest;
using EEMod.Tiles.Foliage.Aquamarine;

namespace EEMod.EEWorld
{
    public partial class EEWorld
    {
        public static PerlinNoiseFunction perlinNoise;

        public static bool GemsandCheck(int i, int j)
        {
            int ecksdee = Framing.GetTileSafely(i, j).type;
            return ecksdee == ModContent.TileType<LightGemsandTile>() || ecksdee == ModContent.TileType<LightGemsandstoneTile>() || ecksdee == ModContent.TileType<GemsandTile>() || ecksdee == ModContent.TileType<GemsandstoneTile>() || ecksdee == ModContent.TileType<DarkGemsandTile>() || ecksdee == ModContent.TileType<DarkGemsandstoneTile>();
        }

        internal static void PlaceWallGrass()
        {
            /*for (int i = 10; i < Main.maxTilesX - 10; i++)
            {
                for (int j = 10; j < Main.maxTilesY - 10; j++)
                {
                    int X = i;
                    int Y = j;
                    switch (TileCheck2(X, Y))
                    {
                        case (int)TileSpacing.Top:
                        {
                            for (int a = 0; a < WorldGen.genRand.Next(11); a++)
                                WorldGen.PlaceWall(X, Y - a, ModContent.WallType<KelpForestLeafyWall>());
                            break;
                        }
                        case (int)TileSpacing.Bottom:
                        {
                            for (int a = 0; a < WorldGen.genRand.Next(11); a++)
                                WorldGen.PlaceWall(X, Y + a, ModContent.WallType<KelpForestLeafyWall>());
                            break;
                        }
                        case (int)TileSpacing.Left:
                        {
                            for (int a = 0; a < WorldGen.genRand.Next(11); a++)
                                WorldGen.PlaceWall(X - a, Y, ModContent.WallType<KelpForestLeafyWall>());
                            break;
                        }
                        case (int)TileSpacing.Right:
                        {
                            for (int a = 0; a < WorldGen.genRand.Next(11); a++)
                                WorldGen.PlaceWall(X + a, Y, ModContent.WallType<KelpForestLeafyWall>());
                            break;
                        }
                    }
                }
            }*/
        }

        public static void MakeCoralRoom(int xPos, int yPos, int size, int type, bool ensureNoise = false)
        {
            int sizeX = size;
            int sizeY = size / 2;
            Vector2 TL = new Vector2(xPos - sizeX / 2f, yPos - sizeY / 2f);
            Vector2 BR = new Vector2(xPos + sizeX / 2f, yPos + sizeY / 2f);

            Vector2 startingPoint = new Vector2(xPos - sizeX, yPos - sizeY);
            int tile2;
            tile2 = (ushort)GetGemsandType((int)TL.Y);
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
                                        tile.type = (ushort)GetGemsandType(j);
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
                    MakeJaggedOval(sizeX, sizeY, new Vector2(TL.X, TL.Y), TileID.StoneSlab, true, 100);
                    MakeOvalFlatTop(sizeX / 3, sizeY / 3, new Vector2(xPos + 0, yPos + 0), tile2);
                    MakeOvalFlatTop(sizeX / 3, sizeY / 3, new Vector2(xPos + (-sizeX / 5 - sizeX / 6), yPos + (-sizeY / 5 - sizeY / 6)), tile2);
                    MakeOvalFlatTop(sizeX / 3, sizeY / 3, new Vector2(xPos + (sizeX / 5 - sizeX / 6), yPos + (-sizeY / 5 - sizeY / 6)), tile2);
                    MakeOvalFlatTop(sizeX / 3, sizeY / 3, new Vector2(xPos + (sizeX / 5 - sizeX / 6), yPos + (sizeY / 5 - sizeY / 6)), tile2);
                    MakeOvalFlatTop(sizeX / 3, sizeY / 3, new Vector2(xPos + (-sizeX / 5 - sizeX / 6), yPos + (sizeY / 5 - sizeY / 6)), tile2);
                    for (int i = (int)startingPoint.X + 20; i < (int)startingPoint.X + sizeX * 2 - 20; i++)
                    {
                        for (int j = (int)startingPoint.Y + 20; j < (int)startingPoint.Y + sizeY * 2 - 20; j++)
                        {
                            int buffer = 0;
                            for (int a = 0; a < 20; a++)
                            {
                                if (Framing.GetTileSafely(i, j - a).active())
                                {
                                    buffer++;
                                }
                            }
                            if (buffer < 17 && buffer > 3)
                            {
                                if (TileCheck2(i, j) == 1 && TileCheckVertical(i, j + 1, 1) - (j + 1) <= 50)
                                {
                                    for (int a = 0; a < TileCheckVertical(i, j + 1, 1) - (j + 1); a++)
                                    {
                                        if (Main.rand.Next(2) == 1)
                                        {
                                            WorldGen.PlaceWall(i, j + a, ModContent.WallType<GemsandstoneWallTile>());
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;

                case 0:
                    MakeJaggedOval(sizeX, sizeY, new Vector2(TL.X, TL.Y), TileID.StoneSlab, true, 100);
                    MakeJaggedOval(sizeX + 50, sizeY - 50, new Vector2(TL.X, TL.Y), TileID.StoneSlab, true, 100);
                    MakeJaggedOval(sizeX - 50, sizeY + 50, new Vector2(TL.X, TL.Y), TileID.StoneSlab, true, 100);
                    MakeOvalFlatTop(sizeX / 3, sizeY / 3, new Vector2(xPos + 0, yPos + 0), tile2);
                    MakeOvalFlatTop(sizeX / 3, sizeY / 3, new Vector2(xPos + (-sizeX / 5), yPos + (-sizeY / 5)), tile2);
                    MakeOvalFlatTop(sizeX / 3, sizeY / 3, new Vector2(xPos + (sizeX / 5), yPos + (-sizeY / 5)), tile2);
                    MakeOvalFlatTop(sizeX / 3, sizeY / 3, new Vector2(xPos + (sizeX / 5), yPos + (sizeY / 5)), tile2);
                    MakeOvalFlatTop(sizeX / 3, sizeY / 3, new Vector2(xPos + (-sizeX / 5), yPos + (sizeY / 5)), tile2);
                    CreateNoise(!ensureNoise, Main.rand.Next(20, 50), Main.rand.Next(20, 50), 0.3f);
                    CreateNoise(!ensureNoise, Main.rand.Next(20, 50), Main.rand.Next(20, 50), 0.3f);
                    for (int i = (int)startingPoint.X + 20; i < (int)startingPoint.X + sizeX * 2 - 20; i++)
                    {
                        for (int j = (int)startingPoint.Y + 20; j < (int)startingPoint.Y + sizeY * 2 - 20; j++)
                        {
                            int buffer = 0;
                            for (int a = 0; a < 14; a++)
                            {
                                if (WorldGen.InWorld(i, j - a, 10))
                                    if (Framing.GetTileSafely(i, j - a).active())
                                    {
                                        buffer++;
                                    }
                            }
                            if (buffer < 7)
                            {
                                if (TileCheck2(i, j) == 1 && TileCheckVertical(i, j + 1, 1) - (j + 1) <= 50)
                                {
                                    for (int a = 0; a < 50; a++)
                                    {
                                        if (Main.rand.Next(4) == 1)
                                        {
                                            WorldGen.PlaceWall(i, j + a, ModContent.WallType<GemsandstoneWallTile>());
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;

                case (int)MinibiomeID.KelpForest: //A normally shaped room cut out with noise
                    KelpForest kelpForest = new KelpForest
                    {
                        Position = TL.ToPoint(),
                        Size = new Point(sizeX * 2, sizeY * 2),
                        EnsureNoise = ensureNoise
                    };
                    kelpForest.StructureStep();
                    break;


                case (int)MinibiomeID.GlowshroomGrotto: //One medium-sized open room completely covered in bulbous blocks
                    GlowshroomGrotto GlowshroomGrotto = new GlowshroomGrotto
                    {
                        Position = TL.ToPoint(),
                        Size = new Point(sizeX * 2, sizeY * 2),
                        EnsureNoise = ensureNoise
                    };
                    GlowshroomGrotto.StructureStep();
                    break;

                case (int)MinibiomeID.ThermalVents: //A wide-open room with floating platforms that hold abandoned ashen houses with huge chasms in between
                    MakeJaggedOval(sizeX, (int)(sizeY * 1.5f), new Vector2(TL.X, yPos - sizeY), TileID.StoneSlab, true, 50);

                    for (int i = 0; i < 30; i++)
                    {
                        MakeCircle(WorldGen.genRand.Next(5, 20), new Vector2(TL.X + WorldGen.genRand.Next(sizeX), yPos - sizeY + WorldGen.genRand.Next(sizeY * 2)), TileID.StoneSlab, true);
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        MakeOvalJaggedTop(WorldGen.genRand.Next(25, 50), WorldGen.genRand.Next(10, 30), new Vector2(TL.X + WorldGen.genRand.Next((int)(sizeX * 0.25f), (int)(sizeX * 0.75f)), TL.X + WorldGen.genRand.Next((int)(sizeY * 0.25f * 1.5f), (int)(sizeY * 0.75f * 1.5f))), ModContent.TileType<GemsandTile>());
                    }
                    break;

                case (int)MinibiomeID.CrystallineCaves: //Massive caves made with noise surrounding a central large room(where the spire is, if there's a spire)
                    MakeJaggedOval((int)(sizeX * 1.3f), sizeY, new Vector2(TL.X, TL.Y), TileID.StoneSlab, true, 50);
                    CreateNoise(true, 100, 20, 0.3f);
                    CreateNoise(true, 20, 100, 0.4f);
                    RemoveStoneSlabs();
                    for (int j = (int)startingPoint.Y; j < (int)startingPoint.Y + sizeY * 2; j++)
                    {
                        for (int i = (int)startingPoint.X; i < (int)startingPoint.X + sizeX * 2; i++)
                        {
                            //if ((TileCheck2(i, j) == 3 || TileCheck2(i, j) == 4) && Main.rand.Next(8) == 1)
                            if ((TileCheck2(i, j) != 0) && Main.rand.NextBool(10))
                            {
                                if (EESubWorlds.AquamarineZiplineLocations.Count == 0)
                                {
                                    EESubWorlds.AquamarineZiplineLocations.Add(new Vector2(i, j));
                                }
                                else
                                {
                                    Vector2 lastPos = EESubWorlds.AquamarineZiplineLocations[EESubWorlds.AquamarineZiplineLocations.Count - 1];
                                    if ((Vector2.DistanceSquared(lastPos, new Vector2(i, j)) > 10 * 10 && Vector2.DistanceSquared(lastPos, new Vector2(i, j)) < 210 * 210) || Vector2.DistanceSquared(lastPos, new Vector2(i, j)) > 200 * 200)
                                    {
                                        EESubWorlds.AquamarineZiplineLocations.Add(new Vector2(i, j));
                                    }
                                }
                            }
                        }
                    }
                    for (int i = (int)startingPoint.X; i < (int)startingPoint.X + sizeX * 2; i++)
                    {
                        for (int j = (int)startingPoint.Y; j < (int)startingPoint.Y + sizeY * 2; j++)
                           {
                            //if ((TileCheck2(i, j) == 3 || TileCheck2(i, j) == 4) && Main.rand.Next(8) == 1)
                            if ((TileCheck2(i, j) != 0) && Main.rand.NextBool(10))
                            {
                                if (EESubWorlds.AquamarineZiplineLocations.Count == 0)
                                {
                                    EESubWorlds.AquamarineZiplineLocations.Add(new Vector2(i, j));
                                }
                                else
                                {
                                    Vector2 lastPos = EESubWorlds.AquamarineZiplineLocations[EESubWorlds.AquamarineZiplineLocations.Count - 1];
                                    if ((Vector2.DistanceSquared(lastPos, new Vector2(i, j)) > 10 * 10 && Vector2.DistanceSquared(lastPos, new Vector2(i, j)) < 210 * 210) || Vector2.DistanceSquared(lastPos, new Vector2(i, j)) > 200 * 200)
                                    {
                                        EESubWorlds.AquamarineZiplineLocations.Add(new Vector2(i, j));
                                    }
                                }
                            }
                        }
                    }
                    EESubWorlds.SpirePosition = new Vector2(xPos, yPos);
                    break;
            }
            CreateNoise(ensureNoise, Main.rand.Next(30, 50), Main.rand.Next(20, 40), Main.rand.NextFloat(0.3f, 0.6f));
        }

        public static void MakeCrystal(int xPos, int yPos, int length, int width, int vertDir, int horDir, int type)
        {
            for (int a = 0; a < length; a++)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (!Framing.GetTileSafely(i, j).active())
                        {
                            WorldGen.TileRunner(i + xPos + (a * horDir), j + yPos + (a * vertDir), Main.rand.Next(2, 3), Main.rand.Next(1, 2), type, true, 0, 0, false, false);
                        }
                    }
                }
            }
        }

        public static int GetGemsandType(int height)
        {
            if (height < Main.maxTilesY * 0.4f)
                return ModContent.TileType<LightGemsandTile>();
            else if (height < Main.maxTilesY * 0.8f)
                return ModContent.TileType<GemsandTile>();
            else if (height > Main.maxTilesY * 0.8f)
                return ModContent.TileType<DarkGemsandTile>();
            if (height < Main.maxTilesY / 20)
                return ModContent.TileType<CoralSandTile>();
            else
                return 0;
        }

        public static void MakeCoralStack()
        {

        }

        public static void PlaceCoral()
        {
            #region Surface Reefs

            TilePopulate(
            new int[] { ModContent.TileType<Hanging1x2Coral>(),
            ModContent.TileType<Hanging1x3Coral>(),
            ModContent.TileType<Hanging2x3Coral>(),
            ModContent.TileType<Hanging2x4Coral>(),
            ModContent.TileType<Hanging1x4Coral>(),

            ModContent.TileType<Floor1x1Coral>(),
            ModContent.TileType<Floor1x2Coral>(),
            ModContent.TileType<Floor2x1Coral>(),
            ModContent.TileType<Floor2x2Coral>(),
            ModContent.TileType<FloorGlow2x2Coral>(),
            ModContent.TileType<Floor2x6Coral>(),
            ModContent.TileType<Floor3x2Coral>(),
            ModContent.TileType<Floor3x3Coral>(),
            ModContent.TileType<Floor4x3Coral>(),
            ModContent.TileType<Floor7x7Coral>(),
            ModContent.TileType<Floor8x7Coral>(),
            ModContent.TileType<Floor8x3Coral>(),
            ModContent.TileType<FloorGlow9x4Coral>(),
            ModContent.TileType<Floor9x9Coral>(),
            ModContent.TileType<Floor11x11Coral>(),

            ModContent.TileType<Wall2x2CoralL>(),
            ModContent.TileType<Wall3x2CoralL>(),
            ModContent.TileType<Wall3x2NonsolidCoralL>(),
            ModContent.TileType<Wall5x2NonsolidCoralL>(),
            ModContent.TileType<Wall6x3CoralL>(),

            ModContent.TileType<Wall2x2CoralR>(),
            ModContent.TileType<Wall3x2CoralR>(),
            ModContent.TileType<Wall3x2NonsolidCoralR>(),
            ModContent.TileType<Wall5x2NonsolidCoralR>(),
            ModContent.TileType<Wall6x3CoralR>() },
            new Rectangle(42, 42, Main.maxTilesX - 84, 240));

            #endregion Surface Reefs

            /*for (int i = 42; i < Main.maxTilesX - 42; i++)
            {
                for (int j = 42; j < Main.maxTilesY - 42; j++)
                {
                    if (WorldGen.InWorld(i, j) && GemsandCheck(i, j))
                    {
                        int minibiome = 0;
                        List<float> BufferLengths = new List<float>();
                        List<int> BufferMinibiome = new List<int>();
                        for (int k = 0; k < EESubWorlds.MinibiomeLocations.Count; k++)
                        {
                            if (Vector2.DistanceSquared(new Vector2(EESubWorlds.MinibiomeLocations[k].X, EESubWorlds.MinibiomeLocations[k].Y), new Vector2(i, j)) < (180 * 180) && EESubWorlds.MinibiomeLocations[k].Z != 0)
                            {
                                BufferLengths.Add(Vector2.DistanceSquared(new Vector2(EESubWorlds.MinibiomeLocations[k].X, EESubWorlds.MinibiomeLocations[k].Y), new Vector2(i, j)));
                                BufferMinibiome.Add((int)EESubWorlds.MinibiomeLocations[k].Z);
                            }
                        }

                        float MakingMyWayDownTown = -1;
                        int WalkingFast = -1;
                        for (int a = 0; a < BufferLengths.Count; a++)
                        {
                            if (BufferLengths[a] < MakingMyWayDownTown || MakingMyWayDownTown == -1)
                            {
                                MakingMyWayDownTown = BufferLengths[a];
                                WalkingFast = BufferMinibiome[a];
                            }
                        }
                        if (WalkingFast != -1) minibiome = WalkingFast;
                        int selection;
                        switch ((MinibiomeID)minibiome)
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
                                                    WorldGen.PlaceTile(i, j + 1, ModContent.TileType<GlowHangCoral2>());
                                                    break;

                                                case 5:
                                                    WorldGen.PlaceTile(i, j + 1, ModContent.TileType<Hanging1x4Coral>());
                                                    break;
                                            }
                                            break;

                                        case 2:
                                        {
                                            selection = WorldGen.genRand.Next(15);
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
                                                    WorldGen.PlaceTile(i, j - 2, ModContent.TileType<FloorGlow2x2Coral>(), style: WorldGen.genRand.Next(3));
                                                    break;

                                                case 13:
                                                    WorldGen.PlaceTile(i, j - 1, ModContent.TileType<Floor2x1Coral>(), style: WorldGen.genRand.Next(5));
                                                    break;

                                                case 14:
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
                                                    WorldGen.PlaceTile(i + 1, j, ModContent.TileType<Wall4x2CoralL>(), style: WorldGen.genRand.Next(3));
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
                                                    WorldGen.PlaceTile(i + 1, j, ModContent.TileType<Wall6x3CoralL>());
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
                                                    WorldGen.PlaceTile(i - 4, j, ModContent.TileType<Wall4x2CoralR>(), style: WorldGen.genRand.Next(3));
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
                                                    WorldGen.PlaceTile(i - 5, j, ModContent.TileType<Wall6x3CoralR>());
                                                    break;
                                            }
                                            break;
                                    }
                                }
                                break;

                            #endregion Default

                            #region Bulbous Grove

                            case MinibiomeID.GlowshroomGrotto:
                                if (WorldGen.genRand.NextBool())
                                {
                                    switch (TileCheck2(i, j))
                                    {
                                        case 1:
                                            selection = WorldGen.genRand.Next(3);
                                            switch (selection)
                                            {
                                                case 0:
                                                    WorldGen.PlaceTile(i, j + 1, ModContent.TileType<KelpberryPlaced>());
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
                                            if (WorldGen.genRand.NextBool(20))
                                                WorldGen.TileRunner(i, j, WorldGen.genRand.Next(2, 10), 1, ModContent.TileType<BulbousBlockTile>(), true);
                                            /*selection = WorldGen.genRand.Next(9);
                                            switch (selection)
                                            {
                                                case 0:
                                                    WorldGen.PlaceTile(i, j - 2, ModContent.TileType<Floor2x2Coral>(), style: 3);
                                                    break;

                                                case 1:
                                                    WorldGen.PlaceTile(i, j - 2, ModContent.TileType<FloorGlow2x2Coral1>());
                                                    break;

                                                case 2:ewdGen.PlaceTile(i, j - 2, ModContent.TileType<FloorGlow2x2Coral2>());
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
                                                    WorldGen.PlaceTile(i, j + 1, ModContent.TileType<Hanging1x4Coral>());
                                                    break;

                                                case 5:
                                                    int helloFutureProgrammersGetDabbedOn = WorldGen.genRand.Next(3, 6);
                                                    MakeTriangle(new Vector2(i, j), helloFutureProgrammersGetDabbedOn, helloFutureProgrammersGetDabbedOn * 3, 3, ModContent.TileType<ScorchedGemsandTile>(), -1, false, 1);
                                                    break;
                                            }
                                            break;

                                        case 2:
                                            /*selection = WorldGen.genRand.Next(6);
                                            switch (selection)
                                            {
                                                case 0:
                                                    WorldGen.PlaceTile(i, j - 1, ModContent.TileType<ThermalVent1x1>(), style: WorldGen.genRand.Next(2));
                                                    break;

                                                case 1:
                                                    WorldGen.PlaceTile(i, j - 2, ModContent.TileType<ThermalVent1x2>(), style: WorldGen.genRand.Next(2));
                                                    break;

                                                case 2:
                                                    WorldGen.PlaceTile(i, j - 3, ModContent.TileType<ThermalVent1x3>(), style: WorldGen.genRand.Next(2));
                                                    break;

                                                case 3:
                                                    WorldGen.PlaceTile(i, j - 1, ModContent.TileType<ThermalVent2x1>());
                                                    break;

                                                case 4:
                                                    WorldGen.PlaceTile(i, j - 2, ModContent.TileType<ThermalVent2x2>(), style: WorldGen.genRand.Next(2));
                                                    break;

                                                case 5:
                                                    int helloFutureProgrammersGetDabbedOn = WorldGen.genRand.Next(3, 6);
                                                    MakeTriangle(new Vector2(i, j), helloFutureProgrammersGetDabbedOn, helloFutureProgrammersGetDabbedOn * 4, 3, ModContent.TileType<ScorchedGemsandTile>(), -1, true, 1);
                                                    break;
                                            }
                                            break;
                                    }
                                }
                                break;

                            #endregion Thermal Vents

                            #region Crystalline Caves

                            case MinibiomeID.CrystallineCaves:
                                if (!WorldGen.genRand.NextBool(5))
                                {
                                    if (WorldGen.genRand.NextBool(200) && Framing.GetTileSafely(i, j).active() && Framing.GetTileSafely(i, j).type != ModContent.TileType<AquamarineTile>())
                                    {
                                        MakeCrystal(i, j, WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(2, 5), WorldGen.genRand.NextBool().ToDirectionInt(), WorldGen.genRand.NextBool().ToDirectionInt(), ModContent.TileType<AquamarineTile>());
                                    }
                                    else
                                    {
                                        if (Main.tileSolid[Framing.GetTileSafely(i, j).type])
                                        {
                                            #region spawning nomis crystal

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
                                                    if (!Framing.GetTileSafely((int)TopLeft.X, (int)TopLeft.Y).active() && !Framing.GetTileSafely((int)TopLeft.X + width + Vert, (int)TopLeft.Y + height + Hori).active())
                                                    {
                                                        EmptyTileEntities.Instance.AddPair(new BigCrystal(TopLeft, "Tiles/EmptyTileArrays/LuminantCoralCrystalBigTopLeft", "ShaderAssets/LuminantCoralCrystalBigTopLeftLightMap"), TopLeft, EmptyTileArrays.LuminantCoralCrystalBigTopLeft);
                                                    }
                                                    EESubWorlds.CoralCrystalPosition.Add(TopLeft);
                                                }
                                            }

                                            #endregion spawning nomis crystal

                                            if (!Framing.GetTileSafely(i, j - 1).active())
                                            {
                                                if (!Framing.GetTileSafely(i + 1, j).active())
                                                {
                                                    ETAHelpers.PlaceCrystal(ETAHelpers.ETAAnchor.BottomLeft, new Vector2(i, j), EmptyTileArrays.LuminantCoralCrystalDiagTopRight1, "Tiles/EmptyTileArrays/LuminantCoralCrystalDiagTopRight1", "ShaderAssets/CrystalLightMapDiagTopRight1");
                                                }
                                                if (Framing.GetTileSafely(i - 1, j).active())
                                                {
                                                }
                                            }
                                        }
                                        switch (TileCheck2(i, j))
                                        {
                                            case 0:
                                                break;

                                            case 1:
                                                selection = WorldGen.genRand.Next(3);
                                                switch (selection)
                                                {
                                                    case 1:
                                                        ETAHelpers.PlaceCrystal(ETAHelpers.ETAAnchor.Top, new Vector2(i, j), EmptyTileArrays.LuminantCoralCrystalHang1, "Tiles/EmptyTileArrays/LuminantCoralHang1", "ShaderAssets/CrystalLightMapHang1");
                                                        break;

                                                    case 2:
                                                        ETAHelpers.PlaceCrystal(ETAHelpers.ETAAnchor.Top, new Vector2(i, j), EmptyTileArrays.LuminantCoralCrystalHang2, "Tiles/EmptyTileArrays/LuminantCoralHang2", "ShaderAssets/CrystalLightMapHang2");
                                                        break;

                                                    case 3:
                                                        ETAHelpers.PlaceCrystal(ETAHelpers.ETAAnchor.Top, new Vector2(i, j), EmptyTileArrays.LuminantCoralCrystalHang3, "Tiles/EmptyTileArrays/LuminantCoralHang3", "ShaderAssets/CrystalLightMapHang3");
                                                        break;
                                                }
                                                break;

                                            case 2:
                                                selection = WorldGen.genRand.Next(8);
                                                switch (selection)
                                                {
                                                    case 0:
                                                        ETAHelpers.PlaceCrystal(ETAHelpers.ETAAnchor.Bottom, new Vector2(i, j), EmptyTileArrays.LuminantCoralCrystalGround1, "Tiles/EmptyTileArrays/LuminantCoralGround1", "ShaderAssets/CrystalLightMapGround1");
                                                        break;

                                                    case 1:
                                                        ETAHelpers.PlaceCrystal(ETAHelpers.ETAAnchor.Bottom, new Vector2(i, j), EmptyTileArrays.LuminantCoralCrystalGround2, "Tiles/EmptyTileArrays/LuminantCoralGround2", "ShaderAssets/CrystalLightMapGround2");
                                                        break;

                                                    case 2:
                                                        ETAHelpers.PlaceCrystal(ETAHelpers.ETAAnchor.Bottom, new Vector2(i, j), EmptyTileArrays.LuminantCoralCrystalGround3, "Tiles/EmptyTileArrays/LuminantCoralGround3", "ShaderAssets/CrystalLightMapGround3");
                                                        break;

                                                    case 3:
                                                        ETAHelpers.PlaceCrystal(ETAHelpers.ETAAnchor.Bottom, new Vector2(i, j), EmptyTileArrays.LuminantCoralCrystalGround4, "Tiles/EmptyTileArrays/LuminantCoralGround4", "ShaderAssets/CrystalLightMapGround4");
                                                        break;

                                                    case 4:
                                                        ETAHelpers.PlaceCrystal(ETAHelpers.ETAAnchor.Bottom, new Vector2(i, j), EmptyTileArrays.LuminantCoralCrystalGround5, "Tiles/EmptyTileArrays/LuminantCoralGround5", "ShaderAssets/CrystalLightMapGround5");
                                                        break;

                                                    case 5:
                                                        ETAHelpers.PlaceCrystal(ETAHelpers.ETAAnchor.Bottom, new Vector2(i, j), EmptyTileArrays.LuminantCoralCrystalGround6, "Tiles/EmptyTileArrays/LuminantCoralGround6", "ShaderAssets/CrystalLightMapGround6");
                                                        break;

                                                    case 6:
                                                        ETAHelpers.PlaceCrystal(ETAHelpers.ETAAnchor.Bottom, new Vector2(i, j), EmptyTileArrays.LuminantCoralCrystalGround7, "Tiles/EmptyTileArrays/LuminantCoralGround7", "ShaderAssets/CrystalLightMapGround7");
                                                        break;

                                                    case 7:
                                                        WorldGen.PlaceTile(i, j - 3, ModContent.TileType<AquamarineLamp1>());
                                                        break;
                                                }
                                                break;

                                            case 3:
                                                selection = WorldGen.genRand.Next(3);
                                                switch (selection)
                                                {
                                                    case 1:
                                                        ETAHelpers.PlaceCrystal(ETAHelpers.ETAAnchor.Left, new Vector2(i, j), EmptyTileArrays.LuminantCoralCrystalHang1, "Tiles/EmptyTileArrays/LuminantCoralSideLeft1", "ShaderAssets/CrystalLightMapWallLeft1");
                                                        break;

                                                    case 2:
                                                        ETAHelpers.PlaceCrystal(ETAHelpers.ETAAnchor.Left, new Vector2(i, j), EmptyTileArrays.LuminantCoralCrystalHang2, "Tiles/EmptyTileArrays/LuminantCoralSideLeft2", "ShaderAssets/CrystalLightMapWallLeft2");
                                                        break;

                                                    case 3:
                                                        ETAHelpers.PlaceCrystal(ETAHelpers.ETAAnchor.Left, new Vector2(i, j), EmptyTileArrays.LuminantCoralCrystalHang3, "Tiles/EmptyTileArrays/LuminantCoralSideLeft3", "ShaderAssets/CrystalLightMapWallLeft3");
                                                        break;
                                                }
                                                break;

                                            case 4:
                                                selection = WorldGen.genRand.Next(3);
                                                switch (selection)
                                                {
                                                    case 1:
                                                        ETAHelpers.PlaceCrystal(ETAHelpers.ETAAnchor.Right, new Vector2(i, j), EmptyTileArrays.LuminantCoralCrystalHang1, "Tiles/EmptyTileArrays/LuminantCoralSideRight1", "ShaderAssets/CrystalLightMapWallRight1");
                                                        break;

                                                    case 2:
                                                        ETAHelpers.PlaceCrystal(ETAHelpers.ETAAnchor.Right, new Vector2(i, j), EmptyTileArrays.LuminantCoralCrystalHang2, "Tiles/EmptyTileArrays/LuminantCoralSideRight2", "ShaderAssets/CrystalLightMapWallRight2");
                                                        break;

                                                    case 3:
                                                        ETAHelpers.PlaceCrystal(ETAHelpers.ETAAnchor.Right, new Vector2(i, j), EmptyTileArrays.LuminantCoralCrystalHang3, "Tiles/EmptyTileArrays/LuminantCoralSideRight3", "ShaderAssets/CrystalLightMapWallRight3");
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
            }*/
        }
    }
}