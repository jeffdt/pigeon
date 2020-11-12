using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using pigeon.collision;
using pigeon.console;
using pigeon.core;
using pigeon.core.events;
using pigeon.data;
using pigeon.debugger;
using pigeon.gfx;
using pigeon.input;
using pigeon.sound;
using pigeon.time;
using System;
using pigeon.sound.music;
using PigeonEngine.logger;
using System.IO;
using pigeon.gfx.drawable.sprite;
using Serilog.Core;
using Serilog;

namespace pigeon {
    public abstract class Pigeon : Game {
        public static Pigeon Instance;

        // set these for each new game
        public abstract DisplayParams DisplayParams { get; }
        protected abstract string WindowTitle { get; }
        protected abstract string Version { get; }
        protected abstract int FrameRate { get; }
        protected abstract ConsoleOptions ConsoleOptions { get; }
        protected abstract bool StartMouseVisible { get; }
        protected abstract Color DefaultBkgdColor { get; }
        protected abstract string SaveFolderName { get; }
        protected abstract TextureTemplateProcessor TemplateProcessor { get; }
        protected abstract World InitialWorld { get; }
        protected abstract void LoadGame();
        protected abstract void InitializeGame();

        public static Logger Logger;
        public static PGNConsole Console;
        public static Renderer Renderer;
        public static readonly EventRegistry GameEventRegistry = new EventRegistry();
        public static readonly EventRegistry EngineEventRegistry = new EventRegistry();
        public static readonly Camera Camera = new Camera();
        public static ContentManager ContentManager;
        public static World World { get; private set; }
        public static bool IsInFocus;

        public static InputManager InputManager = new InputManager();

        internal static Color EngineBkgdColor;

        public static bool PauseWorld;
        internal static World nextWorld = null;
        private static bool isNextWorldAlreadyInitialized;
        private static World lastWorld;

        private bool _pauseWhenInactive = false;
        public static bool PauseWhenInactive {
            get { return Instance._pauseWhenInactive; }
            set { Instance._pauseWhenInactive = value; }
        }

        public static bool MouseVisible { set { Instance.IsMouseVisible = value; } }

        protected Pigeon() {
            ContentManager = Content;
            ContentManager.RootDirectory = @"Content";

            Renderer.GraphicsDeviceMgr = new GraphicsDeviceManager(this);

            Instance = this;

            PlayerData.SaveFolderName = SaveFolderName;
        }

        protected sealed override void LoadContent() {
            PlayerData.Initialize();

            Logger = new LoggerConfiguration()
                .WriteTo.PGNConsole()
                .WriteTo.File(Path.Combine(PlayerData.UserDataPath, "log.txt"), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Renderer = new Renderer(DisplayParams.ScreenWidth, DisplayParams.ScreenHeight, DisplayParams.InitialScale);

            Renderer.GraphicsDeviceMgr.SynchronizeWithVerticalRetrace = false; //Vsync
            IsFixedTimeStep = true;

            TargetElapsedTime = TimeSpan.FromSeconds(1 / (float) FrameRate);
            Instance.Window.Title = WindowTitle;

            ResourceCache.Initialize(TemplateProcessor);

            GameData.Initialize();

            Renderer.SpriteBatch = new SpriteBatch(GraphicsDevice);

            loadResources();

            Sfx.Initialize();
            Music.Initialize();

            LoadGame();

            Console = new PGNConsole(ConsoleOptions);
            Console.LoadContent();
            Console.AddGlobalCommands(EngineCommands.Build());

            EngineBkgdColor = DefaultBkgdColor;

            World = InitialWorld;
            World.LoadContent();

            Logger.Information("Starting {WindowTitle}", WindowTitle, TargetElapsedTime);
        }

        private static void loadResources() {
            string[] texturePaths = GameData.GetContentFiles(@"textures");
            var templates = GameData.Read(@"textures\templates.cfg");
            ResourceCache.LoadTextures(texturePaths, templates);

            string[] sfxPaths = GameData.GetContentFiles(@"sfx");
            ResourceCache.LoadSounds(sfxPaths);

            Font[] fonts = GameData.Deserialize<Font[]>(@"fonts\fonts.xml");
            if (fonts != null) {
                ResourceCache.LoadFonts(fonts);
            }

            HitboxSetManager.Load(@"data\hitboxes.dat");
            Sprite.LoadCompact(@"textures\compactAnimations.cfg");
        }

        protected override void Initialize() {
            base.Initialize();

            MouseReader.Initialize();
            RawKeyboardInput.Initialize();
            RawGamepadInput.Initialize();
            GameSpeed.Reinitialize();
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            DebugHelper.Initialize();

            MouseVisible = StartMouseVisible;

            InitializeGame();
        }

        protected override void UnloadContent() {
            World.UnloadContent();
        }

        protected override void Update(GameTime gameTime) {
            if (nextWorld != null && World != nextWorld) {
                swapWorld();
            }

            IsInFocus = IsActive || !_pauseWhenInactive;

            InputManager.Update();
            RawKeyboardInput.Update();
            RawGamepadInput.Update();
            MouseReader.Update();

            Time.Set(gameTime);

            base.Update(gameTime);

            Console.Update();

            if (!Console.IsDisplaying && !PauseWorld) {
                UpdateGameplay();
            }

            Renderer.Update();
        }

        public void UpdateGameplay() {
            World.Update();
            Sfx.Update();
            Music.Update();
        }

        public static void SetWorld(World world, bool isAlreadyInitialized = false) {
            if (nextWorld != null) {
                Console.Error("NextWorld has already been set.");
                Console.Error("Cannot override " + nextWorld + " with " + world);
            }

            nextWorld = world;
            isNextWorldAlreadyInitialized = isAlreadyInitialized;
        }

        private static void swapWorld() {
            if (!isNextWorldAlreadyInitialized) {
                Camera.Position = Vector2.Zero;
                World.UnloadContent();
                Console.ResetWorldCommands();
            }

            lastWorld = World;
            World = nextWorld;
            nextWorld = null;

            if (!isNextWorldAlreadyInitialized) {
                GameEventRegistry.Reset();
                World.LoadContent();
            }

            Sfx.StopAllSfx();
            World.Enter();
        }

        protected override void Draw(GameTime gameTime) {
            base.Draw(gameTime);

            if (lastWorld != null) {
                lastWorld.Draw();
                lastWorld = null;
            } else {
                World.Draw();
            }

            if (Console.IsDisplaying) { Console.Draw(); }

            Renderer.FinalDraw();
        }

        public static void Reset() {
            SetWorld(Instance.InitialWorld);
        }

        public void ExitGame() {
            Exit();
        }
    }
}
