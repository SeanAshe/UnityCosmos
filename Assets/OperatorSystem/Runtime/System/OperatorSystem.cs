using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.unity
{
    public static class InputManager
    {
        public static Vector2 GetInputPosion()
        {
#if UNITY_ANDROID || UNITY_IOS
            return Input.touches[0].position;
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
            return Input.mousePosition;
#endif
        }
    }
}
