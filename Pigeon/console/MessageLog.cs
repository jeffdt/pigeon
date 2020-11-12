using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using pigeon.core;
using pigeon.gameobject;
using pigeon.gfx.drawable.text;
using pigeon.utilities;
using pigeon.utilities.extensions;

namespace pigeon.console {
    internal class MessageLog {
        private readonly int limit;
        private readonly int lineSpacing;
        private readonly SpriteFont font;
        private readonly Point bottomMessagePosition;
        private readonly int wrapWidth;
        private readonly Dictionary<LogMessageTypes, Color> typeColors;
        private readonly World console;

        private readonly List<LogMessage> allMessages;
        private readonly List<GameObject> messageEntities;

        public MessageLog(SpriteFont font, int lineWrapWidth, Point bottomMessagePosition, int lineSpacing, ConsoleOptions options, World console) {
            this.font = font;
            this.wrapWidth = lineWrapWidth;
            this.lineSpacing = lineSpacing;
            this.bottomMessagePosition = bottomMessagePosition;
            this.console = console;

            limit = options.LogHistoryLimit;
            typeColors = new Dictionary<LogMessageTypes, Color> { { LogMessageTypes.Command, options.HistoryColor }, { LogMessageTypes.Info, options.InfoColor }, { LogMessageTypes.Error, options.ErrorColor } };

            allMessages = new List<LogMessage>();
            messageEntities = new List<GameObject>();
        }

        public void AddMessage(LogMessage message) {
            var splitLines = message.Text.SplitWrap(font, wrapWidth, "|");

            allMessages.Add(message);

            foreach (string line in splitLines) {
                logOne(line, message.Type);
            }

            Pigeon.EngineEventRegistry.RaiseEvent(this, new ConsoleLogChangedEvent { Message = message });
        }

        public List<LogMessage> GetAllMessages() {
            return allMessages;
        }

        private void logOne(string text, LogMessageTypes type) {
            if (messageEntities.Count == limit) {
                // TODO: for scrollable log history, this part will need to change
                messageEntities[limit - 1].Delete();
                messageEntities.RemoveAt(limit - 1);
            }

            var textRenderer = new TextRenderer() { Text = text, Font = font, Color = typeColors[type], Justification = Justifications.TopLeft };
            var textObj = new GameObject("logMsg") { LocalPosition = bottomMessagePosition, LayerSortingVarianceEnabled = false, Layer = 1f };
            textObj.AddComponent(textRenderer);
            console.AddObj(textObj);
            messageEntities.Insert(0, textObj);

            for (int index = 1; index < messageEntities.Count; index++) {
                var line = messageEntities[index];
                var previousLine = messageEntities[index - 1];
                line.LocalPosition = line.LocalPosition.WithY(previousLine.LocalPosition.Y - lineSpacing);
            }
        }
    }

    internal class ConsoleLogChangedEvent : EventArgs {
        public LogMessage Message { get; set; }
    }

    internal enum LogMessageTypes {
        Command, Info, Error
    }
}
