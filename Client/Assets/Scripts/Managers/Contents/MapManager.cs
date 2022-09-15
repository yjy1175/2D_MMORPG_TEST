using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct Pos
{
    public Pos(int y, int x) { Y = y; X = x; }
    public int Y;
    public int X;
}

public struct PQNode : IComparable<PQNode>
{
    public int F;
    public int G;
    public int Y;
    public int X;

    public int CompareTo(PQNode other)
    {
        if (F == other.F)
            return 0;
        return F < other.F ? 1 : -1;
    }
}

public class MapManager
{
    public Grid CurrentGrid { get; private set; }

    public int MinX { get; set; }
    public int MaxX { get; set; }
    public int MinY { get; set; }
    public int MaxY { get; set; }

	public int SizeX { get { return MaxX - MinX + 1; } }
	public int SizeY { get { return MaxY - MinY + 1; } }


	bool[,] collision;

    public bool CanGo(Vector3Int cellPos)
    {
        if (cellPos.x < MinX || cellPos.x > MaxX)
            return false;
        if (cellPos.y < MinY || cellPos.y > MaxY)
            return false;

        return !collision[MaxY - cellPos.y, cellPos.x - MinX];
    }
    public void LoadMap(int mapId)
    {
        DestoryMap();

        string _mapName = $"Map_{mapId.ToString("000")}";
        GameObject _go = Managers.Resource.Instantiate($"Map/{_mapName}");
        _go.name = "Map";

        GameObject _col = Util.FindChild(_go, "Tilemap_Collision", true);
        if (_col != null)
            _col.SetActive(false);

        CurrentGrid = _go.GetComponent<Grid>();

        // Collision ���� ����
        TextAsset _txt = Managers.Resource.Load<TextAsset>($"Map/{_mapName}");
        StringReader _reader = new StringReader(_txt.text);

        MinX = int.Parse(_reader.ReadLine());
        MaxX = int.Parse(_reader.ReadLine());
        MinY = int.Parse(_reader.ReadLine());
        MaxY = int.Parse(_reader.ReadLine());

        int _xCount = MaxX - MinX + 1;
        int _yCount = MaxY - MinY + 1;

        collision = new bool[_yCount, _xCount];
        for(int y = 0; y < _yCount; y++)
        {
            string _line = _reader.ReadLine();
            for(int x = 0; x < _xCount; x++)
            {
                collision[y, x] = _line[x] == '1' ? true : false;
            }
        }
    }

    public void DestoryMap()
    {
        GameObject _map = GameObject.Find("Map");
        if (_map != null)
        {
            GameObject.Destroy(_map);
            CurrentGrid = null;
        }
    }

	#region A* PathFinding

	// U D L R
	int[] _deltaY = new int[] { 1, -1, 0, 0 };
	int[] _deltaX = new int[] { 0, 0, -1, 1 };
	int[] _cost = new int[] { 10, 10, 10, 10 };

	public List<Vector3Int> FindPath(Vector3Int startCellPos, Vector3Int destCellPos, bool ignoreDestCollision = false)
	{
		List<Pos> path = new List<Pos>();

		// ���� �ű��
		// F = G + H
		// F = ���� ���� (���� ���� ����, ��ο� ���� �޶���)
		// G = ���������� �ش� ��ǥ���� �̵��ϴµ� ��� ��� (���� ���� ����, ��ο� ���� �޶���)
		// H = ���������� �󸶳� ������� (���� ���� ����, ����)

		// (y, x) �̹� �湮�ߴ��� ���� (�湮 = closed ����)
		bool[,] closed = new bool[SizeY, SizeX]; // CloseList

		// (y, x) ���� ���� �� ���̶� �߰��ߴ���
		// �߰�X => MaxValue
		// �߰�O => F = G + H
		int[,] open = new int[SizeY, SizeX]; // OpenList
		for (int y = 0; y < SizeY; y++)
			for (int x = 0; x < SizeX; x++)
				open[y, x] = Int32.MaxValue;

		Pos[,] parent = new Pos[SizeY, SizeX];

		// ���¸���Ʈ�� �ִ� ������ �߿���, ���� ���� �ĺ��� ������ �̾ƿ��� ���� ����
		PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

		// CellPos -> ArrayPos
		Pos pos = Cell2Pos(startCellPos);
		Pos dest = Cell2Pos(destCellPos);

		// ������ �߰� (���� ����)
		open[pos.Y, pos.X] = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X));
		pq.Push(new PQNode() { F = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X)), G = 0, Y = pos.Y, X = pos.X });
		parent[pos.Y, pos.X] = new Pos(pos.Y, pos.X);

		while (pq.Count > 0)
		{
			// ���� ���� �ĺ��� ã�´�
			PQNode node = pq.Pop();
			// ������ ��ǥ�� ���� ��η� ã�Ƽ�, �� ���� ��η� ���ؼ� �̹� �湮(closed)�� ��� ��ŵ
			if (closed[node.Y, node.X])
				continue;

			// �湮�Ѵ�
			closed[node.Y, node.X] = true;
			// ������ ���������� �ٷ� ����
			if (node.Y == dest.Y && node.X == dest.X)
				break;

			// �����¿� �� �̵��� �� �ִ� ��ǥ���� Ȯ���ؼ� ����(open)�Ѵ�
			for (int i = 0; i < _deltaY.Length; i++)
			{
				Pos next = new Pos(node.Y + _deltaY[i], node.X + _deltaX[i]);

				// ��ȿ ������ ������� ��ŵ
				// ������ ������ �� �� ������ ��ŵ
				if (!ignoreDestCollision || next.Y != dest.Y || next.X != dest.X)
				{
					if (CanGo(Pos2Cell(next)) == false) // CellPos
						continue;
				}

				// �̹� �湮�� ���̸� ��ŵ
				if (closed[next.Y, next.X])
					continue;

				// ��� ���
				int g = 0;// node.G + _cost[i];
				int h = 10 * ((dest.Y - next.Y) * (dest.Y - next.Y) + (dest.X - next.X) * (dest.X - next.X));
				// �ٸ� ��ο��� �� ���� �� �̹� ã������ ��ŵ
				if (open[next.Y, next.X] < g + h)
					continue;

				// ���� ����
				open[dest.Y, dest.X] = g + h;
				pq.Push(new PQNode() { F = g + h, G = g, Y = next.Y, X = next.X });
				parent[next.Y, next.X] = new Pos(node.Y, node.X);
			}
		}

		return CalcCellPathFromParent(parent, dest);
	}

	List<Vector3Int> CalcCellPathFromParent(Pos[,] parent, Pos dest)
	{
		List<Vector3Int> cells = new List<Vector3Int>();

		int y = dest.Y;
		int x = dest.X;
		while (parent[y, x].Y != y || parent[y, x].X != x)
		{
			cells.Add(Pos2Cell(new Pos(y, x)));
			Pos pos = parent[y, x];
			y = pos.Y;
			x = pos.X;
		}
		cells.Add(Pos2Cell(new Pos(y, x)));
		cells.Reverse();

		return cells;
	}

	Pos Cell2Pos(Vector3Int cell)
	{
		// CellPos -> ArrayPos
		return new Pos(MaxY - cell.y, cell.x - MinX);
	}

	Vector3Int Pos2Cell(Pos pos)
	{
		// ArrayPos -> CellPos
		return new Vector3Int(pos.X + MinX, MaxY - pos.Y, 0);
	}

	#endregion
}
