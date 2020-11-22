﻿using EEMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

//TODO:
//-Shimmer on blade itself
//-Proper right click motion
//-Proper hovering on daggers
//-Dagger dust
//-Sound effects
namespace EEMod.Items.Weapons.Melee
{
    public class PrismaticBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prismatic Blade");
        }

        public override void SetDefaults()
        {
            item.melee = true;
            item.rare = ItemRarityID.Green;
            item.autoReuse = false;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 7f; // 5 and 1/4
            item.useTime = 25;
            item.useAnimation = 25;
            item.value = Item.buyPrice(0, 0, 30, 0);
            item.damage = 50;
            item.width = 30;
            item.height = 30;
            item.UseSound = SoundID.Item1;
        }

        private int swordsActive = 0;
        private int[] swordArray = new int[9];

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                for (int i = 0; i < swordsActive; i++)
                {
                    if (Main.projectile[swordArray[i]].active)
                    {
                        Main.projectile[swordArray[i]].friendly = true;
                        if (Main.netMode != NetmodeID.Server)
                        {
                            EEMod.prims.CreateTrail(Main.projectile[swordArray[i]]);
                        }
                    }
                    swordArray[i] = 0;
                }
                swordsActive = 0;
            }
            return base.UseItem(player);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            int damage2 = damage;
            if (damage2 > target.lifeMax)
            {
                damage2 = target.lifeMax;
            }
            if (target.life <= 0 && swordsActive < 9)
            {
                float angle = (float)(swordsActive * 0.7f);
                swordArray[swordsActive] = Projectile.NewProjectile(target.position, Vector2.Zero, ModContent.ProjectileType<PrismDagger>(), damage2, 0, player.whoAmI, angle);
                swordsActive++;
            }
        }
    }
}