using System.Runtime.InteropServices;
using System.Text;

namespace NES_Disassembler.Classes
{
    public class Program
    {
        // The file in question to disassemble
        static List<byte>? binaryFile;

        static void Main(string[] args)
        {
            // Read NES binary file
            try
            {
                binaryFile = FileReadWrite.ReadFileData(args[0]);
                //binaryFile = FileReadWrite.ReadFileData("Friday the 13th.nes");
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine("Invalid nes file passed as argument. Closing...");
                Environment.Exit(0);
            }

            // If we were able to read the NES binary file
            if (binaryFile != null)
            {
                // Get the iNES header (if it exists)
                // Check the iNES header (4E 45 53 1A)
                if(AssemblyInstructions.CheckIfHeaderIsValid(binaryFile))
                {
                    // For the first pass, we just want to get all the assembly instructions, line by line (ignore any jumps, subroutines, etc.)
                    List<string> rawAssemblyInstructionsList = AssemblyInstructions.DisassembleFile(binaryFile, false);

                    // TODO:
                    // For the second pass, gather most of the labels (Reset label, subroutines, branches, functions, IRQ)
                    // Pass in the raw assembly instructions for the first argument. For the second argument, pass in the address location of the assembly instruction.
                    List<string> nonDataLabels = AssemblyInstructions.GetNonDataLabels(binaryFile);

                    // TODO:
                    // Once we have a list of the other labels on the third and final pass, we can follow the subroutines, branches and jumps to outline where the data is
                    // (Data is locations that the program counter never reaches normally to interpret as instructions)
                    List<string> dataLabels = AssemblyInstructions.GetDataLabels(binaryFile);

                    // Get the final disassembled strings in a list of lines by combining them all together
                    List<string> outputFileStringList = rawAssemblyInstructionsList; // + header stuff + nonDataLabels + dataLabels

                    // Write the disassembled strings to console
                    foreach (string s in outputFileStringList)
                        if (s != "")
                            Console.WriteLine(s);
                }
                else
                    Console.WriteLine("Invalid nes file passed as argument (No correctly formatted iNES file header visible). Closing...");
            }
            else
                Console.WriteLine("Invalid nes file passed as argument. Closing...");
        }
    }
}
