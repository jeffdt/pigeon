using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace pigeon.utilities.helpers {
    public static class EightWayDirections {
        public const string CenterFriendly = @"center";
        public const string UpFriendly = @"up";
        public const string LeftFriendly = @"left";
        public const string DownFriendly = @"down";
        public const string RightFriendly = @"right";
        public const string UpLeftFriendly = @"upleft";
        public const string DownLeftFriendly = @"downleft";
        public const string DownRightFriendly = @"downright";
        public const string UpRightFriendly = @"upright";

        public static Vector2 Center = new Vector2(0, 0);
        public static Vector2 Up = new Vector2(0, -1);
        public static Vector2 Left = new Vector2(-1, 0);
        public static Vector2 Down = new Vector2(0, 1);
        public static Vector2 Right = new Vector2(1, 0);
        public static Vector2 UpLeft = new Vector2(-1, -1);
        public static Vector2 DownLeft = new Vector2(-1, 1);
        public static Vector2 DownRight = new Vector2(1, 1);
        public static Vector2 UpRight = new Vector2(1, -1);

        private static readonly Dictionary<Vector2, string> descriptions = new Dictionary<Vector2, string> {
            {Center, CenterFriendly },
            {Up, UpFriendly },
            {Left, LeftFriendly},
            {Down, DownFriendly},
            {Right, RightFriendly},
            {UpLeft, UpLeftFriendly},
            {DownLeft, DownLeftFriendly},
            {DownRight, DownRightFriendly},
            {UpRight, UpRightFriendly}
        };

        public static string Describe(Vector2 direction) {
            return descriptions[direction];
        }

        public static string DescribeCardinal(Vector2 direction) {
            if (direction.X == -1) {
                return LeftFriendly;
            }

            if (direction.X == 1) {
                return RightFriendly;
            }

            if (direction.Y == -1) {
                return UpFriendly;
            }

            return direction.Y == 1 ? DownFriendly : CenterFriendly;
        }

        public static Vector2 Translate(string description) {
            switch (description) {
                case CenterFriendly:
                    return Center;

                case UpFriendly:
                    return Up;
                case DownFriendly:
                    return Down;
                case LeftFriendly:
                    return Left;
                case RightFriendly:
                    return Right;

                case UpLeftFriendly:
                    return UpLeft;
                case UpRightFriendly:
                    return UpRight;
                case DownLeftFriendly:
                    return DownLeft;
                case DownRightFriendly:
                    return DownRight;

                default:
                    throw new ArgumentException(string.Format("direction '{0}' cannot be translated to one of the major directions", description));
            }
        }
    }
}
