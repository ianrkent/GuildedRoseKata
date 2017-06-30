using System;
using System.Collections.Generic;

namespace GildedRose
{
	class GuildedRoseRunner
	{
		public static void RunWithInput(IList<Item> items)
		{
			System.Console.WriteLine("OMGHAI!");

		    var app = new GildedRose(items);
			
			for (var i = 0; i < 31; i++)
			{
				System.Console.WriteLine("-------- day " + i + " --------");
				System.Console.WriteLine("name, sellIn, quality");
				for (var j = 0; j < items.Count; j++)
				{
					System.Console.WriteLine(items[j].Name + ", " + items[j].SellIn + ", " + items[j].Quality);
				}
				System.Console.WriteLine("");
				app.UpdateQuality();
			}
		}

	    
	}
}
