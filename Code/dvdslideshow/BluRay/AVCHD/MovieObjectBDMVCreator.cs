using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DVDSlideshow.BluRay.AVCHD
{
    public class MovieObjectBDMVCreator
    {
        private List<MOBJObject> mobjObjects = new List<MOBJObject>();

        private const uint MOBJ_SIG1 = 0x4a424f4d;  // 'MOBJ'
        private const uint MOBJ_SIG_2A = 0x30303230; // 0200

        private void writeHeader(BinaryWriter writter)
        {
            writter.Write(MOBJ_SIG1);
            writter.Write(MOBJ_SIG_2A);

            uint extensionBlank = 0;
            for (int i = 0; i < 8; i++)
            {
                writter.Write(extensionBlank);
            }
        }

        private void CreateObjects()
        {
            mobjObjects.Clear();

            MOBJCommand command1Object1 = new MOBJCommand(new HDMVInstruction(0x21810000), 0, 0x1);
            List<MOBJCommand> commandsObject1 = new List<MOBJCommand>();
            commandsObject1.Add(command1Object1);
            MOBJObject obj1 = new MOBJObject(commandsObject1, false, false, false);
            mobjObjects.Add(obj1);


            MOBJCommand command1Object2 = new MOBJCommand(new HDMVInstruction(0x21810000), 0, 0x1);
            List<MOBJCommand> commandsObject2 = new List<MOBJCommand>();
            commandsObject2.Add(command1Object2);
            MOBJObject obj2 = new MOBJObject(commandsObject2, false, false, false);
            mobjObjects.Add(obj2);

            /*
             * Old way 1 movie HDMV from sintel ?
             * 
            MOBJCommand command1Object1 = new MOBJCommand(new HDMVInstruction(0x50000001), 0, 0xa);
            MOBJCommand command2Object1 = new MOBJCommand(new HDMVInstruction(0x50400001), 0, 0);
            MOBJCommand command3Object1 = new MOBJCommand(new HDMVInstruction(0x50400001), 0, 0);
            MOBJCommand command4Object1 = new MOBJCommand(new HDMVInstruction(0x42820000), 0xa, 0);
            MOBJCommand command5Object1 = new MOBJCommand(new HDMVInstruction(0x21810000), 0, 0x1);
            List<MOBJCommand> commandsObject1 = new List<MOBJCommand>();
            commandsObject1.Add(command1Object1);
            commandsObject1.Add(command2Object1);
            commandsObject1.Add(command3Object1);
            commandsObject1.Add(command4Object1);
            commandsObject1.Add(command5Object1);
            MOBJObject obj1 = new MOBJObject(commandsObject1, true, false, false);
            mobjObjects.Add(obj1);

            MOBJCommand command1Object2 = new MOBJCommand(new HDMVInstruction(0x50000001), 3, 0xa);
            MOBJCommand command2Object2 = new MOBJCommand(new HDMVInstruction(0x50400001), 0xffff, 3);
            MOBJCommand command3Object2 = new MOBJCommand(new HDMVInstruction(0x48400300), 0xffff, 0xa);
            MOBJCommand command4Object2 = new MOBJCommand(new HDMVInstruction(0x22000000), 0, 0xa);
            MOBJCommand command5Object2 = new MOBJCommand(new HDMVInstruction(0x50000001), 4, 0xa);
            MOBJCommand command6Object2 = new MOBJCommand(new HDMVInstruction(0x50400001), 0, 0x4);
            MOBJCommand command7Object2 = new MOBJCommand(new HDMVInstruction(0x48400300), 0, 0xa);
            MOBJCommand command8Object2 = new MOBJCommand(new HDMVInstruction(0x21010000), 0, 0xa);
            MOBJCommand command9Object2 = new MOBJCommand(new HDMVInstruction(0x21810000), 0, 0x1);
            List<MOBJCommand> commandsObject2 = new List<MOBJCommand>();
            commandsObject2.Add(command1Object2);
            commandsObject2.Add(command2Object2);
            commandsObject2.Add(command3Object2);
            commandsObject2.Add(command4Object2);
            commandsObject2.Add(command5Object2);
            commandsObject2.Add(command6Object2);
            commandsObject2.Add(command7Object2);
            commandsObject2.Add(command8Object2);
            commandsObject2.Add(command9Object2);
            MOBJObject obj2 = new MOBJObject(commandsObject2, true, false, false);
            mobjObjects.Add(obj2);

            MOBJCommand command1Object3 = new MOBJCommand(new HDMVInstruction(0x50400001), 0, 0);
            MOBJCommand command2Object3 = new MOBJCommand(new HDMVInstruction(0x50400001), 1, 2);
            MOBJCommand command3Object3 = new MOBJCommand(new HDMVInstruction(0x50400001), 0xffff, 3);
            MOBJCommand command4Object3 = new MOBJCommand(new HDMVInstruction(0x50400001), 0, 4);
            MOBJCommand command5Object3 = new MOBJCommand(new HDMVInstruction(0x21810000), 0, 0);    
            List<MOBJCommand> commandsObject3 = new List<MOBJCommand>();
            commandsObject3.Add(command1Object3);
            commandsObject3.Add(command2Object3);
            commandsObject3.Add(command3Object3);
            commandsObject3.Add(command4Object3);
            commandsObject3.Add(command5Object3);
            MOBJObject obj3 = new MOBJObject(commandsObject3, true, false, false);
            mobjObjects.Add(obj3);
            */

        }

        public bool CreateMovieObjectBDMV(string folder)
        {
            CreateObjects();

            string file = folder + "\\MovieObject.bdmv";
            try
            {
                FileStream stream = new FileStream(file, FileMode.CreateNew);

                try
                {
                    using (BinaryWriter writter = new BinaryWriter(stream))
                    {
                        writeHeader(writter);

                        // Write length remaining
                        int length = 6;

                        foreach (MOBJObject obj in mobjObjects)
                        {
                            length += obj.GetLengthInBytes();
                        }
                        writter.Write(EndianSwap.Swap((uint)length));

                        // next 32 bits blank
                        uint reserved = 0;
                        writter.Write(reserved);

                        // number of objects
                        ushort numObjects = (ushort) mobjObjects.Count;
                        writter.Write(EndianSwap.Swap(numObjects));

                        foreach (MOBJObject obj in mobjObjects)
                        {
                            obj.Write(writter);
                        }
                    }
                }
                finally
                {
                    stream.Close();
                }
            }
            catch (Exception exception)
            {
                AVCHDLog.Error("Failed to create '" + file + "' " + exception.Message);
                return false;
            }

            return true;

        }

    }
}
