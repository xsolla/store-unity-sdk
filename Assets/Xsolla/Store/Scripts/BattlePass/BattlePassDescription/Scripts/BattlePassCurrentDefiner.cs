using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassCurrentDefiner : BaseBattlePassCurrentDefiner
	{
		public override void OnBattlePassDescriptionsConverted(IEnumerable<BattlePassDescription> battlePassDescriptions)
		{
			if (battlePassDescriptions.Count() == 1)
				base.RaiseCurrentBattlePassDefined(battlePassDescriptions.First());
			else
			{
				var currentBattlePass = DefineCurrent(battlePassDescriptions);
				base.RaiseCurrentBattlePassDefined(currentBattlePass);
			}
		}

		private BattlePassDescription DefineCurrent(IEnumerable<BattlePassDescription> battlePassDescriptions)
		{
			var root = new BattlePassNode(){ ExpiryDate = DateTime.Now };
			
			BuildTheTree(root, battlePassDescriptions);

			if (root.Right != null)
				return TakeClosestRight(root).BattlePassItemDescription;
			else
				return TakeClosestLeft(root).BattlePassItemDescription;
		}

		private void BuildTheTree(BattlePassNode root, IEnumerable<BattlePassDescription> battlePassDescriptions)
		{
			foreach (var item in battlePassDescriptions)
			{
				var currentNode = root;
				var isLater = default(bool);

				while (true)
				{
					isLater = DateTime.Compare(item.ExpiryDate, currentNode.ExpiryDate) >= 0;

					if (isLater)
					{
						if (currentNode.Right != null)
							currentNode = currentNode.Right;
						else
						{
							currentNode.Right = new BattlePassNode() { ExpiryDate = item.ExpiryDate, BattlePassItemDescription = item };
							break;
						}
					}
					else
					{
						if (currentNode.Left != null)
							currentNode = currentNode.Left;
						else
						{
							currentNode.Left = new BattlePassNode() { ExpiryDate = item.ExpiryDate, BattlePassItemDescription = item };
							break;
						}
					}
				}
			}
		}

		private BattlePassNode TakeClosestRight(BattlePassNode treeRoot)
		{
			var currentNode = treeRoot.Right;

			while (currentNode.Left != null)
				currentNode = currentNode.Left;

			return currentNode;
		}

		private BattlePassNode TakeClosestLeft(BattlePassNode treeRoot)
		{
			var currentNode = treeRoot.Left;

			while (currentNode.Right != null)
				currentNode = currentNode.Right;

			return currentNode;
		}

		private class BattlePassNode
		{
			public DateTime ExpiryDate;
			public BattlePassNode Left;
			public BattlePassNode Right;

			public BattlePassDescription BattlePassItemDescription;
		}
	}
}
