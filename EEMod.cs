﻿using EEMod.Autoloading;
using EEMod.Effects;
using EEMod.Extensions;
using EEMod.ID;
using EEMod.Items.Dyes;
using EEMod.MachineLearning;
using EEMod.Net;
using EEMod.NPCs.CoralReefs;
using EEMod.Prim;
using EEMod.Seamap.SeamapContent;
using EEMod.Skies;
using EEMod.Systems;
using EEMod.Tiles.EmptyTileArrays;
using EEMod.Tiles.Furniture;
using EEMod.UI.States;
using EEMod.VerletIntegration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization; // :sadge:
using Terraria.ModLoader;
using Terraria.UI;// l a g
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader.IO;

namespace EEMod
{
    public partial class EEMod : Mod
    {
        public static bool isSaving = false;
        public static int loadingChoose;
        public static int loadingChooseImage;
        public static bool loadingFlag = true;
        public static ModKeybind RuneActivator;
        public static ModKeybind RuneSpecial;
        public static ModKeybind Inspect;
        public static ModKeybind ActivateVerletEngine;
        public static ModKeybind Train;
        public static Noise2D Noise2D;
        public static Effect White;
        public static Effect Effervescence;
        public static Effect Colorify;
        public static ParticleZoneHandler Particles;
        public static UIManager UI;
        internal static ParticleZone MainParticles;
        public static SubworldInstanceManager Subworlds;
        private GameTime lastGameTime;
        public UserInterface EEInterface;
        public FishermansLogUI FishermansLogUI;
        public KelpArmorAmmoUI KelpArmorAmmoUI;
        public IndicatorsUI IndicatorsUI;
        public DialogueUI DialogueUI;
        public ComponentManager<TileObjVisual> TVH;

        public override void Load()
        {
            TVH = new ComponentManager<TileObjVisual>();
            verlet = new Verlet();
            Subworlds = new SubworldInstanceManager();

            TagSerializer.AddSerializer(new BigCrystalSerializer());
            TagSerializer.AddSerializer(new EmptyTileEntitySerializer());
            TagSerializer.AddSerializer(new CrystalSerializer());

            if (!Main.dedServ)
            {
                UI = new UIManager();

                FishermansLogUI = new FishermansLogUI();
                FishermansLogUI.Activate();
                UI.AddInterface("EEInterfacee");
                UI.AddUIState("FishermansLogUI", FishermansLogUI);

                KelpArmorAmmoUI = new KelpArmorAmmoUI();
                KelpArmorAmmoUI.Activate();
                UI.AddInterface("KelpArmorAmmoInterface");
                UI.AddUIState("KelpArmorAmmoUI", KelpArmorAmmoUI);

                IndicatorsUI = new IndicatorsUI();
                IndicatorsUI.Activate();
                UI.AddInterface("IndicatorsInterface");
                UI.AddUIState("IndicatorsUI", IndicatorsUI);

                DialogueUI = new DialogueUI();
                DialogueUI.Activate();
                UI.AddInterface("DialogueInterface");
                UI.AddUIState("DialogueUI", DialogueUI);

                Noise2D = new Noise2D(Assets.Request<Effect>("Effects/Noise2D").Value);
                primitives = new PrimTrailManager();
            }
            //HandwritingCNN = new Handwriting();

            RuneActivator = KeybindLoader.RegisterKeybind(this, "Rune UI", Keys.Z);
            RuneSpecial = KeybindLoader.RegisterKeybind(this, "Activate Runes", Keys.V);
            Inspect = KeybindLoader.RegisterKeybind(this, "Inspect", Keys.OemCloseBrackets);
            ActivateVerletEngine = KeybindLoader.RegisterKeybind(this, "Activate VerletEngine", Keys.N);
            //Train = RegisterHotKey("Train Neural Network", "P");

            AutoloadingManager.LoadManager(this);

            //IL.Terraria.IO.WorldFile.SaveWorldTiles += ILSaveWorldTiles;
            if (!Main.dedServ)
            {
                Ref<Effect> screenRef3 = new Ref<Effect>(Assets.Request<Effect>("Effects/Ripple").Value);
                Ref<Effect> screenRef2 = new Ref<Effect>(Assets.Request<Effect>("Effects/SeaTrans").Value);
                Ref<Effect> screenRef = new Ref<Effect>(Assets.Request<Effect>("Effects/SunThroughWalls").Value);
                Ref<Effect> MyTestShader = new Ref<Effect>(Assets.Request<Effect>("Effects/MyTestShader").Value);
                Filters.Scene["EEMod:Ripple"] = new Filter(new ScreenShaderData(screenRef3, "Ripple"), EffectPriority.High);
                Filters.Scene["EEMod:Ripple"].Load();
                Filters.Scene["EEMod:SeaTrans"] = new Filter(new ScreenShaderData(screenRef2, "SeaTrans"), EffectPriority.High);
                Filters.Scene["EEMod:SeaTrans"].Load();
                Filters.Scene["EEMod:SunThroughWalls"] = new Filter(new ScreenShaderData(screenRef, "SunThroughWalls"), EffectPriority.High);
                Filters.Scene["EEMod:SunThroughWalls"].Load();
                Filters.Scene["EEMod:SavingCutscene"] = new Filter(new SavingSkyData("FilterMiniTower").UseColor(0f, 0.20f, 1f).UseOpacity(0.3f), EffectPriority.High);
                Filters.Scene["EEMod:MyTestShader"] = new Filter(new ScreenShaderData(MyTestShader, "MyTestShaderFlot"), EffectPriority.High);
                Filters.Scene["EEMod:MyTestShader"].Load();

                GameShaders.Misc["EEMod:SpireHeartbeat"] = new MiscShaderData(new Ref<Effect>(Assets.Request<Effect>("Effects/SpireShine").Value), "SpireHeartbeat").UseImage0("Textures/Noise/WormNoisePixelated");

                SkyManager.Instance["EEMod:SavingCutscene"] = new SavingSky();
                NoiseSurfacing = Assets.Request<Effect>("Effects/NoiseSurfacing").Value;
                White = Assets.Request<Effect>("Effects/WhiteOutline").Value;
                Effervescence = Assets.Request<Effect>("Effects/Effervescence").Value;


                Ref<Effect> hydrosDye = new Ref<Effect>(Assets.Request<Effect>("Effects/HydrosDye").Value);
                GameShaders.Armor.BindShader(ModContent.ItemType<HydrosDye>(), new ArmorShaderData(hydrosDye, "HydrosDyeShader"));
                Ref<Effect> aquamarineDye = new Ref<Effect>(Assets.Request<Effect>("Effects/AquamarineDye").Value);
                GameShaders.Armor.BindShader(ModContent.ItemType<HydrosDye>(), new ArmorShaderData(aquamarineDye, "AquamarineDyeShader"));

                /*
                  SpeedrunnTimer = new UserInterface();
                  //RunUI.Activate();
                  RunUI = new RunninUI();
                  SpeedrunnTimer.SetState(RunUI);
                */

                if (Main.netMode != NetmodeID.Server)
                {
                    trailManager = new TrailManager(this);
                    prims = new Prims(this);
                    primitives.CreateTrail(new RainbowLightTrail(null));
                    prims.CreateVerlet();
                }
                LoadUI();
            }
            LoadIL();
            LoadDetours();
            if (!Main.dedServ)
            {
                Particles = new ParticleZoneHandler();
                Particles.AddZone("Main", 40000);
                MainParticles = Particles.Get("Main");
            }
            InitializeAmbience();
            //Example
            //LayeredMusic.Groups[GetSoundSlot(SoundType.Music, "Sounds/Music/UpperReefs")] = "AquamarineGroup";
            //LayeredMusic.Groups[GetSoundSlot(SoundType.Music, "Sounds/Music/LowerReefs")] = "AquamarineGroup";
        }

        public override void Unload()
        {
            //IL.Terraria.IO.WorldFile.SaveWorldTiles -= ILSaveWorldTiles;
            //HandwritingCNN = null;
            PrismShader = null;
            SpireShader = null;
            Noise2D = null;
            RuneActivator = null;
            Inspect = null;
            RuneSpecial = null;
            simpleGame = null;
            ActivateVerletEngine = null;
            Train = null;
            NoiseSurfacing = null;
            White = null;
            Effervescence = null;
            Subworlds = null;
            Colorify = null;
            UnloadIL();
            UnloadDetours();
            UnloadUI();
            AutoloadingManager.UnloadManager(this);
            Noise2DShift = null;
            //BufferPool.ClearBuffers();
            //Main.logo2Texture = ModContent.Request<Texture2D>("Terraria/Logo2").Value;
            //Main.logoTexture = ModContent.Request<Texture2D>("Terraria/Logo").Value;
            //Main.sun2Texture = ModContent.Request<Texture2D>("Terraria/Sun2").Value;
            //Main.sun3Texture = ModContent.Request<Texture2D>("Terraria/Sun3").Value;
            //Main.sunTexture = ModContent.Request<Texture2D>("Terraria/Sun").Value;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            EENet.ReceievePacket(reader, whoAmI);
        }

        public override void MidUpdateProjectileItem()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                trailManager.UpdateTrails();
                prims.UpdateTrails();
                primitives.UpdateTrailsAboveTiles();
            }

            Seamap.SeamapContent.Seamap.UpdateSeamap();
        }

        public override void MidUpdateNPCGore()
        {
            MechanicManager.MidUpdateNPCGore();
        }

        public override void MidUpdateDustTime()
        {
            MechanicManager.MidUpdateDustTime();
        }

        //Mechanic Port
        public override void PreUpdateEntities()
        {
            base.PreUpdateEntities();
            MechanicManager.PreUpdateEntities();      
        }

        public override void PostUpdateEverything()
        {
            UpdateVerlet();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                LegacyGameInterfaceLayer EEInterfaceLayerUI = new LegacyGameInterfaceLayer("EEMod: EEInterface", delegate
                {
                    if (lastGameTime != null)
                    {
                        UI.DrawWithScaleUI(lastGameTime);
                    }

                    return true;
                }, InterfaceScaleType.UI);
                layers.Insert(mouseTextIndex, EEInterfaceLayerUI);
                LegacyGameInterfaceLayer EEInterfaceLayerGame = new LegacyGameInterfaceLayer("EEMod: EEInterface", delegate
                {
                    if (lastGameTime != null)
                    {
                        UI.DrawWithScaleGame(lastGameTime);
                        UpdateGame(lastGameTime);
                        if (Main.worldName == KeyID.CoralReefs)
                        {
                            DrawCR();
                        }
                    }

                    return true;
                }, InterfaceScaleType.Game);
                layers.Insert(mouseTextIndex, EEInterfaceLayerGame);
            }
            if (Main.LocalPlayer.GetModPlayer<EEPlayer>().ridingZipline)
            {
                DrawZipline();
            }

            var textLayer = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (textLayer != -1)
            {
                var computerState = new LegacyGameInterfaceLayer("EE: UI", delegate
                {
                    if (Main.worldName == KeyID.Pyramids || Main.worldName == KeyID.Sea || Main.worldName == KeyID.CoralReefs)
                    {
                        DrawText();
                    }
                    return true;
                },
                InterfaceScaleType.UI);
                layers.Insert(textLayer, computerState);
            }
            /*if (mouseTextIndex != -1)
		    {
		        layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
		        "SpeedrunTimer: SpeedrunnTimer",
		        delegate
		        {
		            if (_lastUpdateUiGameTime != null && SpeedrunnTimer?.CurrentState != null)
		            {
			            SpeedrunnTimer.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
		            }
		            return true;
		        },
		        InterfaceScaleType.UI));
		    }*/
            if (Main.worldName == KeyID.Sea)
            {
                for (int i = 0; i < layers.Count; i++)
                {
                    var layer = layers[i];
                    //Remove Resource bars
                    if (layer.Name.Contains("Vanilla: Resource Bars") || layer.Name.Contains("Vanilla: Info Accessories Bar") || layer.Name.Contains("Vanilla: Map / Minimap") || layer.Name.Contains("Vanilla: Inventory"))
                    {
                        layers.RemoveAt(i);
                    }
                }
            }
        }

        public override void AddRecipeGroups()
        {
            RecipeGroup group0 = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Gemstones", new int[]
            {
                ItemID.Amber,
                ItemID.Amethyst,
                ItemID.Diamond,
                ItemID.Emerald,
                ItemID.Ruby,
                ItemID.Sapphire,
                ItemID.Topaz
            });
            // Registers the new recipe group with the specified name
            RecipeGroup.RegisterGroup("EEMod:Gemstones", group0);
        }

        /*public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemType<SaharaSceptoid>(), 1);
            recipe.AddIngredient(ItemID.CrystalShard, 8);
            recipe.AddIngredient(ItemID.SoulofLight, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(ItemID.CrystalSerpent, 1);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemType<QuartzicLifeFragment>(), 1);
            recipe.AddIngredient(ItemID.Gel, 25);
            recipe.AddIngredient(ItemID.Wood, 10);
            recipe.AddTile(TileID.Solidifier);
            recipe.SetResult(ItemID.SlimeStaff, 1);
            recipe.AddRecipe();
        }*/

        //mechanic port

        /*public override void UpdateMusic(ref int music, ref MusicPriority priority)
        {
            if (Main.gameMenu)
                return;

            Player player = Main.LocalPlayer;
            EEPlayer eeplayer = player?.GetModPlayer<EEPlayer>();

            if (eeplayer == null)
                return;

            if (Main.worldName == KeyID.CoralReefs)
            {
                if (Main.LocalPlayer.Center.Y < ((Main.maxTilesY / 20) + (Main.maxTilesY / 60) + (Main.maxTilesY / 60)) * 16)
                {
                    LayeredMusic.ShouldLayerMusic = true;
                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/SurfaceReefs");
                    priority = MusicPriority.Environment;
                }

                if (Main.LocalPlayer.Center.Y >= ((Main.maxTilesY / 20) + (Main.maxTilesY / 60) + (Main.maxTilesY / 60)) * 16 && Main.LocalPlayer.Center.Y < (Main.maxTilesY / 10) * 4 * 16)
                {
                    LayeredMusic.ShouldLayerMusic = true;
                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/UpperReefs");
                    priority = MusicPriority.Environment;
                }

                if (Main.LocalPlayer.Center.Y >= ((Main.maxTilesY / 10) * 4) * 16 && Main.LocalPlayer.Center.Y < (Main.maxTilesY / 10) * 7 * 16)
                {
                    LayeredMusic.ShouldLayerMusic = true;
                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/LowerReefs");
                    priority = MusicPriority.Environment;
                }

                if (eeplayer.reefMinibiome == MinibiomeID.KelpForest)
                {
                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/KelpForest");
                    priority = MusicPriority.BiomeHigh;
                }
                
                if (eeplayer.reefMinibiome == MinibiomeID.AquamarineCaverns)
                {
                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/Aquamarine");
                    priority = MusicPriority.BiomeHigh;
                }

                if (eeplayer.reefMinibiome == MinibiomeID.GlowshroomGrotto)
                {
                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/GlowshroomGrotto");
                    priority = MusicPriority.BiomeHigh;
                }

                if ((int)MinibiomeID.ThermalVents < length)
                {
                    if (eeplayer.reefMinibiome[(int)MinibiomeID.ThermalVents])
                    {
                        music = GetSoundSlot(SoundType.Music, "Sounds/Music/ThermalVents");
                        priority = MusicPriority.BiomeHigh;
                    }
                }
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.ModNPC is AquamarineSpire spire)
                {
                    if (spire.awake)
                    {
                        music = GetSoundSlot(SoundType.Music, "Sounds/Music/AquamarineSpire");
                        priority = MusicPriority.BossLow;
                    }
                }
            }


            if (Main.worldName == KeyID.Sea)
            {
                music = GetSoundSlot(SoundType.Music, "Sounds/Music/Seamap");
                priority = MusicPriority.BiomeHigh;
            }

            MechanicManager.UpdateMusic(ref music, ref priority);
        }*/
    }
}
