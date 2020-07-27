using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using EEMod.Items.Materials.Fruit;

namespace EEMod.Items.Food
{
    public class CoconutMilk : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coconut Milk");
            ItemID.Sets.SortingPriorityMaterials[item.type] = 100;
        }

        public override void SetDefaults()
        {
            item.width = 50;
            item.height = 34;
            item.maxStack = 999;
            item.useAnimation = 12;
            item.useTime = 12;
            item.consumable = true;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Blue;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item2;
        }

        public override bool UseItem(Player player)
        {
            player.AddBuff(BuffID.WellFed, 60 * 300);
            player.AddBuff(BuffID.Ironskin, 60 * 60);
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Coconut>(), 10);
            recipe.AddTile(TileID.Kegs);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}