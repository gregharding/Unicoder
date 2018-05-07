using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Unicoder {

	public struct UnicodeBlock {

		public static UnicodeBlock Unknown { get { return _unknown; } }
		private static UnicodeBlock _unknown = new UnicodeBlock(0, 0, "Unknown");

		public int low;
		public int high;
		public string name;


		public UnicodeBlock(int low, int high, string name) {
			this.low = low;
			this.high = high;
			this.name = name;
		}

		public override string ToString() {
			return string.Format("{0} (U+{1}..U+{2})", name, low.ToString("X4"), high.ToString("X4"));
		}
	}


	public static class UnicodeUtils {

		public static List<UnicodeBlock> UnicodeBlocks { get { return _unicodeBlocks; } }
		private static List<UnicodeBlock> _unicodeBlocks;

		public static int BlockCount { get { return _unicodeBlocks.Count; } }


		static UnicodeUtils() {
			_unicodeBlocks = new List<UnicodeBlock>();

			var pattern = @"^U\+(\w+)\.\.U\+(\w+)\t(.*?)\t.*$"; // U+0000..U+0000\ttext\trestofline\n
			foreach (Match m in Regex.Matches(unicodeBlockData, pattern, RegexOptions.Multiline)) {
				var low = int.Parse(m.Groups[1].Value, System.Globalization.NumberStyles.HexNumber);
				var high = int.Parse(m.Groups[2].Value, System.Globalization.NumberStyles.HexNumber);
				string name = m.Groups[3].Value;

				_unicodeBlocks.Add(new UnicodeBlock(low, high, name));
			}

			//Console.WriteLine($"UnicodeBlockUtils loaded {BlockCount} code blocks.");
		}


		//
		// helpers
		//

		// Unicode escape sequence \u0000

		public static string GetUnicodeEscapeSequence(string s) {
			return GetUnicodeEscapeSequence(s.ToCharArray());
		}

		public static string GetUnicodeEscapeSequence(char[] chars) {
			var sb = new StringBuilder();

			foreach (var c in chars) {
				sb.Append(GetUnicodeEscapeSequence(c));
			}

			return sb.ToString();
		}

		public static string GetUnicodeEscapeSequence(char c) {
			return "\\u" + ((int)c).ToString("X4");
		}


		// Unicode codepoint U+0000

		public static string GetUnicodeCodePointUHex(string s) {
			return GetUnicodeCodePointUHex(s.ToCharArray());
		}

		public static string GetUnicodeCodePointUHex(char[] chars) {
			var sb = new StringBuilder();

			for (int i = 0; i < chars.Length; i++) {
				if (i > 0) sb.Append(" ");
				sb.Append(GetUnicodeCodePointUHex(chars[i]));
			}

			return sb.ToString();
		}

		public static string GetUnicodeCodePointUHex(char c) {
			return "U+" + ((int)c).ToString("X4");
		}


		// Unicode codepoint hex 0000

		public static string GetUnicodeCodePointHex(string s) {
			return GetUnicodeCodePointHex(s.ToCharArray());
		}

		public static string GetUnicodeCodePointHex(char[] chars) {
			var sb = new StringBuilder();

			for (int i = 0; i < chars.Length; i++) {
				if (i > 0) sb.Append(" ");
				sb.Append(GetUnicodeCodePointHex(chars[i]));
			}

			return sb.ToString();
		}

		public static string GetUnicodeCodePointHex(char c) {
			return ((int)c).ToString("X4");
		}


		// byte encoding in hex

		public static string GetHexBytes(string text, Encoding encoding) {
			return GetHexBytes(text.ToCharArray(), encoding);
		}

		public static string GetHexBytes(char[] chars, Encoding encoding) {
			if (encoding == Encoding.UTF8) {
				return GetHexBytes(Encoding.UTF8.GetBytes(chars));
			} else if (encoding == Encoding.Unicode) {
				return GetHexBytes(Encoding.Unicode.GetBytes(chars));
			}

			throw new ArgumentException($"Unsupported encoding {encoding}!");
		}

		public static string GetHexBytes(char c, Encoding encoding) {
			return GetHexBytes(new char[] { c }, encoding);
		}

		public static string GetHexBytes(byte[] bytes, bool useSpace = true) {
			if ((bytes == null) || (bytes.Length == 0))
				return "<none>"; // string.Empty

			var sb = new StringBuilder();

			for (int i = 0; i < bytes.Length; i++) {
				if (useSpace && i > 0) sb.Append(" ");
				sb.Append(string.Format("{0:X2}", bytes[i]));
			}

			return sb.ToString();
		}


		//
		// Unicode Block
		// https://en.wikipedia.org/wiki/Unicode_block
		//

		public static UnicodeBlock GetUnicodeCodeBlock(char c) {
			foreach (var cb in _unicodeBlocks) {
				if ((int)c >= cb.low && (int)c <= cb.high)
					return cb;
			}

			return UnicodeBlock.Unknown;
		}

		private static string unicodeBlockData =
@"U+0000..U+007F	Basic Latin	128	128	Latin (52 characters), Common (76 characters)
U+0080..U+00FF	Latin-1 Supplement	128	128	Latin (64 characters), Common (64 characters)
U+0100..U+017F	Latin Extended-A	128	128	Latin
U+0180..U+024F	Latin Extended-B	208	208	Latin
U+0250..U+02AF	IPA Extensions	96	96	Latin
U+02B0..U+02FF	Spacing Modifier Letters	80	80	Bopomofo (2 characters), Latin (14 characters), Common (64 characters)
U+0300..U+036F	Combining Diacritical Marks	112	112	Inherited
U+0370..U+03FF	Greek and Coptic	144	135	Coptic (14 characters), Greek (117 characters), Common (4 characters)
U+0400..U+04FF	Cyrillic	256	256	Cyrillic (254 characters), Inherited (2 characters)
U+0500..U+052F	Cyrillic Supplement	48	48	Cyrillic
U+0530..U+058F	Armenian	96	89	Armenian (88 characters), Common (1 character)
U+0590..U+05FF	Hebrew	112	87	Hebrew
U+0600..U+06FF	Arabic	256	255	Arabic (237 characters), Common (6 characters), Inherited (12 characters)
U+0700..U+074F	Syriac	80	77	Syriac
U+0750..U+077F	Arabic Supplement	48	48	Arabic
U+0780..U+07BF	Thaana	64	50	Thaana
U+07C0..U+07FF	NKo	64	59	Nko
U+0800..U+083F	Samaritan	64	61	Samaritan
U+0840..U+085F	Mandaic	32	29	Mandaic
U+0860..U+086F	Syriac Supplement	16	11	Syriac
U+08A0..U+08FF	Arabic Extended-A	96	73	Arabic (72 characters), Common (1 character)
U+0900..U+097F	Devanagari	128	128	Devanagari (124 characters), Common (2 characters), Inherited (2 characters)
U+0980..U+09FF	Bengali	128	95	Bengali
U+0A00..U+0A7F	Gurmukhi	128	79	Gurmukhi
U+0A80..U+0AFF	Gujarati	128	91	Gujarati
U+0B00..U+0B7F	Oriya	128	90	Oriya
U+0B80..U+0BFF	Tamil	128	72	Tamil
U+0C00..U+0C7F	Telugu	128	96	Telugu
U+0C80..U+0CFF	Kannada	128	88	Kannada
U+0D00..U+0D7F	Malayalam	128	117	Malayalam
U+0D80..U+0DFF	Sinhala	128	90	Sinhala
U+0E00..U+0E7F	Thai	128	87	Thai (86 characters), Common (1 character)
U+0E80..U+0EFF	Lao	128	67	Lao
U+0F00..U+0FFF	Tibetan	256	211	Tibetan (207 characters), Common (4 characters)
U+1000..U+109F	Myanmar	160	160	Myanmar
U+10A0..U+10FF	Georgian	96	88	Georgian (87 characters), Common (1 character)
U+1100..U+11FF	Hangul Jamo	256	256	Hangul
U+1200..U+137F	Ethiopic	384	358	Ethiopic
U+1380..U+139F	Ethiopic Supplement	32	26	Ethiopic
U+13A0..U+13FF	Cherokee	96	92	Cherokee
U+1400..U+167F	Unified Canadian Aboriginal Syllabics	640	640	Canadian Aboriginal
U+1680..U+169F	Ogham	32	29	Ogham
U+16A0..U+16FF	Runic	96	89	Runic (86 characters), Common (3 characters)
U+1700..U+171F	Tagalog	32	20	Tagalog
U+1720..U+173F	Hanunoo	32	23	Hanunoo (21 characters), Common (2 characters)
U+1740..U+175F	Buhid	32	20	Buhid
U+1760..U+177F	Tagbanwa	32	18	Tagbanwa
U+1780..U+17FF	Khmer	128	114	Khmer
U+1800..U+18AF	Mongolian	176	156	Mongolian (153 characters), Common (3 characters)
U+18B0..U+18FF	Unified Canadian Aboriginal Syllabics Extended	80	70	Canadian Aboriginal
U+1900..U+194F	Limbu	80	68	Limbu
U+1950..U+197F	Tai Le	48	35	Tai Le
U+1980..U+19DF	New Tai Lue	96	83	New Tai Lue
U+19E0..U+19FF	Khmer Symbols	32	32	Khmer
U+1A00..U+1A1F	Buginese	32	30	Buginese
U+1A20..U+1AAF	Tai Tham	144	127	Tai Tham
U+1AB0..U+1AFF	Combining Diacritical Marks Extended	80	15	Inherited
U+1B00..U+1B7F	Balinese	128	121	Balinese
U+1B80..U+1BBF	Sundanese	64	64	Sundanese
U+1BC0..U+1BFF	Batak	64	56	Batak
U+1C00..U+1C4F	Lepcha	80	74	Lepcha
U+1C50..U+1C7F	Ol Chiki	48	48	Ol Chiki
U+1C80..U+1C8F	Cyrillic Extended-C	16	9	Cyrillic
U+1CC0..U+1CCF	Sundanese Supplement	16	8	Sundanese
U+1CD0..U+1CFF	Vedic Extensions	48	42	Common (15 characters), Inherited (27 characters)
U+1D00..U+1D7F	Phonetic Extensions	128	128	Cyrillic (2 characters), Greek (15 characters), Latin (111 characters)
U+1D80..U+1DBF	Phonetic Extensions Supplement	64	64	Greek (1 character), Latin (63 characters)
U+1DC0..U+1DFF	Combining Diacritical Marks Supplement	64	63	Inherited
U+1E00..U+1EFF	Latin Extended Additional	256	256	Latin
U+1F00..U+1FFF	Greek Extended	256	233	Greek
U+2000..U+206F	General Punctuation	112	111	Common (109 characters), Inherited (2 characters)
U+2070..U+209F	Superscripts and Subscripts	48	42	Latin (15 characters), Common (27 characters)
U+20A0..U+20CF	Currency Symbols	48	32	Common
U+20D0..U+20FF	Combining Diacritical Marks for Symbols	48	33	Inherited
U+2100..U+214F	Letterlike Symbols	80	80	Greek (1 character), Latin (4 characters), Common (75 characters)
U+2150..U+218F	Number Forms	64	60	Latin (41 characters), Common (19 characters)
U+2190..U+21FF	Arrows	112	112	Common
U+2200..U+22FF	Mathematical Operators	256	256	Common
U+2300..U+23FF	Miscellaneous Technical	256	256	Common
U+2400..U+243F	Control Pictures	64	39	Common
U+2440..U+245F	Optical Character Recognition	32	11	Common
U+2460..U+24FF	Enclosed Alphanumerics	160	160	Common
U+2500..U+257F	Box Drawing	128	128	Common
U+2580..U+259F	Block Elements	32	32	Common
U+25A0..U+25FF	Geometric Shapes	96	96	Common
U+2600..U+26FF	Miscellaneous Symbols	256	256	Common
U+2700..U+27BF	Dingbats	192	192	Common
U+27C0..U+27EF	Miscellaneous Mathematical Symbols-A	48	48	Common
U+27F0..U+27FF	Supplemental Arrows-A	16	16	Common
U+2800..U+28FF	Braille Patterns	256	256	Braille
U+2900..U+297F	Supplemental Arrows-B	128	128	Common
U+2980..U+29FF	Miscellaneous Mathematical Symbols-B	128	128	Common
U+2A00..U+2AFF	Supplemental Mathematical Operators	256	256	Common
U+2B00..U+2BFF	Miscellaneous Symbols and Arrows	256	207	Common
U+2C00..U+2C5F	Glagolitic	96	94	Glagolitic
U+2C60..U+2C7F	Latin Extended-C	32	32	Latin
U+2C80..U+2CFF	Coptic	128	123	Coptic
U+2D00..U+2D2F	Georgian Supplement	48	40	Georgian
U+2D30..U+2D7F	Tifinagh	80	59	Tifinagh
U+2D80..U+2DDF	Ethiopic Extended	96	79	Ethiopic
U+2DE0..U+2DFF	Cyrillic Extended-A	32	32	Cyrillic
U+2E00..U+2E7F	Supplemental Punctuation	128	74	Common
U+2E80..U+2EFF	CJK Radicals Supplement	128	115	Han
U+2F00..U+2FDF	Kangxi Radicals	224	214	Han
U+2FF0..U+2FFF	Ideographic Description Characters	16	12	Common
U+3000..U+303F	CJK Symbols and Punctuation	64	64	Han (15 characters), Hangul (2 characters), Common (43 characters), Inherited (4 characters)
U+3040..U+309F	Hiragana	96	93	Hiragana (89 characters), Common (2 characters), Inherited (2 characters)
U+30A0..U+30FF	Katakana	96	96	Katakana (93 characters), Common (3 characters)
U+3100..U+312F	Bopomofo	48	42	Bopomofo
U+3130..U+318F	Hangul Compatibility Jamo	96	94	Hangul
U+3190..U+319F	Kanbun	16	16	Common
U+31A0..U+31BF	Bopomofo Extended	32	27	Bopomofo
U+31C0..U+31EF	CJK Strokes	48	36	Common
U+31F0..U+31FF	Katakana Phonetic Extensions	16	16	Katakana
U+3200..U+32FF	Enclosed CJK Letters and Months	256	254	Hangul (62 characters), Katakana (47 characters), Common (145 characters)
U+3300..U+33FF	CJK Compatibility	256	256	Katakana (88 characters), Common (168 characters)
U+3400..U+4DBF	CJK Unified Ideographs Extension A	6,592	6,582	Han
U+4DC0..U+4DFF	Yijing Hexagram Symbols	64	64	Common
U+4E00..U+9FFF	CJK Unified Ideographs	20,992	20,971	Han
U+A000..U+A48F	Yi Syllables	1,168	1,165	Yi
U+A490..U+A4CF	Yi Radicals	64	55	Yi
U+A4D0..U+A4FF	Lisu	48	48	Lisu
U+A500..U+A63F	Vai	320	300	Vai
U+A640..U+A69F	Cyrillic Extended-B	96	96	Cyrillic
U+A6A0..U+A6FF	Bamum	96	88	Bamum
U+A700..U+A71F	Modifier Tone Letters	32	32	Common
U+A720..U+A7FF	Latin Extended-D	224	160	Latin (155 characters), Common (5 characters)
U+A800..U+A82F	Syloti Nagri	48	44	Syloti Nagri
U+A830..U+A83F	Common Indic Number Forms	16	10	Common
U+A840..U+A87F	Phags-pa	64	56	Phags Pa
U+A880..U+A8DF	Saurashtra	96	82	Saurashtra
U+A8E0..U+A8FF	Devanagari Extended	32	30	Devanagari
U+A900..U+A92F	Kayah Li	48	48	Kayah Li (47 characters), Common (1 character)
U+A930..U+A95F	Rejang	48	37	Rejang
U+A960..U+A97F	Hangul Jamo Extended-A	32	29	Hangul
U+A980..U+A9DF	Javanese	96	91	Javanese (90 characters), Common (1 character)
U+A9E0..U+A9FF	Myanmar Extended-B	32	31	Myanmar
U+AA00..U+AA5F	Cham	96	83	Cham
U+AA60..U+AA7F	Myanmar Extended-A	32	32	Myanmar
U+AA80..U+AADF	Tai Viet	96	72	Tai Viet
U+AAE0..U+AAFF	Meetei Mayek Extensions	32	23	Meetei Mayek
U+AB00..U+AB2F	Ethiopic Extended-A	48	32	Ethiopic
U+AB30..U+AB6F	Latin Extended-E	64	54	Latin (52 characters), Greek (1 character), Common (1 character)
U+AB70..U+ABBF	Cherokee Supplement	80	80	Cherokee
U+ABC0..U+ABFF	Meetei Mayek	64	56	Meetei Mayek
U+AC00..U+D7AF	Hangul Syllables	11,184	11,172	Hangul
U+D7B0..U+D7FF	Hangul Jamo Extended-B	80	72	Hangul
U+D800..U+DB7F	High Surrogates	896	0	Unknown
U+DB80..U+DBFF	High Private Use Surrogates	128	0	Unknown
U+DC00..U+DFFF	Low Surrogates	1,024	0	Unknown
U+E000..U+F8FF	Private Use Area	6,400	6,400	Unknown
U+F900..U+FAFF	CJK Compatibility Ideographs	512	472	Han
U+FB00..U+FB4F	Alphabetic Presentation Forms	80	58	Armenian (5 characters), Hebrew (46 characters), Latin (7 characters)
U+FB50..U+FDFF	Arabic Presentation Forms-A	688	611	Arabic (609 characters), Common (2 characters)
U+FE00..U+FE0F	Variation Selectors	16	16	Inherited
U+FE10..U+FE1F	Vertical Forms	16	10	Common
U+FE20..U+FE2F	Combining Half Marks	16	16	Cyrillic (2 characters), Inherited (14 characters)
U+FE30..U+FE4F	CJK Compatibility Forms	32	32	Common
U+FE50..U+FE6F	Small Form Variants	32	26	Common
U+FE70..U+FEFF	Arabic Presentation Forms-B	144	141	Arabic (140 characters), Common (1 character)
U+FF00..U+FFEF	Halfwidth and Fullwidth Forms	240	225	Hangul (52 characters), Katakana (55 characters), Latin (52 characters), Common (66 characters)
U+FFF0..U+FFFF	Specials	16	5	Common
U+10000..U+1007F	Linear B Syllabary	128	88	Linear B
U+10080..U+100FF	Linear B Ideograms	128	123	Linear B
U+10100..U+1013F	Aegean Numbers	64	57	Common
U+10140..U+1018F	Ancient Greek Numbers	80	79	Greek
U+10190..U+101CF	Ancient Symbols	64	13	Greek (1 character), Common (12 characters)
U+101D0..U+101FF	Phaistos Disc	48	46	Common (45 characters), Inherited (1 character)
U+10280..U+1029F	Lycian	32	29	Lycian
U+102A0..U+102DF	Carian	64	49	Carian
U+102E0..U+102FF	Coptic Epact Numbers	32	28	Common (27 characters), Inherited (1 character)
U+10300..U+1032F	Old Italic	48	39	Old Italic
U+10330..U+1034F	Gothic	32	27	Gothic
U+10350..U+1037F	Old Permic	48	43	Old Permic
U+10380..U+1039F	Ugaritic	32	31	Ugaritic
U+103A0..U+103DF	Old Persian	64	50	Old Persian
U+10400..U+1044F	Deseret	80	80	Deseret
U+10450..U+1047F	Shavian	48	48	Shavian
U+10480..U+104AF	Osmanya	48	40	Osmanya
U+104B0..U+104FF	Osage	80	72	Osage
U+10500..U+1052F	Elbasan	48	40	Elbasan
U+10530..U+1056F	Caucasian Albanian	64	53	Caucasian Albanian
U+10600..U+1077F	Linear A	384	341	Linear A
U+10800..U+1083F	Cypriot Syllabary	64	55	Cypriot
U+10840..U+1085F	Imperial Aramaic	32	31	Imperial Aramaic
U+10860..U+1087F	Palmyrene	32	32	Palmyrene
U+10880..U+108AF	Nabataean	48	40	Nabataean
U+108E0..U+108FF	Hatran	32	26	Hatran
U+10900..U+1091F	Phoenician	32	29	Phoenician
U+10920..U+1093F	Lydian	32	27	Lydian
U+10980..U+1099F	Meroitic Hieroglyphs	32	32	Meroitic Hieroglyphs
U+109A0..U+109FF	Meroitic Cursive	96	90	Meroitic Cursive
U+10A00..U+10A5F	Kharoshthi	96	65	Kharoshthi
U+10A60..U+10A7F	Old South Arabian	32	32	Old South Arabian
U+10A80..U+10A9F	Old North Arabian	32	32	Old North Arabian
U+10AC0..U+10AFF	Manichaean	64	51	Manichaean
U+10B00..U+10B3F	Avestan	64	61	Avestan
U+10B40..U+10B5F	Inscriptional Parthian	32	30	Inscriptional Parthian
U+10B60..U+10B7F	Inscriptional Pahlavi	32	27	Inscriptional Pahlavi
U+10B80..U+10BAF	Psalter Pahlavi	48	29	Psalter Pahlavi
U+10C00..U+10C4F	Old Turkic	80	73	Old Turkic
U+10C80..U+10CFF	Old Hungarian	128	108	Old Hungarian
U+10E60..U+10E7F	Rumi Numeral Symbols	32	31	Arabic
U+11000..U+1107F	Brahmi	128	109	Brahmi
U+11080..U+110CF	Kaithi	80	66	Kaithi
U+110D0..U+110FF	Sora Sompeng	48	35	Sora Sompeng
U+11100..U+1114F	Chakma	80	67	Chakma
U+11150..U+1117F	Mahajani	48	39	Mahajani
U+11180..U+111DF	Sharada	96	94	Sharada
U+111E0..U+111FF	Sinhala Archaic Numbers	32	20	Sinhala
U+11200..U+1124F	Khojki	80	62	Khojki
U+11280..U+112AF	Multani	48	38	Multani
U+112B0..U+112FF	Khudawadi	80	69	Khudawadi
U+11300..U+1137F	Grantha	128	85	Grantha
U+11400..U+1147F	Newa	128	92	Newa
U+11480..U+114DF	Tirhuta	96	82	Tirhuta
U+11580..U+115FF	Siddham	128	92	Siddham
U+11600..U+1165F	Modi	96	79	Modi
U+11660..U+1167F	Mongolian Supplement	32	13	Mongolian
U+11680..U+116CF	Takri	80	66	Takri
U+11700..U+1173F	Ahom	64	57	Ahom
U+118A0..U+118FF	Warang Citi	96	84	Warang Citi
U+11A00..U+11A4F	Zanabazar Square	80	72	Zanabazar Square
U+11A50..U+11AAF	Soyombo	96	80	Soyombo
U+11AC0..U+11AFF	Pau Cin Hau	64	57	Pau Cin Hau
U+11C00..U+11C6F	Bhaiksuki	112	97	Bhaiksuki
U+11C70..U+11CBF	Marchen	80	68	Marchen
U+11D00..U+11D5F	Masaram Gondi	96	75	Masaram Gondi
U+12000..U+123FF	Cuneiform	1,024	922	Cuneiform
U+12400..U+1247F	Cuneiform Numbers and Punctuation	128	116	Cuneiform
U+12480..U+1254F	Early Dynastic Cuneiform	208	196	Cuneiform
U+13000..U+1342F	Egyptian Hieroglyphs	1,072	1,071	Egyptian Hieroglyphs
U+14400..U+1467F	Anatolian Hieroglyphs	640	583	Anatolian Hieroglyphs
U+16800..U+16A3F	Bamum Supplement	576	569	Bamum
U+16A40..U+16A6F	Mro	48	43	Mro
U+16AD0..U+16AFF	Bassa Vah	48	36	Bassa Vah
U+16B00..U+16B8F	Pahawh Hmong	144	127	Pahawh Hmong
U+16F00..U+16F9F	Miao	160	133	Miao
U+16FE0..U+16FFF	Ideographic Symbols and Punctuation	32	2	Nushu (1 character), Tangut (1 character)
U+17000..U+187FF	Tangut	6,144	6,125	Tangut
U+18800..U+18AFF	Tangut Components	768	755	Tangut
U+1B000..U+1B0FF	Kana Supplement	256	256	Hiragana (255 characters), Katakana (1 character)
U+1B100..U+1B12F	Kana Extended-A	48	31	Hiragana
U+1B170..U+1B2FF	Nushu	400	396	Nüshu
U+1BC00..U+1BC9F	Duployan	160	143	Duployan
U+1BCA0..U+1BCAF	Shorthand Format Controls	16	4	Common
U+1D000..U+1D0FF	Byzantine Musical Symbols	256	246	Common
U+1D100..U+1D1FF	Musical Symbols	256	231	Common (209 characters), Inherited (22 characters)
U+1D200..U+1D24F	Ancient Greek Musical Notation	80	70	Greek
U+1D300..U+1D35F	Tai Xuan Jing Symbols	96	87	Common
U+1D360..U+1D37F	Counting Rod Numerals	32	18	Common
U+1D400..U+1D7FF	Mathematical Alphanumeric Symbols	1,024	996	Common
U+1D800..U+1DAAF	Sutton SignWriting	688	672	SignWriting
U+1E000..U+1E02F	Glagolitic Supplement	48	38	Glagolitic
U+1E800..U+1E8DF	Mende Kikakui	224	213	Mende Kikakui
U+1E900..U+1E95F	Adlam	96	87	Adlam
U+1EE00..U+1EEFF	Arabic Mathematical Alphabetic Symbols	256	143	Arabic
U+1F000..U+1F02F	Mahjong Tiles	48	44	Common
U+1F030..U+1F09F	Domino Tiles	112	100	Common
U+1F0A0..U+1F0FF	Playing Cards	96	82	Common
U+1F100..U+1F1FF	Enclosed Alphanumeric Supplement	256	191	Common
U+1F200..U+1F2FF	Enclosed Ideographic Supplement	256	64	Hiragana (1 character), Common (63 characters)
U+1F300..U+1F5FF	Miscellaneous Symbols and Pictographs	768	768	Common
U+1F600..U+1F64F	Emoticons	80	80	Common
U+1F650..U+1F67F	Ornamental Dingbats	48	48	Common
U+1F680..U+1F6FF	Transport and Map Symbols	128	107	Common
U+1F700..U+1F77F	Alchemical Symbols	128	116	Common
U+1F780..U+1F7FF	Geometric Shapes Extended	128	85	Common
U+1F800..U+1F8FF	Supplemental Arrows-C	256	148	Common
U+1F900..U+1F9FF	Supplemental Symbols and Pictographs	256	148	Common
U+20000..U+2A6DF	CJK Unified Ideographs Extension B	42,720	42,711	Han
U+2A700..U+2B73F	CJK Unified Ideographs Extension C	4,160	4,149	Han
U+2B740..U+2B81F	CJK Unified Ideographs Extension D	224	222	Han
U+2B820..U+2CEAF	CJK Unified Ideographs Extension E	5,776	5,762	Han
U+2CEB0..U+2EBEF	CJK Unified Ideographs Extension F	7,488	7,473	Han
U+2F800..U+2FA1F	CJK Compatibility Ideographs Supplement	544	542	Han
U+E0000..U+E007F	Tags	128	97	Common
U+E0100..U+E01EF	Variation Selectors Supplement	240	240	Inherited
U+F0000..U+FFFFF	Supplementary Private Use Area-A	65,536	65,534	Unknown
U+100000..U+10FFFF	Supplementary Private Use Area-B	65,536	65,534	Unknown";
		
	}
}
