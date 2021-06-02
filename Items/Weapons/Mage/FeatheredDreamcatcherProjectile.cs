using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EEMod.Items.Weapons.Mage
{
    public class FeatheredDreamcatcherProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dreamcatcher Feather");
        }

        public override void SetDefaults()
        {
            projectile.width = 42;
            projectile.height = 18;
            projectile.tileCollide = false;
            projectile.timeLeft = 120;
            projectile.rotation = (float)(Math.PI / 2);
            projectile.penetrate = 1;
            projectile.hostile = false;
            projectile.friendly = true;
        }

        private int dropTimer = 10;
        private bool firstFrame = true;

        public override void AI()
        {
            if (firstFrame)
            {
                Vector2 closestNPCPos = Vector2.Zero;
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    if (Vector2.DistanceSquared(Main.LocalPlayer.Center, Main.npc[i].Center) <= Vector2.DistanceSquared(Main.LocalPlayer.Center, closestNPCPos) && Main.npc[i].active)
                    {
                        closestNPCPos = Main.npc[i].Center;
                    }
                }
                if (closestNPCPos == Vector2.Zero || Main.npc.Length == 0)
                {
                    projectile.Kill();
                }
                projectile.Center = new Vector2(closestNPCPos.X, closestNPCPos.Y - 80);
                firstFrame = false;
            }
            Dust.NewDust(projectile.Center, 0, 0, 127);
            if (dropTimer > 0)
            {
                dropTimer--;
            }
            else
            {
                projectile.velocity.Y = 32;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180);
            KillVisible();
        }

        private void KillVisible()
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(projectile.Center, 0, 0, 127);
            }
            projectile.Kill();
        }

        public override void Kill(int timeLeft)
        {
        }
    }
}