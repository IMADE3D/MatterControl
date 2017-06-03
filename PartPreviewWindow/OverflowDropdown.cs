﻿/*
Copyright (c) 2014, Lars Brubaker
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those
of the authors and should not be interpreted as representing official policies,
either expressed or implied, of the FreeBSD Project.
*/

using System;
using System.Collections.Generic;
using System.IO;
using MatterHackers.Agg;
using MatterHackers.Agg.ImageProcessing;
using MatterHackers.Agg.PlatformAbstract;
using MatterHackers.Agg.UI;
using MatterHackers.Localizations;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.PartPreviewWindow
{

	public class OverflowDropdown : PopupButton
	{
		public OverflowDropdown(bool allowLightnessInvert)
			: base(LoadThemedIcon(allowLightnessInvert))
		{
			this.ToolTipText = "More...".Localize();
		}

		public static ImageWidget LoadThemedIcon(bool allowLightnessInvert)
		{
			var imageBuffer = StaticData.Instance.LoadIcon(Path.Combine("ViewTransformControls", "overflow.png"), 32, 32);
			if (!ActiveTheme.Instance.IsDarkTheme && allowLightnessInvert)
			{
				imageBuffer.InvertLightness();
			}

			return new ImageWidget(imageBuffer);
		}
	}

	public class PopupButton : GuiWidget
	{
		private static readonly RGBA_Bytes slightShade = new RGBA_Bytes(0, 0, 0, 40);

		private bool menuVisible = false;
		private bool menuVisibileAtMouseDown = false;

		private PopupWidget popupWidget;

		//private GuiWidget buttonView;

		private GuiWidget buttonView;

		public PopupButton(GuiWidget buttonView)
		{
			this.Margin = 3;
			this.HAnchor = HAnchor.FitToChildren;
			this.VAnchor = VAnchor.FitToChildren;
			this.buttonView = buttonView;

			this.AddChild(buttonView);

			// After the buttonView looses focus, wait a moment for the mouse to arrive at the target and if not found, close the menu
			buttonView.MouseLeaveBounds += (s, e) =>
			{
				if (menuVisible)
				{
					UiThread.RunOnIdle(() =>
					{
						if (this.PopupContent?.UnderMouseState != UnderMouseState.NotUnderMouse)
						{
							return;
						}

						popupWidget?.CloseMenu();
					}, 0.1);
				}
			};
		}

		public Direction PopDirection { get; set; } = Direction.Down;

		public GuiWidget PopupContent { get; set; }

		public Func<GuiWidget> DynamicPopupContent { get; set; }

		public bool AlignToRightEdge { get; set; }

		public override void OnMouseDown(MouseEventArgs mouseEvent)
		{
			// Store the menu state at the time of mousedown
			menuVisibileAtMouseDown = menuVisible;
			base.OnMouseDown(mouseEvent);
		}

		public override void OnMouseUp(MouseEventArgs mouseEvent)
		{
			// Only show the popup if the menu was hidden as the mouse events started
			if (buttonView.MouseCaptured && !menuVisibileAtMouseDown)
			{
				ShowPopup();
				this.BackgroundColor = slightShade;
			}

			base.OnMouseUp(mouseEvent);
		}

		public void ShowPopup()
		{
			menuVisible = true;

			this.PopupContent?.ClearRemovedFlag();

			if (this.DynamicPopupContent != null)
			{
				this.PopupContent = this.DynamicPopupContent();
			}

			if (this.PopupContent == null)
			{
				return;
			}

			popupWidget = new PopupWidget(this.PopupContent, this, Vector2.Zero, this.PopDirection, 0, this.AlignToRightEdge)
			{
				BorderWidth = 1,
				BorderColor = RGBA_Bytes.Gray,
			};

			popupWidget.Closed += (s, e) =>
			{
				this.BackgroundColor = RGBA_Bytes.Transparent;
				menuVisible = false;
				popupWidget = null;
			};
			popupWidget.Focus();
		}
	}
}