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

using System.Threading;
using MatterHackers.Agg.VertexSource;
using MatterHackers.DataConverters3D;
using MatterHackers.PolygonMesh;
using MatterHackers.VectorMath;

namespace MatterHackers.MatterControl.DesignTools
{
	public class CylinderObject3D : Object3D, IRebuildable
	{
		public override string ActiveEditor => "PublicPropertyEditor";

		public CylinderObject3D()
		{
		}

		public static CylinderObject3D Create()
		{
			var item = new CylinderObject3D();

			item.Rebuild();
			return item;
		}

		public double Diameter { get; set; } = 20;
		public double Height { get; set; } = 20;
		public int Sides { get; set; } = 30;

		public void Rebuild()
		{
			var aabb = this.GetAxisAlignedBoundingBox();

			var path = new VertexStorage();
			path.MoveTo(0, 0);
			path.LineTo(Diameter / 2, 0);
			path.LineTo(Diameter / 2, Height);
			path.LineTo(0, Height);

			Mesh = VertexSourceToMesh.Revolve(path, Sides);
			Mesh.CleanAndMergeMesh(CancellationToken.None);
			if (aabb.ZSize > 0)
			{
				// If the part was already created and at a height, maintain the height.
				PlatingHelper.PlaceMeshAtHeight(this, aabb.minXYZ.Z);
			}
		}
	}
}