using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.ISO.UDF
{
    /// <summary>
    /// This class represents a ECMA EntityID
    /// </summary>
    public class EntityID
    {
        //
        // ECMA 167 1/7.4 
        //
        private byte Flags = 0;
        private byte [] Identifier = new byte[23];
        private byte [] IdentifierSuffix = new byte[8];

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="indentifier"></param>
        /// <param name="version"> version should be like 0x250 for version 2.5</param>
        public EntityID(string indentifier, ushort version, bool setDomainFlags)
        {
            byte[] b2 = System.Text.Encoding.ASCII.GetBytes(indentifier);
            int toCopy = b2.Length;
            if (b2.Length > Identifier.Length)
            {
                UDFLog.Warning("Identifier too big for EntityID container. Truncating");
                toCopy = Identifier.Length;
            }

            System.Buffer.BlockCopy(b2, 0, Identifier, 0, toCopy);

            if (version != 0)
            {
                IdentifierSuffix[0] = (byte) (version & 255);
                IdentifierSuffix[1] = (byte) ((version >> 8) & 255);     
            }

            if (setDomainFlags)
            {
                //
                // The OSTA Domain Identifiers are only used in
                // the Logical Volume Descriptor and the File Set Descriptor. The DomainFlags
                // field defines the following bit flags:
                // Domain Flags
                // Bit Description
                // 0 Hard Write-Protect
                // 1 Soft Write-Protect
                //
                IdentifierSuffix[2] = 3;
            }
        }

        /// <summary>
        /// Write the EntityID to the given binary writer
        /// </summary>
        /// <param name="writer"></param>
        public void Write(BinaryWriter writer)
        {
            writer.Write(Flags);
            writer.Write(Identifier);
            writer.Write(IdentifierSuffix);
        }
    }
}
