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

        static string Decode(byte b)
        {
            // Instruction set
            string instructionString = "Instruction: ";

            // Instructions (1 byte length + 0-2 bytes of operands)
            switch (b)
            {
                case 0x00:
                    instructionString += "BRK: Break (software IRQ)";

                    // Set the interrupt disable flag to 1
                    statusRegister = (byte)(statusRegister & 0x00000100);

                    // The return address pushed to the stack skips the byte after the BRK opcode, so it's often considered a 2-byte instruction with an unused immediate
                    operandLength = 1;
                    break;

                case 0x01:
                    instructionString += "ORA: Bitwise OR with (Indirect, X)";
                    operandLength = 1;
                    break;

                case 0x02:
                    instructionString += "KIL: Halt program (Illegal)";
                    break;

                case 0x03:
                    instructionString += "SLO: Set LSB in memory OR Accumulator in Indirect, X (Illegal)";
                    break;

                case 0x04:
                    instructionString += "NOP: Illegal";
                    break;

                case 0x05:
                    instructionString += "ORA: Bitwise OR with Zero Page";
                    operandLength += 1;
                    break;

                case 0x06:
                    instructionString += "ASL: Arithmetic Shift Left of the Zero Page";
                    operandLength += 1;
                    break;

                case 0x07:
                    instructionString += "SLO: Illegal";
                    break;

                case 0x08:
                    instructionString += "PHP: Push Processor Status Register Implied";
                    break;

                case 0x09:
                    instructionString += "ORA: Bitwise OR with #Immediate";
                    operandLength = 1;
                    break;

                case 0x0A:
                    instructionString += "ASL: Arithmetic Shift Left of the Accumulator";
                    break;

                case 0x0B:
                    instructionString += "ANC: Illegal";
                    break;

                case 0x0C:
                    instructionString += "NOP: Illegal";
                    break;

                case 0x0D:
                    instructionString += "ORA: Bitwise OR with Absolute";
                    operandLength += 2;
                    break;

                case 0x0E:
                    instructionString += "ASL: Arithmetic Shift Left of the Absolute";
                    operandLength += 2;
                    break;

                case 0x0F:
                    instructionString += "SLO: Illegal";
                    break;

                case 0x10:
                    instructionString += "BPL: Branch if Plus Relative";
                    operandLength = 1;
                    break;

                case 0x11:
                    instructionString += "ORA: Bitwise OR with (Indirect), Y";
                    operandLength = 1;
                    break;

                case 0x12:
                    instructionString += "KIL: Crash (Illegal)";
                    break;

                case 0x13:
                    instructionString += "SLO: Illegal";
                    break;

                case 0x14:
                    instructionString += "NOP: Illegal";
                    break;

                case 0x15:
                    instructionString += "ORA: Bitwise OR with Zero Page, X";
                    operandLength = 1;
                    break;

                case 0x16:
                    instructionString += "ASL: Arithmetic Shift Left of the Zero Page, X";
                    operandLength += 1;
                    break;

                case 0x17:
                    instructionString += "SLO: Illegal";
                    break;

                case 0x18:
                    instructionString += "CLC: Clear Carry";
                    break;

                case 0x19:
                    instructionString += "ORA: Bitwise OR with Absolute, Y";
                    operandLength = 2;
                    break;

                case 0x1A:
                    instructionString += "NOP: Illegal";
                    break;

                case 0x1B:
                    instructionString += "SLO: Illegal";
                    break;

                case 0x1C:
                    instructionString += "NOP: Illegal";
                    break;

                case 0x1D:
                    instructionString += "ORA: Bitwise OR with Absolute, X";
                    operandLength = 2;
                    break;

                case 0x1E:
                    instructionString += "ASL: Arithmetic Shift Left of the Absolute, X";
                    operandLength += 2;
                    break;

                case 0x1F:
                    instructionString += "SLO: Illegal";
                    break;

                case 0x20:
                    instructionString += "JSR: Jump to Subroutine Absolute";
                    operandLength = 2;
                    break;

                case 0x21:
                    instructionString += "AND: Bitwise AND with (Indirect, X)";
                    operandLength += 1;
                    break;

                case 0x22:
                    instructionString += "KIL: Halt program (Illegal)";
                    break;

                case 0x23:
                    instructionString += "RLA: Illegal";
                    break;

                case 0x24:
                    instructionString += "BIT: Bit Test using Zero Page";
                    operandLength = 1;
                    break;

                case 0x25:
                    instructionString += "AND: Bitwise AND with Zero Page";
                    operandLength += 1;
                    break;

                case 0x26:
                    instructionString += "ROL: Rotate Left Zero Page";
                    operandLength += 1;
                    break;

                case 0x27:
                    instructionString += "RLA: ROL and AND (Illegal)";
                    break;

                case 0x28:
                    instructionString += "PLP: Pull Processor Status";
                    break;

                case 0x29:
                    instructionString += "AND: Bitwise AND #Immediate";
                    operandLength += 1;
                    break;

                case 0x2A:
                    instructionString += "ROL: Rotate Left Accumulator";
                    break;

                case 0x2B:
                    instructionString += "ANC: Illegal";
                    break;

                case 0x2C:
                    instructionString += "BIT: Bit Test using Absolute";
                    operandLength = 2;
                    break;

                case 0x2D:
                    instructionString += "AND: Bitwise AND with Absolute";
                    operandLength += 2;
                    break;

                case 0x2E:
                    instructionString += "ROL: Rotate Left Absolute";
                    operandLength += 2;
                    break;

                case 0x2F:
                    instructionString += "RLA: Illegal";
                    break;

                case 0x30:
                    instructionString += "BMI: Branch if Minus";
                    operandLength += 1;
                    break;

                case 0x31:
                    instructionString += "AND: Bitwise AND with (Indirect), Y";
                    operandLength += 1;
                    break;

                case 0x32:
                    instructionString += "KIL: Halt program (Illegal)";
                    break;

                case 0x33:
                    instructionString += "RLA: Illegal";
                    break;

                case 0x34:
                    instructionString += "NOP: Illegal";
                    break;

                case 0x35:
                    instructionString += "AND: Bitwise AND with Zero Page, X";
                    operandLength += 1;
                    break;

                case 0x36:
                    instructionString += "ROL: Rotate Left Zero Page, X";
                    operandLength += 1;
                    break;

                case 0x37:
                    instructionString += "RLA: Illegal";
                    break;

                case 0x38:
                    instructionString += "SEC: Set Carry";
                    break;

                case 0x39:
                    instructionString += "AND: Bitwise AND with Absolute, Y";
                    operandLength += 2;
                    break;

                case 0x3A:
                    instructionString += "NOP: Illegal";
                    break;

                case 0x3B:
                    instructionString += "RLA: Illegal";
                    break;

                case 0x3C:
                    instructionString += "NOP: Illegal";
                    break;

                case 0x3D:
                    instructionString += "AND: Bitwise AND with Absolute, X";
                    operandLength += 2;
                    break;

                case 0x3E:
                    instructionString += "ROL: Rotate Left Absolute, X";
                    operandLength += 2;
                    break;

                case 0x3F:
                    instructionString += "RLA: Illegal";
                    break;

                case 0x40:
                    instructionString += "RTI: Return from Interrupt";
                    break;

                case 0x41:
                    instructionString += "EOR: Bitwise Exclusive OR with (Indirect, X)";
                    operandLength = 1;
                    break;

                case 0x42:
                    instructionString += "KIL: Halt program (Illegal)";
                    break;

                case 0x43:
                    instructionString += "SRE: Illegal";
                    break;

                case 0x44:
                    instructionString += "NOP: Illegal";
                    break;

                case 0x45:
                    instructionString += "EOR: Bitwise Exclusive OR Zero Page";
                    operandLength = 1;
                    break;

                case 0x46:
                    instructionString += "LSR: Logical Shift Right of the Zero Page";
                    operandLength += 1;
                    break;

                case 0x47:
                    instructionString += "SRE: Illegal";
                    break;

                case 0x48:
                    instructionString += "PHA: Push A";
                    break;

                case 0x49:
                    instructionString += "EOR: Bitwise Exclusive OR #Immediate";
                    operandLength = 1;
                    break;

                case 0x4A:
                    instructionString += "LSR: Logical Shift Right of the Accumulator";
                    break;

                case 0x4B:
                    instructionString += "ALR: Illegal";
                    break;

                case 0x4C:
                    instructionString += "JMP: Jump Absolute";
                    operandLength = 2;
                    break;

                case 0x4D:
                    instructionString += "EOR: Bitwise Exclusive OR with Absolute";
                    operandLength = 2;
                    break;

                case 0x4E:
                    instructionString += "LSR: Logical Shift Right of the Absolute";
                    operandLength += 2;
                    break;

                case 0x4F:
                    instructionString += "ALR: Illegal";
                    break;

                case 0x50:
                    instructionString = "BVC: Branch if Overflow Clear";
                    operandLength += 1;
                    break;

                case 0x51:
                    instructionString += "EOR: Bitwise Exclusive OR with (Indirect, Y)";
                    operandLength += 1;
                    break;

                case 0x52:
                    instructionString += "KIL: Halt program (Illegal)";
                    break;

                case 0x53:
                    instructionString += "SRE: Illegal";
                    break;

                case 0x54:
                    instructionString += "NOP: Illegal";
                    break;

                case 0x55:
                    instructionString += "EOR: Bitwise Exclusive OR Zero Page, X";
                    operandLength = 1;
                    break;

                case 0x56:
                    instructionString += "LSR: Logical Shift Right of the Zero Page, X";
                    operandLength += 1;
                    break;

                case 0x57:
                    instructionString += "SRE: Illegal";
                    break;

                case 0x58:
                    instructionString += "CLI: Clear Interrupt Disable";
                    break;

                case 0x59:
                    instructionString += "EOR: Bitwise Exclusive OR with Absolute, Y";
                    operandLength = 2;
                    break;

                case 0x5A:
                    instructionString += "NOP: Illegal";
                    break;

                case 0x5B:
                    instructionString += "Illegal operation";
                    break;

                case 0x5C:
                    instructionString += "NOP: Illegal";
                    break;

                case 0x5D:
                    instructionString += "EOR: Bitwise Exclusive OR with Absolute, X";
                    operandLength = 2;
                    break;

                case 0x5E:
                    instructionString += "LSR: Logical Shift Right of the Absolute, X";
                    operandLength += 2;
                    break;

                case 0x5F:
                    instructionString += "SRE: Illegal";
                    break;

                case 0x60:
                    instructionString += "RTS: Return from Subroutine";
                    break;

                case 0x61:
                    instructionString += "ADC: Add with Carry using (Indirect, X)";
                    operandLength += 1;
                    break;

                case 0x62:
                    instructionString += "KIL: Halt program (Illegal)";
                    break;

                case 0x63:
                    instructionString += "RRE: Illegal";
                    break;

                case 0x64:
                    instructionString += "NOP: Illegal";
                    break;

                case 0x65:
                    instructionString += "ADC: Add with Carry using Zero Page";
                    operandLength += 1;
                    break;

                case 0x66:
                    instructionString += "ROR: Rotate Right the Zero Page";
                    operandLength += 1;
                    break;

                case 0x67:
                    instructionString += "RRA: Illegal";
                    break;

                case 0x68:
                    instructionString += "PLA: Pull Accumulator";
                    break;

                case 0x69:
                    instructionString += "ADC: Add with Carry using #Immediate";
                    operandLength += 1;
                    break;

                case 0x6A:
                    instructionString += "ROR: Rotate Right the Accumulator";
                    break;

                case 0x6B:
                    instructionString += "ARR: Illegal";
                    break;

                case 0x6C:
                    instructionString += "JMP: Jump (Indirect)";
                    operandLength = 2;
                    break;

                case 0x6D:
                    instructionString += "ADC: Add with Carry using Absolute";
                    operandLength += 2;
                    break;

                case 0x6E:
                    instructionString += "ROR: Rotate Right the Absolute";
                    operandLength += 2;
                    break;

                case 0x6F:
                    instructionString += "RRA: Illegal";
                    break;

                case 0x70:
                    instructionString = "BVS: Branch if Overflow Set";
                    operandLength += 1;
                    break;

                case 0x71:
                    instructionString += "ADC: Add with Carry (Indirect, Y)";
                    operandLength = 1;
                    break;

                case 0x72:
                    instructionString += "KIL: Halt program (Illegal)";
                    break;

                case 0x73:
                    instructionString += "RRA: Illegal";
                    break;

                case 0x74:
                    instructionString += "NOP: Illegal";
                    break;

                case 0x75:
                    instructionString += "ADC: Add with Carry using Zero Page, X";
                    operandLength += 1;
                    break;

                case 0x76:
                    instructionString += "ROR: Rotate Right the Zero Page, X";
                    operandLength += 1;
                    break;

                case 0x77:
                    instructionString += "RRA: Illegal";
                    break;

                case 0x78:
                    instructionString += "SEI: Set Interrupt Disable";
                    break;

                case 0x79:
                    instructionString += "ADC: Add with Carry using Absolute, Y";
                    operandLength += 2;
                    break;

                case 0x7A:
                    instructionString += "NOP: Illegal";
                    break;

                case 0x7B:
                    instructionString += "RRA: Illegal";
                    break;

                case 0x7C:
                    instructionString += "NOP: Illegal";
                    break;

                case 0x7D:
                    instructionString += "ADC: Add with Carry using Absolute, X";
                    operandLength += 2;
                    break;

                case 0x7E:
                    instructionString += "ROR: Rotate Right the Absolute, X";
                    operandLength += 2;
                    break;

                case 0x7F:
                    instructionString += "RRA: Illegal";
                    break;

                case 0x80:
                    instructionString += "NOP: Illegal";
                    break;

                case 0x81:
                    instructionString += "STA: Store Accumulator into (Indirect, X)";
                    operandLength += 1;
                    break;

                case 0x82:
                    instructionString += "NOP: Illegal";
                    break;

                case 0x83:
                    instructionString += "SAX: Illegal";
                    break;

                case 0x84:
                    instructionString += "STY: Store from Y using Zero Page";
                    operandLength += 1;
                    break;

                case 0x85:
                    instructionString += "STA: Store from Accumulator into Zero Page";
                    operandLength += 1;
                    break;

                case 0x86:
                    instructionString += "STX: Store from X using Zero Page";
                    operandLength += 1;
                    break;

                case 0x87:
                    instructionString += "SAX: Illegal";
                    break;

                case 0x88:
                    instructionString += "DEY: Decrement Index Register Y";
                    break;

                case 0x89:
                    instructionString += "NOP: Illegal";
                    break;

                case 0x8A:
                    instructionString += "TXA: Transfer X to A";
                    break;

                case 0x8B:
                    instructionString += "XAA or ANE: Illegal operation";
                    break;

                case 0x8C:
                    instructionString += "STY: Store Register Y to Absolute";
                    operandLength += 2;
                    break;

                case 0x8D:
                    instructionString += "STA: Store from Accumulator into Absolute";
                    operandLength += 2;
                    break;

                case 0x8E:
                    instructionString += "STX: Store from X into Absolute";
                    operandLength += 2;
                    break;

                case 0x8F:
                    instructionString += "SAX: Illegal";
                    break;

                case 0x90:
                    instructionString += "BCC: Branch if Carry Clear";
                    operandLength = 1;
                    break;

                case 0x91:
                    instructionString += "STA: Store from Accumulator using (Indirect), Y";
                    operandLength = 1;
                    break;

                case 0x92:
                    instructionString += "KIL: Halt program (Illegal)";
                    break;

                case 0x93:
                    instructionString += "AHX: Illegal";
                    break;

                case 0x94:
                    instructionString += "STA: Store from Y into Zero Page, X";
                    operandLength += 1;
                    break;

                case 0x95:
                    instructionString += "STA: Store Accumulator into Zero Page, X";
                    operandLength += 1;
                    break;

                case 0x96:
                    instructionString += "STA: Store from X into Zero Page, Y";
                    operandLength += 1;
                    break;

                case 0x97:
                    instructionString += "SAX: Illegal";
                    break;

                case 0x98:
                    instructionString += "TYA: Transfer Y to A";
                    break;

                case 0x99:
                    instructionString += "STA: Store Accumulator into Absolute, Y";
                    operandLength += 2;
                    break;

                case 0x9A:
                    instructionString += "TXS: Transfer X to Stack Pointer";
                    break;

                case 0x9B:
                    instructionString += "TAS: Illegal";
                    break;

                case 0x9C:
                    instructionString += "SHY: Illegal";
                    break;

                case 0x9D:
                    instructionString += "STA: Store Accumulator into Absolute, X";
                    operandLength += 2;
                    break;

                case 0x9E:
                    instructionString += "SHX: Illegal";
                    break;

                case 0x9F:
                    instructionString += "AHX: Illegal";
                    break;

                case 0xA0:
                    instructionString += "LDY: Load Index Register Y using #Immediate";
                    operandLength += 1;
                    break;

                case 0xA1:
                    instructionString += "LDA: Load Accumulator (Indirect), X";
                    operandLength += 1;
                    break;

                case 0xA2:
                    instructionString += "LDX: Load Index Register X using #Immediate";
                    operandLength += 1;
                    break;

                case 0xA3:
                    instructionString += "LAX: Illegal";
                    break;

                case 0xA4:
                    instructionString += "LDY: Load Index Register Y using Zero Page";
                    operandLength += 1;
                    break;

                case 0xA5:
                    instructionString += "LDA: Load Accumulator using Zero Page";
                    operandLength += 1;
                    break;

                case 0xA6:
                    instructionString += "LDX: Load X using Zero Page";
                    operandLength += 1;
                    break;

                case 0xA7:
                    instructionString += "LAX: Illegal";
                    break;

                case 0xA8:
                    instructionString += "TAY: Transfer A to Y";
                    break;

                case 0xA9:
                    instructionString += "LDA: Load Accumulator using #Immediate";
                    operandLength = 1;
                    break;

                case 0xAA:
                    instructionString += "TAX: Transfer Accumulator to X";
                    break;

                case 0xAB:
                    instructionString += "LAX: Illegal";
                    break;

                case 0xAC:
                    instructionString += "LDY: Load Index Register Y using Absolute";
                    operandLength += 2;
                    break;

                case 0xAD:
                    instructionString += "LDA: Load Accumulator Absolute";
                    operandLength = 2;
                    break;

                case 0xAE:
                    instructionString += "LDX: Load X using Absolute";
                    operandLength += 2;
                    break;

                case 0xAF:
                    instructionString += "LAX: Illegal";
                    break;

                case 0xB0:
                    instructionString += "BCS: Branch if Carry Set";
                    operandLength += 1;
                    break;

                case 0xB1:
                    instructionString += "LDA: Load Accumulator (Indirect), Y";
                    operandLength += 1;
                    break;

                case 0xB2:
                    instructionString += "KIL: Halt program (Illegal)";
                    break;

                case 0xB3:
                    instructionString += "LAX: Illegal";
                    break;

                case 0xB4:
                    instructionString += "LDY: Load Index Register Y using Zero Page, X";
                    operandLength += 1;
                    break;

                case 0xB5:
                    instructionString += "LDA: Load Accumulator using Zero Page, X";
                    operandLength += 1;
                    break;

                case 0xB6:
                    instructionString += "LDX: Load X using Zero Page, Y";
                    operandLength += 1;
                    break;

                case 0xB7:
                    instructionString += "LAX: Illegal";
                    break;

                case 0xB8:
                    instructionString += "CLV: Clear Overflow";
                    break;

                case 0xB9:
                    instructionString += "LDA: Load into Accumulator using Absolute, Y";
                    operandLength += 2;
                    break;

                case 0xBA:
                    instructionString += "TSX: Transfer Stack Pointer to X";
                    break;

                case 0xBB:
                    instructionString += "LAS: Illegal";
                    break;

                case 0xBC:
                    instructionString += "LDY: Load Index Register Y using Absolute, X";
                    operandLength += 2;
                    break;

                case 0xBD:
                    instructionString += "LDA: Load into Accumulator using Absolute, X";
                    operandLength += 2;
                    break;

                case 0xBE:
                    instructionString += "LDX: Load X using Absolute, Y";
                    operandLength += 2;
                    break;

                case 0xBF:
                    instructionString += "LAX: Illegal";
                    break;

                case 0xC0:
                    instructionString += "CPY: Compare Y with #Immediate";
                    operandLength += 1;
                    break;

                case 0xC1:
                    instructionString += "CMP: Compare Accumulator to (Indirect, X)";
                    operandLength += 1;
                    break;

                case 0xC2:
                    instructionString += "NOP: Illegal";
                    break;

                case 0xC3:
                    instructionString += "DCP: Illegal";
                    break;

                case 0xC4:
                    instructionString += "CPY: Compare Y with Zero Page";
                    operandLength += 1;
                    break;

                case 0xC5:
                    instructionString += "CMP: Compare Accumulator to Zero Page";
                    operandLength += 1;
                    break;

                case 0xC6:
                    instructionString += "DEC: Decrement Zero Page";
                    operandLength += 1;
                    break;

                case 0xC7:
                    instructionString += "DCP: Illegal";
                    break;

                case 0xC8:
                    instructionString += "INY: Increment Y";
                    break;

                case 0xC9:
                    instructionString += "CPY: Compare Accumulator with #Immediate";
                    operandLength = 1;
                    break;

                case 0xCA:
                    instructionString += "DEX: Decrement X";
                    break;

                case 0xCB:
                    instructionString += "AXS: Illegal";
                    break;

                case 0xCC:
                    instructionString += "CPY: Compare Y with Absolute";
                    operandLength += 2;
                    break;

                case 0xCD:
                    instructionString += "CPY: Compare Accumulator with Absolute";
                    operandLength += 2;
                    break;

                case 0xCE:
                    instructionString += "DEC: Decrement Absolute";
                    operandLength += 2;
                    break;

                case 0xCF:
                    instructionString += "DCP: Illegal";
                    break;

                case 0xD0:
                    instructionString += "BNE: Branch if Not Equal";
                    operandLength = 1;
                    break;

                case 0xD1:
                    instructionString += "CMP: Compare Accumulator to (Indirect), Y";
                    operandLength += 1;
                    break;

                case 0xD2:
                    instructionString += "KIL: Halt program (Illegal)";
                    break;

                case 0xD3:
                    instructionString += "DCP: Illegal";
                    break;

                case 0xD4:
                    instructionString += "NOP: Illegal";
                    break;

                case 0xD5:
                    instructionString += "CMP: Compare Accumulator to Zero Page, X";
                    operandLength += 1;
                    break;

                case 0xD6:
                    instructionString += "DEC: Decrement Zero Page, X";
                    operandLength += 1;
                    break;

                case 0xD7:
                    instructionString += "DCP: Illegal";
                    break;

                case 0xD8:
                    instructionString += "CLD: Clear Decimal";
                    break;

                case 0xD9:
                    instructionString += "CPY: Compare Accumulator with Absolute, Y";
                    operandLength += 2;
                    break;

                case 0xDA:
                    instructionString += "NOP: Illegal";
                    break;

                case 0xDB:
                    instructionString += "DCP: Illegal";
                    break;

                case 0xDC:
                    instructionString += "NOP: Illegal";
                    break;

                case 0xDD:
                    instructionString += "CPY: Compare Accumulator with Absolute, X";
                    operandLength += 2;
                    break;

                case 0xDE:
                    instructionString += "DEC: Decrement Absolute, X";
                    operandLength += 2;
                    break;

                case 0xDF:
                    instructionString += "DCP: Illegal";
                    break;

                case 0xE0:
                    instructionString += "CPX: Compare X to #Immediate";
                    operandLength += 1;
                    break;

                case 0xE1:
                    instructionString += "SBC: Subtract with Carry using (Indirect, X)";
                    operandLength += 1;
                    break;

                case 0xE2:
                    instructionString += "NOP: Illegal";
                    break;

                case 0xE3:
                    instructionString += "ISC: Illegal";
                    break;

                case 0xE4:
                    instructionString += "CPX: Compare X to Zero Page";
                    operandLength += 1;
                    break;

                case 0xE5:
                    instructionString += "SBC: Subtract with Carry using Zero Page";
                    operandLength = 1;
                    break;

                case 0xE6:
                    instructionString += "INC: Increment Zero Page";
                    operandLength += 1;
                    break;

                case 0xE7:
                    instructionString += "ISC: Illegal";
                    break;

                case 0xE8:
                    instructionString += "INX: Increment Index Register X";
                    break;

                case 0xE9:
                    instructionString += "SBC: Subtract with Carry using #Immediate";
                    operandLength = 1;
                    break;

                case 0xEA:
                    instructionString += "NOP: No Operation";
                    break;

                case 0xEB:
                    instructionString += "SBC: Illegal";
                    break;

                case 0xEC:
                    instructionString += "CPX: Compare X to Absolute";
                    operandLength += 2;
                    break;

                case 0xED:
                    instructionString += "SBC: Subtract with Carry using Absolute";
                    operandLength += 2;
                    break;

                case 0xEE:
                    instructionString += "INC: Increment Absolute";
                    operandLength += 2;
                    break;

                case 0xEF:
                    instructionString += "ISC: Illegal";
                    break;

                case 0xF0:
                    instructionString += "BEQ: Branch if Equal Relative";
                    operandLength += 1;
                    break;

                case 0xF1:
                    instructionString += "SBC: Subtract with Carry using (Indirect, Y)";
                    operandLength += 1;
                    break;

                case 0xF2:
                    instructionString += "KIL: Halt program (Illegal)";
                    break;

                case 0xF3:
                    instructionString += "ISC: Illegal";
                    break;

                case 0xF4:
                    instructionString += "NOP: Illegal";
                    break;

                case 0xF5:
                    instructionString += "SBC: Subtract with Carry using Zero Page, X";
                    operandLength += 1;
                    break;

                case 0xF6:
                    instructionString += "INC: Increment Zero Page, X";
                    operandLength += 1;
                    break;

                case 0xF7:
                    instructionString += "ISC: Illegal";
                    break;

                case 0xF8:
                    instructionString += "SED: Set Decimal";
                    break;

                case 0xF9:
                    instructionString += "SBC: Subtract with Carry using Absolute, Y";
                    operandLength += 2;
                    break;

                case 0xFA:
                    instructionString += "NOP: Illegal";
                    break;

                case 0xFB:
                    instructionString += "ISC: Illegal";
                    break;

                case 0xFC:
                    instructionString += "NOP: Illegal";
                    break;

                case 0xFD:
                    instructionString += "SBC: Subtract with Carry using Absolute, X";
                    operandLength += 2;
                    break;

                case 0xFE:
                    instructionString += "INC: Increment Absolute, X";
                    operandLength += 2;
                    break;

                case 0xFF:
                    instructionString += "N/A";
                    break;
            }

            return instructionString;
        }


        static void Print(List<byte> binaryFile, string instructionString)
        {
            // Print
            string PCString = "PC: ";

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
            if (binaryFile[(int)PC] <= 0xF)
                opcodeString += $"0x0{binaryFile[(int)PC]:X}";
            else
                opcodeString += $"0x{binaryFile[(int)PC]:X}";

            Console.WriteLine(PCString + " | " + opcodeString + " | " + instructionString);
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

                // Check the NES header (4E 45 53 1A)
                if(binaryFile[0] == 0x4E && binaryFile[1] == 0x45 && binaryFile[2] == 0x53 && binaryFile[3] == 0x1A)
                {
                    numberOfPRGROMBlocks = binaryFile[4];
                    numberOfCHRROMBlocks = binaryFile[5];

                    // Check the mapper type

                    //while (PC < binaryFile.Count)
                    while (PC < 0x8000 * numberOfPRGROMBlocks)
                    {
                        // 
                        //if (PC >= header)
                        {
                            operandLength = 0;
                            b = binaryFile[(int)PC + 0x10];

                            // Fetch

                            // Decode
                            string instructionString = Decode(b);


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
