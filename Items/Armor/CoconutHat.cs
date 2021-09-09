using EEMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EEMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class CoconutHat : EEItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coconut Hat");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.sellPrice(0, 0, 30);
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<Coconut>(), 10).AddTile(TileID.Anvils).Register();
        }
    }
}