﻿using EEMod.Autoloading;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using System.Collections.Generic;
using EEMod.Extensions;
using System.Linq;
using System;
using EEMod.Effects;
using EEMod.Items.Weapons.Mage;
using static Terraria.ModLoader.ModContent;
using System.Reflection;
using EEMod.Items.Weapons.Melee;
using EEMod.Items.Weapons.Ranger;
using EEMod.NPCs.CoralReefs;
using EEMod.Items.Weapons.Melee.Swords;

namespace EEMod
{
    public class Prims
    {
        //Obsolete

        //Global.graphics.GraphicsDevice for future referencea
        public interface ITrailShader
        {
            string ShaderPass { get; }
            void ApplyShader<T>(Effect effect, T trail, List<Vector2> positions, string ESP, float progressParam);
        }
        public void DrawTrails(SpriteBatch spriteBatch)
        {
            foreach (Trail trail in _trails)
            {
                if (trail.isProjectile)
                    trail.Draw(_effect, _basicEffect, Main.graphics.GraphicsDevice);
            }
            foreach (VerletBuffer verlet in _Verlets)
            {
                verlet.DrawCape(_basicEffect, Main.graphics.GraphicsDevice);
            }
        }
        public void DrawProjectileTrails()
        {
            foreach (Trail trail in _trails)
            {
                if (trail.isProjectile)
                    trail.Draw(_effect, _basicEffect, Main.graphics.GraphicsDevice);
            }
        }
        public class DefaultShader : ITrailShader
        {
            public string ShaderPass => "DefaultPass";
            public void ApplyShader<T>(Effect effect, T trail, List<Vector2> positions, string ESP, float progressParam)
            {
                try
                {
                    effect.Parameters["progress"].SetValue(progressParam);
                    effect.CurrentTechnique.Passes[ESP].Apply();
                    effect.CurrentTechnique.Passes[ShaderPass].Apply();
                }
                catch
                {

                }

            }
        }
        private Effect _effect;
        public static List<Trail> _trails = new List<Trail>();
        private List<VerletBuffer> _Verlets = new List<VerletBuffer>();
        private static BasicEffect _basicEffect;
        public void UpdateTrails()
        {
            for (int i = 0; i < _trails.Count; i++)
            {
                Trail trail = _trails[i];
                trail.Update();
            }
            for (int i = 0; i < _Verlets.Count; i++)
            {
                VerletBuffer trail = _Verlets[i];
                trail.Update();
            }
            Dispose();
        }
        public Prims(Mod mod)
        {
            _trails = new List<Trail>();
            _effect = mod.Assets.Request<Effect>("Effects/trailShaders").Value;
            _basicEffect = new BasicEffect(Main.graphics.GraphicsDevice);
            _basicEffect.VertexColorEnabled = true;
        }
        public void CreateTrail(Projectile projectile = null)
        {
            Trail newTrail = new Trail(new RoundCap(), new DefaultShader(), projectile);
            newTrail.isProjectile = true;
            _trails.Add(newTrail);
        }
        public void CreateTrailWithNPC(Projectile projectile = null, NPC npc = null)
        {
            Trail newTrail = new Trail(new RoundCap(), new DefaultShader(), projectile, npc);
            _trails.Add(newTrail);
        }
        public void Dispose()
        {
            for (int i = 0; i < _trails.Count; i++)
            {
                Trail trail = _trails[i];
                if (trail.npc != null)
                {
                    if (!trail.npc.active)
                    {
                        trail._points.Clear();
                        _trails.RemoveAt(i);
                    }
                }
                if (trail._projectile != null)
                {
                    if (trail._projectile.active)
                        continue;

                    if (trail._projectile.type != ProjectileType<DalantiniumFan>() &&
                        trail._projectile.type != ProjectileType<DalantiniumFanAlt>() &&
                        trail._projectile.type != ProjectileType<DalantiniumSpike>() &&
                        trail._projectile.type != ProjectileType<AxeLightning>() &&
                        trail._projectile.type != ProjectileType<PrismDagger>())
                    {
                        _trails.RemoveAt(i);
                    }
                    if (trail.lerper > 20 && trail._projectile.type == ProjectileType<DalantiniumFan>())
                    {
                        _trails.RemoveAt(i);
                    }
                    if (trail.lerper > 20 && trail._projectile.type == ProjectileType<DalantiniumSpike>())
                    {
                        _trails.RemoveAt(i);
                    }
                    if (trail._projectile.type == ProjectileType<AxeLightning>())
                    {
                        trail.width *= 0.9f;
                        if (trail.width < 0.05f)
                        {
                            trail._points.Clear();
                            _trails.RemoveAt(i);
                        }
                    }
                    if (trail._projectile.type == ProjectileType<PrismDagger>())
                    {
                        trail.width *= 0.9f;
                        if (trail.width < 0.05f)
                        {
                            trail._points.Clear();
                            _trails.RemoveAt(i);
                        }
                    }
                    //if (i >= 0 && i < _trails.Count)
                    {
                        if (trail._projectile.type == ProjectileType<DalantiniumFanAlt>())
                        {
                            if (trail.lerper > 165)
                            {
                                trail._points.Clear();
                                _trails.RemoveAt(i);
                            }
                        }
                    }
                }
            }
        }
        public void CreateVerlet()
        {
            VerletBuffer newTrail = new VerletBuffer();
            _Verlets.Add(newTrail);
        }
        public class VerletBuffer
        {
            bool active = false;
            public void Update()
            {
                if (lerpage >= 1)
                {
                    lerpage = 0;
                }
                lerpage += 0.01f;
            }
            float lerpage;
            public void DrawCape(BasicEffect effect2, GraphicsDevice device)
            {
                Vector2[] pointsArray = Main.LocalPlayer.GetModPlayer<EEPlayer>().arrayPoints;
                if (pointsArray.Length <= 1) return;
                if (!active) return;
                int currentIndex = 0;
                VertexPositionColor[] vertices = new VertexPositionColor[pointsArray.Length * 6 - 9];
                void AddVertex(Vector2 position, Color color)
                {
                    vertices[currentIndex++] = new VertexPositionColor(new Vector3(position.ForDraw(), 0f), color);
                }
                {
                    AddVertex(pointsArray[0], Color.Red);
                    AddVertex(pointsArray[1] + CurveNormal(pointsArray.ToList(), 1) * -5 * (0), Color.DarkRed);
                    AddVertex(pointsArray[1] + CurveNormal(pointsArray.ToList(), 1) * 5 * (0), Color.DarkRed);
                }
                for (int i = 1; i < pointsArray.Length - 1; i++)
                {
                    float j = (pointsArray.Length - i) / (float)pointsArray.Length;
                    float increment = i / (float)pointsArray.Length;
                    Vector2 normal = CurveNormal(pointsArray.ToList(), i);
                    Vector2 normalAhead = CurveNormal(pointsArray.ToList(), i + 1);

                    Vector2 firstUp = pointsArray[i] - normal * 5 * j;
                    Vector2 firstDown = pointsArray[i] + normal * 5 * j;
                    Vector2 secondUp = pointsArray[i + 1] - (normalAhead * 5 * ((pointsArray.Length) - (i + 1)) / pointsArray.Length);
                    Vector2 secondDown = pointsArray[i + 1] + (normalAhead * 5 * ((pointsArray.Length) - (i + 1)) / pointsArray.Length);
                    float varLerp = Math.Abs(lerpage - increment);

                    float varLerpAhead = Math.Abs(lerpage - ((i + 1) / (float)pointsArray.Length));
                    float addon = 0f;
                    Color Base = Color.Red;
                    Color Base2 = Color.DarkRed;
                    Color varColor = new Color(Base.R + (Base2.R - Base.R) * varLerp,
                                               Base.G + (Base2.G - Base.G) * varLerp,
                                               Base.B + (Base2.B - Base.B) * varLerp);
                    Color varColorAhead = new Color(Base.R + (Base2.R - Base.R) * varLerpAhead,
                                                    Base.G + (Base2.G - Base.G) * varLerpAhead,
                                                    Base.B + (Base2.B - Base.B) * varLerpAhead);
                    if (pointsArray[i].Y - pointsArray[i - 1].Y > 3)
                    {
                        if (pointsArray[i].Y > pointsArray[i - 1].Y && pointsArray[i].Y > pointsArray[i + 1].Y)
                        {
                            AddVertex(firstUp, varColorAhead);
                            AddVertex(secondUp, varColorAhead);
                            AddVertex(firstDown, varColorAhead);

                            AddVertex(secondUp, varColorAhead);
                            AddVertex(secondDown, varColorAhead);
                            AddVertex(firstDown, varColorAhead);
                            continue;
                        }
                        if (pointsArray[i].Y > pointsArray[i - 1].Y)
                        {
                            AddVertex(firstUp, Color.DarkRed);
                            AddVertex(secondUp, Color.DarkRed);
                            AddVertex(firstDown, Color.DarkRed);

                            AddVertex(secondUp, Color.DarkRed);
                            AddVertex(secondDown, Color.DarkRed);
                            AddVertex(firstDown, Color.DarkRed);
                        }
                        if (pointsArray[i].Y < pointsArray[i - 1].Y & pointsArray[i].Y < pointsArray[i + 1].Y)
                        {
                            AddVertex(firstUp, varColorAhead);
                            AddVertex(secondUp, varColorAhead);
                            AddVertex(firstDown, varColor);

                            AddVertex(secondUp, varColorAhead);
                            AddVertex(secondDown, varColorAhead);
                            AddVertex(firstDown, varColor);
                            continue;
                        }
                        if (pointsArray[i].Y <= pointsArray[i - 1].Y)
                        {
                            AddVertex(firstUp, Color.DarkRed);
                            AddVertex(secondUp, Color.DarkRed);
                            AddVertex(firstDown, Color.DarkRed);

                            AddVertex(secondUp, Color.DarkRed);
                            AddVertex(secondDown, Color.DarkRed);
                            AddVertex(firstDown, Color.DarkRed);
                        }
                    }
                    else
                    {
                        AddVertex(firstUp, Color.DarkRed);
                        AddVertex(secondUp, Color.DarkRed);
                        AddVertex(firstDown, Color.DarkRed);

                        AddVertex(secondUp, Color.DarkRed);
                        AddVertex(secondDown, Color.DarkRed);
                        AddVertex(firstDown, Color.DarkRed);
                    }

                }
                int width = device.Viewport.Width;
                int height = device.Viewport.Height;
                Vector2 zoom = Main.GameViewMatrix.Zoom;
                Matrix view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(width / 2, height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(zoom.X, zoom.Y, 1f);
                Matrix projection = Matrix.CreateOrthographic(width, height, 0, 1000);
                effect2.View = view;
                effect2.Projection = projection;
                foreach (EffectPass pass in effect2.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, pointsArray.Length * 2 - 3);
                }
            }
            //Helper methods
            private Vector2 CurveNormal(List<Vector2> points, int index)
            {
                if (points.Count == 1) return points[0];

                if (index == 0)
                {
                    return Clockwise90(Vector2.Normalize(points[1] - points[0]));
                }
                return Clockwise90(Vector2.Normalize(points[index] - points[index - 1]));
            }

            private Vector2 Clockwise90(Vector2 vector)
            {
                return new Vector2(-vector.Y, vector.X);
            }
        }
        public delegate void DrawPrimDelegate(int noOfPoints);
        public delegate void UpdatePrimDelegate();
        public static Type[] types => Assembly.GetExecutingAssembly().GetTypes();
        public class Trail
        {
            public void Dispose()
            {

            }
            private ITrailShader _trailShader;
            public Projectile _projectile;
            public NPC npc;
            public List<Vector2> _points = new List<Vector2>();
            public bool active;
            public int lerper;
            float Cap;
            public bool isProjectile;
            public float width;
            List<UpdatePrimDelegate> UpdateMethods = new List<UpdatePrimDelegate>();
            void LythenPrimUpdates()
            {
                if (_projectile.type == ProjectileType<LythenStaffProjectile>())
                {
                    lerper++;
                    LythenStaffProjectile LR = (_projectile.ModProjectile as LythenStaffProjectile);
                    if (LR.positionOfOthers[0] != Vector2.Zero && LR.positionOfOthers[1] != Vector2.Zero)
                    {
                        _points = new List<Vector2>
                          {
                          _projectile.Center,
                          LR.positionOfOthers[0],
                          LR.positionOfOthers[1]
                          };
                    }
                    else
                    {
                        active = false;
                    }
                }
            }
            void DalantiniumPrimUpdates()
            {
                if (_projectile.type == ProjectileType<DalantiniumFan>())
                {
                    Cap = 10;
                    DalantiniumFan DF = (_projectile.ModProjectile as DalantiniumFan);
                    lerper++;
                    _points.Add(DF.DrawPos);
                    active = true;
                    if (_points.Count > 10)
                    {
                        _points.RemoveAt(0);
                    }
                }
            }
            void DalantiniumAltPrimUpdates()
            {
                if (_projectile.type == ProjectileType<DalantiniumFanAlt>())
                {
                    Cap = 20;
                    DalantiniumFanAlt DF = (_projectile.ModProjectile as DalantiniumFanAlt);
                    lerper++;
                    _points.Add(_projectile.Center);
                    active = true;
                    if (_points.Count > Cap)
                    {
                        _points.RemoveAt(0);
                    }
                }
            }
            void AxeLightningPrimUpdates()
            {
                if (_projectile.type == ProjectileType<AxeLightning>())
                {
                    Cap = 80;
                    AxeLightning DF = (_projectile.ModProjectile as AxeLightning);
                    lerper++;
                    _points.Add(_projectile.Center);
                    active = true;
                    width = 1;
                    if (_points.Count > Cap)
                    {
                        _points.RemoveAt(0);
                    }
                }
            }
            void PrismDaggerPrimUpdates()
            {
                if (_projectile.type == ProjectileType<PrismDagger>())
                {
                    Cap = 15;
                    PrismDagger DF = (_projectile.ModProjectile as PrismDagger);
                    lerper++;
                    _points.Add(_projectile.Center);
                    active = true;
                    width = 1;
                    if (_points.Count > Cap)
                    {
                        _points.RemoveAt(0);
                    }
                }
            }
            void DalantiniumSpikePrimUpdates()
            {
                if (_projectile.type == ProjectileType<DalantiniumSpike>())
                {
                    Cap = 20;
                    DalantiniumSpike DF = (_projectile.ModProjectile as DalantiniumSpike);
                    lerper++;
                    _points.Add(_projectile.Center);
                    active = true;
                    if (_points.Count > Cap)
                    {
                        _points.RemoveAt(0);
                    }
                }
            }
            void GliderUpdates()
            {
                _points.Add(Main.LocalPlayer.Center + new Vector2(0, -25));
                active = true;
                Cap = 20;
                lerper++;
                if (_points.Count > Cap)
                {
                    _points.RemoveAt(0);
                }
            }
            void TesterUpdates()
            {
                _points.Add(Main.LocalPlayer.Center);
                active = true;
                Cap = 100;
                lerper++;
                if (_points.Count > Cap)
                {
                    _points.RemoveAt(0);
                }
            }

            void JellyFishUpdates()
            {

            }
            public Trail(ITrailCap cap, ITrailShader shader, Projectile projectile, NPC npc = null)
            {
                _trailShader = shader;
                _projectile = projectile;
                active = true;
                UpdateMethods.Add(LythenPrimUpdates);
                UpdateMethods.Add(DalantiniumPrimUpdates);
                UpdateMethods.Add(DalantiniumAltPrimUpdates);
                UpdateMethods.Add(DalantiniumSpikePrimUpdates);
                UpdateMethods.Add(AxeLightningPrimUpdates);
                UpdateMethods.Add(PrismDaggerPrimUpdates);
                this.npc = npc;
            }
            public void Update()
            {
                if (_projectile == null)
                {
                    //GliderUpdates();
                    TesterUpdates();
                }
                if (_projectile != null)
                {
                    if (_projectile.active)
                    {
                        foreach (UpdatePrimDelegate UPD in UpdateMethods)
                        {
                            UPD.Invoke();
                        }
                    }
                }
                if (npc != null)
                {
                    JellyFishUpdates();
                }
            }
            public void Draw(Effect effect, BasicEffect effect2, GraphicsDevice device)
            {
                //PREPARATION
                if (_points.Count <= 1) return;
                if (!active) return;
                int currentIndex = 0;
                VertexPositionColorTexture[] vertices;
                void AddVertex(Vector2 position, Color color, Vector2 uv)
                {
                    if (currentIndex < vertices.Length)
                        vertices[currentIndex++] = new VertexPositionColorTexture(new Vector3(position.ForDraw(), 0f), color, uv);
                }
                void PrepareShader(Effect effects, float progress = 0)
                {
                    int width = device.Viewport.Width;
                    int height = device.Viewport.Height;
                    Vector2 zoom = Main.GameViewMatrix.Zoom;
                    Matrix view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(width / 2, height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(zoom.X, zoom.Y, 1f);
                    Matrix projection = Matrix.CreateOrthographic(width, height, 0, 1000);
                    effects.Parameters["WorldViewProjection"].SetValue(view * projection);
                    _trailShader.ApplyShader(effects, this, _points, "RainbowLightPass", progress);
                }
                void PrepareBasicShader()
                {
                    int width = device.Viewport.Width;
                    int height = device.Viewport.Height;
                    Vector2 zoom = Main.GameViewMatrix.Zoom;
                    Matrix view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(width / 2, height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(zoom.X, zoom.Y, 1f);
                    Matrix projection = Matrix.CreateOrthographic(width, height, 0, 1000);
                    effect2.View = view;
                    effect2.Projection = projection;
                    foreach (EffectPass pass in effect2.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                    }
                }
                void MakePrimMidFade(int i, int Width, float alphaValue, Color baseColour = default, float fadeValue = 1, float sineFactor = 0)
                {
                    Color c = (baseColour == default ? Color.White : baseColour) * (i / Cap) * fadeValue;
                    Vector2 normal = CurveNormal(_points, i);
                    Vector2 normalAhead = CurveNormal(_points, i + 1);
                    float j = (Cap - (i * 0.9f)) / Cap;
                    float width = (i / Cap) * Width;
                    float width2 = ((i + 1) / Cap) * Width;
                    Vector2 firstUp = _points[i] - normal * width + new Vector2(0, (float)Math.Sin(lerper / 10f + i / 3f)) * sineFactor;
                    Vector2 firstDown = _points[i] + normal * width + new Vector2(0, (float)Math.Sin(lerper / 10f + i / 3f)) * sineFactor;
                    Vector2 secondUp = _points[i + 1] - normalAhead * width2 + new Vector2(0, (float)Math.Sin(lerper / 10f + (i + 1) / 3f)) * sineFactor;
                    Vector2 secondDown = _points[i + 1] + normalAhead * width2 + new Vector2(0, (float)Math.Sin(lerper / 10f + (i + 1) / 3f)) * sineFactor;

                    AddVertex(firstDown, c * alphaValue, new Vector2((i / Cap), 1));
                    AddVertex(firstUp, c * alphaValue, new Vector2((i / Cap), 0));
                    AddVertex(secondDown, c * alphaValue, new Vector2((i + 1) / Cap, 1));

                    AddVertex(secondUp, c * alphaValue, new Vector2((i + 1) / Cap, 0));
                    AddVertex(secondDown, c * alphaValue, new Vector2((i + 1) / Cap, 1));
                    AddVertex(firstUp, c * alphaValue, new Vector2((i / Cap), 0));
                }
                void MakePrimMidFadeEnds(int i, int Width, float alphaValue, Color baseColour = default, float fadeValue = 1, float sineFactor = 0)
                {
                    Color c = (baseColour == default ? Color.White : baseColour) * (i / Cap) * fadeValue;
                    Vector2 normal = CurveNormal(_points, i);
                    Vector2 normalAhead = CurveNormal(_points, i + 1);
                    float fallout = (float)Math.Sin(i * (3.14f / Cap));
                    float fallout1 = (float)Math.Sin((i + 1) * (3.14f / Cap));
                    float lerpers = lerper / 15f;
                    float sine1 = i * (6.14f / _points.Count);
                    float sine2 = (i + 1) * (6.14f / Cap);
                    float width = Width * Math.Abs((float)Math.Sin(sine1 + lerpers) * (i / Cap)) * fallout;
                    float width2 = Width * Math.Abs((float)Math.Sin(sine2 + lerpers) * ((i + 1) / Cap)) * fallout1;
                    Vector2 firstUp = _points[i] - normal * width + new Vector2(0, (float)Math.Sin(lerper / 10f + i / 3f)) * sineFactor;
                    Vector2 firstDown = _points[i] + normal * width + new Vector2(0, (float)Math.Sin(lerper / 10f + i / 3f)) * sineFactor;
                    Vector2 secondUp = _points[i + 1] - normalAhead * width2 + new Vector2(0, (float)Math.Sin(lerper / 10f + (i + 1) / 3f)) * sineFactor;
                    Vector2 secondDown = _points[i + 1] + normalAhead * width2 + new Vector2(0, (float)Math.Sin(lerper / 10f + (i + 1) / 3f)) * sineFactor;

                    AddVertex(firstDown, c * alphaValue, new Vector2((i / Cap), 1));
                    AddVertex(firstUp, c * alphaValue, new Vector2((i / Cap), 0));
                    AddVertex(secondDown, c * alphaValue, new Vector2((i + 1) / Cap, 1));

                    AddVertex(secondUp, c * alphaValue, new Vector2((i + 1) / Cap, 0));
                    AddVertex(secondDown, c * alphaValue, new Vector2((i + 1) / Cap, 1));
                    AddVertex(firstUp, c * alphaValue, new Vector2((i / Cap), 0));
                }
                //PRIM DELEGATES
                DrawPrimDelegate LythenPrims = (int noOfPoints) =>
                {
                    vertices = new VertexPositionColorTexture[noOfPoints];
                    Vector2 leftMostPoint = Vector2.Zero;
                    Vector2 rightMostPoint = Vector2.Zero;
                    Vector2 between = Vector2.Zero;
                    for (int i = 0; i < _points.Count; i++)
                    {
                        if (leftMostPoint == Vector2.Zero || _points[i].X < leftMostPoint.X)
                        {
                            leftMostPoint = _points[i];
                        }
                        if (rightMostPoint == Vector2.Zero || _points[i].X > rightMostPoint.X)
                        {
                            rightMostPoint = _points[i];
                        }
                    }
                    for (int i = 0; i < _points.Count; i++)
                    {
                        if (_points[i].X > leftMostPoint.X && _points[i].X < rightMostPoint.X)
                        {
                            between = _points[i];
                        }
                    }
                    AddVertex(rightMostPoint, Color.LightBlue * (float)Math.Sin(lerper / 20f), new Vector2((float)Math.Sin(lerper / 20f), (float)Math.Sin(lerper / 20f)));
                    AddVertex(between, Color.LightBlue * (float)Math.Sin(lerper / 20f), new Vector2((float)Math.Sin(lerper / 20f), (float)Math.Sin(lerper / 20f)));
                    AddVertex(leftMostPoint, Color.LightBlue * (float)Math.Sin(lerper / 20f), new Vector2((float)Math.Sin(lerper / 20f), (float)Math.Sin(lerper / 20f)));
                    device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, noOfPoints / 3);
                };

                void DrawJelly(int noOfPoints)
                {
                    Jellyfish ja = (npc.ModNPC as Jellyfish);
                    Cap = ja.cap;
                    List<List<List<Vector2>>> tentacle = new List<List<List<Vector2>>>();

                    for (int b = 0; b < 2; b++)
                    {
                        List<List<Vector2>> tempTentA = new List<List<Vector2>>();
                        for (int a = 0; a < ja.noOfTentacles / 2; a++)
                        {
                            List<Vector2> tempTent = new List<Vector2>();
                            for (int i = 0; i < Cap; i++)
                            {
                                tempTent.Add(ja.lol1[a, (int)Cap - i - 1, b]);
                            }
                            tempTentA.Add(tempTent);
                        }
                        tentacle.Add(tempTentA);
                    }

                    List<VertexPositionColorTexture[]> vertices2 = new List<VertexPositionColorTexture[]>();
                    vertices = new VertexPositionColorTexture[noOfPoints];

                    float width = 2;
                    float alphaValue = 0.8f;
                    for (int b = 0; b < tentacle.Count; b++)
                    {
                        for (int a = 0; a < tentacle[b].Count; a++)
                        {
                            for (int i = 1; i < tentacle[b][a].Count - 1; i++)
                            {

                                Color base1 = new Color(7, 86, 122);
                                Color base2 = new Color(255, 244, 173);

                                Color drawColour = Lighting.GetColor((int)npc.Center.X / 16, (int)npc.Center.Y / 16);
                                Color c = Color.Lerp(Color.DarkCyan, base2, i / Cap).MultiplyRGB(drawColour);
                                Color c1 = Color.Lerp(Color.DarkCyan, base2, (i + 1) / Cap).MultiplyRGB(drawColour);

                                Vector2 normal = CurveNormal(tentacle[b][a], i);
                                Vector2 normalAhead = CurveNormal(tentacle[b][a], i + 1);

                                float j = (Cap - (i * 0.9f)) / Cap;
                                width = (i / Cap) * 3;

                                Vector2 firstUp = tentacle[b][a][i] - normal * width;
                                Vector2 firstDown = tentacle[b][a][i] + normal * width;
                                Vector2 secondUp = tentacle[b][a][i + 1] - normalAhead * width;
                                Vector2 secondDown = tentacle[b][a][i + 1] + normalAhead * width;

                                AddVertex(firstDown, c * alphaValue, new Vector2((float)Math.Sin(lerper / 20f), (float)Math.Sin(lerper / 20f)));
                                AddVertex(firstUp, c * alphaValue, new Vector2((float)Math.Sin(lerper / 20f), (float)Math.Sin(lerper / 20f)));
                                AddVertex(secondDown, c1 * alphaValue, new Vector2((float)Math.Sin(lerper / 20f), (float)Math.Sin(lerper / 20f)));

                                AddVertex(secondUp, c1 * alphaValue, new Vector2((float)Math.Sin(lerper / 20f) * j, (float)Math.Sin(lerper / 20f) * j));
                                AddVertex(secondDown, c1 * alphaValue, new Vector2((float)Math.Sin(lerper / 20f) * j, (float)Math.Sin(lerper / 20f) * j));
                                AddVertex(firstUp, c * alphaValue, new Vector2((float)Math.Sin(lerper / 20f) * j, (float)Math.Sin(lerper / 20f) * j));
                            }

                            vertices2.Add(vertices);
                            PrepareBasicShader();
                            device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices2[a], 0, noOfPoints / 3);
                        }
                    }
                }

                DrawPrimDelegate DalantiniumPrims = (int noOfPoints) =>
                {
                    vertices = new VertexPositionColorTexture[noOfPoints];
                    width = 5;
                    float alphaValue = 0.2f;
                    {
                        Color c = Color.Lerp(Color.Red, Color.DarkRed, 0);

                        Vector2 normalAhead = CurveNormal(_points, 1);
                        Vector2 secondUp = _points[1] - normalAhead * width;
                        Vector2 secondDown = _points[1] + normalAhead * width;
                        Vector2 v = new Vector2((float)Math.Sin(lerper / 20f));
                        AddVertex(_points[0], c * alphaValue, v);
                        AddVertex(secondUp, c * alphaValue, v);
                        AddVertex(secondDown, c * alphaValue, v);
                    }
                    for (int i = 1; i < _points.Count - 1; i++)
                    {
                        MakePrimMidFade(i, 5, 1f, Color.Red);
                    }

                    PrepareBasicShader();
                    device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, noOfPoints / 3);
                };

                DrawPrimDelegate GliderPrims = (int noOfPoints) =>
                {
                    if (!Main.LocalPlayer.GetModPlayer<EEPlayer>().isHoldingGlider) return;
                    vertices = new VertexPositionColorTexture[noOfPoints];
                    float width = 5;
                    float alphaValue = 0.8f;
                    for (int i = 0; i < _points.Count; i++)
                    {
                        if (i == 0)
                        {

                        }
                        else
                        {

                            if (i != _points.Count - 1)
                            {
                                MakePrimMidFade(i, 5, 0.8f, default, Math.Abs(Main.LocalPlayer.velocity.X) / 20f, 1);
                            }
                            else
                            {

                            }
                        }
                    }

                    PrepareShader(EEMod.TrailPractice, lerper);
                    device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, noOfPoints / 3);
                };
                DrawPrimDelegate TesterPrims = (int noOfPoints) =>
                {
                    vertices = new VertexPositionColorTexture[noOfPoints];
                    for (int i = 0; i < _points.Count; i++)
                    {
                        if (i == 0)
                        {

                        }
                        else
                        {

                            if (i != _points.Count - 1)
                            {
                                MakePrimMidFadeEnds(i, 20, 0.8f, default, 1f, 2);
                            }
                            else
                            {

                            }
                        }
                    }
                    lerper++;
                    PrepareShader(EEMod.TrailPractice, lerper / 40f);
                    device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, noOfPoints / 3);
                };
                DrawPrimDelegate JellyfishPrims = (int noOfPoints) =>
                {
                };
                DrawPrimDelegate DalantiniumAltPrims = (int noOfPoints) =>
                {
                    vertices = new VertexPositionColorTexture[noOfPoints];
                    float width = 6;
                    float alphaValue = 0.2f;
                    for (int i = 0; i < _points.Count; i++)
                    {
                        if (i == 0)
                        {
                            Color c = Color.Red;
                            Vector2 normalAhead = CurveNormal(_points, i + 1);
                            Vector2 secondUp = _points[i + 1] - normalAhead * width;
                            Vector2 secondDown = _points[i + 1] + normalAhead * width;
                            AddVertex(_points[i], c * alphaValue, new Vector2((float)Math.Sin(lerper / 20f), (float)Math.Sin(lerper / 20f)));
                            AddVertex(secondUp, c * alphaValue, new Vector2((float)Math.Sin(lerper / 20f), (float)Math.Sin(lerper / 20f)));
                            AddVertex(secondDown, c * alphaValue, new Vector2((float)Math.Sin(lerper / 20f), (float)Math.Sin(lerper / 20f)));
                        }
                        else
                        {
                            if (i != _points.Count - 1)
                            {
                                MakePrimMidFade(i, 5, 1f);
                            }
                            else
                            {

                            }
                        }
                    }
                    PrepareBasicShader();
                    device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, noOfPoints / 3);
                };
                DrawPrimDelegate AxeLightningPrims = (int noOfPoints) =>
                {
                    vertices = new VertexPositionColorTexture[noOfPoints];
                    float widthVar;
                    float alphaValue = 0.7f;
                    float colorSin = (float)Math.Sin(_projectile.timeLeft / 10f);

                    for (int i = 0; i < _points.Count; i++)
                    {
                        if (i == 0)
                        {
                            widthVar = (float)Math.Sqrt(_points.Count) * width;
                            Color c1 = Color.Lerp(Color.White, Color.Cyan, colorSin);
                            Vector2 normalAhead = CurveNormal(_points, i + 1);
                            Vector2 secondUp = _points[i + 1] - normalAhead * widthVar;
                            Vector2 secondDown = _points[i + 1] + normalAhead * widthVar;
                            AddVertex(_points[i], c1 * alphaValue, new Vector2((float)Math.Sin(lerper / 20f), (float)Math.Sin(lerper / 20f)));
                            AddVertex(secondUp, c1 * alphaValue, new Vector2((float)Math.Sin(lerper / 20f), (float)Math.Sin(lerper / 20f)));
                            AddVertex(secondDown, c1 * alphaValue, new Vector2((float)Math.Sin(lerper / 20f), (float)Math.Sin(lerper / 20f)));
                        }
                        else
                        {
                            if (i != _points.Count - 1)
                            {
                                widthVar = (float)Math.Sqrt(_points.Count - i) * width;
                                Color base1 = new Color(7, 86, 122);
                                Color base2 = new Color(255, 244, 173);
                                Color c = Color.Lerp(Color.White, Color.Cyan, colorSin) * (1 - (i / (float)_points.Count));
                                Color CBT = Color.Lerp(Color.White, Color.Cyan, colorSin) * (1 - ((i + 1) / (float)_points.Count));
                                Vector2 normal = CurveNormal(_points, i);
                                Vector2 normalAhead = CurveNormal(_points, i + 1);
                                float j = (Cap + ((float)(Math.Sin(lerper / 10f)) * 1) - i * 0.1f) / Cap;
                                widthVar *= j;
                                Vector2 firstUp = _points[i] - normal * widthVar;
                                Vector2 firstDown = _points[i] + normal * widthVar;
                                Vector2 secondUp = _points[i + 1] - normalAhead * widthVar;
                                Vector2 secondDown = _points[i + 1] + normalAhead * widthVar;

                                AddVertex(firstDown, c * alphaValue, new Vector2((i / Cap), 1));
                                AddVertex(firstUp, c * alphaValue, new Vector2((i / Cap), 0));
                                AddVertex(secondDown, CBT * alphaValue, new Vector2((i + 1) / Cap, 1));

                                AddVertex(secondUp, CBT * alphaValue, new Vector2((i + 1) / Cap, 0));
                                AddVertex(secondDown, CBT * alphaValue, new Vector2((i + 1) / Cap, 1));
                                AddVertex(firstUp, c * alphaValue, new Vector2((i / Cap), 0));
                            }
                            else
                            {

                            }
                        }
                    }
                    PrepareShader(EEMod.TrailPractice);
                    device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, noOfPoints / 3);
                };
                DrawPrimDelegate PrismDaggerPrims = (int noOfPoints) =>
                {
                    vertices = new VertexPositionColorTexture[noOfPoints];
                    float widthvar;
                    float alphaValue = 0.7f;
                    for (int i = 0; i < _points.Count; i++)
                    {
                        if (i == 0)
                        {
                            widthvar = (float)Math.Sqrt(i) * width;
                            Color c = Main.hslToRgb((_projectile.ai[0] / 16.96f) + 0.46f, 1f, 0.7f) * (i / (float)_points.Count);
                            Vector2 normalAhead = CurveNormal(_points, i + 1);
                            Vector2 secondUp = _points[i + 1] - normalAhead * widthvar;
                            Vector2 secondDown = _points[i + 1] + normalAhead * widthvar;
                            AddVertex(_points[i], c * alphaValue, new Vector2((float)Math.Sin(lerper / 20f), (float)Math.Sin(lerper / 20f)));
                            AddVertex(secondUp, c * alphaValue, new Vector2((float)Math.Sin(lerper / 20f), (float)Math.Sin(lerper / 20f)));
                            AddVertex(secondDown, c * alphaValue, new Vector2((float)Math.Sin(lerper / 20f), (float)Math.Sin(lerper / 20f)));
                        }
                        else
                        {
                            if (i != _points.Count - 1)
                            {
                                widthvar = (float)Math.Sqrt(i) * width;
                                Color base1 = new Color(7, 86, 122);
                                Color base2 = new Color(255, 244, 173);
                                Color c = Main.hslToRgb((_projectile.ai[0] / 16.96f) + 0.46f, 1f, 0.7f) * ((i * 3) / (float)_points.Count);
                                Color cBT = Main.hslToRgb((_projectile.ai[0] / 16.96f) + 0.46f, 1f, 0.7f) * (((i + 1) * 3) / (float)_points.Count);
                                Vector2 normal = CurveNormal(_points, i);
                                Vector2 normalAhead = CurveNormal(_points, i + 1);
                                float j = (Cap + ((float)(Math.Sin(lerper / 10f)) * 1) - i * 0.1f) / Cap;
                                widthvar *= j;
                                Vector2 firstUp = _points[i] - normal * widthvar;
                                Vector2 firstDown = _points[i] + normal * widthvar;
                                Vector2 secondUp = _points[i + 1] - normalAhead * widthvar;
                                Vector2 secondDown = _points[i + 1] + normalAhead * widthvar;

                                AddVertex(firstUp, c * alphaValue, new Vector2(1));
                                AddVertex(secondDown, cBT * alphaValue, new Vector2(0));
                                AddVertex(firstDown, c * alphaValue, new Vector2(0));


                                AddVertex(secondUp, cBT * alphaValue, new Vector2(1));
                                AddVertex(secondDown, cBT * alphaValue, new Vector2(0));
                                AddVertex(firstUp, c * alphaValue, new Vector2(0));
                            }
                            else
                            {

                            }
                        }
                    }
                    PrepareBasicShader();
                    device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, noOfPoints / 3);
                };
                if (_projectile != null)
                {
                    if (_projectile.type == ProjectileType<LythenStaffProjectile>())
                    {
                        LythenPrims.Invoke(3);
                    }
                    if (_projectile.type == ProjectileType<DalantiniumFan>())
                    {
                        DalantiniumPrims.Invoke((int)Cap * 6 - 9);
                    }
                    if (_projectile.type == ProjectileType<DalantiniumFanAlt>())
                    {
                        DalantiniumAltPrims.Invoke((int)Cap * 6 - 9);
                    }
                    if (_projectile.type == ProjectileType<AxeLightning>())
                    {
                        AxeLightningPrims.Invoke((int)Cap * 6 - 9);
                    }
                    if (_projectile.type == ProjectileType<PrismDagger>())
                    {
                        PrismDaggerPrims.Invoke((int)Cap * 6 - 9);
                    }

                }
                if (npc != null)
                {
                    if (npc.type == NPCType<Jellyfish>())
                    {
                        DrawJelly(((int)Cap * 6 - 12) * (npc.ModNPC as Jellyfish).noOfTentacles);
                    }
                }
                GliderPrims.Invoke((int)Cap * 6 - 12);
                TesterPrims.Invoke((int)Cap * 6 - 12);
            }
            //Helper methods
            private Vector2 CurveNormal(List<Vector2> points, int index)
            {
                if (points.Count == 1) return points[0];

                if (index == 0)
                {
                    return Clockwise90(Vector2.Normalize(points[1] - points[0]));
                }
                if (index == points.Count - 1)
                {
                    return Clockwise90(Vector2.Normalize(points[index] - points[index - 1]));
                }
                return Clockwise90(Vector2.Normalize(points[index + 1] - points[index - 1]));
            }

            private Vector2 Clockwise90(Vector2 vector)
            {
                return new Vector2(-vector.Y, vector.X);
            }
        }

    }
}


