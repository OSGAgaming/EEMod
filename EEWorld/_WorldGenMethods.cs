﻿using EEMod.ID;
using EEMod.Tiles;
using EEMod.Tiles.Furniture;
using EEMod.Tiles.Ores;
using EEMod.Tiles.Walls;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;
using System.Diagnostics;
//using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using Terraria.DataStructures;
using EEMod.Tiles.EmptyTileArrays;
using System.Linq;
using EEMod.VerletIntegration;
using EEMod.Tiles.Furniture.Chests;
using EEMod.Tiles.Foliage.Coral;
using Terraria.ObjectData;
using Microsoft.Xna.Framework.Graphics;
using EEMod.Extensions;
using EEMod.Systems.Noise;
using EEMod.Systems;
using EEMod.Tiles.Foliage;

namespace EEMod.EEWorld
{
    public enum TileSpacing
    {
        None,
        Bottom,
        Top,
        Right,
        Left
    }

    public partial class EEWorld
    {
        public static void FillRegionNoEdit(int width, int height, Vector2 startingPoint, int type)
        {
            string messageBefore = EEMod.progressMessage;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Tile tile = Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    tile.type = (ushort)type;
                    tile.IsActive = true;
                    EEMod.progressMessage = messageBefore;
                    EEMod.progressMessage += $" {(int)((j + (i * height)) / (float)(width * height) * 100)}% done";
                }
            }
            EEMod.progressMessage = messageBefore;
        }

        public static float[] PerlinArray(int width, int seedVar, float amplitude, Vector2 res)
        {
            PNF = new PerlinNoiseFunction(width, seedVar, (int)res.X, (int)res.Y, 0.5f, WorldGen.genRand);
            int rand = Main.rand.Next(0, seedVar);
            float[] PerlinStrip = new float[width];
            for (int i = 0; i < width; i++)
            {
                PerlinStrip[i] = PNF.perlin2[i, rand] * amplitude;
            }
            return PerlinStrip;
        }

        public static float[] PerlinArrayNoZero(int width, float amplitude, Vector2 res, int seedVar = 1000)
        {
            PNF = new PerlinNoiseFunction(width, seedVar, (int)res.X, (int)res.Y, 0.5f,WorldGen.genRand);
            int rand = Main.rand.Next(0, seedVar);
            float[] PerlinStrip = new float[width];
            for (int i = 0; i < width; i++)
            {
                PerlinStrip[i] = PNF.perlin[i, rand] * amplitude;
            }
            return PerlinStrip;
        }

        public static void CreateInvisibleTiles(byte[,,] array, Vector2 TilePosition)
        {
            for (int i = 0; i < array.GetLength(1); i++)
            {
                for (int j = 0; j < array.GetLength(0); j++)
                {
                    if (array[j, i, 0] == 1)
                    {
                        Tile tile = Framing.GetTileSafely(i + (int)TilePosition.X, j + (int)TilePosition.Y);
                        tile.type = (ushort)ModContent.TileType<EmptyTile>();
                        tile.Slope = (SlopeType)array[j, i, 1];
                        tile.IsActive = true;
                    }
                }
            }
        }

        public static void FillRegionNoEditWithNoise(int width, int height, Vector2 startingPoint, int type, int amplitude)
        {
            string messageBefore = EEMod.progressMessage;
            float[] PerlinStrip = PerlinArray(width, 1000, amplitude, new Vector2(60, 200));
            for (int i = 0; i < width; i++)
            {
                for (int j = (int)PerlinStrip[i]; j < height; j++)
                {
                    Tile tile = Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    tile.type = (ushort)type;
                    tile.IsActive = true;
                    EEMod.progressMessage = messageBefore;
                    EEMod.progressMessage += $" {(int)((j + (i * height)) / (float)(width * height) * 100)}% done";
                }
            }
            EEMod.progressMessage = messageBefore;
        }

        public static void FillRegionNoChangeWithNoise(int width, int height, Vector2 startingPoint, int type, int amplitude)
        {
            string messageBefore = EEMod.progressMessage;
            float[] PerlinStrip = PerlinArray(width, 1000, amplitude, new Vector2(60, 200));
            for (int i = 0; i < width; i++)
            {
                for (int j = (int)PerlinStrip[i]; j < height; j++)
                {
                    Tile tile = Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    if (tile.IsActive == false)
                    {
                        tile.type = (ushort)type;
                        tile.IsActive = true;
                        EEMod.progressMessage = messageBefore;
                        EEMod.progressMessage += $" {(int)((j + (i * height)) / (float)(width * height) * 100)}% done";
                    }
                }
            }
            EEMod.progressMessage = messageBefore;
        }

        public static void FillRegionEditWithNoise(int width, int height, Vector2 startingPoint, int type, int amplitude)
        {
            string messageBefore = EEMod.progressMessage;
            float[] PerlinStrip = PerlinArray(width, 1000, amplitude, new Vector2(60, 200));
            for (int i = 0; i < width; i++)
            {
                for (int j = (int)PerlinStrip[i]; j < height; j++)
                {
                    Tile tile = Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    if (tile.IsActive)
                    {
                        tile.type = (ushort)type;
                        EEMod.progressMessage = messageBefore;
                        EEMod.progressMessage += $" {(int)((j + (i * height)) / (float)(width * height) * 100)}% done";
                    }
                }
            }
            EEMod.progressMessage = messageBefore;
        }

        public static void FillWall(int width, int height, Vector2 startingPoint, int type)
        {
            string messageBefore = EEMod.progressMessage;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    WorldGen.PlaceWall(i + (int)startingPoint.X, j + (int)startingPoint.Y, type);
                    if (EEMod.isSaving)
                    {
                        EEMod.progressMessage = messageBefore;
                        EEMod.progressMessage += $" {(int)((j + (i * height)) / (float)(width * height) * 100)}% done";
                    }
                }
            }
        }

        public static void ClearOval(int width, int height, Vector2 startingPoint)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (OvalCheck((int)(startingPoint.X + width / 2), (int)(startingPoint.Y + height / 2), i + (int)startingPoint.X, j + (int)startingPoint.Y, (int)(width * .5f), (int)(height * .5f)))
                    {
                        WorldGen.KillTile(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    }

                    if (i == width / 2 && j == height / 2)
                    {
                        WorldGen.TileRunner(i + (int)startingPoint.X, j + (int)startingPoint.Y + 2, WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(10, 20), TileID.StoneSlab, true, 0f, 0f, true, true);
                    }
                }
            }
        }

        public static void MakeLavaPit(int width, int height, Vector2 startingPoint, float lavaLevel)
        {
            ClearOval(width, height, startingPoint);
            FillRegionWithLava(width, (int)(height * lavaLevel), new Vector2(startingPoint.X, startingPoint.Y + (int)(height - (height * lavaLevel))));
        }

        public static void PlaceShipyard(int x, int y)
        {
            EEMod eemood = ModContent.GetInstance<EEMod>();

            Structure.DeserializeFromBytes(eemood.GetFileBytes("EEWorld/Structures/Pier.lcs")).PlaceAt(x - 52, y, true, true);

            int x2 = x - 47;
            int y2 = y + 25;
            
            void DoTheThing()
            {
                while (Main.tile[x2, y2 - 3].LiquidAmount > 64 || Main.tile[x2, y2].LiquidAmount > 64)
                {
                    WorldGen.PlaceTile(x2, y2, TileID.LivingWood, false, true);
                    WorldGen.PlaceTile(x2 + 1, y2, TileID.LivingWood, false, true);

                    Main.tile[x2, y2].Slope = SlopeType.Solid;
                    Main.tile[x2 + 1, y2].Slope = SlopeType.Solid;

                    y2++;
                }
            }

            DoTheThing();

            x2 = x - 31;
            y2 = y + 25;

            DoTheThing();

            x2 = x - 15;
            y2 = y + 25;

            DoTheThing();

            Structure.DeserializeFromBytes(eemood.GetFileBytes("EEWorld/Structures/SailorHouse.lcs")).PlaceAt(x, y - 13, true, true);

            Structure.DeserializeFromBytes(eemood.GetFileBytes("EEWorld/Structures/ruinedboat.lcs")).PlaceAt(x - 108, y, true, true);

            shipCoords = new Vector2(x - 108, y);
        }

        public static int TileCheckWater(int positionX)
        {
            for (int i = 0; i < Main.maxTilesY; i++)
            {
                Tile tile = Framing.GetTileSafely(positionX, i);
                if (tile.LiquidAmount > 64)
                {
                    return i;
                }
            }
            return 0;
        }

        public static void ClearRegion(int width, int height, Vector2 startingPoint)
        {
            //string messageBefore = EEMod.progressMessage;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (WorldGen.InWorld(i + (int)startingPoint.X, j + (int)startingPoint.Y, 2))
                    {
                        Tile tile = Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y);

                        tile.ClearTile();
                        tile.wall = WallID.None;
                        tile.WallColor = PaintID.None;

                        //EEMod.progressMessage = messageBefore;
                        //EEMod.progressMessage += $" {(int)((j + (i * height)) / (float)(width * height) * 100)}% done";
                    }
                }
            }
        }

        public static Vector2 FindClosest(Vector2 pos, Vector2[] List)
        {
            Vector2 closest = Vector2.Zero;
            for (int i = 0; i < List.Length; i++)
            {
                if (closest == Vector2.Zero || Vector2.DistanceSquared(pos, List[i]) < Vector2.DistanceSquared(pos, closest) && Vector2.DistanceSquared(pos, List[i]) > 5)
                {
                    closest = List[i];
                }
            }
            return closest;
        }

        public static Vector2[] MakeDistantLocations(int number, float distance, Rectangle Bounds, int maxIterations = 100)
        {
            List<Vector2> Points = new List<Vector2>();
            for (int k = 0; k < number; k++)
            {
                Vector2 chosen = Vector2.Zero;
                if (Points.Count != 0)
                {
                    int count = -1;
                    int iterations = 0;

                    while ((count == -1 || count != 0) && iterations < maxIterations)
                    {
                        chosen = new Vector2(WorldGen.genRand.Next(Bounds.Left, Bounds.Right), WorldGen.genRand.Next(Bounds.Top, Bounds.Bottom));
                        count = 0;
                        for (int i = 0; i < Points.Count; i++)
                        {
                            if (Vector2.DistanceSquared(chosen, Points[i]) < distance * distance)
                            {
                                count++;
                            }
                        }
                        iterations++;
                    }
                    Points.Add(chosen);
                }
                else
                {
                    Points.Add(new Vector2(WorldGen.genRand.Next(Bounds.Left, Bounds.Right), WorldGen.genRand.Next(Bounds.Top, Bounds.Bottom)));
                }
            }
            return Points.ToArray();
        }

        public static void ClearRegionSafely(int width, int height, Vector2 startingPoint, int type)
        {
            string messageBefore = EEMod.progressMessage;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Tile tile = Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    if (tile.type == type && Main.tileSolid[tile.type])
                    {
                        WorldGen.KillTile(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                        //WorldGen.KillWall(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                        EEMod.progressMessage = messageBefore;
                        EEMod.progressMessage += $" {(int)((j + (i * height)) / (float)(width * height) * 100)}% done";
                    }
                }
            }
        }

        public static void FillRegionWithWater(int width, int height, Vector2 startingPoint)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y).LiquidType = 0;; // set liquid type 0 is water 1 lava 2 honey 3+ water iirc
                    Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y).LiquidAmount = 255; // set liquid ammount
                    WorldGen.SquareTileFrame(i + (int)startingPoint.X, j + (int)startingPoint.Y, true); // soemthing for astatic voiding the liquid from being static
                    if (Main.netMode == NetmodeID.MultiplayerClient) // sync
                    {
                        NetMessage.sendWater(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    }
                }
            }
        }

        public static void FillRegionWithLava(int width, int height, Vector2 startingPoint)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (WorldGen.InWorld(i + (int)startingPoint.X, j + (int)startingPoint.Y))
                    {
                        Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y).LiquidType = 1;; // set liquid type 0 is water 1 lava 2 honey 3+ water iirc
                        Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y).LiquidAmount = 255; // set liquid ammount
                        WorldGen.SquareTileFrame(i + (int)startingPoint.X, j + (int)startingPoint.Y, true); // soemthing for astatic voiding the liquid from being static
                        if (Main.netMode == NetmodeID.MultiplayerClient) // sync
                        {
                            NetMessage.sendWater(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                        }
                    }
                }
            }
        }

        public static void RemoveWaterFromRegion(int width, int height, Vector2 startingPoint)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y).LiquidType == 0 && Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y).LiquidAmount > 64)
                    {
                        Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y).ClearEverything();
                        if (Main.netMode == NetmodeID.MultiplayerClient) // sync
                        {
                            NetMessage.sendWater(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                        }
                    }
                }
            }
        }

        public static void RemoveWaterFromRegionWallsOnly(int width, int height, Vector2 startingPoint)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y).LiquidType == 0 
                        && Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y).LiquidAmount > 64 
                        && (Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y).wall != WallID.None 
                        || Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y).IsActive))
                    {
                        Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y).LiquidAmount = 0;
                        if (Main.netMode == NetmodeID.MultiplayerClient) // sync
                        {
                            NetMessage.sendWater(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                        }
                    }
                }
            }
        }

        public static int TileCheck2(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Tile tileBelow = Framing.GetTileSafely(i, j - 1);
            Tile tileBelow2 = Framing.GetTileSafely(i, j - 2);
            Tile tileAbove = Framing.GetTileSafely(i, j + 1);
            Tile tileAbove2 = Framing.GetTileSafely(i, j + 2);
            Tile TileLeft = Framing.GetTileSafely(i - 1, j);
            Tile tileLeft2 = Framing.GetTileSafely(i - 2, j);
            Tile tileRight = Framing.GetTileSafely(i + 1, j);
            Tile tileRight2 = Framing.GetTileSafely(i + 2, j);
            if (tile.IsActive && tileBelow.IsActive && tileBelow2.IsActive && !tileAbove.IsActive && !tileAbove2.IsActive && tile.Slope == 0) //If 2 tiles below are clear and 2 tiles above are solid
            {
                return 1;
            }
            if (tile.IsActive && !tileBelow.IsActive && !tileBelow2.IsActive && tileAbove.IsActive && tileAbove2.IsActive && tile.Slope == 0) //If 2 tiles below are solid and 2 tiles above are clear
            {
                return 2;
            }
            if (tile.IsActive && TileLeft.IsActive && tileLeft2.IsActive && !tileRight.IsActive && !tileRight2.IsActive) //If 2 tiles left are solid and 2 tiles right are clear
            {
                return 3;
            }
            if (tile.IsActive && !TileLeft.IsActive && !tileLeft2.IsActive && tileRight.IsActive && tileRight2.IsActive) //If 2 tiles right are solid and 2 tiles left are clear
            {
                return 4;
            }
            else
            {
                return 0;
            }
            if (WorldGen.InWorld(i, j, 4))
            {
                Tile tile1 = Framing.GetTileSafely(i, j);
                Tile tile2 = Framing.GetTileSafely(i, j - 1);
                Tile tile3 = Framing.GetTileSafely(i, j - 2);
                Tile tile4 = Framing.GetTileSafely(i, j + 1);
                Tile tile5 = Framing.GetTileSafely(i, j + 2);
                Tile tile6 = Framing.GetTileSafely(i - 1, j);
                Tile tile7 = Framing.GetTileSafely(i - 2, j);
                Tile tile8 = Framing.GetTileSafely(i + 1, j);
                Tile tile9 = Framing.GetTileSafely(i + 2, j);

                if (tile1.IsActive && tile2.IsActive && tile3.IsActive && !tile4.IsActive && !tile5.IsActive && tile1.Slope == 0)
                {
                    return 1;
                }
                if (tile1.IsActive && !tile2.IsActive && !tile3.IsActive && tile4.IsActive && tile5.IsActive && tile1.Slope == 0)
                {
                    return 2;
                }
                if (tile1.IsActive && tile6.IsActive && tile7.IsActive && !tile8.IsActive && !tile9.IsActive)
                {
                    return 3;
                }
                if (tile1.IsActive && !tile6.IsActive && !tile7.IsActive && tile8.IsActive && tile9.IsActive)
                {
                    return 4;
                }
                else
                {
                    return 0;
                }
            }
            return 0;
        }

        public static bool CheckRangeRight(int i, int j, int length, bool opposite = false)
        {
            for (int k = 0; k < length; k++)
            {
                if (WorldGen.InWorld(i + (opposite ? -k : k), j, 20))
                {
                    if (!Framing.GetTileSafely(i + (opposite ? -k : k), j).IsActive || !Main.tileSolid[Framing.GetTileSafely(i + (opposite ? -k : k), j).type] || Framing.GetTileSafely(i + (opposite ? -k : k), j).type == ModContent.TileType<EmptyTile>())
                        return false;
                }
            }

            return true;
        }

        public static bool CheckRangeDown(int i, int j, int length, bool opposite = false)
        {
            for (int k = 0; k < length; k++)
            {
                if (WorldGen.InWorld(i, j + (opposite ? -k : k), 20))
                {
                    if (!Framing.GetTileSafely(i, j + (opposite ? -k : k)).IsActive || !Main.tileSolid[Framing.GetTileSafely(i, j + (opposite ? -k : k)).type] || Framing.GetTileSafely(i + (opposite ? -k : k), j).type == ModContent.TileType<EmptyTile>())
                        return false;
                }
            }
            return true;
        }

        public static int WaterCheck(int i, int j)
        {
            Tile tile1 = Framing.GetTileSafely(i, j);
            Tile tileBelow = Framing.GetTileSafely(i, j - 1);
            Tile tileBelow2 = Framing.GetTileSafely(i, j - 2);
            Tile tileAbove = Framing.GetTileSafely(i, j + 1);
            Tile tileAbove2 = Framing.GetTileSafely(i, j + 2);
            Tile tileLeft = Framing.GetTileSafely(i - 1, j);
            Tile tileLeft2 = Framing.GetTileSafely(i - 2, j);
            Tile tileRight = Framing.GetTileSafely(i + 1, j);
            Tile tileRight2 = Framing.GetTileSafely(i + 2, j);
            bool IsSolid(Tile tile)
            {
                return tile.IsActive || Main.tileSolid[tile.type];
            }
            if (tile1.IsActive && tileBelow.IsActive && tileBelow2.IsActive && !tileAbove.IsActive && !tileAbove2.IsActive)
            {
                return 1;
            }
            if (tile1.IsActive && !IsSolid(tileBelow) && !IsSolid(tileBelow2) && tileAbove.IsActive && tileAbove2.IsActive)
            {
                return 2;
            }
            if (tile1.IsActive && tileLeft.IsActive && tileLeft2.IsActive && !tileRight.IsActive && !tileRight2.IsActive)
            {
                return 3;
            }
            if (tile1.IsActive && !tileLeft.IsActive && !tileLeft2.IsActive && tileRight.IsActive && tileRight2.IsActive)
            {
                return 4;
            }
            else
            {
                return 0;
            }
        }

        public static void MakeOvalJaggedTop(int width, int height, Vector2 startingPoint, int type, int lowRand = 10, int highRand = 20)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (OvalCheck((int)(startingPoint.X + width / 2), (int)(startingPoint.Y + height / 2), i + (int)startingPoint.X, j + (int)startingPoint.Y, (int)(width * .5f), (int)(height * .5f)))
                    {
                        WorldGen.PlaceTile(i + (int)startingPoint.X, j + (int)startingPoint.Y, type);
                    }

                    if (i == width / 2 && j == height / 2)
                    {
                        WorldGen.TileRunner(i + (int)startingPoint.X, j + (int)startingPoint.Y + 2, WorldGen.genRand.Next(lowRand, highRand), WorldGen.genRand.Next(lowRand, highRand), type, true, 0f, 0f, true, true);
                    }
                }
            }
            int steps = 0;
            for (int i = 0; i < width; i++)
            {
                if (Main.rand.NextBool(2))
                {
                    steps += Main.rand.Next(-1, 2);
                }

                for (int j = -6; j < height / 2 - 2 + steps; j++)
                {
                    Tile tile = Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    if (tile.type == type)
                    {
                        WorldGen.KillTile(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    }
                }
            }
        }

        public static void MakeIsland(int width, int height, Vector2 Middle, int type)
        {
            PerlinNoiseFunction PN = new PerlinNoiseFunction(width * 2, height * 2, 10, 10, 0.5f, WorldGen.genRand);
            for (int i = -width; i < width; i++)
            {
                for (int j = -height; j < height; j++)
                {
                    if (j > 0)
                    {
                        float Param = PN.perlin2[i + width, j + height] * 15;
                        if (OvalCheck((int)Middle.X, (int)Middle.Y, (int)Middle.X + i, (int)Middle.Y + j, width, height + (int)Param))
                        {
                            WorldGen.PlaceTile(i + (int)Middle.X, j + (int)Middle.Y, type, false, true);
                        }
                    }
                }
            }
        }

        public static void MakeOvalJaggedBottom(int width, int height, Vector2 startingPoint, int type, bool overwrite = false)
        {
            int steps = 0;
            for (int i = 0; i < width; i++)
            {
                if (Main.rand.NextBool(2))
                {
                    steps += Main.rand.Next(-1, 2);
                }

                for (int j = 0; j < height; j++)
                {
                    if (OvalCheck((int)(startingPoint.X + width / 2), (int)(startingPoint.Y + height / 2) + steps, i + (int)startingPoint.X, j + (int)startingPoint.Y, (int)(width * .5f), (int)(height * .5f)))
                    {
                        WorldGen.PlaceTile(i + (int)startingPoint.X, j + (int)startingPoint.Y, type);
                    }
                }
            }
            int steps2 = 0;
            for (int i = 0; i < width; i++)
            {
                if (Main.rand.NextBool(2))
                {
                    steps2 += Main.rand.Next(-1, 2);
                }

                for (int j = height / 2 - 2 + steps2; j < height + 12 + steps2; j++)
                {
                    Tile tile = Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    if (tile.type == type)
                    {
                        WorldGen.KillTile(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    }
                }
            }
        }

        public static void TilePopulate(int[] types, Rectangle bounds)
        {
            for (int i = bounds.X; i < bounds.Width; i++)
            {
                for (int j = bounds.Y; j < bounds.Height; j++)
                {
                    int chosen = WorldGen.genRand.Next(types.Length);
                    int tile = types[chosen];

                    TileObjectData TOD = TileObjectData.GetTileData(tile, 0);
                    if (TOD != null && tile != null)
                    {
                        if (TOD?.AnchorTop != AnchorData.Empty)
                        {
                            if (TileCheck2(i, j) == (int)TileSpacing.Bottom)
                            {
                                WorldGen.PlaceTile(i, j + 1, tile, default, default, default, Main.rand.Next(0, TOD.RandomStyleRange));
                                for (int a = 0; a < TOD.Width; a++)
                                    Framing.GetTileSafely(i + a, j).Slope = 0;
                            }
                        }
                        else if (TOD?.AnchorBottom != AnchorData.Empty)
                        {
                            if (TileCheck2(i, j) == (int)TileSpacing.Top)
                            {
                                WorldGen.PlaceTile(i, j - TOD.Height, tile, default, default, default, Main.rand.Next(0, TOD.RandomStyleRange));
                                for (int a = 0; a < TOD.Width; a++)
                                    Framing.GetTileSafely(i + a, j).Slope = 0;
                            }
                        }
                        else if (TOD?.AnchorLeft != AnchorData.Empty)
                        {
                            if (TileCheck2(i, j) == (int)TileSpacing.Right)
                            {
                                WorldGen.PlaceTile(i + 1, j, tile, default, default, default, Main.rand.Next(0, TOD.RandomStyleRange));
                            }
                        }
                        else if (TOD?.AnchorRight != AnchorData.Empty)
                        {
                            if (TileCheck2(i, j) == (int)TileSpacing.Left)
                            {
                                WorldGen.PlaceTile(i + TOD.Width, j, tile, default, default, default, Main.rand.Next(0, TOD.RandomStyleRange));
                            }
                        }
                    }
                }
            }
        }

        public static void TilePopulate(int[] types, Rectangle bounds, int chance)
        {
            for (int i = bounds.X; i < bounds.Width; i++)
            {
                for (int j = bounds.Y; j < bounds.Height; j++)
                {
                    if (WorldGen.genRand.NextBool(chance))
                    {
                        int chosen = WorldGen.genRand.Next(types.Length);
                        int tile = types[chosen];

                        TileObjectData TOD = TileObjectData.GetTileData(tile, 0);

                        if (TOD != null)
                        {
                            if (TOD.AnchorTop != AnchorData.Empty)
                            {
                                if (TileCheck2(i, j) == (int)TileSpacing.Bottom)
                                {
                                    WorldGen.PlaceTile(i, j + 1, tile, default, default, default, Main.rand.Next(0, TOD.RandomStyleRange));
                                    for (int a = 0; a < TOD.Width; a++)
                                        Framing.GetTileSafely(i + a, j).Slope = 0;
                                }
                            }
                            else if (TOD.AnchorBottom != AnchorData.Empty)
                            {
                                if (TileCheck2(i, j) == (int)TileSpacing.Top)
                                {
                                    WorldGen.PlaceTile(i, j - TOD.Height, tile, default, default, default, Main.rand.Next(0, TOD.RandomStyleRange));
                                    for (int a = 0; a < TOD.Width; a++)
                                        Framing.GetTileSafely(i + a, j).Slope = 0;
                                }
                            }
                            else if (TOD.AnchorLeft != AnchorData.Empty)
                            {
                                if (TileCheck2(i, j) == (int)TileSpacing.Right)
                                {
                                    WorldGen.PlaceTile(i + 1, j, tile, default, default, default, Main.rand.Next(0, TOD.RandomStyleRange));
                                }
                            }
                            else if (TOD.AnchorRight != AnchorData.Empty)
                            {
                                if (TileCheck2(i, j) == (int)TileSpacing.Left)
                                {
                                    WorldGen.PlaceTile(i + TOD.Width, j, tile, default, default, default, Main.rand.Next(0, TOD.RandomStyleRange));
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void MakeOvalFlatTop(int width, int height, Vector2 startingPoint, int type)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (j > height / 2)
                    {
                        if (OvalCheck((int)(startingPoint.X + width / 2), (int)(startingPoint.Y + height / 2), i + (int)startingPoint.X, j + (int)startingPoint.Y, (int)(width * .5f), (int)(height * .5f)))
                        {
                            WorldGen.PlaceTile(i + (int)startingPoint.X, j + (int)startingPoint.Y, type);
                        }
                    }
                    if (i == width / 2 && j == height / 2)
                    {
                        WorldGen.TileRunner(i + (int)startingPoint.X, j + (int)startingPoint.Y + 2, WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(10, 20), type, true, 0f, 0f, true, true);
                    }
                }
            }
        }

        public static void MakeOval(int width, int height, Vector2 startingPoint, int type, bool forced)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (OvalCheck((int)(startingPoint.X + width / 2), (int)(startingPoint.Y + height / 2), i + (int)startingPoint.X, j + (int)startingPoint.Y, (int)(width * .5f), (int)(height * .5f)))
                    {
                        WorldGen.PlaceTile(i + (int)startingPoint.X, j + (int)startingPoint.Y, type, false, forced);
                    }

                    if (i == width / 2 && j == height / 2)
                    {
                        WorldGen.TileRunner(i + (int)startingPoint.X, j + (int)startingPoint.Y + 2, WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(10, 20), type, true, 0f, 0f, true, true);
                    }
                }
            }
        }

        public static void MakeCircleFromCenter(int size, Vector2 Center, int type, bool forced)
        {
            Vector2 startingPoint = new Vector2(Center.X - size * .5f, Center.Y - size * .5f);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    float f = size * 0.5f;
                    if (Vector2.DistanceSquared(new Vector2(i + (int)startingPoint.X, j + (int)startingPoint.Y), startingPoint + new Vector2(size * 0.5f, size * 0.5f)) < f * f)
                    {
                        WorldGen.PlaceTile(i + (int)startingPoint.X, j + (int)startingPoint.Y, type, false, forced);
                    }
                }
            }
        }

        public static void MakeCircle(int size, Vector2 startingPoint, int type, bool forced)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    float f = size * 0.5f;
                    if (Vector2.DistanceSquared(new Vector2(i + (int)startingPoint.X, j + (int)startingPoint.Y), startingPoint + new Vector2(size * 0.5f, size * 0.5f)) < f * f)
                    {
                        WorldGen.PlaceTile(i + (int)startingPoint.X, j + (int)startingPoint.Y, type, false, forced);
                    }
                }
            }
        }

        public static void MakeWallCircle(int size, Vector2 startingPoint, int type, bool forced)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    float f = size * 0.5f;
                    if (Vector2.DistanceSquared(new Vector2(i + (int)startingPoint.X, j + (int)startingPoint.Y), startingPoint + new Vector2(size * 0.5f, size * 0.5f)) < f * f)
                    {
                        WorldGen.PlaceWall(i + (int)startingPoint.X, j + (int)startingPoint.Y, type);
                    }
                }
            }
        }

        public static void MakeJaggedOval(int width, int height, Vector2 startingPoint, int type, bool forced = false, int chance = 1)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Point Center = new Point((int)startingPoint.X + width / 2, (int)startingPoint.Y + height / 2);
                    if (OvalCheck(Center.X, Center.Y, i + (int)startingPoint.X, j + (int)startingPoint.Y, (int)(width * .5f), (int)(height * .5f)) && Main.rand.Next(chance) <= 1)
                    {
                        WorldGen.TileRunner(i + (int)startingPoint.X, j + (int)startingPoint.Y, WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(5, 10), type, true, 0f, 0f, true, true);
                    }

                    if (i == width / 2 && j == height / 2)
                    {
                        WorldGen.TileRunner(i + (int)startingPoint.X, j + (int)startingPoint.Y + 2, WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(10, 20), type, true, 0f, 0f, true, true);
                    }
                }
            }
        }

        public static void RemoveStoneSlabs()
        {
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
        }

        public static void MakeTriangle(Vector2 startingPoint, int width, int height, int slope, int type, int wallType = 0, bool pointingUp = true)
        {
            int initialStartingPosX = (int)startingPoint.X;
            int initialWidth = width;
            int initialSlope = slope;
            for (int j = 0; j < height; j++)
            {
                slope = Main.rand.Next(-1, 2) + initialSlope;
                for (int k = 0; k < slope; k++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        WorldGen.PlaceTile(i + (int)startingPoint.X, (int)startingPoint.Y - (j + k), type);
                    }
                }
                startingPoint.X += 1;
                width -= 2;
                j += slope - 1;
            }
        }

        public static void MakeTriangle(Vector2 startingPoint, int width, int height, int slope, int tileType = -1, int wallType = -1, bool pointingUp = true, int randFactor = 0)
        {
            int dir = 0;

            if (pointingUp) dir = 1;
            else dir = -1;

            int j = 0;

            while (j < height * dir)
            {
                for (int k = 0; k < slope + Main.rand.Next(-randFactor, randFactor + 1); k++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        if (tileType == -1)
                            WorldGen.PlaceTile(i + (int)startingPoint.X, (int)startingPoint.Y - (j + k), tileType);
                        if (wallType != -1)
                            WorldGen.PlaceWall(i + (int)startingPoint.X, (int)startingPoint.Y - (j + k), wallType);
                    }
                }
                startingPoint.X += 1;
                width -= 2;
                j += slope * dir;
            }
        }

        public static void FillRegion(int width, int height, Vector2 startingPoint, int type)
        {
            string messageBefore = EEMod.progressMessage;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Tile tile = Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    tile.type = (ushort)type;
                    tile.IsActive = true;
                    EEMod.progressMessage = messageBefore;
                    EEMod.progressMessage += $" {(int)((j + (i * height)) / (float)(width * height) * 100)}% done";
                }
            }
            EEMod.progressMessage = messageBefore;
        }

        public static void MakeCoral(Vector2 startingPoint, int type, int strength)
        {
            for (int j = 0; j < 5; j++)
            {
                int displacement = 0;
                for (int i = 0; i < strength; i++)
                {
                    if (Main.rand.NextBool(1))
                    {
                        displacement += Main.rand.Next(-1, 2);
                    }
                    WorldGen.PlaceTile(displacement + (int)startingPoint.X, (int)startingPoint.Y - i, type, false, true);
                }
            }
        }

        public static void MakeChasm(int positionX, int positionY, int height, int type, float slant, int sizeAddon, int stepAddon)
        {
            for (int i = 0; i < height; i++)
            {
                // Tile tile = Framing.GetTileSafely(positionX + (int)(i * slant), positionY + i);
                WorldGen.TileRunner(positionX + (int)(i * slant), positionY + i, WorldGen.genRand.Next(5 + sizeAddon / 2, 10 + sizeAddon), WorldGen.genRand.Next(5, 10) + stepAddon, type, true, 0f, 0f, true, true);
            }
        }

        public static void MakeWavyChasm(int positionX, int positionY, int height, int type, float slant, int sizeAddon)
        {
            for (int i = 0; i < height; i++)
            {
                // Tile tile = Framing.GetTileSafely(positionX + (int)(i * slant), positionY + i);
                WorldGen.TileRunner(positionX + (int)(i * slant) + (int)(Math.Sin(i / (float)50) * (20 * (1 + (i * 1.5f / height)))), positionY + i, WorldGen.genRand.Next(5 + sizeAddon / 2, 10 + sizeAddon), WorldGen.genRand.Next(5, 10), type, true, 0f, 0f, true, true);
            }
        }

        public static void MakeWavyChasm2(int positionX, int positionY, int height, int type, float slant, int sizeAddon, bool Override)
        {
            for (int i = 0; i < height; i++)
            {
                // Tile tile = Framing.GetTileSafely(positionX + (int)(i * slant), positionY + i);
                WorldGen.TileRunner(positionX + (int)(i * slant) + (int)(Math.Sin(i / (float)50) * (20 * (1 + (i * 1.5f / height)))), positionY + i, WorldGen.genRand.Next(5 + sizeAddon / 2, 10 + sizeAddon), WorldGen.genRand.Next(10, 12), type, true, 0f, 0f, true, Override);
            }
        }

        public static void MakeExpandingChasm(Vector2 position1, Vector2 position2, int type, int accuracy, int sizeAddon, bool Override, Vector2 stepBounds, float expansionRate)
        {
            for (int i = 0; i < accuracy; i++)
            {
                // Tile tile = Framing.GetTileSafely(positionX + (int)(i * slant), positionY + i);
                float perc = i / (float)accuracy;
                Vector2 currentPos = new Vector2(position1.X + (perc * (position2.X - position1.X)), position1.Y + (perc * (position2.Y - position1.Y)));
                WorldGen.TileRunner((int)currentPos.X,
                    (int)currentPos.Y,
                    WorldGen.genRand.Next(5 + sizeAddon / 2 + (int)(i * expansionRate), 10 + sizeAddon + (int)(i * expansionRate)),
                    WorldGen.genRand.Next((int)stepBounds.X, (int)stepBounds.Y),
                    type,
                    true,
                    0f,
                    0f,
                    true,
                    Override);
            }
        }

        public static void MakeWavyChasm3(Vector2 position1, Vector2 position2, int type, int accuracy, int sizeAddon, bool Override, Vector2 stepBounds, int waveInvolvment = 0, float frequency = 5, bool withBranches = false, int branchFrequency = 0, int lengthOfBranches = 0)
        {
            for (int i = 0; i < accuracy; i++)
            {
                // Tile tile = Framing.GetTileSafely(positionX + (int)(i * slant), positionY + i);
                float perc = i / (float)accuracy;
                Vector2 currentPos = new Vector2(position1.X + (perc * (position2.X - position1.X)), position1.Y + (perc * (position2.Y - position1.Y)));
                WorldGen.TileRunner((int)currentPos.X + (int)(Math.Sin(i / frequency) * waveInvolvment),
                    (int)currentPos.Y,
                    WorldGen.genRand.Next(5 + sizeAddon / 2, 10 + sizeAddon),
                    WorldGen.genRand.Next((int)stepBounds.X, (int)stepBounds.Y),
                    type,
                    true,
                    0f,
                    0f,
                    true,
                    Override);
                if (withBranches)
                {
                    if (i % branchFrequency == 0 && WorldGen.genRand.Next(2) == 0)
                    {
                        int Side = Main.rand.Next(0, 2);
                        if (Side == 0)
                        {
                            Vector2 NormalizedGradVec = Vector2.Normalize(position2 - position1).RotatedBy(MathHelper.PiOver2 + Main.rand.NextFloat(-0.3f, 0.3f));
                            //int ChanceForRecursion = Main.rand.Next(0, 4);
                            MakeWavyChasm3(currentPos, currentPos + NormalizedGradVec * lengthOfBranches, type, 100, 20, true, new Vector2(0, 20), 2, 5, true, 50, (int)(lengthOfBranches * 0.5f));
                        }
                        if (Side == 1)
                        {
                            Vector2 NormalizedGradVec = Vector2.Normalize(position2 - position1).RotatedBy(-MathHelper.PiOver2);
                            //int ChanceForRecursion = Main.rand.Next(0, 4);
                            MakeWavyChasm3(currentPos, currentPos + NormalizedGradVec * lengthOfBranches, type, 100, 20, true, new Vector2(0, 20), 7, 5, true, 50, (int)(lengthOfBranches * 0.5f));
                        }
                    }
                }
            }
        }
        
        public static int TileCheckVertical(int positionX, int positionY, int step, int maxIterations = 100)
        {
            int a = 0;
            for (int i = positionY; i < Main.maxTilesY || i > 0; i += step)
            {
                a++;
                if (WorldGen.InWorld(positionX, i, 15))
                {
                    Tile tile = Framing.GetTileSafely(positionX, i);
                    if (a == maxIterations)
                    {
                        return 0;
                    }
                    if (tile.IsActive && Main.tileSolid[tile.type])
                    {
                        return i;
                    }
                }
                else
                {
                    return 0;
                }
            }
            return 0;
        }
        
        public static int TileCheck(int positionX, int type)
        {
            for (int i = 0; i < Main.maxTilesY; i++)
            {
                Tile tile = Framing.GetTileSafely(positionX, i);
                if (tile.type == type)
                {
                    return i;
                }
            }
            return 0;
        }

        public static int TileCheck(int positionX, int type, int cap)
        {
            for (int i = 0; i < cap; i++)
            {
                Tile tile = Framing.GetTileSafely(positionX, i);
                if (tile.type == type)
                {
                    return i;
                }
            }
            return 0;
        }

        public static void KillWall(int width, int height, Vector2 startingPoint)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y).wall != ModContent.WallType<GemsandWallTile>())
                        WorldGen.KillWall(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                }
            }
        }

        public static bool OvalCheck(int midX, int midY, int x, int y, int sizeX, int sizeY)
        {
            double a = x - midX;
            double b = y - midY;

            double p = (a * a) / (sizeX * sizeX)
                    + (b * b) / (sizeY * sizeY);

            return p < 1;
        }

        public static void MakeLayer(int X, int midY, int size, int layer, int type)
        {
            int maxTiles = (int)(Main.maxTilesX * Main.maxTilesY * 9E-04);
            for (int k = 0; k < maxTiles * (size / 8); k++)
            {
                int x = WorldGen.genRand.Next(X - 160, X + 160);
                int y = WorldGen.genRand.Next(midY - 160, midY + 160);
                // Tile tile = Framing.GetTileSafely(x, y);
                if (layer == 1)
                {
                    if (Vector2.DistanceSquared(new Vector2(x, y), new Vector2(X, midY)) < size * size)
                    {
                        WorldGen.TileRunner(x, y, WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(5, 10), TileID.StoneSlab, true, 0f, 0f, true, true);
                    }
                }
                if (layer == 2)
                {
                    if (OvalCheck(X, midY, x, y, size * 3, size))
                    {
                        WorldGen.TileRunner(x, y, WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(5, 10), TileID.StoneSlab, true, 0f, 0f, true, true);
                    }
                }
            }
            RemoveStoneSlabs();
            /*  for (int k = 0; k < density; k++)
              {
                  int x = WorldGen.genRand.Next(X - 80, X + 80);
                  int y = WorldGen.genRand.Next(midY - 100, midY + 100);
                  Tile tile = Framing.GetTileSafely(x, y);
                  if (layer == 1)
                  {
                      int sizeSQ = size * size + 50 * 50;
                     // if (Vector2.DistanceSquared(new Vector2(x, y), new Vector2(X, midY)) < (sizeSQ) && tile.IsActive)
                        //  WorldGen.TileRunner(x, y, WorldGen.genRand.Next(4, 10), WorldGen.genRand.Next(5, 10), ModContent.TileType<HardenedGemsandTile>(), true, 0f, 0f, true, true);
                  }
                  if (layer == 2)
                  {
                     // if (OvalCheck(X, midY, x, y, (size * 3) + 10, (size) + 10) && tile.IsActive)
                        //  WorldGen.TileRunner(x, y, WorldGen.genRand.Next(4, 10), WorldGen.genRand.Next(5, 10), ModContent.TileType<GemsandstoneTile>(), true, 1, 1, true, true);
                  }
              }*/
            if (layer == 1)
            {
                WorldGen.TileRunner(X, midY, WorldGen.genRand.Next(size / 3 - 10, size / 3 + 10), WorldGen.genRand.Next(5, 10), type, true, 1f, 1f, false, true);
            }
        }

        public static void MakeLayerWithOutline(int X, int midY, int size, int layer, int type, int thickness)
        {
            int maxTiles = (int)(Main.maxTilesX * Main.maxTilesY * 9E-04);
            for (int k = 0; k < maxTiles * (size / 8); k++)
            {
                int x = WorldGen.genRand.Next(X - (size * 2), X + (size * 2));
                int y = WorldGen.genRand.Next(midY - (size * 2), midY + (size * 2));
                // Tile tile = Framing.GetTileSafely(x, y);
                if (layer == 1)
                {
                    if (Vector2.DistanceSquared(new Vector2(x, y), new Vector2(X, midY)) < size * size)
                    {
                        WorldGen.TileRunner(x, y, WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(5, 10), TileID.StoneSlab, true, 0f, 0f, true, true);
                    }
                }
                if (layer == 2)
                {
                    if (OvalCheck(X, midY, x, y, size * 3, size))
                    {
                        WorldGen.TileRunner(x, y, WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(5, 10), TileID.StoneSlab, true, 0f, 0f, true, true);
                    }
                }
            }

            for (int k = 0; k < maxTiles * 3; k++)
            {
                int x = WorldGen.genRand.Next(X - (size * 2), X + (size * 2));
                int y = WorldGen.genRand.Next(midY - (size * 2), midY + (size * 2));
                Tile tile = Framing.GetTileSafely(x, y);
                if (layer == 1)
                {
                    int sizeSQ = size + thickness;
                    if (Vector2.DistanceSquared(new Vector2(x, y), new Vector2(X, midY)) < sizeSQ * sizeSQ)
                    {
                        WorldGen.TileRunner(x, y, WorldGen.genRand.Next(4, 10), WorldGen.genRand.Next(5, 10), ModContent.TileType<GemsandTile>(), true, 0f, 0f, false, false);
                    }
                }
                if (layer == 2)
                {
                    if (OvalCheck(X, midY, x, y, (size * 3) + 10, size + 10) && tile.IsActive)
                    {
                        WorldGen.TileRunner(x, y, WorldGen.genRand.Next(4, 10), WorldGen.genRand.Next(5, 10), ModContent.TileType<DarkGemsandTile>(), true, 1, 1, true, true);
                    }
                }
            }
            for (int i = 0; i < 800; i++)
            {
                for (int j = 0; j < 2000; j++)
                {
                    Tile tile = Framing.GetTileSafely(i, j);
                    if (tile.type == TileID.StoneSlab)
                    {
                        WorldGen.KillTile(i, j);
                    }
                }
            }
            if (layer == 1)
            {
                //MakeOvalJaggedTop(20, 10, new Vector2(X - 12, midY), ModContent.TileType<GemsandstoneTile>());
            }
        }

        public static void Island(int islandWidth, int islandHeight, int posY)
        {
            MakeOvalJaggedBottom(islandWidth, islandHeight, new Vector2((Main.maxTilesX / 2) - islandWidth / 2, posY), ModContent.TileType<CoralSandTile>());
            MakeOvalJaggedBottom((int)(islandWidth * 0.6), (int)(islandHeight * 0.6), new Vector2((Main.maxTilesX / 2) - (Main.maxTilesX / 4), TileCheck(Main.maxTilesX / 2, ModContent.TileType<CoralSandTile>()) - 10), TileID.Dirt);
            //KillWall(Main.maxTilesX, Main.maxTilesY, Vector2.Zero);

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    WorldGen.SpreadGrass(i, j);
                }
            }
        }

        public static void SolidTileRunner(int i, int j, double strength, int steps, int type, bool addTile = false, float speedX = 0f, float speedY = 0f, bool noYChange = false, bool overRide = true, int ignoreTileType = -1)
        {
            double num = strength;
            float num2 = steps;

            Vector2 vector = default(Vector2);
            vector.X = i;
            vector.Y = j;

            Vector2 vector2 = default(Vector2);
            vector2.X = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
            vector2.Y = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;

            if (speedX != 0f || speedY != 0f)
            {
                vector2.X = speedX;
                vector2.Y = speedY;
            }

            while (num > 0.0 && num2 > 0f)
            {
                if (vector.Y < 0f && num2 > 0f && type == 59)
                {
                    num2 = 0f;
                }
                num = strength * (double)(num2 / (float)steps);
                num2 -= 1f;
                int num3 = (int)((double)vector.X - num * 0.5);
                int num4 = (int)((double)vector.X + num * 0.5);
                int num5 = (int)((double)vector.Y - num * 0.5);
                int num6 = (int)((double)vector.Y + num * 0.5);
                if (num3 < 1)
                {
                    num3 = 1;
                }
                if (num4 > Main.maxTilesX - 1)
                {
                    num4 = Main.maxTilesX - 1;
                }
                if (num5 < 1)
                {
                    num5 = 1;
                }
                if (num6 > Main.maxTilesY - 1)
                {
                    num6 = Main.maxTilesY - 1;
                }
                for (int k = num3; k < num4; k++)
                {
                    for (int l = num5; l < num6; l++)
                    {
                        if (overRide || !Main.tile[k, l].IsActive)
                        {
                            if (Main.tileSolid[Main.tile[k, l].type] == true)
                            {
                                Main.tile[k, l].type = (ushort)type;
                            }
                        }
                        if (addTile)
                        {
                            Main.tile[k, l].IsActive = true;
                            Main.tile[k, l].LiquidAmount = 0;
                            Main.tile[k, l].LiquidType = 0;
                        }
                    }
                }
                vector += vector2;

                if ((WorldGen.genRand.NextBool(3)) && num > 50.0)
                {
                    vector += vector2;
                    num2 -= 1f;
                    vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                    vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                    if (num > 100.0)
                    {
                        vector += vector2;
                        num2 -= 1f;
                        vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                        vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                        if (num > 150.0)
                        {
                            vector += vector2;
                            num2 -= 1f;
                            vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                            vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                            if (num > 200.0)
                            {
                                vector += vector2;
                                num2 -= 1f;
                                vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                if (num > 250.0)
                                {
                                    vector += vector2;
                                    num2 -= 1f;
                                    vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                    vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                    if (num > 300.0)
                                    {
                                        vector += vector2;
                                        num2 -= 1f;
                                        vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                        vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                        if (num > 400.0)
                                        {
                                            vector += vector2;
                                            num2 -= 1f;
                                            vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                            vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                            if (num > 500.0)
                                            {
                                                vector += vector2;
                                                num2 -= 1f;
                                                vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                if (num > 600.0)
                                                {
                                                    vector += vector2;
                                                    num2 -= 1f;
                                                    vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                    vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                    if (num > 700.0)
                                                    {
                                                        vector += vector2;
                                                        num2 -= 1f;
                                                        vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                        vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                        if (num > 800.0)
                                                        {
                                                            vector += vector2;
                                                            num2 -= 1f;
                                                            vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                            vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                            if (num > 900.0)
                                                            {
                                                                vector += vector2;
                                                                num2 -= 1f;
                                                                vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                                vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                vector2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;

                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if (!noYChange)
                {
                    vector2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
                    if (vector2.Y > 1f)
                    {
                        vector2.Y = 1f;
                    }
                    if (vector2.Y < -1f)
                    {
                        vector2.Y = -1f;
                    }
                }
                else if (type != 59 && num < 3.0)
                {
                    if (vector2.Y > 1f)
                    {
                        vector2.Y = 1f;
                    }
                    if (vector2.Y < -1f)
                    {
                        vector2.Y = -1f;
                    }
                }
                if (type == 59 && !noYChange)
                {
                    if ((double)vector2.Y > 0.5)
                    {
                        vector2.Y = 0.5f;
                    }
                    if ((double)vector2.Y < -0.5)
                    {
                        vector2.Y = -0.5f;
                    }
                    if ((double)vector.Y < Main.rockLayer + 100.0)
                    {
                        vector2.Y = 1f;
                    }
                    if (vector.Y > (float)(Main.maxTilesY - 300))
                    {
                        vector2.Y = -1f;
                    }
                }
            }
        }
    }
}