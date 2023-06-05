using System;
using System.Drawing;

namespace CustomButton
{
	/// <summary>
	/// Summary description for FrontEndGlobals.
	/// </summary>
	public class FrontEndGlobals
	{

		public static Color mThumbnailBorderMouseHoverColour = System.Drawing.SystemColors.ControlDark;
		public static Color mThumbnailBorderNormalColour = System.Drawing.SystemColors.ControlLight ;
        public static Color mThumbnailSelectColour = Color.FromArgb(60, 95, 134);

		public static string mApplicationName ="PhotoVidShow";
        public static bool mShowSelectedMotionDecoration = false;
	}
}
