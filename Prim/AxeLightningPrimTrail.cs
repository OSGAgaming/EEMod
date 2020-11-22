﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;

namespace EEMod.Prim
{
    internal class AxeLightningPrimTrail : PrimTrail
    {
        public AxeLightningPrimTrail(Projectile projectile)
        {
            _projectile = projectile;
            _points.Add(_projectile.position);
        }

        public override void SetDefaults()
        {
            _alphaValue = 0.7f;
            _width = 1;
            _cap = 80;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_noOfPoints <= 1) return;
            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[_noOfPoints];
            float widthVar;
            float colorSin = (float)Math.Sin(_projectile.timeLeft / 10f);
            int currentIndex = 0;
            for (int i = 0; i < _points.Count; i++)
            {
                if (i == 0)
                {
                    widthVar = (float)Math.Sqrt(_points.Count) * _width;
                    Color c1 = Color.Lerp(Color.White, Color.Cyan, colorSin);
                    Vector2 normalAhead = CurveNormal(_points, i + 1);
                    Vector2 secondUp = _points[i + 1] - normalAhead * widthVar;
                    Vector2 secondDown = _points[i + 1] + normalAhead * widthVar;
                    AddVertex(_points[i], c1 * _alphaValue, new Vector2((float)Math.Sin(_counter / 20f), (float)Math.Sin(_counter / 20f)), ref currentIndex, ref vertices);
                    AddVertex(secondUp, c1 * _alphaValue, new Vector2((float)Math.Sin(_counter / 20f), (float)Math.Sin(_counter / 20f)), ref currentIndex, ref vertices);
                    AddVertex(secondDown, c1 * _alphaValue, new Vector2((float)Math.Sin(_counter / 20f), (float)Math.Sin(_counter / 20f)), ref currentIndex, ref vertices);
                }
                else
                {
                    if (i != _points.Count - 1)
                    {
                        widthVar = (float)Math.Sqrt(_points.Count - i) * _width;
                        Color base1 = new Color(7, 86, 122);
                        Color base2 = new Color(255, 244, 173);
                        Color c = Color.Lerp(Color.White, Color.Cyan, colorSin) * (1 - (i / (float)_points.Count));
                        Color CBT = Color.Lerp(Color.White, Color.Cyan, colorSin) * (1 - ((i + 1) / (float)_points.Count));
                        Vector2 normal = CurveNormal(_points, i);
                        Vector2 normalAhead = CurveNormal(_points, i + 1);
                        float j = (_cap + ((float)(Math.Sin(_counter / 10f)) * 1) - i * 0.1f) / _cap;
                        widthVar *= j;
                        Vector2 firstUp = _points[i] - normal * widthVar;
                        Vector2 firstDown = _points[i] + normal * widthVar;
                        Vector2 secondUp = _points[i + 1] - normalAhead * widthVar;
                        Vector2 secondDown = _points[i + 1] + normalAhead * widthVar;

                        AddVertex(firstDown, c * _alphaValue, new Vector2((i / _cap), 1), ref currentIndex, ref vertices);
                        AddVertex(firstUp, c * _alphaValue, new Vector2((i / _cap), 0), ref currentIndex, ref vertices);
                        AddVertex(secondDown, CBT * _alphaValue, new Vector2((i + 1) / _cap, 1), ref currentIndex, ref vertices);

                        AddVertex(secondUp, CBT * _alphaValue, new Vector2((i + 1) / _cap, 0), ref currentIndex, ref vertices);
                        AddVertex(secondDown, CBT * _alphaValue, new Vector2((i + 1) / _cap, 1), ref currentIndex, ref vertices);
                        AddVertex(firstUp, c * _alphaValue, new Vector2((i / _cap), 0), ref currentIndex, ref vertices);
                    }
                    else
                    {
                    }
                }
            }
            PrepareBasicShader();
            _device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, _noOfPoints / 3);
        }

        public override void OnUpdate()
        {
            _counter++;
            _noOfPoints = _points.Count() * 6;
            if (_cap < _noOfPoints)
            {
                _points.RemoveAt(0);
            }
            if ((!_projectile.active && _projectile != null))
            {
                OnDestroy();
            }
            else
            {
                _points.Add(_projectile.Center);
            }
            base.Update();
        }

        public override void OnDestroy()
        {
            _width *= 0.9f;
            if (_width < 0.05f)
            {
                Dispose();
            }
        }
    }
}