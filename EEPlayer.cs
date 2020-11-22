using EEMod.Autoloading;
using EEMod.Buffs.Buffs;
using EEMod.Common.IDs;
using EEMod.Config;
using EEMod.Extensions;
using EEMod.Items.Fish;
using EEMod.Projectiles;
using EEMod.Projectiles.Armor;
using EEMod.Projectiles.Mage;
using EEMod.Projectiles.Runes;
using EEMod.VerletIntegration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameInput;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static EEMod.EEWorld.EEWorld;
using static Terraria.ModLoader.ModContent;

namespace EEMod
{
    public partial class EEPlayer : ModPlayer
    {
        public bool importantCutscene;
        public static bool startingText;
        public bool godMode = false;

        //Biome checks
        public bool ZoneCoralReefs;

        public bool ZoneKelpForest;
        public bool ZonePolypZone;
        public bool ZoneJellyfishCaverns;
        public bool ZoneThermalVents;
        public bool ZoneBublousGrove;
        public bool ZoneSubterraneanWaters;

        public bool ZoneTropicalIsland;

        //Equipment booleans
        public bool hydroGear;

        public bool dragonScale;
        public bool lythenSet;
        public int lythenSetTimer;
        public bool dalantiniumSet;
        public bool hydriteSet;
        public bool hydrofluoricSet;
        public int hydrofluoricSetTimer;
        public bool dalantiniumHood;
        public bool hydriteVisage;
        public bool quartzCrystal = false;
        public bool isQuartzRangedOn = false;
        public bool isQuartzSummonOn = false;
        public bool isQuartzMeleeOn = false;
        public bool isQuartzChestOn = false;
        public bool FlameSpirit;

        public int timerForCutscene;
        public bool arrowFlag = false;
        public static bool isSaving;
        public float titleText;
        public float titleText2;
        public float subTextAlpha;
        public bool noU;
        public bool triggerSeaCutscene;
        public int cutSceneTriggerTimer;
        public float cutSceneTriggerTimer3;
        public int coralReefTrans;
        public int markerPlacer;
        public Vector2 position;
        public Vector2 velocity;
        public List<Vector2> objectPos = new List<Vector2>();
        public List<Island> SeaObject = new List<Island>();
        public List<int> SeaObjectFrames = new List<int>();
        public Dictionary<string, Island> Islands = new Dictionary<string, Island>();
        public string baseWorldName;

        //Runes
        public byte[] hasGottenRuneBefore = new byte[7];

        public byte[] inPossesion = new byte[7];

        //Morality
        public static int moralScore;

        public int initialMoralScore;

        public readonly SubworldManager SM = new SubworldManager();
        public int rippleCount = 3;
        public int rippleSize = 5;
        public int rippleSpeed = 15;
        public int distortStrength = 100;
        public List<ParticlesClass> Particles = new List<ParticlesClass>();
        public List<Vector2> Velocity;
        private static string prevKey = "Main";
        public float powerLevel = 0;
        public int maxPowerLevel = 11;
        public float zipMultiplier = 1;
        public int thermalHealingTimer = 30;
        public int cannonballType = 0;
        public bool isPickingUp;
        private float propagation;

        public Dictionary<int, int> fishLengths = new Dictionary<int, int>();

        public int bubbleRuneBubble = 0;

        private readonly int displaceX = 2;
        private readonly int displaceY = 4;
        private readonly float[] dis = new float[51];
        public bool isWearingCape = false;
        public string NameForJoiningClients = "";
        public Vector2[] arrayPoints = new Vector2[24];
        public static EEPlayer instance => Main.LocalPlayer.GetModPlayer<EEPlayer>();
        private int Arrow;
        private int Arrow2;
        private float speedOfPan = 1;
        public int offSea = 1000;
        private int opac;
        public int boatSpeed = 1;
        private readonly string shad1 = "EEMod:Ripple";
        private readonly string shad2 = "EEMod:SunThroughWalls";
        private readonly string shad3 = "EEMod:SeaTrans";
        public bool firstFrameVolcano;
        public Vector2 PylonBegin;
        public Vector2 PylonEnd;
        public bool holdingPylon;
        public bool ridingZipline;
        public Vector2 secondPlayerMouse;
        public int PlayerX;
        public int PlayerY;
        public Vector2 velHolder;

        [FieldInit]
        internal static List<IOceanMapElement> OceanMapElements = new List<IOceanMapElement>();

        public bool currentlyRotated, currentlyRotatedByToRotation, wasAirborn, lerpingToRotation = false;
        public int timeAirborne = 0;

        public override void PostUpdate()
        {
            if (player.wet)
            {
                if (player.fullRotation % MathHelper.ToRadians(-360f) < 1 && player.fullRotation % MathHelper.ToRadians(-360f) > -1 && !lerpingToRotation)
                {
                    player.fullRotation = 0;
                    wasAirborn = false;
                }

                if (player.mount.Type == -1)
                {
                    player.fullRotationOrigin = new Vector2(player.width / 2, player.height / 2);

                    if (player.fullRotation != 0)
                    {
                        currentlyRotated = true;
                    }

                    if ((player.velocity.X != 0 && player.velocity.Y != 0) || (player.velocity.Y != 0 && timeAirborne > 60))
                    {
                        timeAirborne++;

                        if (timeAirborne > 60)
                        {
                            lerpingToRotation = true;
                            player.fullRotation = player.fullRotation.AngleLerp(player.velocity.ToRotation() + (float)Math.PI / 2f, 0.05f);
                            wasAirborn = true;
                        }
                        else
                        {
                            lerpingToRotation = false;
                            wasAirborn = false;
                        }
                    }
                    else
                    {
                        lerpingToRotation = false;

                        if (player.direction == -1)
                        {
                            if (wasAirborn)
                            {
                                player.fullRotation = MathHelper.Lerp(player.fullRotation, 0f, -0.085f);
                            }
                            else
                            {
                                player.fullRotation = 0;
                                timeAirborne = 0;
                            }
                        }
                        else
                        {
                            if (wasAirborn)
                            {
                                player.fullRotation = MathHelper.Lerp(player.fullRotation, 0f, -0.085f);
                            }
                            else
                            {
                                player.fullRotation = 0;
                                timeAirborne = 0;
                            }
                        }

                        if (player.fullRotation == 0)
                        {
                            player.fullRotation += player.velocity.X / 7f;

                            if (player.fullRotation > MathHelper.ToRadians(player.velocity.X))
                            {
                                player.fullRotation = MathHelper.ToRadians(player.velocity.X);
                            }

                            if (player.fullRotation < MathHelper.ToRadians(-player.velocity.X))
                            {
                                player.fullRotation = -MathHelper.ToRadians(-player.velocity.X);
                            }
                        }
                    }
                }
                else
                {
                    if (currentlyRotated)
                    {
                        player.fullRotation = 0f;
                        currentlyRotated = false;
                        wasAirborn = false;
                        lerpingToRotation = false;
                    }
                }
            }
        }

        public override void UpdateBiomes()
        {
            ZoneCoralReefs = Main.ActiveWorldFileData.Name == KeyID.CoralReefs;
            if (ZoneCoralReefs)
            {
                opac++;
                if (opac > 100)
                {
                    opac = 100;
                }
                //Filters.Scene.Activate("EEMod:CR").GetShader().UseOpacity(opac);
            }
            else
            {
                opac--;
                if (opac < 0)
                {
                    opac = 0;
                }
                //	Filters.Scene.Deactivate("EEMod:CR");
            }
        }

        private int bubbleTimer = 6;
        private int bubbleLen = 0;
        private int dur = 0;
        private int bubbleColumn;
        public bool isHoldingGlider;
        public Vector2 currentAltarPos;
        public bool isInSubworld;

        public override void UpdateVanityAccessories()
        {
            if (hydroGear || dragonScale)
            {
                player.accFlipper = true;
            }

            if (hydroGear)
            {
                player.accDivingHelm = true;
            }

            if (dragonScale && player.wet && PlayerInput.Triggers.JustPressed.Jump)
            {
                if (dur <= 0)
                {
                    bubbleColumn = 0;
                    dur = 36;
                }
            }
        }

        public override void CatchFish(Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk)
        {
            if (junk)
            {
                return;
            }

            if (ZoneCoralReefs)
            {
                if (Main.rand.NextFloat() < 0.01f && questFish == ItemType<BlueTang>())
                {
                    caughtType = ItemType<BlueTang>();
                }

                if (Main.rand.NextFloat() < 0.01f && questFish == ItemType<Spiritfish>() && Main.hardMode)
                {
                    caughtType = ItemType<Spiritfish>();
                }

                if (Main.rand.NextFloat() < 0.01f && questFish == ItemType<GlitteringPearlfish>() && downedCoralGolem)
                {
                    caughtType = ItemType<GlitteringPearlfish>();
                }

                if (Main.rand.NextFloat() < 0.01f && questFish == ItemType<Ironfin>() && downedTalos)
                {
                    caughtType = ItemType<Ironfin>();
                }

                if (Main.rand.NextFloat() < 0.01f)
                {
                    caughtType = ItemType<LunaJellyItem>();
                }

                if (Main.rand.NextFloat() < 0.1f)
                {
                    caughtType = ItemType<Barracuda>();
                }

                if (Main.rand.NextFloat() < 0.4f)
                {
                    caughtType = ItemType<ReeftailMinnow>();
                }

                if (Main.rand.NextFloat() < 0.4f)
                {
                    caughtType = ItemType<Coralfin>();
                }
            }
        }

        public override bool CustomBiomesMatch(Player other)
        {
            EEPlayer modOther = other.GetModPlayer<EEPlayer>();
            return ZoneCoralReefs == modOther.ZoneCoralReefs;
        }

        public override void CopyCustomBiomesTo(Player other)
        {
            EEPlayer modOther = other.GetModPlayer<EEPlayer>();
            modOther.ZoneCoralReefs = ZoneCoralReefs;
        }

        public override void SendCustomBiomes(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte();
            flags[0] = ZoneCoralReefs;
            writer.Write(flags);
        }

        public override void ReceiveCustomBiomes(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            ZoneCoralReefs = flags[0];
        }

        private void MoralFirstFrame()
        {
            switch (player.name)
            {
                case "OS":
                case "EpicCrownKing":
                case "Coolo109":
                case "Pyxis":
                case "phanta":
                case "cynik":
                case "daimgamer":
                case "Thecherrynuke":
                case "Vadim":
                case "Exitium":
                case "Chkylis":
                case "LolXD87":
                case "Nomis":
                case "A44":
                case "Stevie":
                    initialMoralScore += 1000;
                    break;
            }
        }

        public bool isHangingOnVine;

        private void Moral()
        {
            moralScore = 0;
            moralScore += initialMoralScore;
            moralScore -= WorldGen.totalEvil * 30;
            if (WorldGen.totalEvil == 0)
            {
                moralScore += 1000;
            }
            //Main.NewText(moralScore);
        }

        public override void Initialize()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                try
                {
                    if (Main.gameMenu)
                        isInSubworld = false;
                    else
                        isInSubworld = Main.ActiveWorldFileData.Path.Contains($@"{Main.SavePath}\Worlds\{Main.LocalPlayer.GetModPlayer<EEPlayer>().baseWorldName}Subworlds");
                }
                catch
                {
                }
                for (int i = 0; i < arrayPoints.Length; i++)
                {
                    arrayPoints[i] = new Vector2(mainPoint.X + (i * displaceX), mainPoint.Y + (i * displaceY));
                }
                isPickingUp = false;
                quickOpeningFloat = 20;
                EEMod.AscentionHandler = 0;
                EEMod.startingTextHandler = 0;
                EEMod.isAscending = false;
                EEMod.AscentionHandler = 0;
                isSaving = false;
                godMode = false;
                timerForCutscene = 0;
                markerPlacer = 0;
                arrowFlag = false;
                noU = false;
                triggerSeaCutscene = false;
                cutSceneTriggerTimer = 0;
                position = player.Center;
                importantCutscene = false;
                speedOfPan = 0;
                subTextAlpha = 0;
                EEMod.instance.position = new Vector2(1700, 900);
                objectPos.Clear();
                SeaObject.Clear();
                EEMod.ShipHelth = EEMod.ShipHelthMax;
                MoralFirstFrame();
                displacmentX = 0;
                displacmentY = 0;
                startingText = false;
                Particles.Clear();
                OceanMapElements.Clear();
                isCameraFixating = false;
            }
        }

        public override void ResetEffects()
        {
            isQuartzChestOn = false;
            isQuartzRangedOn = false;
            isQuartzMeleeOn = false;
            isQuartzSummonOn = false;
            ResetMinionEffect();
            isSaving = false;
            dragonScale = false;
            hydroGear = false;
            lythenSet = false;
            dalantiniumSet = false;
            hydriteSet = false;
            hydrofluoricSet = false;
            isWearingCape = false;
        }

        public void ReturnHome()
        {
            Initialize();
            SM.SaveAndQuit(KeyID.BaseWorldName);
        }

        private float displacmentX = 0;
        private float displacmentY = 0;
        public bool isCameraFixating;
        public bool isCameraShaking;
        public Vector2 fixatingPoint;
        public float fixatingSpeedInv;
        public int intensity;
        private int runeCooldown = 0;

        private readonly Dictionary<int, bool[]> RuneData = new Dictionary<int, bool[]>()
                                            {
                                                {0,new []{false,false }},
                                                {1,new []{false,false }},
                                                {2,new []{false,false }},
                                                {3,new []{false,false }},
                                                {4,new []{false,false }},
                                                {5,new []{false,false }},
                                                {6,new []{false,false }},
                                            };

        public void FixateCameraOn(Vector2 fixatingPointCamera, float fixatingSpeed, bool isCameraShakings, bool CameraMove, int intensity)
        {
            fixatingPoint = fixatingPointCamera;
            isCameraFixating = CameraMove;
            fixatingSpeedInv = fixatingSpeed;
            isCameraShaking = isCameraShakings;
            this.intensity = intensity;
        }

        public void TurnCameraFixationsOff()
        {
            isCameraFixating = false;
            isCameraShaking = false;
            fixatingPoint.X = player.Center.X;
            fixatingPoint.Y = player.Center.Y;
        }

        public override void ModifyScreenPosition()
        {
            int clamp = 80;
            float disSpeed = .4f;
            base.ModifyScreenPosition();
            if (Main.ActiveWorldFileData.Name == KeyID.Cutscene1)
            {
                if (markerPlacer < 120 * 8)
                {
                    displacmentX -= (displacmentX - (200 * 16)) / 32f;
                    displacmentY -= (displacmentY - (110 * 16)) / 32f;
                    Main.screenPosition += new Vector2(displacmentX - player.Center.X, displacmentY - player.Center.Y);
                    player.position = player.oldPosition;
                }
                else
                {
                    startingText = true;
                    Filters.Scene[shad1].GetShader().UseOpacity(timerForCutscene);
                    if (Main.netMode != NetmodeID.Server && !Filters.Scene[shad1].IsActive())
                    {
                        Filters.Scene.Activate(shad1, player.Center).GetShader().UseOpacity(timerForCutscene);
                    }
                    Main.screenPosition += new Vector2(displacmentX - player.Center.X, displacmentY - player.Center.Y);
                    displacmentX -= (displacmentX - player.Center.X) / 16f;
                    displacmentY -= (displacmentY - player.Center.Y) / 16f;
                    timerForCutscene += 10;
                    if (timerForCutscene > 1000)
                    {
                        timerForCutscene = 1000;
                    }

                    if (markerPlacer >= (120 * 8) + 1400)
                    {
                        if (Main.netMode != NetmodeID.Server && Filters.Scene[shad1].IsActive())
                        {
                            Filters.Scene[shad1].Deactivate();
                        }
                        if (Main.netMode != NetmodeID.Server && !Filters.Scene["EEMod:WhiteFlash"].IsActive())
                        {
                            //  Filters.Scene.Activate("EEMod:WhiteFlash", player.Center).GetShader().UseOpacity(markerPlacer - ((120 * 8) + 1400));
                        }

                        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
                        Main.spriteBatch.Draw(GetTexture("EEMod/Projectiles/Nice"), player.Center.ForDraw(), new Rectangle(0, 0, 174, 174), Color.White * (markerPlacer - ((120 * 8) + 1400)) * 0.05f, (markerPlacer - ((120 * 8) + 1400)) / 10, new Rectangle(0, 0, 174, 174).Size() / 2, markerPlacer - ((120 * 8) + 1400), SpriteEffects.None, 0);
                        Main.spriteBatch.End();
                        //  Filters.Scene["EEMod:WhiteFlash"].GetShader().UseOpacity(markerPlacer - ((120 * 8) + 1400));
                    }
                    if (markerPlacer >= (120 * 8) + 1800)
                    {
                        startingText = false;
                        if (Main.netMode != NetmodeID.Server && Filters.Scene["EEMod:WhiteFlash"].IsActive())
                        {
                            //    Filters.Scene["EEMod:WhiteFlash"].Deactivate();
                        }

                        Initialize();
                        SM.SaveAndQuit(KeyID.BaseWorldName);
                    }
                }
            }
            if (Main.worldName != KeyID.Sea && Main.ActiveWorldFileData.Name != KeyID.Cutscene1 && EEModConfigClient.Instance.CamMoveBool)
            {
                if (player.velocity.X > 1)
                {
                    displacmentX += disSpeed;
                }
                else if (player.velocity.X < -1)
                {
                    displacmentX -= disSpeed;
                }
                else
                {
                    displacmentX -= displacmentX / 16f;
                }
                if (player.velocity.Y > 1)
                {
                    displacmentY += disSpeed / 2;
                }
                else if (player.velocity.Y < -1)
                {
                    displacmentY -= disSpeed / 2;
                }
                else
                {
                    displacmentY -= displacmentY / 16f;
                }
                displacmentX = Helpers.Clamp(displacmentX, -clamp, clamp);
                displacmentY = Helpers.Clamp(displacmentY, -clamp, clamp);
                Main.screenPosition += new Vector2(displacmentX, displacmentY);
            }

            if (Main.worldName == KeyID.Sea)
            {
                player.position = player.oldPosition;
                if (markerPlacer > 1)
                {
                    Main.screenPosition += new Vector2(0, offSea);
                }
            }
            if (cutSceneTriggerTimer > 0 && triggerSeaCutscene)
            {
                if (!Main.gamePaused)
                {
                    speedOfPan += 0.01f;
                }

                Main.screenPosition.X += cutSceneTriggerTimer * speedOfPan;
            }
            if (isCameraFixating)
            {
                displacmentX += (fixatingPoint.X - player.Center.X - displacmentX) / fixatingSpeedInv;
                displacmentY += (fixatingPoint.Y - player.Center.Y - displacmentY) / fixatingSpeedInv;
                Main.screenPosition += new Vector2(displacmentX, displacmentY);
            }
            else if (Main.ActiveWorldFileData.Name != KeyID.Cutscene1 && Math.Abs(displacmentX + displacmentY) > 0.01f)
            {
                displacmentX *= 0.95f;
                displacmentY *= 0.95f;
                Main.screenPosition += new Vector2(displacmentX, displacmentY);
            }
            else
            {
                displacmentX = 0;
                displacmentY = 0;
                Main.screenPosition += new Vector2(displacmentX, displacmentY);
            }
            if (isCameraShaking)
            {
                Main.screenPosition += new Vector2(Main.rand.Next(-intensity, intensity), Main.rand.Next(-intensity, intensity));
            }
        }

        Vector2 mainPoint => new Vector2(player.Center.X, player.position.Y);

        private float inspectTimer = 0;

        public void InspectObject()
        {
            Main.spriteBatch.Draw(EEMod.instance.GetTexture("UI/InspectIcon"), (player.Center + new Vector2(0, (float)Math.Sin(inspectTimer) * 32)).ForDraw(), Color.White);
            inspectTimer += 0.5f;
        }

        public void UpdateVerletCollisions(int pRP, float velDamp, int fakeElevation, int newFeetPos, float gradientFunction)
        {
            foreach (Verlet.Stick stick in Verlet.stickPoints)
            {
                Rectangle pRect = new Rectangle((int)player.position.X - pRP, (int)player.position.Y - pRP, player.width + pRP, player.height + pRP);
                Vector2 Vec1 = Verlet.Points[stick.a].point;
                Vector2 Vec2 = Verlet.Points[stick.b].point;
                int Y = Vec1.Y < Vec2.Y ? (int)Vec1.Y : (int)Vec2.Y;
                int X = Vec1.X < Vec2.X ? (int)Vec1.X : (int)Vec2.X;
                int Y1 = Y == (int)Vec1.Y ? (int)Vec2.Y : (int)Vec1.Y;
                int X1 = X == (int)Vec1.X ? (int)Vec2.X : (int)Vec1.X;

                Rectangle vRect = new Rectangle(X - pRP, Y - pRP, X1 - X + pRP, Y1 - Y + pRP);
                if (pRect.Intersects(vRect))
                {
                    float perc = (player.Center.X - Vec1.X) / (Vec2.X - Vec1.X);
                    float yTarget = Vec1.Y + (Vec2.Y - Vec1.Y) * perc + fakeElevation;
                    float feetPos = player.position.Y + player.height;
                    float grad = (Vec2.Y - Vec1.Y) / (Vec2.X - Vec1.X);
                    grad *= gradientFunction;
                    if (feetPos - 5 - player.velocity.Y < yTarget && feetPos > yTarget)
                    {
                        player.velocity.Y = 0;
                        player.gravity = 0f;
                        player.position.Y = yTarget - (newFeetPos - grad * player.direction * Math.Abs(player.velocity.X / velDamp));
                        player.bodyFrameCounter += Math.Abs(velocity.X) * 0.5f;
                        while (player.bodyFrameCounter > 8.0)
                        {
                            player.bodyFrameCounter -= 8.0;
                            player.bodyFrame.Y += player.bodyFrame.Height;
                        }
                        if (player.bodyFrame.Y < player.bodyFrame.Height * 7)
                        {
                            player.bodyFrame.Y = player.bodyFrame.Height * 19;
                        }
                        else if (player.bodyFrame.Y > player.bodyFrame.Height * 19)
                        {
                            player.bodyFrame.Y = player.bodyFrame.Height * 7;
                        }
                    }
                }
            }
        }

        public bool playingGame;
        public float seamapLightColor;

        public override void UpdateBiomeVisuals()
        {
            seamapLightColor = MathHelper.Clamp((isStorming ? 1 : 2 / 3f) + brightness, 0.333f, 2f);
            int minibiome = 0;
            for (int k = 0; k < EESubWorlds.MinibiomeLocations.Count; k++)
            {
                if (Vector2.DistanceSquared(new Vector2(EESubWorlds.MinibiomeLocations[k].X, EESubWorlds.MinibiomeLocations[k].Y), new Vector2(player.Center.X / 16, player.Center.Y / 16)) < (220 * 220) && EESubWorlds.MinibiomeLocations[k].Z != 0)
                {
                    minibiome = (int)EESubWorlds.MinibiomeLocations[k].Z;
                    break;
                }
            }
            //  Main.NewText(minibiome);

            if (playingGame)
            {
                player.velocity = Vector2.Zero;
            }
            //UpdateVerletCollisions(1, 3f, 10, 54, 1.6f);
            if (isWearingCape)
            {
                UpdateArrayPoints();
            }
            thermalHealingTimer--;
            if (player.HasBuff(BuffType<ThermalHealing>()) && thermalHealingTimer <= 0)
            {
                player.statLife++;
                thermalHealingTimer = 30;
            }
            UpdateRunes();
            UpdateSets();
            UpdatePowerLevel();

            if (dur > 0)
            {
                bubbleTimer--;
                if (bubbleTimer <= 0)
                {
                    bubbleTimer = 6;
                    if (player.wet)
                    {
                        Projectile.NewProjectile(new Vector2(player.Center.X + bubbleLen - 16, player.Center.Y - bubbleColumn), new Vector2(0, -1), ProjectileType<WaterDragonsBubble>(), 5, 0, Owner: player.whoAmI);
                    }

                    bubbleLen = Main.rand.Next(-16, 17);
                    bubbleColumn += 2;
                }
                dur--;
            }
            Moral();

            EEMod.isSaving = false;
            if (Main.worldName != KeyID.Sea)
            {
                if (triggerSeaCutscene && cutSceneTriggerTimer <= 500)
                {
                    cutSceneTriggerTimer += 2;
                    player.position = player.oldPosition;
                }
                if (godMode)
                {
                    timerForCutscene += 20;
                }
            }
            switch (Main.worldName)
            {
                case KeyID.Pyramids:
                {
                    UpdatePyramids();
                    break;
                }
                case KeyID.Sea:
                {
                    UpdateSea();
                    break;
                }
                case KeyID.CoralReefs:
                {
                    UpdateCR();
                    break;
                }
                case KeyID.Island:
                {
                    UpdateIsland();
                    break;
                }
                case KeyID.VolcanoIsland:
                {
                    UpdateVolcano();
                    break;
                }
                case KeyID.VolcanoInside:
                {
                    UpdateInnerVolcano();
                    break;
                }
                case KeyID.Cutscene1:
                {
                    UpdateCutscene();
                    break;
                }
                default:
                {
                    UpdateWorld();
                    break;
                }
            }
            UpdateCutscenesAndTempShaders();
        }

        public void UpdateArrayPoints()
        {
            float acc = arrayPoints.Length;
            float upwardDrag = 0.2f;
            float smoothStepSpeed = 8;
            float yDis = 15;
            float propagtionSpeedWTRdisX = 15;
            float propagtionSpeedWTRvelY = 4;
            float basePosFluncStatic = 5f;
            float basePosFlunc = 3f;
            propagation += (Math.Abs(player.velocity.X / 2f) * 0.015f) + 0.1f;
            for (int i = 0; i < acc; i++)
            {
                float prop = (float)Math.Sin(propagation + (i * propagtionSpeedWTRdisX / acc));
                Vector2 basePos = new Vector2(mainPoint.X + (i * displaceX) + (Math.Abs(player.velocity.X / basePosFluncStatic) * i), mainPoint.Y + (i * displaceY) + 20);
                float dist = player.position.Y + yDis - basePos.Y + prop / acc * Math.Abs(-Math.Abs(player.velocity.X) - (i / acc));
                float amp = Math.Abs(player.velocity.X * basePosFlunc) * (i * basePosFlunc / acc) + 1f;
                float goTo = Math.Abs(dist * (Math.Abs(player.velocity.X) * upwardDrag)) + (player.velocity.Y / propagtionSpeedWTRvelY * i);
                float disClamp = (goTo - dis[i]) / smoothStepSpeed;
                disClamp = MathHelper.Clamp(disClamp, -1.7f, 15);
                dis[i] += disClamp;
                if (i == 0)
                {
                    arrayPoints[i] = basePos;
                }
                else
                {
                    arrayPoints[i] = new Vector2(basePos.X, basePos.Y + prop / acc * amp - dis[i] + i * 2);
                }

                if (player.direction == 1)
                {
                    float distX = arrayPoints[i].X - player.Center.X;
                    arrayPoints[i].X = player.Center.X - distX;
                }
                int tracker = 0;
                if (i != 0)
                {
                    while ((Main.tile[(int)arrayPoints[i].X / 16, (int)arrayPoints[i].Y / 16].active() &&
                            Main.tileSolid[Main.tile[(int)arrayPoints[i].X / 16, (int)arrayPoints[i].Y / 16].type])
                           || !Collision.CanHit(new Vector2(arrayPoints[i].X, arrayPoints[i].Y), 1, 1, new Vector2(arrayPoints[i - 1].X, arrayPoints[i - 1].Y), 1, 1))
                    {
                        arrayPoints[i].Y--;
                        tracker++;
                        if (tracker >= displaceY * acc)
                        {
                            break;
                        }

                        if (arrayPoints[i].Y <= arrayPoints[i - 1].Y - 4)
                        {
                            break;
                        }
                    }
                }
            }
        }

        public void UpdateRunes()
        {
            if (runeCooldown > 0) runeCooldown--;

            bool[][] states = new bool[][] { new bool[] { false, false }, new bool[] { true, false }, new bool[] { true, true } };
            for (int i = 0; i < hasGottenRuneBefore.Length; i++)
            {
                if (hasGottenRuneBefore[i] == 1)
                {
                    RuneData.TryGetValue(i, out states[(int)RuneStateID.RetrievedButNotEquiped]);
                    if (inPossesion[i] == 1)
                    {
                        RuneData.TryGetValue(i, out states[(int)RuneStateID.Equiped]);
                    }
                }
                else
                {
                    RuneData.TryGetValue(i, out states[(int)RuneStateID.Nothing]);
                }
                if (RuneData[i] == states[(int)RuneStateID.Equiped])
                {
                    switch ((RuneID)i)
                    {
                        case RuneID.SandRune:
                        {
                            if (EEMod.RuneSpecial.JustPressed && runeCooldown == 0)
                            {
                                runeCooldown = 180;
                            }
                            else
                            {
                                player.moveSpeed *= 1.15f;
                                player.jumpSpeedBoost *= 1.6f;
                                player.noFallDmg = true;
                                if (player.wet)
                                {
                                    player.meleeSpeed *= 1.07f;
                                    player.noKnockback = false;
                                }
                            }
                            break;
                        }
                        case RuneID.WaterRune:
                        {
                            if (EEMod.RuneSpecial.JustPressed && runeCooldown == 0)
                            {
                                if (bubbleRuneBubble == 0)
                                {
                                    bubbleRuneBubble = Projectile.NewProjectile(player.Center, Vector2.Zero, ProjectileType<BubblingWatersBubble>(), 0, 0, Main.myPlayer);
                                }
                                else
                                {
                                    Main.projectile[bubbleRuneBubble].Kill();
                                }
                                runeCooldown = 600;
                            }
                            else
                            {
                                if (player.wet)
                                {
                                    player.gravity = 0;
                                    if (player.controlUp)
                                        player.gravity = -0.2f;
                                    if (player.controlDown)
                                        player.gravity = 0.1f;
                                    if (!player.controlUp && !player.controlDown)
                                        player.gravity = -0.1f;
                                }
                            }
                            break;
                        }
                        case RuneID.LeafRune:
                        {
                            if (EEMod.RuneSpecial.JustPressed && runeCooldown == 0)
                            {
                                runeCooldown = 180;
                            }
                            else
                            {
                                player.meleeSpeed *= 1.08f;
                            }
                            player.moveSpeed *= 1.06f;
                            break;
                        }
                        case RuneID.FireRune:
                        {
                            if (EEMod.RuneSpecial.JustPressed && runeCooldown == 0)
                            {
                                runeCooldown = 180;
                            }
                            else
                            {
                                player.dash = 3;
                            }
                            player.moveSpeed *= 1.06f;
                            player.statDefense = (int)(player.statDefense * 0.93f);
                            break;
                        }
                        case RuneID.IceRune:
                        {
                            if (EEMod.RuneSpecial.JustPressed && runeCooldown == 0)
                            {
                                runeCooldown = 180;
                            }
                            else
                            {
                                player.dash = 3;
                            }
                            break;
                        }
                        case RuneID.SkyRune:
                        {
                            if (EEMod.RuneSpecial.JustPressed && runeCooldown == 0)
                            {
                                player.velocity -= new Vector2((player.Center.X - Main.MouseWorld.X) / 32, 16 * (Main.MouseWorld.Y > player.Center.Y ? -1 : 1));
                                runeCooldown = 300;
                            }
                            /*else
                            {
                                player.dash = 3;
                            }*/
                            break;
                        }
                    }
                }
            }
            //synergies
            if (RuneData[(int)RuneID.SandRune] == states[(int)RuneStateID.Equiped])
            {
            }
        }

        public void UpdateSets()
        {
            if (hydrofluoricSet)
            {
                hydrofluoricSetTimer++;
                if (hydrofluoricSetTimer >= 30 && player.velocity != Vector2.Zero)
                {
                    Projectile.NewProjectile(player.Center, player.velocity / 2, ProjectileType<CorrosiveBubble>(), 20, 0f);
                    hydrofluoricSetTimer = 0;
                }
            }

            if (lythenSet)
            {
                lythenSetTimer++;
                if (lythenSetTimer >= 480)
                {
                    NPC closest = null;
                    float closestDistance = 9999999;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.active && npc.Distance(position) < closestDistance)
                        {
                            closest = npc;
                            closestDistance = npc.Distance(position);
                        }
                    }
                    if (closest != null)
                    {
                        Projectile.NewProjectile(closest.Center, Vector2.Zero, ProjectileType<CyanoburstTomeKelp>(), 10, 0f, Owner: player.whoAmI);
                    }

                    lythenSetTimer = 0;
                }
            }

            if (hydriteSet)
            {
                player.gills = true;
            }
        }

        public void UpdateZipLines()
        {
            if (Main.LocalPlayer.GetModPlayer<EEPlayer>().ridingZipline)
            {
                Vector2 begin = Main.LocalPlayer.GetModPlayer<EEPlayer>().PylonBegin;
                Vector2 end = Main.LocalPlayer.GetModPlayer<EEPlayer>().PylonEnd;
                Main.LocalPlayer.velocity = Vector2.Normalize(end - begin) * zipMultiplier;
                Main.LocalPlayer.gravity = 0;
                Main.LocalPlayer.AddBuff(BuffID.Cursed, 2, true);
                if (zipMultiplier <= 30)
                {
                    zipMultiplier *= 1.02f;
                }
            }
            if (Vector2.DistanceSquared(Main.LocalPlayer.position, Main.LocalPlayer.GetModPlayer<EEPlayer>().PylonEnd) <= 32 * 32 && Main.LocalPlayer.GetModPlayer<EEPlayer>().ridingZipline)
            {
                int i;
                for (i = 0; i <= 100; i++)
                {
                    if (i < 99 && EEWorld.EEWorld.PylonEnd[i] == EEWorld.EEWorld.PylonBegin[i + 1] && EEWorld.EEWorld.PylonEnd[i + 1] != default && Main.LocalPlayer.GetModPlayer<EEPlayer>().PylonBegin == EEWorld.EEWorld.PylonBegin[i] && Main.LocalPlayer.GetModPlayer<EEPlayer>().PylonEnd == EEWorld.EEWorld.PylonEnd[i])
                    {
                        break;
                    }
                }

                if (i >= 99)
                {
                    //Leaving zipline
                    Main.LocalPlayer.GetModPlayer<EEPlayer>().PylonBegin = default;
                    Main.LocalPlayer.GetModPlayer<EEPlayer>().PylonEnd = default;
                    Main.LocalPlayer.GetModPlayer<EEPlayer>().ridingZipline = false;
                    zipMultiplier = 1;
                }
                else
                {
                    //Continue on zipline
                    Main.LocalPlayer.GetModPlayer<EEPlayer>().PylonBegin = EEWorld.EEWorld.PylonEnd[i];
                    Main.LocalPlayer.GetModPlayer<EEPlayer>().PylonEnd = EEWorld.EEWorld.PylonEnd[i + 1];
                    Main.LocalPlayer.GetModPlayer<EEPlayer>().ridingZipline = true;
                }
            }
        }

        public void UpdatePowerLevel()
        {
            if (player.controlUseItem)
            {
                powerLevel += 0.2f;
                if (powerLevel > maxPowerLevel)
                {
                    powerLevel = maxPowerLevel;
                }
            }
            else
            {
                powerLevel = 0;
            }
        }

        public void UpdateCutscenesAndTempShaders()
        {
            Filters.Scene[shad1].GetShader().UseOpacity(timerForCutscene);
            if (Main.netMode != NetmodeID.Server && !Filters.Scene[shad1].IsActive())
            {
                Filters.Scene.Activate(shad1, player.Center).GetShader().UseOpacity(timerForCutscene);
            }
            if (!godMode)
            {
                if (Main.netMode != NetmodeID.Server && Filters.Scene[shad1].IsActive())
                {
                    Filters.Scene.Deactivate(shad1);
                }
            }
            Filters.Scene[shad3].GetShader().UseOpacity(cutSceneTriggerTimer);
            if (Main.netMode != NetmodeID.Server && !Filters.Scene[shad3].IsActive())
            {
                Filters.Scene.Activate(shad3, player.Center).GetShader().UseOpacity(cutSceneTriggerTimer);
            }
            if (!triggerSeaCutscene)
            {
                if (Main.netMode != NetmodeID.Server && Filters.Scene[shad3].IsActive())
                {
                    Filters.Scene.Deactivate(shad3);
                }
            }
            /*if(Main.netMode != NetmodeID.Server && !Filters.Scene["EEMod:MyTestShader"].IsActive())
            {
                Filters.Scene.Activate("EEMod:MyTestShader", player.Center).GetShader().UseColor(Color.Red).UseTargetPosition(player.Center);
            }*/
            if (timerForCutscene >= 1400)
            {
                Initialize();
                prevKey = KeyID.BaseWorldName;
                SM.SaveAndQuit(KeyID.Pyramids); //pyramid
            }
            if (cutSceneTriggerTimer > 0)
            {
                if (cutSceneTriggerTimer >= 500)
                {
                    Initialize();
                    prevKey = KeyID.BaseWorldName;
                    SM.SaveAndQuit(KeyID.Sea); //sea
                }
            }
        }

        public override Texture2D GetMapBackgroundImage()
        {
            if (ZoneCoralReefs)
            {
                return EEMod.instance.GetTexture("Backgrounds/CoralReefsSurfaceClose");
            }
            return null;
        }

        public override TagCompound Save()
        {
            return new TagCompound
            {
                ["hasGottenRuneBefore"] = hasGottenRuneBefore,
                ["moral"] = moralScore,
                ["baseworldname"] = baseWorldName,
                ["importantCutscene"] = importantCutscene,
                ["swiftSail"] = boatSpeed,
                ["cannonball"] = cannonballType//,
                //["fishLengths"] = fishLengths
                /*
             {"Hours", Hours},
		     {"Minutes", Minutes},
		     {"Seconds", Seconds},
		     {"Milliseconds", Milliseconds},
             */
            };
        }

        public override void Load(TagCompound tag)
        {
            if (tag.ContainsKey("hasGottenRuneBefore"))
            {
                hasGottenRuneBefore = tag.GetByteArray("hasGottenRuneBefore");
            }
            if (tag.ContainsKey("moral"))
            {
                moralScore = tag.GetInt("moral");
            }
            if (tag.ContainsKey("baseworldname"))
            {
                baseWorldName = tag.GetString("baseworldname");
            }
            if (tag.ContainsKey("importantCutscene"))
            {
                importantCutscene = tag.GetBool("importantCutscene");
            }
            if (tag.ContainsKey("swiftSail"))
            {
                boatSpeed = tag.GetInt("swiftSail");
            }
            if (tag.ContainsKey("cannonball"))
            {
                cannonballType = tag.GetInt("cannonball");
            }
            /*if (tag.ContainsKey("fishLengths"))
            {
                fishLengths = tag.GetList("fishLengths");
            }*/
            /*
                if (tag.ContainsKey("Hours"))
		           Hours = tag.GetInt("Hours");
		       if (tag.ContainsKey("Minutes"))
		            Minutes = tag.GetInt("Minutes");
		       if (tag.ContainsKey("Seconds"))
		           Seconds = tag.GetInt("Seconds");
		      if (tag.ContainsKey("Milliseconds"))
		          Milliseconds = tag.GetInt("Milliseconds");
                  */
        }

        public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            /*if (dalantiniumSet)
            {
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(player.Center, new Vector2(Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-2, 2)), ProjectileType<DalantiniumFang>(), 12, 2f);
                }
            }*/
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (godMode)
            {
                int getRand = Main.rand.Next(5);
                int healSet = Helpers.Clamp(damage / 9, 1, 5);

                if (getRand == 1)
                {
                    player.statLife += healSet;
                    player.HealEffect(healSet);
                }
            }
            if (isQuartzRangedOn && item.ranged)
            {
                if (crit)
                {
                    target.AddBuff(BuffID.CursedInferno, 120);
                }
            }
            if (isQuartzSummonOn && item.summon)
            {
                if (Main.rand.Next(10) < 3)
                {
                    target.AddBuff(BuffID.OnFire, 180);
                }
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (isQuartzRangedOn && proj.ranged)
            {
                if (crit)
                {
                    target.AddBuff(BuffID.CursedInferno, 120);
                }
            }
            if (isQuartzSummonOn && proj.minion)
            {
                if (Main.rand.Next(10) < 3)
                {
                    target.AddBuff(BuffID.OnFire, 180);
                }
            }
        }

        public override void clientClone(ModPlayer clientClone)
        {
        }

        public Vector2 EEPosition;

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = mod.GetPacket();
            packet.Write(triggerSeaCutscene);
            packet.WriteVector2(EEPosition);
            packet.Send(toWho, fromWho);
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            EEPlayer clone = clientPlayer as EEPlayer;

            if (clone.EEPosition != EEMod.instance.position)
            {
                var packet = mod.GetPacket();
                packet.Write(triggerSeaCutscene);
                packet.WriteVector2(EEPosition);
                packet.Send();
            }
        }

        private void ResetMinionEffect()
        {
            quartzCrystal = false;
        }

        public int hours;
        public int minutes;
        public int seconds;
        public int milliseconds;

        public override void PreUpdate()
        {
            if (Main.frameRate != 0)
            {
                milliseconds += 1000 / Main.frameRate;
            }

            if (milliseconds >= 1000)
            {
                milliseconds = 0;
                seconds++;
            }
            if (seconds >= 60)
            {
                seconds = 0;
                minutes++;
            }
            if (minutes >= 60)
            {
                minutes = 0;
                hours++;
            }
        }

        internal static void UpdateOceanMapElements()
        {
            for (int i = 0; i < OceanMapElements.Count; i++)
            {
                OceanMapElements[i].Update();
            }
        }

        public struct Island
        {
            EEPlayer player => Main.LocalPlayer.GetModPlayer<EEPlayer>();

            public Island(Vector2 pos, Texture2D tex, string NameOfIsland, int frameCount = 1, int frameSpid = 2, bool canCollide = false, int startingFrame = 0)
            {
                posX = (int)pos.X;
                posY = (int)pos.Y;
                texture = tex;
                frames = frameCount;
                frameSpeed = frameSpid;
                currentFrame = startingFrame;
                this.canCollide = canCollide;
                if (NameOfIsland != null)
                {
                    if (!player.Islands.ContainsKey(NameOfIsland))
                    {
                        player.Islands.Add(NameOfIsland, this);
                    }
                }
            }

            private readonly int posX;
            private readonly int posY;
            public int frames;
            public int currentFrame;
            public int frameSpeed;
            public bool canCollide;

            public int posXToScreen
            {
                get => posX + (int)Main.screenPosition.X + Main.screenWidth;
            }

            public int posYToScreen
            {
                get => posY + (int)Main.screenPosition.Y + Main.screenHeight;
            }

            public Texture2D texture;
            public Vector2 posToScreen => new Vector2(posXToScreen - texture.Width / 2, posYToScreen - texture.Height / (2 * frames));
            public Rectangle hitBox => new Rectangle((int)posToScreen.X, (int)posToScreen.Y - texture.Height / (frames * 2), texture.Width, texture.Height / (frames));
            private Rectangle ShipHitBox => new Rectangle((int)Main.screenPosition.X + (int)EEMod.instance.position.X - 30, (int)Main.screenPosition.Y + (int)EEMod.instance.position.Y - 30, 30, 30);
            public bool isColliding => hitBox.Intersects(ShipHitBox) && canCollide;
        }

        public class DarkCloud : IOceanMapElement
        {
            public Vector2 pos;
            public float flash;

            public int posXToScreen
            {
                get => (int)(pos.X + Main.screenPosition.X);
            }

            public int posYToScreen
            {
                get => (int)(pos.Y + Main.screenPosition.Y);
            }

            public Texture2D texture;
            public float scale, alpha;
            private readonly EEPlayer modPlayer = Main.LocalPlayer.GetModPlayer<EEPlayer>();

            public DarkCloud(Vector2 pos, Texture2D tex, float scale, float alpha)
            {
                flash += 0.01f;
                this.pos = pos;
                texture = tex;
                this.scale = scale;
                this.alpha = alpha;
                flash = Main.rand.NextFloat(0, 4);
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                flash += 0.003f;
                Vector2 p = new Vector2(posXToScreen + (float)Math.Sin(flash) * 10, posYToScreen - 1000).ForDraw();
                Color drawcolor = Lighting.GetColor(posXToScreen / 16, (posYToScreen - 1000) / 16) * modPlayer.seamapLightColor;
                drawcolor.A = (byte)alpha;
                if (modPlayer.quickOpeningFloat > 0.01f)
                {
                    float lerp = 1 - (modPlayer.quickOpeningFloat / 10f);
                    spriteBatch.Draw(texture, p, null, drawcolor * lerp, 0f, default, scale, SpriteEffects.None, 0f);
                    return;
                }
                spriteBatch.Draw(texture, p, null, drawcolor * (1 - (modPlayer.cutSceneTriggerTimer / 180f)), 0f, default, scale, SpriteEffects.None, 0f);
            }

            public void Update()
            {
            }
        }

        public class MovingCloud : IOceanMapElement
        {
            public Vector2 pos;
            public float flash;

            public int posXToScreen
            {
                get => (int)(pos.X + Main.screenPosition.X);
            }

            public int posYToScreen
            {
                get => (int)(pos.Y + Main.screenPosition.Y);
            }

            public Texture2D texture;
            public float scale, alpha;
            private readonly EEPlayer modPlayer = Main.LocalPlayer.GetModPlayer<EEPlayer>();

            public MovingCloud(Vector2 pos, Texture2D tex, float scale, float alpha)
            {
                this.pos = pos;
                texture = tex;
                this.scale = scale;
                this.alpha = alpha;
                flash = Main.rand.NextFloat(0, 4);
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                flash += 0.003f;
                Vector2 p = new Vector2(posXToScreen + (float)Math.Sin(flash) * 10, posYToScreen - 1000).ForDraw();
                Color drawcolor = Lighting.GetColor(posXToScreen / 16, (posYToScreen - 1000) / 16) * modPlayer.brightness * (modPlayer.isStorming ? 2 / 3f : 1);
                drawcolor.A = (byte)alpha;
                spriteBatch.Draw(texture, p, null, drawcolor * (1 - (modPlayer.cutSceneTriggerTimer / 180f)), 0f, default, scale, SpriteEffects.None, 0f);
            }

            public void Update()
            {
            }
        }

        public class MCloud : IOceanMapElement
        {
            private Vector2 position;
            private readonly int width, height;
            private readonly float alpha, scale;
            private readonly Texture2D texture;
            private readonly EEPlayer modPlayer = Main.LocalPlayer.GetModPlayer<EEPlayer>();
            private Vector2 Center => new Vector2(position.X + width / 2f, position.Y + height / 2f);

            public MCloud(Texture2D texture, Vector2 position, int width, int height, float scale, float alpha)
            {
                //scale = projectile.ai[0];
                //alpha = (int)projectile.ai[1];
                this.scale = scale;
                this.alpha = alpha;
                this.texture = texture;
                this.position = position;
                this.width = width;
                this.height = height;
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                Vector2 newPos = Center + Main.screenPosition;
                Rectangle rect = new Rectangle(0, 0, width, height);
                Color lightColour = Lighting.GetColor((int)newPos.X / 16, (int)newPos.Y / 16) * modPlayer.brightness * (modPlayer.isStorming ? 2 / 3f : 1);
                spriteBatch.Draw(texture, Center, rect, lightColour * (alpha / 255f) * (1 - (modPlayer.cutSceneTriggerTimer / 180f)), 0f, rect.Size() / 2, scale, SpriteEffects.None, 0f);
            }

            public void Update()
            {
                position.X -= 0.3f;
            }
        }

        internal interface IOceanMapElement
        {
            void Update();

            void Draw(SpriteBatch spriteBatch);
        }

        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            if (!Main.gameMenu)
            {
                if (player.wet)
                {
                    if (drawInfo.drawPlayer.fullRotation < MathHelper.ToRadians(90) && drawInfo.drawPlayer.fullRotation > MathHelper.ToRadians(-90))
                    {
                        if (drawInfo.drawPlayer.direction == 1 && Main.MouseWorld.X > drawInfo.drawPlayer.position.X)
                        {
                            drawInfo.drawPlayer.headRotation = Utils.Clamp((Main.MouseWorld - drawInfo.drawPlayer.Center).ToRotation(), -0.5f, 0.5f);
                        }
                        else if (drawInfo.drawPlayer.direction == -1 && Main.MouseWorld.X < drawInfo.drawPlayer.position.X)
                        {
                            drawInfo.drawPlayer.headRotation = Utils.Clamp((drawInfo.drawPlayer.Center - Main.MouseWorld).ToRotation(), -0.5f, 0.5f);
                        }
                    }
                }
            }
        }
    }
}