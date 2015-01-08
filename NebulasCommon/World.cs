using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nebulas.Physics;
using Nebulas.Objects;

namespace Nebulas
{
    namespace World
    {
        struct FactionInfo
        {
            String mName;
            Int16 mLoyalty;
            Int16 mRank;
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
        class Galaxy
        {
            Sector[] mSectors;
            Int32[][] mBackground;
        }
    }
}
