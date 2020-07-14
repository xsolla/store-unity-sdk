using System;

[Serializable]
public class InventoryItemModel : ItemModel
{
	public override bool IsVirtualCurrency() => false;

	public string InstanceId;
	public uint? RemainingUses;
}