﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace ThScoreFileConverter
{
    public class Th095Converter : ThConverter
    {
        private static readonly string[] LevelArray =
        {
            "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "ex"
        };
        private static readonly string[] LevelShortArray =
        {
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "X"
        };

        private enum Enemy
        {
            Wriggle, Rumia, Cirno, Letty, Alice, Keine, Medicine, Tewi, Reisen, Meirin, Patchouli,
            Chen, Youmu, Sakuya, Remilia, Ran, Yuyuko, Eirin, Kaguya, Komachi, Shikieiki,
            Flandre, Yukari, Mokou, Suika
        }

        private static readonly Dictionary<Enemy, string> EnemyNames = new Dictionary<Enemy, string>()
        {
            { Enemy.Wriggle,    "リグル・ナイトバグ"         },
            { Enemy.Rumia,      "ルーミア"                   },
            { Enemy.Cirno,      "チルノ"                     },
            { Enemy.Letty,      "レティ・ホワイトロック"     },
            { Enemy.Alice,      "アリス・マーガトロイド"     },
            { Enemy.Keine,      "上白沢 慧音"                },
            { Enemy.Medicine,   "メディスン・メランコリー"   },
            { Enemy.Tewi,       "因幡 てゐ"                  },
            { Enemy.Reisen,     "鈴仙・優曇華院・イナバ"     },
            { Enemy.Meirin,     "紅 美鈴"                    },
            { Enemy.Patchouli,  "パチュリー・ノーレッジ"     },
            { Enemy.Chen,       "橙"                         },
            { Enemy.Youmu,      "魂魄 妖夢"                  },
            { Enemy.Sakuya,     "十六夜 咲夜"                },
            { Enemy.Remilia,    "レミリア・スカーレット"     },
            { Enemy.Ran,        "八雲 藍"                    },
            { Enemy.Yuyuko,     "西行寺 幽々子"              },
            { Enemy.Eirin,      "八意 永琳"                  },
            { Enemy.Kaguya,     "蓬莱山 輝夜"                },
            { Enemy.Komachi,    "小野塚 小町"                },
            { Enemy.Shikieiki,  "四季映姫・ヤマザナドゥ"     },
            { Enemy.Flandre,    "フランドール・スカーレット" },
            { Enemy.Yukari,     "八雲 紫"                    },
            { Enemy.Mokou,      "藤原 妹紅"                  },
            { Enemy.Suika,      "伊吹 萃香"                  }
        };

        private class LevelScenePair : Pair<int, int>
        {
            public int Level { get { return this.First; } }     // 1-origin
            public int Scene { get { return this.Second; } }    // 1-origin

            public LevelScenePair(int level, int scene) : base(level, scene) { }
        }

        private class EnemyCardPair : Pair<Enemy, string>
        {
            public Enemy Enemy { get { return this.First; } }
            public string Card { get { return this.Second; } }

            public EnemyCardPair(Enemy enemy, string card) : base(enemy, card) { }
        }

        // Thanks to thwiki.info
        private static readonly Dictionary<LevelScenePair, EnemyCardPair> SpellCards =
            new Dictionary<LevelScenePair, EnemyCardPair>()
            {
                { new LevelScenePair(1, 1), new EnemyCardPair(Enemy.Wriggle, "") },
                { new LevelScenePair(1, 2), new EnemyCardPair(Enemy.Rumia,   "") },
                { new LevelScenePair(1, 3), new EnemyCardPair(Enemy.Wriggle, "蛍符「地上の恒星」") },
                { new LevelScenePair(1, 4), new EnemyCardPair(Enemy.Rumia,   "闇符「ダークサイドオブザムーン」") },
                { new LevelScenePair(1, 5), new EnemyCardPair(Enemy.Wriggle, "蝶符「バタフライストーム」") },
                { new LevelScenePair(1, 6), new EnemyCardPair(Enemy.Rumia,   "夜符「ミッドナイトバード」") },

                { new LevelScenePair(2, 1), new EnemyCardPair(Enemy.Cirno, "") },
                { new LevelScenePair(2, 2), new EnemyCardPair(Enemy.Letty, "") },
                { new LevelScenePair(2, 3), new EnemyCardPair(Enemy.Cirno, "雪符「ダイアモンドブリザード」") },
                { new LevelScenePair(2, 4), new EnemyCardPair(Enemy.Letty, "寒符「コールドスナップ」") },
                { new LevelScenePair(2, 5), new EnemyCardPair(Enemy.Cirno, "凍符「マイナスＫ」") },
                { new LevelScenePair(2, 6), new EnemyCardPair(Enemy.Letty, "冬符「ノーザンウイナー」") },

                { new LevelScenePair(3, 1), new EnemyCardPair(Enemy.Alice, "") },
                { new LevelScenePair(3, 2), new EnemyCardPair(Enemy.Keine, "光符「アマテラス」") },
                { new LevelScenePair(3, 3), new EnemyCardPair(Enemy.Alice, "操符「ドールズインシー」") },
                { new LevelScenePair(3, 4), new EnemyCardPair(Enemy.Keine, "包符「昭和の雨」") },
                { new LevelScenePair(3, 5), new EnemyCardPair(Enemy.Alice, "呪符「ストロードールカミカゼ」") },
                { new LevelScenePair(3, 6), new EnemyCardPair(Enemy.Keine, "葵符「水戸の光圀」") },
                { new LevelScenePair(3, 7), new EnemyCardPair(Enemy.Alice, "赤符「ドールミラセティ」") },
                { new LevelScenePair(3, 8), new EnemyCardPair(Enemy.Keine, "倭符「邪馬台の国」") },

                { new LevelScenePair(4, 1), new EnemyCardPair(Enemy.Reisen,   "") },
                { new LevelScenePair(4, 2), new EnemyCardPair(Enemy.Medicine, "霧符「ガシングガーデン」") },
                { new LevelScenePair(4, 3), new EnemyCardPair(Enemy.Tewi,     "脱兎「フラスターエスケープ」") },
                { new LevelScenePair(4, 4), new EnemyCardPair(Enemy.Reisen,   "散符「朧月花栞（ロケット・イン・ミスト）」") },
                { new LevelScenePair(4, 5), new EnemyCardPair(Enemy.Medicine, "毒符「ポイズンブレス」") },
                { new LevelScenePair(4, 6), new EnemyCardPair(Enemy.Reisen,   "波符「幻の月（インビジブルハーフムーン）」") },
                { new LevelScenePair(4, 7), new EnemyCardPair(Enemy.Medicine, "譫妄「イントゥデリリウム」") },
                { new LevelScenePair(4, 8), new EnemyCardPair(Enemy.Tewi,     "借符「大穴牟遅様の薬」") },
                { new LevelScenePair(4, 9), new EnemyCardPair(Enemy.Reisen,   "狂夢「風狂の夢（ドリームワールド）」") },

                { new LevelScenePair(5, 1), new EnemyCardPair(Enemy.Meirin,    "") },
                { new LevelScenePair(5, 2), new EnemyCardPair(Enemy.Patchouli, "日＆水符「ハイドロジェナスプロミネンス」") },
                { new LevelScenePair(5, 3), new EnemyCardPair(Enemy.Meirin,    "華符「彩光蓮華掌」") },
                { new LevelScenePair(5, 4), new EnemyCardPair(Enemy.Patchouli, "水＆火符「フロギスティックレイン」") },
                { new LevelScenePair(5, 5), new EnemyCardPair(Enemy.Meirin,    "彩翔「飛花落葉」") },
                { new LevelScenePair(5, 6), new EnemyCardPair(Enemy.Patchouli, "月＆木符「サテライトヒマワリ」") },
                { new LevelScenePair(5, 7), new EnemyCardPair(Enemy.Meirin,    "彩華「虹色太極拳」") },
                { new LevelScenePair(5, 8), new EnemyCardPair(Enemy.Patchouli, "日＆月符「ロイヤルダイアモンドリング」") },

                { new LevelScenePair(6, 1), new EnemyCardPair(Enemy.Chen,  "") },
                { new LevelScenePair(6, 2), new EnemyCardPair(Enemy.Youmu, "人智剣「天女返し」") },
                { new LevelScenePair(6, 3), new EnemyCardPair(Enemy.Chen,  "星符「飛び重ね鱗」") },
                { new LevelScenePair(6, 4), new EnemyCardPair(Enemy.Youmu, "妄執剣「修羅の血」") },
                { new LevelScenePair(6, 5), new EnemyCardPair(Enemy.Chen,  "鬼神「鳴動持国天」") },
                { new LevelScenePair(6, 6), new EnemyCardPair(Enemy.Youmu, "天星剣「涅槃寂静の如し」") },
                { new LevelScenePair(6, 7), new EnemyCardPair(Enemy.Chen,  "化猫「橙」") },
                { new LevelScenePair(6, 8), new EnemyCardPair(Enemy.Youmu, "四生剣「衆生無情の響き」") },

                { new LevelScenePair(7, 1), new EnemyCardPair(Enemy.Sakuya,  "") },
                { new LevelScenePair(7, 2), new EnemyCardPair(Enemy.Remilia, "魔符「全世界ナイトメア」") },
                { new LevelScenePair(7, 3), new EnemyCardPair(Enemy.Sakuya,  "時符「トンネルエフェクト」") },
                { new LevelScenePair(7, 4), new EnemyCardPair(Enemy.Remilia, "紅符「ブラッディマジックスクウェア」") },
                { new LevelScenePair(7, 5), new EnemyCardPair(Enemy.Sakuya,  "空虚「インフレーションスクウェア」") },
                { new LevelScenePair(7, 6), new EnemyCardPair(Enemy.Remilia, "紅蝙蝠「ヴァンピリッシュナイト」") },
                { new LevelScenePair(7, 7), new EnemyCardPair(Enemy.Sakuya,  "銀符「パーフェクトメイド」") },
                { new LevelScenePair(7, 8), new EnemyCardPair(Enemy.Remilia, "神鬼「レミリアストーカー」") },

                { new LevelScenePair(8, 1), new EnemyCardPair(Enemy.Ran,    "") },
                { new LevelScenePair(8, 2), new EnemyCardPair(Enemy.Yuyuko, "幽雅「死出の誘蛾灯」") },
                { new LevelScenePair(8, 3), new EnemyCardPair(Enemy.Ran,    "密符「御大師様の秘鍵」") },
                { new LevelScenePair(8, 4), new EnemyCardPair(Enemy.Yuyuko, "蝶符「鳳蝶紋の死槍」") },
                { new LevelScenePair(8, 5), new EnemyCardPair(Enemy.Ran,    "行符「八千万枚護摩」") },
                { new LevelScenePair(8, 6), new EnemyCardPair(Enemy.Yuyuko, "死符「酔人の生、死の夢幻」") },
                { new LevelScenePair(8, 7), new EnemyCardPair(Enemy.Ran,    "超人「飛翔役小角」") },
                { new LevelScenePair(8, 8), new EnemyCardPair(Enemy.Yuyuko, "「死蝶浮月」") },

                { new LevelScenePair(9, 1), new EnemyCardPair(Enemy.Eirin,  "") },
                { new LevelScenePair(9, 2), new EnemyCardPair(Enemy.Kaguya, "新難題「月のイルメナイト」") },
                { new LevelScenePair(9, 3), new EnemyCardPair(Enemy.Eirin,  "薬符「胡蝶夢丸ナイトメア」") },
                { new LevelScenePair(9, 4), new EnemyCardPair(Enemy.Kaguya, "新難題「エイジャの赤石」") },
                { new LevelScenePair(9, 5), new EnemyCardPair(Enemy.Eirin,  "錬丹「水銀の海」") },
                { new LevelScenePair(9, 6), new EnemyCardPair(Enemy.Kaguya, "新難題「金閣寺の一枚天井」") },
                { new LevelScenePair(9, 7), new EnemyCardPair(Enemy.Eirin,  "秘薬「仙香玉兎」") },
                { new LevelScenePair(9, 8), new EnemyCardPair(Enemy.Kaguya, "新難題「ミステリウム」") },

                { new LevelScenePair(10, 1), new EnemyCardPair(Enemy.Komachi,   "") },
                { new LevelScenePair(10, 2), new EnemyCardPair(Enemy.Shikieiki, "嘘言「タン・オブ・ウルフ」") },
                { new LevelScenePair(10, 3), new EnemyCardPair(Enemy.Komachi,   "死歌「八重霧の渡し」") },
                { new LevelScenePair(10, 4), new EnemyCardPair(Enemy.Shikieiki, "審判「十王裁判」") },
                { new LevelScenePair(10, 5), new EnemyCardPair(Enemy.Komachi,   "古雨「黄泉中有の旅の雨」") },
                { new LevelScenePair(10, 6), new EnemyCardPair(Enemy.Shikieiki, "審判「ギルティ・オワ・ノットギルティ」") },
                { new LevelScenePair(10, 7), new EnemyCardPair(Enemy.Komachi,   "死価「プライス・オブ・ライフ」") },
                { new LevelScenePair(10, 8), new EnemyCardPair(Enemy.Shikieiki, "審判「浄頗梨審判 -射命丸文-」") },

                { new LevelScenePair(11, 1), new EnemyCardPair(Enemy.Flandre, "禁忌「フォービドゥンフルーツ」") },
                { new LevelScenePair(11, 2), new EnemyCardPair(Enemy.Flandre, "禁忌「禁じられた遊び」") },
                { new LevelScenePair(11, 3), new EnemyCardPair(Enemy.Yukari,  "境符「色と空の境界」") },
                { new LevelScenePair(11, 4), new EnemyCardPair(Enemy.Yukari,  "境符「波と粒の境界」") },
                { new LevelScenePair(11, 5), new EnemyCardPair(Enemy.Mokou,   "貴人「サンジェルマンの忠告」") },
                { new LevelScenePair(11, 6), new EnemyCardPair(Enemy.Mokou,   "蓬莱「瑞江浦嶋子と五色の瑞亀」") },
                { new LevelScenePair(11, 7), new EnemyCardPair(Enemy.Suika,   "鬼気「濛々迷霧」") },
                { new LevelScenePair(11, 8), new EnemyCardPair(Enemy.Suika,   "「百万鬼夜行」") }
            };

        private class AllScoreData
        {
            public Header header;
            public List<Score> scores;
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
                this.Signature = Encoding.Default.GetString(reader.ReadBytes(4));
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
            public int Size { get; private set; }
            public uint Checksum { get; private set; }

            public Chapter() { }
            public Chapter(Chapter ch)
            {
                this.Signature = ch.Signature;
                this.Unknown = ch.Unknown;
                this.Size = ch.Size;
                this.Checksum = ch.Checksum;
            }

            public virtual void ReadFrom(BinaryReader reader)
            {
                this.Signature = Encoding.Default.GetString(reader.ReadBytes(2));
                this.Unknown = reader.ReadUInt16();
                this.Size = reader.ReadInt32();
                this.Checksum = reader.ReadUInt32();
            }
        }

        private class Score : Chapter   // per scene
        {
            public LevelScenePair LevelScene { get; private set; }
            public int HighScore { get; private set; }
            public uint Unknown1 { get; private set; }      // always 0x00000000?
            public int BestshotScore { get; private set; }
            public byte[] Unknown2 { get; private set; }    // .Length = 0x20
            public uint DateTime { get; private set; }      // UNIX time (unit: [s])
            public uint Unknown3 { get; private set; }      // checksum of the bestshot file?
            public int TrialCount { get; private set; }
            public float SlowRate1 { get; private set; }    // ??
            public float SlowRate2 { get; private set; }    // ??
            public byte[] Unknown4 { get; private set; }    // .Length = 0x10

            public Score(Chapter ch)
                : base(ch)
            {
                if (this.Signature != "SC")
                    throw new InvalidDataException("Signature");
                if (this.Unknown != 0x0001)
                    throw new InvalidDataException("Unknown");
                if (this.Size != 0x00000060)
                    throw new InvalidDataException("Size");
            }

            public override void ReadFrom(BinaryReader reader)
            {
                var number = reader.ReadInt32();
                this.LevelScene = new LevelScenePair((number / 10) + 1, (number % 10) + 1);
                this.HighScore = reader.ReadInt32();
                this.Unknown1 = reader.ReadUInt32();
                this.BestshotScore = reader.ReadInt32();
                this.Unknown2 = reader.ReadBytes(0x20);
                this.DateTime = reader.ReadUInt32();
                this.Unknown3 = reader.ReadUInt32();
                this.TrialCount = reader.ReadInt32();
                this.SlowRate1 = reader.ReadSingle();
                this.SlowRate2 = reader.ReadSingle();
                this.Unknown4 = reader.ReadBytes(0x10);
            }
        }

        private class Status : Chapter
        {
            public byte[] LastName { get; private set; }    // .Length = 10 (The last 2 bytes are always 0x00 ?)
            public byte[] Unknown1 { get; private set; }    // .Length = 0x0442

            public Status(Chapter ch)
                : base(ch)
            {
                if (this.Signature != "ST")
                    throw new InvalidDataException("Signature");
                if (this.Unknown != 0x0000)
                    throw new InvalidDataException("Unknown");
                if (this.Size != 0x00000458)
                    throw new InvalidDataException("Size");
            }

            public override void ReadFrom(BinaryReader reader)
            {
                this.LastName = reader.ReadBytes(10);
                this.Unknown1 = reader.ReadBytes(0x0442);
            }
        }

        private class BestShotHeader : Utils.IBinaryReadable
        {
            public string Signature { get; private set; }   // "BSTS"
            public ushort Unknown1 { get; private set; }
            public short Level { get; private set; }        // 0-origin?
            public short Scene { get; private set; }        // 0-origin?
            public ushort Unknown2 { get; private set; }    // 0x0102 ... Version?
            public short Width { get; private set; }
            public short Height { get; private set; }
            public int Score { get; private set; }
            public float SlowRate { get; private set; }
            public byte[] CardName { get; private set; }    // .Length = 0x50

            public void ReadFrom(BinaryReader reader)
            {
                this.Signature = new string(reader.ReadChars(4));
                if (this.Signature == "BSTS")
                {
                    this.Unknown1 = reader.ReadUInt16();
                    this.Level = reader.ReadInt16();
                    this.Scene = reader.ReadInt16();
                    this.Unknown2 = reader.ReadUInt16();
                    this.Width = reader.ReadInt16();
                    this.Height = reader.ReadInt16();
                    this.Score = reader.ReadInt32();
                    this.SlowRate = reader.ReadSingle();
                    this.CardName = reader.ReadBytes(0x50);
                }
            }
        }

        private AllScoreData allScoreData = null;
        private Dictionary<LevelScenePair, BestShotHeader> bestshotHeaders = null;

        public override string SupportedVersions
        {
            get
            {
                return "1.02a";
            }
        }

        public override bool HasBestShotConverter
        {
            get
            {
                return true;
            }
        }

        public Th095Converter() { }

        protected override bool ReadScoreFile(Stream input)
        {
            using (var decrypted = new MemoryStream())
#if DEBUG
            using (var decoded = new FileStream("th095decoded.dat", FileMode.Create, FileAccess.ReadWrite))
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

            if (header.Signature != "TH95")
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
                    var signature = reader.ReadUInt32();
                    reader.BaseStream.Seek(-4, SeekOrigin.Current);

                    chapter.ReadFrom(reader);
                    if (!((chapter.Signature == "SC") && (chapter.Unknown == 0x0001)) &&
                        !((chapter.Signature == "ST") && (chapter.Unknown == 0x0000)))
                        return false;

                    Int64 sum = signature + chapter.Size;
                    // 3 means Signature, Checksum, and Size.
                    for (var count = 3; count < chapter.Size / sizeof(uint); count++)
                        sum += reader.ReadUInt32();
                    if ((uint)sum != chapter.Checksum)
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

            allScoreData.scores = new List<Score>(SpellCards.Count);
            allScoreData.header = new Header();
            allScoreData.header.ReadFrom(reader);

            try
            {
                while (true)
                {
                    chapter.ReadFrom(reader);
                    switch (chapter.Signature)
                    {
                        case "SC":
                            var score = new Score(chapter);
                            score.ReadFrom(reader);
                            allScoreData.scores.Add(score);
                            break;
                        case "ST":
                            var status = new Status(chapter);
                            status.ReadFrom(reader);
                            allScoreData.status = status;
                            break;
                        default:
                            // 12 means the total size of Signature, Unknown, Size and Checksum.
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
                // (allScoreData.scores.Count >= 0) &&
                (allScoreData.status != null))
                return allScoreData;
            else
                return null;
        }

        protected override void Convert(Stream input, Stream output)
        {
            var reader = new StreamReader(input, Encoding.GetEncoding("shift_jis"));
            var writer = new StreamWriter(output, Encoding.GetEncoding("shift_jis"));

            var allLine = reader.ReadToEnd();
            allLine = this.ReplaceScore(allLine);
            allLine = this.ReplaceScoreTotal(allLine);
            allLine = this.ReplaceCard(allLine);
            allLine = this.ReplaceShot(allLine);
            allLine = this.ReplaceShotEx(allLine);
            writer.Write(allLine);

            writer.Flush();
            writer.BaseStream.SetLength(writer.BaseStream.Position);
        }

        // %T95SCR[x][y][z]
        private string ReplaceScore(string input)
        {
            return new Regex(@"%T95SCR([\dX])([1-9])([1-4])", RegexOptions.IgnoreCase)
                .Replace(input, new MatchEvaluator(match =>
                {
                    try
                    {
                        var level = match.Groups[1].Value.ToUpper();
                        var scene = int.Parse(match.Groups[2].Value);
                        var type = int.Parse(match.Groups[3].Value);

                        var levelIndex = Array.FindIndex<string>(
                            LevelShortArray, new Predicate<string>(elem => (elem == level)));
                        var levelScene = new LevelScenePair(levelIndex + 1, scene);
                        var score = this.allScoreData.scores.Find(new Predicate<Score>(
                            elem => ((elem != null) && elem.LevelScene.Equals(levelScene))));

                        switch (type)
                        {
                            case 1:     // high score
                                return this.ToNumberString((score != null) ? score.HighScore : 0);
                            case 2:     // bestshot score
                                return this.ToNumberString((score != null) ? score.BestshotScore : 0);
                            case 3:     // num of shots
                                return this.ToNumberString((score != null) ? score.TrialCount : 0);
                            case 4:     // slow rate
                                return (score != null) ? (score.SlowRate2.ToString("F3") + "%") : "-----%";
                            default:    // unreachable
                                return match.ToString();
                        }
                    }
                    catch
                    {
                        return match.ToString();
                    }
                }));
        }

        // %T95SCRTL[x]
        private string ReplaceScoreTotal(string input)
        {
            return new Regex(@"%T95SCRTL([1-4])", RegexOptions.IgnoreCase)
                .Replace(input, new MatchEvaluator(match =>
                {
                    try
                    {
                        var type = int.Parse(match.Groups[1].Value);

                        switch (type)
                        {
                            case 1:     // total score
                                return this.ToNumberString(Utils.Accumulate<Score>(
                                    this.allScoreData.scores, new Converter<Score, int>(
                                        score => ((score != null) ? score.HighScore : 0))));
                            case 2:     // total of bestshot scores
                                return this.ToNumberString(Utils.Accumulate<Score>(
                                    this.allScoreData.scores, new Converter<Score, int>(
                                        score => ((score != null) ? score.BestshotScore : 0))));
                            case 3:     // total of num of shots
                                return this.ToNumberString(Utils.Accumulate<Score>(
                                    this.allScoreData.scores, new Converter<Score, int>(
                                        score => ((score != null) ? score.TrialCount : 0))));
                            case 4:     // num of succeeded scenes
                                return Utils.CountIf<Score>(
                                    this.allScoreData.scores, new Predicate<Score>(
                                        score => ((score != null) && (score.HighScore > 0)))).ToString();
                            default:    // unreachable
                                return match.ToString();
                        }
                    }
                    catch
                    {
                        return match.ToString();
                    }
                }));
        }

        // %T95CARD[x][y][z]
        private string ReplaceCard(string input)
        {
            return new Regex(@"%T95CARD([\dX])([1-9])([12])", RegexOptions.IgnoreCase)
                .Replace(input, new MatchEvaluator(match =>
                {
                    try
                    {
                        var level = match.Groups[1].Value.ToUpper();
                        var scene = int.Parse(match.Groups[2].Value);
                        var type = int.Parse(match.Groups[3].Value);

                        var levelIndex = Array.FindIndex<string>(
                            LevelShortArray, new Predicate<string>(elem => (elem == level)));
                        var key = new LevelScenePair(levelIndex + 1, scene);
                        var score = this.allScoreData.scores.Find(
                            new Predicate<Score>(elem => ((elem != null) && elem.LevelScene.Equals(key))));

                        switch (type)
                        {
                            case 1:     // target Name
                                return (score != null) ? EnemyNames[SpellCards[key].Enemy] : "??????????";
                            case 2:     // spell card Name
                                return (score != null) ? SpellCards[key].Card : "??????????";
                            default:    // unreachable
                                return match.ToString();
                        }
                    }
                    catch
                    {
                        return match.ToString();
                    }
                }));
        }

        // %T95SHOT[x][y]
        private string ReplaceShot(string input)
        {
            return new Regex(@"%T95SHOT([\dX])([1-9])", RegexOptions.IgnoreCase)
                .Replace(input, new MatchEvaluator(match =>
                {
                    try
                    {
                        var level = match.Groups[1].Value.ToUpper();
                        var scene = int.Parse(match.Groups[2].Value);

                        var levelIndex = Array.FindIndex<string>(
                            LevelShortArray, new Predicate<string>(elem => (elem == level)));
                        var key = new LevelScenePair(levelIndex + 1, scene);

                        if ((this.bestshotHeaders != null) && this.bestshotHeaders.ContainsKey(key))
                            return string.Format(
                                "<img src=\"./{0}/bs_{1}_{2}{3}\" alt=\"{4}\" title=\"{4}\" border=0>",
                                Properties.Resources.strBestShotDirectory,
                                LevelArray[levelIndex],
                                scene,
                                Properties.Resources.strBestShotExtension,
                                string.Format("ClearData: {0}\nSlow: {1:F6}%\nSpellName: {2}",
                                    this.ToNumberString(bestshotHeaders[key].Score),
                                    bestshotHeaders[key].SlowRate,
                                    Encoding.Default.GetString(bestshotHeaders[key].CardName).TrimEnd('\0')));
                        else
                            return "";
                    }
                    catch
                    {
                        return "";
                    }
                }));
        }

        // %T95SHOTEX[x][y]
        private string ReplaceShotEx(string input)
        {
            return new Regex(@"%T95SHOTEX([\dX])([1-9])([1-6])", RegexOptions.IgnoreCase)
                .Replace(input, new MatchEvaluator(match =>
                {
                    try
                    {
                        var level = match.Groups[1].Value.ToUpper();
                        var scene = int.Parse(match.Groups[2].Value);
                        var type = int.Parse(match.Groups[3].Value);

                        var levelIndex = Array.FindIndex<string>(
                            LevelShortArray, new Predicate<string>(elem => (elem == level)));
                        var key = new LevelScenePair(levelIndex + 1, scene);

                        if ((this.bestshotHeaders != null) && this.bestshotHeaders.ContainsKey(key))
                            switch (type)
                            {
                                case 1:     // relative path to the bestshot file
                                    return string.Format(
                                        "./{0}/bs_{1}_{2}{3}",
                                        Properties.Resources.strBestShotDirectory,
                                        LevelArray[levelIndex],
                                        scene,
                                        Properties.Resources.strBestShotExtension);
                                case 2:     // width
                                    return this.bestshotHeaders[key].Width.ToString();
                                case 3:     // height
                                    return this.bestshotHeaders[key].Height.ToString();
                                case 4:     // score
                                    return this.ToNumberString(this.bestshotHeaders[key].Score);
                                case 5:     // slow rate
                                    return this.bestshotHeaders[key].SlowRate.ToString("F6") + "%";
                                case 6:     // date & time
                                    var score = this.allScoreData.scores.Find(new Predicate<Score>(
                                        elem => ((elem != null) && elem.LevelScene.Equals(key))));
                                    if (score != null)
                                        return new DateTime(1970, 1, 1).AddSeconds(score.DateTime)
                                            .ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss");
                                    else
                                        return "----/--/-- --:--:--";
                                default:    // unreachable
                                    return match.ToString();
                            }
                        else
                            switch (type)
                            {
                                case 1: return "";
                                case 2: return "0";
                                case 3: return "0";
                                case 4: return "--------";
                                case 5: return "-----%";
                                case 6: return "----/--/-- --:--:--";
                                default: return match.ToString();
                            }
                    }
                    catch
                    {
                        return match.ToString();
                    }
                }));
        }

        protected override void ConvertBestShot(Stream input, Stream output)
        {
            using (var decoded = new MemoryStream())
            {
                var reader = new BinaryReader(input);
                var header = new BestShotHeader();
                header.ReadFrom(reader);

                var key = new LevelScenePair(header.Level, header.Scene);
                if (this.bestshotHeaders == null)
                    this.bestshotHeaders = new Dictionary<LevelScenePair, BestShotHeader>(SpellCards.Count);
                if (!this.bestshotHeaders.ContainsKey(key))
                    this.bestshotHeaders.Add(key, header);

                Lzss.Extract(input, decoded);

                decoded.Seek(0, SeekOrigin.Begin);
                var source = decoded.ToArray();
                var sourceStride = 3 * header.Width;    // "3" means 24bpp.
                var bitmap = new Bitmap(header.Width, header.Height, PixelFormat.Format24bppRgb);
                var bitmapData = bitmap.LockBits(
                    new Rectangle(0, 0, header.Width, header.Height),
                    ImageLockMode.WriteOnly, bitmap.PixelFormat);
                var destination = bitmapData.Scan0;
                for (var sourceIndex = 0; sourceIndex < source.Length; sourceIndex += sourceStride)
                {
                    Marshal.Copy(source, sourceIndex, destination, sourceStride);
                    destination = new IntPtr(destination.ToInt32() + bitmapData.Stride);
                }
                bitmap.UnlockBits(bitmapData);

                bitmap.Save(output, ImageFormat.Png);
                output.Flush();
                output.SetLength(output.Position);
            }
        }
    }
}
