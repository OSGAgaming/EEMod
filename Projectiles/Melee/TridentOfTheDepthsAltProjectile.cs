using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EEMod.Projectiles.Melee
{
    public class TridentOfTheDepthsAltProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Trident of the Depths");
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 32;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.scale = 1f;
            projectile.alpha = 0;
            projectile.ownerHitCheck = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.damage = 20;
            projectile.knockBack = 4.5f;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        private Vector2 InitPos;

        public override void AI()
        {
            projectile.ai[0]++;
            if (projectile.ai[0] == 1)
            {
                InitPos = Main.player[projectile.owner].Center;
                projectile.Center = Main.player[projectile.owner].Center - new Vector2(2000, 0);
            }
            if (Math.Abs(InitPos.X - 100 - projectile.Center.X) > 20)
            {
                projectile.velocity = (InitPos - new Vector2(100, 0) - projectile.Center) / 32f;
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.Pi * 0.75f;
            }
            else
            {
                projectile.position.Y += (float)Math.Sin(projectile.ai[0] / 20f) * 0.5f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            for (int i = 0; i < 4; i++)
            {
                // int num = Dust.NewDust(target.Center, 2, 2, 182, Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f), 6, Color.Red, 1);
                // Main.dust[num].noGravity = false;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * ((projectile.oldPos.Length - k) / (float)projectile.oldPos.Length / 2);
                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, new Rectangle(0, 0, projectile.width, projectile.height), color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
}