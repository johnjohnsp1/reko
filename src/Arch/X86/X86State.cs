
#region License
/* 
 * Copyright (C) 1999-2017 John K�ll�n.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Arch.X86
{
    /// <summary>
    /// The state of an X86 processor. Used in the Scanning phase of the decompiler.
    /// </summary>
	public class X86State : ProcessorState
	{
		private ulong [] regs;              // register values
		private ulong [] valid;             // masks out only valid bits
        private uint flags;
        private uint validFlags;
        private IntelArchitecture arch;

        private const int StackItemSize = 2;

		public X86State(IntelArchitecture arch, SegmentMap map) : base(map)
		{
            this.arch = arch;
			this.regs = new ulong[(int)Registers.Max];
			this.valid = new ulong[(int)Registers.Max];
		}

		public X86State(X86State st) : base(st)
		{
            arch = st.arch;
            FpuStackItems = st.FpuStackItems;
            regs = (ulong[])st.regs.Clone();
			valid = (ulong []) st.valid.Clone();
		}

        public override IProcessorArchitecture Architecture { get { return arch; } }
        public int FpuStackItems { get; set; }

		public Address AddressFromSegOffset(RegisterStorage seg, uint offset)
		{
			Constant c = GetRegister(seg);
			if (c.IsValid)
			{
				return arch.ProcessorMode.CreateSegmentedAddress((ushort) c.ToUInt32(), offset & 0xFFFF);
			}
			else
				return null;
		}

        public Address AddressFromSegReg(RegisterStorage seg, RegisterStorage reg)
		{
			Constant c = GetRegister(reg);
			if (c.IsValid)
			{
				return AddressFromSegOffset(seg, c.ToUInt32());
			}
			else 
				return null;
		}

		public override ProcessorState Clone()
		{
			return new X86State(this);
		}

        public bool IsValid(RegisterStorage reg)
        {
            return (valid[reg.Number] & reg.BitMask) == reg.BitMask;
        }

        public override Constant GetRegister(RegisterStorage reg)
        {
            if (IsValid(reg))
            {
                var val = (regs[reg.Number] & reg.BitMask) >> (int)reg.BitAddress;
                return Constant.Create(reg.DataType, val);
            }
            else
                return Constant.Invalid;
        }

		public override void SetRegister(RegisterStorage reg, Constant c)
		{
			if (c == null || !c.IsValid)
			{
                valid[reg.Number] &= ~reg.BitMask;
			}
			else
			{
                valid[reg.Number] |= reg.BitMask;
                regs[reg.Number] = (regs[reg.Number] & ~reg.BitMask) | (c.ToUInt64() << (int)reg.BitAddress);
			}
		}

        public override void SetInstructionPointer(Address addr)
        {
            if (addr.Selector.HasValue)
                SetRegister(Registers.cs, Constant.Word16(addr.Selector.Value));
        }

        public override void OnProcedureEntered()
        {
            FpuStackItems = 0;
            // We're making an assumption that the direction flag is always clear
            // when a procedure is entered. This is true of the vast majority of
            // x86 code out there, and the assumption is certainly made by most
            // compilers and code libraries. If you know the DF flag is set on
            // procedure entry, you can manually set that flag using a user-
            // defined register value.
            SetFlagGroup(arch.GetFlagGroup((uint) FlagM.DF), Constant.False());
        }

        public override void OnProcedureLeft(FunctionType sig)
        {
            sig.FpuStackDelta = FpuStackItems;     
        }

        public override CallSite OnBeforeCall(Identifier sp, int returnAddressSize)
        {
            return new CallSite(returnAddressSize, FpuStackItems);  
        }

        public override void OnAfterCall(FunctionType sig)
        {
            if (sig == null)
                return;

            ShrinkFpuStack(-sig.FpuStackDelta);
        }

		public void GrowFpuStack(Address addrInstr)
		{
			++FpuStackItems;
			if (FpuStackItems > 7)
			{
				Debug.WriteLine(string.Format("Possible FPU stack overflow at address {0}", addrInstr));	//$BUGBUG: should be an exception
			}
		}

        public void ShrinkFpuStack(int cItems)
        {
            FpuStackItems -= cItems;
        }

        public Constant GetFlagGroup(uint mask)
        {
            bool sigle = Bits.IsSingleBitSet(mask);
            if ((mask & validFlags) == mask)
            {
                if (sigle)
                {
                    return Constant.Bool((flags & mask) != 0);
                }
                else {
                    return Constant.Byte((byte)(flags & mask));
                }
            }
            else 
            {
                return Constant.Invalid;
            }
        }

        public void SetFlagGroup(FlagGroupStorage reg, Constant value)
        {
            uint mask = reg.FlagGroupBits;
            if (value.IsValid)
            {
                validFlags |= mask;
                if (value.ToBoolean())
                {
                    this.flags |= mask;
                }
                else
                {
                    this.flags &= ~mask;
                }
            }
            else
            {
                validFlags &= ~mask;
            }
        }

    }
}
