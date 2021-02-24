using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Tests
{
	public class TESTBATTLEPASSDESCRIPTIONS : MonoBehaviour
	{
		public const string DescriptionA =
			@"{
				""Name"": ""BP2021JAN"",
				""ExpiryDate"": ""DD-MM-YYYY"",
				""Levels"": [
				{
					""Tier"": 1,
					""Experience"": 1000,
					""FreeItem"": {
					    ""Sku"": ""Sku"",
					    ""Quantity"": 10,
					    ""Promocode"": ""HELLO_WORLD""

					},
					""PremiumItem"": {
					    ""Sku"": ""Sku2"",
					    ""Promocode"": ""HELLO_WORLD2""
					}
				},
				{
					""Tier"": 2,
					""Experience"": 1000
				},
				{
					""Tier"": 3,
					""Experience"": 1000
				}
			    ]
			}";

		public const string DescriptionB =
			@"{
				""Name"":""BP2021JAN"",
				""ExpiryDate"":""01-07-2021"",
				""Levels"": [
				{
					""Tier"":1,
					""Experience"":100,
					""FreeItem"": {
						""Sku"":""Bullets"",
						""Promocode"":""B2021S1FL1E100"",
						""Quantity"":50 }
				},
				{
					""Tier"":2,
					""Experience"":250,
					""FreeItem"": {
						""Sku"":""Bullets"",
						""Promocode"":""B2021S1FL2E250"",
						""Quantity"":1000 },
					""PremiumItem"": {
						""Sku"":""Simple_Gun"",
						""Promocode"":""B2021S1PL2E250"",
						""Quantity"":1 }
				},
				{
					""Tier"":3,
					""Experience"":300,
					""FreeItem"": {
						""Sku"":""MST"",
						""Promocode"":""B2021S1PL3E300"",
						""Quantity"":10 }
				}
				]
			}";
	}
}
