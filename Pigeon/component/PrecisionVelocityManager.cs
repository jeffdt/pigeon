using System;
using Microsoft.Xna.Framework;
using pigeon.gameobject;

namespace pigeon.component {
    public class PrecisionVelocityManager : Component {
        public float Multiplier = 1f;

        private int xPix;
        private int xFramesPerPix;
        private int yPix;
        private int yFramesPerPix;

        private int xFramesAccum;
        private int yFramesAccum;

        // 2,0,1,0 would be moving two pixels to the right on each frame	// for fast moving objects
        // 1,0,2,0 would be moving one pixel to the right every 2nd frame	// for slow moving objects
        public PrecisionVelocityManager SetVelocity(int xPerSecond, int yPerSecond) {
            int xPerFrame = xPerSecond / 60;
            if (xPerSecond == 0) {
                xPix = 0;
                xFramesPerPix = 100;
            } else if (xPerFrame != 0) {
                xPix = (int) Math.Round(xPerSecond / 60f);
                xFramesPerPix = 1;
            } else {
                xPix = (xPerSecond < 0) ? -1 : 1;
                xFramesPerPix = (int) Math.Abs(1 / (xPerSecond / 60f));
            }

            int yPerFrame = yPerSecond / 60;
            if (yPerSecond == 0) {
                yPix = 0;
                yFramesPerPix = 100;
            } else if (yPerFrame != 0) {
                yPix = (int) Math.Round(yPerSecond / 60f);
                yFramesPerPix = 1;
            } else {
                yPix = (yPerSecond < 0) ? -1 : 1;
                yFramesPerPix = (int) Math.Abs(1 / (yPerSecond / 60f));
            }

            xFramesAccum = 0;
            yFramesAccum = 0;

            return this;
        }

        protected override void Initialize() { }

        protected override void Update() {
            int newX = Object.LocalPosition.X;
            int newY = Object.LocalPosition.Y;

            xFramesAccum++;
            if (xFramesAccum >= xFramesPerPix) {
                xFramesAccum = 0;
                newX += xPix;
            }

            yFramesAccum++;
            if (yFramesAccum >= yFramesPerPix) {
                yFramesAccum = 0;
                newY += yPix;
            }

            if (newX != Object.LocalPosition.X || newY != Object.LocalPosition.Y) {
                Object.LocalPosition = new Point(newX, newY);
            }
        }
    }
}