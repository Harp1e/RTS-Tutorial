using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS {
    public static class ResourceManager
    {
        public static int ScrollWidth { get { return 15; } }

        public static float ScrollSpeed { get { return 25f; } }
        public static float RotateSpeed { get { return 100f; } }
        public static float RotateAmount { get { return 10; } }
        public static float MinCameraHeight { get { return 10; } }
        public static float MaxCameraHeight { get { return 40; } }
    }
}
