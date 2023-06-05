using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace DVDSlideshow
{
    //
    // This class represents an interface to a class that will contain a TextDecoration within it.
    // This is useful because many "text editable" decorations exist that are not CTextDecorations objects.  E.g. menu buttons which contain text.
    // Because they implement this interface, they can still have their text be editable in the editor.
    //
    public interface ITextDecorationContainer
    {
        CTextDecoration TextDecoration
        {
            get;
        }
    }
}
