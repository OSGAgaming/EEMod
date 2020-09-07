using Terraria;
using Terraria.ModLoader;

namespace EEMod.Projectiles.OceanMap
{
    public class FriendlyHallowedCannonball : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hallowed Cannonball");
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.magic = true;
        }

        private int killTimer = 180;
        private bool sinking;

        public override void AI()
        {
            if (!sinking)
            {
                projectile.velocity *= 1.05f;
                projectile.rotation = projectile.velocity.ToRotation();
                killTimer--;
            }
            if (killTimer <= 0)
            {
                Sink();
                sinking = true;
            }
        }

        private int sinkTimer = 32;

        private void Sink()
        {
            projectile.velocity.X = 0;
            projectile.velocity.Y = 0.3f;
            projectile.alpha += 8;
            sinkTimer--;
            if (sinkTimer <= 0)
            {
                projectile.Kill();
            }
        }
    }
}