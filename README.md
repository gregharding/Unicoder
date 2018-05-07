# Unicoder

**Simple C# Unicode helper console application.**

Unicoder is a small tool to quickly print [Unicode](http://unicode.org/) information about text or file input. It is mildly useful to show the characters used, along with their Unicode blocks and codepoints.

Unicoder can also dump UTF-8 hex output to help match text data with raw file data.

**Usage**

Usage (Mono/macOS):

    > mono unicoder.exe [-exact|-allcase] [-detailed] filename|text

Output example:

	> mono unicoder.exe -exact -detailed hello©™
	Source:
	hello©™

	Used characters (exact): 6
	ehlo©™

	Used Unicode blocks (exact):
	Basic Latin (U+0000..U+007F)
	Latin-1 Supplement (U+0080..U+00FF)
	Letterlike Symbols (U+2100..U+214F)

	Used Unicode codepoints info [char U+0000 UTF-8 Unicode Block] (exact): 6
	e	U+0065	65	Basic Latin
	h	U+0068	68	Basic Latin
	l	U+006C	6C	Basic Latin
	o	U+006F	6F	Basic Latin
	©	U+00A9	C2 A9	Latin-1 Supplement
	™	U+2122	E2 84 A2	Letterlike Symbols

	Source Unicode codepoints U+0000:
	0068 0065 006C 006C 006F 00A9 2122

	Source UTF-8 bytes (+LF):
	68 65 6C 6C 6F C2 A9 E2 84 A2 0A

	Source Unicode info:
	h	U+0068	68	Basic Latin
	e	U+0065	65	Basic Latin
	l	U+006C	6C	Basic Latin
	l	U+006C	6C	Basic Latin
	o	U+006F	6F	Basic Latin
	©	U+00A9	C2 A9	Latin-1 Supplement
	™	U+2122	E2 84 A2	Letterlike Symbols

**Todo**

Unicoder does not include per-character descriptions from [UnicodeData.txt](ftp://ftp.unicode.org/Public/UNIDATA/UnicodeData.txt) (ftp://ftp.unicode.org/Public/UNIDATA/UnicodeData.txt). It also does not generate final rendered characters in languages that combine glyphs into other glyphs, eg. Arabic.

**Author**

Greg Harding [http://www.flightless.co.nz](http://www.flightless.co.nz)

Copyright 2018 Flightless Ltd

**License**

> The MIT License (MIT)
> 
> Copyright (c) 2018 Flightless Ltd
> 
> Permission is hereby granted, free of charge, to any person obtaining
> a copy of this software and associated documentation files (the
> "Software"), to deal in the Software without restriction, including
> without limitation the rights to use, copy, modify, merge, publish,
> distribute, sublicense, and/or sell copies of the Software, and to
> permit persons to whom the Software is furnished to do so, subject to
> the following conditions:
> 
> The above copyright notice and this permission notice shall be
> included in all copies or substantial portions of the Software.
> 
> THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
> EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
> MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
> NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
> BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
> ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
> CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
> SOFTWARE.
