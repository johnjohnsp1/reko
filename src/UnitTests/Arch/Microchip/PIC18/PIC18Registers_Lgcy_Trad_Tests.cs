﻿#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work of:
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Libraries.Microchip;
using NUnit.Framework;
using Reko.Arch.Microchip.Common;
using Reko.Arch.Microchip.PIC18;

namespace Reko.UnitTests.Arch.Microchip.PIC18.Registers
{
    using static Common.Sample;

    [TestFixture]
    public class PIC18Registers_Lgcy_Trad_Tests
    {
        private PICProcessorMode picMode;
        private PICArchitecture arch;

        public PIC18Registers_Lgcy_Trad_Tests()
        {
            picMode = PICProcessorMode.GetMode(PIC18LegacyName);
            arch = picMode.CreateArchitecture();
            arch.ExecMode = PICExecMode.Traditional;
        }

        [Test]
        public void PIC18Lgcy_GetSubRegisterOfFSR0()
        {
            Assert.AreSame(PIC18Registers.FSR0L, arch.GetSubregister(PIC18Registers.FSR0, 0, 8));
            Assert.AreSame(PIC18Registers.FSR0H, arch.GetSubregister(PIC18Registers.FSR0, 8, 8));
        }

        [Test]
        public void PIC18Lgcy_GetSubRegisterOfFSR1()
        {
            Assert.AreSame(PIC18Registers.FSR1L, arch.GetSubregister(PIC18Registers.FSR1, 0, 8));
            Assert.AreSame(PIC18Registers.FSR1H, arch.GetSubregister(PIC18Registers.FSR1, 8, 8));
        }

        [Test]
        public void PIC18Lgcy_GetSubRegisterOfFSR2()
        {
            Assert.AreSame(PIC18Registers.FSR2L, arch.GetSubregister(PIC18Registers.FSR2, 0, 8));
            Assert.AreSame(PIC18Registers.FSR2H, arch.GetSubregister(PIC18Registers.FSR2, 8, 8));
        }

        [Test]
        public void PIC18Lgcy_GetSubRegisterOfTOS()
        {
            Assert.AreSame(PIC18Registers.TOSL, arch.GetSubregister(PIC18Registers.TOS, 0, 8));
            Assert.AreSame(PIC18Registers.TOSH, arch.GetSubregister(PIC18Registers.TOS, 8, 8));
            Assert.AreSame(PIC18Registers.TOSU, arch.GetSubregister(PIC18Registers.TOS, 16, 8));
        }

        [Test]
        public void PIC18Lgcy_GetSubRegisterOfPC()
        {
            Assert.AreSame(PIC18Registers.PCL, arch.GetSubregister(PIC18Registers.PCLAT, 0, 8));
            Assert.AreSame(PIC18Registers.PCLATH, arch.GetSubregister(PIC18Registers.PCLAT, 8, 8));
            Assert.AreSame(PIC18Registers.PCLATU, arch.GetSubregister(PIC18Registers.PCLAT, 16, 8));
        }

        [Test]
        public void PIC18Lgcy_GetSubRegisterOfTBLPTR()
        {
            Assert.AreSame(PIC18Registers.TBLPTRL, arch.GetSubregister(PIC18Registers.TBLPTR, 0, 8));
            Assert.AreSame(PIC18Registers.TBLPTRH, arch.GetSubregister(PIC18Registers.TBLPTR, 8, 8));
            Assert.AreSame(PIC18Registers.TBLPTRU, arch.GetSubregister(PIC18Registers.TBLPTR, 16, 8));
        }

        [Test]
        public void PIC18Lgcy_BitOffsetOfTOS()
        {
            Assert.AreEqual(0, PIC18Registers.TOSL.BitAddress);
            Assert.AreEqual(8, PIC18Registers.TOSH.BitAddress);
            Assert.AreEqual(16, PIC18Registers.TOSU.BitAddress);
        }

        [Test]
        public void PIC18Lgcy_BitOffsetOfPC()
        {
            Assert.AreEqual(0, PIC18Registers.PCL.BitAddress);
            Assert.AreEqual(8, PIC18Registers.PCLATH.BitAddress);
            Assert.AreEqual(16, PIC18Registers.PCLATU.BitAddress);
        }

    }

}
