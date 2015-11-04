using System;
using System.Collections.Generic;
using System.IO;

namespace eNetwork3.Utils
{
    /// <summary>
    /// Packet writer
    /// </summary>
    public class PacketWriter : IDisposable
    {

        private List<Byte> buffer;
        public int Position { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public PacketWriter()
        {
            buffer = new List<Byte>();
            Position = 0;
        }

        /// <summary>
        /// Write an int16
        /// </summary>
        /// <param name="value">Value</param>
        public void WriteInt16(Int16 value)
        {
            buffer.Add((Byte)value);
            buffer.Add((Byte)(value >> 8));
            Position += 2;
        }

        /// <summary>
        /// Overwrite an int16, dont forget to modify position before
        /// </summary>
        /// <param name="value">Value</param>
        public void OverWriteInt16(Int16 value)
        {
            if (Position + 1 < buffer.Count)
            {
                buffer[Position] = (Byte)value;
                buffer[Position + 1] = (Byte)(value >> 8);
                Position += 2;
            }
            else
            {
                throw new IndexOutOfRangeException("Canno't overwrite at this position.");
            }
        }

        /// <summary>
        /// Write an int32
        /// </summary>
        /// <param name="value">Value</param>
        public void WriteInt32(Int32 value)
        {
            buffer.Add((Byte)value);
            buffer.Add((Byte)(value >> 8));
            buffer.Add((Byte)(value >> 16));
            buffer.Add((Byte)(value >> 24));
            Position += 4;
        }

        /// <summary>
        /// Overwrite an int32, dont forget to modify position before
        /// </summary>
        /// <param name="value">Value</param>
        public void OverWriteInt32(Int32 value)
        {
            if (Position + 3 < buffer.Count)
            {
                buffer[Position] = (Byte)value;
                buffer[Position + 1] = (Byte)(value >> 8);
                buffer[Position + 2] = (Byte)(value >> 16);
                buffer[Position + 3] = (Byte)(value >> 24);
                Position += 4;
            }
            else
            {
                throw new IndexOutOfRangeException("Canno't overwrite at this position.");
            }
        }

        /// <summary>
        /// Write an int64
        /// </summary>
        /// <param name="value">Value</param>
        public void WriteInt64(Int64 value)
        {
            buffer.Add((Byte)value);
            buffer.Add((Byte)(value >> 8));
            buffer.Add((Byte)(value >> 16));
            buffer.Add((Byte)(value >> 24));
            buffer.Add((Byte)(value >> 32));
            buffer.Add((Byte)(value >> 40));
            buffer.Add((Byte)(value >> 48));
            buffer.Add((Byte)(value >> 56));
            Position += 8;
        }

        /// <summary>
        /// Overwrite an int64, dont forget to modify position before
        /// </summary>
        /// <param name="value">Value</param>
        public void OverWriteInt64(Int64 value)
        {
            if (Position + 7 < buffer.Count)
            {
                buffer[Position] = (Byte)value;
                buffer[Position + 1] = (Byte)(value >> 8);
                buffer[Position + 2] = (Byte)(value >> 16);
                buffer[Position + 3] = (Byte)(value >> 24);
                buffer[Position + 4] = (Byte)(value >> 32);
                buffer[Position + 5] = (Byte)(value >> 40);
                buffer[Position + 6] = (Byte)(value >> 48);
                buffer[Position + 7] = (Byte)(value >> 56);
                Position += 8;
            }
            else
            {
                throw new IndexOutOfRangeException("Canno't overwrite at this position.");
            }
        }

        /// <summary>
        /// Write a boolean
        /// </summary>
        /// <param name="value">Value</param>
        public void WriteBoolean(Boolean value)
        {
            buffer.Add((Byte)(value == true ? 1 : 0));
            Position++;
        }

        /// <summary>
        /// Overwrite a boolean, dont forget to modify position before
        /// </summary>
        /// <param name="value">Value</param>
        public void OverWriteBoolean(Boolean value)
        {
            if (Position < buffer.Count)
            {
                buffer[Position] = (Byte)(value == true ? 1 : 0);
                Position++;
            }
            else
            {
                throw new IndexOutOfRangeException("Canno't overwrite at this position.");
            }
        }

        /// <summary>
        /// Write a byte
        /// </summary>
        /// <param name="value">Value</param>
        public void WriteByte(Byte value)
        {
            buffer.Add(value);
            Position++;
        }

        /// <summary>
        /// Overwrite a byte, dont forget to modify position before
        /// </summary>
        /// <param name="value">Value</param>
        public void OverWriteByte(Byte value)
        {
            if (Position < buffer.Count)
            {
                buffer[Position] = value;
                Position++;
            }
            else
            {
                throw new IndexOutOfRangeException("Canno't overwrite at this position.");
            }
        }

        /// <summary>
        /// Write a char
        /// </summary>
        /// <param name="value">Value</param>
        public void WriteChar(Char value)
        {
            buffer.Add((Byte)value);
            Position++;
        }

        /// <summary>
        /// Overwrite a char, dont forget to modify position before
        /// </summary>
        /// <param name="value">Value</param>
        public void OverWriteChar(Char value)
        {
            if (Position < buffer.Count)
            {
                buffer[Position] = (Byte)value;
                Position++;
            }
            else
            {
                throw new IndexOutOfRangeException("Canno't overwrite at this position.");
            }
        }

        /// <summary>
        /// Write a decimal
        /// </summary>
        /// <param name="value">Value</param>
        public void WriteDecimal(Decimal value)
        {
            Byte[] tempBuffer = new Byte[16];
            using (MemoryStream ms = new MemoryStream(tempBuffer))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(value);
                }
            }
            for (int i = 0; i < 16; i++)
            {
                buffer.Add(tempBuffer[i]);
            }
            Position += 16;
        }

        /// <summary>
        /// Overwrite a decimal, dont forget to modify position before
        /// </summary>
        /// <param name="value">Value</param>
        public void OverWriteDecimal(Decimal value)
        {
            if (Position + 16 < buffer.Count)
            {
                Byte[] tempBuffer = new Byte[16];
                using (MemoryStream ms = new MemoryStream(tempBuffer))
                {
                    using (BinaryWriter bw = new BinaryWriter(ms))
                    {
                        bw.Write(value);
                    }
                }
                for (int i = 0; i < 16; i++)
                {
                    buffer[Position + i] = tempBuffer[i];
                }
                Position += 16;
            }
            else
            {
                throw new IndexOutOfRangeException("Canno't overwrite at this position.");
            }
        }

        /// <summary>
        /// Write a double
        /// </summary>
        /// <param name="value">Value</param>
        public void WriteDouble(Double value)
        {
            Byte[] tempBuffer = new Byte[8];
            using (MemoryStream ms = new MemoryStream(tempBuffer))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(value);
                }
            }
            for (int i = 0; i < 8; i++)
            {
                buffer.Add(tempBuffer[i]);
            }
            Position += 8;
        }

        /// <summary>
        /// Overwrite a double, dont forget to modify position before
        /// </summary>
        /// <param name="value">Value</param>
        public void OverWriteDouble(Double value)
        {
            if (Position + 8 < buffer.Count)
            {
                Byte[] tempBuffer = new Byte[8];
                using (MemoryStream ms = new MemoryStream(tempBuffer))
                {
                    using (BinaryWriter bw = new BinaryWriter(ms))
                    {
                        bw.Write(value);
                    }
                }
                for (int i = 0; i < 8; i++)
                {
                    buffer[Position + i] = tempBuffer[i];
                }
                Position += 8;
            }
            else
            {
                throw new IndexOutOfRangeException("Canno't overwrite at this position.");
            }
        }

        /// <summary>
        /// Write a single
        /// </summary>
        /// <param name="value">Value</param>
        public void WriteSingle(Single value)
        {
            Byte[] tempBuffer = new Byte[4];
            using (MemoryStream ms = new MemoryStream(tempBuffer))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(value);
                }
            }
            for (int i = 0; i < 4; i++)
            {
                buffer.Add(tempBuffer[i]);
            }
            Position += 4;
        }

        /// <summary>
        /// Overwrite a single, dont forget to modify position before
        /// </summary>
        /// <param name="value">Value</param>
        public void OverWriteSingle(Single value)
        {
            if (Position + 4 < buffer.Count)
            {
                Byte[] tempBuffer = new Byte[4];
                using (MemoryStream ms = new MemoryStream(tempBuffer))
                {
                    using (BinaryWriter bw = new BinaryWriter(ms))
                    {
                        bw.Write(value);
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    buffer[Position + i] = tempBuffer[i];
                }
                Position += 4;
            }
            else
            {
                throw new IndexOutOfRangeException("Canno't overwrite at this position.");
            }
        }

        /// <summary>
        /// Write a string
        /// </summary>
        /// <param name="value">Value</param>
        public void WriteString(String value)
        {
            WriteInt16((Int16)value.Length);
            foreach (Char c in value)
            {
                WriteChar(c);
            }
        }

        /// <summary>
        /// Overwrite a string, dont forget to modify position before
        /// </summary>
        /// <param name="value">Value</param>
        public void OverWriteString(String value)
        {
            if (Position + value.ToCharArray().Length + 2 <= buffer.Count)
            {
                OverWriteInt16((Int16)value.Length);
                foreach (Char c in value)
                {
                    OverWriteChar(c);
                }
            }
            else
            {
                throw new IndexOutOfRangeException("Canno't overwrite at this position.");
            }
        }

        /// <summary>
        /// Get the byte array from the PacketWriter
        /// </summary>
        /// <returns>Byte array</returns>
        public Byte[] ToArray()
        {
            return buffer.ToArray();
        }

        /// <summary>
        /// Dispose the PacketWriter, not necessary
        /// </summary>
        public void Dispose()
        {
            buffer.Clear();
            buffer = null;
            Position = 0;
        }

    }
}
