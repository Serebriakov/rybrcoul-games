using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

// Class with other functions

namespace HelpingClasses
{
    public static class Helpers
    {
        /// <summary>
        /// Converts x, y and z axes rotations into quaternion.
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Quaternion QFromV3(Vector3 vec)
        {
            Quaternion a = new Mogre.Quaternion(new Radian((float)(vec.x)), new Mogre.Vector3(1, 0, 0));
            Quaternion b = new Mogre.Quaternion(new Radian((float)(vec.y)), new Mogre.Vector3(0, 1, 0));
            Quaternion c = new Mogre.Quaternion(new Radian((float)(vec.z)), new Mogre.Vector3(0, 0, 1));

            Quaternion q = a * b * c;

            return q;
        }

        public static Vector3 V3FromQ(Quaternion q)
        {
            float X = q.Pitch.ValueRadians;
            float Y = q.Roll.ValueRadians;
            float Z = q.Yaw.ValueRadians;
            return new Vector3(X,Y,Z);
        }
    }
    
    /// <summary>
    /// Class for easy converting data to byte arrays.
    /// </summary>
    public class ByteList
    {
        private int size=0;
        private List<Byte[]> bytes = new List<byte[]>();

        public void Add(params Object[] args)
        {
            foreach (Object obj in args)
            {
                if (obj is Int16) _Add((Int16)obj);
                if (obj is Int32) _Add((Int32)obj);
                if (obj is Int64) _Add((Int64)obj);
                if (obj is UInt16) _Add((UInt16)obj);
                if (obj is UInt32) _Add((UInt32)obj);
                if (obj is UInt64) _Add((UInt64)obj);
                if (obj is String) _Add((String)obj);
                if (obj is Boolean) _Add((Boolean)obj);
                if (obj is Single) _Add((Single)obj);
                if (obj is Double) _Add((Double)obj);
            }
        }

        private void _Add(Double par)
        {
            _Add(BitConverter.GetBytes(par));
        }

        private void _Add(Single par)
        {
            _Add(BitConverter.GetBytes(par));
        }

        private void _Add(Boolean par)
        {
            _Add(BitConverter.GetBytes(par));
        }

        private void _Add(Int16 par)
        {
            _Add(BitConverter.GetBytes(par));
        }

        private void _Add(Int32 par)
        {
            _Add(BitConverter.GetBytes(par));
        }

        private void _Add(Int64 par)
        {
            _Add(BitConverter.GetBytes(par));
        }

        private void _Add(UInt16 par)
        {
            _Add(BitConverter.GetBytes(par));
        }

        private void _Add(UInt32 par)
        {
            _Add(BitConverter.GetBytes(par));
        }

        private void _Add(UInt64 par)
        {
            _Add(BitConverter.GetBytes(par));
        }

        private void _Add(String par)
        {
            _Add(par.Length);
            _Add(Encoding.Default.GetBytes(par));
        }

        private void _Add(byte[] bytearray)
        {
            bytes.Add(bytearray);
        }

        /// <summary>
        /// Converts all data and returns as one byte array.
        /// </summary>
        /// <returns>Returns all data as one byte array.</returns>
        public byte[] GetArray()
        {
            byte[] ret = new byte[size];
            int index=0;

            foreach (byte[] ba in bytes)
            {
                ba.CopyTo(ret, index+ba.Length);
            }

            return ret;
        }

        public int GetSize()
        {
            return size;
        }

        public void Clear()
        {
            bytes.Clear();
            size = 0;
        }
    }
}
