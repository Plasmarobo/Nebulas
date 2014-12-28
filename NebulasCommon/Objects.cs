using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nebulas.Events;
using Nebulas.Math;

namespace Nebulas
{
    namespace Objects
    {
        public class Prop : EventListener
        {
            //A part can have multiple parents
            Int32 mObjectId; //Unique to universe object
            String mResourceId; //Use to load image resource client side
            Prop[] mParent;
            Prop[] mChildren;
            Boolean mLocked; //Locked to parent
            Vec3 mPosition; //z rotation
            Vec3 mVelocity; //z rotation
            Vec3 mAcceleration; //z rotation
            Vec3 mPositionDamping;
            Vec3 mVelocityDamping;
            Vec3 mAccelerationDamping;

        }

        public class Effect : Prop
        {
            String mDefinition;
        }

        public class BasicObject : Prop
        {
            String mName;
            String mIcon;
            Dictionary<String, String> mOverlayFootprint;
        }
    }
}
