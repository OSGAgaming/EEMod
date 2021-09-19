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
    class SceptorPrimTrail : PrimTrail
    {
        public SceptorPrimTrail(Projectile projectile) : base(projectile)
        {
            _projectile = projectile;
        }
        public override void SetDefaults()
        {
            _alphaValue = 0.7f;
            _width = 1;
            _cap = 80;
        }
        public override void PrimStructure(SpriteBatch spriteBatch)
        {
            /*if (_noOfPoints <= 1) return; //for easier, but less customizable, drawing
            float colorSin = (float)Math.Sin(_counter / 3f);
            Color c1 = Color.Lerp(Color.White, Color.Cyan, colorSin);
            float widthVar = (float)Math.Sqrt(_points.Count) * _width;
            DrawBasicTrail(c1, widthVar);*/

            if (_noOfPoints <= 6) return;
            float widthVar;
            float colorSin = (float)Math.Sin(_counter / 3f);
            {
                widthVar = (float)Math.Sqrt(_points.Count) * _width;
                Color c1 = Color.Lerp(Color.White, Color.Cyan, colorSin);
                Vector2 normalAhead = CurveNormal(_points, 1);
                Vector2 secondUp = _points[1] - normalAhead * widthVar;
                Vector2 secondDown = _points[1] + normalAhead * widthVar;
                AddVertex(_points[0], c1 * _alphaValue, new Vector2((float)Math.Sin(_counter / 20f), (float)Math.Sin(_counter / 20f)));
                AddVertex(secondUp, c1 * _alphaValue, new Vector2((float)Math.Sin(_counter / 20f), (float)Math.Sin(_counter / 20f)));
                AddVertex(secondDown, c1 * _alphaValue, new Vector2((float)Math.Sin(_counter / 20f), (float)Math.Sin(_counter / 20f)));
            }
            for (int i = 1; i < _points.Count - 1; i++)
            {
                widthVar = (float)Math.Sqrt(_points.Count - i) * _width;
                Color base1 = new Color(7, 86, 122);
                Color base2 = new Color(255, 244, 173);
                Color c = Color.Lerp(Color.White, Color.Cyan, colorSin);
                Color CBT = Color.Lerp(Color.White, Color.Cyan, colorSin);
                Vector2 normal = CurveNormal(_points, i);
                Vector2 normalAhead = CurveNormal(_points, i + 1);
                float j = (_cap + ((float)(Math.Sin(_counter / 10f)) * 1) - i * 0.1f) / _cap;
                widthVar *= j;
                Vector2 firstUp = _points[i] - normal * widthVar;
                Vector2 firstDown = _points[i] + normal * widthVar;
                Vector2 secondUp = _points[i + 1] - normalAhead * widthVar;
                Vector2 secondDown = _points[i + 1] + normalAhead * widthVar;

                AddVertex(firstDown, c * _alphaValue, new Vector2((i / _cap), 1));
                AddVertex(firstUp, c * _alphaValue, new Vector2((i / _cap), 0));
                AddVertex(secondDown, CBT * _alphaValue, new Vector2((i + 1) / _cap, 1));

                AddVertex(secondUp, CBT * _alphaValue, new Vector2((i + 1) / _cap, 0));
                AddVertex(secondDown, CBT * _alphaValue, new Vector2((i + 1) / _cap, 1));
                AddVertex(firstUp, c * _alphaValue, new Vector2((i / _cap), 0));
            }
        }
        public override void SetShaders()
        {
            PrepareBasicShader();
        }
        public override void OnUpdate()
        {
            _counter++;
            _noOfPoints = _points.Count() * 6;
            if (_cap < _noOfPoints / 6)
            {
                _points.RemoveAt(0);
            }
            if ((!_projectile.active && _projectile != null) || _destroyed)
            {
                OnDestroy();
            }
            else
            {
                _points.Add(_projectile.Center);
            }
        }
        public override void OnDestroy()
        {
            _destroyed = true;
            _width *= 0.8f;
            if (_width < 0.05f)
            {
                Dispose();
            }
        }
    }
}