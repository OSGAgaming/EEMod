using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace EEMod.Projectiles.Mage
{
    public class ExtrusiveStaffProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("ExtrusiveStaffProjectile");
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.magic = true;
        }

        public override void AI()
        {
            if (Main.rand.Next(3) == 0) // only spawn 33% of the time
            {
                // Spawn the dust
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 33, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 150, default, 0.7f);
            }
        }
    }
}