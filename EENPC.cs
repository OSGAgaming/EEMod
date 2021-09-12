using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace EEMod
{
    public abstract class EENPC : ModNPC
    {
        public NPC NPC => base.NPC; // for 1.4 port

        public virtual void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            PostDraw(spriteBatch, Main.screenPosition, drawColor);
        }

        public virtual bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            PreDraw(spriteBatch, Main.screenPosition, lightColor);

            return default;
        }

        public virtual void NPCLoot()
        {
            OnKill();
        }
    }
}
