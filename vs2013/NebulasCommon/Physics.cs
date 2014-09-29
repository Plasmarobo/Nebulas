using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulas
{
    namespace Physics
    {
        class PhysicsObject
        {
            //Model physics as 2d rigid bodies
            Nebulas.Math.Vec3 mPivot;
            Double mRadius;
            Double mMass;
            Boolean mNoclip;
            Double mReflection;
            Double mAbsorption;
            Double mIntegrity;
        }
    }
}
