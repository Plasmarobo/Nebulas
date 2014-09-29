using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nebulas.Physics;
using Nebulas.Objects;
using Nebulas.World;


namespace Nebulas
{
    namespace Ships
    {
        struct System
        {
            PhysicsObject mObject;
            Dictionary<String, String> mInternalProperties;
            Dictionary<String, String> mExternalProperties;
            Dictionary<String, String> mWeaponProperties;
            Dictionary<String, String> mShieldProperties;
            Dictionary<String, String> mDroneProperties;
            Dictionary<String, String> mSystemProperties;
            Boolean mIsExternal;
            Dictionary<String, Double> mAttributes;
        }
        struct Mod
        {
            System mTarget;
            Dictionary<String, String> mModProperties;
        }
        struct Link
        {
            System mFirst;
            System mSecond;
            Dictionary<String, String> mLinkProperties;
            String mOnTransfer;
        }
        class Component : PhysicsObject
        {
            System[] mSystemSlots;
            Mod[] mModSlots;
            Link[] mLinkages;
            Boolean mDockPoint;
            Boolean mFuelPoint;
            Boolean mEssential;
        }
        struct Behavior
        {
            String mName;
            Prop[] mTargets;
            String mBehavior;
        }
        class AI
        {
            Dictionary<String, Behavior> mBehaviors;
            String mDecisionScript;
        }
        class Ship
        {
            Component[] mComponents;
            String mName;
            FactionInfo mFactionInfo;
            AI mAI;
            Boolean mUseAI;
            Events.EventListener mEventListener;
        }
    }
}
