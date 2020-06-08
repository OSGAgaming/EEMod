using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using System.Reflection;
using Terraria.Map;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria.DataStructures;
using Terraria.GameContent.UI;
using System;

namespace EEMod.Items.Weapons.Melee
{
    public class DuneSlicer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dune Slicer");
            Tooltip.SetDefault("Shoots a homing shot every few swings");
        }
        private int keeper;
        public override void SetDefaults()
        {
            item.damage = 22;
            item.melee = true;
            item.width = 68;
            item.height = 80;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 1;
            item.knockBack = 5;
            item.value = 10000;
            item.rare = 3;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.crit = 6;
            item.shoot = mod.ProjectileType("CrystalSword");
        }
        public override void AddRecipes()
        {
            {
                ModRecipe recipe = new ModRecipe(mod);
                recipe.AddIngredient(ItemID.HardenedSand, 5);
                recipe.AddIngredient(ItemID.Sandstone, 8);
                recipe.AddIngredient(ItemID.Diamond, 1);
                recipe.AddIngredient(mod.ItemType("MummifiedRag"), 2);
                recipe.AddTile(TileID.Anvils);
                recipe.SetResult(this);
                recipe.AddRecipe();
            }
        }
        public override bool UseItem(Player player)
        {
            return base.UseItem(player);
        }
        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            keeper++;
            float speed = 7;
            float distX = Main.mouseX + Main.screenPosition.X - player.Center.X;
            float distY = Main.mouseY + Main.screenPosition.Y - player.Center.Y;
            float mag = (float)Math.Sqrt(distX * distX + distY * distY);
            if (keeper % 3 == 0)
                Projectile.NewProjectile(position.X, position.Y, distX*speed/mag, distY*speed/mag, mod.ProjectileType("CrystalSword"), damage, knockBack, player.whoAmI, 0f, 0f);

            return false;
        }
    }
}
