using EEMod.UI.States;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EEMod
{
    public partial class EEMod : Mod
    {
        public void LoadUI()
        {
            if (!Main.dedServ)
            {
                UI.AddUIState("RunUI", new RunninUI());
                UI.AddUIState("MBUI", new MerchantBoatUI());
                UI.AddUIState("EEUI", new EEUI());
                UI.AddInterface("CustomResources");
                //autobind
                UI.AddInterface("SpeedrunnTimer", "RunUI");
                UI.AddInterface("MerchantBoatUI", "MBUI");
                UI.AddInterface("EEInterface", "EEUI");
            }
        }

        public void UnloadUI()
        {
            UI.UnLoad();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            UI.Update(gameTime);
            lastGameTime = gameTime;
            UIControls();
            base.UpdateUI(gameTime);
        }

        public void UIControls()
        {
            if (RuneActivator.JustPressed && delay == 0)
            {
                UI.SwitchBindedState("EEInterface");
            }
        }
    }
}