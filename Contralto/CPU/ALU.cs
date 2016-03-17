﻿using System;

namespace Contralto.CPU
{
    //
    // This implements the stripped-down version of the 74181 ALU 
    // that the Alto exposes to the microcode, and nothing more.
    //
    static class ALU
    {
        static ALU()
        {
            Reset();
        }

        public static void Reset()
        {
            _carry = 0;
        }

        public static int Carry
        {
            get { return _carry; }
            set { _carry = value; }
        }

        public static ushort Execute(AluFunction fn, ushort bus, ushort t, int skip)
        {
            int r;
            switch (fn)
            {
                case AluFunction.Bus:
                    _carry = 0;     // M = 1
                    r = bus;
                    break;

                case AluFunction.T:
                    _carry = 0;     // M = 1
                    r = t;
                    break;

                case AluFunction.BusOrT:
                    _carry = 0;     // M = 1
                    r = (bus | t);
                    break;

                case AluFunction.BusAndT:
                case AluFunction.AluBusAndT:
                    _carry = 0;     // M = 1
                    r = (bus & t);
                    break;

                case AluFunction.BusXorT:
                    _carry = 0;     // M = 1
                    r = (bus ^ t);
                    break;

                case AluFunction.BusPlus1:
                    r = bus + 1;
                    _carry = (r > 0xffff) ? 1 : 0;
                    break;

                case AluFunction.BusMinus1:
                    r = bus - 1;

                    // Just for clarification; the datasheet specifies:
                    // "Because subtraction is actually performed by complementary
                    //  addition (1s complement), a carry out means borrow; thus,
                    //  a carry is generated when there is no underflow and no carry
                    //  is generated when there is underflow."
                    _carry = (r < 0) ? 0 : 1;
                    break;

                case AluFunction.BusPlusT:
                    r = bus + t;
                    _carry = (r > 0xffff) ? 1 : 0;
                    break;

                case AluFunction.BusMinusT:
                    r = bus - t;
                    _carry = (r < 0) ? 0 : 1;
                    break;

                case AluFunction.BusMinusTMinus1:
                    r = bus - t - 1;
                    _carry = (r < 0) ? 0 : 1;
                    break;

                case AluFunction.BusPlusTPlus1:
                    r = bus + t + 1;
                    _carry = (r > 0xffff) ? 1 : 0;
                    break;

                case AluFunction.BusPlusSkip:
                    r = bus + skip;
                    _carry = (r > 0xffff) ? 1 : 0;
                    break;

                case AluFunction.BusAndNotT:
                    r = bus & (~t);
                    _carry = 0;
                    break;

                default:
                    throw new InvalidOperationException(String.Format("Unhandled ALU function {0}", fn));
            }     
       
            return (ushort)r;
        }

        private static int _carry;
    }
}
