using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NES_Disassembler.Classes
{
    static public class FileReadWrite
    {
        static public List<byte> ReadFileData(string fileName)
        {
            List<byte> bytes = new List<byte>();
            Stream? stream = null;
            BinaryReader? binaryReader = null;

            try
            {
                // Get the file length
                ulong length = (ulong)new FileInfo(fileName).Length;

                // Open the stream
                stream = File.Open(fileName, FileMode.Open);

                // Open the binary reader
                binaryReader = new BinaryReader(stream);

                // Read the file
                for (ulong i = 0; i < length; i++)
                    bytes.Add(binaryReader.ReadByte());
            }

            // Catch errors trying to open the file or the binary reader
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }

            // Close the stream and binary reader if they are open
            finally
            {
                if (stream != null)
                    stream.Close();

                if (binaryReader != null)
                    binaryReader.Close();
            }

            return bytes;
        }

        static public string BuildTextFile(List<byte> binaryFile, string instructionString, int operandLength, uint pc)
        {
            if (instructionString == "")
                return "";

            // Calculate how many spaces to append to the instructino before the comment at the end
            int numOfSpaces = 33 - instructionString.Length;
            string s = instructionString;

            for (int i = 0; i < numOfSpaces; i++)
                s += " ";

            // Append program counter
            s += "; $" + $"{0x8000 + (int)pc:X}  ";

            // Append byte code
            if (binaryFile[(int)pc + 0x10] <= 0xF)
                s += $"0{binaryFile[(int)pc + 0x10]:X}";
            else
                s += $"{binaryFile[(int)pc + 0x10]:X}";

            if (operandLength >= 1 && (int)pc + 0x10 + 0x1 < binaryFile.Count)
            {
                if (binaryFile[(int)pc + 0x10 + 0x1] <= 0xF)
                    s += $" 0{binaryFile[(int)pc + 0x10 + 0x1]:X}";
                else
                    s += $" {binaryFile[(int)pc + 0x10 + 0x1]:X}";
            }

            if (operandLength == 2 && (int)pc + 0x10 + 0x2 < binaryFile.Count)
            {
                if (binaryFile[(int)pc + 0x10 + 0x2] <= 0xF)
                    s += $" 0{binaryFile[(int)pc + 0x10 + 0x2]:X}";
                else
                    s += $" {binaryFile[(int)pc + 0x10 + 0x2]:X}";
            }

            return s;
        }

    }
}
