using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace EEMod.UI.States
{
    internal class MerchantBoatUI : UIState
    {
        public override void OnInitialize()
        {
            UIPanel panel = new UIPanel();
            panel.Height.Set(50, 0);
            panel.Width.Set(100, 0);
            panel.HAlign = 0.5f;
            panel.VAlign = 0.01f;
            Append(panel);
        }

        public override void OnActivate()
        {
        }

        public override void OnDeactivate()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}