using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Babel.System.Data
{
    public class SimpleVector3
    {
        public float x;
        public float y;
        public float z;

        public SimpleVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        //
        //Data type Covertion
        //
        public Vector3 getUnityVector3()
        {
            return new Vector3(x, y, z);
        }

        public Quaternion getUnityRotationQuaternion()
        {

            return Quaternion.Euler(this.getUnityVector3());
        }

        public override string ToString()
        {
            return getUnityVector3().ToString();
        }


        public static SimpleVector3 operator +(SimpleVector3 a, SimpleVector3 b)
        {
            return new SimpleVector3(a.x + b.x, a.y + b.y, a.z + b.z);

        }
    }
}
