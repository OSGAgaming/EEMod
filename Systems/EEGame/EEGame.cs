using EEMod.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace EEMod.Systems.EEGame
{
    public class EEGame
    {
        public GameElement[] elementArray = new GameElement[200];
        public virtual Vector2 sizeOfMainCanvas => Vector2.Zero;
        public virtual Vector2 centerOfMainCanvas => Main.LocalPlayer.Center;
        public virtual Color colourOfMainCanvas => Color.White;
        public virtual float speedOfStartUp => 16f;
        public virtual Texture2D tex => Terraria.GameContent.TextureAssets.MagicPixel.Value;
        public Vector2 TopLeft => centerOfMainCanvas + new Vector2(-sizeOfMainCanvas.X / 2, -sizeOfMainCanvas.Y / 2);
        public Vector2 TopRight => centerOfMainCanvas + new Vector2(sizeOfMainCanvas.X / 2, -sizeOfMainCanvas.Y / 2);
        public Vector2 BottomLeft => centerOfMainCanvas + new Vector2(-sizeOfMainCanvas.X / 2, sizeOfMainCanvas.Y / 2);
        public Vector2 BottomRight => centerOfMainCanvas + new Vector2(sizeOfMainCanvas.X / 2, sizeOfMainCanvas.Y / 2);

        public float colourOfStartUp = 0;
        public bool gameActive;

        public EEGame()
        {
            Initialize();
        }

        public virtual void Initialize()
        {
        }

        public virtual void OnDeactivate()
        {
        }

        public virtual void StartGame(int host)
        {
            gameActive = true;
            this.host = host;
        }

        public int host;

        public virtual void EndGame() => gameActive = false;

        public virtual int AddUIElement(Vector2 size, Color color, Vector2 Center)
        {
            for (int i = 0; i < elementArray.Length; i++)
            {
                if (elementArray[i] == null)
                {
                    elementArray[i] = new GameElement(size, color, Center);
                    return i;
                    //break;
                }
            }
            return 0;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (gameActive)
            {
                Main.spriteBatch.Draw(tex, centerOfMainCanvas.ForDraw(), new Rectangle(0, 0, (int)sizeOfMainCanvas.X, (int)sizeOfMainCanvas.Y), colourOfMainCanvas * colourOfStartUp, 0f, new Rectangle(0, 0, (int)sizeOfMainCanvas.X, (int)sizeOfMainCanvas.Y).Size() / 2, 1, SpriteEffects.None, 0f);
                colourOfStartUp += (1 - colourOfStartUp) / speedOfStartUp;
                int d = 0;
                for (int i = 0; i < elementArray.Length; i++)
                {
                    GameElement GE = elementArray[i];
                    if (GE != null)
                    {
                        GE.Update(gameTime);
                        if (GE.colourOfStartUp == 0 && GE.elementActive == false)
                        {
                            elementArray[i] = null;
                        }
                    }
                }
            }
            else
            {
                OnDeactivate();
                Main.spriteBatch.Draw(tex, centerOfMainCanvas.ForDraw(), new Rectangle(0, 0, (int)sizeOfMainCanvas.X, (int)sizeOfMainCanvas.Y), colourOfMainCanvas * colourOfStartUp, 0f, new Rectangle(0, 0, (int)sizeOfMainCanvas.X, (int)sizeOfMainCanvas.Y).Size() / 2, 1, SpriteEffects.None, 0f);
                colourOfStartUp += (-colourOfStartUp) / speedOfStartUp;
            }
        }
    }
}