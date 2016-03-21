﻿#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Types;
using Reko.Gui.Forms;
using Reko.Gui.Windows.Controls;
using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.Gui.Windows
{
    /// <summary>
    /// This class manages user interaction with the LowLevelView control.
    /// </summary>
    public class LowLevelViewInteractor : IWindowPane, ICommandTarget
    {
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        private IServiceProvider services;
        private LowLevelView control;
        private TypeMarker typeMarker;
        private Program program;
        private bool ignoreAddressChange;
        private NavigationInteractor<Address> navInteractor;

        public LowLevelView Control { get { return control; } }

        public Program Program
        {
            get { return program; }
            set
            {
                program = value;
                if (value != null)
                {
                    control.MemoryView.ImageMap = value.ImageMap;
                    control.MemoryView.Architecture = value.Architecture;
                    control.DisassemblyView.Program = value;
                    var seg = program.ImageMap.Segments.Values.First();
                    control.DisassemblyView.Program = value;
                    control.DisassemblyView.Model = new DisassemblyTextModel(value, seg);
                    control.ImageMapView.ImageMap = value.ImageMap;
                    control.ImageMapView.Granularity = value.ImageMap.GetExtent();
                    control.ByteMapView.ImageMap = value.ImageMap;
                }
            }
        }

        public virtual Address SelectedAddress
        {
            get { return control.MemoryView.SelectedAddress; }
            set
            {
                control.CurrentAddress = value;
            }
        }

        public Control CreateControl()
        {
            var uiService = services.RequireService<IDecompilerShellUiService>();
            var uiPrefsSvc = services.RequireService<IUiPreferencesService>();
            this.control = new LowLevelView();
            this.Control.Font = new Font("Lucida Console", 10F); //$TODO: use user preference
            this.Control.CurrentAddressChanged += LowLevelView_CurrentAddressChanged;

            this.Control.ImageMapView.SelectedAddressChanged += ImageMapView_SelectedAddressChanged;

            this.Control.MemoryView.SelectionChanged += MemoryView_SelectionChanged;
            this.Control.MemoryView.ContextMenu = uiService.GetContextMenu(MenuIds.CtxMemoryControl);
            this.control.MemoryView.Services = this.services;

            this.Control.DisassemblyView.StyleClass = UiStyles.Disassembler;
            this.Control.DisassemblyView.SelectedObjectChanged += DisassemblyView_SelectedObjectChanged;
            this.Control.DisassemblyView.ContextMenu = uiService.GetContextMenu(MenuIds.CtxDisassembler);
            this.Control.DisassemblyView.Services = this.services;
            this.Control.DisassemblyView.Navigate += DisassemblyControl_Navigate;

            this.Control.ToolBarGoButton.Click += ToolBarGoButton_Click;
            this.Control.ToolBarAddressTextbox.KeyDown += ToolBarAddressTextbox_KeyDown;

            this.navInteractor = new NavigationInteractor<Address>();
            this.navInteractor.Attach(this.Control);

            typeMarker = new TypeMarker(control.MemoryView);

            return control;
        }

        public void SetSite(IServiceProvider sp)
        {
            services = sp;
        }

        public void Close()
        {
        }

        private void NavigateToToolbarAddress()
        {
            Address addr;
            var txtAddr = Control.ToolBarAddressTextbox.Text.Trim();
            if (txtAddr.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
                txtAddr = txtAddr.Substring(2);
            if (!program.Architecture.TryParseAddress(txtAddr, out addr))
                return;
            UserNavigateToAddress(Control.MemoryView.TopAddress, addr);
        }

        private void UserNavigateToAddress(Address addrFrom, Address addrTo)
        {
            if (!program.ImageMap.IsValidAddress(addrTo))
                return;
            navInteractor.RememberAddress(addrFrom);
            control.CurrentAddress = addrTo;        // ...and move to the new position.
        }

        public bool QueryStatus(CommandID cmdId, CommandStatus status, CommandText text)
        {
            if (Control.MemoryView.Focused)
            {
                if (cmdId.Guid == CmdSets.GuidReko)
                {
                    switch (cmdId.ID)
                    {
                    case CmdIds.ViewGoToAddress:
                    case CmdIds.ActionMarkType:
                    case CmdIds.ViewFindWhatPointsHere:
                    case CmdIds.ActionMarkProcedure:
                    case CmdIds.TextEncodingChoose:
                        status.Status = MenuStatus.Visible | MenuStatus.Enabled; return true;
                    case CmdIds.EditCopy:
                    case CmdIds.ViewFindPattern:
                        status.Status = ValidSelection()
                            ? MenuStatus.Visible | MenuStatus.Enabled
                            : MenuStatus.Visible;
                        return true;
                    }
                }
            }
            else if (Control.DisassemblyView.Focused)
            {
                var selAddress = Control.DisassemblyView.SelectedObject as Address;
                var instr = Control.DisassemblyView.SelectedObject as MachineInstruction;
                if (cmdId.Guid == CmdSets.GuidReko)
                {
                    switch (cmdId.ID)
                    {
                    case CmdIds.OpenLink:
                    case CmdIds.OpenLinkInNewWindow:
                        status.Status = selAddress != null ? MenuStatus.Visible | MenuStatus.Enabled : 0;
                        return true;
                    case CmdIds.EditAnnotation:
                        status.Status = instr != null ? MenuStatus.Visible | MenuStatus.Enabled : 0;
                        return true;
                    case CmdIds.TextEncodingChoose:
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Execute(CommandID cmdId)
        {
            if (Control.MemoryView.Focused)
            {
                if (cmdId.Guid == CmdSets.GuidReko)
                {
                    switch (cmdId.ID)
                    {
                    case CmdIds.EditCopy: return CopySelectionToClipboard();
                    case CmdIds.ViewGoToAddress: GotoAddress(); return true;
                    case CmdIds.ActionMarkType: return MarkType();
                    case CmdIds.ActionMarkProcedure: MarkAndScanProcedure(); return true;
                    case CmdIds.ViewFindWhatPointsHere: return ViewWhatPointsHere();
                    case CmdIds.ViewFindPattern: return ViewFindPattern();
                    case CmdIds.TextEncodingChoose: return ChooseTextEncoding();
                    }
                }
            }
            else if (Control.DisassemblyView.Focused)
            {
                if (cmdId.Guid == CmdSets.GuidReko)
                {
                    switch (cmdId.ID)
                    {
                    case CmdIds.EditAnnotation: return EditDasmAnnotation();
                    case CmdIds.TextEncodingChoose: return ChooseTextEncoding();
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the range of address that are selected in the memory view.
        /// </summary>
        /// <returns></returns>
        public virtual AddressRange GetSelectedAddressRange()
        {
            return control.MemoryView.GetAddressRange();
        }

        public void GotoAddress()
        {
            AddressRange addrRange = GetSelectedAddressRange();
            if (addrRange == null)
                return;
            var rdr = program.CreateImageReader(addrRange.Begin);
            var addrDst = rdr.Read(program.Platform.PointerType);
            var txt = control.ToolBarAddressTextbox;
            txt.Text = addrDst.ToString();
            txt.SelectAll();
            txt.Focus();
        }

        public void InvalidateControl()
        {
            control.Invalidate();
        }

        public void MarkAndScanProcedure()
        {
            AddressRange addrRange;
            if (!TryGetSelectedAddressRange(out addrRange))
                return;
            var address = new ProgramAddress(program, addrRange.Begin);
            services.RequireService<ICommandFactory>().MarkProcedure(address).Do();
        }

        private bool TryGetSelectedAddressRange(out AddressRange addrRange)
        {
            addrRange = null;
            if (control.MemoryView.Focused)
            {
                addrRange = control.MemoryView.GetAddressRange();
                if (!addrRange.IsValid)
                    return false;
            }
            else if (control.DisassemblyView.Focused)
            {
                var addr = control.DisassemblyView.SelectedObject as Address;
                if (addr == null)
                    return false;
                addrRange = new AddressRange(addr, addr);
                return true;
            }
            return true;
        }

        /// <summary>
        /// Copies the selected range of bytes into the clipboard.
        /// </summary>
        /// <returns></returns>
        private bool CopySelectionToClipboard()
        {
            AddressRange range;
            if (!TryGetSelectedAddressRange(out range))
                return true;
            if (control.MemoryView.Focused)
            {
                 var decompiler = services.GetService<IDecompilerService>().Decompiler;
                 var dumper = new Dumper(decompiler.Project.Programs.First().Architecture);
                var sb = new StringWriter();
                dumper.DumpData(control.MemoryView.ImageMap, range, sb);
                Clipboard.SetText(sb.ToString());       //$TODO: abstract this.
            }
            return true;
        }

        public bool MarkType()
        {
            var addrRange = control.MemoryView.GetAddressRange();
            if (addrRange.IsValid)
            {
                typeMarker.Show(
                    control.MemoryView.AddressToPoint(addrRange.Begin),
                    typeMarker_TextAccepted);
            }
            return true;
        }

        private void typeMarker_TextAccepted(string text)
        {
            var item = SetTypeAtAddressRange(GetSelectedAddressRange().Begin, text);
            if (item == null)
                return;
            // Advance selection to beyond item.
            this.SelectedAddress = item.Address + item.Size;
        }

        private bool ValidSelection()
        {
            if (control.MemoryView.Focused)
            {
                var addrRange = control.MemoryView.GetAddressRange();
                if (addrRange.IsValid)
                    return true;
            }
            return false;
        }

        public ImageMapItem SetTypeAtAddressRange(Address address, string userText)
        {
            var parser = new HungarianParser();
            var dataType = parser.Parse(userText);
            if (dataType == null)
                return null;
            var arr = dataType as ArrayType;
            if (arr != null && arr.ElementType.Size != 0)
            {
                var range = control.MemoryView.GetAddressRange();
                if (range.IsValid)
                {
                    long size = (range.End - range.Begin) + 1;
                    int nElems = (int)(size / arr.ElementType.Size);
                    arr.Length = nElems;
                }
            }
            var item = program.AddUserGlobalItem(address, dataType);
            control.MemoryView.Invalidate();
            return item;
        }

        public bool ViewWhatPointsHere()
        {
            AddressRange addrRange = control.MemoryView.GetAddressRange();
            if (!addrRange.IsValid)
                return true;
            if (program == null)
                return true;
            services.RequireService<ICommandFactory>().ViewWhatPointsHere(program, addrRange.Begin).Do();
            return true;
        }

        public bool ViewFindPattern()
        {
            AddressRange addrRange = control.MemoryView.GetAddressRange();
            if (!addrRange.IsValid || program == null)
                return true;
            var dlgFactory = services.RequireService<IDialogFactory>();
            var uiSvc = services.RequireService<IDecompilerShellUiService>();
            var srSvc = services.RequireService<ISearchResultService>();
            using (ISearchDialog dlg = dlgFactory.CreateSearchDialog())
            {
                dlg.InitialPattern = SelectionToHex(addrRange);
                if (uiSvc.ShowModalDialog(dlg) == DialogResult.OK)
                {
                    var re = Scanning.Dfa.Automaton.CreateFromPattern(dlg.Patterns.Text);
                    var hits = 
                        //$BUG: wrong result
                        program.ImageMap.Segments.Values
                        .SelectMany(s => re.GetMatches(s.MemoryArea.Bytes, 0))
                        .Select(offset => new ProgramAddress(
                            program,
                            program.ImageMap.BaseAddress + offset));
                    srSvc.ShowAddressSearchResults(hits, AddressSearchDetails.Code);
                }
            }
            return true;
        }

        public bool ChooseTextEncoding()
        {
            var dlgFactory = services.RequireService<IDialogFactory>();
            var uiSvc = services.RequireService<IDecompilerShellUiService>();
            using (ITextEncodingDialog dlg = dlgFactory.CreateTextEncodingDialog())
            {
                if (uiSvc.ShowModalDialog(dlg) == DialogResult.OK)
                {
                    var enc = dlg.GetSelectedTextEncoding();
                    program.Platform.DefaultTextEncoding = enc;
                }
            }
            return true;
        }

        public bool EditDasmAnnotation()
        {
            return true;
        }

        private string SelectionToHex(AddressRange addr)
        {
            var sb = new StringBuilder();
            var rdr = program.CreateImageReader(addr.Begin);
            var sep = "";
            while (rdr.Address <= addr.End)
            {
                sb.Append(sep);
                sep = " ";
                sb.AppendFormat("{0:X2}", (uint)rdr.ReadByte());
            }
            return sb.ToString();
        }

        void ImageMapView_SelectedAddressChanged(object sender, EventArgs e)
        {
            if (ignoreAddressChange)
                return;
            var addr = Control.ImageMapView.SelectedAddress; 
            this.ignoreAddressChange = true;
            this.Control.MemoryView.SelectedAddress = addr;
            this.Control.MemoryView.TopAddress = addr;
            this.Control.DisassemblyView.SelectedObject = addr;
            this.control.DisassemblyView.TopAddress = addr;
            this.SelectionChanged.Fire(this, new SelectionChangedEventArgs(new AddressRange(addr, addr)));
            UserNavigateToAddress(Control.MemoryView.TopAddress, addr);
            this.ignoreAddressChange = false;
        }

        private void MemoryView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ignoreAddressChange)
                return;
            this.ignoreAddressChange = true;
            this.Control.DisassemblyView.SelectedObject = e.AddressRange.Begin;
            this.Control.DisassemblyView.TopAddress = e.AddressRange.Begin;
            this.SelectionChanged.Fire(this, e);
            this.ignoreAddressChange = false;
        }

        void DisassemblyView_SelectedObjectChanged(object sender, EventArgs e)
        {
            var selectedAddr = Control.DisassemblyView.SelectedObject as Address;
            if (ignoreAddressChange || selectedAddr == null)
                return;
            this.ignoreAddressChange = true;
            this.Control.MemoryView.SelectedAddress = selectedAddr;
            this.Control.MemoryView.TopAddress = selectedAddr;
            this.ignoreAddressChange = false;
        }

        void LowLevelView_CurrentAddressChanged(object sender, EventArgs e)
        {
            ignoreAddressChange = true;
            var value = Control.CurrentAddress;
            var addrTop = value - ((int)value.ToLinear() & 0x0F);
            control.MemoryView.SelectedAddress = value;
            control.MemoryView.TopAddress = addrTop;
            control.DisassemblyView.TopAddress = value;
            ignoreAddressChange = false;
        }

        void ToolBarAddressTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData != Keys.Return)
                return;
            e.Handled = true;
            e.SuppressKeyPress = true;
            NavigateToToolbarAddress();
        }

        void ToolBarGoButton_Click(object sender, EventArgs e)
        {
            if (ignoreAddressChange)
                return;
            NavigateToToolbarAddress();
        }

        private void DisassemblyControl_Navigate(object sender, EditorNavigationArgs e)
        {
            var addr = e.Destination as Address;
            if (addr == null)
                return;
            UserNavigateToAddress(Control.DisassemblyView.TopAddress, addr);
        }
    }
}
