using System.Collections;
using System.Collections.Generic;


public class TDArea {

	public int left, top, width, height;
	public int right { get { return left + width - 1; } }
	public int bottom { get { return top + height - 1; } }

	public TDArea(int left, int top, int width, int height)
	{
		this.left = left;
		this.top = top;
		this.width = width;
		this.height = height;
	}

	public bool CollidesWith(TDArea other)
	{
		if (left > other.right + 1) return false;

		if (top > other.bottom + 1) return false;

		if (right < other.left - 1) return false;

		if (bottom < other.top - 1) return false;

		return true;
	}

}
