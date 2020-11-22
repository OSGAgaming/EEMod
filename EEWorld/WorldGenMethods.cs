﻿using EEMod.Common.IDs;
using EEMod.Tiles;

//using Microsoft.Office.Interop.Excel;
using EEMod.Tiles.EmptyTileArrays;
using EEMod.Tiles.Furniture;
using EEMod.Tiles.Ores;
using EEMod.VerletIntegration;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;

namespace EEMod.EEWorld
{
    public partial class EEWorld
    {
        public static IList<Vector2> Vines = new List<Vector2>();

        public override void Load(TagCompound tag)
        {
            if (tag.ContainsKey("EntracesPosses"))
            {
                EntracesPosses = tag.GetList<Vector2>("EntracesPosses");
            }
            if (tag.ContainsKey("CoralBoatPos"))
            {
                EESubWorlds.CoralBoatPos = tag.Get<Vector2>("CoralBoatPos");
            }
            if (tag.ContainsKey("SubWorldSpecificVolcanoInsidePos"))
            {
                SubWorldSpecificVolcanoInsidePos = tag.Get<Vector2>("SubWorldSpecificVolcanoInsidePos");
            }
            if (tag.ContainsKey("yes"))
            {
                yes = tag.Get<Vector2>("yes");
            }
            if (tag.ContainsKey("ree"))
            {
                ree = tag.Get<Vector2>("ree");
            }
            if (tag.ContainsKey("ChainConnections"))
            {
                EESubWorlds.ChainConnections = tag.GetList<Vector2>("ChainConnections");
            }
            if (tag.ContainsKey("OrbPositions"))
            {
                EESubWorlds.OrbPositions = tag.GetList<Vector2>("OrbPositions");
            }
            if (tag.ContainsKey("BulbousTreePosition"))
            {
                EESubWorlds.BulbousTreePosition = tag.GetList<Vector2>("BulbousTreePosition");
            }
            if (tag.ContainsKey("SwingableVines"))
            {
                VerletHelpers.SwingableVines = tag.GetList<Vector2>("SwingableVines");
                if (VerletHelpers.SwingableVines.Count != 0)
                {
                    foreach (Vector2 vec in VerletHelpers.SwingableVines)
                    {
                        VerletHelpers.AddStickChainNoAdd(ref ModContent.GetInstance<EEMod>().verlet, vec, Main.rand.Next(5, 15), 27);
                    }
                }
            }
            if (tag.ContainsKey("EmptyTileVectorMain") && tag.ContainsKey("EmptyTileVectorSub"))
            {
                IList<Vector2> VecMains = tag.GetList<Vector2>("EmptyTileVectorMain");
                IList<Vector2> VecSubs = tag.GetList<Vector2>("EmptyTileVectorSub");
                EmptyTileEntityCache.EmptyTilePairs = VecMains.Zip(VecSubs, (k, v) => new { Key = k, Value = v }).ToDictionary(x => x.Key, x => x.Value);
            }
            if (tag.ContainsKey("EmptyTileVectorEntities") && tag.ContainsKey("EmptyTileEntities"))
            {
                IList<Vector2> VecMains = tag.GetList<Vector2>("EmptyTileVectorEntities");
                IList<EmptyTileDrawEntity> VecSubs = tag.GetList<EmptyTileDrawEntity>("EmptyTileEntities");
                EmptyTileEntityCache.EmptyTileEntityPairs = VecMains.Zip(VecSubs, (k, v) => new { Key = k, Value = v }).ToDictionary(x => x.Key, x => x.Value);
            }
            if (tag.ContainsKey("CoralCrystalPosition"))
            {
                EESubWorlds.CoralCrystalPosition = tag.GetList<Vector2>("CoralCrystalPosition");
                // for (int i = 0; i < EESubWorlds.CoralCrystalPosition.Count; i++)
                //    EmptyTileEntityCache.AddPair(new Crystal(EESubWorlds.CoralCrystalPosition[i]), EESubWorlds.CoralCrystalPosition[i], EmptyTileArrays.CoralCrystal);
            }
            if (tag.ContainsKey("LightStates"))
            {
                LightStates = tag.GetByteArray("LightStates");
            }
            List<string> downed = new List<string>();
            if (eocFlag)
            {
                downed.Add("eocFlag");
            }
            IList<string> flags = tag.GetList<string>("boolFlags");

            // Game modes

            // Downed bosses
            downedAkumo = flags.Contains("downedAkumo");
            downedHydros = flags.Contains("downedHydros");
            downedKraken = flags.Contains("downedKraken");
        }

        public override TagCompound Save()
        {
            TagCompound tag = new TagCompound();
            if (Main.ActiveWorldFileData.Name == KeyID.CoralReefs)
            {
                tag["CoralBoatPos"] = EESubWorlds.CoralBoatPos;
                tag["ChainConnections"] = EESubWorlds.ChainConnections;
                tag["OrbPositions"] = EESubWorlds.OrbPositions;
                tag["BulbousTreePosition"] = EESubWorlds.BulbousTreePosition;
                tag["SwingableVines"] = VerletHelpers.SwingableVines;
                tag["LightStates"] = LightStates;
                tag["CoralCrystalPosition"] = EESubWorlds.CoralCrystalPosition;
                tag["EmptyTileVectorMain"] = EmptyTileEntityCache.EmptyTilePairs.Keys.ToList();
                tag["EmptyTileVectorSub"] = EmptyTileEntityCache.EmptyTilePairs.Values.ToList();
                tag["EmptyTileVectorEntities"] = EmptyTileEntityCache.EmptyTileEntityPairs.Keys.ToList();
                tag["EmptyTileEntities"] = EmptyTileEntityCache.EmptyTileEntityPairs.Values.ToList();
            }
            if (Main.ActiveWorldFileData.Name == KeyID.VolcanoInside)
            {
                tag["SubWorldSpecificVolcanoInsidePos"] = SubWorldSpecificVolcanoInsidePos;
            }
            tag["EntracesPosses"] = EntracesPosses;
            tag["yes"] = yes;
            tag["ree"] = ree;
            return tag;
        }

        /*List<string> boolflags = new List<string>();

        // Game modes
        if (GenkaiMode)
            boolflags.Add("GenkaiMode");

        // Downed bosses
        if (downedGallagar)
            boolflags.Add("downedGallagar");
        if (downedForerunner)
            boolflags.Add("downedForerunner");
        if (downedSoS)
            boolflags.Add("downedSoS");
        if (downedFlare)
            boolflags.Add("downedFlare");
        if (downedAssimilator)
            boolflags.Add("downedAssimilator");
        if (downedAkumo)
            boolflags.Add("downedAkumo");
        if (downedHydros)
            boolflags.Add("downedHydros");
        if (downedStagrel)
            boolflags.Add("downedStagrel");
        if (downedBeheader)
            boolflags.Add("downedBeheader");

        return new TagCompound
        {
            ["SaveVersion"] = new Version(0, 3, 0, 0).ToString(),
            ["boolFlags"] = boolflags
        };*/

        private static void StartSandstorm()
        {
            Sandstorm.Happening = true;
            Sandstorm.TimeLeft = (int)(3600f * (8f + Main.rand.NextFloat() * 16f));
            ChangeSeverityIntentions();
        }

        private static void ChangeSeverityIntentions()
        {
            if (Sandstorm.Happening)
            {
                Sandstorm.IntendedSeverity = 0.4f + Main.rand.NextFloat();
            }
            else if (Main.rand.NextBool(3))
            {
                Sandstorm.IntendedSeverity = 0f;
            }
            else
            {
                Sandstorm.IntendedSeverity = Main.rand.NextFloat() * 0.3f;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(msgType: MessageID.WorldData, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
            }
        }

        private static void EEModOres(GenerationProgress progress)
        {
            progress.Message = "Endless Escapade Ores";
            int maxTiles = Main.maxTilesX * Main.maxTilesY;
            int rockLayerLow = (int)WorldGen.rockLayerLow;
            int OreAmount;

            OreAmount = (int)(maxTiles * 0.00008);
            for (int k = 0; k < OreAmount; k++)
            {
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next(rockLayerLow, Main.maxTilesY);
                WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(5, 7), ModContent.TileType<HydrofluoricOreTile>());

                x = WorldGen.genRand.Next(0, Main.maxTilesX);
                y = WorldGen.genRand.Next(rockLayerLow, Main.maxTilesY);
                WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(5, 7), ModContent.TileType<DalantiniumOreTile>());
            }
        }

        public static void FillRegionNoEdit(int width, int height, Vector2 startingPoint, int type)
        {
            string messageBefore = EEMod.progressMessage;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Tile tile = Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    tile.type = (ushort)type;
                    tile.active(true);
                    EEMod.progressMessage = messageBefore;
                    EEMod.progressMessage += $" {(int)((j + (i * height)) / (float)(width * height) * 100)}% done";
                }
            }
            EEMod.progressMessage = messageBefore;
        }

        private static PerlinNoiseFunction PNF;

        public static float[] PerlinArray(int width, int seedVar, float amplitude, Vector2 res)
        {
            PNF = new PerlinNoiseFunction(width, seedVar, (int)res.X, (int)res.Y, 0.5f);
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
            PNF = new PerlinNoiseFunction(width, seedVar, (int)res.X, (int)res.Y, 0.5f);
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
                        tile.slope(array[j, i, 1]);
                        tile.active(true);
                    }
                }
            }
        }

        public static void FillRegionNoEditWithNoise(int width, int height, Vector2 startingPoint, int type)
        {
            string messageBefore = EEMod.progressMessage;
            float[] PerlinStrip = PerlinArray(width, 1000, 15, new Vector2(60, 200));
            for (int i = 0; i < width; i++)
            {
                for (int j = (int)PerlinStrip[i]; j < height; j++)
                {
                    Tile tile = Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    tile.type = (ushort)type;
                    tile.active(true);
                    EEMod.progressMessage = messageBefore;
                    EEMod.progressMessage += $" {(int)((j + (i * height)) / (float)(width * height) * 100)}% done";
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

        private static void FillRegionDiag(int width, int height, Vector2 startingPoint, int type, int leftOrRight)
        {
            string messageBefore = EEMod.progressMessage;
            if (leftOrRight == 0)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height - i; j++)
                    {
                        WorldGen.PlaceTile(i + (int)startingPoint.X, j + (int)startingPoint.Y, type);
                        EEMod.progressMessage = messageBefore;
                        EEMod.progressMessage += $" {(int)((j + (i * height)) / (float)(width * height) * 100)}% done";
                    }
                }
            }
            if (leftOrRight == 1)
            {
                for (int i = width; i > -1; i--)
                {
                    for (int j = 0; j < i; j++)
                    {
                        WorldGen.PlaceTile(i + (int)startingPoint.X - width, j + (int)startingPoint.Y, type);
                        EEMod.progressMessage = messageBefore;
                        EEMod.progressMessage += $" {(int)((j + (i * height)) / (float)(width * height) * 100)}% done";
                    }
                }
            }
            EEMod.progressMessage = messageBefore;
        }

        private static void ClearPathWay(int width, int height, float gradient, Vector2 startingPoint, bool withPillars)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    WorldGen.KillTile(i + (int)startingPoint.X, j + (int)startingPoint.Y + (int)(i * gradient));
                }
            }
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (withPillars)
                    {
                        if (i % 10 == 0)
                        {
                            MakePillarWalls(new Vector2(i + (int)startingPoint.X, +(int)startingPoint.Y + (int)(i * gradient) - 1), 11);
                        }
                        //WorldGen.PlaceWall(i + (int)startingPoint.X, j + (int)startingPoint.Y + +(int)Math.Round(i * gradient), WallID.SandstoneBrick);
                    }
                }
            }
        }

        private static void Hole(int height, int width, Vector2 startingPoint)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    WorldGen.KillTile((int)startingPoint.X + j, (int)startingPoint.Y + i);
                    WorldGen.KillWall((int)startingPoint.X + j, (int)startingPoint.Y + i);
                }
            }
        }

        private static void MakePathWay(Vector2 firstRoom, Vector2 secondRoom, Vector2 firstRoomSize, Vector2 secondRoomSize, int heightOfConnection, bool withPillars)
        {
            Vector2 secondRoomDoorPos = new Vector2(secondRoom.X, secondRoomSize.Y / 2 + secondRoom.Y - heightOfConnection);
            Vector2 firstRoomDoorPos = new Vector2(firstRoom.X, firstRoomSize.Y / 2 + firstRoom.Y - heightOfConnection);
            if (firstRoom.X > secondRoom.X)
            {
                float gradient = (firstRoomDoorPos.Y - secondRoomDoorPos.Y) / (firstRoomDoorPos.X - firstRoomSize.X / 2 - (secondRoomDoorPos.X + secondRoomSize.X / 2));
                ClearPathWay((int)(firstRoomDoorPos.X - firstRoomSize.X / 2) - (int)(secondRoomDoorPos.X + secondRoomSize.X / 2) + 1, heightOfConnection, gradient, secondRoomDoorPos + new Vector2(secondRoomSize.X / 2, 0), withPillars);
                if (firstRoomDoorPos.X - firstRoomSize.X / 2 - (int)(secondRoomDoorPos.X + secondRoomSize.X / 2) <= 4)
                {
                    if (secondRoomDoorPos.Y < firstRoomDoorPos.Y)
                    {
                        Hole((int)(firstRoomDoorPos.Y - secondRoomDoorPos.Y), 5, new Vector2(firstRoomDoorPos.X - firstRoomSize.X / 2, secondRoomDoorPos.Y));
                    }
                    else
                    {
                        Hole((int)(secondRoomDoorPos.Y - firstRoomDoorPos.Y), 5, new Vector2(secondRoomDoorPos.X - secondRoomSize.X / 2, firstRoomDoorPos.Y));
                    }
                }
            }
            else
            {
                float gradient = (secondRoomDoorPos.Y - firstRoomDoorPos.Y) / (secondRoomDoorPos.X - secondRoomSize.X / 2 - (firstRoomDoorPos.X + firstRoomSize.X / 2));
                ClearPathWay((int)(secondRoomDoorPos.X - secondRoomSize.X / 2) - (int)(firstRoomDoorPos.X + firstRoomSize.X / 2) + 1, heightOfConnection, gradient, firstRoomDoorPos + new Vector2(firstRoomSize.X / 2, 0), withPillars);
                if (secondRoomDoorPos.X - secondRoomSize.X / 2 - (int)(firstRoomDoorPos.X + firstRoomSize.X / 2) <= 4)
                {
                    if (secondRoomDoorPos.Y < firstRoomDoorPos.Y)
                    {
                        Hole((int)(firstRoomDoorPos.Y - secondRoomDoorPos.Y), 5, new Vector2(firstRoomDoorPos.X + firstRoomSize.X / 2, secondRoomDoorPos.Y));
                    }
                    else
                    {
                        Hole((int)(secondRoomDoorPos.Y - firstRoomDoorPos.Y), 5, new Vector2(secondRoomDoorPos.X + secondRoomSize.X / 2, firstRoomDoorPos.Y));
                    }
                }
            }
        }

        public static void PlaceEntrance(int i, int j, int[,] shape)
        {
            for (int y = 0; y < shape.GetLength(0); y++)
            {
                for (int x = 0; x < shape.GetLength(1); x++)
                {
                    int k = i - 3 + x;
                    int l = j - 6 + y;
                    if (WorldGen.InWorld(k, l, 30))
                    {
                        Tile tile = Framing.GetTileSafely(k, l);
                        tile.ClearTile();
                        switch (shape[y, x])
                        {
                            case 0:
                                WorldGen.KillTile(k, l, false, false, true);
                                break;

                            case 1:
                                tile.type = TileID.SandstoneBrick;
                                tile.active(true);
                                break;

                            case 2:
                                tile.type = TileID.RubyGemspark;
                                tile.active(true);
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
        }

        public static void PlaceWalls(int i, int j, int[,] shape)
        {
            for (int y = 0; y < shape.GetLength(0); y++)
            {
                for (int x = 0; x < shape.GetLength(1); x++)
                {
                    int k = i - 3 + x;
                    int l = j - 6 + y;
                    if (WorldGen.InWorld(k, l, 30))
                    {
                        Tile tile = Framing.GetTileSafely(k, l);
                        switch (shape[y, x])
                        {
                            case 0:
                                WorldGen.KillWall(y, x, false);
                                break;

                            case 1:
                                tile.wall = WallID.SandFall;
                                break;

                            case 2:
                                tile.wall = WallID.SandstoneBrick;
                                break;

                            case 3:
                                tile.wall = WallID.DesertFossil;
                                tile.wallColor(29);
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
        }

        public static void PlaceShip(int i, int j, int[,] shape)
        {
            for (int y = 0; y < shape.GetLength(0); y++)
            {
                for (int x = 0; x < shape.GetLength(1); x++)
                {
                    int k = i - 3 + x;
                    int l = j - 6 + y;
                    if (WorldGen.InWorld(k, l, 30))
                    {
                        Tile tile = Framing.GetTileSafely(k, l);
                        switch (shape[y, x])
                        {
                            case 0:
                                if (Main.netMode == NetmodeID.MultiplayerClient) // sync
                                {
                                    NetMessage.sendWater(k, l);
                                }

                                break;

                            case 1:
                                tile.type = TileID.WoodBlock;
                                tile.active(true);
                                break;

                            case 2:
                                tile.type = TileID.RichMahogany;
                                tile.color(28);
                                tile.active(true);
                                break;

                            case 3:
                                tile.type = TileID.GoldCoinPile;
                                tile.active(true);
                                break;

                            case 4:
                                tile.type = TileID.Platforms;
                                tile.active(true);
                                break;

                            case 5:
                                tile.type = TileID.WoodenBeam;
                                tile.active(true);
                                break;

                            case 6:
                                tile.type = TileID.SilkRope;
                                tile.active(true);
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
        }

        public static void PlaceShipWalls(int i, int j, int[,] shape)
        {
            for (int y = 0; y < shape.GetLength(0); y++)
            {
                for (int x = 0; x < shape.GetLength(1); x++)
                {
                    int k = i - 3 + x;
                    int l = j - 6 + y;
                    if (WorldGen.InWorld(k, l, 30))
                    {
                        Tile tile = Framing.GetTileSafely(k, l);
                        switch (shape[y, x])
                        {
                            case 0:
                                WorldGen.KillWall(y, x, false);
                                break;

                            case 1:
                                tile.wall = WallID.Cloud;
                                break;

                            case 2:
                                tile.wall = WallID.RichMahoganyFence;
                                tile.wallColor(28);
                                break;

                            case 3:
                                tile.wall = WallID.Cloud;
                                tile.wallColor(29);
                                break;

                            case 4:
                                tile.wall = WallID.Wood;
                                break;

                            case 5:
                                tile.type = TileID.WoodenBeam;
                                tile.active(true);
                                break;

                            case 6:
                                tile.wall = WallID.Sail;
                                tile.wallColor(29);
                                break;

                            default:
                                break;
                        }
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

        public static void GenerateStructure(int i, int j, int[,] shape, int[] blocks, int[] paints = null, int[,] wallShape = null, int[] walls = null, int[] wallPaints = null)
        {
            for (int y = 0; y < shape.GetLength(0); y++)
            {
                for (int x = 0; x < shape.GetLength(1); x++)
                {
                    int k = i - 3 + x;
                    int l = j - 6 + y;
                    if (WorldGen.InWorld(k, l, 30))
                    {
                        Tile tile = Framing.GetTileSafely(k, l);
                        tile.type = (ushort)blocks[shape[y, x]];
                        if (paints[blocks[shape[y, x]]] != default)
                        {
                            tile.color((byte)paints[blocks[shape[y, x]]]);
                        }

                        tile.active(true);
                    }
                }
            }
            if (wallShape != null && walls != null)
            {
                for (int y = 0; y < wallShape.GetLength(0); y++)
                {
                    for (int x = 0; x < wallShape.GetLength(1); x++)
                    {
                        int k = i - 3 + x;
                        int l = j - 6 + y;
                        if (WorldGen.InWorld(k, l, 30))
                        {
                            Tile tile = Framing.GetTileSafely(k, l);
                            tile.wall = (ushort)walls[wallShape[y, x]];
                            if (wallPaints[walls[wallShape[y, x]]] != default)
                            {
                                tile.color((byte)wallPaints[walls[wallShape[y, x]]]);
                            }
                        }
                    }
                }
            }
        }

        private static void MakePillar(Vector2 startingPos, int height, bool water, bool fire)
        {
            if (water)
            {
                Main.tile[(int)startingPos.X - 1, (int)startingPos.Y - 3].liquidType(0); // set liquid type 0 is water 1 lava 2 honey 3+ water iirc
                Main.tile[(int)startingPos.X - 1, (int)startingPos.Y - 3].liquid = 255; // set liquid ammount
                Main.tile[(int)startingPos.X - 1, (int)startingPos.Y - 4].liquid = 255;
                WorldGen.SquareTileFrame((int)startingPos.X - 1, (int)startingPos.Y - 3, true); // soemthing for astatic voiding the liquid from being static
                if (Main.netMode == NetmodeID.MultiplayerClient) // sync
                {
                    NetMessage.sendWater((int)startingPos.X - 1, (int)startingPos.Y - 3);
                }
            }
            if (fire)
            {
                WorldGen.PlaceTile((int)startingPos.X - 1, (int)startingPos.Y - 3, TileID.LivingFire);
                WorldGen.PlaceTile((int)startingPos.X, (int)startingPos.Y - 3, TileID.LivingFire);
                WorldGen.PlaceTile((int)startingPos.X + 1, (int)startingPos.Y - 3, TileID.LivingFire);
                WorldGen.PlaceTile((int)startingPos.X, (int)startingPos.Y - 3, TileID.LivingFire);
            }
            WorldGen.PlaceTile((int)startingPos.X - 2, (int)startingPos.Y - 3, TileID.SandStoneSlab);
            FillRegion(5, 1, new Vector2(startingPos.X - 2, startingPos.Y + 1 - 3), TileID.SandStoneSlab);
            FillRegion(3, 1, new Vector2(startingPos.X + 1 - 2, startingPos.Y + 2 - 3), TileID.SandStoneSlab);
            WorldGen.PlaceTile((int)startingPos.X + 4 - 2, (int)startingPos.Y - 3, TileID.SandStoneSlab);
            WorldGen.PlaceTile((int)startingPos.X + 2 - 2, (int)startingPos.Y + 3 - 3, TileID.SandStoneSlab);
            if (water)
            {
                Tile tile = Framing.GetTileSafely((int)startingPos.X - 2, (int)startingPos.Y - 3);
                Tile tile1 = Framing.GetTileSafely((int)startingPos.X - 2 + 4, (int)startingPos.Y - 3);
                tile.halfBrick(true);
                tile1.halfBrick(true);
            }
            MakePillarWalls(new Vector2(startingPos.X + 2 - 2, startingPos.Y + 2 - 4), height);
        }

        private static void MakePillarWalls(Vector2 startingPos, int height)
        {
            Tile tile1 = Framing.GetTileSafely((int)startingPos.X + 1, (int)startingPos.Y);
            Tile tile2 = Framing.GetTileSafely((int)startingPos.X + 0, (int)startingPos.Y);
            Tile tile3 = Framing.GetTileSafely((int)startingPos.X + -1, (int)startingPos.Y);
            Tile tile4 = Framing.GetTileSafely((int)startingPos.X + 1, (int)startingPos.Y + height + 1);
            Tile tile5 = Framing.GetTileSafely((int)startingPos.X + 0, (int)startingPos.Y + height + 1);
            Tile tile6 = Framing.GetTileSafely((int)startingPos.X + -1, (int)startingPos.Y + height + 1);
            if (tile1.active() && tile2.active() && tile3.active() && tile4.active() && tile5.active() && tile6.active())
            {
                for (int i = -1; i < 2; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        Tile tile = Framing.GetTileSafely((int)startingPos.X + i, (int)startingPos.Y + j);
                        WorldGen.PlaceWall((int)startingPos.X + i, (int)startingPos.Y + j, WallID.StoneSlab);
                        tile.wallColor(28);
                    }
                }
            }
        }

        private static void MakeGoldPile(Vector2 startingPos, int type)
        {
            if (type == 0)
            {
                WorldGen.PlaceTile((int)startingPos.X, (int)startingPos.Y, TileID.GoldCoinPile);
                WorldGen.PlaceTile((int)startingPos.X + 1, (int)startingPos.Y, TileID.GoldCoinPile);
                WorldGen.PlaceTile((int)startingPos.X + 2, (int)startingPos.Y, TileID.GoldCoinPile);
                WorldGen.PlaceTile((int)startingPos.X + 3, (int)startingPos.Y, TileID.GoldCoinPile);
                WorldGen.PlaceTile((int)startingPos.X + 4, (int)startingPos.Y, TileID.GoldCoinPile);
                WorldGen.PlaceTile((int)startingPos.X + 1, (int)startingPos.Y - 1, TileID.GoldCoinPile);
                WorldGen.PlaceTile((int)startingPos.X + 2, (int)startingPos.Y - 1, TileID.GoldCoinPile);
                WorldGen.PlaceTile((int)startingPos.X + 3, (int)startingPos.Y - 1, TileID.GoldCoinPile);
            }
            if (type == 1)
            {
                WorldGen.PlaceTile((int)startingPos.X, (int)startingPos.Y, TileID.GoldCoinPile);
                WorldGen.PlaceTile((int)startingPos.X + 1, (int)startingPos.Y, TileID.GoldCoinPile);
                WorldGen.PlaceTile((int)startingPos.X + 2, (int)startingPos.Y, TileID.GoldCoinPile);
            }
        }

        private static void MakeShelf(Vector2 startingPos, int leftOrRight, int length)
        {
            if (leftOrRight == 0)
            {
                Tile tile = Framing.GetTileSafely((int)startingPos.X - 1, (int)startingPos.Y);
                Tile tile1 = Framing.GetTileSafely((int)startingPos.X, (int)startingPos.Y);
                if (tile.active() && !tile1.active())
                {
                    for (int i = 0; i < length; i++)
                    {
                        WorldGen.PlaceTile((int)startingPos.X + i, (int)startingPos.Y, TileID.Platforms, true, true, -1, 31);
                        WorldGen.PlaceTile((int)startingPos.X + i, (int)startingPos.Y - 1, TileID.Books);
                    }
                }
            }
            if (leftOrRight == 1)
            {
                Tile tile = Framing.GetTileSafely((int)startingPos.X + 1, (int)startingPos.Y);
                Tile tile1 = Framing.GetTileSafely((int)startingPos.X, (int)startingPos.Y);
                if (tile.active() && !tile1.active())
                {
                    for (int i = 0; i < length; i++)
                    {
                        WorldGen.PlaceTile((int)startingPos.X - i, (int)startingPos.Y, TileID.Platforms, true, true, -1, 31);
                        WorldGen.PlaceTile((int)startingPos.X - i, (int)startingPos.Y - 1, TileID.Books);
                    }
                }
            }
        }

        private static void FirstRoomFirstVariation(Vector2 startingPos)
        {
            int RoomPosX = (int)startingPos.X;
            int RoomPosY = (int)startingPos.Y;

            for (int x = 0; x < TowerTiles.GetLength(1); x++)
            {
                for (int y = 0; y < TowerTiles.GetLength(0); y++)
                {
                    Tile tile = Framing.GetTileSafely(RoomPosX + x, RoomPosY - y);
                    switch (TowerTiles[TowerTiles.GetLength(0) - 1 - y, x])
                    {
                        case 0:
                            if (tile.type == TileID.Trees)
                            {
                                WorldGen.KillTile(RoomPosX + x, RoomPosY - y, false, false, true);
                            }
                            break;

                        case 1:
                            tile.type = TileID.SandstoneBrick;
                            tile.active(true);
                            break;

                        case 2:
                            tile.type = TileID.GoldCoinPile;
                            tile.active(true);
                            break;

                        case 3:
                            tile.type = TileID.SandStoneSlab;
                            tile.active(true);
                            tile.color(28);
                            break;

                        case 4:
                            tile.type = TileID.MarbleBlock;
                            tile.active(true);
                            tile.color(28);
                            break;

                        case 5:
                            tile.type = TileID.RubyGemspark;
                            tile.active(true);
                            break;

                        case 6:
                            tile.type = TileID.PalladiumColumn;
                            tile.active(true);
                            tile.inActive(true);
                            tile.color(28);
                            break;

                        case 7:
                            tile.type = TileID.Lamps;
                            tile.active(true);
                            tile.inActive(true);
                            tile.color(28);
                            break;

                        case 8:
                            tile.type = TileID.Banners;
                            tile.active(true);
                            tile.inActive(true);
                            tile.color(28);
                            break;

                        case 9:
                            WorldGen.PlaceObject(RoomPosX + x, RoomPosY - y, TileID.TallGateClosed);
                            tile.active(true);
                            break;
                    }
                    /*if (TowerSlopes[y, x] == 5)
                    {
                        tile.halfBrick(true);
                    }
                    else
                    {
                        tile.halfBrick(false);
                        tile.slope(TowerSlopes[y, x]);
                    }*/
                    switch (TowerWalls[TowerWalls.GetLength(0) - 1 - y, x])
                    {
                        case 1:
                            tile.wall = WallID.SandstoneBrick;
                            break;

                        case 2:
                            tile.wall = WallID.StoneSlab;
                            tile.wallColor(28);
                            break;

                        case 3:
                            tile.wall = WallID.DesertFossil;
                            break;

                        case 4:
                            tile.wall = WallID.DesertFossil;
                            tile.wallColor(29);
                            break;

                        case 5:
                            tile.wall = WallID.SandFall;
                            break;
                    }
                }
            }
        }

        public static void DoAndAssignShrineValues()
        {
            int posX = WorldGen.genRand.Next(0, Main.maxTilesX);
            int posY = WorldGen.genRand.Next((int)WorldGen.worldSurfaceHigh, Main.maxTilesY);
            PlaceEntrance(posX, posY, pyroEntrance);
            PlaceWalls(posX, posY, pyroEntranceWalls);
            EntracesPosses.Add(new Vector2(posX, posY));
            yes = new Vector2(posX, posY);
        }

        public static void DoAndAssignShipValues()
        {
            PlaceShipWalls(100, TileCheckWater(100) - 22, ShipWalls);
            PlaceShip(100, TileCheckWater(100) - 22, ShipTiles);
            ree = new Vector2(100, TileCheckWater(100) - 22);
        }

        public static int TileCheckWater(int positionX)
        {
            for (int i = 0; i < Main.maxTilesY; i++)
            {
                Tile tile = Framing.GetTileSafely(positionX, i);
                if (tile.liquid > 64)
                {
                    return i;
                }
            }
            return 0;
        }

        public static void Pyramid(float startX, float startY)
        {
            int noOfRooms = 12;
            int waterBoltRoom = WorldGen.genRand.Next(2, noOfRooms - 2);
            int fireRoom = 0;
            while (fireRoom == waterBoltRoom || fireRoom == 0)
            {
                fireRoom = WorldGen.genRand.Next(2, noOfRooms - 2);
                if (fireRoom != waterBoltRoom)
                {
                    break;
                }
            }
            float startingX = startX;
            float startingY = startY;
            int width = 200;
            int height = 250;
            Vector2[] Rooms = new Vector2[noOfRooms];
            Vector2[] RoomSizes = new Vector2[noOfRooms];
            bool[] isRoom = new bool[noOfRooms];
            ClearRegion(width, height, new Vector2(startingX, startingY));
            FillRegion(width, height, new Vector2(startingX, startingY), TileID.SandstoneBrick);
            for (int i = 0; i < noOfRooms; i++)
            {
                if (i == 0)
                {
                    int chosenWidth = 50;
                    int chosenHeight = 50;
                    Rooms[i] = new Vector2(startingX + 145 + chosenWidth / 2, startingY + 5 + chosenHeight / 2);
                    RoomSizes[i] = new Vector2(chosenWidth, chosenHeight);
                    ClearRegion(50, 50, new Vector2(startingX + 145, startingY + 5));
                    isRoom[i] = true;
                }
                if (i != 0 && i != noOfRooms - 1)
                {
                    for (int j = 0; j < 500; j++)
                    {
                        int counter = 0;
                        int chosenWidth = WorldGen.genRand.Next(20, 35);
                        int chosenHeight = WorldGen.genRand.Next(15, 25);
                        float chosenX = startingX + WorldGen.genRand.Next(width - chosenWidth);
                        float chosenY = startingY + WorldGen.genRand.Next(height - chosenHeight - 55);
                        Rooms[i] = new Vector2(chosenX + chosenWidth / 2, chosenY + chosenHeight / 2);
                        RoomSizes[i] = new Vector2(chosenWidth, chosenHeight);
                        for (int k = 1; k <= i; k++)
                        {
                            if (
                               (Math.Abs(Rooms[i].X - Rooms[i - k].X) > RoomSizes[i].X / 2 + RoomSizes[i - k].X / 2 + 20 ||
                               Math.Abs(Rooms[i].Y - Rooms[i - k].Y) > RoomSizes[i].Y / 2 + RoomSizes[i - 1].Y / 2) &&
                               Math.Abs(Rooms[i].X - Rooms[i - 1].X) < RoomSizes[i].X / 2 + RoomSizes[i - 1].X / 2 + 50 &&
                               Math.Abs(Rooms[i].Y - Rooms[i - 1].Y) < RoomSizes[i].Y / 2 + RoomSizes[i - 1].Y / 2 + 2 &&
                               Rooms[i].Y - Rooms[i - 1].Y > 0)
                            {
                                counter++;
                            }
                        }
                        if (counter == i)
                        {
                            isRoom[i] = true;
                            ClearRegion(chosenWidth, chosenHeight, new Vector2(chosenX, chosenY));
                            break;
                        }
                        else
                        {
                            isRoom[i] = false;
                        }
                    }
                }
                if (i == noOfRooms - 1)
                {
                    int chosenWidth1 = 150;
                    int chosenHeight1 = 50;
                    Rooms[i] = new Vector2(startingX + 25 + chosenWidth1 / 2, startingY + height - 55 + chosenHeight1 / 2);
                    RoomSizes[i] = new Vector2(chosenWidth1, chosenHeight1);
                    ClearRegion(chosenWidth1, chosenHeight1, new Vector2(startingX + 25, startingY + height - 55));
                }
            }
            for (int j = 0; j < 2; j++)
            {
                for (int i = 1; i < noOfRooms - 1; i++)
                {
                    if (isRoom[i] && isRoom[i - 1])
                    {
                        if (j == 0)
                        {
                            MakePathWay(Rooms[i], Rooms[i - 1], RoomSizes[i], RoomSizes[i - 1], 9, false);
                        }

                        if (j == 1)
                        {
                            MakePathWay(Rooms[i], Rooms[i - 1], RoomSizes[i], RoomSizes[i - 1], 9, true);
                        }
                    }
                }
            }
            Hole((int)(Rooms[noOfRooms - 1].Y - RoomSizes[noOfRooms - 1].Y / 2 - (Rooms[noOfRooms - 2].Y + RoomSizes[noOfRooms - 2].Y / 2)) + 5, 10, new Vector2(Rooms[noOfRooms - 2].X, Rooms[noOfRooms - 2].Y + RoomSizes[noOfRooms - 2].Y / 2));

            FillWall(width, height, new Vector2(startingX, startingY), WallID.SandstoneBrick);

            // var TowerChestIndex = Chest.FindChest((int)Rooms[1].X, (int)Rooms[1].Y + (int)RoomSizes[1].Y / 2 - 1);
            //WorldGen.PlaceObject((int)Rooms[1].X, (int)Rooms[1].Y + (int)RoomSizes[1].Y/2-32, TileID);
            //  WorldGen.PlaceObject((int)Rooms[1].X, (int)Rooms[1].Y + (int)RoomSizes[1].Y / 2 - 0, TileID.Pianos);
            for (int i = 0; i < noOfRooms - 1; i++)
            {
                if (WorldGen.genRand.Next(3) == 0 && i != waterBoltRoom && i != fireRoom)
                {
                    MakePillar(Rooms[i], (int)RoomSizes[i].Y / 2, false, false);
                }

                if (i == waterBoltRoom && i != fireRoom)
                {
                    WorldGen.PlaceObject((int)Rooms[i].X, (int)Rooms[i].Y - (int)RoomSizes[i].Y / 2, TileID.Chandeliers, false, 2);
                    MakePillar(Rooms[i] + new Vector2(-RoomSizes[i].X / 4, 0), (int)RoomSizes[i].Y / 2 + 3, true, false);
                    MakePillar(Rooms[i] + new Vector2(RoomSizes[i].X / 4, 0), (int)RoomSizes[i].Y / 2 + 3, true, false);
                    FillRegion((int)RoomSizes[i].X / 2, 1, Rooms[i] + new Vector2(-RoomSizes[i].X / 4, 0), TileID.Platforms);
                    FillRegion((int)RoomSizes[i].X / 2 - 2, 1, Rooms[i] + new Vector2(-RoomSizes[i].X / 4 + 1, -1), TileID.Books);
                    WorldGen.PlaceObject((int)Rooms[i].X, (int)Rooms[i].Y + (int)RoomSizes[i].Y / 2, TileID.WaterFountain, true, 1);
                    WorldGen.PlaceObject(WorldGen.genRand.Next((int)Rooms[i].X - (int)RoomSizes[i].X / 4 + 1, (int)Rooms[i].X - (int)RoomSizes[i].X / 4 + 1 + (int)RoomSizes[i].X / 2 - 2), (int)Rooms[i].Y - 1, TileID.Books, false, 1);
                }
                if (i == fireRoom)
                {
                    MakePillar(Rooms[i], (int)RoomSizes[i].Y / 2 + 3, false, true);
                }
                int slit1 = WorldGen.genRand.Next(2, 4);
                int slit2 = WorldGen.genRand.Next(2, 4);
                int tablePos = (int)Rooms[i].X + WorldGen.genRand.Next(-(int)RoomSizes[i].X / 2, (int)RoomSizes[i].X / 2);
                if (i < 9)
                {
                    FillRegionDiag(slit1, slit1, Rooms[i] - new Vector2(RoomSizes[i].X / 2, RoomSizes[i].Y / 2), TileID.SandstoneBrick, 0);
                    FillRegionDiag(slit2, slit2, Rooms[i] - new Vector2(-RoomSizes[i].X / 2, RoomSizes[i].Y / 2), TileID.SandstoneBrick, 1);
                }
                if (i != waterBoltRoom)
                {
                    MakeGoldPile(new Vector2((int)Rooms[i].X + WorldGen.genRand.Next(-(int)RoomSizes[i].X / 2, (int)RoomSizes[i].X / 2), (int)Rooms[i].Y + (int)RoomSizes[i].Y / 2), WorldGen.genRand.Next(2));
                }

                MakeShelf(new Vector2(Rooms[i].X - RoomSizes[i].X / 2, Rooms[i].Y - WorldGen.genRand.Next((int)RoomSizes[i].Y / 2)), 0, Main.rand.Next(2, 6));
                MakeShelf(new Vector2(Rooms[i].X + RoomSizes[i].X / 2, Rooms[i].Y - WorldGen.genRand.Next((int)RoomSizes[i].Y / 2)), 1, Main.rand.Next(2, 6));
                //WorldGen.PlaceObject((int)Rooms[i].X + WorldGen.genRand.Next(-(int)RoomSizes[i].X / 2, (int)RoomSizes[i].X / 2), (int)Rooms[i].Y + (int)RoomSizes[i].Y / 2, TileID.Bathtubs, false, 26);
                if (i != waterBoltRoom && i != fireRoom)
                {
                    if (WorldGen.genRand.Next(1) == 0)
                    {
                        WorldGen.PlaceObject((int)Rooms[i].X + WorldGen.genRand.Next(-(int)RoomSizes[i].X / 2, (int)RoomSizes[i].X / 2), (int)Rooms[i].Y - (int)RoomSizes[i].Y / 2, TileID.Chandeliers, false, 2);
                    }

                    WorldGen.PlaceObject((int)Rooms[i].X + WorldGen.genRand.Next(-(int)RoomSizes[i].X / 2, (int)RoomSizes[i].X / 2), (int)Rooms[i].Y + (int)RoomSizes[i].Y / 2, TileID.Beds, false, 10);
                    WorldGen.PlaceObject(tablePos, (int)Rooms[i].Y + (int)RoomSizes[i].Y / 2, TileID.Tables, false, 18);
                    WorldGen.PlaceObject(tablePos + 2, (int)Rooms[i].Y + (int)RoomSizes[i].Y / 2, TileID.Chairs, false, 19);
                    WorldGen.PlaceObject(tablePos - 2, (int)Rooms[i].Y + (int)RoomSizes[i].Y / 2, TileID.Chairs, false, 19, 0, -1, 1);
                    WorldGen.PlaceObject((int)Rooms[i].X + WorldGen.genRand.Next(-(int)RoomSizes[i].X / 2, (int)RoomSizes[i].X / 2), (int)Rooms[i].Y + (int)RoomSizes[i].Y / 2 - 1, TileID.Containers, false, 2);
                    WorldGen.PlaceObject((int)Rooms[i].X + WorldGen.genRand.Next(-(int)RoomSizes[i].X / 2, (int)RoomSizes[i].X / 2), (int)Rooms[i].Y + (int)RoomSizes[i].Y / 2, TileID.GrandfatherClocks, false, 2);
                }
            }
            FirstRoomFirstVariation(new Vector2(Rooms[0].X - RoomSizes[0].X / 2 - 2, Rooms[0].Y + RoomSizes[0].Y / 2));
        }

        public static void ClearRegion(int width, int height, Vector2 startingPoint)
        {
            string messageBefore = EEMod.progressMessage;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Tile tile = Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    tile.ClearTile();
                    WorldGen.KillWall(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    EEMod.progressMessage = messageBefore;
                    EEMod.progressMessage += $" {(int)((j + (i * height)) / (float)(width * height) * 100)}% done";
                }
            }
        }

        public static void ClearRegionSafely(int width, int height, Vector2 startingPoint, int type)
        {
            string messageBefore = EEMod.progressMessage;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y).type == type)
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
                    Main.tile[i + (int)startingPoint.X, j + (int)startingPoint.Y].liquidType(0); // set liquid type 0 is water 1 lava 2 honey 3+ water iirc
                    Main.tile[i + (int)startingPoint.X, j + (int)startingPoint.Y].liquid = 255; // set liquid ammount
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
                        Main.tile[i + (int)startingPoint.X, j + (int)startingPoint.Y].liquidType(1); // set liquid type 0 is water 1 lava 2 honey 3+ water iirc
                        Main.tile[i + (int)startingPoint.X, j + (int)startingPoint.Y].liquid = 255; // set liquid ammount
                        WorldGen.SquareTileFrame(i + (int)startingPoint.X, j + (int)startingPoint.Y, true); // soemthing for astatic voiding the liquid from being static
                        if (Main.netMode == NetmodeID.MultiplayerClient) // sync
                        {
                            NetMessage.sendWater(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                        }
                    }
                }
            }
        }

        public static void MakeVolcanoEntrance(int i, int j, int[,] shape)
        {
            for (int y = 0; y < shape.GetLength(0); y++)
            {
                for (int x = 0; x < shape.GetLength(1); x++)
                {
                    int k = i - 3 + x;
                    int l = j - 6 + y;
                    if (WorldGen.InWorld(k, l, 30))
                    {
                        Tile tile = Framing.GetTileSafely(k, l);
                        //tile.ClearTile();
                        switch (shape[y, x])
                        {
                            case 0:
                                break;

                            case 1:
                                WorldGen.PlaceTile(k, l, ModContent.TileType<VolcanicAshTile>());
                                break;

                            case 2:
                                WorldGen.PlaceWall(k, l, WallID.DiamondGemspark);
                                tile.wallColor(29);
                                break;
                        }
                    }
                }
            }
        }

        public static void GenerateLuminite()
        {
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesX; j++)
                {
                    Tile tile = Framing.GetTileSafely(i, j);
                    if (tile.type == TileID.Stone)
                    {
                        if (Main.rand.NextBool(2000))
                        {
                            WorldGen.TileRunner(i, j, 10, 10, TileID.LunarOre);
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
                    if (Main.tile[i + (int)startingPoint.X, j + (int)startingPoint.Y].liquidType() == 0 && Main.tile[i + (int)startingPoint.X, j + (int)startingPoint.Y].liquid > 64)
                    {
                        Main.tile[i + (int)startingPoint.X, j + (int)startingPoint.Y].ClearEverything();
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
            Tile tile1 = Framing.GetTileSafely(i, j);
            Tile tile2 = Framing.GetTileSafely(i, j - 1);
            Tile tile3 = Framing.GetTileSafely(i, j - 2);
            Tile tile4 = Framing.GetTileSafely(i, j + 1);
            Tile tile5 = Framing.GetTileSafely(i, j + 2);
            Tile tile6 = Framing.GetTileSafely(i - 1, j);
            Tile tile7 = Framing.GetTileSafely(i - 2, j);
            Tile tile8 = Framing.GetTileSafely(i + 1, j);
            Tile tile9 = Framing.GetTileSafely(i + 2, j);
            if (tile1.active() && tile2.active() && tile3.active() && !tile4.active() && !tile5.active() && tile1.slope() == 0)
            {
                return 1;
            }
            if (tile1.active() && !tile2.active() && !tile3.active() && tile4.active() && tile5.active() && tile1.slope() == 0)
            {
                return 2;
            }
            if (tile1.active() && tile6.active() && tile7.active() && !tile8.active() && !tile9.active())
            {
                return 3;
            }
            if (tile1.active() && !tile6.active() && !tile7.active() && tile8.active() && tile9.active())
            {
                return 4;
            }
            else
            {
                return 0;
            }
        }

        public static bool CheckRangeRight(int i, int j, int length, bool opposite = false)
        {
            for (int k = 0; k < length; k++)
            {
                if (WorldGen.InWorld(i + (opposite ? -k : k), j, 20))
                {
                    if (!Framing.GetTileSafely(i + (opposite ? -k : k), j).active() || !Main.tileSolid[Framing.GetTileSafely(i + (opposite ? -k : k), j).type] || Framing.GetTileSafely(i + (opposite ? -k : k), j).type == ModContent.TileType<EmptyTile>())
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
                    if (!Framing.GetTileSafely(i, j + (opposite ? -k : k)).active() || !Main.tileSolid[Framing.GetTileSafely(i, j + (opposite ? -k : k)).type] || Framing.GetTileSafely(i + (opposite ? -k : k), j).type == ModContent.TileType<EmptyTile>())
                        return false;
                }
            }
            return true;
        }

        public static int WaterCheck(int i, int j)
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
            if (tile1.active() && tile2.active() && tile3.active() && !tile4.active() && !tile5.active())
            {
                return 1;
            }
            if (tile1.active() && !tile2.active() && !tile3.active() && tile4.active() && tile5.active())
            {
                return 2;
            }
            if (tile1.active() && tile6.active() && tile7.active() && !tile8.active() && !tile9.active())
            {
                return 3;
            }
            if (tile1.active() && !tile6.active() && !tile7.active() && tile8.active() && tile9.active())
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

        public static void MakeJaggedOval(int width, int height, Vector2 startingPoint, int type, bool forced)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (OvalCheck((int)(startingPoint.X + width / 2), (int)(startingPoint.Y + height / 2), i + (int)startingPoint.X, j + (int)startingPoint.Y, (int)(width * .5f), (int)(height * .5f)))
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

        public delegate bool NoiseConditions(Vector2 point);

        public static void NoiseGen(Vector2 topLeft, Vector2 size, Vector2 dimensions, float thresh, ushort type, NoiseConditions noiseFilter = null)
        {
            perlinNoise = new PerlinNoiseFunction((int)size.X, (int)size.Y, (int)dimensions.X, (int)dimensions.Y, thresh);
            int[,] perlinNoiseFunction = perlinNoise.perlinBinary;
            for (int i = (int)topLeft.X; i < (int)topLeft.X + (int)size.X; i++)
            {
                for (int j = (int)topLeft.Y; j < (int)topLeft.Y + (int)size.Y; j++)
                {
                    //Tile tile = Framing.GetTileSafely(i, j);
                    if (perlinNoiseFunction[i - (int)topLeft.X, j - (int)topLeft.Y] == 1)
                    {
                        WorldGen.PlaceTile(i, j, type);
                    }
                }
            }
        }

        public static void NoiseGenWave(Vector2 topLeft, Vector2 size, Vector2 dimensions, ushort type, float thresh, NoiseConditions noiseFilter = null)
        {
            PerlinNoiseFunction perlinNoise = new PerlinNoiseFunction((int)size.X, (int)size.Y, (int)dimensions.X, (int)dimensions.Y, thresh);
            int[,] perlinNoiseFunction = perlinNoise.perlinBinary;
            float[] disp = PerlinArrayNoZero((int)size.X, size.Y * 0.5f, new Vector2(50, 100));
            for (int i = (int)topLeft.X; i < (int)topLeft.X + (int)size.X; i++)
            {
                for (int j = (int)topLeft.Y + (int)disp[i - (int)topLeft.X]; j < (int)topLeft.Y + (int)size.Y; j++)
                {
                    //Tile tile = Framing.GetTileSafely(i, j);
                    if (perlinNoiseFunction[i - (int)topLeft.X, j - (int)topLeft.Y] == 1)
                    {
                        WorldGen.PlaceTile(i, j, type);
                        if (j < Main.maxTilesY * 0.33f)
                        {
                            WorldGen.PlaceTile(i, j, (ushort)ModContent.TileType<LightGemsandTile>());
                        }
                        else if (j < Main.maxTilesY * 0.66f)
                        {
                            WorldGen.PlaceTile(i, j, (ushort)ModContent.TileType<GemsandTile>());
                        }
                        else if (j > Main.maxTilesY * 0.66f)
                        {
                            WorldGen.PlaceTile(i, j, (ushort)ModContent.TileType<DarkGemsandTile>());
                        }
                        if (j < Main.maxTilesY / 10)
                        {
                            WorldGen.PlaceTile(i, j, (ushort)ModContent.TileType<CoralSandTile>());
                        }
                    }
                }
            }
        }

        public static void MakeTriangle(Vector2 startingPoint, int width, int height, int slope, int type, bool isFlat = false, bool hasChasm = false, int wallType = 0)
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
            int topRight = (int)startingPoint.Y - height;
            if (hasChasm)
            {
                MakeChasm((int)(startingPoint.X + width / 2), topRight + (height / (slope * 10)), height - 30, TileID.StoneSlab, 0, 10, 20);

                for (int i = 0; i < Main.maxTilesX; i++)
                {
                    for (int j = 0; j < Main.maxTilesY; j++)
                    {
                        Tile tile = Framing.GetTileSafely(i, j);
                        if (tile.type == TileID.StoneSlab)
                        {
                            WorldGen.KillTile(i, j);
                            if (wallType != 0)
                            {
                                tile.wall = (ushort)wallType;
                            }
                        }
                    }
                }
            }
            if (isFlat)
            {
                ClearRegion(initialWidth, height / 5, new Vector2(initialStartingPosX, topRight - 5));
                KillWall(initialWidth, height / 5, new Vector2(initialStartingPosX, topRight - 5));
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
                    tile.active(true);
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
                    Override)
                    ;
                if (withBranches)
                {
                    if (i % branchFrequency == 0 && WorldGen.genRand.Next(2) == 0)
                    {
                        int Side = Main.rand.Next(0, 2);
                        if (Side == 0)
                        {
                            Vector2 NormalizedGradVec = Vector2.Normalize(position2 - position1).RotatedBy(MathHelper.PiOver2 + Main.rand.NextFloat(-0.3f, 0.3f));
                            //int ChanceForRecursion = Main.rand.Next(0, 4);
                            MakeWavyChasm3(currentPos, currentPos + NormalizedGradVec * lengthOfBranches, type, 100, 10, true, new Vector2(0, 5), 2, 5, true, 50, (int)(lengthOfBranches * 0.5f));
                        }
                        if (Side == 1)
                        {
                            Vector2 NormalizedGradVec = Vector2.Normalize(position2 - position1).RotatedBy(-MathHelper.PiOver2);
                            //int ChanceForRecursion = Main.rand.Next(0, 4);
                            MakeWavyChasm3(currentPos, currentPos + NormalizedGradVec * lengthOfBranches, type, 100, 10, true, new Vector2(0, 5), 7, 5, true, 50, (int)(lengthOfBranches * 0.5f));
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
                Tile tile = Framing.GetTileSafely(positionX, i);
                if (a == maxIterations)
                {
                    return 0;
                }
                if (tile.active() && Main.tileSolid[tile.type])
                {
                    return i;
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

        public static void KillWall(int width, int height, Vector2 startingPoint)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    WorldGen.KillWall(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                }
            }
        }

        public static bool OvalCheck(int midX, int midY, int x, int y, int sizeX, int sizeY)
        {
            double p = Math.Pow(x - midX, 2) / Math.Pow(sizeX, 2)
                    + Math.Pow(y - midY, 2) / Math.Pow(sizeY, 2);

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
                     // if (Vector2.DistanceSquared(new Vector2(x, y), new Vector2(X, midY)) < (sizeSQ) && tile.active())
                        //  WorldGen.TileRunner(x, y, WorldGen.genRand.Next(4, 10), WorldGen.genRand.Next(5, 10), ModContent.TileType<HardenedGemsandTile>(), true, 0f, 0f, true, true);
                  }
                  if (layer == 2)
                  {
                     // if (OvalCheck(X, midY, x, y, (size * 3) + 10, (size) + 10) && tile.active())
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
                    if (OvalCheck(X, midY, x, y, (size * 3) + 10, size + 10) && tile.active())
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

        public static void PlaceRuins(int i, int j, int[,] shape)
        {
            for (int y = 0; y < shape.GetLength(0); y++)
            {
                for (int x = 0; x < shape.GetLength(1); x++)
                {
                    int k = i - 3 + x;
                    int l = j - 6 + y;
                    if (WorldGen.InWorld(k, l, 30))
                    {
                        Tile tile = Framing.GetTileSafely(k, l);
                        tile.ClearTile();
                        switch (shape[y, x])
                        {
                            case 1:
                                WorldGen.PlaceTile(k, l, TileID.GrayBrick);
                                tile.active(true);
                                break;

                            case 2:
                                WorldGen.PlaceTile(k, l, TileID.Stone);
                                tile.active(true);
                                break;

                            case 5:
                                WorldGen.PlaceTile(k, l, TileID.HangingLanterns);
                                tile.active(true);
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
        }

        public static Vector2 ChestPos = Vector2.Zero;

        public static void PlaceAnyBuilding(int i, int j, int[,,] shape)
        {
            ChestPos = Vector2.Zero;
            for (int y = 0; y < shape.GetLength(0); y++)
            {
                for (int x = 0; x < shape.GetLength(1); x++)
                {
                    int k = i - 3 + x;
                    int l = j - 6 + y;
                    if (WorldGen.InWorld(k, l, 30))
                    {
                        Tile tile = Framing.GetTileSafely(k, l);

                        if (shape[y, x, 0] != 0)
                        {
                            if (shape[y, x, 0] == ModContent.TileType<GemsandChestTile>() && ChestPos == Vector2.Zero)
                            {
                                ChestPos = new Vector2(k, l);
                                for (int u = k; u < k + 2; u++)
                                {
                                    for (int p = l; p < l + 2; p++)
                                    {
                                        tile.type = 0;
                                        tile.active(false);
                                    }
                                }
                            }

                            if (shape[y, x, 0] != ModContent.TileType<GemsandChestTile>())
                            {
                                tile.type = (ushort)shape[y, x, 0];
                                tile.active(true);
                                tile.wall = (ushort)shape[y, x, 1];
                                tile.color((byte)shape[y, x, 2]);
                                tile.slope((byte)shape[y, x, 3]);
                                tile.wallColor((byte)shape[y, x, 4]);
                                if ((byte)shape[y, x, 5] == 1)
                                {
                                    tile.inActive(true);
                                }
                                else
                                {
                                    tile.inActive(false);
                                }
                                if ((byte)shape[y, x, 6] > 0)
                                {
                                    tile.liquid = (byte)shape[y, x, 6];
                                    tile.liquidType((byte)shape[y, x, 7]);
                                }
                                tile.frameX = (byte)shape[y, x, 8];
                                tile.frameY = (byte)shape[y, x, 9];
                            }

                            /*Debug.WriteLine("saifnaskdlfjnasldfjnalkdsfjnfalksjdfnalksjdnfalkdjnflaksdjfnalkdjfnakldjfnakldjfnalkjsdnflajsdnflakjsdnfklajsndf");
                            WorldGen.PlaceChest(k, l, (ushort)ModContent.TileType<GemsandChestTile>());*/
                        }
                    }
                }
            }
            if (ChestPos != Vector2.Zero)
            {
                WorldGen.PlaceChest((int)ChestPos.X, (int)ChestPos.Y, 21);
                Debug.WriteLine("Chest Placed");
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
    }
}