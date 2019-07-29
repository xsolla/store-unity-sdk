using System;

public interface IGroup
{
	string Name
	{
		get; set;
	}

	Action<string> OnGroupClick { get; set; }

	void Select();
	void Deselect();
	bool IsSelected();
}