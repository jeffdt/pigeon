using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using pigeon.gameobject;

namespace pigeon.gfx.drawable.text {
    public class TextRenderer : Component, IRenderable {
        public delegate string TimeBasedUpdater();
        public delegate string EventBasedUpdater(object sender, EventArgs evt);

        private TextGraphic textGraphic;

        private string _text = string.Empty;
        public string Text {
            get { return _text; }
            set {
                _text = value;
                if (textGraphic != null) {
                    textGraphic.Text = _text;
                }
            }
        }

        private Color _color = Color.Black;
        public Color Color {
            get { return _color; }
            set {
                _color = value;
                if (textGraphic != null) {
                    textGraphic.Color = _color;
                }
            }
        }

        public Point Size {
            get { return textGraphic.Size; }
        }

        private SpriteFont _font;
        public SpriteFont Font {
            get { return _font; }
            set {
                _font = value;

                if (textGraphic != null) {
                    textGraphic.Font = _font;
                }
            }
        }

        public Justifications Justification;

        protected override void Initialize() {
            if (_font == null) {
                throw new ArgumentNullException("_font");
            }

            textGraphic = new TextGraphic(_text, _font, _color, Justification);
        }

        protected override void Update() { }

        public void Draw() {
            if (Enabled) {
                textGraphic.Draw(Object.WorldPosition.ToVector2(), Object.DrawLayer);
            }
        }
    }
}
