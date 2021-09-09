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
using EEMod.NPCs.CoralReefs;

namespace EEMod.Prim
{
    public partial class PrimTrail
    {
        public interface ITrailShader
        {
            string ShaderPass { get; }
            void ApplyShader<T>(Effect effect, T trail, List<Vector2> positions, string ESP, float progressParam);
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
        protected static Vector2 CurveNormal(List<Vector2> points, int index)
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
        protected static Vector2 Clockwise90(Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }

        public void PrepareShader(Effect effects, string PassName, float progress = 0)
        {
            int width = _device.Viewport.Width;
            int height = _device.Viewport.Height;
            Vector2 zoom = Main.GameViewMatrix.Zoom;
            Matrix view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(width / 2, height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(zoom.X, zoom.Y, 1f);
            Matrix projection = Matrix.CreateOrthographic(width, height, 0, 1000);
            effects.Parameters["WorldViewProjection"].SetValue(view * projection);
            effects.Parameters["noiseTexture"].SetValue(ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("Textures/Noise/noise").Value);
            effects.Parameters["spotTexture"].SetValue(ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("Textures/Noise/Spot").Value);
            effects.Parameters["polkaTexture"].SetValue(ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("Textures/Noise/RandomPolkaDots").Value);
            effects.Parameters["Voronoi"].SetValue(ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("Textures/Noise/VoronoiNoise").Value);
            _trailShader.ApplyShader(effects, this, _points, PassName, progress);
        }

        protected void PrepareShader(Effect effects)
        {
            int width = _device.Viewport.Width;
            int height = _device.Viewport.Height;
            Vector2 zoom = Main.GameViewMatrix.Zoom;
            Matrix view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(width / 2, height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(zoom.X, zoom.Y, 1f);
            Matrix projection = Matrix.CreateOrthographic(width, height, 0, 1000);
            effects.Parameters["WorldViewProjection"].SetValue(view * projection);
            //_trailShader.ApplyShader(effects, this, _points, "MainPS");
        }

        protected void PrepareBasicShader()
        {
            int width = _device.Viewport.Width;
            int height = _device.Viewport.Height;
            Vector2 zoom = Main.GameViewMatrix.Zoom;
            Matrix view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(width / 2, height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(zoom.X, zoom.Y, 1f);
            Matrix projection = Matrix.CreateOrthographic(width, height, 0, 1000);
            _basicEffect.View = view;
            _basicEffect.Projection = projection;
            foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
            }
        }

        protected void AddVertex(Vector2 position, Color color, Vector2 uv)
        {
            if (currentIndex < vertices.Length)
                vertices[currentIndex++] = new VertexPositionColorTexture(new Vector3(position.ForDraw(), 0f), color, uv);
        }

        protected void MakePrimHelix(int i, int Width, float alphaValue, Color baseColour = default, float fadeValue = 1, float sineFactor = 0)
        {
            float _cap = (float)this._cap;
            Color c = (baseColour == default ? Color.White : baseColour) * (i / _cap) * fadeValue;
            Vector2 normal = CurveNormal(_points, i);
            Vector2 normalAhead = CurveNormal(_points, i + 1);
            float fallout = (float)Math.Sin(i * (3.14f / _cap));
            float fallout1 = (float)Math.Sin((i + 1) * (3.14f / _cap));
            float lerpers = _counter / 15f;
            float sine1 = i * (6.14f / _points.Count);
            float sine2 = (i + 1) * (6.14f / _cap);
            float width = Width * Math.Abs((float)Math.Sin(sine1 + lerpers) * (i / _cap)) * fallout;
            float width2 = Width * Math.Abs((float)Math.Sin(sine2 + lerpers) * ((i + 1) / _cap)) * fallout1;
            Vector2 firstUp = _points[i] - normal * width + new Vector2(0, (float)Math.Sin(_counter / 10f + i / 3f)) * sineFactor;
            Vector2 firstDown = _points[i] + normal * width + new Vector2(0, (float)Math.Sin(_counter / 10f + i / 3f)) * sineFactor;
            Vector2 secondUp = _points[i + 1] - normalAhead * width2 + new Vector2(0, (float)Math.Sin(_counter / 10f + (i + 1) / 3f)) * sineFactor;
            Vector2 secondDown = _points[i + 1] + normalAhead * width2 + new Vector2(0, (float)Math.Sin(_counter / 10f + (i + 1) / 3f)) * sineFactor;

            AddVertex(firstDown, c * alphaValue, new Vector2((i / _cap), 1));
            AddVertex(firstUp, c * alphaValue, new Vector2((i / _cap), 0));
            AddVertex(secondDown, c * alphaValue, new Vector2((i + 1) / _cap, 1));

            AddVertex(secondUp, c * alphaValue, new Vector2((i + 1) / _cap, 0));
            AddVertex(secondDown, c * alphaValue, new Vector2((i + 1) / _cap, 1));
            AddVertex(firstUp, c * alphaValue, new Vector2((i / _cap), 0));
        }

        protected void MakePrimMidFade(int i, int Width, float alphaValue, Color baseColour = default, float fadeValue = 1, float sineFactor = 0)
        {
            float _cap = (float)this._cap;
            Color c = (baseColour == default ? Color.White : baseColour) * (i / _cap) * fadeValue;
            Vector2 normal = CurveNormal(_points, i);
            Vector2 normalAhead = CurveNormal(_points, i + 1);
            float width = (i / _cap) * Width;
            float width2 = ((i + 1) / _cap) * Width;
            Vector2 firstUp = _points[i] - normal * width + new Vector2(0, (float)Math.Sin(_counter / 10f + i / 3f)) * sineFactor;
            Vector2 firstDown = _points[i] + normal * width + new Vector2(0, (float)Math.Sin(_counter / 10f + i / 3f)) * sineFactor;
            Vector2 secondUp = _points[i + 1] - normalAhead * width2 + new Vector2(0, (float)Math.Sin(_counter / 10f + (i + 1) / 3f)) * sineFactor;
            Vector2 secondDown = _points[i + 1] + normalAhead * width2 + new Vector2(0, (float)Math.Sin(_counter / 10f + (i + 1) / 3f)) * sineFactor;

            AddVertex(firstDown, c * alphaValue, new Vector2((i / _cap), 1));
            AddVertex(firstUp, c * alphaValue, new Vector2((i / _cap), 0));
            AddVertex(secondDown, c * alphaValue, new Vector2((i + 1) / _cap, 1));

            AddVertex(secondUp, c * alphaValue, new Vector2((i + 1) / _cap, 0));
            AddVertex(secondDown, c * alphaValue, new Vector2((i + 1) / _cap, 1));
            AddVertex(firstUp, c * alphaValue, new Vector2((i / _cap), 0));
        }

        protected void DrawBasicTrail(Color c1, float widthVar)
        {
            //int currentIndex = 0;
            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[_noOfPoints];
            Vector2 normalAhead;
            Vector2 secondUp;
            Vector2 secondDown;

            normalAhead = CurveNormal(_points, 1);
            secondUp = _points[1] - normalAhead * widthVar;
            secondDown = _points[1] + normalAhead * widthVar;

            Vector2 vector = new Vector2((float)Math.Sin(_counter / 20.0));
            AddVertex(_points[0], c1 * _alphaValue, vector);
            AddVertex(secondUp, c1 * _alphaValue, vector);
            AddVertex(secondDown, c1 * _alphaValue, vector);


            float sinCounterOver10 = (float)Math.Sin(_counter / 10f); // _counter doesn't seem to change within the loop so
            for (int i = 1; i < _points.Count - 1; i++)
            {
                Vector2 normal = CurveNormal(_points, i);
                normalAhead = CurveNormal(_points, i + 1);
                float j = (_cap + sinCounterOver10 - i * 0.1f) / _cap;
                widthVar *= j;
                Vector2 firstUp = _points[i] - normal * widthVar;
                Vector2 firstDown = _points[i] + normal * widthVar;
                secondUp = _points[i + 1] - normalAhead * widthVar;
                secondDown = _points[i + 1] + normalAhead * widthVar;

                float p = i / (float)_cap;
                AddVertex(firstDown, c1 * _alphaValue, new Vector2(p, 1));
                AddVertex(firstUp, c1 * _alphaValue, new Vector2(p, 0));
                AddVertex(secondDown, c1 * _alphaValue, new Vector2((i + 1) / _cap, 1));

                AddVertex(secondUp, c1 * _alphaValue, new Vector2((i + 1) / _cap, 0));
                AddVertex(secondDown, c1 * _alphaValue, new Vector2((i + 1) / _cap, 1));
                AddVertex(firstUp, c1 * _alphaValue, new Vector2(p, 0));
            }
            PrepareBasicShader();
            _device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, _noOfPoints / 3);
        }
    }
}
