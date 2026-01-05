using System.Text;

namespace NES_Disassembler
{
    internal class Program
    {
        /*
         * Registers
         */

        // Accumulator
        static byte accumulator;

        // Indexes
        static byte indexX;
        static byte indexY;

        // Program counter (PC)
        static uint PC;

        // Stack pointer (SP)
        static byte SP;

        // PRG ROM size

        // CHR ROM size

        // Status register (processor status, flags, P)
        // 7  bit  0
        // ---- ----
        // NV1B DIZC
        // From most significant bit to least significant bit:
        // N = Negative
        // V = Overflow
        // 1 = (No CPU effect, always pushed as 1)
        // B = (No CPU effect, B flag)

        // D = Decimal
        // I = Interrupt Disable
        // Z = Zero
        // C = Carry
        static byte statusRegister; // NV1B DIZC

        // The file in question to disassemble
        static List<byte>? binaryFile;

        // Length of operand after the opcode (0-2 bytes)
        static byte operandLength;

        // Number of 16 KB blocks of PRG ROM
        static byte numberOfPRGROMBlocks;

        // Number of 8 KB blocks of CHR ROM
        static byte numberOfCHRROMBlocks;

        // Subroutine starting addresses
        static List<int>? subroutineStartingAddresses;

        enum AddressingMode
        {
            Implied,
            Accumulator,
            Immediate,
            ZeroPage,
            ZeroPageX,
            ZeroPageY,
            Absolute,
            AbsoluteX,
            AbsoluteY,
            Indirect,
            IndirectX,
            IndirectY,
            Relative,
        }

        static string UseAddressingMode( AddressingMode a, List<Byte> bF, uint pc)
        {
            
            //String s = "          ; ";
            String s = "";

            String opcodeString = "";

            switch (a)
            {
                case AddressingMode.Implied:
                    break;

                case AddressingMode.Accumulator:
                    //s += "Accumulator";
                    break;
                case AddressingMode.Immediate:
                    //s += "#Immediate";
                    opcodeString += "#$";
                    operandLength = 1;
                    break;
                case AddressingMode.ZeroPage:
                    //s += "Zero Page";
                    operandLength = 1;
                    opcodeString += "z:$";
                    break;
                case AddressingMode.ZeroPageX:
                    //s += "Zero Page, X";
                    operandLength = 1;
                    opcodeString += "z:$";
                    break;
                case AddressingMode.ZeroPageY:
                    //s += "Zero Page, Y";
                    operandLength = 1;
                    opcodeString += "z:$";
                    break;
                case AddressingMode.Absolute:
                    //s += "Absolute";
                    operandLength = 2;
                    opcodeString += "a:$";
                    break;
                case AddressingMode.AbsoluteX:
                    //s += "Absolute, X";
                    operandLength = 2;
                    opcodeString += "a:$";
                    break;
                case AddressingMode.AbsoluteY:
                    //s += "Absolute, Y";
                    operandLength = 2;
                    opcodeString += "a:$";
                    break;
                case AddressingMode.Indirect:
                    operandLength = 2;
                    opcodeString += "($";
                    break;
                case AddressingMode.IndirectX:
                    //s += "(Indirect, X)";
                    opcodeString += "($";
                    operandLength = 1;
                    break;
                case AddressingMode.IndirectY:
                    //s += "(Indirect), Y";
                    opcodeString += "($";
                    operandLength = 1;
                    break;
                case AddressingMode.Relative:
                    //s += "Relative";
                    operandLength = 1;
                    opcodeString += "$";
                    break;
            }

            /*if(bF[(int)PC + 0x10] < 0x10)
                opcodeString += $"0{bF[(int)PC + 0x10]:X}";
            else
                opcodeString += $"{bF[(int)PC + 0x10]:X}";*/

            if (operandLength == 1)
            {
                if (bF[(int)PC + 0x10 + 0x1] < 0x10)
                    opcodeString += $"0{bF[(int)PC + 0x10 + 0x1]:X}";
                else
                    opcodeString += $"{bF[(int)PC + 0x10 + 0x1]:X}";
            }
                

            //if(operandLength == 2)
                //opcodeString += $" {bF[(int)PC + 0x10 + 0x2]:X}";

            // 6502 is little endian, so reverse the bytes when the operand is 2 bytes long
            if (operandLength == 2)
            {
                if (bF[(int)PC + 0x10 + 0x2] < 0x10)
                    opcodeString += $"0{bF[(int)PC + 0x10 + 0x2]:X}";
                else
                    opcodeString += $"{bF[(int)PC + 0x10 + 0x2]:X}";

                /*if (bF[(int)PC + 0x10 + 0x1] < 0x10)
                    opcodeString += $" $0{bF[(int)PC + 0x10 + 0x1]:X}";
                else
                    opcodeString += $" ${bF[(int)PC + 0x10 + 0x1]:X}";*/

                if (bF[(int)PC + 0x10 + 0x1] < 0x10)
                    opcodeString += $"0{bF[(int)PC + 0x10 + 0x1]:X}";
                else
                    opcodeString += $"{bF[(int)PC + 0x10 + 0x1]:X}";
            }

            //return s;

            //if (a == AddressingMode.ZeroPage)
                //opcodeString += " zp";

            if (a == AddressingMode.ZeroPageX)
                opcodeString += ", X";

            else if (a == AddressingMode.ZeroPageY)
                opcodeString += ", Y";

            //else if (a == AddressingMode.Absolute)
                //opcodeString += " abs";

            else if (a == AddressingMode.AbsoluteX)
                opcodeString += ", X";

            else if (a == AddressingMode.AbsoluteY)
                opcodeString += ", Y";

            if (a == AddressingMode.Indirect)
                opcodeString += ")";

            else if (a == AddressingMode.IndirectX)
                opcodeString += ", X)";

            else if (a == AddressingMode.IndirectY)
                opcodeString += "), Y";

            return opcodeString;
        }

        /*
         * Assembly functions
         */

        static string ADC(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "ADC: Add with Carry using " + UseAddressingMode(a);
            return "ADC " + UseAddressingMode(a, bF, pc);
        }

        static string AND(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "AND: Bitwise AND with " + UseAddressingMode(a);
            return "AND " + UseAddressingMode(a, bF, pc);
        }

        static string ASL(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "ASL: Arithmetic Shift Left of the " + UseAddressingMode(a);
            return "ASL " + UseAddressingMode(a, bF, pc);
        }

        static string BCC(AddressingMode a, List<Byte> bF, uint pc)
        {
            return "BCC " + UseAddressingMode(a, bF, pc);
        }

        static string BCS(AddressingMode a, List<Byte> bF, uint pc)
        {
            return "BCS " + UseAddressingMode(a, bF, pc);
        }

        static string BEQ(AddressingMode a, List<Byte> bF, uint pc)
        {
            return "BEQ " + UseAddressingMode(a, bF, pc);
        }

        static string BIT(AddressingMode a, List<Byte> bF, uint pc)
        {
            return "BIT " + UseAddressingMode(a, bF, pc);
        }

        static string BMI(AddressingMode a, List<Byte> bF, uint pc)
        {
            return "BMI " + UseAddressingMode(a, bF, pc);
        }

        static string BNE(AddressingMode a, List<Byte> bF, uint pc)
        {
            return "BNE " + UseAddressingMode(a, bF, pc);
        }

        static string BPL(AddressingMode a, List<Byte> bF, uint pc)
        {
            return "BPL " + UseAddressingMode(a, bF, pc);
        }

        static string BVC(AddressingMode a, List<Byte> bF, uint pc)
        {
            return "BVC " + UseAddressingMode(a, bF, pc);
        }

        static string BVS(AddressingMode a, List<Byte> bF, uint pc)
        {
            return "BVS " + UseAddressingMode(a, bF, pc);
        }

        static string CLC(AddressingMode a, List<Byte> bF, uint pc)
        {
            return "CLC " + UseAddressingMode(a, bF, pc);
        }

        static string CLD(AddressingMode a, List<Byte> bF, uint pc)
        {
            return "CLD" + UseAddressingMode(a, bF, pc);
        }

        static string CLI(AddressingMode a, List<Byte> bF, uint pc)
        {
            return "CLI " + UseAddressingMode(a, bF, pc);
        }

        static string CLV(AddressingMode a, List<Byte> bF, uint pc)
        {
            return "CLV " + UseAddressingMode(a, bF, pc);
        }

        static string CMP(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "CMP: Compare Accumulator to " + UseAddressingMode(a);
            return "CMP " + UseAddressingMode(a, bF, pc);
        }

        static string CPX(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "CPX: Compare X to " + UseAddressingMode(a);
            return "CPX " + UseAddressingMode(a, bF, pc);
        }

        static string CPY(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "CPY: Compare Y to " + UseAddressingMode(a);
            return "CPY " + UseAddressingMode(a, bF, pc);
        }

        static string DEC(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "DEC: Decrement " + UseAddressingMode(a);
            return "DEC " + UseAddressingMode(a, bF, pc);
        }

        static string DEX(AddressingMode a, List<Byte> bF, uint pc)
        {
            return "DEX " + UseAddressingMode(a, bF, pc);
        }

        static string DEY(AddressingMode a, List<Byte> bF, uint pc)
        {
            return "DEY " + UseAddressingMode(a, bF, pc);
        }

        static string EOR(AddressingMode a, List<Byte> bF, uint pc)
        {
            return "EOR " + UseAddressingMode(a, bF, pc);
        }

        static string INC(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "INC: Increment " + UseAddressingMode(a);
            return "INC " + UseAddressingMode(a, bF, pc);
        }

        static string INX(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "INC: Increment " + UseAddressingMode(a);
            return "INX " + UseAddressingMode(a, bF, pc);
        }

        static string INY(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "INC: Increment " + UseAddressingMode(a);
            return "INY" + UseAddressingMode(a, bF, pc);
        }

        static string JMP(AddressingMode a, List<Byte> bF, uint pc)
        {
            return "JMP " + UseAddressingMode(a, bF, pc);
        }

        // Jump to subroutine
        static string JSR(AddressingMode a, List<Byte> bF, uint pc)
        {
            // When we jump to a particular subroutine address, we want to mark it
            // JSR always has an absolute addressing mode, so read the next two bytes and reverse them to get the address
            int b1 = bF[(int)pc + 0x10 + 0x2];
            int b2 = bF[(int)pc + 0x10 + 0x1];
            subroutineStartingAddresses.Add(b1 * 256 + b2);

            return "JSR " + UseAddressingMode(a, bF, pc);
        }

        static string LDA(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "LDA: Load Accumulator using " + UseAddressingMode(a);
            return "LDA " + UseAddressingMode(a, bF, pc);
        }

        static string LDX(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "LDX: Load X using " + UseAddressingMode(a);
            return "LDX " + UseAddressingMode(a, bF, pc);
        }

        static string LDY(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "LDY: Load Y using " + UseAddressingMode(a);
            return "LDY " + UseAddressingMode(a, bF, pc);
        }

        static string LSR(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "LSR: Logical Shift Right of the " + UseAddressingMode(a);
            return "LSR " + UseAddressingMode(a, bF, pc);
        }

        static string NOP(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "LSR: Logical Shift Right of the " + UseAddressingMode(a);
            return "NOP " + UseAddressingMode(a, bF, pc);
        }

        static string ORA(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "ORA: Bitwise OR with " + UseAddressingMode(a);
            return "ORA " + UseAddressingMode(a, bF, pc);
        }

        static string PHP(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "LSR: Logical Shift Right of the " + UseAddressingMode(a);
            return "PHP " + UseAddressingMode(a, bF, pc);
        }

        static string PLA(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "LSR: Logical Shift Right of the " + UseAddressingMode(a);
            return "PLA " + UseAddressingMode(a, bF, pc);
        }

        static string PLP(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "LSR: Logical Shift Right of the " + UseAddressingMode(a);
            return "PLP " + UseAddressingMode(a, bF, pc);
        }

        static string ROL(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "ROL: Rotate Left " + UseAddressingMode(a);
            return "ROL " + UseAddressingMode(a, bF, pc);
        }

        static string ROR(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "ROR: Rotate Right the " + UseAddressingMode(a);
            return "ROR " + UseAddressingMode(a, bF, pc);
        }

        static string RTI(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "ROL: Rotate Left " + UseAddressingMode(a);
            return "RTI " + UseAddressingMode(a, bF, pc);
        }

        static string RTS(AddressingMode a, List<Byte> bF, uint pc)
        {
            // When we return fom a particular subroutine address, we want to mark it
            // RTS always has an implied addressing mode, so use the current program counter
            //subroutineEndingAddresses.Add(pc);

            return "RTS" + UseAddressingMode(a, bF, pc);
        }

        static string SBC(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "SBC: Subtract with Carry using " + UseAddressingMode(a);
            return "SBC " + UseAddressingMode(a, bF, pc);
        }

        static string SEC(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "SEC: Set Carry;
            return "SEC " + UseAddressingMode(a, bF, pc);
        }

        static string SED(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "SEC: Set Carry;
            return "SED " + UseAddressingMode(a, bF, pc);
        }

        static string SEI(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "SEC: Set Carry;
            return "SEI" + UseAddressingMode(a, bF, pc);
        }

        static string STA(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "STA: Store from Accumulator into " + UseAddressingMode(a);
            return "STA " + UseAddressingMode(a, bF, pc);
        }

        static string STX(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "STX: Store from X into " + UseAddressingMode(a);
            return "STX " + UseAddressingMode(a, bF, pc);
        }

        static string STY(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "STY: Store from Y into " + UseAddressingMode(a);
            return "STY " + UseAddressingMode(a, bF, pc);
        }

        static string TAX(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "STY: Store from Y into " + UseAddressingMode(a);
            return "TAX" + UseAddressingMode(a, bF, pc);
        }

        static string TAY(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "STY: Store from Y into " + UseAddressingMode(a);
            return "TAY" + UseAddressingMode(a, bF, pc);
        }

        static string TSX(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "STY: Store from Y into " + UseAddressingMode(a);
            return "TSX" + UseAddressingMode(a, bF, pc);
        }

        static string TSY(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "STY: Store from Y into " + UseAddressingMode(a);
            return "TSY" + UseAddressingMode(a, bF, pc);
        }

        static string TXA(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "STY: Store from Y into " + UseAddressingMode(a);
            return "TXA " + UseAddressingMode(a, bF, pc);
        }

        static string TXS(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "STY: Store from Y into " + UseAddressingMode(a);
            return "TXS " + UseAddressingMode(a, bF, pc);
        }

        static string TYA(AddressingMode a, List<Byte> bF, uint pc)
        {
            //return "STY: Store from Y into " + UseAddressingMode(a);
            return "TYA " + UseAddressingMode(a, bF, pc);
        }

        static List<byte> ReadFileData(string fileName)
        {
            List<byte> bytes = new List<byte>();
            Stream? stream = null;
            BinaryReader? binaryReader = null;

            try
            {
                // Get the file length
                ulong length = (ulong)(new FileInfo(fileName).Length);

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

        static string Decode(List<Byte> bF, uint pc)
        {
            byte b = bF[(int)PC + 0x10];

            // Instruction set
            //string instructionString = "Instruction: ";
            string instructionString = "";

            // Instructions (1 byte length + 0-2 bytes of operands)
            switch (b)
            {
                case 0x00:
                    instructionString += "BRK";

                    // Set the interrupt disable flag to 1
                    //statusRegister = (byte)(statusRegister & 0x00000100);

                    // The return address pushed to the stack skips the byte after the BRK opcode, so it's often considered a 2-byte instruction with an unused immediate
                    operandLength = 1;
                    break;

                case 0x01:
                    instructionString += ORA(AddressingMode.IndirectX, bF, pc);
                    break;

                case 0x02:
                    instructionString += "*KIL";
                    break;

                case 0x03:
                    instructionString += "*SLO";
                    break;

                case 0x04:
                    instructionString += "*NOP";
                    break;

                case 0x05:
                    instructionString += ORA(AddressingMode.ZeroPage, bF, pc);
                    break;

                case 0x06:
                    instructionString += ASL(AddressingMode.ZeroPage, bF, pc);
                    break;

                case 0x07:
                    instructionString += "*SLO";
                    break;

                case 0x08:
                    //instructionString += "PHP: Push Processor Status Register";
                    instructionString += PHP(AddressingMode.Implied, bF, pc);
                    break;

                case 0x09:
                    instructionString += ORA(AddressingMode.Immediate, bF, pc);
                    break;

                case 0x0A:
                    instructionString += ASL(AddressingMode.Accumulator, bF, pc);
                    break;

                case 0x0B:
                    instructionString += "*ANC";
                    break;

                case 0x0C:
                    instructionString += "*NOP";
                    break;

                case 0x0D:
                    instructionString += ORA(AddressingMode.Absolute, bF, pc);
                    break;

                case 0x0E:
                    instructionString += ASL(AddressingMode.Absolute, bF, pc);
                    break;

                case 0x0F:
                    instructionString += "*SLO";
                    break;

                case 0x10:
                    //instructionString += "BPL: Branch if Plus Relative";
                    instructionString += BPL(AddressingMode.Relative, bF, pc);
                    break;

                case 0x11:
                    instructionString += ORA(AddressingMode.IndirectY, bF, pc);
                    break;

                case 0x12:
                    instructionString += "*KIL";
                    break;

                case 0x13:
                    instructionString += "*SLO";
                    break;

                case 0x14:
                    instructionString += "*NOP";
                    break;

                case 0x15:
                    instructionString += ORA(AddressingMode.ZeroPageX, bF, pc);
                    break;

                case 0x16:
                    instructionString += ASL(AddressingMode.ZeroPageX, bF, pc);
                    break;

                case 0x17:
                    instructionString += "*SLO";
                    break;

                case 0x18:
                    //instructionString += "CLC: Clear Carry";
                    instructionString += CLC(AddressingMode.Implied, bF, pc);
                    break;

                case 0x19:
                    instructionString += ORA(AddressingMode.AbsoluteY, bF, pc);
                    break;

                case 0x1A:
                    instructionString += "*NOP";
                    break;

                case 0x1B:
                    instructionString += "*SLO";
                    break;

                case 0x1C:
                    instructionString += "*NOP";
                    break;

                case 0x1D:
                    instructionString += ORA(AddressingMode.AbsoluteX, bF, pc);
                    break;

                case 0x1E:
                    instructionString += ASL(AddressingMode.AbsoluteX, bF, pc);
                    break;

                case 0x1F:
                    instructionString += "*SLO";
                    break;

                case 0x20:
                    //instructionString += "JSR: Jump to Subroutine Absolute";
                    instructionString += JSR(AddressingMode.Absolute, bF, pc);
                    operandLength = 2;
                    break;

                case 0x21:
                    instructionString += AND(AddressingMode.AbsoluteX, bF, pc);
                    break;

                case 0x22:
                    instructionString += "*KIL";
                    break;

                case 0x23:
                    instructionString += "*RLA";
                    break;

                case 0x24:
                    //instructionString += "BIT: Bit Test using Zero Page";
                    instructionString += BIT(AddressingMode.ZeroPage, bF, pc);
                    break;

                case 0x25:
                    instructionString += AND(AddressingMode.ZeroPage, bF, pc);
                    break;

                case 0x26:
                    instructionString += ROL(AddressingMode.ZeroPage, bF, pc);
                    break;

                case 0x27:
                    instructionString += "*RLA";
                    break;

                case 0x28:
                    //instructionString += "PLP: Pull Processor Status";
                    instructionString += PLP(AddressingMode.Implied, bF, pc);
                    break;

                case 0x29:
                    instructionString += AND(AddressingMode.Immediate, bF, pc);
                    break;

                case 0x2A:
                    instructionString += ROL(AddressingMode.Accumulator, bF, pc);
                    break;

                case 0x2B:
                    instructionString += "*ANC";
                    break;

                case 0x2C:
                    //instructionString += "BIT: Bit Test using Absolute";
                    instructionString += BIT(AddressingMode.Absolute, bF, pc);
                    break;

                case 0x2D:
                    instructionString += AND(AddressingMode.Absolute, bF, pc);
                    break;

                case 0x2E:
                    instructionString += ROL(AddressingMode.Absolute, bF, pc);
                    break;

                case 0x2F:
                    instructionString += "*RLA";
                    break;

                case 0x30:
                    //instructionString += "BMI: Branch if Minus";
                    instructionString += BMI(AddressingMode.Relative, bF, pc);
                    break;

                case 0x31:
                    instructionString += AND(AddressingMode.IndirectY, bF, pc);
                    break;

                case 0x32:
                    instructionString += "*KIL";
                    break;

                case 0x33:
                    instructionString += "*RLA";
                    break;

                case 0x34:
                    instructionString += "*NOP";
                    break;

                case 0x35:
                    instructionString += AND(AddressingMode.ZeroPageX, bF, pc);
                    break;

                case 0x36:
                    instructionString += ROL(AddressingMode.ZeroPageX, bF, pc);
                    break;

                case 0x37:
                    instructionString += "*RLA";
                    break;

                case 0x38:
                    //instructionString += "SEC: Set Carry";
                    instructionString += SEC(AddressingMode.Implied, bF, pc);
                    break;

                case 0x39:
                    instructionString += AND(AddressingMode.AbsoluteY, bF, pc);
                    break;

                case 0x3A:
                    instructionString += "*NOP";
                    break;

                case 0x3B:
                    instructionString += "*RLA";
                    break;

                case 0x3C:
                    instructionString += "*NOP";
                    break;

                case 0x3D:
                    instructionString += AND(AddressingMode.AbsoluteX, bF, pc);
                    break;

                case 0x3E:
                    instructionString += ROL(AddressingMode.AbsoluteX, bF, pc);
                    break;

                case 0x3F:
                    instructionString += "*RLA";
                    break;

                case 0x40:
                    //instructionString += "RTI: Return from Interrupt";
                    instructionString += RTI(AddressingMode.Implied, bF, pc);
                    break;

                case 0x41:
                    //instructionString += "EOR: Bitwise Exclusive OR with (Indirect, X)";
                    instructionString += EOR(AddressingMode.IndirectX, bF, pc);
                    break;

                case 0x42:
                    instructionString += "*KIL";
                    break;

                case 0x43:
                    instructionString += "*SRE";
                    break;

                case 0x44:
                    instructionString += "*NOP";
                    break;

                case 0x45:
                    //instructionString += "EOR: Bitwise Exclusive OR Zero Page";
                    instructionString += EOR(AddressingMode.ZeroPage, bF, pc);
                    operandLength = 1;
                    break;

                case 0x46:
                    instructionString += LSR(AddressingMode.ZeroPage, bF, pc);
                    break;

                case 0x47:
                    instructionString += "*SRE";
                    break;

                case 0x48:
                    //instructionString += "PHA: Push A";
                    instructionString += "PHA";
                    break;

                case 0x49:
                    //instructionString += "EOR: Bitwise Exclusive OR #Immediate";
                    instructionString += EOR(AddressingMode.Immediate, bF, pc);
                    operandLength = 1;
                    break;

                case 0x4A:
                    instructionString += LSR(AddressingMode.Accumulator, bF, pc);
                    break;

                case 0x4B:
                    instructionString += "*ALR";
                    break;

                case 0x4C:
                    //instructionString += "JMP: Jump Absolute";
                    instructionString += JMP(AddressingMode.Absolute, bF, pc);
                    operandLength = 2;
                    break;

                case 0x4D:
                    //instructionString += "EOR: Bitwise Exclusive OR with Absolute";
                    instructionString += EOR(AddressingMode.Absolute, bF, pc);
                    operandLength = 2;
                    break;

                case 0x4E:
                    instructionString += LSR(AddressingMode.Absolute, bF, pc);
                    break;

                case 0x4F:
                    instructionString += "*ALR";
                    break;

                case 0x50:
                    //instructionString = "BVC: Branch if Overflow Clear";
                    instructionString = BVC(AddressingMode.Relative, bF, pc);
                    operandLength += 1;
                    break;

                case 0x51:
                    //instructionString += "EOR: Bitwise Exclusive OR with (Indirect, Y)";
                    instructionString += EOR(AddressingMode.IndirectY, bF, pc);
                    break;

                case 0x52:
                    instructionString += "*KIL";
                    break;

                case 0x53:
                    instructionString += "*SRE";
                    break;

                case 0x54:
                    instructionString += "*NOP";
                    break;

                case 0x55:
                    //instructionString += "EOR: Bitwise Exclusive OR Zero Page, X";
                    instructionString += EOR(AddressingMode.ZeroPageX, bF, pc);
                    operandLength = 1;
                    break;

                case 0x56:
                    instructionString += LSR(AddressingMode.ZeroPageX, bF, pc);
                    break;

                case 0x57:
                    instructionString += "*SRE";
                    break;

                case 0x58:
                    //instructionString += "CLI: Clear Interrupt Disable";
                    instructionString += CLI(AddressingMode.Implied, bF, pc);
                    break;

                case 0x59:
                    //instructionString += "EOR: Bitwise Exclusive OR with Absolute, Y";
                    instructionString += EOR(AddressingMode.AbsoluteY, bF, pc);
                    operandLength = 2;
                    break;

                case 0x5A:
                    instructionString += "*NOP";
                    break;

                case 0x5B:
                    instructionString += "*SRE";
                    break;

                case 0x5C:
                    instructionString += "*NOP";
                    break;

                case 0x5D:
                    //instructionString += "EOR: Bitwise Exclusive OR with Absolute, X";
                    instructionString += EOR(AddressingMode.AbsoluteX, bF, pc);
                    operandLength = 2;
                    break;

                case 0x5E:
                    instructionString += LSR(AddressingMode.AbsoluteX, bF, pc);
                    break;

                case 0x5F:
                    instructionString += "*SRE";
                    break;

                case 0x60:
                    //instructionString += "RTS: Return from Subroutine";
                    instructionString += RTS(AddressingMode.Implied, bF, pc);
                    break;

                case 0x61:
                    instructionString += ADC(AddressingMode.IndirectX, bF, pc);
                    break;

                case 0x62:
                    instructionString += "*KIL";
                    break;

                case 0x63:
                    instructionString += "*RRE";
                    break;

                case 0x64:
                    instructionString += "*NOP";
                    break;

                case 0x65:
                    instructionString += ADC(AddressingMode.ZeroPage, bF, pc);
                    break;

                case 0x66:
                    instructionString += ROR(AddressingMode.ZeroPage, bF, pc);
                    break;

                case 0x67:
                    instructionString += "*RRA";
                    break;

                case 0x68:
                    //instructionString += "PLA: Pull Accumulator";
                    instructionString += PLA(AddressingMode.Implied, bF, pc);
                    break;

                case 0x69:
                    instructionString += ADC(AddressingMode.Immediate, bF, pc);
                    break;

                case 0x6A:
                    instructionString += ROR(AddressingMode.Accumulator, bF, pc);
                    break;

                case 0x6B:
                    instructionString += "*ARR";
                    break;

                case 0x6C:
                    //instructionString += "JMP: Jump (Indirect)";
                    instructionString += JMP(AddressingMode.Indirect, bF, pc);
                    operandLength = 2;
                    break;

                case 0x6D:
                    instructionString += ADC(AddressingMode.Absolute, bF, pc);
                    break;

                case 0x6E:
                    instructionString += ROR(AddressingMode.Absolute, bF, pc);
                    break;

                case 0x6F:
                    instructionString += "*RRA";
                    break;

                case 0x70:
                    //instructionString = "BVS: Branch if Overflow Set";
                    instructionString = BVS(AddressingMode.Relative, bF, pc);
                    break;

                case 0x71:
                    instructionString += ADC(AddressingMode.IndirectY, bF, pc);
                    break;

                case 0x72:
                    instructionString += "*KIL";
                    break;

                case 0x73:
                    instructionString += "*RRA";
                    break;

                case 0x74:
                    instructionString += "*NOP";
                    break;

                case 0x75:
                    instructionString += ADC(AddressingMode.ZeroPageX, bF, pc);
                    break;

                case 0x76:
                    instructionString += ROR(AddressingMode.ZeroPageX, bF, pc);
                    break;

                case 0x77:
                    instructionString += "*RRA";
                    break;

                case 0x78:
                    //instructionString += "SEI: Set Interrupt Disable";
                    instructionString += SEI(AddressingMode.Implied, bF, pc);
                    break;

                case 0x79:
                    instructionString += ADC(AddressingMode.AbsoluteY, bF, pc);
                    break;

                case 0x7A:
                    instructionString += "*NOP";
                    break;

                case 0x7B:
                    instructionString += "*RRA";
                    break;

                case 0x7C:
                    instructionString += "*NOP";
                    break;

                case 0x7D:
                    instructionString += ADC(AddressingMode.AbsoluteX, bF, pc);
                    break;

                case 0x7E:
                    instructionString += ROR(AddressingMode.AbsoluteX, bF, pc);
                    break;

                case 0x7F:
                    instructionString += "*RRA";
                    break;

                case 0x80:
                    instructionString += "*NOP";
                    break;

                case 0x81:
                    instructionString += STA(AddressingMode.AbsoluteX, bF, pc);
                    break;

                case 0x82:
                    instructionString += "*NOP";
                    break;

                case 0x83:
                    instructionString += "*SAX";
                    break;

                case 0x84:
                    instructionString += STY(AddressingMode.ZeroPage, bF, pc);
                    break;

                case 0x85:
                    instructionString += STA(AddressingMode.ZeroPage, bF, pc);
                    break;

                case 0x86:
                    instructionString += STX(AddressingMode.ZeroPage, bF, pc);
                    break;

                case 0x87:
                    instructionString += "*SAX";
                    break;

                case 0x88:
                    //instructionString += "DEY: Decrement Index Register Y";
                    instructionString += DEY(AddressingMode.Implied, bF, pc);
                    break;

                case 0x89:
                    instructionString += "*NOP";
                    break;

                case 0x8A:
                    //instructionString += "TXA: Transfer X to A";
                    instructionString += TXA(AddressingMode.Implied, bF, pc);
                    break;

                case 0x8B:
                    instructionString += "*XAA";
                    break;

                case 0x8C:
                    instructionString += STY(AddressingMode.Absolute, bF, pc);
                    break;

                case 0x8D:
                    instructionString += STA(AddressingMode.Absolute, bF, pc);
                    break;

                case 0x8E:
                    instructionString += STX(AddressingMode.Absolute, bF, pc);
                    break;

                case 0x8F:
                    instructionString += "*SAX";
                    break;

                case 0x90:
                    //instructionString += "BCC: Branch if Carry Clear";
                    instructionString += BCC(AddressingMode.Relative, bF, pc);
                    operandLength = 1;
                    break;

                case 0x91:
                    instructionString += STA(AddressingMode.IndirectY, bF, pc);
                    break;

                case 0x92:
                    instructionString += "*KIL";
                    break;

                case 0x93:
                    instructionString += "*AHX";
                    break;

                case 0x94:
                    instructionString += STY(AddressingMode.ZeroPageX, bF, pc);
                    break;

                case 0x95:
                    instructionString += STA(AddressingMode.ZeroPageX, bF, pc);
                    break;

                case 0x96:
                    instructionString += STX(AddressingMode.ZeroPageY, bF, pc);
                    break;

                case 0x97:
                    instructionString += "*SAX";
                    break;

                case 0x98:
                    //instructionString += "TYA: Transfer Y to A";
                    instructionString += TYA(AddressingMode.Implied, bF, pc);
                    break;

                case 0x99:
                    instructionString += STA(AddressingMode.AbsoluteY, bF, pc);
                    break;

                case 0x9A:
                    //instructionString += "TXS: Transfer X to Stack Pointer";
                    instructionString += TXS(AddressingMode.Implied, bF, pc);
                    break;

                case 0x9B:
                    instructionString += "*TAS";
                    break;

                case 0x9C:
                    instructionString += "*SHY";
                    break;

                case 0x9D:
                    instructionString += STA(AddressingMode.AbsoluteX, bF, pc);
                    break;

                case 0x9E:
                    instructionString += "*SHX";
                    break;

                case 0x9F:
                    instructionString += "*AHX";
                    break;

                case 0xA0:
                    instructionString += LDY(AddressingMode.Immediate, bF, pc);
                    break;

                case 0xA1:
                    instructionString += LDA(AddressingMode.IndirectX, bF, pc);
                    break;

                case 0xA2:
                    instructionString += LDX(AddressingMode.Immediate, bF, pc);
                    break;

                case 0xA3:
                    instructionString += "*LAX";
                    break;

                case 0xA4:
                    instructionString += LDY(AddressingMode.ZeroPage, bF, pc);
                    break;

                case 0xA5:
                    instructionString += LDA(AddressingMode.ZeroPage, bF, pc);
                    break;

                case 0xA6:
                    instructionString += LDX(AddressingMode.ZeroPage, bF, pc);
                    break;

                case 0xA7:
                    instructionString += "*LAX";
                    break;

                case 0xA8:
                    //instructionString += "TAY: Transfer A to Y";
                    instructionString += TAY(AddressingMode.Implied, bF, pc);
                    break;

                case 0xA9:
                    instructionString += LDA(AddressingMode.Immediate, bF, pc);
                    break;

                case 0xAA:
                    //instructionString += "TAX: Transfer Accumulator to X";
                    instructionString += TAX(AddressingMode.Implied, bF, pc);
                    break;

                case 0xAB:
                    instructionString += "*LAX";
                    break;

                case 0xAC:
                    instructionString += LDY(AddressingMode.Absolute, bF, pc);
                    break;

                case 0xAD:
                    instructionString += LDA(AddressingMode.Absolute, bF, pc);
                    break;

                case 0xAE:
                    instructionString += LDX(AddressingMode.Absolute, bF, pc);
                    break;

                case 0xAF:
                    instructionString += "*LAX";
                    break;

                case 0xB0:
                    //instructionString += "BCS: Branch if Carry Set";
                    instructionString += BCS(AddressingMode.Relative, bF, pc);
                    break;

                case 0xB1:
                    instructionString += LDA(AddressingMode.IndirectY, bF, pc);
                    break;

                case 0xB2:
                    instructionString += "*KIL";
                    break;

                case 0xB3:
                    instructionString += "*LAX";
                    break;

                case 0xB4:
                    instructionString += LDY(AddressingMode.ZeroPageX, bF, pc);
                    break;

                case 0xB5:
                    instructionString += LDA(AddressingMode.ZeroPageX, bF, pc);
                    break;

                case 0xB6:
                    instructionString += LDX(AddressingMode.ZeroPageY, bF, pc);
                    break;

                case 0xB7:
                    instructionString += "*LAX";
                    break;

                case 0xB8:
                    //instructionString += "CLV: Clear Overflow";
                    instructionString += CLV(AddressingMode.Implied, bF, pc);
                    break;

                case 0xB9:
                    instructionString += LDA(AddressingMode.AbsoluteY, bF, pc);
                    break;

                case 0xBA:
                    //instructionString += "TSX: Transfer Stack Pointer to X";
                    instructionString += TSX(AddressingMode.Implied, bF, pc);
                    break;

                case 0xBB:
                    instructionString += "*LAS";
                    break;

                case 0xBC:
                    instructionString += LDY(AddressingMode.AbsoluteX, bF, pc);
                    break;

                case 0xBD:
                    instructionString += LDA(AddressingMode.AbsoluteX, bF, pc);
                    break;

                case 0xBE:
                    instructionString += LDX(AddressingMode.AbsoluteY, bF, pc);
                    break;

                case 0xBF:
                    instructionString += "*LAX";
                    break;

                case 0xC0:
                    instructionString += CPY(AddressingMode.Immediate, bF, pc);
                    break;

                case 0xC1:
                    instructionString += CMP(AddressingMode.IndirectX, bF, pc);
                    break;

                case 0xC2:
                    instructionString += "*NOP";
                    break;

                case 0xC3:
                    instructionString += "*DCP";
                    break;

                case 0xC4:
                    instructionString += CPY(AddressingMode.ZeroPage, bF, pc);
                    break;

                case 0xC5:
                    instructionString += CMP(AddressingMode.ZeroPage, bF, pc);
                    break;

                case 0xC6:
                    instructionString += DEC(AddressingMode.ZeroPage, bF, pc);
                    break;

                case 0xC7:
                    instructionString += "*DCP";
                    break;

                case 0xC8:
                    //instructionString += "INY: Increment Y";
                    instructionString += INY(AddressingMode.Implied, bF, pc);
                    break;

                case 0xC9:
                    instructionString += CMP(AddressingMode.Immediate, bF, pc);
                    break;

                case 0xCA:
                    //instructionString += "DEX: Decrement X";
                    instructionString += DEX(AddressingMode.Implied, bF, pc);
                    break;

                case 0xCB:
                    instructionString += "*AXS";
                    break;

                case 0xCC:
                    instructionString += CPY(AddressingMode.Absolute, bF, pc);
                    break;

                case 0xCD:
                    instructionString += CMP(AddressingMode.Absolute, bF, pc);
                    break;

                case 0xCE:
                    instructionString += DEC(AddressingMode.Absolute, bF, pc);
                    break;

                case 0xCF:
                    instructionString += "*DCP";
                    break;

                case 0xD0:
                    //instructionString += "BNE: Branch if Not Equal";
                    instructionString += BNE(AddressingMode.Relative, bF, pc);
                    operandLength = 1;
                    break;

                case 0xD1:
                    instructionString += CMP(AddressingMode.IndirectY, bF, pc);
                    break;

                case 0xD2:
                    instructionString += "*KIL";
                    break;

                case 0xD3:
                    instructionString += "*DCP";
                    break;

                case 0xD4:
                    instructionString += "*NOP";
                    break;

                case 0xD5:
                    instructionString += CMP(AddressingMode.ZeroPageX, bF, pc);
                    break;

                case 0xD6:
                    instructionString += DEC(AddressingMode.ZeroPageX, bF, pc);
                    break;

                case 0xD7:
                    instructionString += "*DCP";
                    break;

                case 0xD8:
                    //instructionString += "CLD: Clear Decimal";
                    instructionString += CLD(AddressingMode.Implied, bF, pc);
                    break;

                case 0xD9:
                    instructionString += CMP(AddressingMode.AbsoluteY, bF, pc);
                    break;

                case 0xDA:
                    instructionString += "*NOP";
                    break;

                case 0xDB:
                    instructionString += "*DCP";
                    break;

                case 0xDC:
                    instructionString += "*NOP";
                    break;

                case 0xDD:
                    instructionString += CMP(AddressingMode.AbsoluteX, bF, pc);
                    break;

                case 0xDE:
                    instructionString += DEC(AddressingMode.AbsoluteX, bF, pc);
                    break;

                case 0xDF:
                    instructionString += "*DCP";
                    break;

                case 0xE0:
                    instructionString += CPX(AddressingMode.Immediate, bF, pc);
                    break;

                case 0xE1:
                    instructionString += SBC(AddressingMode.IndirectX, bF, pc);
                    break;

                case 0xE2:
                    instructionString += "*NOP";
                    break;

                case 0xE3:
                    instructionString += "*ISC";
                    break;

                case 0xE4:
                    instructionString += CPX(AddressingMode.ZeroPage, bF, pc);
                    break;

                case 0xE5:
                    instructionString += SBC(AddressingMode.ZeroPage, bF, pc);
                    break;

                case 0xE6:
                    instructionString += INC(AddressingMode.ZeroPage, bF, pc);
                    break;

                case 0xE7:
                    instructionString += "*ISC";
                    break;

                case 0xE8:
                    //instructionString += "INX: Increment Index Register X";
                    instructionString += INX(AddressingMode.Implied, bF, pc);
                    break;

                case 0xE9:
                    instructionString += SBC(AddressingMode.Immediate, bF, pc);
                    break;

                case 0xEA:
                    //instructionString += "NOP: No Operation";
                    instructionString += NOP(AddressingMode.Implied, bF, pc);
                    break;

                case 0xEB:
                    instructionString += "*SBC";
                    break;

                case 0xEC:
                    instructionString += CPX(AddressingMode.Absolute, bF, pc);
                    break;

                case 0xED:
                    instructionString += SBC(AddressingMode.Absolute, bF, pc);
                    break;

                case 0xEE:
                    instructionString += INC(AddressingMode.Absolute, bF, pc);
                    break;

                case 0xEF:
                    instructionString += "*ISC";
                    break;

                case 0xF0:
                    //instructionString += "BEQ: Branch if Equal Relative";
                    instructionString += BEQ(AddressingMode.Relative, bF, pc);
                    break;

                case 0xF1:
                    instructionString += SBC(AddressingMode.IndirectY, bF, pc);
                    break;

                case 0xF2:
                    instructionString += "*KIL";
                    break;

                case 0xF3:
                    instructionString += "*ISC";
                    break;

                case 0xF4:
                    instructionString += "*NOP";
                    break;

                case 0xF5:
                    instructionString += SBC(AddressingMode.ZeroPageX, bF, pc);
                    break;

                case 0xF6:
                    instructionString += INC(AddressingMode.ZeroPageX, bF, pc);
                    break;

                case 0xF7:
                    instructionString += "*ISC";
                    break;

                case 0xF8:
                    //instructionString += "SED: Set Decimal";
                    instructionString += SED(AddressingMode.Implied, bF, pc);
                    break;

                case 0xF9:
                    instructionString += SBC(AddressingMode.AbsoluteY, bF, pc);
                    break;

                case 0xFA:
                    instructionString += "*NOP";
                    break;

                case 0xFB:
                    instructionString += "*ISC";
                    break;

                case 0xFC:
                    instructionString += "*NOP";
                    break;

                case 0xFD:
                    instructionString += SBC(AddressingMode.AbsoluteX, bF, pc);
                    break;

                case 0xFE:
                    instructionString += INC(AddressingMode.AbsoluteX, bF, pc);
                    break;

                case 0xFF:
                    instructionString += "*ISC";
                    break;
            }

            return instructionString;
        }

        static void Print(List<byte> binaryFile, string instructionString)
        {
            // Print
            string PCString = "";
            //"PC: ";

            if (PC <= 0xF)
                PCString += $"0x000{PC:X}";
            else if (PC <= 0xFF)
                PCString += $"0x00{PC:X}";
            else if (PC <= 0xFFF)
                PCString += $"0x0{PC:X}";
            else
                PCString += $"0x{PC:X}";

            string opcodeString = "Opcode: ";

            //$"0x{ binaryFile[PC]:X}"
            if (binaryFile[(int)PC + 0x10] <= 0xF)
                opcodeString += $"0x0{binaryFile[(int)PC + 0x10]:X}";
            else
                opcodeString += $"0x{binaryFile[(int)PC + 0x10]:X}";

            //Console.WriteLine(PCString + " | " + opcodeString + " | " + instructionString);
            Console.WriteLine(instructionString);
        }
        
        static void Main(string[] args)
        {
            // Read NES binary file
            try
            {
                binaryFile = ReadFileData(args[0]);
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
                byte b;

                // Check the iNES header (4E 45 53 1A)
                if(binaryFile[0] == 0x4E && binaryFile[1] == 0x45 && binaryFile[2] == 0x53 && binaryFile[3] == 0x1A)
                {
                    // Set the number of PRG and CHR blocks
                    numberOfPRGROMBlocks = binaryFile[4];
                    numberOfCHRROMBlocks = binaryFile[5];

                    // Check the mapper type

                    subroutineStartingAddresses = new List<int>();

                    //while (PC < binaryFile.Count)
                    while (PC < 0x8000 * numberOfPRGROMBlocks)
                    {
                        // 
                        //if (PC >= header)
                        {
                            operandLength = 0;
                            //b = binaryFile[(int)PC + 0x10];

                            // Fetch

                            // Decode
                            string instructionString = Decode(binaryFile, PC);


                            // Print
                            Print(binaryFile, instructionString);

                            // Increment program counter by 1 plus the operand length
                            PC += (uint)(1 + operandLength);
                        }
                        // Increment program counter for the bootstrap ROM and header sections by 1
                        //else
                        //PC++;
                    }
                }
                else
                    Console.WriteLine("Invalid nes file passed as argument. Closing...");
            }
            else
                Console.WriteLine("Invalid nes file passed as argument. Closing...");
        }
    }
}
