﻿/*
Copyright (c) 2018, Lars Brubaker, John Lewin
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
using System.ComponentModel;
using MatterHackers.Agg.VertexSource;
using MatterHackers.DataConverters3D;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.DesignTools
{
	public class SphereObject3D : Object3D, IRebuildable
	{
		public override string ActiveEditor => "PublicPropertyEditor";

		public SphereObject3D()
		{
			Rebuild();
		}

		public double Diameter { get; set; } = 20;
		[DisplayName("Longitude Sides")]
		public int LongitudeSides { get; set; } = 30;
		[DisplayName("Latitude Sides")]
		public int LatitudeSides { get; set; } = 20;

		public void Rebuild()
		{
			var aabb = AxisAlignedBoundingBox.Zero;
			if (Mesh != null)
			{
				this.GetAxisAlignedBoundingBox();
			}
			var path = new VertexStorage();
			var angleDelta = MathHelper.Tau / 2 / LatitudeSides;
			var angle = -MathHelper.Tau / 4;
			var radius = Diameter / 2;
			path.MoveTo(new Vector2(radius * Math.Cos(angle), radius * Math.Sin(angle)));
			for (int i = 0; i < LatitudeSides; i++)
			{
				angle += angleDelta;
				path.LineTo(new Vector2(radius * Math.Cos(angle), radius * Math.Sin(angle)));
			}

			Mesh = VertexSourceToMesh.Revolve(path, LongitudeSides);
			PlatingHelper.PlaceMeshAtHeight(this, aabb.minXYZ.Z);
		}
	}
}