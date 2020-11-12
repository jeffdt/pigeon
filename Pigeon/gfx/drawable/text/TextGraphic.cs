using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using pigeon.gfx;
using pigeon.gfx.drawable.text;
using pigeon.utilities;
using pigeon.utilities.extensions;

namespace pigeon.gfx.drawable.text {
    public class TextGraphic : Graphic {
        private SpriteFont _font;

        public SpriteFont Font {
            get { return _font; }
            set {
                var oldFont = _font;
                _font = value;

                if (oldFont != null) {
                    setSizing();
                }
            }
        }

        private readonly Justifications just;

        private string _text;
        public string Text {
            get { return _text; }
            set {
                _text = value;

                for (int i = 0; i < _text.Length; i++) {
                    var character = _text[i];
                    if (!Font.Characters.Contains(character) && character != '\r' && character != '\n') {
                        throw new ArgumentException(string.Format("Font {0} cannot draw character '{1}' ({2})", Font, character, character.Ascii()));
                    }
                }

                setSizing();
            }
        }

        private void setSizing() {
            Vector2 vector2 = Font.MeasureString(_text);
            Size = vector2.ToPoint();
            justify();
        }

        private void justify() {
            switch (just) {
                case Justifications.Left:
                    Offset = new Point(0, Size.Y / 2);
                    break;
                case Justifications.Center:
                    Offset = Size.Div(2);
                    break;
                case Justifications.Right:
                    Offset = new Point(Size.X, Size.Y / 2);
                    break;
                case Justifications.TopLeft:
                    Offset = Point.Zero;
                    break;
                case Justifications.TopCenter:
                    Offset = new Point(Size.X / 2, 0);
                    break;
                case Justifications.TopRight:
                    Offset = new Point(Size.X, 0);
                    break;
                case Justifications.BottomLeft:
                    Offset = new Point(0, Size.Y);
                    break;
                case Justifications.BottomCenter:
                    Offset = new Point(Size.X / 2, Size.Y);
                    break;
                case Justifications.BottomRight:
                    Offset = new Point(Size.X, Size.Y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public TextGraphic(string txt, SpriteFont font, Color color, Justifications justification) {
            Font = font;
            just = justification;
            Color = color;
            Text = txt;
        }

        public override void Update() { }

        public override void Draw(Vector2 position, float layer) {
            Renderer.SpriteBatch.DrawString(Font, _text, position, Color, Rotation, InternalOffset, Scale, Flip, layer);
        }
    }
}
