using System;

public interface IGroup
{
	string Id { get; set; }

	string Name { get; set; }

	Action<string> OnGroupClick { get; set; }

	void Select();
	void Deselect();
	bool IsSelected();
}