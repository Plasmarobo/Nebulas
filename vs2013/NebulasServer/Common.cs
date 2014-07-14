using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulas
{
    namespace Common
    {
        class Vec3
        {
            Double x;
            Double y;
            Double z;
        }
        class Camera
        {
            Vec3 mTranslation; //z Zoom
            Double mRotation;
        }
        
        class Prop
        {
            //A part can have multiple parents
            String mId;
            Prop[] mParent;
            Prop[] mChildren;
            Boolean mLocked; //Locked to parent
            String mImageName;
            Vec3 mPosition; //z rotation
            Vec3 mVelocity; //z rotation
            Vec3 mAcceleration; //z rotation
            Vec3 mPositionDamping;
            Vec3 mVelocityDamping;
            Vec3 mAccelerationDamping;
            Events.EventListener mEventListener;
        }
        class Effect : Prop
        {
            String mDefinition;
        }
        class BasicObject : Prop
        {
            String mName;
            String mIcon;
            Dictionary<String, String> mOverlayFootprint;
        }
        class PhysicsObject
        {
            //Model physics as 2d rigid bodies
            Vec3 mPivot;
            Double mRadius;
            Double mMass;
            Boolean mNoclip;
            Double mReflection;
            Double mAbsorption;
            Double mIntegrity;
        }
        struct FactionInfo
        {
            String mName;
            Int16 mLoyalty;
            Int16 mRank;
        }
        namespace Ship
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
        class Planet : PhysicsObject
        {
            String mName;
            FactionInfo mFactionInfo;
            Dictionary<String, Int32> mResources;
            String mDescription;
        }
        class Sector
        {
            static String mSectorPath = "sector";
            Int16 mId;
            Int32 mSize;
            String mName;
            List<Prop> mObjects;
            Sector mUp;
            Sector mDown;
            Sector mLeft;
            Sector mRight;
            void Load(String mPath, String mFile)
            {
                System.IO.FileStream file = System.IO.File.OpenRead(mPath + System.IO.Path.DirectorySeparatorChar + mFile);
                System.IO.BinaryReader reader = new System.IO.BinaryReader(file, Encoding.UTF8);
                mId = reader.ReadInt16();
                mSize = reader.ReadInt32();
                //TODO: Read in object states
            }
            void Save(String mPath, String mFile)
            {
                System.IO.FileStream file = System.IO.File.OpenWrite(mPath + System.IO.Path.DirectorySeparatorChar + mFile);
                System.IO.BinaryWriter writer = new System.IO.BinaryWriter(file, Encoding.UTF8);
                writer.Write(mId);
                writer.Write(mSize);
                //TODO: Write objects out
                //writer.Write(mObjects);
                
            }
            
        }
        class Scene
        {
            enum SectorGridPosition
            {
                TopLeft = 0,
                TopMid,
                TopRight,
                MidLeft,
                MidMid,
                MidRight,
                BottomLeft,
                BottomMid,
                BottomRight
            }
            Sector[] mGrid;
            Camera mCamera;
            Events.EventListener mEventListener;
            void Shift(SectorGridPosition direction)
            {

            }
        }
        class Galaxy
        {
            Sector[] mSectors;
        }
    }
    namespace Events
    {
        
        class Event
        {
            String mName;
            DateTime mTimestamp;
            Dictionary<String, String> mProperties;
        }
        class EventStream
        {
            Queue<Event> mEventQueue;
            EventStream mMaster;
            void SyncWithMaster()
            {

            }
            void DispatchAll()
            {

            }
        }

        class EventListener
        {
            void registerWithStream(EventStream parent)
            { }
            void registerEvent(String event_name, String response)
            { }
            Dictionary<String, String> mRegisteredEvents;
            void handleEvent(Event evt) { }
        }
        
    }
}
