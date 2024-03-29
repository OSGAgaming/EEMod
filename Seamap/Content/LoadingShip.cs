﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EEMod.ID;
using ReLogic.Graphics;
using Terraria.Audio;
using Terraria.ID;
using EEMod.Seamap.Content;
using System.Diagnostics;
using ReLogic.Content;
using EEMod.Prim;
using EEMod.Extensions;
using EEMod.Seamap.Core;
using EEMod.Seamap.Content.Cannonballs;
using EEMod.Seamap.Content.Islands;

namespace EEMod.Seamap.Content
{
    public class LoadingShip : SeamapObject
    {
        public float ShipHelthMax = 5;
        public float shipHelth = 5;
        public int cannonDelay = 60;

        public int abilityDelay = 120;

        public Player myPlayer;

        public int invFrames = 20;


        public LoadingShip(Vector2 pos, Vector2 vel, Player player) : base(pos, vel)
        {
            position = pos;

            velocity = vel;

            myPlayer = player;

            width = 192;
            height = 160;

            rot = MathHelper.TwoPi * 2f / 4f;

            texture = ModContent.Request<Texture2D>("EEMod/Seamap/Content/SeamapPlayerShip", AssetRequestMode.ImmediateLoad).Value;

            PrimitiveSystem.primitives.CreateTrail(foamTrail = new FoamTrailLoading(this, Color.Orange, 0.25f, 260));
        }

        public FoamTrailLoading foamTrail;

        public float boatSpeed = 0.01f;

        public float rot;
        public float forwardSpeed;
        public float topSpeed = 2.4f;

        public Vector2 movementVel;

        public override void Update()
        {
            float prevForwardSpeed = movementVel.Length();

            movementVel = Vector2.UnitX.RotatedBy(rot) * forwardSpeed;

            //position += movementVel;

            velocity = (position - oldPosition);

            if (foamTrail != null && !foamTrail.disposing)
            {
                boatTrailVector.X += VectorAbs(movementVel).Length() / 2f;
                boatTrailVector.X += VectorAbs(velocity).Length() / 2f;
            }

            int sign = forwardSpeed < 0 ? -1 : 1;

            forwardSpeed = movementVel.Length() * sign;

            forwardSpeed = (position - oldPosition).Length();

            oldPosition = position;
            oldVelocity = velocity;

            //position += velocity;

            forwardSpeed *= 0.999f;
            velocity *= 0.96f;

            if (Main.mouseLeft && Hitbox.Contains(Main.MouseScreen.ToPoint()))
            {
                Center = Main.MouseScreen;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D playerShipTexture = ModContent.Request<Texture2D>("EEMod/Seamap/Content/SeamapPlayerShip").Value;

            int frame = 0;
            float spriteRot = 0f;
            bool flipped = false;

            if(position != oldPosition)
                rot = (position - oldPosition).ToRotation();

            rot = TwoPiRestrict(rot);

            float rotForSprite = TwoPiRestrict(rot + MathHelper.PiOver2);
            float rotAbsed = Math.Abs(rotForSprite - MathHelper.Pi);

            int origY = 86;

            if (rotForSprite > MathHelper.Pi && rotAbsed > (MathHelper.Pi / 9f) && rotAbsed < (8f * MathHelper.Pi / 9f)) flipped = true;

            if(rotAbsed < MathHelper.Pi / 9f)
            {
                frame = 8;
                spriteRot = (DynamicClamp(rotForSprite, MathHelper.Pi / 4.5f) - (MathHelper.Pi / 9f));

                origY = 70;
            }
            else if(rotAbsed < 2 * MathHelper.Pi / 9f)
            {
                frame = 7;
                spriteRot = DynamicClamp(rotForSprite, MathHelper.Pi / 9f) - (MathHelper.Pi / 18f);

                origY = 76;
            }
            else if(rotAbsed < 3 * MathHelper.Pi / 9f)
            {
                frame = 6;
                spriteRot = DynamicClamp(rotForSprite, MathHelper.Pi / 9f) - (MathHelper.Pi / 18f);

                origY = 80;
            }
            else if(rotAbsed < 4 * MathHelper.Pi / 9f)
            {
                frame = 5;
                spriteRot = DynamicClamp(rotForSprite, MathHelper.Pi / 9f) - (MathHelper.Pi / 18f);

                origY = 82;
            }
            else if(rotAbsed < 5 * MathHelper.Pi / 9f)
            {
                frame = 4;
                spriteRot = DynamicClamp(rotForSprite, MathHelper.Pi / 9f) - (MathHelper.Pi / 18f);

                origY = 86;
            }
            else if(rotAbsed < 6 * MathHelper.Pi / 9f)
            {
                frame = 3;
                spriteRot = DynamicClamp(rotForSprite, MathHelper.Pi / 9f) - (MathHelper.Pi / 18f);
            }
            else if(rotAbsed < 7 * MathHelper.Pi / 9f)
            {
                frame = 2;
                spriteRot = DynamicClamp(rotForSprite, MathHelper.Pi / 9f) - (MathHelper.Pi / 18f);
            }
            else if (rotAbsed < 8 * MathHelper.Pi / 9f)
            {
                frame = 1;
                spriteRot = DynamicClamp(rotForSprite, MathHelper.Pi / 9f) - (MathHelper.Pi / 18f);
            }
            else
            {
                frame = 0;
                spriteRot = (DynamicClamp(rotAbsed, MathHelper.Pi / 4.5f) - (MathHelper.Pi / 9f)) * (rotForSprite > MathHelper.Pi ? 1f : -1f);
            }


            int yVal = 160 * frame;

            spriteBatch.Draw(playerShipTexture, Center,
                new Rectangle(0, yVal, 192, 160),
                Color.White, spriteRot, 
                new Vector2(96, origY),
                1, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            return false;
        }


        public float TwoPiRestrict(float val)
        {
            while (val > MathHelper.TwoPi)
                val -= MathHelper.TwoPi;

            while (val < 0)
                val += MathHelper.TwoPi;

            return val;
        }

        public float DynamicClamp(float val, float clamper)
        {
            while (val > clamper)
                val -= clamper;

            while (val < 0)
                val += clamper;

            return val;
        }

        public Vector2 boatTrailVector;

        public Vector2 VectorAbs(Vector2 toAbs)
        {
            return new Vector2(Math.Abs(toAbs.X), Math.Abs(toAbs.Y));
        }
    }

    public class FoamTrailLoading : Primitive
    {
        public FoamTrailLoading(Entity projectile, Color _color, float width = 40, int cap = 10) : base(projectile)
        {
            BindableEntity = projectile;
            _width = width;
            color = _color;
            _cap = cap;

            velocities = new List<float>();

            PrimitiveSystem.primitives.CreateTrail(trailLeft = new WakeTrailLoading(this, BindableEntity, new Color(74, 189, 255), 2, _width, 1000, true));
            PrimitiveSystem.primitives.CreateTrail(trailRight = new WakeTrailLoading(this, BindableEntity, new Color(74, 189, 255), 2, _width, 1000, false));
        }

        private Color color;

        public WakeTrailLoading trailLeft;
        public WakeTrailLoading trailRight;

        public List<float> velocities;

        public bool disposing = false;

        public override void SetDefaults()
        {
            Alpha = 0.8f;

            behindTiles = false;
            pixelated = true;
            manualDraw = true;
        }

        public float lastDisposalSpeed;

        public override void PrimStructure(SpriteBatch spriteBatch)
        {
            if ((_noOfPoints <= 1 || _points.Count() <= 1) && _counter > 0) return;
            float widthVar;

            for (int i = 0; i < 5; i++)
            {
                velocities.Insert(velocities.Count - 1, (BindableEntity as LoadingShip).movementVel.Length());

                _points.Insert(_points.Count - 1, BindableEntity.Center + new Vector2(0, 14) + new Vector2(21f * (float)Math.Cos((BindableEntity as LoadingShip).rot), 24f * (float)Math.Sin((BindableEntity as LoadingShip).rot)));
            }

            float colorSin = (float)Math.Sin(_counter / 3f);
            {
                widthVar = 0;

                Vector2 normalAhead = CurveNormal(_points, 1);
                Vector2 secondUp = _points[1] - normalAhead * widthVar;
                Vector2 secondDown = _points[1] + normalAhead * widthVar;

                normalAhead.X *= 5f / 3f;

                AddVertex(Main.screenPosition + _points[0], Color.Lerp(Color.Black, Color.White, 1 / (float)(_points.Count() - 1)), new Vector2(0, 1));
                AddVertex(Main.screenPosition + secondUp, Color.Lerp(Color.Black, Color.White, 1 / (float)(_points.Count() - 1)), new Vector2(0, 0));
                AddVertex(Main.screenPosition + secondDown, Color.Lerp(Color.Black, Color.White, 1 / (float)(_points.Count() - 1)), new Vector2(0, 1));
            }

            for (int i = 1; i < _points.Count() - 1; i++)
            {
                widthVar = (_points.Count() - 1 - i) * _width * (velocities[i] / (BindableEntity as LoadingShip).topSpeed)
                    + ((float)Math.Sin(_points[i].X / 5f - _points[i].Y / 5f) - (float)Math.Cos(_points[i].Y / 5f - _points[i].X / 5f));

                widthVar = MathHelper.Clamp(widthVar, 0, 25);

                Vector2 normal = CurveNormal(_points, i);
                Vector2 normalAhead = CurveNormal(_points, i + 1);

                normal.X *= 4f / 3f;
                normalAhead.X *= 4f / 3f;

                Vector2 firstUp = _points[i] - normal * (widthVar + 1f * (float)Math.Sin((i / 1f) + (_counter / 10f)));
                Vector2 firstDown = _points[i] + normal * (widthVar + 1f * (float)Math.Sin((i / 1f) + (_counter / 10f)));
                Vector2 secondUp = _points[i + 1] - normalAhead * (widthVar + 1f * (float)Math.Sin((i / 1f) + (_counter / 10f)));
                Vector2 secondDown = _points[i + 1] + normalAhead * (widthVar + 1f * (float)Math.Sin((i / 1f) + (_counter / 10f)));

                AddVertex(Main.screenPosition + firstDown, Color.Lerp(Color.Lerp(Color.Black, Color.White, i / (float)(_points.Count() - 1)), Color.Black, 1 - disposingFloat), new Vector2((i / (float)(_points.Count())) % 1, 1));
                AddVertex(Main.screenPosition + firstUp, Color.Lerp(Color.Lerp(Color.Black, Color.White, i / (float)(_points.Count() - 1)), Color.Black, 1 - disposingFloat), new Vector2((i / (float)(_points.Count())) % 1, 0));
                AddVertex(Main.screenPosition + secondDown, Color.Lerp(Color.Lerp(Color.Black, Color.White, i / (float)(_points.Count() - 1)), Color.Black, 1 - disposingFloat), new Vector2(((i + 1) / (float)(_points.Count())) % 1, 1));

                AddVertex(Main.screenPosition + secondUp, Color.Lerp(Color.Lerp(Color.Black, Color.White, i / (float)(_points.Count() - 1)), Color.Black, 1 - disposingFloat), new Vector2(((i + 1) / (float)(_points.Count())) % 1, 0));
                AddVertex(Main.screenPosition + secondDown, Color.Lerp(Color.Lerp(Color.Black, Color.White, i / (float)(_points.Count() - 1)), Color.Black, 1 - disposingFloat), new Vector2(((i + 1) / (float)(_points.Count())) % 1, 1));
                AddVertex(Main.screenPosition + firstUp, Color.Lerp(Color.Lerp(Color.Black, Color.White, i / (float)(_points.Count() - 1)), Color.Black, 1 - disposingFloat), new Vector2((i / (float)(_points.Count())) % 1, 0));
            }

            Main.NewText("updating 3");

            for (int i = 0; i < 5; i++)
            {
                velocities.RemoveAt(velocities.Count - 1);

                _points.RemoveAt(_points.Count - 1);
            }

            disposingFloat = 1f;
        }

        public float disposingFloat = 1f;

        public override void SetShaders()
        {
            Matrix view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(_device.Viewport.Width / 2, _device.Viewport.Height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi);

            Matrix projection = Matrix.CreateOrthographic(_device.Viewport.Width, _device.Viewport.Height, 0, 1000);

            Main.spriteBatch.End(); Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, default, default, EEMod.SeafoamShader);

            EEMod.SeafoamShader.Parameters["foamTexture"].SetValue(ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("Textures/Noise/FoamTrail3").Value);
            EEMod.SeafoamShader.Parameters["rippleTexture"].SetValue(ModContent.GetInstance<EEMod>().Assets.Request<Texture2D>("Textures/Noise/FoamTrail3").Value);

            EEMod.SeafoamShader.Parameters["offset"].SetValue(new Vector2((BindableEntity as LoadingShip).boatTrailVector.X + (BindableEntity as LoadingShip).boatTrailVector.Y, 0) / 400f);

            EEMod.SeafoamShader.Parameters["noColor"].SetValue(new Color(58, 110, 172).ToVector4() * 0f * disposingFloat);
            EEMod.SeafoamShader.Parameters["color1"].SetValue(new Color(98, 153, 217).ToVector4() * 0.15f * disposingFloat);
            EEMod.SeafoamShader.Parameters["color2"].SetValue(new Color(72, 159, 199).ToVector4() * 0.29f * disposingFloat);
            EEMod.SeafoamShader.Parameters["color3"].SetValue(new Color(65, 198, 224).ToVector4() * 0.35f * disposingFloat);
            EEMod.SeafoamShader.Parameters["color4"].SetValue(new Color(108, 211, 235).ToVector4() * 0.55f * disposingFloat);
            EEMod.SeafoamShader.Parameters["color5"].SetValue(new Color(250, 255, 224).ToVector4() * 0.85f * disposingFloat);

            EEMod.SeafoamShader.Parameters["WorldViewProjection"].SetValue(view * projection);

            if (vertices.Length == 0) return;

            DynamicVertexBuffer buffer = VertexBufferPool.Shared.RentDynamicVertexBuffer(VertexPositionColorTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            buffer.SetData(vertices);

            Main.graphics.GraphicsDevice.SetVertexBuffer(buffer);

            foreach (EffectPass pass in EEMod.SeafoamShader.CurrentTechnique.Passes)
            {
                pass.Apply();
            }

            if (_noOfPoints >= 1)
            {
                _device.DrawPrimitives(PrimitiveType.TriangleList, 0, _noOfPoints / 3);
            }

            VertexBufferPool.Shared.Return(buffer);
        }

        public override void OnUpdate()
        {
            _counter++;
            _noOfPoints = _points.Count() * 6;

            if (_cap < _noOfPoints / 6)
            {
                _points.RemoveAt(0);

                velocities.RemoveAt(0);
            }

            _points.Add(BindableEntity.Center + new Vector2(0, 14) + new Vector2(21f * (float)Math.Cos((BindableEntity as LoadingShip).rot), 24f * (float)Math.Sin((BindableEntity as LoadingShip).rot)));

            velocities.Add((BindableEntity as LoadingShip).movementVel.Length());
        }

        public override void OnDestroy()
        {

        }

        public override void PostDraw()
        {
            Main.spriteBatch.End(); Main.spriteBatch.Begin();
        }
    }

    public class WakeTrailLoading : Primitive
    {
        public WakeTrailLoading(FoamTrailLoading trail, Entity projectile, Color _color, int width = 40, float myWidth = 10, int cap = 10, bool _left = false) : base(projectile)
        {
            myTrail = trail;
            _width = width;
            _myWidth = myWidth;
            color = _color;
            _cap = cap;
            left = _left;
        }

        private Color color;

        public FoamTrailLoading myTrail;

        public float _myWidth;

        public bool left;

        public float counter;

        public override void SetDefaults()
        {
            Alpha = 0.8f;

            behindTiles = false;
            pixelated = true;
            manualDraw = true;
        }

        public override void PrimStructure(SpriteBatch spriteBatch)
        {
            if (myTrail._points.Count() <= 1) return;

            _myWidth = myTrail._width + 0.1f;

            _points.Clear();

            if (left)
            {
                for (int i = 1; i < myTrail._points.Count() - 1; i++)
                {
                    Vector2 normal = CurveNormal(myTrail._points, i);

                    normal.X *= 4f / 3f;

                    _points.Add(myTrail._points[i] - normal * ((myTrail._points.Count() - 1 - i) * _myWidth * (myTrail.velocities[i] / (BindableEntity as LoadingShip).topSpeed) + (1f * (float)Math.Sin((i) + (counter / 10f)))));
                }
            }
            else
            {
                for (int i = 1; i < myTrail._points.Count() - 1; i++)
                {
                    Vector2 normal = CurveNormal(myTrail._points, i);

                    normal.X *= 4f / 3f;

                    _points.Add(myTrail._points[i] + normal * ((myTrail._points.Count() - 1 - i) * _myWidth * (myTrail.velocities[i] / (BindableEntity as LoadingShip).topSpeed) + (1f * (float)Math.Sin((i) + (counter / 10f)))));
                }
            }

            if (_points.Count() <= 1) return;

            float widthVar;

            {
                widthVar = 0;

                Vector2 normalAhead = CurveNormal(_points, 1);
                Vector2 secondUp = _points[1] - normalAhead * widthVar;
                Vector2 secondDown = _points[1] + normalAhead * widthVar;
                Vector2 v = new Vector2((float)Math.Sin((_counter) / 20f));

                AddVertex(Main.screenPosition + _points[0], color.LightSeamap() * Alpha, v);
                AddVertex(Main.screenPosition + secondUp, color.LightSeamap() * Alpha, v);
                AddVertex(Main.screenPosition + secondDown, color.LightSeamap() * Alpha, v);
            }

            for (int i = 1; i < _points.Count - 1; i++)
            {
                Alpha = (i / (float)(_points.Count - 1)) * 0.5f;
                widthVar = ((i) / (float)_points.Count) * _width * MathHelper.Clamp(myTrail.velocities[i], 0f, 1f);

                Vector2 normal = CurveNormal(_points, i);
                Vector2 normalAhead = CurveNormal(_points, i + 1);

                float j = (_cap - ((float)(Math.Sin((_counter + i) / 3f)) * 1) - i * 0.1f) / _cap;
                widthVar *= j;

                Vector2 firstUp = _points[i] - normal * widthVar;
                Vector2 firstDown = _points[i] + normal * widthVar;
                Vector2 secondUp = _points[i + 1] - normalAhead * widthVar;
                Vector2 secondDown = _points[i + 1] + normalAhead * widthVar;

                AddVertex(Main.screenPosition + firstDown, color.LightSeamap() * Alpha * myTrail.disposingFloat, new Vector2(((i + (_counter / BindableEntity.velocity.Length())) / (float)_cap) % 1, 1));
                AddVertex(Main.screenPosition + firstUp, color.LightSeamap() * Alpha * myTrail.disposingFloat, new Vector2(((i + (_counter / BindableEntity.velocity.Length())) / (float)_cap) % 1, 0));
                AddVertex(Main.screenPosition + secondDown, color.LightSeamap() * Alpha * myTrail.disposingFloat, new Vector2((((i + (_counter / BindableEntity.velocity.Length())) + 1) / (float)_cap) % 1, 1));

                AddVertex(Main.screenPosition + secondUp, color.LightSeamap() * Alpha * myTrail.disposingFloat, new Vector2((((i + (_counter / BindableEntity.velocity.Length())) + 1) / (float)_cap) % 1, 0));
                AddVertex(Main.screenPosition + secondDown, color.LightSeamap() * Alpha * myTrail.disposingFloat, new Vector2((((i + (_counter / BindableEntity.velocity.Length())) + 1) / (float)_cap) % 1, 1));
                AddVertex(Main.screenPosition + firstUp, color.LightSeamap() * Alpha * myTrail.disposingFloat, new Vector2((((i + (_counter / BindableEntity.velocity.Length())) / (float)_cap)) % 1, 0));
            }
        }

        public override void SetShaders()
        {
            Matrix view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(_device.Viewport.Width / 2, _device.Viewport.Height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi);

            Matrix projection = Matrix.CreateOrthographic(_device.Viewport.Width, _device.Viewport.Height, 0, 1000);

            Main.spriteBatch.End(); Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, default, default, EEMod.BasicEffect);

            EEMod.BasicEffect.Projection = view * projection;

            if (vertices.Length == 0) return;

            DynamicVertexBuffer buffer = VertexBufferPool.Shared.RentDynamicVertexBuffer(VertexPositionColorTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            buffer.SetData(vertices);

            Main.graphics.GraphicsDevice.SetVertexBuffer(buffer);

            foreach (EffectPass pass in EEMod.BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
            }

            if (_noOfPoints >= 1)
            {
                _device.DrawPrimitives(PrimitiveType.TriangleList, 0, _noOfPoints / 3);
            }

            VertexBufferPool.Shared.Return(buffer);
        }

        public override void OnUpdate()
        {
            counter += BindableEntity.velocity.Length();
            _counter++;
            _noOfPoints = _points.Count() * 6;
            if (_cap < _noOfPoints / 6)
            {
                _points.RemoveAt(0);
            }
            if ((!BindableEntity.active && BindableEntity != null) || _destroyed)
            {
                Dispose();
            }
        }

        public override void OnDestroy()
        {
            _destroyed = true;
            _width *= 0.9f;
            if (_width < 0.05f)
            {
                Dispose();
            }
        }

        public override void PostDraw()
        {
            Main.spriteBatch.End(); Main.spriteBatch.Begin();
        }
    }
}
