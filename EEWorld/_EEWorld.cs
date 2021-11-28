using EEMod.Autoloading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using EEMod.Tiles;
using EEMod.Tiles.Furniture;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using EEMod.ID;
using EEMod.Tiles.Foliage.Coral;
using EEMod.Tiles.Ores;
using EEMod.Tiles.Walls;
using Terraria.GameContent.Events;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Terraria.DataStructures;
using EEMod.Tiles.EmptyTileArrays;
using System.Linq;
using EEMod.VerletIntegration;
using EEMod.Prim;
using EEMod.Systems.Subworlds.EESubworlds;
using Terraria.UI;
using EEMod.NPCs.Friendly;
using EEMod.Systems.Noise;
using EEMod.Systems;
using EEMod.Tiles.Foliage;

namespace EEMod.EEWorld
{
    public partial class EEWorld : ModSystem
    {
        public int minionsKilled;
        public static EEWorld instance => ModContent.GetInstance<EEWorld>();

        public static Vector2 yes;
        public static Vector2 ree;

        [FieldInit(FieldInitType.SubType, typeof(List<Vector2>))]
        public static IList<Vector2> EntracesPosses = new List<Vector2>();

        [FieldInit(FieldInitType.ArrayIntialization, 6)]
        public static byte[] LightStates = new byte[6];

        [FieldInit(FieldInitType.ArrayIntialization, 100)]
        public static Vector2[] PylonBegin = new Vector2[100];

        [FieldInit(FieldInitType.ArrayIntialization, 100)]
        public static Vector2[] PylonEnd = new Vector2[100];

        private static PerlinNoiseFunction PNF;

        public static Vector2 shipCoords;

        public static IList<Vector2> Vines = new List<Vector2>();


        public override void LoadWorldData(TagCompound tag)
        {
            tag.TryGetListRef("EntracesPosses", ref EntracesPosses);
            tag.TryGetRef("CoralBoatPos", ref CoralReefs.CoralBoatPos);
            tag.TryGetRef("yes", ref yes);
            tag.TryGetRef("ree", ref ree);
            tag.TryGetRef("SpirePosition", ref CoralReefs.SpirePosition);
            tag.TryGetListRef("CoralReefVineLocations", ref CoralReefs.CoralReefVineLocations);
            tag.TryGetListRef("AquamarineZiplineLocations", ref CoralReefs.AquamarineZiplineLocations);
            tag.TryGetListRef("ThinCrystalBambooLocations", ref CoralReefs.ThinCrystalBambooLocations);
            tag.TryGetListRef("BulbousTreePosition", ref CoralReefs.BulbousTreePosition);
            tag.TryGetListRef("WebPositions", ref CoralReefs.WebPositions);


            IList<Vector2> positions = new List<Vector2>();
            IList<Vector2> sizes = new List<Vector2>();
            IList<int> ids = new List<int>();

            tag.TryGetListRef("CoralMinibiomesPositions", ref positions);
            tag.TryGetListRef("CoralMinibiomesSizes", ref sizes);
            tag.TryGetListRef("CoralMinibiomesIds", ref ids);

            tag.TryGetRef("ShipCoords", ref shipCoords);

            for (int i = 0; i < positions.Count; i++)
            {
                if (ids[i] == (int)MinibiomeID.None)
                {
                    continue;
                }
                else if (ids[i] == (int)MinibiomeID.AquamarineCaverns)
                {
                    CoralReefs.Minibiomes.Add(new AquamarineCaverns()
                    {
                        Position = positions[i].ToPoint(),
                        Size = sizes[i].ToPoint(),
                        EnsureNoise = false
                    });

                    continue;
                }
                else if (ids[i] == (int)MinibiomeID.GlowshroomGrotto)
                {
                    CoralReefs.Minibiomes.Add(new GlowshroomGrotto()
                    {
                        Position = positions[i].ToPoint(),
                        Size = sizes[i].ToPoint(),
                        EnsureNoise = false
                    });

                    continue;
                }
                else if (ids[i] == (int)MinibiomeID.KelpForest)
                {
                    CoralReefs.Minibiomes.Add(new KelpForest()
                    {
                        Position = positions[i].ToPoint(),
                        Size = sizes[i].ToPoint(),
                        EnsureNoise = false
                    });

                    continue;
                }
                else if (ids[i] == (int)MinibiomeID.ThermalVents)
                {
                    CoralReefs.Minibiomes.Add(new ThermalVents()
                    {
                        Position = positions[i].ToPoint(),
                        Size = sizes[i].ToPoint(),
                        EnsureNoise = false
                    });

                    continue;
                }
                else
                {
                    Main.NewText("something went wrong");
                }
            }



            //tag.TryGetListRef("CoralReefMinibiomes", ref CoralReefs.Minibiomes);

            /*if (tag.TryGetListRef("WebPositions", ref EESubWorlds.WebPositions))
            {
                if (EESubWorlds.WebPositions.Count != 0)
                {
                    foreach (Vector2 vec in EESubWorlds.WebPositions)
                    {
                        for (int i = 0; i < 12; i++)
                            PrimSystem.primitives.CreateTrail(new WebPrimTrail(null, vec * 16, i));
                    }
                }
            }*/

            if (tag.TryGetListRef("SwingableVines", ref VerletHelpers.SwingableVines))
            {
                //TODO: Confirm moving this here didn't break anything
                VerletHelpers.SwingableVines.Clear();
                if (VerletHelpers.SwingableVines.Count != 0)
                {
                    foreach (Vector2 vec in VerletHelpers.SwingableVines)
                    {
                        VerletHelpers.AddStickChainNoAdd(ref ModContent.GetInstance<EEMod>().verlet, vec, Main.rand.Next(5, 10), 27);
                    }
                }
            }
            if (tag.TryGetList<Vector2>("EmptyTileVectorMain", out IList<Vector2> vecMains) && tag.TryGetList<Vector2>("EmptyTileVectorSub", out IList<Vector2> list2))
            {
                EmptyTileEntities.Instance.EmptyTilePairsCache = vecMains.Zip(list2, (k, v) => new { Key = k, Value = v }).ToDictionary(x => x.Key, x => x.Value);
            }
            if (tag.TryGetList<Vector2>("EmptyTileVectorEntities", out var VecMains) && tag.TryGetList<EmptyTileEntity>("EmptyTileEntities", out var VecSubs))
            {
                EmptyTileEntities.Instance.EmptyTileEntityPairsCache = VecMains.Zip(VecSubs, (k, v) => new { Key = k, Value = v }).ToDictionary(x => x.Key, x => x.Value);
                IList<EmptyTileEntity> emptyTileEntities = VecSubs;
                EmptyTileEntities.Instance.ETES.Clear();
                foreach (EmptyTileEntity ETEnt in emptyTileEntities)
                {
                    EmptyTileEntities.Instance.ETES.Add(ETEnt);
                }
            }
            if (tag.TryGetListRef<Vector2>("CoralCrystalPosition", ref CoralReefs.CoralCrystalPosition))
            {
                // for (int i = 0; i < EESubWorlds.CoralCrystalPosition.Count; i++)
                //    EmptyTileEntityCache.AddPair(new Crystal(EESubWorlds.CoralCrystalPosition[i]), EESubWorlds.CoralCrystalPosition[i], EmptyTileArrays.CoralCrystal);
            }
            tag.TryGetByteArrayRef("LightStates", ref LightStates);
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (Main.ActiveWorldFileData.Name == KeyID.CoralReefs)
            {
                tag["CoralBoatPos"] = CoralReefs.CoralBoatPos;
                tag["CoralReefVineLocations"] = CoralReefs.CoralReefVineLocations;
                tag["AquamarineZiplineLocations"] = CoralReefs.AquamarineZiplineLocations;
                tag["ThinCrystalBambooLocations"] = CoralReefs.ThinCrystalBambooLocations;
                tag["BulbousTreePosition"] = CoralReefs.BulbousTreePosition;
                tag["WebPositions"] = CoralReefs.WebPositions;

                List<Vector2> positions = new List<Vector2>();
                List<Vector2> sizes = new List<Vector2>();
                List<int> ids = new List<int>();

                foreach (CoralReefMinibiome mb in CoralReefs.Minibiomes)
                {
                    positions.Add(mb.Position.ToVector2());
                    sizes.Add(mb.Size.ToVector2());
                    ids.Add((int)mb.id);
                }

                tag["CoralMinibiomesSize"] = positions;
                tag["CoralMinibiomesId"] = sizes;
                tag["CoralMinibiomesPosition"] = ids;

                //tag["CoralReefMinibiomes"] = CoralReefs.Minibiomes;

                tag["SwingableVines"] = VerletHelpers.SwingableVines;
                tag["LightStates"] = LightStates;
                tag["CoralCrystalPosition"] = CoralReefs.CoralCrystalPosition;
                tag["SpirePosition"] = CoralReefs.SpirePosition;
                tag["EmptyTileVectorMain"] = EmptyTileEntities.Instance.EmptyTilePairsCache.Keys.ToList();
                tag["EmptyTileVectorSub"] = EmptyTileEntities.Instance.EmptyTilePairsCache.Values.ToList();
                tag["EmptyTileVectorEntities"] = EmptyTileEntities.Instance.EmptyTileEntityPairsCache.Keys.ToList();
                tag["EmptyTileEntities"] = EmptyTileEntities.Instance.EmptyTileEntityPairsCache.Values.ToList();


            }
            tag["EntracesPosses"] = EntracesPosses;
            tag["yes"] = yes;
            tag["ree"] = ree;
            tag["ShipCoords"] = shipCoords;
        }

        public override void OnWorldLoad()
        {
            Main.LocalPlayer.GetModPlayer<EEPlayer>().isInSubworld = Main.ActiveWorldFileData.Path.Contains($@"{Main.SavePath}\Worlds\{Main.LocalPlayer.GetModPlayer<EEPlayer>().baseWorldName}Subworlds");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.WriteVector2(ree);
            writer.WriteVector2(yes);
            for (int i = 0; i < LightStates.Length; i++)
            {
                writer.Write(LightStates[i]);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            ree = reader.ReadVector2();
            yes = reader.ReadVector2();
            for (int i = 0; i < LightStates.Length; i++)
            {
                LightStates[i] = reader.ReadByte();
            }
        }

        public override void PostWorldGen()
        {
            GenerateShipyard();
        }

        public static void GenerateShipyard()
        {
            for (int i = 0; i < 300; i++)
            {
                for (int j = (int)(Main.worldSurface * 0.35f); j < Main.rockLayer; j++)
                {
                    if (!WorldGen.InWorld(i, j)) break;

                    Tile tile = Framing.GetTileSafely(i, j);

                    if (tile.LiquidAmount > 0)
                    {
                        //Ocean surface worldgen

                        /*switch (WorldGen.genRand.Next(3))
                        {
                            case 0:
                                WorldGen.PlaceTile(i, j, ModContent.TileType<LilyPadSmol>());
                                break;

                            case 1:
                                WorldGen.PlaceTile(i, j, ModContent.TileType<LilyPadMedium>());
                                break;

                            default:
                                break;
                        }*/

                        for (int k = j; k < Main.rockLayer; k++)
                        {
                            Tile tile2 = Framing.GetTileSafely(i, k);

                            if (tile2.IsActive && tile2.type == TileID.Sand &&
                                !Framing.GetTileSafely(i, k - 1).IsActive && Framing.GetTileSafely(i, k - 1).LiquidAmount > 0 && WorldGen.genRand.NextBool(3))
                            {
                                //Ocean floor worldgen

                                if (WorldGen.genRand.NextBool(20) && k > j + 20/* && i < - 5*/)
                                {
                                    switch (WorldGen.genRand.Next(3))
                                    {
                                        case 0:
                                            Structure.DeserializeFromBytes(ModContent.GetInstance<EEMod>().GetFileBytes("EEWorld/Structures/RockSpire1.lcs")).PlaceAt(i, k - 15, true, true, true);
                                            break;
                                        case 1:
                                            Structure.DeserializeFromBytes(ModContent.GetInstance<EEMod>().GetFileBytes("EEWorld/Structures/RockSpire2.lcs")).PlaceAt(i, k - 15, true, true, true);
                                            break;
                                        case 2:
                                            Structure.DeserializeFromBytes(ModContent.GetInstance<EEMod>().GetFileBytes("EEWorld/Structures/SunkRaft.lcs")).PlaceAt(i, k - 8, true, true, true);
                                            break;
                                    }

                                    break;
                                }

                                Main.tile[i, k].Slope = 0;

                                switch (WorldGen.genRand.Next(3))
                                {
                                    case 0:
                                        int rand = WorldGen.genRand.Next(7, 20);

                                        for (int l = k - 1; l >= k - rand; l--)
                                        {
                                            Main.tile[i, l].type = (ushort)ModContent.TileType<SeagrassTile>();
                                            Main.tile[i, l].IsActive = true;
                                        }
                                        break;
                                    case 1:
                                        int rand2 = WorldGen.genRand.Next(4, 13);

                                        for (int l = k - 1; l >= k - rand2; l--)
                                        {
                                            Main.tile[i, l].type = TileID.Seaweed;
                                            Main.tile[i, l].IsActive = true;

                                            if (l == k - rand2)
                                            {
                                                Main.tile[i, l].frameX = (short)(WorldGen.genRand.Next(8, 13) * 18);
                                            }
                                            else
                                            {
                                                Main.tile[i, l].frameX = (short)(WorldGen.genRand.Next(1, 8) * 18);
                                            }
                                        }
                                        break;
                                    case 2:
                                        //WorldGen.PlaceTile(i, k - 2, TileID.DyePlants, false, false, -1, 6);

                                        /*Main.tile[i, k - 2].type = TileID.DyePlants;
                                        Main.tile[i, k - 2].frameX = 11 * 16;
                                        Main.tile[i, k - 2].IsActive = true;

                                        Main.tile[i, k - 1].type = TileID.DyePlants;
                                        Main.tile[i, k - 1].frameX = 11 * 16;
                                        Main.tile[i, k - 1].frameY = 1 * 16;
                                        Main.tile[i, k - 1].IsActive = true;*/

                                        int rand3 = WorldGen.genRand.Next(4, 8);

                                        for (int l = k - 1; l >= k - rand3; l--)
                                        {
                                            Main.tile[i, l].type = TileID.Bamboo;
                                            Main.tile[i, l].IsActive = true;

                                            if (l == k - 1)
                                            {
                                                Main.tile[i, l].frameX = (short)(WorldGen.genRand.Next(1, 5) * 18);
                                            }
                                            else if (l == k - rand3)
                                            {
                                                Main.tile[i, l].frameX = (short)(WorldGen.genRand.Next(15, 20) * 18);
                                            }
                                            else
                                            {
                                                Main.tile[i, l].frameX = (short)(WorldGen.genRand.Next(5, 15) * 18);
                                            }
                                        }

                                        break;
                                }

                                //WorldGen.PlaceTile(i, k - 2=, TileID.Sandcastles);

                                break;
                            }
                        }

                        break;
                    }

                    else if (tile.IsActive && tile.type == TileID.Sand)
                    {
                        PlaceShipyard(i, j - 13);
                        return;
                    }
                }
            }
        }
    }
}