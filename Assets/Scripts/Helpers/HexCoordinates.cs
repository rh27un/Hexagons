using UnityEngine;

[System.Serializable]
public struct HexCoordinates {

	[SerializeField]
	private int x, z;

	public int X {
		get {
			return x;
		}
	}

	public int Z {
		get {
			return z;
		}
	}

	public int Y {
		get {
			return -X - Z;
		}
	}

	public HexCoordinates (int x, int z) {
		this.x = x;
		this.z = z;
	}

	public HexCoordinates[] GetNeighbours()
	{
		HexCoordinates[] neighbours = new HexCoordinates[6];
		neighbours[0] = new HexCoordinates(X, Z + 1);
		neighbours[1] = new HexCoordinates(X + 1, Z);
		neighbours[2] = new HexCoordinates(X + 1, Z - 1);
		neighbours[3] = new HexCoordinates(X, Z - 1);
		neighbours[4] = new HexCoordinates(X - 1, Z);
		neighbours[5] = new HexCoordinates(X - 1, Z + 1);
		return neighbours;
	}
	public static HexCoordinates FromOffsetCoordinates (int x, int z) {
		return new HexCoordinates(x - z / 2, z);
	}

	public static HexCoordinates FromPosition (Vector3 position) {
		float x = position.x / (HexMetrics.innerRadius * 2f);
		float y = -x;

		float offset = position.z / (HexMetrics.outerRadius * 3f);
		x -= offset;
		y -= offset;

		int iX = Mathf.RoundToInt(x);
		int iY = Mathf.RoundToInt(y);
		int iZ = Mathf.RoundToInt(-x -y);

		if (iX + iY + iZ != 0) {
			float dX = Mathf.Abs(x - iX);
			float dY = Mathf.Abs(y - iY);
			float dZ = Mathf.Abs(-x -y - iZ);

			if (dX > dY && dX > dZ) {
				iX = -iY - iZ;
			}
			else if (dZ > dY) {
				iZ = -iX - iY;
			}
		}

		return new HexCoordinates(iX, iZ);
	}

	public override string ToString () {
		return "(" +
			X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
	}

	public string ToStringOnSeparateLines () {
		return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
	}

	public static bool operator ==(HexCoordinates lhs, HexCoordinates rhs) => lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Z == rhs.Z;
	public static bool operator !=(HexCoordinates lhs, HexCoordinates rhs) => !lhs.Equals(rhs);

	public static HexCoordinates zero = new HexCoordinates(0, 0);
}