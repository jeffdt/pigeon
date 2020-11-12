using Microsoft.Xna.Framework;
using pigeon.core;
using pigeon.gameobject;
using pigeon.input;
using Microsoft.Xna.Framework.Input;
using pigeon.gfx.drawable.sprite;
using pigeon;
using pigeon.sound.music;

namespace SampleGame.src.worlds {
    class PadDisplay : World {
        private const int X_OFFSET = 24;
        private int objCount = 1;

        protected override void Load() {
            Pigeon.PauseWhenInactive = false;
            BackgroundColor = Color.Black;

            AddObj(new GameObject("" + objCount++) { Layer = 0f }.AddComponent(new SpriteRenderer(@"controllerNew", "frame")));
            
            addButton(62, 8, "a", Buttons.A);
            addButton(55, 16, "b", Buttons.B);
            addButton(55, 0, "x", Buttons.X);
            addButton(48, 8, "y", Buttons.Y);

            addButton(31, 1, "up", Buttons.DPadUp);
            addButton(37, 9, "right", Buttons.DPadRight);
            addButton(31, 16, "down", Buttons.DPadDown);
            addButton(24, 9, "left", Buttons.DPadLeft);
        }

        private void addButton(int x, int y, string sprite, Buttons button) =>
            AddObj(new GameObject("" + objCount++) { Layer = .5f, WorldPosition = new Point(x - X_OFFSET, y) }
                .AddComponent(new SpriteRenderer(@"controllerNew", sprite))
                .AddComponent(new ButtonDisplay { Button = button })
            );

        protected override void Unload() { }
    }
}

class ButtonDisplay : Component {
    public Buttons Button;

    private SpriteRenderer spriteRenderer;

    protected override void Initialize() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Update() {
        if (RawGamepadInput.IsHeld(Button)) {
            spriteRenderer.Enabled = true;
        } else {
            spriteRenderer.Enabled = false;
        }
    }
}
 