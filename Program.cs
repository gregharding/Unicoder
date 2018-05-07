/*

	Unicoder - Simple C# Unicode helper console application

	Unicoder is a small tool to quickly print [Unicode](http://unicode.org/) information about text or file input. It is mildly useful to show the characters used, along with their Unicode blocks and codepoints.

	Unicoder can also dump UTF-8 hex output to help match text data up with raw file data.
	
	Usage:
		> mono unicoder.exe [-exact|-allcase] [-detailed] filename|text
			
	Author:
		Greg Harding greg@flightless.co.nz
		www.flightless.co.nz
	
	Copyright 2018 Flightless Ltd.
	

	The MIT License (MIT)
	
	Copyright (c) 2018 Flightless Ltd
	
	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:
	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.
	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	SOFTWARE.

*/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace Unicoder {

	class Program {

		private static string usage = "Usage: unicoder [-exact|-allcase] [-detailed] filename|text";


		public static void Main(string[] args) {
			if (args.Length < 1) {
				Console.WriteLine(usage);
				return;
			}

			var content = new List<string>(args);

			// options
			int exactCount = content.RemoveAll(s => s == "-exact");
			int allcaseCount = content.RemoveAll(s => s == "-allcase");
			int detailedCount = content.RemoveAll(s => s == "-detailed");

			bool showExact = exactCount > 0 || allcaseCount == 0;
			bool showAllCase = allcaseCount > 0 || exactCount == 0;
			bool showDetailed = detailedCount > 0;

			if (!showExact && !showAllCase) {
				showExact = true;
				showAllCase = true;
			}

			if (content.Count == 0) {
				Console.WriteLine("usage");
				return;
			}

			// filename
			var filename = (content.Count > 0) ? content.First() : "";

			try {
				string text = string.Join(" ", content);
				string[] lines = new string[] { text };

				if (File.Exists(filename)) {
					// load file, split into lines
					// reads file with automatic detection of Encoding.UTF8 etc.
					text = File.ReadAllText(filename);
					lines = File.ReadAllLines(filename);
				}

				Console.WriteLine($"Source:\n{text}\n");

				// find unique characters used
				// nb. does not include final rendered characters in languages that combine glyphs into other glyphs, eg. Arabic
				var usedChars = new HashSet<char>();
				var usedCharsAllCases = new HashSet<char>();
				foreach (var line in lines) {
					// specific usage
					foreach (var c in line.ToCharArray()) {
						usedChars.Add(c);
					}

					// upper and lower case
					foreach (var c in line.ToUpperInvariant().ToCharArray()) {
						usedCharsAllCases.Add(c);
					}
					foreach (var c in line.ToLowerInvariant().ToCharArray()) {
						usedCharsAllCases.Add(c);
					}
				}

				var sortedUsedChars = new List<char>(usedChars);
				sortedUsedChars.Sort();

				var sortedUsedCharsAllCases = new List<char>(usedCharsAllCases);
				sortedUsedCharsAllCases.Sort();

				// find unique Unicode blocks used
				var usedUnicodeCodeBlocks = new HashSet<UnicodeBlock>();
				foreach (var c in sortedUsedChars) {
					usedUnicodeCodeBlocks.Add(UnicodeUtils.GetUnicodeCodeBlock(c));
				}

				var usedUnicodeCodeBlocksAllCases = new HashSet<UnicodeBlock>();
				foreach (var c in sortedUsedCharsAllCases) {
					usedUnicodeCodeBlocksAllCases.Add(UnicodeUtils.GetUnicodeCodeBlock(c));
				}

				// specific characters
				if (showExact) {
					Console.WriteLine("Used characters (exact): {0}\n{1}\n", sortedUsedChars.Count, string.Join("", sortedUsedChars));
					Console.WriteLine("Used Unicode blocks (exact):\n{0}\n", string.Join("\n", usedUnicodeCodeBlocks));
					Console.WriteLine("Used Unicode codepoints info [char U+0000 UTF-8 Unicode Block] (exact): {0}", sortedUsedChars.Count);
					foreach (var c in sortedUsedChars) {
						Console.WriteLine("{0}\t{1}\t{2}\t{3}", c, UnicodeUtils.GetUnicodeCodePointUHex(c), UnicodeUtils.GetHexBytes(c, Encoding.UTF8), UnicodeUtils.GetUnicodeCodeBlock(c).name);
					}
					Console.WriteLine();
				}

				// all case characters
				if (showAllCase) {
					Console.WriteLine("Used characters (upper+lowercase): {0}\n{1}\n", sortedUsedCharsAllCases.Count, string.Join("", sortedUsedCharsAllCases));
					Console.WriteLine("Used Unicode blocks (upper+lowercase):\n{0}\n", string.Join("\n", usedUnicodeCodeBlocksAllCases));
					Console.WriteLine("Used Unicode codepoints info [char U+0000 UTF-8 Unicode Block] (upper+lowercase): {0}", sortedUsedCharsAllCases.Count);
					foreach (var c in sortedUsedCharsAllCases) {
						Console.WriteLine("{0}\t{1}\t{2}\t{3}", c, UnicodeUtils.GetUnicodeCodePointUHex(c), UnicodeUtils.GetHexBytes(c, Encoding.UTF8), UnicodeUtils.GetUnicodeCodeBlock(c).name);
					}
					Console.WriteLine();
				}

				// full character listing
				if (showDetailed) {
					// Unicode codepoints
					Console.WriteLine("Source Unicode codepoints U+0000:");
					foreach (var line in lines) {
						Console.WriteLine(UnicodeUtils.GetUnicodeCodePointHex(line));
					}
					Console.WriteLine();

					// Unicode UTF-8 hex bytes
					Console.WriteLine("Source UTF-8 bytes (+LF):");
					foreach (var line in lines) {
						Console.WriteLine(UnicodeUtils.GetHexBytes(line+"\n", Encoding.UTF8));
					}
					Console.WriteLine();

					// Unicode info
					Console.WriteLine("Source Unicode info:");
					foreach (var line in lines) {
						foreach (var c in line) {
							Console.WriteLine("{0}\t{1}\t{2}\t{3}", c, UnicodeUtils.GetUnicodeCodePointUHex(c), UnicodeUtils.GetHexBytes(c, Encoding.UTF8), UnicodeUtils.GetUnicodeCodeBlock(c).name);
						}
					}
					Console.WriteLine();
				}
			}
			catch (Exception e) {
				Console.WriteLine($"Error: {e}");
			}
		}
	}
}