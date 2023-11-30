using Godot;
using System;

public partial class WorldModule : Node
{
	private int test = 0;
	public void testSetter(int setter)
	{
		test = setter;
	}
	public int testGetter()
	{
		return test;
	}
	public void MyPrint(string text)
	{
		GD.Print("This is message from c " + text);
		GD.Print("Ahoj jak se mas");
	}
}
