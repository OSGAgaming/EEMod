using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using EEMod.Projectiles.TennisRackets;

namespace EEMod.Items.TennisRackets
{
    public class Terracket : EEItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terracket");
            Tooltip.SetDefault("Better than the toilet, I guess");
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shootSpeed = 1f;
            Item.rare = ItemRarityID.Orange;
            Item.width = 20;
            Item.height = 20;
            // Item.noMelee = false;
            Item.damage = 20;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.value = Item.buyPrice(0, 0, 30, 0);
            Item.autoReuse = true;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item11;
            Item.crit = 1;
            Item.noUseGraphic = true;
        }

        public int yeet;
        private int proj;

        public override void UpdateInventory(Player player)
        {
            yeet = 0;
        }

        public override void HoldItem(Player player)
        {
            if (player.controlUseItem && yeet == 0 && Main.myPlayer == player.whoAmI)
            {
                proj = Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<TerracketProj>(), 0, 0f, player.whoAmI);
                yeet = 1;
                Main.projectile[proj].netUpdate = true;
            }
            if (yeet == 1)
            {
                yeet = 1;
            }
        }
    }
}