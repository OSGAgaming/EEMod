using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using EEMod.Items.Placeables.Ores;

namespace EEMod.Items.Tools.Lythen
{
    public class LythenPickaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lythen Pickaxe");
            Tooltip.SetDefault("I wonder who cares about tooltips");
        }

        public override void SetDefaults()
        {
            item.pick = 42;
            item.useTime = 20;
            item.useAnimation = 20;
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Green;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.value = Item.sellPrice(0, 0, 18);
            item.damage = 8;
            item.melee = true;
            item.autoReuse = true;
            item.UseSound = SoundID.Item1;
            item.knockBack = 2f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<LythenBar>(), 8);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}