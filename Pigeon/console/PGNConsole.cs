using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using pigeon.data;
using pigeon.input;
using pigeon.core;
using pigeon.utilities.extensions;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using pigeon.time;
using pigeon.gfx.drawable.text;
using pigeon.gfx;
using pigeon.gameobject;
using pigeon.gfx.drawable.image;
using pigeon.utilities;
using pigeon.gfx.drawable.sprite;

namespace pigeon.console {
    public class PGNConsole : World {
        #region constants
        private const int bufferMaxLength = 300;
        private int lineOverflowWidth;
        internal readonly ConsoleOptions options;
        private readonly SpriteFont font;
        private readonly Point bufferHomePosition;
        #endregion

        #region helpers
        private readonly CommandHistory history;
        internal MessageLog messageLog;
        internal readonly AliasManager AliasManager = new AliasManager();
        private GameObject cursor;
        private TextRenderer buffer;
        #endregion

        #region commands
        internal List<string> AllCommandNames {
            get {
                var all = new List<string>();
                all.AddRange(EngineCommandNames);
                all.AddRange(GameCommandNames);
                return all;
            }
        }
        internal List<string> EngineCommandNames { get { return engineCommands.Keys.ToList(); } }
        internal List<string> GameCommandNames { get { return gameCommands.Keys.ToList(); } }
        private readonly Dictionary<string, ConsoleCommand> engineCommands = new Dictionary<string, ConsoleCommand>();
        private readonly Dictionary<string, ConsoleCommand> gameCommands = new Dictionary<string, ConsoleCommand>();
        #endregion

        #region key repeating
        private Keys lastKey;
        private float lastKeyHoldTimeSecs;
        private bool isRepeatingKey;
        private const float beginRepeatKeySecs = .6f;
        private const float continueRepeatKeySecs = .05f;
        #endregion
        
        #region command buffer state
        private string previousCommand = "";

        private int _commandCursorIndex = 0;
        private int commandCursorIndex {
            get { return _commandCursorIndex; }
            set {
                _commandCursorIndex = value.Clamp(0, commandBuffer.Length);

                cursor.LocalPosition = cursor.LocalPosition.WithX(bufferHomePosition.X + font.MeasureWidth(buffer.Text.Substring(0, _commandCursorIndex) + " ") + (int) font.Spacing - 1);
            }
        }

        private string _commandBuffer;
        private string commandBuffer {
            get { return _commandBuffer; }
            set {
                if (font.MeasureWidth(value) > bufferMaxLength) {
                    return;
                }

                _commandBuffer = value;
                var displayPortion = _commandBuffer.LastByPixels(lineOverflowWidth, font);
                buffer.Text = string.Format(">{0}", displayPortion);

                if (commandCursorIndex >= value.Length) {
                    commandCursorIndex = value.Length;
                }
            }
        }
        #endregion

        public bool IsDisplaying { get; private set; }

        public PGNConsole(ConsoleOptions options) {
            this.options = options;

            font = ResourceCache.Font("console");
            bufferHomePosition = new Point(this.options.TextInset, this.options.PanelHeight - (this.options.TextInset * 2));

            history = new CommandHistory(options.CommandHistory);
        }

        protected override void Load() {
            AliasManager.Load();

            var panelTexture = new Texture2D(Renderer.GraphicsDeviceMgr.GraphicsDevice, Pigeon.Renderer.BaseResolutionX, this.options.PanelHeight);

            Color[] panelPixels = new Color[panelTexture.Width * panelTexture.Height];
            for (int i = 0; i < panelPixels.Length; i++) {
                panelPixels[i] = options.PanelColor;
            }

            panelTexture.SetData(panelPixels);
            var objPanel = new GameObject("panel") { LayerInheritanceEnabled = false, LayerSortingVarianceEnabled = false };
            objPanel.AddComponent(new ImageRenderer() { Image = Image.Create(panelTexture) } );
            AddObj(objPanel);

            Sprite cursorSprite = Sprite.Clone("consoleCursor", @"console\cursor");
            cursorSprite.Loop("flash");
            cursorSprite.Color = options.CursorColor;
            cursor = new GameObject("cursor") { LocalPosition = bufferHomePosition - new Point(0, 1), Layer = .5f, LayerSortingVarianceEnabled = false } ;
            cursor.AddComponent(new SpriteRenderer() { Sprite = cursorSprite });
            AddObj(cursor);

            lineOverflowWidth = Pigeon.Renderer.BaseResolutionX - options.TextInset - (font.MeasureWidth(">") * 3);
            buffer = new TextRenderer() { Font = font, Color = options.BufferColor, Justification = Justifications.TopLeft };
            AddObj(new GameObject("buffer") { LocalPosition = bufferHomePosition, Layer = 1f }.AddComponent(buffer));
            commandBuffer = string.Empty;

            int lineSpacing = font.MeasureHeight(">");
            Point bottomMessagePosition = bufferHomePosition.MinusY(lineSpacing);
            messageLog = new MessageLog(font, lineOverflowWidth, bottomMessagePosition, lineSpacing, options, this);
            
            AddDebugger = false;

            Log("Console loaded...");
        }

        protected override void Unload() { }

        public override void Update() {
            base.Update();

            if (RawKeyboardInput.IsPressed(Keys.OemTilde)) {
                IsDisplaying = !IsDisplaying;
            }

            if (!IsDisplaying) {
                return;
            }

            handleKeyboardInput();
        }

        public override void Draw() {
            if (IsDisplaying) {
                Pigeon.Renderer.RenderOverlay(RootObj.Draw);
            }
        }

        internal void AddGlobalCommands(Dictionary<string, ConsoleCommand> newCommands) {
            foreach (var command in newCommands) {
                engineCommands.Add(command.Key, command.Value);
            }
        }

        public void AddWorldCommands(Dictionary<string, ConsoleCommand> newCommands) {
            foreach (var command in newCommands) {
                gameCommands.Add(command.Key, command.Value);
            }
        }

        public void ResetWorldCommands() {
            gameCommands.Clear();
        }

        private void handleKeyboardInput() {
            Keys key = RawKeyboardInput.GetSingleJustPressedKey();

            if (key == Keys.None) { // if did not press a new key
                if (lastKey != Keys.None) { // but key was down on previous frame
                    checkKeyRepeat();
                    if (isRepeatingKey) {
                        handleContinueRepeatingKey();
                    } else {
                        handleBeginRepeatingKey();
                    }
                }
                return;
            }

            if (isRepeatingKey) {
                resetKeyRepeat();
            }

            lastKey = key;

            handleSingleKey(key);
        }

        private void handleSingleKey(Keys key, bool isFromKeyRepeat = false) {
            if (key == Keys.Up && !isFromKeyRepeat) {
                handleUp();
            } else if (key == Keys.Down && !isFromKeyRepeat) {
                handleDown();
            } else if(key == Keys.Enter && !isFromKeyRepeat) {
                history.Reset();
                commitCommand();
            } else if (key == Keys.Tab && !isFromKeyRepeat) {
                history.Reset();
                handleAutocomplete();
            } else if (key == Keys.Home && !isFromKeyRepeat) {
                handleHome();
            } else if (key == Keys.End && !isFromKeyRepeat) {
                handleEnd();
            } else if (key == Keys.Left) {
                handleLeft();
            } else if (key == Keys.Right) {
                handleRight();
            } else if (key == Keys.Back) {
                handleBackspace();
            } else if (key == Keys.Delete) {
                handleDelete();
            } else  if (key.IsPrintable()) {
                handleCharacters(key.ToChar());
            }
        }

        private void handleBeginRepeatingKey() {
            if (lastKeyHoldTimeSecs >= beginRepeatKeySecs) {
                isRepeatingKey = true;
                lastKeyHoldTimeSecs = 0f;
                handleSingleKey(lastKey, true);
            }
        }

        private void handleContinueRepeatingKey() {
            if (lastKeyHoldTimeSecs >= continueRepeatKeySecs) {
                lastKeyHoldTimeSecs = 0f;

                handleSingleKey(lastKey, true);
            }
        }

        private void checkKeyRepeat() {
            if (RawKeyboardInput.IsHeld(lastKey)) { // if key from last frame is still down on this frame, we're in repeat territory
                lastKeyHoldTimeSecs += Time.Seconds;

                if (!isRepeatingKey) {
                    handleBeginRepeatingKey();
                } else {
                    handleContinueRepeatingKey();
                }
            } else { // key repeat is over. RESET EVERYTHING!
                resetKeyRepeat();
            }
        }

        private void resetKeyRepeat() {
            isRepeatingKey = false;
            lastKey = Keys.None;
            lastKeyHoldTimeSecs = 0;
        }

        #region key handlers
        private void handleEnd() {
            commandCursorIndex = commandBuffer.Length;
        }

        private void handleHome() {
            commandCursorIndex = 0;
        }

        private void handleDown() {
            commandBuffer = history.Previous();
            commandCursorIndex = commandBuffer.Length;
        }

        private void handleUp() {
            var command = history.Next();
            if (command != null) {
                commandBuffer = command;
                commandCursorIndex = commandBuffer.Length;
            }
        }

        private void handleAutocomplete() {
            if (commandBuffer.Length == 0) {
                return;
            }

            List<string> possibleCommands = new List<string>();

            int minMatchingChars = 0;

            foreach (var commandPair in engineCommands) {
                var commandName = commandPair.Key;
                if (commandName.StartsWith(commandBuffer)) {
                    minMatchingChars = Math.Min(minMatchingChars, commandName.Length - commandBuffer.Length);
                    possibleCommands.Add(commandName);
                }
            }

            foreach (var commandPair in gameCommands) {
                var commandName = commandPair.Key;
                if (commandName.StartsWith(commandBuffer)) {
                    minMatchingChars = Math.Min(minMatchingChars, commandName.Length - commandBuffer.Length);
                    possibleCommands.Add(commandName);
                }
            }

            switch (possibleCommands.Count) {
                case 0:
                    Log(string.Format("no commands starting with '{0}'...", commandBuffer));
                    break;
                case 1:
                    commandBuffer = possibleCommands[0];
                    break;
                default:
                    var referenceCommand = possibleCommands[0];

                    bool breakLoop = false;
                    int charInd = 0;
                    while (charInd < referenceCommand.Length) {
                        // check every other command to see how many of their letters match
                        var character = referenceCommand[charInd];
                        for (int commandInd = 1; commandInd < possibleCommands.Count && !breakLoop; commandInd++) {
                            string command = possibleCommands[commandInd];

                            if (charInd >= command.Length || command[charInd] != character) {
                                charInd--;
                                breakLoop = true;
                            }
                        }

                        if (!breakLoop && charInd < referenceCommand.Length - 1) {
                            charInd++;
                        } else {
                            break;
                        }
                    }

                    if (charInd + 1 > commandBuffer.Length) {
                        commandBuffer = referenceCommand.Substring(0, charInd + 1);
                    } else {
                        Log(string.Format("commands starting with '{0}': ", commandBuffer));
                        Log(ConsoleUtilities.BracketedList(possibleCommands));
                    }

                    break;
            }

            commandCursorIndex = commandBuffer.Length;
        }

        private void handleRight() {
            commandCursorIndex++;
        }

        private void handleLeft() {
            commandCursorIndex--;
        }

        private void handleCharacters(char character) {
            history.Reset();

            if (commandBuffer.Length == bufferMaxLength) {
                return;
            }

            if (isShiftHeld() && character.TryGetShiftChar(out char value)) {
                character = value;
            } else if (isCapsLockXorShift()) {
                if (char.IsLetter(character) && char.IsLower(character)) {
                    character = char.ToUpper(character);
                }
            }

            commandBuffer = commandBuffer.Insert(commandCursorIndex, character.ToString());
            commandCursorIndex++;
        }

        private void commitCommand() {
            string command = commandBuffer;
            ExecuteCommand(command, true);
        }

        private void handleBackspace() {
            history.Reset();

            if (commandBuffer.Length > 0 && commandCursorIndex > 0) {
                bool isCursorAtEnd = commandCursorIndex == commandBuffer.Length;

                commandBuffer = commandBuffer.Remove(commandCursorIndex - 1, 1);

                if (!isCursorAtEnd) {
                    commandCursorIndex--;
                }
            }
        }

        private void handleDelete() {
            history.Reset();

            if (commandBuffer.Length > 0 && commandCursorIndex < commandBuffer.Length) {
                commandBuffer = commandBuffer.Remove(commandCursorIndex, 1);
            }
        }
        #endregion

        public void ExecuteCommand(string commandFull, bool addToHistory = false) {
            string lowercaseBuffer = commandFull;

            if (addToHistory) {
                logCommand(">" + commandFull);
                history.Commit(commandFull);
                commandBuffer = "";
            }

            try {
                string[] splitBuffer = lowercaseBuffer.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

                if (splitBuffer.Length == 0) {
                    return;
                }

                string commandName = splitBuffer[0].ToLower();

                if (engineCommands.TryGetValue(commandName, out ConsoleCommand command) || gameCommands.TryGetValue(commandName, out command)) {
                    string args = splitBuffer.Length == 2 ? splitBuffer[1] : string.Empty;
                    command.Invoke(args);
                } else if (AliasManager.Exists(commandName)) {
                    List<string> aliasCommands = AliasManager.Get(commandName);
                    foreach (var aliasCommand in aliasCommands) {
                        ExecuteCommand(aliasCommand);
                    }
                } else {
                    Error("Command name not recognized: " + commandName);
                }

                previousCommand = (commandName == "repeat") ? previousCommand : commandFull;
            } catch (Exception e) {
                Error("command failed: " + commandFull);
                Log(e.Message);
            }
        }

        private static bool isShiftHeld() {
            bool leftShift = RawKeyboardInput.IsHeld(Keys.LeftShift);
            bool rightShift = RawKeyboardInput.IsHeld(Keys.RightShift);
            return leftShift || rightShift;
        }

        private static bool isCapsLockXorShift() {
            return isShiftHeld() ^ Console.CapsLock;
        }

        #region logging
        public void DebugLog(string message) {
            Log(message);
        }

        public void DebugLogToFile(string message, string logFilename = @"logs\debug.log") {
            PlayerData.AppendToFile(logFilename, message);
        }

        public void Log(object message) {
            Log(message.ToString());
        }

        public void Log(string message) {
            messageLog.AddMessage(new LogMessage(message, LogMessageTypes.Info));
        }

        public void Error(string message) {
            messageLog.AddMessage(new LogMessage(message, LogMessageTypes.Error));
        }

        private void logCommand(string message) {
            messageLog.AddMessage(new LogMessage(message, LogMessageTypes.Command));
        }
        #endregion

        public void RepeatPrevious() {
            ExecuteCommand(previousCommand);
        }
    }
}
