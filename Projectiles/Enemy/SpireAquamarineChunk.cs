using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using EEMod.Prim;
using EEMod.Tiles;
using EEMod.NPCs.CoralReefs;
using Microsoft.Xna.Framework.Graphics;
using EEMod.Extensions;

namespace EEMod.Projectiles.Enemy
{
    public class SpireAquamarineChunk : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aquamarine Chunk");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 36;
            projectile.timeLeft = 30000;
            projectile.ignoreWater = true;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.penetrate = -1;
            projectile.extraUpdates = 12;
            projectile.tileCollide = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.Kill();
            return true;
        }

        public override void AI()
        {
            projectile.velocity.Y *= 1.003f;
            projectile.rotation += projectile.velocity.Y / 128f;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {

        }
    }
}