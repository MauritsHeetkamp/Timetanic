using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Types
{
    public struct Int3
    {
        public int x;
        public int y;
        public int z;

        public Int3(Vector3 vector)
        {
            x = (int)vector.x;
            y = (int)vector.y;
            z = (int)vector.z;
        }

        public Vector3 Normalized()
        {
            Vector3 normalizedVector = new Vector3(x, y, z) / 12;
            return normalizedVector;
        }
    }
}