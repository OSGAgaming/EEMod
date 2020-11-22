using EEMod.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace EEMod
{
    public class TileObjVisual : Entity, IComponent
    {
        public float scale { get; set; }
        public float alpha;
        public Color colour;
        public float rotation;
        public string TexturePath;
        public Vector2 Origin = Vector2.Zero;
        public SpriteEffects SE = SpriteEffects.None;
        private int RENDERDISTANCE => 2000;
        internal bool WithinDistance => Vector2.DistanceSquared(position, Main.LocalPlayer.position) < RENDERDISTANCE * RENDERDISTANCE;
        public Texture2D Texture => ModContent.GetInstance<EEMod>().GetTexture(TexturePath);

        public TileObjVisual(Vector2 position, string TexturePath, float rotation, Color colour = default)
        {
            this.position = position;
            this.TexturePath = TexturePath;
            this.rotation = rotation;
            scale = 1;
            this.colour = colour == default ? Color.White : colour;
        }

        public virtual void PreAI()
        {
        }

        public virtual bool PreDraw()
        {
            return true;
        }

        public void Update()
        {
            if (WithinDistance)
                PreAI();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (WithinDistance)
            {
                if (PreDraw())
                    spriteBatch.Draw(Texture, position.ForDraw(), Texture.Bounds, colour, rotation, Origin, scale, SE, 0);
            }
        }
    }

    public class Leaf : TileObjVisual
    {
        private float RandRot;
        private float additive;
        private Vector2 CenterBuffer;
        private int timer;
        private bool isFlipped;
        private float gottenVel;
        private int rand;
        private int i => (int)position.X / 16;
        private int j => (int)position.Y / 16;

        public Leaf(Vector2 position, string TexturePath, float rotation, Color colour, bool isFlipped) : base(position, TexturePath, rotation, colour)
        {
            this.position = position;
            this.TexturePath = TexturePath;
            this.rotation = rotation;
            scale = 1;
            this.colour = colour;
            this.isFlipped = isFlipped;
            RandRot = Main.rand.Next(30, 80);
            if (isFlipped)
            {
                SE = SpriteEffects.FlipHorizontally;
            }
        }

        public override void PreAI()
        {
            if (timer > 0)
            {
                timer--;
            }
            if (timer < 0)
            {
                timer++;
            }
            bool DeltaLeft = CenterBuffer.X > position.X && Main.LocalPlayer.Center.X < position.X;
            bool DeltaRight = CenterBuffer.X < position.X && Main.LocalPlayer.Center.X > position.X;
            bool isColliding = new Rectangle((int)Main.LocalPlayer.position.X, (int)Main.LocalPlayer.position.Y, 32, 48).Intersects(new Rectangle((int)position.X, (int)position.Y - Texture.Height, Texture.Width, Texture.Height));
            Texture2D leaft = EEMod.instance.GetTexture("ForegroundParticles/Leaf");
            if (isColliding)
            {
                if (DeltaLeft && timer == 0)
                {
                    for (int g = 0; g < 5; g++)
                    {
                        EEMod.Particles.Get("Main").SetSpawningModules(new SpawnRandomly(0.03f));
                        EEMod.Particles.Get("Main").SpawnParticles(new Vector2(position.X + Main.rand.NextFloat(-1f, 1f), position.Y + 2 + Main.rand.NextFloat(-Texture.Height, 0)), new Vector2(0, Main.rand.NextFloat(-Math.Abs(Main.LocalPlayer.velocity.X) * 2, 0f)), leaft, 20, 1, Lighting.GetColor((int)position.X / 16, (int)position.Y / 16), new RotateVelocity(Main.rand.NextFloat(-0.01f, 0.01f)), new SlowDown(0.9f), new SetFrame(leaft.Bounds), new RotateTexture(Main.rand.NextFloat(-0.03f, 0.03f)), new AddVelocity(new Vector2(0, 0.002f)), new SimpleBrownianMotion(0.2f));
                    }
                    rand = Main.rand.Next(30, 60);
                    gottenVel = Math.Min(1, Math.Abs(Main.LocalPlayer.velocity.X / 25f));
                    timer = rand;
                }
                else if (DeltaRight && timer == 0)
                {
                    for (int g = 0; g < 5; g++)
                    {
                        EEMod.Particles.Get("Main").SetSpawningModules(new SpawnRandomly(0.03f));
                        EEMod.Particles.Get("Main").SpawnParticles(new Vector2(position.X + Main.rand.NextFloat(-1f, 1f), position.Y + 2 + Main.rand.NextFloat(-Texture.Height, 0)), new Vector2(0, Main.rand.NextFloat(-Math.Abs(Main.LocalPlayer.velocity.X) * 2, 0f)), leaft, 20, 1, Lighting.GetColor((int)position.X / 16, (int)position.Y / 16), new RotateVelocity(Main.rand.NextFloat(-0.01f, 0.01f)), new SlowDown(0.9f), new SetFrame(leaft.Bounds), new RotateTexture(Main.rand.NextFloat(-0.03f, 0.03f)), new SimpleBrownianMotion(0.2f));
                    }
                    rand = Main.rand.Next(-60, -30);
                    gottenVel = Math.Min(1, Math.Abs(Main.LocalPlayer.velocity.X / 25f));
                    timer = rand;
                }
            }
            if (timer != 0)
            {
                additive = (float)Math.Sin(timer * 3.14f / Math.Abs(rand)) * -gottenVel;
            }
            else
            {
                additive += -additive / 65f;
            }
            Origin = new Vector2(0, Texture.Height);
            rotation = (float)Math.Sin(Main.GameUpdateCount / RandRot) * 0.2f + additive;
            CenterBuffer = Main.LocalPlayer.Center;
        }

        public override bool PreDraw()
        {
            Main.spriteBatch.Draw(Texture, position.ForDraw(), Texture.Bounds, colour.MultiplyRGB(Lighting.GetColor(i, j)), rotation, Origin, scale, SE, 0);
            return false;
        }
    }
}