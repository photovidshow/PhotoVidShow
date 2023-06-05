using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ManagedCore
{
    public delegate void UserMessageCallBackDelegate(string text, string caption);

    //
    // This class represents how a program reports messages to the user. By default is via a windows form messagebox,
    // but this can be turned off and/or callbacks added to the singleton instance of this class
    //
    public class UserMessage
    {
        private static UserMessage instance;
        private bool showAsMessageBox = true;

        public event UserMessageCallBackDelegate MessageShown;

        private UserMessage()
        {
        }

        public bool ShowAsMessageBox
        {
            get { return showAsMessageBox; }
            set { showAsMessageBox = value; }
        }

        public static UserMessage GetInstance()
        {
            if (instance == null)
            {
                instance = new UserMessage();
            }
            return instance;
        }

        public static DialogResult Show(string text, string caption)
        {
            return GetInstance().ShowInternal(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return GetInstance().ShowInternal(null, text, caption, buttons, icon);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption)
        {
            return GetInstance().ShowInternal(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return GetInstance().ShowInternal(owner, text, caption, buttons, icon);
        }

        private DialogResult ShowInternal(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            if (MessageShown != null)
            {
                MessageShown(text, caption);
            }

            if (showAsMessageBox == true)
            {
                if (owner != null)
                {
                    return MessageBox.Show(owner, text, caption, buttons, icon);
                }

                return MessageBox.Show(text, caption, buttons, icon);
            }

            return DialogResult.OK;
        }
    }
}
