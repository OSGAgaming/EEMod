using EEMod.Config;
using EEMod.Effects;
using EEMod.Extensions;
using EEMod.ID;
using EEMod.NPCs.Bosses.Kraken;
using EEMod.Projectiles;
using EEMod.Tiles;
using EEMod.Tiles.Furniture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.GameContent.LiquidAmount;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Social;
using Terraria.UI;
using EEMod.Prim;
using EEMod.Seamap.SeamapContent;
using MonoMod.RuntimeDetour.HookGen;
using Terraria.ModLoader.Audio;
using EEMod.Systems;

namespace EEMod
{
    public partial class EEMod : Mod
    {
        //private delegate void D(ref VertexColors colors);
        private float _alphaBG;
        private Vector2 _sunPos;
        private float _globalAlpha;
        private float _intensityFunction;
        private Vector2 _sunShaderPos;
        private float _nightHarshness = 1f;
        private Color _baseColor;
        private Texture2D _screenTexture;
        private Texture2D _texture2;
        //private Rectangle _screenFrame;//unused?
        private int _counter;
        private int _screenframes;
        private int _screenframeSpeed;
        private float alpha;

        public static string screenMessageText;
        public static string progressMessage;
        public static TrailManager trailManager;
        public static PrimTrailManager primitives;
        public static Prims prims;
        public float seed;
        public float speed;
        /// <summary>
        /// Instance for adding and handling il hooks
        /// </summary>
        internal ILHookList hooklist;

        private void LoadIL()
        {
            //IL.Terraria.Main.DrawBackground += Main_DrawBackground;
            //IL.Terraria.Main.DrawWater += Main_DrawWater;
            //IL.Terraria.Main.OldDrawBackground += Main_OldDrawBackground;
            //IL.Terraria.NPC.AI_001_Slimes += Practice;
            //IL.Terraria.Main.oldDrawWater += Main_oldDrawWater;
            hooklist = new ILHookList();

            IL.Terraria.GameContent.LiquidAmount.LiquidRenderer.InternalPrepareDraw += LiquidRenderer_InternalDraw1;
            hooklist.Add(typeof(MusicStreamingOGG).GetMethod("FillBuffer", BindingFlags.NonPublic | BindingFlags.Instance), LayeredMusic.ILFillBuffer);

            //HookEndpointManager.Modify(typeof(MusicStreamingOGG).GetMethod("FillBuffer", BindingFlags.NonPublic | BindingFlags.Instance), (ILContext.Manipulator)LayeredMusic.ILFillBuffer);
        }

        private void UnloadIL()
        {
            //IL.Terraria.Main.DrawBackground -= Main_DrawBackground;
            //IL.Terraria.Main.DrawWater -= Main_DrawWater;
            //IL.Terraria.Main.OldDrawBackground -= Main_OldDrawBackground;
            //IL.Terraria.Main.oldDrawWater -= Main_oldDrawWater;
            //IL.Terraria.NPC.AI_001_Slimes -= Practice;
            IL.Terraria.GameContent.LiquidAmount.LiquidRenderer.InternalPrepareDraw -= LiquidRenderer_InternalDraw1;
            //IL.Terraria.GameContent.LiquidAmount.LiquidRenderer.InternalDraw -= Traensperentaoiasjpdfdsgwuttttttttttttttryddddddddddtyrrrrrrrrrrrrrrrrrvvfghnmvvb;
            //HookEndpointManager.Unmodify(typeof(MusicStreamingOGG).GetMethod("FillBuffer", BindingFlags.NonPublic | BindingFlags.Instance), (ILContext.Manipulator)LayeredMusic.ILFillBuffer);
            hooklist?.UnloadAll();
            hooklist?.Dispose();
            hooklist = null;
            screenMessageText = null;
            trailManager = null;
            progressMessage = null;
            prims = null;
        }

        private void LiquidRenderer_InternalDraw1(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            Type t = typeof(LiquidRenderer).GetNestedType("LiquidCache", BindingFlags.NonPublic | BindingFlags.Public);
            FieldInfo issolid = t.GetField("IsSolid");
            if (!c.TryGotoNext(i => i.MatchStfld(issolid)))
                throw new Exception();
            // before the stfld there will be an int on the stack
            c.Emit(OpCodes.Ldloc, 3); // tile
            c.EmitDelegate<Func<bool, Tile, bool>>((orig, tile) => orig && tile.type != ModContent.TileType<EmptyTile>());
        }

        private void Main_oldDrawWater(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            ILLabel l = c.DefineLabel(); // where Color color = Lighting.GetColor(j, i);
            MethodInfo drawcall = typeof(Lighting).GetMethod(nameof(Lighting.GetColor), new Type[] { typeof(int), typeof(int) });
            if (!c.TryGotoNext(
                i => i.MatchCallOrCallvirt(typeof(Tile).GetMethod(nameof(Tile.nactive))),
                i => i.MatchBrfalse(out _)))
            {
                throw new Exception("Could not modify draw water");
            }
            // callvirt  instance bool Terraria.Tile::nactive()
            // brfalse.s IL_01CB // after !Main.tileSolid[(int)Main.tile-[j, i].type] || Main.tileSolidTop[(int)Main.tile-[j, i].type]
            c.Index += 2;   // ldsfld    bool[] Terraria.Main::tileSolid
            c.Emit(OpCodes.Ldloc, 12); // i
            c.Emit(OpCodes.Ldloc, 11); // j
            c.EmitDelegate<Func<int, int, bool>>((i, j) => true);

            c.Emit(OpCodes.Brtrue, l); // skip the other checks

            if (!c.TryGotoNext(i => i.MatchLdloc(12),
                i => i.MatchLdloc(11),
                i => i.MatchCall(typeof(Lighting).GetMethod(nameof(Lighting.GetColor), new Type[] { typeof(int), typeof(int) }))))
            { // match call
                throw new Exception("Couldn't find call to Lighting.GetColor");
            }

            c.MarkLabel(l); // point to current instr (ldloc 12)
        }

        private void Main_DrawWater(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            Type liqRend = typeof(LiquidRenderer);
            MethodInfo drawcall = liqRend.GetMethod(nameof(LiquidRenderer.Draw), new Type[] { typeof(SpriteBatch), typeof(Vector2), typeof(int), typeof(float), typeof(bool) });

            if (!c.TryGotoNext(i => i.MatchCallvirt(drawcall)))
            {
                throw new Exception("Couldn't find argument 1 post lc1");
            }

            c.Remove();
            c.EmitDelegate<Action<LiquidRenderer, SpriteBatch, Vector2, int, float, bool>>((t, spritebatch, drawOffset, Style, Alpha, bg) =>
            {
                t.Draw(spritebatch, drawOffset, Style, Alpha / 2, bg);
            });
        }

        /*public void DrawRef()
        {
            RenderTarget2D buffer = Main.screenTarget;

            Main.graphics.GraphicsDevice.SetRenderTarget(null);

            Color[] texdata = new Color[buffer.Width * buffer.Height];

            buffer.GetData(texdata);

            Texture2D screenTex = new Texture2D(Main.graphics.GraphicsDevice, buffer.Width, buffer.Height);

            screenTex.SetData(texdata);

            Main.spriteBatch.Draw(screenTex, Main.LocalPlayer.Center.ForDraw(), new Rectangle(0, 0, 1980, 1017), Color.White * 0.3f, 0f, new Rectangle(0, 0, 1980, 1017).Size() / 2, 1, SpriteEffects.FlipVertically, 0);
            Main.graphics.GraphicsDevice.SetRenderTarget(Main.screenTarget);
        }*/

        /*private void Traensperentaoiasjpdfdsgwuttttttttttttttryddddddddddtyrrrrrrrrrrrrrrrrrvvfghnmvvb(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            MethodInfo call = typeof(Lighting).GetMethod(nameof(Lighting.GetColor4Slice_New), new Type[]
            {
                typeof(int), typeof(int), typeof(VertexColors).MakeByRefType(), typeof(float)
            });

            if (!c.TryGotoNext(i => i.MatchCall(call)))
            {
                throw new Exception("fapsdimajpxfasafasfdddddddddddddddddddddddddddddddddddfvcxfgresdgsedf");
            }

            c.Index++;

            c.Emit(OpCodes.Ldloca, 9);
            c.Emit(OpCodes.Call, new D(ModifyWaterColor).GetMethodInfo());
        }*/

        /*private static void ModifyWaterColor(ref VertexColors colors)
        {
            Color c = Color.White;

            colors.TopLeftColor =c;
            colors.TopRightColor = c;
            colors.BottomLeftColor = c;
            colors.BottomRightColor = c;
        }*/

        //No. Just no.
        /*private void Practice(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(i => i.MatchLdloc(12),
                i => i.MatchLdcR4(200),
                i => i.MatchBneUn(out _),
                i => i.MatchBneUn(out _),
                i => i.MatchStfld(typeof(Vector2).GetField("X"))))
            {
                return;
            }

            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Action<NPC>>(npc =>
            {
                npc.velocity.Y = -10;
            });

            throw new Exception("Couldn't find local variable 19 loading");
        }*/

        public void UnloadShaderAssets()
        {
            if (Main.netMode != NetmodeID.Server && Filters.Scene["EEMod:Saturation"].IsActive())
            {
                Filters.Scene["EEMod:Saturation"].Deactivate();
            }
        }

        private void Main_OldDrawBackground(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            Type spritebatchType = typeof(SpriteBatch);
            MethodInfo drawcall = spritebatchType.GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color) });
            MethodInfo drawcall2 = spritebatchType.GetMethod(nameof(SpriteBatch.Draw), new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) });
            MethodInfo get_noretro = typeof(Lighting).GetProperty(nameof(Lighting.NotRetro)).GetGetMethod();

            if (!c.TryGotoNext(i => i.MatchLdloc(19)))
            {
                throw new Exception("Couldn't find local variable 19 loading");
            }

            if (!c.TryGotoNext(i => i.MatchCallvirt(drawcall)))
            {
                throw new Exception("Couldn't find call (post variable 19)");
            }

            //int p = c.Index;
            //c.Index++; // move past
            //var label = c.DefineLabel(); // define label
            //c.Goto(p); // return
            //c.Emit(OpCodes.Br, label); // skip
            //c.MarkLabel(label); // define label target
            //c.Index--;

            c.Remove();
            c.Emit(OpCodes.Ldloc, 15); // array
            c.EmitDelegate<Action<SpriteBatch, Texture2D, Vector2, Rectangle?, Color, int[]>>((sb, texture, pos, rectangle, color, array) =>
            {
                if (array[4] != 135 && array[4] != 131)
                {
                    sb.Draw(texture, pos, rectangle, color);
                }
            });

            if (!c.TryGotoNext(i => i.MatchCall(get_noretro)))
            {
                throw new Exception("Couldn't find Lighting.NoRetro get call");
            }

            for (int k = 0; k < 4; k++)
            {
                if (!c.TryGotoNext(i => i.MatchCallvirt(drawcall2)))
                {
                    throw new Exception($"Couldn't find call {k}");
                }

                c.Remove();
                c.Emit(OpCodes.Ldloc, 15);
                c.EmitDelegate<Action<SpriteBatch, Texture2D, Vector2, Rectangle?, Color, float, Vector2, float, SpriteEffects, float, int[]>>((sb, texture, position, sourcerectangle, color, rotation, origin, scale, effects, layerdepth, array) =>
                {
                    if (array[5] != 126 && array[5] != 125)
                    {
                        sb.Draw(texture, position, sourcerectangle, color, rotation, origin, scale, effects, layerdepth);
                    }
                });
            }
        }

        public void DrawGlobalShaderTextures()
        {
            double num10;

            if (Main.time < 27000.0)
            {
                num10 = Math.Pow(1.0 - Main.time / 54000.0 * 2.0, 2.0);
            }
            else
            {
                num10 = Math.Pow((Main.time / 54000.0 - 0.5) * 2.0, 2.0);
            }

            Rectangle[] rects = { new Rectangle(0, 0, ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("Textures/SunRing").Value.Width, ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("Textures/SunRing").Value.Height), new Rectangle(0, 0, ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("Textures/LensFlare").Value.Width, ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("Textures/LensFlare").Value.Height) };

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

            if (EEModConfigClient.Instance.BetterLighting)
            {
                Main.spriteBatch.Draw(ModContent.Request<Texture2D>("EEMod/Projectiles/Nice"), _sunPos - Main.screenPosition, new Rectangle(0, 0, 174, 174), Color.White * .5f * _globalAlpha * (_intensityFunction * 0.36f), (float)Math.Sin(Main.time / 540f), new Vector2(87), 10f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("Textures/LensFlare").Value, _sunPos - Main.screenPosition + new Vector2(5, 28 + (float)num10 * 250), rects[1], Color.White * _globalAlpha * _intensityFunction, (float)Math.Sin(Main.time / 540f), rects[1].Size() / 2, 1.3f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("Textures/SunRing").Value, _sunPos - Main.screenPosition + new Vector2(0, 37 + (float)num10 * 250), rects[0], Color.White * .7f * _globalAlpha * (_intensityFunction * 0.36f), (float)Math.Sin(Main.time / 5400f), rects[0].Size() / 2, 1f, SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public void DrawLensFlares()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

            if (EEModConfigClient.Instance.BetterLighting && Main.worldName != KeyID.CoralReefs)
            {
                Main.spriteBatch.Draw(ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("Textures/LensFlare2").Value, _sunPos - Main.screenPosition + new Vector2(-400, 400), new Rectangle(0, 0, 174, 174), Color.White * .7f * _globalAlpha * (_intensityFunction * 0.36f), 0f, new Vector2(87), 1f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("Textures/LensFlare2").Value, _sunPos - Main.screenPosition + new Vector2(-800, 800), new Rectangle(0, 0, 174, 174), Color.White * .8f * _globalAlpha * (_intensityFunction * 0.36f), 0f, new Vector2(87), .5f, SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public void BetterLightingHandler()
        {
            Color DayColour = new Color(2.5f, 1.4f, 1);
            Color NightColour = new Color(1.2f, 1.4f, 2f);
            float shiftSpeed = 32f;
            float Base = 0.5f;
            float timeProgression = (float)Main.time / 54000f;
            float baseIntesity = 0.05f;
            float Intensity = .7f;
            float flunctuationCycle = 20000;
            float nightTransitionSpeed = 0.005f;
            float globalAlphaTransitionSpeed = 0.001f;
            float maxNightDarkness = 0.5f;

            if (Main.dayTime)
            {
                _baseColor.R += (byte)((DayColour.R - _baseColor.R) / shiftSpeed);
                _baseColor.G += (byte)((DayColour.G - _baseColor.G) / shiftSpeed);
                _baseColor.B += (byte)((DayColour.B - _baseColor.B) / shiftSpeed);
                _sunShaderPos = new Vector2(timeProgression, 0.1f);
                _sunPos = Main.screenPosition + new Vector2(timeProgression * Main.screenWidth, 100);

                if (_nightHarshness < 1)
                {
                    _nightHarshness += nightTransitionSpeed;
                }

                if (_globalAlpha < 1)
                {
                    _globalAlpha += globalAlphaTransitionSpeed;
                }
            }
            else
            {
                _baseColor.R += (byte)((NightColour.R - _baseColor.R) / shiftSpeed);
                _baseColor.G += (byte)((NightColour.G - _baseColor.G) / shiftSpeed);
                _baseColor.B += (byte)((NightColour.B - _baseColor.B) / shiftSpeed);
                _sunShaderPos = new Vector2(1 - timeProgression, 0.1f);
                _sunPos = Main.LocalPlayer.Center - new Vector2((timeProgression - 0.5f) * 2 * Main.screenWidth, Main.LocalPlayer.Center.Y - Main.screenHeight / 2.2f);

                if (_nightHarshness > maxNightDarkness)
                {
                    _nightHarshness -= nightTransitionSpeed;
                }

                if (_globalAlpha > 0)
                {
                    _globalAlpha -= globalAlphaTransitionSpeed * 10;
                }
            }

            _intensityFunction = Math.Abs((float)Math.Sin(Main.time / flunctuationCycle) * Intensity) + baseIntesity;

            if (Main.netMode != NetmodeID.Server && !Filters.Scene["EEMod:Saturation"].IsActive())
            {
                Filters.Scene.Activate("EEMod:Saturation", Vector2.Zero).GetShader();
            }

            Filters.Scene["EEMod:Saturation"].GetShader().UseImageOffset(_sunShaderPos).UseIntensity(_intensityFunction).UseOpacity(4f).UseProgress(Main.dayTime ? 0 : 1).UseColor(Base, _nightHarshness, 0).UseSecondaryColor(_baseColor);
        }

        public void DrawCoralReefsBg()
        {
            return; // nothing being drawn atm

            int maxLoops = 5;
            Color drawColor = Lighting.GetColor((int)(Main.LocalPlayer.Center.X / 16f) - 5, (int)(Main.LocalPlayer.Center.Y / 16f) - 5) * _alphaBG;
            float scale = 1.5f;
            Vector2 traverseFunction = new Vector2(4000, 1000);
            Vector2 traverse = new Vector2(-Main.LocalPlayer.Center.X / (Main.maxTilesX * 16) * traverseFunction.X, -Main.LocalPlayer.Center.Y / (Main.maxTilesY * 16) * traverseFunction.Y);
            Texture2D CB1 = ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("Backgrounds/CoralReefsSurfaceFar").Value; //instance.GetTexture("Backgrounds /CoralReefsSurfaceFar");
            Texture2D CB2 = ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("Backgrounds/CoralReefsSurfaceMid").Value; //instance.GetTexture("Backgrounds /CoralReefsSurfaceMid");
            Texture2D CB3 = ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("Backgrounds/CoralReefsSurfaceClose").Value;
            Rectangle GlobalRect = new Rectangle(0, 0, (int)(CB1.Width * scale), (int)(CB1.Height * scale));
            Rectangle GlobalRectUnscaled = new Rectangle(0, 0, CB1.Width, CB1.Height);

            if (Main.ActiveWorldFileData.Name == KeyID.CoralReefs)
            {
                for (int i = 0; i < maxLoops; i++)
                {
                    Vector2 Positions = new Vector2((i - ((maxLoops - 1) * 0.5f)) * CB1.Width * scale, traverseFunction.Y / 3f);
                    Main.spriteBatch.Draw(CB1, Positions + Main.LocalPlayer.Center - Main.screenPosition + traverse, GlobalRectUnscaled, drawColor, 0f, GlobalRectUnscaled.Size() / 2, scale, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(CB2, Positions + Main.LocalPlayer.Center - Main.screenPosition + traverse, GlobalRectUnscaled, drawColor, 0f, GlobalRectUnscaled.Size() / 2, scale, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(CB3, Positions + Main.LocalPlayer.Center - Main.screenPosition + traverse, GlobalRectUnscaled, drawColor, 0f, GlobalRectUnscaled.Size() / 2, scale, SpriteEffects.None, 0f);
                }
            }
        }

        
        int screenLerp;
        public void DrawSky()
        {
            switch (loadingChooseImage)
            {
                case 0:
                    _texture2 = ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("UI/LoadingScreenImages/LoadingScreen1").Value;
                    break;
                case 1:
                    _texture2 = ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("UI/LoadingScreenImages/LoadingScreen2").Value;
                    break;
                case 2:
                    _texture2 = ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("UI/LoadingScreenImages/LoadingScreen3").Value;
                    break;
                default:
                    _texture2 = ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("UI/LoadingScreenImages/LoadingScreen4").Value;
                    break;
            }
            switch (loadingChooseImage)
            {
                case 0:
                {
                    _screenTexture = ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("NPCs/CoconutCrab").Value;
                    _screenframes = 4;
                    _screenframeSpeed = 5;
                    break;
                }

                case 1:
                {
                    _screenTexture = ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("NPCs/CoralReefs/HermitCrab").Value;
                    _screenframes = 4;
                    _screenframeSpeed = 5;
                    break;
                }
                case 2:
                {
                    _screenTexture = ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("NPCs/CoralReefs/Seahorse").Value;
                    _screenframes = 7;
                    _screenframeSpeed = 4;
                    break;
                }
                case 3:
                {
                    _screenTexture = ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("NPCs/CoralReefs/GlisteningReefs/Lionfish").Value;
                    _screenframes = 8;
                    _screenframeSpeed = 10;
                    break;
                }
                case 4:
                {
                    _screenTexture = ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("NPCs/CoralReefs/MechanicalReefs/MechanicalShark").Value;
                    _screenframes = 6;
                    _screenframeSpeed = 10;
                    break;
                }
            }

            if (_counter++ > _screenframeSpeed)
            {
                _counter = 0;
                SeamapPlayerShip.localship.frame.Y += _screenTexture.Height / _screenframes;
            }

            if (SeamapPlayerShip.localship.frame.Y >= _screenTexture.Height / _screenframes * (_screenframes - 1))
            {
                SeamapPlayerShip.localship.frame.Y = 0;
            }

            Vector2 position = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2 + 30);

            float width = _texture2.Width;
            float height = _texture2.Height;

            if (width < Main.screenWidth)
            {
                width = Main.screenWidth;
                height *= (Main.screenWidth / _texture2.Width);

                if (height < Main.screenHeight)
                {
                    width *= (Main.screenHeight / height);
                    height = Main.screenHeight;
                }
            }

            Main.spriteBatch.Draw(_texture2, new Rectangle(Main.screenWidth / 2, Main.screenHeight / 2, (int)width + 8, (int)height + 8), _texture2.Bounds, Color.Lerp(Color.Black, Color.White, lerp), 0, origin: new Vector2(_texture2.Width / 2, _texture2.Height / 2), SpriteEffects.None, 0);

            Main.spriteBatch.Draw(_screenTexture, position, new Rectangle(0, SeamapPlayerShip.localship.frame.Y, _screenTexture.Width, _screenTexture.Height / _screenframes), new Color(0, 0, 0), 0, new Rectangle(0, SeamapPlayerShip.localship.frame.Y, _screenTexture.Width, _screenTexture.Height / _screenframes).Size() / 2, 1, SpriteEffects.None, 0);
        }

        private void Main_DrawBackground(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            Type spritebatchtype = typeof(SpriteBatch);

            if (!c.TryGotoNext(i => i.MatchLdloc(18)))
            {
                throw new Exception("Ldloc for local variable 18 not found");
            }

            MethodInfo call1 = spritebatchtype.GetMethod("Draw", new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color) });

            if (!c.TryGotoNext(i => i.MatchCallvirt(call1)))
            {
                throw new Exception("No call found for SpriteBatch.Draw(Texture2D, Vector2, Rectangle?, Color)");
            }

            // 1st call
            c.Remove();
            c.Emit(OpCodes.Ldloc, 13); // array
            c.EmitDelegate<Action<SpriteBatch, Texture2D, Vector2, Rectangle?, Color, int[]>>((spritebatch, texture, pos, sourcerectangle, color, array) =>
            {
                if (array[4] != 135)
                {
                    spritebatch.Draw(texture, pos, sourcerectangle, color);
                }
            });

            // 2nd call
            // getting to the else
            MethodInfo lightningnoretroget = typeof(Lighting).GetProperty(nameof(Lighting.NotRetro)).GetGetMethod();

            if (!c.TryGotoNext(i => i.MatchCallOrCallvirt(lightningnoretroget)))
            {
                throw new Exception("Call for the get method of the property Lighting.NoRetro not found");
            }

            // finding the call
            MethodInfo draw = spritebatchtype.GetMethod("Draw", new Type[] { typeof(Texture2D), typeof(Vector2), typeof(Rectangle?), typeof(Color), typeof(float), typeof(Vector2), typeof(float), typeof(SpriteEffects), typeof(float) });

            for (int k = 0; k < 4; k++) // 4 calls
            {
                if (!c.TryGotoNext(i => i.MatchCallvirt(draw)))
                {
                    throw new Exception($"Call number {k} not found");
                }

                c.Remove();
                c.Emit(OpCodes.Ldloc, 13); // array
                c.EmitDelegate<Action<SpriteBatch, Texture2D, Vector2, Rectangle?, Color, float, Vector2, float, SpriteEffects, float, int[]>>((spritebatch, texture, position, sourcerectangle, color, rotation, origin, scale, effects, layerdepth, array) =>
                {
                    if (array[5] != 126)
                    {
                        spritebatch.Draw(texture, position, sourcerectangle, color, rotation, origin, scale, effects, layerdepth);
                    }
                });
            }

            // 3rd call
            if (!c.TryGotoNext(i => i.MatchLdloc(20)))
            {
                throw new Exception("Ldloc for local variable 20 (flag4) not found"); // flag4
            }

            if (!c.TryGotoNext(i => i.MatchCallvirt(call1)))
            {
                throw new Exception("'Last' SpriteBatch.Draw call not found"); // same overload
            }

            c.Remove();
            c.Emit(OpCodes.Ldloc, 13); // array
            c.EmitDelegate<Action<SpriteBatch, Texture2D, Vector2, Rectangle?, Color, int[]>>((spritebatch, texture, position, sourcerectangle, color, array) =>
            {
                if (array[6] != 186)
                {
                    spritebatch.Draw(texture, position, sourcerectangle, color);
                }
            });
        }

        public class ILHook : IDisposable
        {
            MethodInfo _to;
            Delegate _manipulator;
            bool _applied;
            private bool disposed;
            public ILHook(MethodInfo to, ILContext.Manipulator manipulator, bool apply = true)
            {
                _to = to;
                _manipulator = manipulator;
                if (apply)
                    Apply();
            }
            public void Apply()
            {
                if (_applied)
                    return;
                if (disposed)
                    throw new ObjectDisposedException("Cannot apply a disposed ILHook");

                HookEndpointManager.Modify(_to, _manipulator);
                _applied = true;
                OnApply?.Invoke();
            }
            public void Unapply()
            {
                if (!_applied)
                    return;
                HookEndpointManager.Unmodify(_to, _manipulator);
                _applied = false;
                OnUnapply?.Invoke();
            }
            public event Action OnDispose;
            public event Action OnApply;
            public event Action OnUnapply;
            public void Dispose() => Dispose(true);
            protected virtual void Dispose(bool disposing)
            {
                if (disposed)
                    return;

                if (disposing)
                {
                    OnDispose?.Invoke();
                }
                Unapply();
                _to = null;
                _manipulator = null;
                OnDispose = null;
                OnApply = null;
                OnUnapply = null;

                disposed = true;
            }
            ~ILHook() => Dispose(false);
        }
        public class ILHookList : IDisposable
        {
            public IList<ILHook> HookList = new List<ILHook>();
            private bool disposed;
            public ILHook Add(MethodInfo to, ILContext.Manipulator manipulator, bool apply = true)
            {
                var ilhook = new ILHook(to, manipulator, apply);
                HookList.Add(ilhook);
                return ilhook;
            }
            public ILHook Add(ILContext.Manipulator manipulator, Type targetType, string methodname, BindingFlags flags = Helpers.FlagsALL, params Type[] paramTypes) => Add(targetType.GetMethod(methodname, flags, null, paramTypes, null), manipulator);
            public ILHook Add<TType>(ILContext.Manipulator manipulator, string methodname, BindingFlags flags = Helpers.FlagsALL) => Add(typeof(TType).GetMethod(methodname, flags), manipulator);
            public ILHook Add<T0>(ILContext.Manipulator manipulator, Type targetType, string methodname, BindingFlags flags = Helpers.FlagsALL) => Add(manipulator, targetType, methodname, flags, typeof(T0));
            public ILHook Add<T0, T1>(ILContext.Manipulator manipulator, Type targetType, string methodname, BindingFlags flags = Helpers.FlagsALL) => Add(manipulator, targetType, methodname, flags, typeof(T0), typeof(T1));
            public ILHook Add<T0, T1, T2>(ILContext.Manipulator manipulator, Type targetType, string methodname, BindingFlags flags = Helpers.FlagsALL) => Add(manipulator, targetType, methodname, flags, typeof(T0), typeof(T1), typeof(T2));
            public ILHook Add<T0, T1, T2, T3>(ILContext.Manipulator manipulator, Type targetType, string methodname, BindingFlags flags = Helpers.FlagsALL) => Add(manipulator, targetType, methodname, flags, typeof(T0), typeof(T1), typeof(T2), typeof(T3));
            public ILHook Add<T0, T1, T2, T3, T4>(ILContext.Manipulator manipulator, Type targetType, string methodname, BindingFlags flags = Helpers.FlagsALL) => Add(manipulator, targetType, methodname, flags, typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4));
            public ILHook Add<T0, T1, T2, T3, T4, T5>(ILContext.Manipulator manipulator, Type targetType, string methodname, BindingFlags flags = Helpers.FlagsALL) => Add(manipulator, targetType, methodname, flags, typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
            public ILHook Add<T0, T1, T2, T3, T4, T5, T6>(ILContext.Manipulator manipulator, Type targetType, string methodname, BindingFlags flags = Helpers.FlagsALL) => Add(manipulator, targetType, methodname, flags, typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
            public ILHook Add<T0, T1, T2, T3, T4, T5, T6, T7>(ILContext.Manipulator manipulator, Type targetType, string methodname, BindingFlags flags = Helpers.FlagsALL) => Add(manipulator, targetType, methodname, flags, typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));
            public ILHook Add<T0, T1, T2, T3, T4, T5, T6, T7, T8>(ILContext.Manipulator manipulator, Type targetType, string methodname, BindingFlags flags = Helpers.FlagsALL) => Add(manipulator, targetType, methodname, flags, typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8));
            public void UnloadAll()
            {
                if (HookList == null)
                    return;
                foreach (var hook in HookList)
                    hook?.Dispose();
                HookList.Clear();
            }
            public void Dispose() => Dispose(true);
            protected virtual void Dispose(bool disposing)
            {
                if (disposed) return;
                if (disposing)
                {

                }
                UnloadAll();
                HookList?.Clear();
                HookList = null;
                disposed = true;
            }
            ~ILHookList() => Dispose(false);
        }

        /*private static void ILSaveWorldTiles(ILContext il)
{
    ILCursor c = new ILCursor(il);
    PropertyInfo statusText = typeof(Main).GetProperty(nameof(Main.statusText));
    MethodInfo set = statusText.GetSetMethod();

    if (!c.TryGotoNext(i => i.MatchCall(set)))
        throw new Exception();

    c.EmitDelegate<Func<string, string>>((originalText) =>
    {
        return originalText;
    });
}*/
        /*
        //private static void ModifyColor(ref Color color, byte val)
        //{
        //
        //}
        // private delegate void colorrefdelegate(ref Color color, byte val);
        private delegate void modifyingdelegate(Main instance, ref int focusmenu, ref int selectedmenu, ref int num2, ref int num4, ref int[] array4, ref byte[] array6, ref string[] array9, ref bool[] array, ref int num5, ref bool flag);
#pragma warning disable ChangeMagicNumberToID // Change magic numbers into appropriate ID values
        private static void GenkaiMenu(Main instance, ref int focusMenu, ref int selectedMenu, ref int num2, ref int num4, ref int[] array4, ref byte[] array6, ref string[] array9, ref bool[] array, ref int num5, ref bool flag5)
        {
            num2 = 200;
            num4 = 60;
            int offset = -10;
            array4[2] = 30 + offset - 1; //30 - 20; // 30
            array4[3] = 30 + offset - 3 - 1; //30 - 10; // 30
            array6[3] = 2; //2; // rarity // 2
            array4[4] = 70; // 70
            array4[5] = -40 + offset / 2 - 1; // -40 - 10;
            array6[5] = 5;
            if (focusMenu == 2)
            {
                array9[0] = Language.GetTextValue("UI.NormalDescriptionFlavor");
                array9[1] = Language.GetTextValue("UI.NormalDescription");
            }
            else if (focusMenu == 3)
            {
                array9[0] = Language.GetTextValue("UI.ExpertDescriptionFlavor");
                array9[1] = Language.GetTextValue("UI.ExpertDescription");
            }
            else if (focusMenu == 5) // Genkai's
            {
                array9[0] = "Not for easily angried";
                array9[1] = "(What'll it be? Who knows, find out ;])";
            }
            else
            {
#pragma warning disable CS0618 // El tipo o el miembro est�n obsoletos
                array9[0] = Lang.menu[32].Value;
#pragma warning restore CS0618 // El tipo o el miembro est�n obsoletos
            }
            array[0] = true;
            array[1] = true;

            array9[2] = Language.GetTextValue("UI.Normal");
            array9[3] = Language.GetTextValue("UI.Expert");
            array9[4] = Language.GetTextValue("UI.Back");
            array9[5] = "Genkai"; // Genkai
            num5 = 6;
            if (selectedMenu == 2)
            {
                Main.expertMode = false;
                Main.PlaySound(10, -1, -1, 1, 1f, 0f);
                Main.menuMode = 7;
                if (Main.SettingsUnlock_WorldEvil)
                {
                    Main.menuMode = -71;
                }
            }
            else if (selectedMenu == 3)
            {
                Main.expertMode = true;
                Main.PlaySound(10, -1, -1, 1, 1f, 0f);
                Main.menuMode = 7;
                if (Main.SettingsUnlock_WorldEvil)
                {
                    Main.menuMode = -71;
                }
            }
            else if (selectedMenu == 5) // Genkai's
            {
                Main.PlaySound(10, -1, -1, 1, 1f, 0f);
                Main.menuMode = Main.SettingsUnlock_WorldEvil ? -71 : 7;
                Main.expertMode = true;
                EEWorld.EEWorld.GenkaiMode = true;
            }
            else if (selectedMenu == 4 || flag5)
            {
                flag5 = false;
                Main.PlaySound(11, -1, -1, 1, 1f, 0f);
                Main.menuMode = 16;
            }
            Main.clrInput();
        }
#pragma warning restore ChangeMagicNumberToID // Change magic numbers into appropriate ID values
        */
    }
}
