using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace DVDSlideshow
{
    public struct CRectangleReferenceFrame
    {
        public CRectangleReferenceFrame(RectangleF position, float rotation , float xRotOfsset, float yRotOffset, RectangleF UVCoords, float UVRotation, float UVxRotOffset, float UVyRotOffset )
        {
            this.position = position;
           
            this.rotation = rotation;
            offsetRotX = xRotOfsset;
            offsetRoyY = yRotOffset;

            this.UVCoords = UVCoords;
            this.UVRotation = UVRotation;
            this.UVOffsetRotX = UVxRotOffset;
            this.UVOffsetRoyY = UVyRotOffset;
        }

        public RectangleF position;
        public float rotation;
        //
        // if these are zero, rotates around centre of rec
        //
        public float offsetRotX;
        public float offsetRoyY;

        //
        // Source uv cords
        //
        public RectangleF UVCoords;
        public float UVRotation;
        public float UVOffsetRotX;
        public float UVOffsetRoyY;
    }
}
