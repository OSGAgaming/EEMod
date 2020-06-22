﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using EEMod.Projectiles;
using EEMod.Buffs.Buffs;
using Microsoft.Xna.Framework;

namespace EEMod.Items
{
    public class GraniteGauntlets : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Granite Gauntlets");
        }

        public override void SetDefaults()
        {
            item.useTime = 120;
            item.useAnimation = 120;
            item.width = 32;
            item.height = 32;
            item.scale = 1f;
            item.rare = ItemRarityID.Pink;
            item.value = Item.sellPrice(silver: 20);
            item.useStyle = 4;
            
            item.noMelee = true;
            item.noUseGraphic = true;

            item.UseSound = SoundID.Item1;
            item.shoot = ModContent.ProjectileType<GraniteGauntletsShield>();
        }

        public override bool CanUseItem(Player player)
        {
            return !player.HasBuff(ModContent.BuffType<RechargingGauntlets>());
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            base.UseItemHitbox(player, ref hitbox, ref noHitbox);
        }
    }
}