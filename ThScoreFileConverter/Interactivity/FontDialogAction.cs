﻿//-----------------------------------------------------------------------
// <copyright file="FontDialogAction.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using ThScoreFileConverter.Properties;
using SysDraw = System.Drawing;
using WinForms = System.Windows.Forms;

namespace ThScoreFileConverter.Interactivity
{
    /// <summary>
    /// Encapsulates the handling of <see cref="WinForms.FontDialog"/>.
    /// </summary>
    public class FontDialogAction : CommonDialogAction
    {
        #region Dependency properties

        /// <summary>Identifies the <see cref="ApplyCommand"/> dependency property.</summary>
        public static readonly DependencyProperty ApplyCommandProperty =
            DependencyProperty.Register(
                nameof(ApplyCommand),
                typeof(ICommand),
                typeof(FontDialogAction),
                new UIPropertyMetadata(defaultValue: null));

        /// <summary>Identifies the <see cref="AllowScriptChange"/> dependency property.</summary>
        public static readonly DependencyProperty AllowScriptChangeProperty = DependencyProperty.Register(
            nameof(AllowScriptChange), typeof(bool), typeof(FontDialogAction), new UIPropertyMetadata(true));

        /// <summary>Identifies the <see cref="AllowSimulations"/> dependency property.</summary>
        public static readonly DependencyProperty AllowSimulationsProperty = DependencyProperty.Register(
            nameof(AllowSimulations), typeof(bool), typeof(FontDialogAction), new UIPropertyMetadata(true));

        /// <summary>Identifies the <see cref="AllowVectorFonts"/> dependency property.</summary>
        public static readonly DependencyProperty AllowVectorFontsProperty = DependencyProperty.Register(
            nameof(AllowVectorFonts), typeof(bool), typeof(FontDialogAction), new UIPropertyMetadata(true));

        /// <summary>Identifies the <see cref="AllowVerticalFonts"/> dependency property.</summary>
        public static readonly DependencyProperty AllowVerticalFontsProperty = DependencyProperty.Register(
            nameof(AllowVerticalFonts), typeof(bool), typeof(FontDialogAction), new UIPropertyMetadata(true));

        /// <summary>Identifies the <see cref="Color"/> dependency property.</summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
                nameof(Color),
                typeof(SysDraw.Color),
                typeof(FontDialogAction),
                new UIPropertyMetadata(SysDraw.Color.Black));

        /// <summary>Identifies the <see cref="FixedPitchOnly"/> dependency property.</summary>
        public static readonly DependencyProperty FixedPitchOnlyProperty = DependencyProperty.Register(
            nameof(FixedPitchOnly), typeof(bool), typeof(FontDialogAction), new UIPropertyMetadata(false));

        /// <summary>Identifies the <see cref="Font"/> dependency property.</summary>
        public static readonly DependencyProperty FontProperty =
            DependencyProperty.Register(
                nameof(Font),
                typeof(SysDraw.Font),
                typeof(FontDialogAction),
                new UIPropertyMetadata(SysDraw.SystemFonts.DefaultFont));

        /// <summary>Identifies the <see cref="FontMustExist"/> dependency property.</summary>
        public static readonly DependencyProperty FontMustExistProperty = DependencyProperty.Register(
            nameof(FontMustExist), typeof(bool), typeof(FontDialogAction), new UIPropertyMetadata(false));

        /// <summary>Identifies the <see cref="MaxSize"/> dependency property.</summary>
        public static readonly DependencyProperty MaxSizeProperty = DependencyProperty.Register(
            nameof(MaxSize), typeof(int), typeof(FontDialogAction), new UIPropertyMetadata(0));

        /// <summary>Identifies the <see cref="MinSize"/> dependency property.</summary>
        public static readonly DependencyProperty MinSizeProperty = DependencyProperty.Register(
            nameof(MinSize), typeof(int), typeof(FontDialogAction), new UIPropertyMetadata(0));

        /// <summary>Identifies the <see cref="ScriptsOnly"/> dependency property.</summary>
        public static readonly DependencyProperty ScriptsOnlyProperty = DependencyProperty.Register(
            nameof(ScriptsOnly), typeof(bool), typeof(FontDialogAction), new UIPropertyMetadata(false));

        /// <summary>Identifies the <see cref="ShowApply"/> dependency property.</summary>
        public static readonly DependencyProperty ShowApplyProperty = DependencyProperty.Register(
            nameof(ShowApply), typeof(bool), typeof(FontDialogAction), new UIPropertyMetadata(false));

        /// <summary>Identifies the <see cref="ShowColor"/> dependency property.</summary>
        public static readonly DependencyProperty ShowColorProperty = DependencyProperty.Register(
            nameof(ShowColor), typeof(bool), typeof(FontDialogAction), new UIPropertyMetadata(false));

        /// <summary>Identifies the <see cref="ShowEffects"/> dependency property.</summary>
        public static readonly DependencyProperty ShowEffectsProperty = DependencyProperty.Register(
            nameof(ShowEffects), typeof(bool), typeof(FontDialogAction), new UIPropertyMetadata(true));

        /// <summary>Identifies the <see cref="ShowHelp"/> dependency property.</summary>
        public static readonly DependencyProperty ShowHelpProperty = DependencyProperty.Register(
            nameof(ShowHelp), typeof(bool), typeof(FontDialogAction), new UIPropertyMetadata(false));

        #endregion

        #region CLR properties

        /// <summary>
        /// Gets or sets the command invoked when the user clicks the <c>Apply</c> button in the font dialog
        /// box.
        /// </summary>
        public ICommand? ApplyCommand
        {
            get => this.GetValue(ApplyCommandProperty) as ICommand;
            set => this.SetValue(ApplyCommandProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can change the character set specified in the
        /// <c>Script</c> combo box to display a character set other than the one currently displayed.
        /// </summary>
        public bool AllowScriptChange
        {
            get => (bool)this.GetValue(AllowScriptChangeProperty);
            set => this.SetValue(AllowScriptChangeProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dialog box allows graphics device interface (GDI)
        /// font simulations.
        /// </summary>
        public bool AllowSimulations
        {
            get => (bool)this.GetValue(AllowSimulationsProperty);
            set => this.SetValue(AllowSimulationsProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dialog box allows vector font selections.
        /// </summary>
        public bool AllowVectorFonts
        {
            get => (bool)this.GetValue(AllowVectorFontsProperty);
            set => this.SetValue(AllowVectorFontsProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dialog box displays both vertical and horizontal fonts
        /// or only horizontal fonts.
        /// </summary>
        public bool AllowVerticalFonts
        {
            get => (bool)this.GetValue(AllowVerticalFontsProperty);
            set => this.SetValue(AllowVerticalFontsProperty, value);
        }

        /// <summary>
        /// Gets or sets the selected font color.
        /// </summary>
        public SysDraw.Color Color
        {
            get => (SysDraw.Color)this.GetValue(ColorProperty);
            set => this.SetValue(ColorProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dialog box allows only the selection of fixed-pitch
        /// fonts.
        /// </summary>
        public bool FixedPitchOnly
        {
            get => (bool)this.GetValue(FixedPitchOnlyProperty);
            set => this.SetValue(FixedPitchOnlyProperty, value);
        }

        /// <summary>
        /// Gets or sets the selected font.
        /// </summary>
        public SysDraw.Font Font
        {
            get => (SysDraw.Font)this.GetValue(FontProperty);
            set => this.SetValue(FontProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dialog box specifies an error condition if the user
        /// attempts to select a font or style that does not exist.
        /// </summary>
        public bool FontMustExist
        {
            get => (bool)this.GetValue(FontMustExistProperty);
            set => this.SetValue(FontMustExistProperty, value);
        }

        /// <summary>
        /// Gets or sets the maximum point size a user can select.
        /// </summary>
        public int MaxSize
        {
            get => (int)this.GetValue(MaxSizeProperty);
            set => this.SetValue(MaxSizeProperty, value);
        }

        /// <summary>
        /// Gets or sets the minimum point size a user can select.
        /// </summary>
        public int MinSize
        {
            get => (int)this.GetValue(MinSizeProperty);
            set => this.SetValue(MinSizeProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dialog box allows selection of fonts for all non-OEM
        /// and Symbol character sets, as well as the ANSI character set.
        /// </summary>
        public bool ScriptsOnly
        {
            get => (bool)this.GetValue(ScriptsOnlyProperty);
            set => this.SetValue(ScriptsOnlyProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dialog box contains an <c>Apply</c> button.
        /// </summary>
        public bool ShowApply
        {
            get => (bool)this.GetValue(ShowApplyProperty);
            set => this.SetValue(ShowApplyProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dialog box displays the color choice.
        /// </summary>
        public bool ShowColor
        {
            get => (bool)this.GetValue(ShowColorProperty);
            set => this.SetValue(ShowColorProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dialog box contains controls that allow the user to
        /// specify strikethrough, underline, and text color options.
        /// </summary>
        public bool ShowEffects
        {
            get => (bool)this.GetValue(ShowEffectsProperty);
            set => this.SetValue(ShowEffectsProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dialog box displays a Help button.
        /// </summary>
        public bool ShowHelp
        {
            get => (bool)this.GetValue(ShowHelpProperty);
            set => this.SetValue(ShowHelpProperty, value);
        }

        #endregion

        /// <summary>
        /// Creates a new <see cref="WinForms.FontDialog"/> instance.
        /// </summary>
        /// <returns>A created <see cref="WinForms.FontDialog"/> instance.</returns>
        internal WinForms.FontDialog CreateDialog()
        {
            return new WinForms.FontDialog
            {
                AllowScriptChange = this.AllowScriptChange,
                AllowSimulations = this.AllowSimulations,
                AllowVectorFonts = this.AllowVectorFonts,
                AllowVerticalFonts = this.AllowVerticalFonts,
                Color = this.Color,
                FixedPitchOnly = this.FixedPitchOnly,
                Font = this.Font,
                FontMustExist = this.FontMustExist,
                MaxSize = this.MaxSize,
                MinSize = this.MinSize,
                ScriptsOnly = this.ScriptsOnly,
                ShowApply = this.ShowApply,
                ShowColor = this.ShowColor,
                ShowEffects = this.ShowEffects,
                ShowHelp = this.ShowHelp,
                Site = this.Site,
                Tag = this.Tag,
            };
        }

        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <param name="parameter">The parameter to the action; but not used.</param>
        protected override void Invoke(object parameter)
        {
            static void ExecuteCommand(ICommand command, SysDraw.Font font, SysDraw.Color color)
            {
                var result = new FontDialogActionResult(font, color);
                if (command.CanExecute(result))
                    command.Execute(result);
            }

            using var dialog = this.CreateDialog();
            using var disposable = new SingleAssignmentDisposable();

            if (this.ShowApply && (this.ApplyCommand is not null))
            {
                disposable.Disposable = Observable
                    .FromEvent<EventHandler, EventArgs>(
                        h => (sender, e) => h(e), h => dialog.Apply += h, h => dialog.Apply -= h)
                    .Subscribe(_ => ExecuteCommand(this.ApplyCommand, dialog.Font, dialog.Color));
            }

            var oldFont = dialog.Font;
            var oldColor = dialog.Color;
            var dialogResult = dialog.ShowDialog(new Win32Window(this.Owner));

            switch (dialogResult)
            {
                case WinForms.DialogResult.OK:
                    if (this.OkCommand is not null)
                        ExecuteCommand(this.OkCommand, dialog.Font, dialog.Color);
                    break;

                case WinForms.DialogResult.Cancel:
                    if (this.CancelCommand is not null)
                        ExecuteCommand(this.CancelCommand, oldFont, oldColor);
                    break;

                default:
                    throw new NotImplementedException(Resources.NotImplementedExceptionShouldNotReachHere);
            }
        }
    }
}
