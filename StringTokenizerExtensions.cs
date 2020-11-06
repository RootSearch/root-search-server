using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.Extensions.Primitive
{
	public static class StringTokenizerExtensions
	{
		/// <summary>
		/// Score tokens based on tf-idf alike algorithm.
		/// </summary>
		public static void Score(this StringTokenizer stringTokenizer, ref ConcurrentDictionary<string, uint> scores)
		{
			try
			{
				foreach (var token in stringTokenizer)
				{
					var normalized = true;

					var segment = token.Trim().Value.ToLower();

					normalized &= TryStripSpecialCharacters(ref segment);
					normalized &= TryFilterByStopWords(ref segment);
					normalized &= TryUnescape(ref segment);
					normalized &= TryFilterNumeric(ref segment);

					if (normalized && segment.Length > 0) {
						scores.AddOrUpdate(segment, 1, (k, v) => v + 1);
					}
				}
			}
			catch (OverflowException e)
			{

			}
		}

		private static bool TryStripSpecialCharacters(ref string segment)
		{
			segment = new string(segment.Select(c => char.IsLetterOrDigit(c) ? c : '\0').ToArray());
			return true;
		}

		private static bool TryFilterByStopWords(ref string segment)
		{
			return !StopWordList.Contains(segment);
		}

		private static bool TryUnescape(ref string segment)
		{
			segment = Regex.Unescape(segment).Replace("\0", string.Empty);
			return true;
		}

		private static bool TryFilterNumeric(ref string segment)
		{
			return !segment.All(Char.IsDigit);
		}

		private static HashSet<string> StopWordList = new HashSet<string>
		{
			"secondly",
			"all",
			"consider",
			"whoever",
			"four",
			"hath",
			"edu",
			"go",
			"causes",
			"seemed",
			"whose",
			"certainly",
			"when's",
			"whoso",
			"to",
			"'twill",
			"present",
			"th",
			"under",
			"sorry",
			"a's",
			"sent",
			"they'll",
			"far",
			"anent",
			"novel",
			"every",
			"yourselves",
			"we'll",
			"sans",
			"did",
			"cause",
			"they've",
			"large",
			"p",
			"small",
			"betwixt",
			"oh",
			"round",
			"it'll",
			"i'll",
			"says",
			"their's",
			"nigh",
			"yourself",
			"past",
			"likely",
			"notwithstanding",
			"further",
			"even",
			"what",
			"appear",
			"exactly",
			"near",
			"anywhere",
			"above",
			"sup",
			"new",
			"ever",
			"public",
			"c'mon",
			"respectively",
			"never",
			"here",
			"whichsoever",
			"let",
			"others",
			"hers",
			"along",
			"aren't",
			"'twere",
			"k",
			"allows",
			"later",
			"i'd",
			"howbeit",
			"usually",
			"que",
			"i'm",
			"changes",
			"thats",
			"'twas",
			"hither",
			"via",
			"followed",
			"merely",
			"till",
			"viz",
			"everybody",
			"use",
			"from",
			"would",
			"contains",
			"two",
			"next",
			"few",
			"therefore",
			"taken",
			"themselves",
			"thru",
			"until",
			"today",
			"more",
			"knows",
			"clearly",
			"becomes",
			"hereby",
			"herein",
			"ain't",
			"particular",
			"known",
			"who'll",
			"midst",
			"must",
			"me",
			"none",
			"f",
			"this",
			"getting",
			"ere",
			"vs",
			"nine",
			"can",
			"theirs",
			"following",
			"my",
			"example",
			"sub",
			"indicated",
			"didn't",
			"high",
			"indicates",
			"something",
			"want",
			"nigher",
			"needs",
			"daring",
			"rather",
			"ie",
			"six",
			"very",
			"instead",
			"okay",
			"tried",
			"may",
			"after",
			"containing",
			"hereupon",
			"such",
			"a",
			"short",
			"third",
			"that'd",
			"amid",
			"appreciate",
			"q",
			"ones",
			"so",
			"specifying",
			"allow",
			"keeps",
			"looking",
			"wouldst",
			"that's",
			"help",
			"don't",
			"indeed",
			"over",
			"despite",
			"mainly",
			"soon",
			"whilst",
			"through",
			"looks",
			"still",
			"thyself",
			"its",
			"before",
			"thank",
			"he's",
			"selves",
			"inward",
			"actually",
			"he'd",
			"willing",
			"asking",
			"thanx",
			"ours",
			"might",
			"haven't",
			"instantly",
			"versus",
			"then",
			"them",
			"someone",
			"somebody",
			"thereby",
			"thee",
			"ye",
			"underneath",
			"touching",
			"you've",
			"they",
			"not",
			"now",
			"nor",
			"wont",
			"gets",
			"hereafter",
			"always",
			"reasonably",
			"qv",
			"whither",
			"l",
			"each",
			"beneath",
			"mustn't",
			"isn't",
			"mean",
			"em",
			"everyone",
			"agin",
			"whosoever",
			"doing",
			"eg",
			"hard",
			"we'd",
			"our",
			"beyond",
			"hadn't",
			"special",
			"really",
			"living",
			"twice",
			"needn't",
			"furthermore",
			"since",
			"excepting",
			"rd",
			"re",
			"seriously",
			"shouldn't",
			"whencesoever",
			"got",
			"forth",
			"thereupon",
			"doesn't",
			"given",
			"quite",
			"qua",
			"aforesaid",
			"what'll",
			"whereupon",
			"besides",
			"ask",
			"anyhow",
			"entirely",
			"g",
			"could",
			"needing",
			"tries",
			"keep",
			"american",
			"thine",
			"w",
			"ltd",
			"hence",
			"onto",
			"think",
			"first",
			"already",
			"saving",
			"seeming",
			"thereafter",
			"one",
			"done",
			"amidst",
			"directly",
			"twas",
			"open",
			"you'd",
			"awfully",
			"you're",
			"little",
			"com",
			"needed",
			"inasmuch",
			"least",
			"name",
			"anyone",
			"their",
			"too",
			"circa",
			"gives",
			"shell",
			"mostly",
			"that",
			"nobody",
			"took",
			"immediate",
			"regards",
			"somewhat",
			"that'll",
			"believe",
			"herself",
			"than",
			"here's",
			"albeit",
			"b",
			"unfortunately",
			"wert",
			"o'er",
			"gotten",
			"gotta",
			"second",
			"i",
			"r",
			"were",
			"toward",
			"minus",
			"anyways",
			"and",
			"respecting",
			"alongside",
			"beforehand",
			"mine",
			"say",
			"unlikely",
			"have",
			"need",
			"seen",
			"seem",
			"obviously",
			"saw",
			"any",
			"relatively",
			"zero",
			"thoroughly",
			"latter",
			"i've",
			"able",
			"mid",
			"thorough",
			"also",
			"astride",
			"which",
			"wanting",
			"towards",
			"unless",
			"though",
			"who",
			"where's",
			"dares",
			"eight",
			"but",
			"nothing",
			"why",
			"shalt",
			"especially",
			"went",
			"sometimes",
			"m",
			"you'll",
			"definitely",
			"normally",
			"came",
			"saying",
			"particularly",
			"couldst",
			"anyway",
			"brief",
			"indicate",
			"fifth",
			"aint",
			"daren't",
			"with",
			"outside",
			"won't",
			"should",
			"only",
			"going",
			"specify",
			"do",
			"his",
			"goes",
			"get",
			"hopefully",
			"overall",
			"truly",
			"they'd",
			"oughtn't",
			"cannot",
			"nearly",
			"durst",
			"during",
			"'twould",
			"him",
			"regarding",
			"nisi",
			"bar",
			"course",
			"h",
			"mayn't",
			"wasn't",
			"she",
			"contain",
			"x",
			"where",
			"greetings",
			"ignored",
			"up",
			"namely",
			"are",
			"close",
			"no-one",
			"best",
			"wonder",
			"said",
			"away",
			"currently",
			"please",
			"behind",
			"there's",
			"various",
			"dost",
			"between",
			"probably",
			"neither",
			"across",
			"available",
			"we",
			"however",
			"come",
			"both",
			"cos",
			"providing",
			"last",
			"'neath",
			"thou",
			"many",
			"wouldn't",
			"thence",
			"according",
			"against",
			"etc",
			"s",
			"became",
			"whole",
			"can't",
			"otherwise",
			"among",
			"presumably",
			"co",
			"afterwards",
			"seems",
			"whatever",
			"hast",
			"non",
			"couldn't",
			"moreover",
			"summat",
			"considering",
			"better",
			"described",
			"it's",
			"three",
			"been",
			"whom",
			"much",
			"likewise",
			"hardly",
			"it'd",
			"wants",
			"corresponding",
			"aslant",
			"latterly",
			"concerning",
			"c",
			"else",
			"athwart",
			"consequently",
			"doth",
			"former",
			"those",
			"myself",
			"save",
			"look",
			"unlike",
			"these",
			"nd",
			"value",
			"n",
			"will",
			"while",
			"pending",
			"ill",
			"theres",
			"seven",
			"whereafter",
			"whenever",
			"almost",
			"wherever",
			"is",
			"everywhere",
			"thus",
			"it",
			"cant",
			"itself",
			"in",
			"alone",
			"id",
			"whore",
			"if",
			"different",
			"perhaps",
			"insofar",
			"same",
			"wherein",
			"beside",
			"another",
			"several",
			"weren't",
			"used",
			"vis-a-vis",
			"see",
			"somewhere",
			"upon",
			"kept",
			"uses",
			"supposing",
			"he'll",
			"off",
			"whereby",
			"nevertheless",
			"try",
			"failing",
			"well",
			"anybody",
			"dared",
			"tho",
			"without",
			"comes",
			"english",
			"y",
			"the",
			"yours",
			"left",
			"lest",
			"just",
			"less",
			"being",
			"downwards",
			"liked",
			"aside",
			"thanks",
			"using",
			"useful",
			"yes",
			"lately",
			"yet",
			"unto",
			"we've",
			"had",
			"except",
			"has",
			"ought",
			"take",
			"real",
			"t's",
			"around",
			"who's",
			"possible",
			"gonna",
			"early",
			"whichever",
			"five",
			"know",
			"afore",
			"immediately",
			"apart",
			"who'd",
			"dare",
			"d",
			"necessary",
			"like",
			"follows",
			"either",
			"become",
			"therein",
			"shed",
			"because",
			"old",
			"often",
			"some",
			"back",
			"self",
			"sure",
			"specified",
			"dear",
			"home",
			"ourselves",
			"shan't",
			"happens",
			"provided",
			"throughout",
			"for",
			"shall",
			"per",
			"everything",
			"does",
			"provides",
			"tends",
			"t",
			"somehow",
			"be",
			"mightn't",
			"sensible",
			"how",
			"nowhere",
			"although",
			"by",
			"on",
			"about",
			"ok",
			"anything",
			"most",
			"of",
			"o",
			"whence",
			"plus",
			"wanna",
			"or",
			"seeing",
			"own",
			"formerly",
			"into",
			"within",
			"down",
			"appropriate",
			"right",
			"c's",
			"your",
			"how's",
			"her",
			"there",
			"long",
			"accordingly",
			"inner",
			"way",
			"was",
			"nighest",
			"himself",
			"elsewhere",
			"enough",
			"becoming",
			"amongst",
			"thro'",
			"hi",
			"trying",
			"true",
			"he",
			"they're",
			"whether",
			"wish",
			"inside",
			"j",
			"hasn't",
			"barring",
			"tell",
			"placed",
			"below",
			"un",
			"z",
			"noone",
			"ex",
			"gone",
			"v",
			"associated",
			"certain",
			"am",
			"an",
			"meanwhile",
			"as",
			"sometime",
			"at",
			"et",
			"inc",
			"again",
			"maybe",
			"us",
			"no",
			"whereas",
			"'twixt",
			"when",
			"tis",
			"other",
			"you",
			"out",
			"what's",
			"'tween",
			"oneself",
			"regardless",
			"welcome",
			"let's",
			"important",
			"serious",
			"ago",
			"e",
			"vice",
			"together",
			"hello",
			"u",
			"we're",
			"she's",
			"having",
			"once"
		};
	}
}
