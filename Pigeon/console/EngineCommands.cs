using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using pigeon.core;
using pigeon.data;
using pigeon.debugger;
using pigeon.gfx;
using pigeon.input;
using pigeon.sound;
using pigeon.gameobject;
using pigeon.time;
using pigeon.utilities.extensions;
// using pigeon.winforms;
using pigeon.sound.music;
using System.IO;
using pigeon.gfx.drawable.text;
using pigeon.gfx.drawable.image;
using pigeon.gfx.drawable.sprite;

namespace pigeon.console {
    public static class EngineCommands {
        public static Dictionary<string, ConsoleCommand> Build() {
            return new Dictionary<string, ConsoleCommand> {
				// console manipulation
				{ "clear", clearScreen },
                { "help", getCommands },
                { "repeat", repeat },

				// binds + aliases
				{ "alias", setAlias },
                { "unbind", setUnbind},
                { "bind", setBind },
                { "findkey", findKeyboardKeyName },

				// audio
                { "sfxvol", setSfxVolume },
                { "bgmplay", bgmPlayTrack },
                { "bgmpause", bgmPause },
                { "bgmresume", bgmResume },
                { "bgmstop", bgmStop },
                { "bgmvol", setBgmVolume },
                { "bgmtempo", bgmTempo },
                { "bgmequalizer", setBgmEqualizer },
                { "bgmmutevoice", setBgmMuteVoice },
                { "bgmmutevoices", setBgmMuteVoices },
                { "bgmstereo", bgmStereoDepth },
                { "bgmfade", setBgmFade },

				// toggled debug info
				{ "togglecd", toggleCd },
                { "togglehitbox", toggleHitbox },
                { "togglemouse", toggleMouse},
                { "toggleposition", togglePosition },
                { "toggleysort", toggleYSort },

				// engine
				{ "exit", exitApp },
                { "gamespeed", setGameSpeed},
                { "reset", reset },
                { "savedir", openSaveDir },
                { "showui", showUi },
                { "tick", tick },

				// objects
				{ "components", getComponents },
                { "bump", bump },
                { "hide", setHideDrawable },
                { "inspect", inspect },
                { "objects", getAllObjects },

				// graphics
				{ "fullscreen", toggleFullscreen },
                { "borderless", toggleBorderless},
                { "lcd", toggleLcd },
                { "scale", setScale },
                { "screenshot", takeScreenshot },

				// world
				{ "pauseworld", togglePauseWorld },
                { "sandbox", createSandboxWorld },

				// vars
				{ "vload", loadVarPreset },
                { "var", setVar },
                { "vsave", saveVarPreset },
                { "vreset", resetVar },
            };
        }

        #region console manipulation
        private static void clearScreen(string args) {
            for (int i = 0; i < Pigeon.Console.options.LogHistoryLimit; i++) {
                Pigeon.Console.Log("");
            }
        }

        private static void getCommands(string args) {
            if (string.IsNullOrWhiteSpace(args)) {
                Pigeon.Console.Log("Available engine commands:");
                Pigeon.Console.Log(ConsoleUtilities.BracketedList(Pigeon.Console.EngineCommandNames));

                if (Pigeon.Console.GameCommandNames.Count > 0) {
                    Pigeon.Console.Log("Available game commands:");
                    Pigeon.Console.Log(ConsoleUtilities.BracketedList(Pigeon.Console.GameCommandNames));
                }
            } else {
                var matches = Pigeon.Console.AllCommandNames.FindStringMatches(args);

                if (matches.Count == 0) {
                    Pigeon.Console.Log(string.Format("no commands match for '{0}'", args));
                } else {
                    Pigeon.Console.Log(string.Format("commands matching '{0}':", args));
                    Pigeon.Console.Log(ConsoleUtilities.BracketedList(matches));
                }
            }
        }

        private static void repeat(string args) {
            var count = string.IsNullOrWhiteSpace(args) ? 1 : args.ToFloat();

            for (int i = 0; i < count; i++) {
                Pigeon.Console.RepeatPrevious();
            }
        }
        #endregion

        #region graphics
        private static void toggleLcd(string args) {
            bool before = Renderer.LcdDisplay;
            bool after = string.IsNullOrEmpty(args) ? !Renderer.LcdDisplay : args.Tokenize()[0].ToBool();

            Renderer.LcdDisplay = after;

            ConsoleUtilities.LogVariableChange("LCD effect", before, after);
        }

        private static void toggleFullscreen(string args) {
            bool before = Pigeon.Renderer.IsFullScreen;
            bool after;

            if (string.IsNullOrEmpty(args)) {
                after = !Pigeon.Renderer.IsFullScreen;
            } else {
                after = args.ToBool();
            }

            if (before != after) {
                Pigeon.Renderer.IsFullScreen = after;
            }

            Pigeon.Renderer.IsFullScreen = after;

            ConsoleUtilities.LogVariableChange("fullscreen", before, after);
        }

        private static void toggleBorderless(string args) {
            bool before = Pigeon.Renderer.IsBorderless;
            bool after;

            if (string.IsNullOrEmpty(args)) {
                after = !Pigeon.Renderer.IsBorderless;
            } else {
                after = args.ToBool();
            }

            if (before != after) {
                Pigeon.Renderer.IsBorderless = after;
            }

            ConsoleUtilities.LogVariableChange("borderless", before, after);
        }

        private static void setScale(string args) {
            if (string.IsNullOrEmpty(args)) {
                Pigeon.Console.Log("drawscale: " + Pigeon.Renderer.DrawScale);
                return;
            }
            int after = args.ToInt();

            if (after < 1 || after > 7) {
                Pigeon.Console.Error("Scale must be between 1 and 7");
            } else {
                int before = Pigeon.Renderer.DrawScale;
                Pigeon.Renderer.DrawScale = after;
                ConsoleUtilities.LogVariableChange("drawscale", before, after);
            }
        }

        private static void takeScreenshot(string args) {
            Pigeon.Renderer.Screenshot();
        }
        #endregion

        #region engine
        private static void exitApp(string args) {
            Pigeon.Instance.Exit();
        }

        private static void setGameSpeed(string args) {
            if (string.IsNullOrEmpty(args)) {
                ConsoleUtilities.LogVariable("gamespeed", GameSpeed.Multiplier);
            } else {
                var oldValue = GameSpeed.Multiplier;
                var newValue = args.ToFloat();
                GameSpeed.Multiplier = newValue;
                ConsoleUtilities.LogVariableChange("gamespeed", oldValue, newValue);
            }
        }

        private static void reset(string args) {
            Pigeon.Reset();
        }

        private static void tick(string args) {
            if (!Pigeon.PauseWorld) {
                Pigeon.PauseWorld = true;
            } else {
                Pigeon.Instance.UpdateGameplay();
            }
        }

        private static void showUi(string args) {
            // no solution for winforms right now
            // new PigeonUi().Show();
        }

        private static void openSaveDir(string args) {
            Process.Start(PlayerData.UserDataPath);
        }
        #endregion

        #region toggled debug info
        private static void toggleCd(string args) {
            var before = World.DrawColliderDebugInfo;
            var after = !before;
            World.DrawColliderDebugInfo = after;
            ConsoleUtilities.LogVariableChange("collision debug", before, after);
        }

        private static void toggleHitbox(string args) {
            var before = GameObject.DrawHitboxesGlobal;
            var after = !before;
            GameObject.DrawHitboxesGlobal = after;
            ConsoleUtilities.LogVariableChange("hitbox debug", before, after);
        }

        private static void toggleMouse(string args) {
            var before = MouseReader.UpdateState;
            var after = !before;
            MouseReader.UpdateState = after;
            ConsoleUtilities.LogVariableChange("mouse updates", before, after);
        }

        private static void togglePosition(string args) {
            var before = GameObject.DrawPositionsGlobal;
            var after = !before;
            GameObject.DrawPositionsGlobal = after;
            ConsoleUtilities.LogVariableChange("position debug", before, after);
        }

        private static void toggleYSort(string args) {
            var before = YSorter.DrawEnabled;
            var after = !before;
            YSorter.DrawEnabled = after;
            ConsoleUtilities.LogVariableChange("ysort debug", before, after);
        }
        #endregion

        #region objects
        private static void getAllObjects(string args) {
            StringBuilder builder = new StringBuilder();
            GameObject obj = string.IsNullOrEmpty(args) ? Pigeon.World.RootObj : Pigeon.World.FindObj(args);
            describeChildren(obj, builder);
        }

        private static void bump(string args) {
            var splitArgs = args.Tokenize();
            var obj = Pigeon.World.FindObj(splitArgs[1]);

            var bumpDir = Point.Zero;
            switch (splitArgs[0].ToLower()) {
                case "up":
                    bumpDir.Y = -1;
                    break;
                case "down":
                    bumpDir.Y = 1;
                    break;
                case "left":
                    bumpDir.X = -1;
                    break;
                case "right":
                    bumpDir.X = 1;
                    break;
            }

            obj.LocalPosition += bumpDir;
        }

        private static void describeChildren(GameObject obj, StringBuilder builder) {
            var children = obj.GetChildren();
            if (children == null || children.Count == 0) {
                Pigeon.Console.Log("object has no children");
                return;
            }

            Pigeon.Console.Log("children:");
            var treeChildren = from child in children
                               where child.GetChildren()?.Count > 0
                               orderby child.Name ascending
                               select child.Name;

            foreach (var child in treeChildren) {
                builder.Append('+');
                builder.Append(child);
                builder.Append(' ');
            }

            var leafChildren = from child in children
                               where child.GetChildren() == null || child.GetChildren().Count == 0
                               orderby child.Name ascending
                               select child.Name;

            foreach (var child in leafChildren) {
                builder.Append('-');
                builder.Append(child);
                builder.Append(' ');
            }

            Pigeon.Console.Log(builder.ToString());
        }

        private static void inspect(string args) {
            var squabject = Pigeon.World.FindObj(args);
            if (squabject != null) {
                Pigeon.Console.Log(squabject.ToString());
                return;
            }

            Pigeon.Console.Error("object does not exist");
        }

        private static void setHideDrawable(string args) {
            var obj = Pigeon.World.FindObj(args);

            if (obj == null) {
                return;
            }

            Component drawable = (obj.GetComponent<SpriteRenderer>() ?? (Component) obj.GetComponent<ImageRenderer>()) ?? obj.GetComponent<TextRenderer>();

            if (drawable != null) {
                drawable.Enabled = !drawable.Enabled;
                ConsoleUtilities.LogVariableChange(args + " visible", !drawable.Enabled, drawable.Enabled);
            } else {
                Pigeon.Console.Log(args + " no drawable components");
            }
        }

        private static void getComponents(string args) {
            var obj = Pigeon.World.FindObj(args);

            if (obj.components?.Count > 0) {
                var componentNames = new List<string>();
                foreach (var component in obj.components) {
                    componentNames.Add(component.GetType().Name);
                }

                Pigeon.Console.Log(ConsoleUtilities.BracketedList(componentNames));
            } else {
                Pigeon.Console.Log("object has no components");
            }
        }
        #endregion

        #region world
        private static void createSandboxWorld(string args) {
            var splitArgs = args.Tokenize();

            Color background = Pigeon.EngineBkgdColor;

            if (splitArgs.Length == 3) {
                background.R = splitArgs[0].ToByte();
                background.G = splitArgs[1].ToByte();
                background.B = splitArgs[2].ToByte();
            }

            Pigeon.SetWorld(new EmptyWorld { BackgroundColor = background });
        }

        private static void togglePauseWorld(string args) {
            bool before = Pigeon.PauseWorld;
            bool after;
            if (string.IsNullOrEmpty(args)) {
                after = !Pigeon.PauseWorld;
            } else {
                after = args.ToBool();
            }

            Pigeon.PauseWorld = after;
            ConsoleUtilities.LogVariableChange("pause world", before, after);
        }
        #endregion

        #region audio
        private static void setBgmEqualizer(string args) {
            if (string.IsNullOrWhiteSpace(args)) {
                Pigeon.Console.Log("usage: bgmequalizer <treble> <bass>");
                Pigeon.Console.Log("-treble: -50 to 5 (def 0)");
                Pigeon.Console.Log("-bass: 1 to 16000 (def 90)");
            } else {
                var splitArgs = args.Tokenize();

                double treble = splitArgs[0].ToDouble();
                double bass = splitArgs[1].ToFloat();

                Music.Treble = treble;
                Music.Bass = bass;
            }
        }

        private static void setBgmMuteVoice(string args) {
            if (string.IsNullOrWhiteSpace(args)) {
                Pigeon.Console.Log("usage: bgmmutevoice <index> <mute>");
                Pigeon.Console.Log("-index: 0 to 7 (channel index)");
                Pigeon.Console.Log("-mute:  0 or 1 (unmute or mute)");
            } else {
                var splitArgs = args.Tokenize();
                Music.SetVoiceMute(splitArgs[0].ToInt(), splitArgs[1].ToBool());
            }
        }

        private static void setBgmMuteVoices(string args) {
            //			var mutingMask = 31;
            var mutingMask = Convert.ToInt32(args, 2);
            Music.MaskMuteVoices(mutingMask);
        }

        private static void bgmPlayTrack(string args) {
            if (string.IsNullOrWhiteSpace(args)) {
                Pigeon.Console.Log("usage: bgmplay <trackName>");
                Pigeon.Console.Log("-trackname: (path to track)");
                Pigeon.Console.Log("e.g. \"bgmplay metallicwing/solitude\"");
            } else {
                if (!args.StartsWith("music/")) {
                    args = "music/" + args;
                }

                if (!args.EndsWith(".nsf")) {
                    args += ".nsf";
                }

                if (!File.Exists(args)) {
                    Pigeon.Console.Error("unknown track");
                    return;
                }

                Music.Stop();
                Music.Load(args);
                Music.Play();
            }
        }

        private static void bgmPause(string args) {
            Music.Pause();
        }

        private static void bgmResume(string args) {
            Music.Play();
        }

        private static void bgmStop(string args) {
            Music.Stop();
        }

        private static void bgmStereoDepth(string args) {
            if (string.IsNullOrWhiteSpace(args)) {
                Pigeon.Console.Log("usage: bgmstereodepth <depth>");
                Pigeon.Console.Log("-depth: 0.0 to 1.0");
                Pigeon.Console.Log("-current: " + Music.StereoDepth);
            } else {
                Music.StereoDepth = args.ToDouble();
            }
        }

        private static void setBgmFade(string args) {
            if (string.IsNullOrWhiteSpace(args)) {
                Pigeon.Console.Log("usage: bgmfade <msLength>");
            } else {
                Music.Fade = args.ToInt();
            }
        }

        private static void bgmTempo(string args) {
            if (string.IsNullOrWhiteSpace(args)) {
                Pigeon.Console.Log("usage: bgmtempo <tempo>");
                Pigeon.Console.Log("-tempo: 0.5 to 2.0 (def 1)");
            } else {
                Music.Tempo = args.ToDouble();
            }
        }

        private static void setBgmVolume(string args) {
            if (string.IsNullOrWhiteSpace(args)) {
                Pigeon.Console.Log("usage: bgmvol <volume>");
                Pigeon.Console.Log("-volume: 0.0 to 1.0");
            }

            double? value = args.ToUnitInterval();

            if (value != null) {
                var before = Music.Volume;
                var after = (float) value;
                Music.SetVolumeInstant(after);
                ConsoleUtilities.LogVariableChange("bgm volume", before, after);
            } else {
                Pigeon.Console.Error("Invalid volume; enter a decimal value from 0.0 to 1.0");
            }
        }

        private static void setSfxVolume(string args) {
            double? value = args.ToUnitInterval();

            if (value != null) {
                var before = Sfx.SfxVolume;
                var after = (float) value;
                Sfx.SfxVolume = after;
                ConsoleUtilities.LogVariableChange("sfx vol", before, after);
            } else {
                Pigeon.Console.Error("Invalid volume; enter a decimal value from 0.0 to 1.0");
            }
        }
        #endregion

        #region vars
        private static void setVar(string args) {
            if (args?.Length == 0) {
                Pigeon.Console.Log("select category:");
                var allCategories = Const.Categories;
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var category in allCategories) {
                    stringBuilder.Append("+");
                    stringBuilder.Append(category);
                    stringBuilder.Append(" ");
                }
                Pigeon.Console.Log(stringBuilder.ToString());
                return;
            }

            var splitArgs = args.Tokenize();
            switch (splitArgs.Length) {
                case 1:
                    Pigeon.Console.Log("select variable:");
                    var vars = Const.GetVarsForCategory(splitArgs[0]);
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (var variable in vars) {
                        stringBuilder.Append("+");
                        stringBuilder.Append(variable);
                        stringBuilder.Append(" ");
                    }
                    Pigeon.Console.Log(stringBuilder.ToString());
                    break;
                case 2:
                    Pigeon.Console.Log("current value: " + Const.GetFloat(splitArgs[0], splitArgs[1]));
                    break;
                case 3:
                    var before = Const.GetFloat(splitArgs[0], splitArgs[1]);
                    Const.SetVar(splitArgs[0], splitArgs[1], splitArgs[2].ToFloat());
                    var after = Const.GetFloat(splitArgs[0], splitArgs[1]);
                    ConsoleUtilities.LogVariableChange(splitArgs[0] + "." + splitArgs[1], before, after);
                    break;
                default:
                    Pigeon.Console.Error("too many arguments provided.");
                    break;
            }
        }

        private static void resetVar(string args) {
            Const.ResetToDefaults();
            Pigeon.Console.Log("all variables restored to defaults");
        }

        private static void saveVarPreset(string args) {
            PlayerData.CreateDirectory(@"constants");
            Const.SavePreset(@"constants\" + args + ".cfg");
            Pigeon.Console.Log("created preset '" + args + "' from current variables");
        }

        private static void loadVarPreset(string args) {
            if (args?.Length == 0) {
                var presets = PlayerData.GetFileList(@"constants\*.cfg", false);

                StringBuilder stringBuilder = new StringBuilder();
                foreach (var preset in presets) {
                    stringBuilder.Append("*");
                    stringBuilder.Append(preset);
                    stringBuilder.Append(" ");
                }

                Pigeon.Console.Log("available presets:");
                Pigeon.Console.Log(stringBuilder.ToString());
            } else {
                Const.LoadPreset(@"constants\" + args + ".cfg");
                Pigeon.Console.Log("loaded preset '" + args + "'");
            }
        }
        #endregion

        #region binds/aliases
        private static void setBind(string args) {
            if (string.IsNullOrWhiteSpace(args)) {
                Pigeon.Console.Log("current keybinds:");
                foreach (var boundKey in KeyBinds.GetAllKeyBinds().Keys) {
                    displayKeybind(boundKey);
                }
                return;
            }

            string[] splitArgs = args.Split(new[] { ' ' }, 2);

            Keys key = KeyBinds.ParseToKey(splitArgs[0]);

            if (splitArgs.Length == 2) {    // create a new bind
                KeyBinds.BindKey(key, splitArgs[1]);
            }

            // if no command is given, just display current bind for that key
            displayKeybind(key);
        }

        private static void setUnbind(string args) {
            if (args == "all") {
                KeyBinds.Reset();
                Pigeon.Console.Log("all custom binds reset");
            } else {
                var key = KeyBinds.ParseToKey(args);
                KeyBinds.UnbindKey(key);
                displayKeybind(key);
            }
        }

        private static void displayKeybind(Keys key) {
            Pigeon.Console.Log(string.Format("{0}: {1}", key, KeyBinds.GetKeyBind(key) ?? "<unbound>"));
        }

        private static void findKeyboardKeyName(string args) {
            var matches = EnumUtil.GetValues<Keys>().Select(k => k.ToString()).FindStringMatches(args);

            if (matches.Count == 0) {
                Pigeon.Console.Log(string.Format("no keys match for '{0}'", args));
            } else {
                Pigeon.Console.Log(string.Format("keys matching '{0}':", args));
                Pigeon.Console.Log(ConsoleUtilities.BracketedList(matches));
            }
        }

        private static void setAlias(string args) {
            var manager = Pigeon.Console.AliasManager;

            if (string.IsNullOrEmpty(args)) {
                var names = manager.GetNames();
                Pigeon.Console.Log(names.Count > 0 ? ConsoleUtilities.BracketedList(names) : "no aliases currently defined");
                return;
            }

            var splitArgs = args.Split(new[] { ' ' }, 2);
            var aliasName = splitArgs[0];

            if (splitArgs.Length == 1) {
                if (manager.Exists(aliasName)) {
                    var commands = manager.Get(aliasName);

                    Pigeon.Console.Log(string.Format("{0}:", aliasName));
                    foreach (var command in commands) {
                        Pigeon.Console.Log(string.Format("  -{0}", command));
                    }
                } else {
                    Pigeon.Console.Log(string.Format("no alias with name '{0}'", splitArgs[0]));
                }
                return;
            }

            string aliasValue = splitArgs[1];
            manager.Set(aliasName, aliasValue);
        }
        #endregion
    }
}
