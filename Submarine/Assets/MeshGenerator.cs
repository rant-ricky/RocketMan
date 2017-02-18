using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshGenerator : MonoBehaviour {

	public SquareGrid squareGrid;
	List<Vector3> vertices;
	List<int> triangles;
	List<int> outlineIndices = new List<int> ();
	Vector3[] outlinePositions;
	List<Triangle> actualTriangles;


	Dictionary<int,List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>> ();

	public class Triangle {
		public int vertexIndexA;
		public int vertexIndexB;
		public int vertexIndexC;
		public int[] vertices;

		public Triangle(int posA, int posB, int posC) {
			this.vertexIndexA = posA;
			this.vertexIndexB = posB;
			this.vertexIndexC = posC;

			vertices = new int[3];
			vertices[0] = posA;
			vertices[1] = posB;
			vertices[2] = posC;
		}

		public int this[int i] {
			get {
				return vertices[i];
			}
		}


		public bool Contains(int vertexIndex) {
			return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
		}
	}
	public void createOutline() {
		for (int index = 0; index < outlineIndices.Count; index++) {
			outlinePositions [index] = vertices [outlineIndices [index]];
		}
	}
	public void searchForOutlineEdges() {
		foreach (Triangle triangle in actualTriangles) {
			if (numberOfTrianglesWithVertexPair(triangle.vertexIndexA, triangle.vertexIndexB) == 1) {
				//this is an outline edge.

				if (!outlineIndices.Contains(triangle.vertexIndexA)) {
					outlineIndices.Add(triangle.vertexIndexA);
				}
				if (!outlineIndices.Contains(triangle.vertexIndexB)) {
					outlineIndices.Add(triangle.vertexIndexB);
				}
			}
			if (numberOfTrianglesWithVertexPair(triangle.vertexIndexB, triangle.vertexIndexC) == 1) {
				//this is an outline edge.
				if (!outlineIndices.Contains(triangle.vertexIndexB)) {
					outlineIndices.Add(triangle.vertexIndexB);
				}
				if (!outlineIndices.Contains(triangle.vertexIndexC)) {
					outlineIndices.Add(triangle.vertexIndexC);
				}
			}
			if (numberOfTrianglesWithVertexPair(triangle.vertexIndexA, triangle.vertexIndexC) == 1) {
				if (!outlineIndices.Contains(triangle.vertexIndexA)) {
					outlineIndices.Add(triangle.vertexIndexA);
				}
				if (!outlineIndices.Contains(triangle.vertexIndexC)) {
					outlineIndices.Add(triangle.vertexIndexC);
				}
			}
		}
	}
	public int numberOfTrianglesWithVertexPair(int vertexA, int vertexB) {
		int numberOfTrinaglesWithVertexPair = 0;
		foreach (Triangle triangle in actualTriangles) {
			if (triangle.Contains (vertexA) && triangle.Contains (vertexB)) {
				numberOfTrinaglesWithVertexPair++;
			}
		}
		return numberOfTrinaglesWithVertexPair;	//if outline edge this should return only 1.

	}
	public void GenerateMesh(int[,] map, float squareSize) {
		squareGrid = new SquareGrid(map, squareSize);

		vertices = new List<Vector3>();
		triangles = new List<int>();
		actualTriangles = new List<Triangle> ();

		for (int x = 0; x < squareGrid.squares.GetLength(0); x ++) {
			for (int y = 0; y < squareGrid.squares.GetLength(1); y ++) {
				TriangulateSquare(squareGrid.squares[x,y]);
			}
		}

		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();

		searchForOutlineEdges ();
		outlinePositions = new Vector3[outlineIndices.Count];
		createOutline ();
		LineRenderer line = GetComponent<LineRenderer> ();
		line.numPositions = outlinePositions.Length;
		line.SetPositions (outlinePositions);
	}

	void TriangulateSquare(Square square) {
		switch (square.configuration) {
		case 0:
			break;

			// 1 points:
		case 1:
			MeshFromPoints(square.centreBottom, square.bottomLeft, square.centreLeft);
			break;
		case 2:
			MeshFromPoints(square.centreRight, square.bottomRight, square.centreBottom);
			break;
		case 4:
			MeshFromPoints(square.centreTop, square.topRight, square.centreRight);
			break;
		case 8:
			MeshFromPoints(square.topLeft, square.centreTop, square.centreLeft);
			break;

			// 2 points:
		case 3:
			MeshFromPoints(square.centreRight, square.bottomRight, square.bottomLeft, square.centreLeft);
			break;
		case 6:
			MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.centreBottom);
			break;
		case 9:
			MeshFromPoints(square.topLeft, square.centreTop, square.centreBottom, square.bottomLeft);
			break;
		case 12:
			MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreLeft);
			break;
		case 5:
			MeshFromPoints(square.centreTop, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft, square.centreLeft);
			break;
		case 10:
			MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.centreBottom, square.centreLeft);
			break;

			// 3 point:
		case 7:
			MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.bottomLeft, square.centreLeft);
			break;
		case 11:
			MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.bottomLeft);
			break;
		case 13:
			MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft);
			break;
		case 14:
			MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centreBottom, square.centreLeft);
			break;

			// 4 point:
		case 15:
			MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
			break;
		}
	}

	void MeshFromPoints(params Node[] points) {
		AssignVertices(points);

		if (points.Length >= 3)
			CreateTriangle(points[0], points[1], points[2]);
		if (points.Length >= 4)
			CreateTriangle(points[0], points[2], points[3]);
		if (points.Length >= 5) 
			CreateTriangle(points[0], points[3], points[4]);
		if (points.Length >= 6)
			CreateTriangle(points[0], points[4], points[5]);
	}

	void AssignVertices(Node[] points) {
		for (int i = 0; i < points.Length; i ++) {
			if (points[i].vertexIndex == -1) {
				points[i].vertexIndex = vertices.Count;
				vertices.Add(points[i].position);
			}
		}
	}

	void CreateTriangle(Node a, Node b, Node c) {
		triangles.Add(a.vertexIndex);
		triangles.Add(b.vertexIndex);
		triangles.Add(c.vertexIndex);

		Triangle triangle = new Triangle (a.vertexIndex, b.vertexIndex, c.vertexIndex);
		actualTriangles.Add (triangle);
		AddTriangleToDictionary (triangle.vertexIndexA, triangle);
		AddTriangleToDictionary (triangle.vertexIndexB, triangle);
		AddTriangleToDictionary (triangle.vertexIndexC, triangle);
	}
	void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle) {
		if (triangleDictionary.ContainsKey (vertexIndexKey)) {
			triangleDictionary [vertexIndexKey].Add (triangle);
		} else {
			List<Triangle> triangleList = new List<Triangle>();
			triangleList.Add(triangle);
			triangleDictionary.Add(vertexIndexKey, triangleList);
		}
	}
	void OnDrawGizmos() {
		/*
        if (squareGrid != null) {
            for (int x = 0; x < squareGrid.squares.GetLength(0); x ++) {
                for (int y = 0; y < squareGrid.squares.GetLength(1); y ++) {
                    Gizmos.color = (squareGrid.squares[x,y].topLeft.active)?Color.black:Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x,y].topLeft.position, Vector3.one * .4f);
                    Gizmos.color = (squareGrid.squares[x,y].topRight.active)?Color.black:Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x,y].topRight.position, Vector3.one * .4f);
                    Gizmos.color = (squareGrid.squares[x,y].bottomRight.active)?Color.black:Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x,y].bottomRight.position, Vector3.one * .4f);
                    Gizmos.color = (squareGrid.squares[x,y].bottomLeft.active)?Color.black:Color.white;
                    Gizmos.DrawCube(squareGrid.squares[x,y].bottomLeft.position, Vector3.one * .4f);
                    Gizmos.color = Color.grey;
                    Gizmos.DrawCube(squareGrid.squares[x,y].centreTop.position, Vector3.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[x,y].centreRight.position, Vector3.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[x,y].centreBottom.position, Vector3.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[x,y].centreLeft.position, Vector3.one * .15f);
                }
            }
        }
        */
	}

	public class SquareGrid {
		public Square[,] squares;

		public SquareGrid(int[,] map, float squareSize) {
			int nodeCountX = map.GetLength(0);
			int nodeCountY = map.GetLength(1);
			float mapWidth = nodeCountX * squareSize;
			float mapHeight = nodeCountY * squareSize;

			ControlNode[,] controlNodes = new ControlNode[nodeCountX,nodeCountY];

			for (int x = 0; x < nodeCountX; x ++) {
				for (int y = 0; y < nodeCountY; y ++) {
					Vector3 pos = new Vector3(-mapWidth/2 + x * squareSize + squareSize/2, 0, -mapHeight/2 + y * squareSize + squareSize/2);
					controlNodes[x,y] = new ControlNode(pos,map[x,y] == 1, squareSize);
				}
			}

			squares = new Square[nodeCountX -1,nodeCountY -1];
			for (int x = 0; x < nodeCountX-1; x ++) {
				for (int y = 0; y < nodeCountY-1; y ++) {
					squares[x,y] = new Square(controlNodes[x,y+1], controlNodes[x+1,y+1], controlNodes[x+1,y], controlNodes[x,y]);
				}
			}

		}
	}

	public class Square {

		public ControlNode topLeft, topRight, bottomRight, bottomLeft;
		public Node centreTop, centreRight, centreBottom, centreLeft;
		public int configuration;

		public Square (ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft) {
			topLeft = _topLeft;
			topRight = _topRight;
			bottomRight = _bottomRight;
			bottomLeft = _bottomLeft;

			centreTop = topLeft.right;
			centreRight = bottomRight.above;
			centreBottom = bottomLeft.right;
			centreLeft = bottomLeft.above;

			if (topLeft.active)
				configuration += 8;
			if (topRight.active)
				configuration += 4;
			if (bottomRight.active)
				configuration += 2;
			if (bottomLeft.active)
				configuration += 1;
		}
	}

	public class Node {
		public Vector3 position;
		public int vertexIndex = -1;

		public Node(Vector3 _pos) {
			position = _pos;
		}
	}

	public class ControlNode : Node {

		public bool active;
		public Node above, right;

		public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos) {
			active = _active;
			above = new Node(position + Vector3.forward * squareSize/2f);
			right = new Node(position + Vector3.right * squareSize/2f);
		}
	}
}