using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EEMod.Autoloading;
using EEMod.Extensions;

namespace EEMod.Seamap.SeamapContent
{
    internal static class SeamapObjects
    {
        [FieldInit] public static List<SeaEntity> SeaObject = new List<SeaEntity>(); //List 1
        [FieldInit] public static List<int> SeaObjectFrames = new List<int>();
        [FieldInit] public static Dictionary<string, SeaEntity> Islands = new Dictionary<string, SeaEntity>(); //List 3
        [FieldInit] internal static List<ISeamapEntity> OceanMapElements = new List<ISeamapEntity>(); //List 4
    }

    public struct SeaEntity
    {
        EEPlayer player => Main.LocalPlayer.GetModPlayer<EEPlayer>();
        public SeaEntity(Vector2 pos, Texture2D tex, string NameOfIsland, int frameCount = 1, int frameSpid = 2, bool canCollide = false, int startingFrame = 0)
        {
            posX = (int)pos.X;
            posY = (int)pos.Y;
            texture = tex;
            frames = frameCount;
            frameSpeed = frameSpid;
            currentFrame = startingFrame;
            this.canCollide = canCollide;
            if (NameOfIsland != null)
            {
                if (!SeamapObjects.Islands.ContainsKey(NameOfIsland))
                {
                    SeamapObjects.Islands.Add(NameOfIsland, this);
                }
            }
        }

        private readonly int posX;
        private readonly int posY;
        public int frames;
        public int currentFrame;
        public int frameSpeed;
        public bool canCollide;

        public int posXToScreen
        {
            get => posX + (int)Main.screenPosition.X + Main.screenWidth;
        }

        public int posYToScreen
        {
            get => posY + (int)Main.screenPosition.Y + Main.screenHeight;
        }

        public Texture2D texture;
        public Vector2 posToScreen => new Vector2(posXToScreen - texture.Width / 2, posYToScreen - texture.Height / (2 * frames));
        public Rectangle hitBox => new Rectangle((int)posToScreen.X, (int)posToScreen.Y - texture.Height / (frames * 2), texture.Width, texture.Height / (frames));
        private Rectangle ShipHitBox => new Rectangle((int)Main.screenPosition.X + (int)SeamapPlayerShip.localship.position.X - 30, (int)Main.screenPosition.Y + (int)SeamapPlayerShip.localship.position.Y - 30, 30, 30);
        public bool isColliding => hitBox.Intersects(ShipHitBox) && canCollide;

        public void Update()
        {

        }
    }

    public class DarkCloud : ISeamapEntity
    {
        public Vector2 pos;
        public float flash;

        public int posXToScreen
        {
            get => (int)(pos.X + Main.screenPosition.X);
        }

        public int posYToScreen
        {
            get => (int)(pos.Y + Main.screenPosition.Y);
        }

        public Texture2D texture;
        public float scale, alpha;
        private readonly EEPlayer modPlayer = Main.LocalPlayer.GetModPlayer<EEPlayer>();

        public DarkCloud(Vector2 pos, Texture2D tex, float scale, float alpha)
        {
            flash += 0.01f;
            this.pos = pos;
            texture = tex;
            this.scale = scale;
            this.alpha = alpha;
            flash = Main.rand.NextFloat(0, 4);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            flash += 0.003f;
            Vector2 p = new Vector2(posXToScreen + (float)Math.Sin(flash) * 10, posYToScreen - 1000).ForDraw();
            Color drawcolor = Lighting.GetColor(posXToScreen / 16, (posYToScreen - 1000) / 16) * modPlayer.seamapLightColor;
            drawcolor.A = (byte)alpha;
            if (modPlayer.quickOpeningFloat > 0.01f)
            {
                float lerp = 1 - (modPlayer.quickOpeningFloat / 10f);
                spriteBatch.Draw(texture, p, null, drawcolor * lerp, 0f, default, scale, SpriteEffects.None, 0f);
                return;
            }
            spriteBatch.Draw(texture, p, null, drawcolor * (1 - (modPlayer.cutSceneTriggerTimer / 180f)), 0f, default, scale, SpriteEffects.None, 0f);
        }

        public void Update()
        {

        }
    }

    public class MovingCloud : ISeamapEntity
    {
        public Vector2 pos;
        public float flash;

        public int posXToScreen
        {
            get => (int)(pos.X + Main.screenPosition.X);
        }

        public int posYToScreen
        {
            get => (int)(pos.Y + Main.screenPosition.Y);
        }

        public Texture2D texture;
        public float scale, alpha;
        private readonly EEPlayer modPlayer = Main.LocalPlayer.GetModPlayer<EEPlayer>();

        public MovingCloud(Vector2 pos, Texture2D tex, float scale, float alpha)
        {
            this.pos = pos;
            texture = tex;
            this.scale = scale;
            this.alpha = alpha;
            flash = Main.rand.NextFloat(0, 4);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            flash += 0.003f;
            Vector2 p = new Vector2(posXToScreen + (float)Math.Sin(flash) * 10, posYToScreen - 1000).ForDraw();
            Color drawcolor = Lighting.GetColor(posXToScreen / 16, (posYToScreen - 1000) / 16) * modPlayer.brightness * (modPlayer.isStorming ? 2 / 3f : 1);
            drawcolor.A = (byte)alpha;
            spriteBatch.Draw(texture, p, null, drawcolor * (1 - (modPlayer.cutSceneTriggerTimer / 180f)), 0f, default, scale, SpriteEffects.None, 0f);
        }

        public void Update()
        {

        }
    }

    public class MCloud : ISeamapEntity
    {
        private Vector2 position;
        private readonly int width, height;
        private readonly float alpha, scale;
        private readonly Texture2D texture;
        private readonly EEPlayer modPlayer = Main.LocalPlayer.GetModPlayer<EEPlayer>();
        private Vector2 Center => new Vector2(position.X + width / 2f, position.Y + height / 2f);

        public MCloud(Texture2D texture, Vector2 position, int width, int height, float scale, float alpha)
        {
            //scale = projectile.ai[0];
            //alpha = (int)projectile.ai[1];
            this.scale = scale;
            this.alpha = alpha;
            this.texture = texture;
            this.position = position;
            this.width = width;
            this.height = height;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 newPos = Center + Main.screenPosition;
            Rectangle rect = new Rectangle(0, 0, width, height);
            Color lightColour = Lighting.GetColor((int)newPos.X / 16, (int)newPos.Y / 16) * modPlayer.brightness * (modPlayer.isStorming ? 2 / 3f : 1);
            spriteBatch.Draw(texture, Center, rect, lightColour * (alpha / 255f) * (1 - (modPlayer.cutSceneTriggerTimer / 180f)), 0f, rect.Size() / 2, scale, SpriteEffects.None, 0f);
        }

        public void Update()
        {
            position.X -= 0.3f;
        }
    }

    internal interface ISeamapEntity
    {
        void Update();

        void Draw(SpriteBatch spriteBatch);
    }
}
