﻿using System;
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
    public abstract class EEProjectile : ModProjectile
    {
        public Projectile Projectile => base.Projectile; // for 1.4 port

        public virtual void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            PostDraw(drawColor);
        }

        public virtual bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            PreDraw(ref lightColor);

            return default;
        }

        //vector8.X + DisX, vector8.Y - 1200, (float)(Math.Cos(rotation) * Speed * -1), (float)(Math.Sin(rotation) * Speed * -1), type, damage, 0f, 0
        public void NewProjectile(Vector2 position, Vector2 velocity, int type, int damage, float knockBack, float ai0, float ai1)
        {
            //Projectile.NewProjectile()
        }
    }
}
