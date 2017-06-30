using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace GildedRose
{
    [TestFixture()]
    public class GildedRoseTest
    {
        [Test()]
        public void ArbirtraryProduct_SellinDecreases()
        {
            IList<Item> Items = new List<Item> {new Item {Name = "foo", SellIn = 1, Quality = 1}};
            GildedRose app = new GildedRose(Items);

            app.UpdateQuality();

            Assert.AreEqual(0, Items[0].SellIn);
        }

        [Test]
        public void ArbirtraryProduct_NameDoesntChange()
        {
            IList<Item> Items = new List<Item> {new Item {Name = "foo", SellIn = 1, Quality = 1}};
            GildedRose app = new GildedRose(Items);

            app.UpdateQuality();

            Assert.AreEqual("foo", Items[0].Name);
        }

        [TestCase("foo", 0, 0, 1, "quality cannot be below 0")]
        [TestCase("foo", 4, 3, 1, "quaility of a normal product should decrease by one")]
        [TestCase("foo", 4, 2, 0, "quality decreases by 2 when the sellIn day has passed")]
        [TestCase("Aged Brie", 4, 5, 1, "Aged Brie goes up by one")]
        [TestCase("Aged Brie", 4, 6, 0, "Aged Brie goes up by 2 when sellIn is 0")]
        [TestCase("Aged Brie", 4, 6, -1, "Aged Brie goes up by 2 when sellin is negative")]
        [TestCase("Aged Brie", 50, 50, 1, "Maximum quality is 50")]
        [TestCase("Backstage passes to a TAFKAL80ETC concert", 5, 8, 1, "Backstage pass quality goes up by 3")]
        [TestCase("Backstage passes to a TAFKAL80ETC concert", 5, 0, 0,
            "Backastge Pass sell in 0 -> quality goes down to 0")]
        [TestCase("Backstage passes to a TAFKAL80ETC concert", 5, 6, 12, "Backstage pass quality goes up by 1")]
        [TestCase("Backstage passes to a TAFKAL80ETC concert", 5, 7, 10, "Backstage pass quality goes up by 2")]
        [TestCase("Conjured", 5, 3, 10, "Conjured quality goes down twice as fast")]
        public void ArbirtraryProduct_QualityBehavesCorrectly(string productName, int initialQuality,
            int expectedQuality, int initialSellIn, string failureMessage)
        {
            IList<Item> Items = new List<Item>
            {
                new Item {Name = productName, SellIn = initialSellIn, Quality = initialQuality}
            };
            GildedRose app = new GildedRose(Items);

            app.UpdateQuality();

            Assert.AreEqual(expectedQuality, Items[0].Quality, failureMessage);
        }

        [Test]
        [Ignore("don't want to regenerate the  Golden Master. Run this mannually")]
        public void GenerateGoldenMaster()
        {
            const string goldenMasterPath = @"C:\Users\ian.kent\Source\Repos\GildedRose-Refactoring-Kata\csharp\GoldenMaster.txt";

            var inputItems = GetInputForGoldenMaster();
            var streamWriter = new StreamWriter(new FileStream(goldenMasterPath, FileMode.Create)) { AutoFlush = true };
            
            RunWithConsoleOutRedirected(() => GuildedRoseRunner.RunWithInput(inputItems), streamWriter);

            streamWriter.Close();
        }

        [Test]
        public void TestAgainstGoldenMaster()
        {
            const string testOutputPath = @"C:\Users\ian.kent\Source\Repos\GildedRose-Refactoring-Kata\csharp\TestOutput.txt";
            const string goldenMasterPath = @"C:\Users\ian.kent\Source\Repos\GildedRose-Refactoring-Kata\csharp\GoldenMaster.txt";

            var inputItems = GetInputForGoldenMaster();
            var streamWriter = new StreamWriter(new FileStream(testOutputPath, FileMode.Create)) { AutoFlush = true };

            RunWithConsoleOutRedirected(() => GuildedRoseRunner.RunWithInput(inputItems), streamWriter);

            streamWriter.Close();

            Assert.IsTrue(FileCompare(goldenMasterPath, testOutputPath));
        }


        private static IList<Item> GetInputForGoldenMaster()
        {
            // TODO: Make this a broader list of inputs, to cover more cases.

            IList<Item> items = new List<Item>
            {
                new Item {Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20},
                new Item {Name = "Aged Brie", SellIn = 2, Quality = 0},
                new Item {Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7},
                new Item {Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80},
                new Item {Name = "Sulfuras, Hand of Ragnaros", SellIn = -1, Quality = 80},
                new Item
                {
                    Name = "Backstage passes to a TAFKAL80ETC concert",
                    SellIn = 15,
                    Quality = 20
                },
                new Item
                {
                    Name = "Backstage passes to a TAFKAL80ETC concert",
                    SellIn = 10,
                    Quality = 49
                },
                new Item
                {
                    Name = "Backstage passes to a TAFKAL80ETC concert",
                    SellIn = 5,
                    Quality = 49
                },
                // this conjured item does not work properly yet
                new Item {Name = "Conjured Mana Cake", SellIn = 3, Quality = 6}
            };
            return items;
        }


        // This method accepts two strings the represent two files to 
        // compare. A return value of 0 indicates that the contents of the files
        // are the same. A return value of any other value indicates that the 
        // files are not the same.

        private void RunWithConsoleOutRedirected(Action thingToRun, StreamWriter redirectedOutput)
        {
            var original = Console.Out;
            Console.SetOut(redirectedOutput);

            thingToRun();

            Console.SetOut(original);
        }

        private bool FileCompare(string file1, string file2)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;

            // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.
            fs1 = new FileStream(file1, FileMode.Open, FileAccess.Read);
            fs2 = new FileStream(file2, FileMode.Open, FileAccess.Read);

            // Check the file sizes. If they are not the same, the files 
            // are not the same.
            if (fs1.Length != fs2.Length)
            {
                // Close the file
                fs1.Close();
                fs2.Close();

                // Return false to indicate files are different
                return false;
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do
            {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));

            // Close the files.
            fs1.Close();
            fs2.Close();

            // Return the success of the comparison. "file1byte" is 
            // equal to "file2byte" at this point only if the files are 
            // the same.
            return ((file1byte - file2byte) == 0);
        }
    }
}

