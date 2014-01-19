﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ThScoreFileConverter
{
    public class Th11Converter : ThConverter
    {
        private enum Level                  { Easy, Normal, Hard, Lunatic, Extra }
        private enum LevelWithTotal         { Easy, Normal, Hard, Lunatic, Extra, Total }
        private enum LevelShort             { E, N, H, L, X }
        private enum LevelShortWithTotal    { E, N, H, L, X, T }

        private enum Chara
        {
            ReimuYukari, ReimuSuika, ReimuAya, MarisaAlice, MarisaPatchouli, MarisaNitori
        }
        private enum CharaWithTotal
        {
            ReimuYukari, ReimuSuika, ReimuAya, MarisaAlice, MarisaPatchouli, MarisaNitori, Total
        }
        private enum CharaShort             { RY, RS, RA, MA, MP, MN }
        private enum CharaShortWithTotal    { RY, RS, RA, MA, MP, MN, TL }

        private enum Stage { Stage1, Stage2, Stage3, Stage4, Stage5, Stage6, Extra }

        private static readonly string[] StageProgressArray =
        {
            "-------", "Stage 1", "Stage 2", "Stage 3", "Stage 4", "Stage 5", "Stage 6",
            "Not Clear", "All Clear"
        };

        private const int NumCards = 175;

        // Thanks to thwiki.info
        private static readonly Dictionary<Stage, Range<int>> StageCardTable =
            new Dictionary<Stage, Range<int>>()
            {
                { Stage.Stage1, new Range<int>{ Min = 0,   Max = 9   } },
                { Stage.Stage2, new Range<int>{ Min = 10,  Max = 25  } },
                { Stage.Stage3, new Range<int>{ Min = 26,  Max = 41  } },
                { Stage.Stage4, new Range<int>{ Min = 42,  Max = 117 } },
                { Stage.Stage5, new Range<int>{ Min = 118, Max = 137 } },
                { Stage.Stage6, new Range<int>{ Min = 138, Max = 161 } },
                { Stage.Extra,  new Range<int>{ Min = 162, Max = 174 } }
            };

        private class LevelStagePair : Pair<Level, Stage>
        {
            public Level Level { get { return this.First; } }
            public Stage Stage { get { return this.Second; } }

            public LevelStagePair(Level level, Stage stage) : base(level, stage) { }
        }

        private class AllScoreData
        {
            public Header header;
            public Dictionary<CharaWithTotal, ClearData> clearData;
            public Status status;
        }

        private class Header : Utils.IBinaryReadable
        {
            public string Signature { get; private set; }
            public int EncodedAllSize { get; private set; }
            public uint Unknown1 { get; private set; }
            public uint Unknown2 { get; private set; }
            public int EncodedBodySize { get; private set; }
            public int DecodedBodySize { get; private set; }

            public void ReadFrom(BinaryReader reader)
            {
                this.Signature = new string(reader.ReadChars(4));
                this.EncodedAllSize = reader.ReadInt32();
                this.Unknown1 = reader.ReadUInt32();
                this.Unknown2 = reader.ReadUInt32();
                this.EncodedBodySize = reader.ReadInt32();
                this.DecodedBodySize = reader.ReadInt32();
            }

            public void WriteTo(BinaryWriter writer)
            {
                writer.Write(this.Signature.ToCharArray());
                writer.Write(this.EncodedAllSize);
                writer.Write(this.Unknown1);
                writer.Write(this.Unknown2);
                writer.Write(this.EncodedBodySize);
                writer.Write(this.DecodedBodySize);
            }
        }

        private class Chapter : Utils.IBinaryReadable
        {
            public string Signature { get; private set; }
            public ushort Unknown { get; private set; }
            public uint Checksum { get; private set; }
            public int Size { get; private set; }

            public Chapter() { }
            public Chapter(Chapter ch)
            {
                this.Signature = ch.Signature;
                this.Unknown = ch.Unknown;
                this.Checksum = ch.Checksum;
                this.Size = ch.Size;
            }

            public virtual void ReadFrom(BinaryReader reader)
            {
                this.Signature = Encoding.Default.GetString(reader.ReadBytes(2));
                this.Unknown = reader.ReadUInt16();
                this.Checksum = reader.ReadUInt32();
                this.Size = reader.ReadInt32();
            }
        }

        private class ClearData : Chapter   // per character
        {
            public CharaWithTotal Chara { get; private set; }   // size: 4Bytes
            public Dictionary<Level, ScoreData[]> Rankings { get; private set; }
            public int TotalPlayCount { get; private set; }
            public int PlayTime { get; private set; }           // = seconds * 60fps
            public Dictionary<Level, int> ClearCounts { get; private set; }
            public Dictionary<LevelStagePair, Practice> Practices { get; private set; }
            public SpellCard[] Cards { get; private set; }      // [0..174]

            public ClearData(Chapter ch)
                : base(ch)
            {
                if (this.Signature != "CR")
                    throw new InvalidDataException("Signature");
                if (this.Unknown != 0x0000)
                    throw new InvalidDataException("Unknown");
                if (this.Size != 0x000068D4)
                    throw new InvalidDataException("Size");

                var numLevels = Enum.GetValues(typeof(Level)).Length;
                this.Rankings = new Dictionary<Level, ScoreData[]>(numLevels);
                this.ClearCounts = new Dictionary<Level, int>(numLevels);
                this.Practices = new Dictionary<LevelStagePair, Practice>();
                this.Cards = new SpellCard[NumCards];
            }

            public override void ReadFrom(BinaryReader reader)
            {
                var levels = Enum.GetValues(typeof(Level));
                this.Chara = (CharaWithTotal)reader.ReadInt32();
                foreach (Level level in levels)
                {
                    if (!this.Rankings.ContainsKey(level))
                        this.Rankings.Add(level, new ScoreData[10]);
                    for (var rank = 0; rank < 10; rank++)
                    {
                        var score = new ScoreData();
                        score.ReadFrom(reader);
                        this.Rankings[level][rank] = score;
                    }
                }
                this.TotalPlayCount = reader.ReadInt32();
                this.PlayTime = reader.ReadInt32();
                foreach (Level level in levels)
                {
                    var clearCount = reader.ReadInt32();
                    if (!this.ClearCounts.ContainsKey(level))
                        this.ClearCounts.Add(level, clearCount);
                }
                foreach (Level level in levels)
                    if (level != Level.Extra)
                        foreach (Stage stage in Enum.GetValues(typeof(Stage)))
                            if (stage != Stage.Extra)
                            {
                                var practice = new Practice();
                                practice.ReadFrom(reader);
                                var key = new LevelStagePair(level, stage);
                                if (!this.Practices.ContainsKey(key))
                                    this.Practices.Add(key, practice);
                            }
                for (var number = 0; number < NumCards; number++)
                {
                    var card = new SpellCard();
                    card.ReadFrom(reader);
                    this.Cards[number] = card;
                }
            }
        }

        private class Status : Chapter
        {
            public byte[] LastName { get; private set; }    // .Length = 10 (The last 2 bytes are always 0x00 ?)
            public byte[] Unknown1 { get; private set; }    // .Length = 0x10
            public byte[] BgmFlags { get; private set; }    // .Length = 17
            public byte[] Unknown2 { get; private set; }    // .Length = 0x0411

            public Status(Chapter ch)
                : base(ch)
            {
                if (this.Signature != "ST")
                    throw new InvalidDataException("Signature");
                if (this.Unknown != 0x0000)
                    throw new InvalidDataException("Unknown");
                if (this.Size != 0x00000448)
                    throw new InvalidDataException("Size");
            }

            public override void ReadFrom(BinaryReader reader)
            {
                this.LastName = reader.ReadBytes(10);
                this.Unknown1 = reader.ReadBytes(0x10);
                this.BgmFlags = reader.ReadBytes(17);
                this.Unknown2 = reader.ReadBytes(0x0411);
            }
        }

        private class ScoreData : Utils.IBinaryReadable
        {
            public uint Score { get; private set; }         // * 10
            public byte StageProgress { get; private set; }
            public byte ContinueCount { get; private set; }
            public byte[] Name { get; private set; }        // .Length = 10 (The last 2 bytes are always 0x00 ?)
            public uint DateTime { get; private set; }      // UNIX time (unit: [s])
            public float SlowRate { get; private set; }     // really...?
            public uint Unknown1 { get; private set; }

            public void ReadFrom(BinaryReader reader)
            {
                this.Score = reader.ReadUInt32();
                this.StageProgress = reader.ReadByte();
                this.ContinueCount = reader.ReadByte();
                this.Name = reader.ReadBytes(10);
                this.DateTime = reader.ReadUInt32();
                this.SlowRate = reader.ReadSingle();
                this.Unknown1 = reader.ReadUInt32();
            }
        }

        private class SpellCard : Utils.IBinaryReadable
        {
            public byte[] Name { get; private set; }    // .Length = 0x80
            public int ClearCount { get; private set; }
            public int TrialCount { get; private set; }
            public int Number { get; private set; }     // 0-origin
            public Level Level { get; private set; }    // Easy .. Extra

            public void ReadFrom(BinaryReader reader)
            {
                this.Name = reader.ReadBytes(0x80);
                this.ClearCount = reader.ReadInt32();
                this.TrialCount = reader.ReadInt32();
                this.Number = reader.ReadInt32();
                this.Level = (Level)reader.ReadInt32();
            }
        }

        private class Practice : Utils.IBinaryReadable
        {
            public uint Score { get; private set; }     // * 10
            public uint StageFlag { get; private set; } // 0x00000000: disable, 0x00000101: enable ?

            public void ReadFrom(BinaryReader reader)
            {
                this.Score = reader.ReadUInt32();
                this.StageFlag = reader.ReadUInt32();
            }
        }

        private AllScoreData allScoreData = null;

        public override string SupportedVersions
        {
            get
            {
                return "1.00a";
            }
        }

        public Th11Converter() { }

        protected override bool ReadScoreFile(Stream input)
        {
            using (var decrypted = new MemoryStream())
#if DEBUG
            using (var decoded = new FileStream("th11decoded.dat", FileMode.Create, FileAccess.ReadWrite))
#else
            using (var decoded = new MemoryStream())
#endif
            {
                if (!this.Decrypt(input, decrypted))
                    return false;

                decrypted.Seek(0, SeekOrigin.Begin);
                if (!this.Extract(decrypted, decoded))
                    return false;

                decoded.Seek(0, SeekOrigin.Begin);
                if (!this.Validate(decoded))
                    return false;

                decoded.Seek(0, SeekOrigin.Begin);
                this.allScoreData = this.Read(decoded);

                return this.allScoreData != null;
            }
        }

        private bool Decrypt(Stream input, Stream output)
        {
            var reader = new BinaryReader(input);
            var writer = new BinaryWriter(output);

            var header = new Header();
            header.ReadFrom(reader);
            if (header.Signature != "TH11")
                return false;
            if (header.EncodedAllSize != reader.BaseStream.Length)
                return false;

            header.WriteTo(writer);
            ThCrypt.Decrypt(input, output, header.EncodedBodySize, 0xAC, 0x35, 0x10, header.EncodedBodySize);

            return true;
        }

        private bool Extract(Stream input, Stream output)
        {
            var reader = new BinaryReader(input);
            var writer = new BinaryWriter(output);

            var header = new Header();
            header.ReadFrom(reader);
            header.WriteTo(writer);

            var bodyBeginPos = output.Position;
            Lzss.Extract(input, output);
            output.Flush();
            output.SetLength(output.Position);

            return header.DecodedBodySize == (output.Position - bodyBeginPos);
        }

        private bool Validate(Stream input)
        {
            var reader = new BinaryReader(input);

            var header = new Header();
            header.ReadFrom(reader);
            var remainSize = header.DecodedBodySize;
            var chapter = new Chapter();

            try
            {
                while (true)
                {
                    chapter.ReadFrom(reader);

                    if (!((chapter.Signature == "CR") && (chapter.Unknown == 0x0000)) &&
                        !((chapter.Signature == "ST") && (chapter.Unknown == 0x0000)))
                        return false;

                    // -4 means the size of Size.
                    reader.BaseStream.Seek(-4, SeekOrigin.Current);
                    // 8 means the total size of Signature, Unknown, and Checksum.
                    var body = reader.ReadBytes(chapter.Size - 8);
                    var sum = body.Sum(elem => (int)elem);
                    if (sum != chapter.Checksum)
                        return false;

                    remainSize -= chapter.Size;
                }
            }
            catch (EndOfStreamException)
            {
                // It's OK, do nothing.
            }

            return remainSize == 0;
        }

        private AllScoreData Read(Stream input)
        {
            var reader = new BinaryReader(input);
            var allScoreData = new AllScoreData();
            var chapter = new Chapter();
            var numCharas = Enum.GetValues(typeof(CharaWithTotal)).Length;

            allScoreData.clearData = new Dictionary<CharaWithTotal, ClearData>(numCharas);
            allScoreData.header = new Header();
            allScoreData.header.ReadFrom(reader);

            try
            {
                while (true)
                {
                    chapter.ReadFrom(reader);
                    switch (chapter.Signature)
                    {
                        case "CR":
                            var clearData = new ClearData(chapter);
                            clearData.ReadFrom(reader);
                            if (!allScoreData.clearData.ContainsKey(clearData.Chara))
                                allScoreData.clearData.Add(clearData.Chara, clearData);
                            break;
                        case "ST":
                            var status = new Status(chapter);
                            status.ReadFrom(reader);
                            allScoreData.status = status;
                            break;
                        default:
                            // 12 means the total size of Signature, Unknown, Checksum, and Size.
                            reader.ReadBytes(chapter.Size - 12);
                            break;
                    }
                }
            }
            catch (EndOfStreamException)
            {
                // It's OK, do nothing.
            }

            if ((allScoreData.header != null) &&
                (allScoreData.clearData.Count == numCharas) &&
                (allScoreData.status != null))
                return allScoreData;
            else
                return null;
        }

        protected override void Convert(Stream input, Stream output)
        {
            var reader = new StreamReader(input, Encoding.GetEncoding("shift_jis"));
            var writer = new StreamWriter(output, Encoding.GetEncoding("shift_jis"));

            var allLines = reader.ReadToEnd();
            allLines = this.ReplaceScore(allLines);
            allLines = this.ReplaceCareer(allLines);
            allLines = this.ReplaceCard(allLines);
            allLines = this.ReplaceCollectRate(allLines);
            allLines = this.ReplaceClear(allLines);
            allLines = this.ReplaceChara(allLines);
            allLines = this.ReplaceCharaEx(allLines);
            allLines = this.ReplacePractice(allLines);
            writer.Write(allLines);

            writer.Flush();
            writer.BaseStream.SetLength(writer.BaseStream.Position);
        }

        // %T11SCR[w][xx][y][z]
        private string ReplaceScore(string input)
        {
            var pattern = string.Format(
                @"%T11SCR([{0}])({1})(\d)([1-5])",
                Utils.JoinEnumNames<LevelShort>(""),
                Utils.JoinEnumNames<CharaShort>("|"));
            return new Regex(pattern, RegexOptions.IgnoreCase)
                .Replace(input, Utils.ToNothrowEvaluator(match =>
                {
                    var level = (Level)Utils.ParseEnum<LevelShort>(match.Groups[1].Value, true);
                    var chara = (CharaWithTotal)Utils.ParseEnum<CharaShort>(match.Groups[2].Value, true);
                    var rank = (int.Parse(match.Groups[3].Value) + 9) % 10;     // from [1..9, 0] to [0..9]
                    var type = int.Parse(match.Groups[4].Value);

                    var ranking = this.allScoreData.clearData[chara].Rankings[level][rank];
                    switch (type)
                    {
                        case 1:     // name
                            return Encoding.Default.GetString(ranking.Name).Split('\0')[0];
                        case 2:     // score
                            return this.ToNumberString(ranking.Score * 10 + ranking.ContinueCount);
                        case 3:     // stage
                            if (ranking.DateTime > 0)
                                return (ranking.StageProgress < StageProgressArray.Length)
                                    ? StageProgressArray[ranking.StageProgress]
                                    : StageProgressArray[0];
                            else
                                return StageProgressArray[0];
                        case 4:     // date & time
                            if (ranking.DateTime > 0)
                                return new DateTime(1970, 1, 1).AddSeconds(ranking.DateTime)
                                    .ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss");
                            else
                                return "----/--/-- --:--:--";
                        case 5:     // slow
                            if (ranking.DateTime > 0)
                                return ranking.SlowRate.ToString("F3") + "%";
                            else
                                return "-----%";
                        default:    // unreachable
                            return match.ToString();
                    }
                }));
        }

        // %T11C[xxx][yy][z]
        private string ReplaceCareer(string input)
        {
            var pattern = string.Format(
                @"%T11C(\d\d\d)({0})([12])",
                Utils.JoinEnumNames<CharaShortWithTotal>("|"));
            return new Regex(pattern, RegexOptions.IgnoreCase)
                .Replace(input, Utils.ToNothrowEvaluator(match =>
                {
                    var number = int.Parse(match.Groups[1].Value);
                    var chara = Utils.ParseEnum<CharaShortWithTotal>(match.Groups[2].Value, true);
                    var type = int.Parse(match.Groups[3].Value);

                    var cards = this.allScoreData.clearData[(CharaWithTotal)chara].Cards;
                    if (number == 0)
                        return this.ToNumberString(
                            (type == 1) ? cards.Sum(card => card.ClearCount) : cards.Sum(card => card.TrialCount));
                    else if ((0 < number) && (number <= NumCards))
                        return this.ToNumberString(
                            (type == 1) ? cards[number - 1].ClearCount : cards[number - 1].TrialCount);
                    else
                        return match.ToString();
                }));
        }

        // %T11CARD[xxx][y]
        private string ReplaceCard(string input)
        {
            return new Regex(@"%T11CARD(\d{3})([NR])", RegexOptions.IgnoreCase)
                .Replace(input, Utils.ToNothrowEvaluator(match =>
                {
                    var number = int.Parse(match.Groups[1].Value);
                    var type = match.Groups[2].Value.ToUpper();

                    if ((0 < number) && (number <= NumCards))
                    {
                        var card = this.allScoreData.clearData[CharaWithTotal.Total].Cards[number - 1];
                        if (type == "N")
                        {
                            var name = Encoding.Default.GetString(card.Name).TrimEnd('\0');
                            return (name.Length > 0) ? name : "??????????";
                        }
                        else
                            return card.Level.ToString();
                    }
                    else
                        return match.ToString();
                }));
        }

        // %T11CRG[w][xx][y][z]
        private string ReplaceCollectRate(string input)
        {
            var pattern = string.Format(
                @"%T11CRG([{0}])({1})([0-6])([12])",
                Utils.JoinEnumNames<LevelShortWithTotal>(""),
                Utils.JoinEnumNames<CharaShortWithTotal>("|"));
            return new Regex(pattern, RegexOptions.IgnoreCase)
                .Replace(input, Utils.ToNothrowEvaluator(match =>
                {
                    var level = Utils.ParseEnum<LevelShortWithTotal>(match.Groups[1].Value, true);
                    var chara = Utils.ParseEnum<CharaShortWithTotal>(match.Groups[2].Value, true);
                    var stage = int.Parse(match.Groups[3].Value);   // 0: total of all stages
                    var type = int.Parse(match.Groups[4].Value);

                    Func<SpellCard, bool> findCard = (card => false);

                    if (level == LevelShortWithTotal.T)
                    {
                        if (stage == 0)     // total
                        {
                            if (type == 1)
                                findCard = (card => (card.ClearCount > 0));
                            else
                                findCard = (card => (card.TrialCount > 0));
                        }
                        else
                        {
                            var st = (Stage)(stage - 1);
                            if (type == 1)
                                findCard = (card =>
                                    (StageCardTable[st].Contains(card.Number) &&
                                    (card.ClearCount > 0)));
                            else
                                findCard = (card =>
                                    (StageCardTable[st].Contains(card.Number) &&
                                    (card.TrialCount > 0)));
                        }
                    }
                    else if (level == LevelShortWithTotal.X)
                    {
                        if (type == 1)
                            findCard = (card =>
                                (StageCardTable[Stage.Extra].Contains(card.Number) &&
                                (card.ClearCount > 0)));
                        else
                            findCard = (card =>
                                (StageCardTable[Stage.Extra].Contains(card.Number) &&
                                (card.TrialCount > 0)));
                    }
                    else
                    {
                        var lv = (Level)level;
                        if (stage == 0)     // total
                        {
                            if (type == 1)
                                findCard = (card => ((card.Level == lv) && (card.ClearCount > 0)));
                            else
                                findCard = (card => ((card.Level == lv) && (card.TrialCount > 0)));
                        }
                        else
                        {
                            var st = (Stage)(stage - 1);
                            if (type == 1)
                                findCard = (card =>
                                    (StageCardTable[st].Contains(card.Number) &&
                                    (card.Level == lv) &&
                                    (card.ClearCount > 0)));
                            else
                                findCard = (card =>
                                    (StageCardTable[st].Contains(card.Number) &&
                                    (card.Level == lv) &&
                                    (card.TrialCount > 0)));
                        }
                    }

                    return this.allScoreData.clearData[(CharaWithTotal)chara].Cards.Count(findCard).ToString();
                }));
        }

        // %T11CLEAR[x][yy]
        private string ReplaceClear(string input)
        {
            var pattern = string.Format(
                @"%T11CLEAR([{0}])({1})",
                Utils.JoinEnumNames<LevelShort>(""),
                Utils.JoinEnumNames<CharaShort>("|"));
            return new Regex(pattern, RegexOptions.IgnoreCase)
                .Replace(input, Utils.ToNothrowEvaluator(match =>
                {
                    var level = (Level)Utils.ParseEnum<LevelShort>(match.Groups[1].Value, true);
                    var chara = (CharaWithTotal)Utils.ParseEnum<CharaShort>(match.Groups[2].Value, true);

                    var stageProgress = 0;
                    for (var rank = 0; rank < 10; rank++)
                    {
                        var ranking = this.allScoreData.clearData[chara].Rankings[level][rank];
                        if (ranking.DateTime > 0)
                            stageProgress = Math.Max(stageProgress, ranking.StageProgress);
                    }

                    if (stageProgress < StageProgressArray.Length)
                    {
                        if (level != Level.Extra)
                            return (stageProgress != 7)
                                ? StageProgressArray[stageProgress] : StageProgressArray[0];
                        else
                            return (stageProgress >= 7)
                                ? StageProgressArray[stageProgress] : StageProgressArray[0];
                    }
                    else
                        return StageProgressArray[0];
                }));
        }

        // %T11CHARA[xx][y]
        private string ReplaceChara(string input)
        {
            var pattern = string.Format(
                @"%T11CHARA({0})([1-3])",
                Utils.JoinEnumNames<CharaShortWithTotal>("|"));
            return new Regex(pattern, RegexOptions.IgnoreCase)
                .Replace(input, Utils.ToNothrowEvaluator(match =>
                {
                    var chara =
                        (CharaWithTotal)Utils.ParseEnum<CharaShortWithTotal>(match.Groups[1].Value, true);
                    var type = int.Parse(match.Groups[2].Value);

                    switch (type)
                    {
                        case 1:     // total play count
                            if (chara == CharaWithTotal.Total)
                                return this.ToNumberString(
                                    this.allScoreData.clearData.Values.Sum(
                                        data => (data.Chara != chara) ? data.TotalPlayCount : 0));
                            else
                                return this.ToNumberString(
                                    this.allScoreData.clearData[chara].TotalPlayCount);
                        case 2:     // play times
                            {
                                var frames = (chara == CharaWithTotal.Total)
                                    ? this.allScoreData.clearData.Values.Sum(
                                        data => (data.Chara != chara) ? (long)data.PlayTime : 0L)
                                    :  (long)this.allScoreData.clearData[chara].PlayTime;
                                return new Time(frames).ToString();
                            }
                        case 3:     // clear count
                            if (chara == CharaWithTotal.Total)
                                return this.ToNumberString(
                                    this.allScoreData.clearData.Values.Sum(
                                        data => (data.Chara != chara)
                                            ? data.ClearCounts.Values.Sum(count => count) : 0));
                            else
                                return this.ToNumberString(
                                    this.allScoreData.clearData[chara].ClearCounts.Values.Sum(count => count));
                        default:    // unreachable
                            return match.ToString();
                    }
                }));
        }

        // %T11CHARAEX[x][yy][z]
        private string ReplaceCharaEx(string input)
        {
            var pattern = string.Format(
                @"%T11CHARAEX([{0}])({1})([1-3])",
                Utils.JoinEnumNames<LevelShortWithTotal>(""),
                Utils.JoinEnumNames<CharaShortWithTotal>("|"));
            return new Regex(pattern, RegexOptions.IgnoreCase)
                .Replace(input, Utils.ToNothrowEvaluator(match =>
                {
                    var level =
                        (LevelWithTotal)Utils.ParseEnum<LevelShortWithTotal>(match.Groups[1].Value, true);
                    var chara =
                        (CharaWithTotal)Utils.ParseEnum<CharaShortWithTotal>(match.Groups[2].Value, true);
                    var type = int.Parse(match.Groups[3].Value);

                    switch (type)
                    {
                        case 1:     // total play count
                            if (chara == CharaWithTotal.Total)
                                return this.ToNumberString(
                                    this.allScoreData.clearData.Values.Sum(
                                        data => (data.Chara != chara) ? data.TotalPlayCount : 0));
                            else
                                return this.allScoreData.clearData[chara].TotalPlayCount.ToString();
                        case 2:     // play times
                            {
                                var frames = (chara == CharaWithTotal.Total)
                                    ? this.allScoreData.clearData.Values.Sum(
                                        data => ((data.Chara != chara) ? (long)data.PlayTime : 0L))
                                    : (long)this.allScoreData.clearData[chara].PlayTime;
                                return new Time(frames).ToString();
                            }
                        case 3:     // clear count
                            if (chara == CharaWithTotal.Total)
                            {
                                if (level == LevelWithTotal.Total)
                                    return this.ToNumberString(
                                        this.allScoreData.clearData.Values.Sum(
                                            data => (data.Chara != chara)
                                                ? data.ClearCounts.Values.Sum(count => count) : 0));
                                else
                                    return this.ToNumberString(
                                        this.allScoreData.clearData.Values.Sum(
                                            data => (data.Chara != chara)
                                                ? data.ClearCounts[(Level)level] : 0));
                            }
                            else
                            {
                                if (level == LevelWithTotal.Total)
                                    return this.ToNumberString(
                                        this.allScoreData.clearData[chara].ClearCounts.Values.Sum(count => count));
                                else
                                    return this.ToNumberString(this.allScoreData.
                                        clearData[chara].ClearCounts[(Level)level]);
                            }
                        default:    // unreachable
                            return match.ToString();
                    }
                }));
        }

        // %T11PRAC[x][yy][z]
        private string ReplacePractice(string input)
        {
            var pattern = string.Format(
                @"%T11PRAC([{0}])({1})([1-6])",
                Utils.JoinEnumNames<LevelShort>(""),
                Utils.JoinEnumNames<CharaShort>("|"));
            return new Regex(pattern, RegexOptions.IgnoreCase)
                .Replace(input, Utils.ToNothrowEvaluator(match =>
                {
                    var level = (Level)Utils.ParseEnum<LevelShort>(match.Groups[1].Value, true);
                    var chara = (CharaWithTotal)Utils.ParseEnum<CharaShort>(match.Groups[2].Value, true);
                    var stage = (Stage)(int.Parse(match.Groups[3].Value) - 1);

                    if (level == Level.Extra)
                        return match.ToString();

                    if (this.allScoreData.clearData.ContainsKey(chara))
                    {
                        var key = new LevelStagePair(level, stage);
                        var practices = this.allScoreData.clearData[chara].Practices;
                        return this.ToNumberString(
                            practices.ContainsKey(key) ? (practices[key].Score * 10) : 0);
                    }
                    else
                        return "0";
                }));
        }
    }
}
