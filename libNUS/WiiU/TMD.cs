using System;
using System.Collections.Generic;
using System.Linq;

namespace libNUS.WiiU
{
    public struct TMDContent
    {
        public readonly Byte[] ID;
        public readonly string IDString;
        public readonly ushort Type;
        public readonly UInt32 Size;
        public readonly bool HasH3;

        public TMDContent(Byte[] contentBytes)
        {
            if (contentBytes.Length != 0x30)
                throw new ArgumentOutOfRangeException("contentBytes", contentBytes, "Expected 48 bytes, received " + contentBytes.Length);

            this.ID = contentBytes.Take(0x04).ToArray();
            this.IDString = HelperFunctions.ByteArrayToHexString(ID);
            this.Type = BitConverter.ToUInt16(contentBytes.Skip(0x06).Take(2).Reverse().ToArray(), 0);
            this.Size = BitConverter.ToUInt32(contentBytes.Skip(0x08).Take(8).Reverse().ToArray(),0);
            this.HasH3 = (this.Type & 0x2) > 0;
        }
    }
    public struct TMD
    {
        private const Int16 tk = 0x140;
        private const Int16 contentStart = 0xB04;
        private const byte contentSize = 0x30;

        public readonly TMDContent[] Content;
        public readonly UInt16 TitleVersion;
        public readonly Int64 TitleContentSize;
        public readonly String TitleID;
        public readonly Byte[] rawBytes;

        public TMD(byte[] tmdBytes)
        {
            TitleContentSize = 0;
            Int16 contentCount = BitConverter.ToInt16(tmdBytes.Skip(tk + 0x9E).Take(0x02).Reverse().ToArray(), 0);
            var content = new List<TMDContent> { };
            for (var i = 0; i < contentCount; i++)
            {
                TMDContent tmdContent = new TMDContent(tmdBytes.Skip(contentStart + (i * contentSize)).Take(contentSize).ToArray());
                content.Add(tmdContent);
                TitleContentSize += tmdContent.Size;
            }

            this.rawBytes = tmdBytes;
            this.TitleVersion = BitConverter.ToUInt16(tmdBytes.Skip(tk + 0x9C).Take(0x02).Reverse().ToArray(), 0);
            this.TitleID = HelperFunctions.ByteArrayToHexString(tmdBytes.Skip(0x18C).Take(0x08).ToArray());
            this.Content = content.ToArray();
        }
        public TMD(bool o)
        {
            Content = new TMDContent[] { };
            TitleVersion = 0;
            TitleContentSize = 0;
            TitleID = "";
            rawBytes = new Byte[] { };
        }
    }
}
