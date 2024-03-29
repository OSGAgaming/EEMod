﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework.Input;
using EEMod.ID;
using EEMod.Tiles;
using EEMod.VerletIntegration;
using static EEMod.EEWorld.EEWorld;
using EEMod.Tiles.Foliage.Coral.HangingCoral;
using EEMod.Tiles.Foliage.Coral.WallCoral;
using EEMod.Tiles.Foliage.Coral;
using EEMod.Tiles.Foliage;
using System;
using EEMod.Systems.Noise;
using System.Collections.Generic;
using EEMod.Autoloading;
using Terraria.WorldBuilding;
using System.Diagnostics;
using EEMod.NPCs.Goblins.Shaman;
using EEMod.NPCs.Goblins.Berserker;
using EEMod.NPCs.Goblins.Watchman;
using EEMod.Subworlds;
using EEMod.Systems;
using EEMod.Tiles.Furniture.GoblinFort;
using EEMod.NPCs.Goblins.Scrapwizard;
using SubworldLibrary;
using Terraria.IO;
using Terraria.DataStructures;

namespace EEMod.Subworlds
{
    public class GoblinFort : EESubworld
    {
        public override int Height => 1200;
        public override int Width => 2400;

        public override string Name => "Goblin Outpost";

        public int hallX;
        public int hallY;

        public override bool NormalUpdates => true;

        public override bool ShouldSave => false;

        //public override bool NoPlayerSaving => base.NoPlayerSaving;

        public override string subworldKey => KeyID.GoblinFort;

        public override List<GenPass> Tasks => new List<GenPass>()
        {
            new SubworldGenerationPass(progress =>
            {
                Main.worldSurface = 1000;
                
                //Base island generation
                #region Island terrain generation

                FillRegion(2400, 475 - 10 + 100, new Vector2(0, 400 + 325 + 10 - 100), TileID.Stone);

                for (int i = 0; i < 300; i++)
                {
                    for (int j = 0; j < 50; j++)
                    {
                        if (j < (((i / 30f) * (i / 30f)) + 10f))
                        {
                            WorldGen.PlaceTile((2399 - i), (50 - j) + 274 + 400 + 10 + 10 - 100 - 5, TileID.Adamantite, true);
                        }
                    }
                }

                for(int i = 0; i < 2400; i++)
                {
                    for (int j = 0; j < 1200; j++)
                    {
                        if (Framing.GetTileSafely(i, j).TileType == TileID.Adamantite && !Framing.GetTileSafely(i, j - 1).HasTile && Main.rand.NextBool(8))
                        {
                            /*if(Main.rand.NextBool(10) || (i > 120 && i < 2399 - 120))
                            {
                                if(Main.rand.NextBool())
                                    MakeFlattenedTriangle(new Vector2(i, j + 3), Main.rand.Next(9, 11), 100, Main.rand.Next(5, 7), TileID.Mythril, 0, true, 2);
                                else
                                    WorldGen.TileRunner(i - 5, j + 3, 15, 25, TileID.Mythril, true);
                            }
                            else
                            {
                                if(Main.rand.NextBool())
                                    MakeFlattenedTriangle(new Vector2(i, j + 3), Main.rand.Next(7, 9), 100, Main.rand.Next(5, 7), TileID.Mythril, 0, true, 2);
                                else
                                    WorldGen.TileRunner(i - 5, j + 3, 10, 25, TileID.Mythril, true);
                            }*/

                            WorldGen.TileRunner(i - 5, j, 25, 20, TileID.Mythril, true);

                            i += 10;

                            break;
                        }
                    }
                }

                //ClearRegion(2400, 50, new Vector2(0, 400 + 276 + 5 - 3 - 50 - 3 - 100 + 2));

                for(int steps = 0; steps < 3; steps++) 
                {
                    for(int i = 0; i < 2400; i++)
                    {
                        for (int j = 0; j < 1200; j++)
                        {
                            if (Framing.GetTileSafely(i, j).HasTile)
                            {
                                int amountOfTiles = 0;

                                for(int k = i - 1; k < i + 2; k++)
                                    for(int l = j - 1; l < j  + 2; l++)
                                        if (Framing.GetTileSafely(k, l).HasTile)
                                            amountOfTiles++;

                                if(amountOfTiles < 3)
                                    Framing.GetTileSafely(i, j).ClearTile();
                            }
                        }
                    }
                }

                /*for(int i = 0; i < 2400; i++)
                {
                    for (int j = 0; j < 1200; j++)
                    {
                        if (!WorldGen.InWorld(i, j)) continue;

                        if (Framing.GetTileSafely(i, j).HasTile
                            && (!Framing.GetTileSafely(i - 1, j - 1).HasTile && !Framing.GetTileSafely(i, j - 1).HasTile && !Framing.GetTileSafely(i + 1, j - 1).HasTile) && Framing.GetTileSafely(i, j - 1).LiquidAmount <= 16)
                        {
                            Framing.GetTileSafely(i, j).IsHalfBlock = true;

                            break;
                        }
                        else
                        {
                            //Tile.SmoothSlope(i, j);
                        }
                    }
                }*/

                for(int i = 0; i < 2400; i++)
                {
                    for (int j = 0; j < 1200; j++)
                    {
                        if (Framing.GetTileSafely(i, j).TileType == TileID.Mythril || Framing.GetTileSafely(i, j).TileType == TileID.Adamantite)
                        {
                            Framing.GetTileSafely(i, j).TileType = (ushort)ModContent.TileType<KelpRockTile>();
                        }
                    }
                }

                for (int i = 0; i < 300; i++)
                {
                    for (int j = 0; j < 70; j++)
                    {
                        if (j < (((i / 50f) * (i / 50f)) + 30f) && !Framing.GetTileSafely((2399 - i) + 0, (70 - j) + 274 + 400 + 10 + 10 - 100 - 5 - 5 - 10 - 10).HasTile)
                        {
                            WorldGen.PlaceTile((2399 - i) + 0, (70 - j) + 274 + 400 + 10 + 10 - 100 - 5 - 5 - 10 - 10, ModContent.TileType<DirtySandTile>(), true);
                        }
                    }
                }

                FillRegionWithWater(2400, 65, new Vector2(0, 400 + 276 + 5 - 3 - 100 + 2));

                #endregion



                //Foliage generation
                #region Foliage generation
                for (int i = 150; i < 2400 - 150; i++)
                {
                    for (int j = 100; j < 280; j++)
                    {
                        if (Framing.GetTileSafely(i, j).TileType == TileID.Dirt &&
                            (!Framing.GetTileSafely(i - 1, j - 1).HasTile || !Framing.GetTileSafely(i - 1, j).HasTile || !Framing.GetTileSafely(i - 1, j + 1).HasTile ||
                             !Framing.GetTileSafely(i, j - 1).HasTile || !Framing.GetTileSafely(i, j + 1).HasTile ||
                             !Framing.GetTileSafely(i + 1, j - 1).HasTile || !Framing.GetTileSafely(i + 1, j).HasTile || !Framing.GetTileSafely(i + 1, j + 1).HasTile))
                        {
                            Framing.GetTileSafely(i, j).TileType = TileID.Grass;
                            //Framing.GetTileSafely(i, j).Color = PaintID.LimePaint;
                            Tile.SmoothSlope(i, j);
                        }
                    }
                }

                for (int i = 150; i < 2400 - 150; i++)
                {
                    for (int j = 100; j < 280; j++)
                    {
                        if (!Framing.GetTileSafely(i, j).HasTile && Framing.GetTileSafely(i, j + 1).HasTile && Framing.GetTileSafely(i, j + 1).TileType == TileID.Grass && WorldGen.genRand.NextBool())
                        {
                            WorldGen.PlaceTile(i, j, TileID.Plants, style: WorldGen.genRand.Next(42));
                        }
                    }
                }

                for (int i = 0; i < 2400; i++)
                {
                    for (int j = 250; j < 1000; j++)
                    {
                        Tile tile = Framing.GetTileSafely(i, j);

                        //if (tile.HasTile && tile.type == TileID.Sand &&
                        //    !Framing.GetTileSafely(i, j - 1).HasTile && Framing.GetTileSafely(i, j - 1).LiquidAmount == 0 && WorldGen.genRand.NextBool(2))
                        //{
                        //WorldGen.GrowPalmTree(i, j - 1);
                        //    break;
                        //}

                        if (tile.HasTile && (tile.TileType == ModContent.TileType<KelpRockTile>() || tile.TileType == ModContent.TileType<DirtySandTile>()) &&
                            !Framing.GetTileSafely(i, j - 1).HasTile && Framing.GetTileSafely(i, j - 1).LiquidAmount > 0 && WorldGen.genRand.NextBool(2)
                            && tile.Slope == SlopeType.Solid)
                        {
                            Framing.GetTileSafely(i, j).Slope = 0;

                            switch (WorldGen.genRand.Next(2))
                            {
                                case 0:
                                    int Rand = WorldGen.genRand.Next(7, 20);

                                    for (int l = j - 1; l >= j - Rand; l--)
                                    {
                                        if (Framing.GetTileSafely(i, l).LiquidAmount < 60) break;

                                        Framing.GetTileSafely(i, l).TileType = (ushort)ModContent.TileType<SeagrassTile>();
                                        Framing.GetTileSafely(i, l).HasTile = true;
                                    }
                                    break;
                                case 1:
                                    int rand2 = WorldGen.genRand.Next(4, 13);

                                    for (int l = j - 1; l >= j - rand2; l--)
                                    {
                                        if (Main.tile[i, l].LiquidAmount < 60) break;

                                        Main.tile[i, l].TileType = TileID.Seaweed;
                                        Framing.GetTileSafely(i, l).HasTile = true;

                                        if (l == j - rand2)
                                        {
                                            Main.tile[i, l].TileFrameX = (short)(WorldGen.genRand.Next(8, 13) * 18);
                                        }
                                        else
                                        {
                                            Main.tile[i, l].TileFrameX = (short)(WorldGen.genRand.Next(1, 8) * 18);
                                        }
                                    }
                                    break;
                            }
                            break;
                        }
                    }
                }
                #endregion



                //Structure generation
                #region Structure generation

                #region Generating the goblin hall parts

                hallX = 1200 - 80;
                hallY = 200;

                ClearRegion(2400, 68, new Vector2(0, hallY));

                Structure.DeserializeFromBytes(ModContent.GetInstance<EEMod>().GetFileBytes("EEWorld/Structures/goblinhall.lcs")).PlaceAt(hallX, hallY, false, false);

                PlaceTileEntity(hallX + (432 / 16), hallY + (464 / 16), ModContent.TileType<GoblinBannerBig>(), ModContent.GetInstance<GoblinBannerBigTE>());
                PlaceTileEntity(hallX + (640 / 16), hallY + (464 / 16), ModContent.TileType<GoblinBannerBig>(), ModContent.GetInstance<GoblinBannerBigTE>());
                PlaceTileEntity(hallX + (848 / 16), hallY + (464 / 16), ModContent.TileType<GoblinBannerBig>(), ModContent.GetInstance<GoblinBannerBigTE>());
                PlaceTileEntity(hallX + (1088 / 16), hallY + (464 / 16), ModContent.TileType<GoblinBannerBig>(), ModContent.GetInstance<GoblinBannerBigTE>());
                PlaceTileEntity(hallX + (1392 / 16), hallY + (464 / 16), ModContent.TileType<GoblinBannerBig>(), ModContent.GetInstance<GoblinBannerBigTE>());
                PlaceTileEntity(hallX + (1632 / 16), hallY + (464 / 16), ModContent.TileType<GoblinBannerBig>(), ModContent.GetInstance<GoblinBannerBigTE>());
                PlaceTileEntity(hallX + (1840 / 16), hallY + (464 / 16), ModContent.TileType<GoblinBannerBig>(), ModContent.GetInstance<GoblinBannerBigTE>());
                PlaceTileEntity(hallX + (2048 / 16), hallY + (464 / 16), ModContent.TileType<GoblinBannerBig>(), ModContent.GetInstance<GoblinBannerBigTE>());

                PlaceTileEntity(hallX + (576 / 16), hallY + (464 / 16), ModContent.TileType<GoblinChandelier>(), ModContent.GetInstance<GoblinChandelierTE>());
                PlaceTileEntity(hallX + (784 / 16), hallY + (464 / 16), ModContent.TileType<GoblinChandelier>(), ModContent.GetInstance<GoblinChandelierTE>());
                PlaceTileEntity(hallX + (1008 / 16), hallY + (464 / 16), ModContent.TileType<GoblinChandelier>(), ModContent.GetInstance<GoblinChandelierTE>());
                PlaceTileEntity(hallX + (1184 / 16), hallY + (464 / 16), ModContent.TileType<GoblinChandelier>(), ModContent.GetInstance<GoblinChandelierTE>());
                PlaceTileEntity(hallX + (1360 / 16), hallY + (464 / 16), ModContent.TileType<GoblinChandelier>(), ModContent.GetInstance<GoblinChandelierTE>());
                PlaceTileEntity(hallX + (1536 / 16), hallY + (464 / 16), ModContent.TileType<GoblinChandelier>(), ModContent.GetInstance<GoblinChandelierTE>());
                PlaceTileEntity(hallX + (1760 / 16), hallY + (464 / 16), ModContent.TileType<GoblinChandelier>(), ModContent.GetInstance<GoblinChandelierTE>());
                PlaceTileEntity(hallX + (1968 / 16), hallY + (464 / 16), ModContent.TileType<GoblinChandelier>(), ModContent.GetInstance<GoblinChandelierTE>());

                WorldGen.PlaceTile(hallX + (464 / 16) + 5, hallY + (1008 / 16), ModContent.TileType<GoblinBanquetTable>());
                WorldGen.PlaceTile(hallX + (880 / 16) + 5, hallY + (1008 / 16), ModContent.TileType<GoblinBanquetTable>());
                WorldGen.PlaceTile(hallX + (1440 / 16), hallY + (1008 / 16), ModContent.TileType<GoblinBanquetTable>());
                WorldGen.PlaceTile(hallX + (1856 / 16), hallY + (1008 / 16), ModContent.TileType<GoblinBanquetTable>());

                WorldGen.PlaceTile(hallX + ((464 + (((880 - 464) / 2))) / 16) + 5, hallY + (1008 / 16), ModContent.TileType<GoblinBanquetTable>());
                WorldGen.PlaceTile(hallX + ((1440 + (((1856 - 1440) / 2))) / 16), hallY + (1008 / 16), ModContent.TileType<GoblinBanquetTable>());

                #endregion

                //Placing player boat
                BuildBoat(2400 - 90, 547 + 3);

                for (int i = 2400 - 90; i < 2400 - 90 + 45; i++)
                {
                    for (int j = 250; j < 250 + 40; j++)
                    {
                        if (Main.tile[i, j].WallType != WallID.None)
                        {
                            Main.tile[i, j].LiquidAmount = 0;
                        }
                    }
                }


                //Placing rafts
                for(int i = 50; i < 400; i++)
                {
                    for (int j = 0; j < Main.maxTilesY; j++)
                    {
                        if (Framing.GetTileSafely(i, j).HasTile) break;

                        if (!Framing.GetTileSafely(i, j).HasTile && Framing.GetTileSafely(i, j).LiquidAmount >= 64)
                        {
                            if (Main.rand.NextBool(80))
                            {
                                switch (WorldGen.genRand.NextBool())
                                {
                                    case true:
                                        Structure.DeserializeFromBytes(ModContent.GetInstance<EEMod>().GetFileBytes("EEWorld/Structures/GoblinRaft1.lcs")).PlaceAt(i, TileCheckWater(i) - 3, false, false);
                                        break;
                                    case false:
                                        Structure.DeserializeFromBytes(ModContent.GetInstance<EEMod>().GetFileBytes("EEWorld/Structures/GoblinRaft2.lcs")).PlaceAt(i, TileCheckWater(i) - 3, false, false);
                                        break;
                                }

                                i += 30;
                                break;
                            }
                        }
                    }
                }

                #endregion



                EESubworld.progressMessage = null;

                Main.spawnTileX = 2400 - 90 + 12;
                Main.spawnTileY = 545 + 25;
            })
        };

        public void PlaceTileEntity(int x, int y, int tileType, ModTileEntity TE)
        {
            WorldGen.PlaceTile(x, y, tileType);
            TE.Place(x, y);
        }

        public override void OnLoad()
        {
            if (Main.netMode == NetmodeID.SinglePlayer || Main.netMode == NetmodeID.Server)
            {
                NPC.NewNPC(new Terraria.DataStructures.EntitySource_WorldGen(), 400 * 16, TileCheck(400, TileID.Grass) * 16 - 40, ModContent.NPCType<GoblinBerserker>());

                NPC.NewNPC(new Terraria.DataStructures.EntitySource_WorldGen(), 410 * 16, TileCheck(410, TileID.Grass) * 16 - 40, ModContent.NPCType<GoblinWatchman>());

                #region Spawning boss

                NPC.NewNPC(new Terraria.DataStructures.EntitySource_WorldGen(), hallX * 16, hallY * 16, ModContent.NPCType<GuardBrute>(), ai0: 0, ai1: hallX * 16, ai2: hallY * 16);

                #endregion
            }

            base.OnLoad();
        }

        public override void OnEnter()
        {
            //ShouldSave = false;
        }

        public override void OnExit()
        {
            Main.LocalPlayer.GetModPlayer<SeamapPlayer>().prevKey = KeyID.GoblinFort;

            base.OnExit();
        }
    }

    public class GoblinFortSystem : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot(ModContent.GetInstance<EEMod>(), "Assets/Music/GoblinRaid");
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player)
        {
            return SubworldSystem.IsActive<GoblinFort>();
        }
    }

    public class GoblinFortPlayer : ModPlayer
    {
        public int updateTicks = 0;
        public override void PreUpdate()
        {
            if (SubworldSystem.IsActive<GoblinFort>())
            {
                Wiring.UpdateMech();

                TileEntity.UpdateStart();
                foreach (TileEntity te in TileEntity.ByID.Values)
                {
                    te.Update();
                }
                TileEntity.UpdateEnd();

                if (++Liquid.skipCount > 1)
                {
                    Liquid.UpdateLiquid();
                    Liquid.skipCount = 0;
                }
            }
        }
    }
}