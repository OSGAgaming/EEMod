﻿using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;
using Microsoft.Xna.Framework;
using EEMod.Tiles;
using EEMod.Tiles.Furniture;
using EEMod.Tiles.Ores;

namespace EEMod.EEWorld
{
    public partial class EEWorld
    {
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
            else if (Main.rand.Next(3) == 0)
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
            progress.Message = "Interitos Mod Ores";
            int maxTiles = Main.maxTilesX * Main.maxTilesY;
            int rockLayerLow = (int)WorldGen.rockLayerLow;
            int OreAmmount;

            OreAmmount = (int)(maxTiles * 0.00008);
            for (int k = 0; k < OreAmmount; k++)
            {
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next(rockLayerLow, Main.maxTilesY);
                WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(5, 7), ModContent.TileType<LythenOreTile>());

                x = WorldGen.genRand.Next(0, Main.maxTilesX);
                y = WorldGen.genRand.Next(rockLayerLow, Main.maxTilesY);
                WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(5, 7), ModContent.TileType<DalantiniumOreTile>());
            }
        }


        public static void FillRegion(int width, int height, Vector2 startingPoint, int type)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    WorldGen.PlaceTile(i + (int)startingPoint.X, j + (int)startingPoint.Y, type);
                }
            }
        }
        public static void FillRegionNoEdit(int width, int height, Vector2 startingPoint, int type)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    WorldGen.TileRunner(i + (int)startingPoint.X, j + (int)startingPoint.Y,30,20, type,false,0,0,false,true);
                }
            }
        }

        public static void FillWall(int width, int height, Vector2 startingPoint, int type)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    WorldGen.PlaceWall(i + (int)startingPoint.X, j + (int)startingPoint.Y, type);
                }
            }
        }
        private static void FillRegionDiag(int width, int height, Vector2 startingPoint, int type, int leftOrRight)
        {
            if (leftOrRight == 0)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height - i; j++)
                    {
                        WorldGen.PlaceTile(i + (int)startingPoint.X, j + (int)startingPoint.Y, type);
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
                    }
                }
            }
        }

        public static void ClearRegion(int width, int height, Vector2 startingPoint)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    WorldGen.KillTile(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    WorldGen.KillWall(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                }
            }
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
                            MakePillarWalls(new Vector2(i + (int)startingPoint.X, +(int)startingPoint.Y + (int)(i * gradient) - 1), 11);
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
                        Hole((int)(firstRoomDoorPos.Y - secondRoomDoorPos.Y), 5, new Vector2(firstRoomDoorPos.X - firstRoomSize.X / 2, secondRoomDoorPos.Y));
                    else
                        Hole((int)(secondRoomDoorPos.Y - firstRoomDoorPos.Y), 5, new Vector2(secondRoomDoorPos.X - secondRoomSize.X / 2, firstRoomDoorPos.Y));
                }
            }
            else
            {
                float gradient = (secondRoomDoorPos.Y - firstRoomDoorPos.Y) / (secondRoomDoorPos.X - secondRoomSize.X / 2 - (firstRoomDoorPos.X + firstRoomSize.X / 2));
                ClearPathWay((int)(secondRoomDoorPos.X - secondRoomSize.X / 2) - (int)(firstRoomDoorPos.X + firstRoomSize.X / 2) + 1, heightOfConnection, gradient, firstRoomDoorPos + new Vector2(firstRoomSize.X / 2, 0), withPillars);
                if (secondRoomDoorPos.X - secondRoomSize.X / 2 - (int)(firstRoomDoorPos.X + firstRoomSize.X / 2) <= 4)
                {
                    if (secondRoomDoorPos.Y < firstRoomDoorPos.Y)
                        Hole((int)(firstRoomDoorPos.Y - secondRoomDoorPos.Y), 5, new Vector2(firstRoomDoorPos.X + firstRoomSize.X / 2, secondRoomDoorPos.Y));
                    else
                        Hole((int)(secondRoomDoorPos.Y - firstRoomDoorPos.Y), 5, new Vector2(secondRoomDoorPos.X + secondRoomSize.X / 2, firstRoomDoorPos.Y));
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
                        //tile.ClearTile();
                        switch (shape[y, x])
                        {
                            case 0:
                                //WorldGen.KillTile(k, l, false, false, true);
                                break;
                            case 1:
                                tile.type = TileID.WoodBlock;
                                tile.active(true);
                                break;
                            case 2:
                                tile.type = TileID.RichMahogany;
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
                                break;
                            case 3:
                                tile.type = TileID.WebRope;
                                tile.active(true);
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
        private static void MakePillar(Vector2 startingPos, int height, bool water, bool fire)
        {

            if (water)
            {
                Main.tile[(int)startingPos.X - 1, (int)startingPos.Y - 3].liquidType(0); // set liquid type 0 is water 1 lava 2 honey 3+ water iirc
                Main.tile[(int)startingPos.X - 1, (int)startingPos.Y - 3].liquid = 255; // set liquid ammount
                Main.tile[(int)startingPos.X - 1, (int)startingPos.Y - 4].liquid = 255;
                WorldGen.SquareTileFrame((int)startingPos.X - 1, (int)startingPos.Y - 3, true); // soemthing for astatic voiding the liquid from being static
                if (Main.netMode == NetmodeID.MultiplayerClient) // sync
                    NetMessage.sendWater((int)startingPos.X - 1, (int)startingPos.Y - 3);
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
                var tile = Framing.GetTileSafely((int)startingPos.X - 2, (int)startingPos.Y - 3);
                var tile1 = Framing.GetTileSafely((int)startingPos.X - 2 + 4, (int)startingPos.Y - 3);
                tile.halfBrick(true);
                tile1.halfBrick(true);
            }
            MakePillarWalls(new Vector2(startingPos.X + 2 - 2, startingPos.Y + 2 - 4), height);
        }

        private static void MakePillarWalls(Vector2 startingPos, int height)
        {
            var tile1 = Framing.GetTileSafely((int)startingPos.X + 1, (int)startingPos.Y);
            var tile2 = Framing.GetTileSafely((int)startingPos.X + 0, (int)startingPos.Y);
            var tile3 = Framing.GetTileSafely((int)startingPos.X + -1, (int)startingPos.Y);
            var tile4 = Framing.GetTileSafely((int)startingPos.X + 1, (int)startingPos.Y + height + 1);
            var tile5 = Framing.GetTileSafely((int)startingPos.X + 0, (int)startingPos.Y + height + 1);
            var tile6 = Framing.GetTileSafely((int)startingPos.X + -1, (int)startingPos.Y + height + 1);
            if (tile1.active() && tile2.active() && tile3.active() && tile4.active() && tile5.active() && tile6.active())
            {
                for (int i = -1; i < 2; i++)
                {

                    for (int j = 0; j < height; j++)
                    {
                        var tile = Framing.GetTileSafely((int)startingPos.X + i, (int)startingPos.Y + j);
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
                var tile = Framing.GetTileSafely((int)startingPos.X - 1, (int)startingPos.Y);
                var tile1 = Framing.GetTileSafely((int)startingPos.X, (int)startingPos.Y);
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
                var tile = Framing.GetTileSafely((int)startingPos.X + 1, (int)startingPos.Y);
                var tile1 = Framing.GetTileSafely((int)startingPos.X, (int)startingPos.Y);
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

            for (var x = 0; x < TowerTiles.GetLength(1); x++)
            {
                for (var y = 0; y < TowerTiles.GetLength(0); y++)
                {
                    var tile = Framing.GetTileSafely(RoomPosX + x, RoomPosY - y);
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
            PlaceShip(100, TileCheck(100) - 22, ShipTiles);
            PlaceShipWalls(100, TileCheck(100) - 22, ShipWalls);
            ree = new Vector2(100, TileCheck(100) - 22);
        }
        public static int TileCheck(int positionX)
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
                    break;
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
                            MakePathWay(Rooms[i], Rooms[i - 1], RoomSizes[i], RoomSizes[i - 1], 9, false);
                        if (j == 1)
                            MakePathWay(Rooms[i], Rooms[i - 1], RoomSizes[i], RoomSizes[i - 1], 9, true);
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
                    MakePillar(Rooms[i], (int)RoomSizes[i].Y / 2, false, false);

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
                    MakeGoldPile(new Vector2((int)Rooms[i].X + WorldGen.genRand.Next(-(int)RoomSizes[i].X / 2, (int)RoomSizes[i].X / 2), (int)Rooms[i].Y + (int)RoomSizes[i].Y / 2), WorldGen.genRand.Next(2));
                MakeShelf(new Vector2(Rooms[i].X - RoomSizes[i].X / 2, Rooms[i].Y - WorldGen.genRand.Next((int)RoomSizes[i].Y / 2)), 0, Main.rand.Next(2, 6));
                MakeShelf(new Vector2(Rooms[i].X + RoomSizes[i].X / 2, Rooms[i].Y - WorldGen.genRand.Next((int)RoomSizes[i].Y / 2)), 1, Main.rand.Next(2, 6));
                //WorldGen.PlaceObject((int)Rooms[i].X + WorldGen.genRand.Next(-(int)RoomSizes[i].X / 2, (int)RoomSizes[i].X / 2), (int)Rooms[i].Y + (int)RoomSizes[i].Y / 2, TileID.Bathtubs, false, 26);
                if (i != waterBoltRoom && i != fireRoom)
                {
                    if (WorldGen.genRand.Next(1) == 0)
                        WorldGen.PlaceObject((int)Rooms[i].X + WorldGen.genRand.Next(-(int)RoomSizes[i].X / 2, (int)RoomSizes[i].X / 2), (int)Rooms[i].Y - (int)RoomSizes[i].Y / 2, TileID.Chandeliers, false, 2);
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




        private static void clearRegion(int width, int height, Vector2 startingPoint)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    WorldGen.KillTile(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    WorldGen.KillWall(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                }
            }
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    WorldGen.PlaceWall(i + (int)startingPoint.X, j + (int)startingPoint.Y + 1060, WallID.CorruptionUnsafe1);
                }
            }

        }
        public static void fillRegionWithWater(int width, int height, Vector2 startingPoint)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Main.tile[i + (int)startingPoint.X, j + (int)startingPoint.Y].liquidType(0); // set liquid type 0 is water 1 lava 2 honey 3+ water iirc
                    Main.tile[i + (int)startingPoint.X, j + (int)startingPoint.Y].liquid = 255; // set liquid ammount
                    WorldGen.SquareTileFrame(i + (int)startingPoint.X, j + (int)startingPoint.Y, true); // soemthing for astatic voiding the liquid from being static
                    if (Main.netMode == NetmodeID.MultiplayerClient) // sync
                        NetMessage.sendWater(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                }
            }
        }
        private static int tileCheck2(int i, int j)
        {
            Tile tile1 = Framing.GetTileSafely(i, j);
            Tile tile2 = Framing.GetTileSafely(i, j - 1);
            Tile tile3 = Framing.GetTileSafely(i, j - 2);
            Tile tile4 = Framing.GetTileSafely(i, j + 1);
            Tile tile5 = Framing.GetTileSafely(i, j + 2);
            if (tile1.active() && tile2.active() && tile3.active() && !tile4.active() && !tile5.active())
            {
                return 1;
            }
            if (tile1.active() && !tile2.active() && !tile3.active() && tile4.active() && tile5.active())
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }
        private static void makeOvalFlatTop(int width, int height, Vector2 startingPoint, int type)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (OvalCheck((int)(startingPoint.X + width / 2), (int)(startingPoint.Y + height / 2), i + (int)startingPoint.X, j + (int)startingPoint.Y, (int)(width * .5f), (int)(height * .5f)))
                        WorldGen.PlaceTile(i + (int)startingPoint.X, j + (int)startingPoint.Y, type);

                    if (i == width / 2 && j == height / 2)
                    {
                        WorldGen.TileRunner(i + (int)startingPoint.X, j + (int)startingPoint.Y + 2, WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(10, 20), type, true, 0f, 0f, true, true);
                    }
                }
            }
            for (int i = 0; i < width; i++)
            {
                for (int j = -6; j < height / 2 - 2; j++)
                {
                    Tile tile = Framing.GetTileSafely(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                    if (tile.type == type)
                        WorldGen.KillTile(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                }
            }
        }
        private static void fillRegion(int width, int height, Vector2 startingPoint, int type)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    WorldGen.PlaceTile(i + (int)startingPoint.X, j + (int)startingPoint.Y, type);
                }
            }
        }
        private static void fillWall(int width, int height, Vector2 startingPoint, int type)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    WorldGen.PlaceWall(i + (int)startingPoint.X, j + (int)startingPoint.Y, type);
                }
            }
        }
        // ---- overload
        private static void fillRegion(int width, int height, Vector2 startingPoint, int type1, int type2)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int tileType;
                    if (Main.rand.Next(100) < 95)
                        tileType = type1;
                    else
                        tileType = type2;

                    WorldGen.PlaceTile(i + (int)startingPoint.X, j + (int)startingPoint.Y, tileType);
                }
            }
        }
        private static void makeChasm(int positionX, int positionY, int height, int type, float slant, int sizeAddon, int stepAddon)
        {
            for (int i = 0; i < height; i++)
            {
                // Tile tile = Framing.GetTileSafely(positionX + (int)(i * slant), positionY + i);
                WorldGen.TileRunner(positionX + (int)(i * slant), positionY + i, WorldGen.genRand.Next(5 + sizeAddon / 2, 10 + sizeAddon), WorldGen.genRand.Next(5, 10) + stepAddon, type, true, 0f, 0f, true, true);
            }
        }
        private static void makeWavyChasm(int positionX, int positionY, int height, int type, float slant, int sizeAddon)
        {
            for (int i = 0; i < height; i++)
            {
                // Tile tile = Framing.GetTileSafely(positionX + (int)(i * slant), positionY + i);
                WorldGen.TileRunner(positionX + (int)(i * slant) + (int)(Math.Sin(i / (float)50) * (20*(1+(i*1.5f/(float)height)))), positionY + i, WorldGen.genRand.Next(5 + sizeAddon / 2, 10 + sizeAddon), WorldGen.genRand.Next(5, 10), type, true, 0f, 0f, true, true);
            }
        }
        private static void makeWavyChasm2(int positionX, int positionY, int height, int type, float slant, int sizeAddon)
        {
            for (int i = 0; i < height; i++)
            {
                // Tile tile = Framing.GetTileSafely(positionX + (int)(i * slant), positionY + i);
                WorldGen.TileRunner(positionX + (int)(i * slant) + (int)(Math.Sin(i / (float)50) * (20 * (1 + (i * 1.5f / (float)height)))), positionY + i, WorldGen.genRand.Next(5 + sizeAddon / 2, 10 + sizeAddon), WorldGen.genRand.Next(20, 40), type, true, 0f, 0f, true, true);
            }
        }
        private static int tileCheck(int positionX)
        {
            for (int i = 0; i < Main.maxTilesY; i++)
            {
                Tile tile = Framing.GetTileSafely(positionX, i);
                if (tile.type == TileID.Sand)
                {
                    return i;
                }
            }
            return 0;
        }
        private static void killWall(int width, int height, Vector2 startingPoint)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    WorldGen.KillWall(i + (int)startingPoint.X, j + (int)startingPoint.Y);
                }
            }
        }

        private static bool OvalCheck(int h, int k, int x, int y, int a, int b)
        {
            double p = Math.Pow(x - h, 2) / Math.Pow(a, 2)
                    + Math.Pow(y - k, 2) / Math.Pow(b, 2);

            return p < 1 ? true : false;
        }

        private static void makeLayer(int X, int midY, int size, int layer)
        {

            int maxTiles = (int)(Main.maxTilesX * Main.maxTilesY * 9E-04);
            for (int k = 0; k < maxTiles*5; k++)
            {
                int x = WorldGen.genRand.Next(X - 80, X + 80);
                int y = WorldGen.genRand.Next(midY -  80, midY + 80);
                // Tile tile = Framing.GetTileSafely(x, y);
                if (layer == 1)
                {
                    if (Vector2.DistanceSquared(new Vector2(x, y), new Vector2(X, midY)) < size * size)
                        WorldGen.TileRunner(x, y, WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(5, 10), TileID.StoneSlab, true, 0f, 0f, true, true);
                }
                if (layer == 2)
                {
                    if (OvalCheck(X, midY, x, y, size * 3, size))
                        WorldGen.TileRunner(x, y, WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(5, 10), TileID.StoneSlab, true, 0f, 0f, true, true);
                }
            }
            for (int i = 0; i < 800; i++)
            {
                for (int j = 0; j < 2000; j++)
                {
                    Tile tile = Framing.GetTileSafely(i, j);
                    if (tile.type == TileID.StoneSlab)
                        WorldGen.KillTile(i, j);
                }
            }
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
                WorldGen.TileRunner(X, midY, WorldGen.genRand.Next(30, 50), WorldGen.genRand.Next(10, 20), ModContent.TileType<HardenedGemsandTile>(), true, 1f, 1f, false, true);
        }

        public static void CoralReef()
        {
            int chasmX = 100;
            int chasmY = 100;
            int sizeOfLayer1 = 80;
            int sizeOfLayer2 = 100;
            int firstLayerPosY = 220;
            int secondLayerPosY = 350;
            //clearRegion(chasmX, 1000, new Vector2(0, tileCheck(chasmX)));
            makeWavyChasm(chasmX, chasmY, 1000, TileID.StoneSlab, 0.3f, WorldGen.genRand.Next(50, 60));
            makeWavyChasm2(chasmX-50, chasmY, 1000, ModContent.TileType<HardenedGemsandTile>(), 0.3f, WorldGen.genRand.Next(10, 20));
            makeWavyChasm2(chasmX + 50, chasmY, 1000, ModContent.TileType<HardenedGemsandTile>(), 0.3f, WorldGen.genRand.Next(10, 20));
            for (int i = 0; i < 5; i++)
            {
                makeChasm(chasmX + Main.rand.Next(-50, 50) + i * 20, chasmY + (i * 200) + Main.rand.Next(-50, 50), Main.rand.Next(5, 30), TileID.StoneSlab, Main.rand.Next(5, 10), WorldGen.genRand.Next(20, 60), Main.rand.Next(10, 20));


            }
            for(int i = 0; i<20; i++)
            {
                makeOvalFlatTop(Main.rand.Next(10, 20), Main.rand.Next(5, 10), new Vector2(chasmX + Main.rand.Next(-10, 10) + i * 15, chasmY + (i * 50) + Main.rand.Next(-20, 20)), ModContent.TileType<HardenedGemsandTile>());
                if (i % 5 == 0)
                {
                    makeLayer(chasmX + Main.rand.Next(-10, 10) + i * 15, chasmY + Main.rand.Next(-20, 20) + (i * 50), 25, 2);

                    for (int j = 0; j < 7; j++)
                        makeOvalFlatTop(WorldGen.genRand.Next(20, 30), WorldGen.genRand.Next(5, 10), new Vector2(chasmX + Main.rand.Next(-10, 10) + i * 15 + (j * 35) - 50, chasmY + Main.rand.Next(-20, 20) + (i * 50)), ModContent.TileType<GemsandstoneTile>());
                }
            }
            
            float grad = WorldGen.genRand.NextFloat(0, 1);
            //  killWall(1000, 500, new Vector2(0, 0));
            //makeLayer(chasmX, chasmY + firstLayerPosY, sizeOfLayer1, 1);
            //makeLayer(chasmX + 30, chasmY + secondLayerPosY + sizeOfLayer1, sizeOfLayer2, 2);
            //---------------------------1
            //makeChasm(chasmX, chasmY + firstLayerPosY + sizeOfLayer1, 100, TileID.StoneSlab, grad, WorldGen.genRand.Next(10, 20));
            //makeChasm(chasmX - 15, chasmY + firstLayerPosY + sizeOfLayer1, 80 * (int)(1 + grad / 2), ModContent.TileType<HardenedGemsandTile>(), grad, WorldGen.genRand.Next(5, 10));
            //makeChasm(chasmX + 15, chasmY + firstLayerPosY + sizeOfLayer1, 80 * (int)(1 + grad / 2), ModContent.TileType<HardenedGemsandTile>(), grad, WorldGen.genRand.Next(5, 10));
            //---------------------------2
            //makeWavyChasm(chasmX, chasmY + secondLayerPosY + sizeOfLayer2 + 60, 150, TileID.StoneSlab, grad, WorldGen.genRand.Next(20, 30));
            //makeWavyChasm(chasmX - 25, chasmY + secondLayerPosY + sizeOfLayer2 + 60, 130 * (int)(1 + grad / 2), ModContent.TileType<GemsandstoneTile>(), grad, WorldGen.genRand.Next(5, 10));
           // makeWavyChasm(chasmX + 25, chasmY + secondLayerPosY + sizeOfLayer2 + 60, 130 * (int)(1 + grad / 2), ModContent.TileType<GemsandstoneTile>(), grad, WorldGen.genRand.Next(5, 10));
           /* for (int i = 0; i < 7; i++)
                makeOvalFlatTop(WorldGen.genRand.Next(20, 30), WorldGen.genRand.Next(5, 10), new Vector2(chasmX - 100 + (i * 35), chasmY + sizeOfLayer1 + 350 + WorldGen.genRand.Next(-10, 0)), ModContent.TileType<GemsandstoneTile>());*/
            for (int i = 0; i < 800; i++)
            {
                for (int j = 0; j < 2000; j++)
                {
                    Tile tile = Framing.GetTileSafely(i, j);
                    if (tile.type == TileID.StoneSlab)
                        WorldGen.KillTile(i, j);
                }
            }
            for (int i = 0; i < 500; i++)
            {
                for (int j = 0; j < 1500; j++)
                {
                    int yes = WorldGen.genRand.Next(5, 10);
                    if (tileCheck2(i, chasmY + j) == 1 && j % yes == 0)
                    {
                        int selection = WorldGen.genRand.Next(2);
                        switch (selection)
                        {
                            case 0:
                                WorldGen.PlaceTile(i, chasmY + j + 1, ModContent.TileType<CoralLanternTile>());
                                break;
                            case 1:
                                WorldGen.PlaceTile(i, chasmY + j + 1, ModContent.TileType<HangingCoralTile>());
                                break;
                        }
                    }
                    if (tileCheck2(i, chasmY + j) == 2 && j % yes <= 4)
                    {
                        int selection = WorldGen.genRand.Next(6);
                        switch (selection)
                        {
                            case 0:
                                WorldGen.PlaceTile(i, chasmY + j - 1, ModContent.TileType<Coral1Tile>());
                                break;
                            case 1:
                                WorldGen.PlaceTile(i, chasmY + j - 1, ModContent.TileType<Coral2Tile>());
                                break;
                            case 2:
                                WorldGen.PlaceTile(i, chasmY + j - 1, ModContent.TileType<Coral3Tile>());
                                break;
                            case 3:
                                WorldGen.PlaceTile(i, chasmY + j - 1, ModContent.TileType<EyeTile>());
                                break;
                            case 4:
                                WorldGen.PlaceTile(i, chasmY + j - 1, ModContent.TileType<CoralLanternLamp>());
                                break;
                            case 5:
                                WorldGen.PlaceTile(i, chasmY + j - 1, ModContent.TileType<BrainTile>());
                                break;
                        }

                    }
                }
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
    }
}