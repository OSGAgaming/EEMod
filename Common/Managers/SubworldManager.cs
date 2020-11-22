using EEMod.Common.IDs;
using EEMod.Common.Utilities;
using EEMod.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.Graphics.Capture;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Social;
using Terraria.Utilities;
using Terraria.World.Generation;

namespace EEMod.Common.Managers
{
    public class SubworldManager
    {
        public string SubworldFilePath;
        public EEServerStateID serverState = EEServerStateID.None;
        public Process eeServerProcess = new Process();
        public int lastSeed;

        public WorldGenerator Generator { get; private set; }

        public void SettleLiquids()
        {
            AddGenerationPass("Settle Liquids", delegate (GenerationProgress progress)
            {
                Liquid.QuickWater(3);
                WorldGen.WaterCheck();

                progress.Message = LanguageUtilities.GetEEModTextValue("WorldGeneration.SettlingLiquids");
                Liquid.quickSettle = true;
                int repeats = 0;

                while (repeats < 10)
                {
                    int liquidPlusBuffer = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
                    float i = 0f;

                    while (Liquid.numLiquid > 0)
                    {
                        float j = (liquidPlusBuffer - (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer)) / (float)liquidPlusBuffer;

                        if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > liquidPlusBuffer)
                            liquidPlusBuffer = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;

                        if (j > i)
                            i = j;
                        else
                            j = i;

                        if (repeats == 1)
                            progress.Set(j / 3f + 0.33f);

                        int maxRepeats = 10;

                        if (repeats > maxRepeats)
                            maxRepeats = repeats;

                        Liquid.UpdateLiquid();
                    }

                    WorldGen.WaterCheck();
                    progress.Set(repeats * 0.1f / 3f + 0.66f);

                    repeats++;
                }

                Liquid.quickSettle = false;
                Main.tileSolid[190] = true;
            });
        }

        public void Reset(int seed)
        {
            lastSeed = seed;
            Generator = new WorldGenerator(seed);

            EEMod.progressMessage = LanguageUtilities.GetEEModTextValue("WorldGeneration.Resetting");
            Logging.Terraria.InfoFormat("Generating World: {0}", Main.ActiveWorldFileData.Name);
            WorldGen.structures = new StructureMap();
            MicroBiome.ResetAll();

            AddGenerationPass("Reset", delegate (GenerationProgress progress)
            {
                progress.Message = LanguageUtilities.GetEEModTextValue("WorldGeneration.Resetting");
                Main.cloudAlpha = 0f;
                Main.maxRaining = 0f;
                Main.raining = false;

                Liquid.ReInit();
                WorldGen.RandomizeTreeStyle();
                WorldGen.RandomizeCaveBackgrounds();
                WorldGen.RandomizeBackgrounds();
                WorldGen.RandomizeMoonState();

                Main.worldID = WorldGen.genRand.Next(int.MaxValue);
            });
        }

        public void PostReset(GenerationProgress customProgressObject = null)
        {
            EEMod.progressMessage = LanguageUtilities.GetEEModTextValue("WorldGeneration.PostResetting");
            Generator.GenerateWorld(customProgressObject);
            Main.WorldFileMetadata = FileMetadata.FromCurrentSettings(FileType.World);
            EEWorld.EEWorld.FillRegion(Main.maxTilesX, Main.maxTilesY, Vector2.Zero, ModContent.TileType<GemsandTile>());
            EEWorld.EEWorld.ClearRegion(Main.maxTilesX, Main.maxTilesY, Vector2.Zero);
        }

        public void PreSaveAndQuit()
        {
            Mod[] mods = ModLoader.Mods;

            for (int i = 0; i < mods.Length; i++)
                mods[i].PreSaveAndQuit();
        }

        public void SaveAndQuit(string key)
        {
            Main.PlaySound(SoundID.MenuClose);
            PreSaveAndQuit();
            ThreadPool.QueueUserWorkItem(SaveAndQuitCallBack, key);
        }

        public void SaveAndQuitCallBack(object threadContext)
        {
            try
            {
                Main.PlaySound(SoundID.Waterfall, -1, -1, 0);
                Main.PlaySound(SoundID.Lavafall, -1, -1, 0);
            }
            catch (Exception e)
            {
                ModContent.GetInstance<EEMod>().Logger.Warn($"[Unhandled Exception] {e.Message}" +
                    $"\n{e.StackTrace}");
            }

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                WorldFile.CacheSaveTime();

                Main.menuMode = 10;
                EEMod.isSaving = true;
            }
            else
            {
                Main.menuMode = 889;
            }

            Main.invasionProgress = 0;
            Main.invasionProgressDisplayLeft = 0;
            Main.invasionProgressAlpha = 0f;
            Main.gameMenu = true;

            Main.StopTrackedSounds();
            CaptureInterface.ResetFocus();
            Main.ActivePlayerFileData.StopPlayTimer();
            Player.SavePlayer(Main.ActivePlayerFileData);

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                WorldFile.saveWorld();
                Main.PlaySound(SoundID.MenuOpen);

                Main.fastForwardTime = false;

                Main.UpdateSundial();

                Main.menuMode = 0;
                serverState = EEServerStateID.Singleplayer;
            }
            else
            {
                Netplay.disconnect = true;
                Main.netMode = NetmodeID.SinglePlayer;
                Main.fastForwardTime = false;

                Main.UpdateSundial();

                Main.menuMode = 889;
                serverState = EEServerStateID.Multiplayer;
            }

            if (threadContext != null)
                EnterSub((string)threadContext);
        }

        public void Do_worldGenCallBack(object threadContext)
        {
            Main.PlaySound(SoundID.MenuOpen);
            WorldGen.clearWorld();
            EEMod.GenerateWorld((string)threadContext, Main.ActiveWorldFileData.Seed, null);
            WorldFile.saveWorld(Main.ActiveWorldFileData.IsCloudSave, resetTime: true);

            Main.ActiveWorldFileData = WorldFile.GetAllMetadata($@"{SubworldFilePath}\{threadContext as string}.wld", false);

            WorldGen.playWorld();
        }

        public void WorldGenCallBack(object threadContext)
        {
            try
            {
                Do_worldGenCallBack(threadContext);
            }
            catch (Exception e)
            {
                Logging.Terraria.Error(Language.GetTextValue("tModLoader.WorldGenError"), e);
            }
        }

        public void CreateNewWorld(string text)
        {
            Main.rand = new UnifiedRandom(Main.ActiveWorldFileData.Seed);
            ThreadPool.QueueUserWorkItem(WorldGenCallBack, text);
        }

        public WorldFileData CreateSubworldMetaData(string name, bool cloudSave, string path)
        {
            WorldFileData worldFileData = new WorldFileData(path, cloudSave)
            {
                Name = name,
                //worldFileData.GameMode = GameMode;
                CreationTime = DateTime.Now,
                Metadata = FileMetadata.FromCurrentSettings(FileType.World)
            };
            worldFileData.SetFavorite(favorite: false);
            worldFileData.WorldGeneratorVersion = 987842478081uL;
            worldFileData.UniqueId = Guid.NewGuid();

            if (Main.DefaultSeed == "")
                worldFileData.SetSeedToRandom();
            else
                worldFileData.SetSeed(Main.DefaultSeed);

            return worldFileData;
        }

        public void EnterSub(string text)
        {
            if (text != Main.LocalPlayer.GetModPlayer<EEPlayer>().baseWorldName)
            {
                SubworldFilePath = $"{Main.SavePath}\\Worlds\\{Main.LocalPlayer.GetModPlayer<EEPlayer>().baseWorldName}Subworlds";

                if (!Directory.Exists(SubworldFilePath))
                    Directory.CreateDirectory(SubworldFilePath);

                string EESubworldPath = $"{SubworldFilePath}\\{text}.wld";

                Main.ActiveWorldFileData = WorldFile.GetAllMetadata(EESubworldPath, false);
                Main.ActivePlayerFileData.SetAsActive();

                if (!File.Exists(EESubworldPath))
                {
                    CreateNewWorld(text);

                    Main.ActiveWorldFileData = CreateSubworldMetaData(text, SocialAPI.Cloud != null && SocialAPI.Cloud.EnabledByDefault, EESubworldPath);
                    Main.worldName = text.Trim();

                    return;
                }
            }
            else
            {
                Main.spawnTileX = 100;
                Main.spawnTileY = EEWorld.EEWorld.TileCheckWater(100) - 22;
                Main.ActiveWorldFileData = WorldFile.GetAllMetadata($@"{Main.SavePath}\Worlds\{text}.wld", false);
                Main.ActivePlayerFileData.SetAsActive();
            }

            /*string path = $"{Main.SavePath}\\Worlds\\{text}.wld";

            if (!File.Exists(path))
            {
                Main.ActiveWorldFileData = WorldFile.CreateMetadata(text, SocialAPI.Cloud != null && SocialAPI.Cloud.EnabledByDefault, Main.expertMode);
                Main.worldName = text.Trim();
                CreateNewWorld(text);
                return;
            }*/

            if (serverState == EEServerStateID.Singleplayer)
                WorldGen.playWorld();
            else
            {
                /*EEServer.StartInfo.FileName = "tModLoaderServer.exe";

                if (Main.libPath != "")
                {
                    ProcessStartInfo startInfo = EEServer.StartInfo;
                    startInfo.Arguments = startInfo.Arguments + " -loadlib " + Main.libPath;
                }

                EEServer.StartInfo.UseShellExecute = false;
                EEServer.StartInfo.CreateNoWindow = !Main.showServerConsole;

                if (SocialAPI.Network != null)
                    SocialAPI.Network.LaunchLocalServer(EEServer, Main.MenuServerMode);
                else
                    EEServer.Start();

                Netplay.SetRemoteIP("127.0.0.1");

                Main.autoPass = true;

                Netplay.StartTcpClient();*/

                Main.clrInput();

                Netplay.ServerPassword = "";

                Main.GetInputText("");

                Main.autoPass = false;
                Main.menuMode = 30;

                Main.PlaySound(SoundID.MenuOpen);
            }
        }

        public static void StartClientGameplay()
        {
            Main.menuMode = 10;
            Netplay.StartTcpClient();
        }

        public void ReturnOnName(string baseWorldName)
        {
            Main.ActiveWorldFileData = WorldFile.GetAllMetadata($"{Main.SavePath}\\Worlds\\{baseWorldName}.wld", false);
            WorldGen.playWorld();
        }

        public void AddGenerationPass(string name, WorldGenLegacyMethod method) => Generator.Append(new PassLegacy(name, method));

        public string ConvertToSafeArgument(string arg) => Uri.EscapeDataString(arg);
    }
}