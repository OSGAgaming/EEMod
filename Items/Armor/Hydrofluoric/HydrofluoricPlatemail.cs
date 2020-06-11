using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using EEMod.Items.Placeables;
using EEMod.Items.Placeables.Ores;

namespace EEMod.Items.Armor.Hydrofluoric
{
	[AutoloadEquip(EquipType.Body)]
	public class HydrofluoricPlatemail : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Hydrofluoric Platemail");
			Tooltip.SetDefault("4% increased all damage");
		}

		public override void SetDefaults() {
			item.width = 18;
			item.height = 18;
			item.value = Item.sellPrice(0, 0, 30);
			item.rare = ItemRarityID.LightPurple;
			item.defense = 16;
		}

		public override void UpdateEquip(Player player) 
		{
			player.allDamage += 0.04f;
		}

		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<HydroFluoricBar>(), 15);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}