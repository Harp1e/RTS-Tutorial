﻿using System.Collections;
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
        public static int BuildSpeed { get { return 2; } }

        static Vector3 invalidPosition = new Vector3 (-99999, -99999, -99999);
        public static Vector3 InvalidPosition { get { return invalidPosition; } }

        static GUISkin selectBoxSkin;
        public static GUISkin SelectBoxSkin { get { return selectBoxSkin; } }

        static Bounds invalidBounds = new Bounds (new Vector3 (-99999, -99999, -99999), new Vector3(0, 0, 0));
        public static Bounds InvalidBounds {get {return invalidBounds; } }

        static GameObjectList gameObjectList;

        public static void StoreSelectBoxItems (GUISkin skin)
        {
            selectBoxSkin = skin;
        }

        public static void SetGameObjectList (GameObjectList objectList)
        {
            gameObjectList = objectList;
        }

        public static GameObject GetBuilding (string name)
        {
            return gameObjectList.GetBuilding (name);
        }

        public static GameObject GetUnit (string name)
        {
            return gameObjectList.GetUnit (name);
        }

        public static GameObject GetWorldObject (string name)
        {
            return gameObjectList.GetWorldObject (name);
        }

        public static GameObject GetPlayerObject ()
        {
            return gameObjectList.GetPlayerObject ();
        }

        public static Texture2D GetBuildImage (string name)
        {
            return gameObjectList.GetBuildImage (name);
        }
    }
}
