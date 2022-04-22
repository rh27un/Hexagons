using UnityEngine;

public static class HexMetrics {

	public const float outerRadius = 10f;

	public const float innerRadius = outerRadius * 0.866025404f;

	public static Vector3[] corners = {
		new Vector3(0f, 0f, outerRadius),
		new Vector3(innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(0f, 0f, -outerRadius),
		new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(0f, 0f, outerRadius)
	};
	public static Vector3[] upperCorners = {
		new Vector3(0f, 3f, outerRadius),
		new Vector3(innerRadius, 3f, 0.5f * outerRadius),
		new Vector3(innerRadius, 3f, -0.5f * outerRadius),
		new Vector3(0f, 3f, -outerRadius),
		new Vector3(-innerRadius, 3f, -0.5f * outerRadius),
		new Vector3(-innerRadius, 3f, 0.5f * outerRadius),
		new Vector3(0f, 3f, outerRadius)
	};
	public static Vector3[] wallCorners =
	{
		// Wall 0, top left
			// Triangle 0 i = 0
			corners[0],
			upperCorners[0],
			corners[1],
			// Triangle 1 i = 3
			upperCorners[0],
			upperCorners[1],
			corners[1],
		// Wall 1, left
			// Triangle 0 i = 6
			corners[1],
			upperCorners[1],
			corners[2],
			// Triangle 1 i = 9
			upperCorners[1],
			upperCorners[2],
			corners[2],
		// Wall 2, bottom left
			// Triangle 0 i = 12
			corners[2],
			upperCorners[2],
			corners[3],
			// Triangle 1 i = 15
			upperCorners[2],
			upperCorners[3],
			corners[3],
		// Wall 3, bottom right
			// Triangle 0 i = 18
			corners[3],
			upperCorners[3],
			corners[4],
			// Triangle 1 i = 21
			upperCorners[3],
			upperCorners[4],
			corners[4],
		// Wall 4, right
			// Triangle 0 i = 24
			corners[4],
			upperCorners[4],
			corners[5],
			// Triangle 1 i = 27
			upperCorners[4],
			upperCorners[5],
			corners[5],
		// Wall 5, top right
			//Triangle 0 i = 30
			corners[5],
			upperCorners[5],
			corners[0],
			// Triangle 1 i = 33
			upperCorners[5],
			upperCorners[0],
			corners[0]
	};
}