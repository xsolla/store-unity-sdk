using System;

public interface IGroup
{
	string Name
	{
		get; set;
	}

	Action<string> OnGroupClick { get; set; }

	void Deselect();
}