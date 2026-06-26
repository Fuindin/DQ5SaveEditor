using System.Text;

namespace DQ5SaveEditor;

/// <summary>
/// Represents one character combining their active-slot data (EXP/HP/MP/items) with
/// their roster data (name/stats/level). These two records are stored separately in the
/// .sav file but are always paired by position.
/// </summary>
public class Character
{
    // ── Identity ─────────────────────────────────────────────────────────────
    public int SlotIndex { get; set; }        // 0-based index in both arrays
    public int RosterOffset { get; set; }     // byte offset of roster record in file
    public int ActiveOffset { get; set; }     // byte offset of active-slot record in file

    public byte Id { get; set; }              // Roster ID byte (01=Jack, 02=Pankraz…)
    public string Name { get; set; } = string.Empty;

    // ── Base stats (roster) ───────────────────────────────────────────────────
    public ushort PersonalityCode { get; set; }
    public byte Str { get; set; }
    public byte Res { get; set; }   // Resilience (Defence base)
    public byte Agl { get; set; }   // Agility
    public byte Wis { get; set; }
    public byte Lck { get; set; }
    public byte Level { get; set; }  // stored at roster+0x1A (was misidentified as Unk1)
    public byte Unk2 { get; set; }

    // ── Live state (active slot) ──────────────────────────────────────────────
    public uint Exp { get; set; }
    public ushort HpCur { get; set; }
    public ushort HpMax { get; set; }
    public ushort MpCur { get; set; }
    public ushort MpMax { get; set; }

    // ── Items (active slot, +0x2B, 12 × 2-byte slots) ────────────────────────
    // Each slot: (item_id, flag) where flag 0x80=equipped, 0x00=in personal bag
    public CharItem[] Items { get; set; } = new CharItem[12];

    // ── Raw preserved bytes ───────────────────────────────────────────────────
    public byte[] RosterTrailing { get; set; } = [];  // +0x1D..+0x43 of roster (not edited)
    public byte ActiveSlotByte29 { get; set; }        // +0x29 of active slot (unknown flag)
    public byte ActiveSlotByte2A { get; set; }        // +0x2A of active slot

    /// <summary>File offset of this character's live STR field in the .ml1 save state.
    /// -1 = not in active party / not found.</summary>
    public int LiveStatOffset { get; set; } = -1;

    // ── Recruited monster ─────────────────────────────────────────────────────
    // True for a monster companion found in the live party array but absent from
    // the human roster. Monsters have NO roster record, so they are edited via
    // the live struct only (never flushed to the .sav roster).
    public bool IsMonster { get; set; }
    public int  SpeciesId { get; set; } = -1;   // monster species id (live slot @-0x28)

    /// <summary>Display name of the monster's species, "" for humans.</summary>
    public string SpeciesName => IsMonster ? SaveData.MonsterName(SpeciesId) : string.Empty;

    // ── Extra copies of this character located by name search (save states) ──
    // A recruited monster is mirrored across several RAM structures. To make an
    // edit actually stick on F1 reload we must write every main-RAM copy.
    /// <summary>Name-anchored live struct offsets (anchor = NAME start). Holds EXP/HP/MP.</summary>
    public List<int> LiveOffsets { get; } = [];
    /// <summary>Additional roster-format record starts found by name (e.g. the main-RAM roster copy).</summary>
    public List<int> ExtraRosterOffsets { get; } = [];

    public bool IsEmpty => string.IsNullOrEmpty(Name);

    public override string ToString() => IsEmpty ? $"(empty slot {SlotIndex})" : Name;
}

public class CharItem
{
    public byte ItemId { get; set; }
    public byte Flag { get; set; }      // live format: bit0=1 → equipped
    public byte Qty { get; set; } = 1;  // live format quantity

    public bool IsEmpty => ItemId == 0x00 || ItemId == 0xFF || ItemId == 0xD5;
    public bool IsEquipped => (Flag & 0x01) != 0 || Flag == 0x80;

    public string ItemName => ItemNames.TryGetValue(ItemId, out var n) ? n : $"Unknown (0x{ItemId:X2})";

    // ── Item names extracted from the ROM's English name table ────────────────
    // Source: Dragon Quest V DS ROM, English item-name bank. The table is ordered
    // by item ID; offsets were calibrated against 21 live-save-state confirmations
    // (all matched exactly). Only ID ranges densely bracketed by confirmed anchors
    // are included, so every entry here is trustworthy:
    //   0x35-0x8B (weapons/armour/shields/helmets), 0xA1-0xAF (consumables/seeds),
    //   0xC2-0xD2 (tools/keys), plus 0x01 & 0xDA. Unmapped IDs (early weapons,
    //   accessories, gap regions) render as "Unknown (0xNN)".
    public static readonly Dictionary<byte, string> ItemNames = new()
    {
        [0x00] = "(empty)",
        [0x01] = "Cypress stick",
        // Early weapons (confirmed via live-save calibration, June 2026)
        [0x03] = "Stone axe",
        [0x05] = "Floral parasol",
        [0x07] = "Steel fangs",
        [0x0C] = "Steel broadsword",
        [0x0D] = "Pankraz's sword",
        [0x0E] = "Serpent sword",
        [0x0F] = "Cautery sword",
        [0x10] = "Dream blade",
        [0x1B] = "Zenithian Sword",
        [0x23] = "Staff of antimagic",
        [0x29] = "Giant mallet",
        [0x2A] = "Sledgehammer",
        [0x2D] = "Iron claw",
        [0x2E] = "Fire claw",
        // Weapons / armour / shields / helmets (0x35-0x8B)
        [0x35] = "Boomerang",
        [0x36] = "Edged boomerang",
        [0x37] = "Flametang boomerang",
        [0x38] = "Thorn whip",
        [0x39] = "Chain whip",
        [0x3A] = "Morning star",
        [0x3B] = "Spiked steel whip",
        [0x3C] = "Gringham whip",
        [0x3D] = "Flail of destruction",
        [0x3E] = "Bamboo spear",
        [0x3F] = "Ionospear",
        [0x40] = "Demon spear",
        [0x41] = "Great bow",
        [0x42] = "Restless armour",
        [0x43] = "Rags",
        [0x44] = "Plain clothes",
        [0x45] = "Serf wear",
        [0x46] = "Handwoven cape",
        [0x47] = "Wayfarer's clothes",
        [0x48] = "Silk apron",
        [0x49] = "Leather armour",
        [0x4A] = "Leather kilt",
        [0x4B] = "Silk robe",
        [0x4C] = "Scale armour",
        [0x4D] = "Boxer shorts",
        [0x4E] = "Leather dress",
        [0x4F] = "Fur cape",
        [0x50] = "Chain mail",
        [0x51] = "Dancer's costume",
        [0x52] = "Slime gooniform",
        [0x53] = "Bronze armour",
        [0x54] = "Iron cuirass",
        [0x55] = "Robust lingerie",
        [0x56] = "Iron armour",
        [0x57] = "Cloak of evasion",
        [0x58] = "Full plate armour",
        [0x59] = "Tortoise shell",
        [0x5A] = "Robe of serenity",
        [0x5B] = "Lacy bustier",
        [0x5C] = "Glombolero",
        [0x5D] = "Legerdemantle",
        [0x5E] = "Zombie mail",
        [0x5F] = "Silver cuirass",
        [0x60] = "Silver mail",
        [0x61] = "Powjamas",
        [0x62] = "Blood mail",
        [0x63] = "Shimmering dress",
        [0x64] = "Dragon mail",
        [0x65] = "Sage's robe",
        [0x66] = "Spiked armour",
        [0x67] = "Flowing dress",
        [0x68] = "Dark robe",
        [0x69] = "Magic armour",
        [0x6A] = "Silk bustier",
        [0x6B] = "Devil armour",
        [0x6C] = "Flame armour",
        [0x6D] = "Angel leotard",
        [0x6E] = "Sacred armour",
        [0x6F] = "Mirror armour",
        [0x70] = "Princess's robe",
        [0x71] = "Hela's armour",
        [0x72] = "Zenithian Armour",
        [0x73] = "Pallium Regale",
        [0x74] = "Metal king armour",
        [0x75] = "Ruinous shield",
        [0x76] = "Pot lid",
        [0x77] = "Leather shield",
        [0x78] = "Scale shield",
        [0x79] = "Bronze shield",
        [0x7A] = "Iron shield",
        [0x7B] = "Magic shield",
        [0x7C] = "Dragon shield",
        [0x7D] = "Tempest shield",
        [0x7E] = "Dark shield",
        [0x7F] = "Flame shield",
        [0x80] = "Power shield",
        [0x81] = "Ogre shield",
        [0x82] = "Silver shield",
        [0x83] = "Zenithian Shield",
        [0x84] = "Shimmering shield",
        [0x85] = "Metal king shield",
        [0x86] = "Leather hat",
        [0x87] = "Pointy hat",
        [0x88] = "Hardwood headwear",
        [0x89] = "Shellmet",
        [0x8A] = "Hairband",
        [0x8B] = "Fur hood",
        [0x8C] = "Silver tiara",
        [0x8D] = "Iron helmet",
        [0x8E] = "Top hat",
        [0x8F] = "Iron mask",
        // Accessories
        [0x92] = "Wedding veil",
        [0x9A] = "Bianca's ribbon",
        [0x9C] = "Kamikazee bracer",
        [0x9E] = "Circle of Fire",
        [0x9F] = "Circle of Water",
        // Consumables / seeds (0xA1-0xAF)
        [0xA1] = "Medicinal herb",
        [0xA2] = "Antidotal herb",
        [0xA3] = "Holy water",
        [0xA4] = "Chimaera wing",
        [0xA5] = "Yggdrasil leaf",
        [0xA6] = "Yggdrasil dew",
        [0xA7] = "Moonwort bulb",
        [0xA8] = "Prayer ring",
        [0xA9] = "Magic water",
        [0xAA] = "Musk",
        [0xAB] = "Sage's stone",
        [0xAC] = "Seed of strength",
        [0xAD] = "Seed of resilience",
        [0xAE] = "Seed of agility",
        [0xAF] = "Seed of wisdom",
        // Special / misc (confirmed via calibration)
        [0xB2] = "T 'n' T ticket",
        [0xB3] = "Tombola ticket",
        [0xB4] = "Night light",
        [0xB5] = "Monster munchies",
        [0xB6] = "Rockbomb shard",
        [0xB7] = "Paxa Punch",
        [0xB9] = "Elfin elixir",
        [0xBB] = "Dieamend",
        // Tools / keys / quest
        [0xC1] = "Ra's mirror",
        [0xC2] = "Torch",
        [0xC3] = "Herald of Spring",
        [0xC4] = "Lunar Zoombloom",
        [0xC5] = "Royal Insignia",
        [0xC6] = "Faerie horn",
        [0xC7] = "Grappling hook",
        [0xC8] = "Aspersorium",
        [0xC9] = "Dragon's right eye",
        [0xCA] = "Dragon's left eye",
        [0xCB] = "Gold orb",
        [0xCC] = "Gold bauble",
        [0xCD] = "Dragon orb",
        [0xCE] = "Key to Coburg",
        [0xCF] = "Magic key",
        [0xD0] = "Ultimate key",
        [0xD1] = "Mini medal",
        [0xD2] = "Adventurer's map",
        [0xD3] = "Pankraz's letter",
        [0xD7] = "Zizzwizz Pillow",
        [0xDA] = "Silver tea tray",
        [0xE1] = "Crude image",
        [0xE6] = "Honey buns",
        [0xEB] = "Chocolate medalliyum",
        [0xEC] = "Maxi medal",
        [0xFB] = "Chamois",
        [0xFE] = "Big Book of Beasts",
    };
}

public class BagItem
{
    public byte ItemId { get; set; }
    public byte Quantity { get; set; }

    public bool IsEmpty => ItemId == 0xFF || ItemId == 0xD5 || ItemId == 0x00;
    public string ItemName => CharItem.ItemNames.TryGetValue(ItemId, out var n) ? n : $"Unknown (0x{ItemId:X2})";
}

public class SaveData
{
    // ── File-layout constants ─────────────────────────────────────────────────
    private const int GoldOffset = 0x414;

    // Roster: one 0x44-byte record per character, first entry = Jack (hero)
    private const int RosterStart = 0x857;
    private const int CharStride = 0x44;
    // Humans occupy the first ~17 slots; the monster den (up to 80 slots) follows
    // in the same array, then padding/other data. Scan the whole array; the parse
    // loop skips padding and stops when it reaches non-record data.
    private const int MaxCharSlots = 100;

    // Active slots: same stride, grow DOWNWARD from (RosterStart - CharStride)
    // Slot 0 (hero) active = RosterStart - CharStride = 0x813
    private const int ActiveSlotBase = RosterStart - CharStride;   // 0x813

    // Character record field offsets (within each 0x44-byte block)
    private const int Off_Name = 0x01;       // 18 bytes, null-term, D5-padded
    private const int Off_Personality = 0x13; // uint16 LE class/personality code
    private const int Off_Str = 0x15;
    private const int Off_Res = 0x16;        // Resilience (Defence base)
    private const int Off_Agl = 0x17;        // Agility
    private const int Off_Wis = 0x18;
    private const int Off_Lck = 0x19;
    private const int Off_Level = 0x1A;      // uint8 actual level
    private const int Off_Unk2 = 0x1B;
    private const int Off_Sentinel = 0x1C;   // always 0xD5
    private const int Off_Exp = 0x1D;        // uint32 LE — READ FROM ACTIVE SLOT
    private const int Off_HpCur = 0x21;      // uint16 LE — READ FROM ACTIVE SLOT
    private const int Off_HpMax = 0x23;
    private const int Off_MpCur = 0x25;
    private const int Off_MpMax = 0x27;
    private const int Off_Byte29 = 0x29;     // unknown flag byte in active slot
    private const int Off_Byte2A = 0x2A;
    private const int Off_Items = 0x2B;      // 12 × (item_id, flag) pairs in active slot
    private const int ItemSlots = 12;

    private const int Off_RosterTrailing = 0x1D;  // preserve +0x1D..+0x43 of roster unchanged
    private const int RosterTrailingLen = CharStride - Off_RosterTrailing;

    // Party bag: standalone 24-slot array of (item_id, qty) pairs
    private const int BagOffset = 0x78C;
    public const int BagItemSlots = 24;

    // ─── Save-state (.ml1) base offset ──────────────────────────────────────
    // In a melonDS save state the save data lives at file offset 0x01028FAE.
    // For a plain .sav file the base is 0. Everything else is identical.
    private const int Ml1SearchPatternOffset = 0x858; // name starts here relative to roster
    private int _base;  // 0 for .sav  |  0x01028FAE for .ml1
    public bool IsSaveState { get; private set; }

    // Translate a .sav-relative offset to an absolute file offset.
    private int FO(int savOffset) => _base + savOffset;

    // ── State ─────────────────────────────────────────────────────────────────
    private byte[] _raw;

    public uint Gold
    {
        get => IsSaveState
            ? BitConverter.ToUInt32(_raw, ML1_GOLD_OFFSET)
            : BitConverter.ToUInt32(_raw, FO(GoldOffset));
        set
        {
            if (IsSaveState)
            {
                BitConverter.GetBytes(value).CopyTo(_raw, ML1_GOLD_OFFSET);
            }
            else
            {
                BitConverter.GetBytes(value).CopyTo(_raw, FO(GoldOffset));
            }
        }
    }

    /// <summary>True if the save state hero live data was found and can be edited.</summary>
    public bool HasLiveHeroData => _heroLiveOffsets.Count > 0;

    /// <summary>
    /// Read any party character's live stats from their confirmed live offset.
    /// </summary>
    public void ReadLiveStats(Character ch, int liveOff)
    {
        if (liveOff < 0)
        {
            return;
        }

        ch.Exp   = BitConverter.ToUInt32(_raw, liveOff + SS_Exp);
        ch.Level = _raw[liveOff + SS_Level];
        ch.Str   = (byte)BitConverter.ToUInt16(_raw, liveOff + SS_Str);
        ch.Res   = (byte)BitConverter.ToUInt16(_raw, liveOff + SS_Res);
        ch.HpCur = BitConverter.ToUInt16(_raw, liveOff + SS_HpCur);
        ch.HpMax = BitConverter.ToUInt16(_raw, liveOff + SS_HpMax);
        ch.MpCur = BitConverter.ToUInt16(_raw, liveOff + SS_MpCur);
        ch.MpMax = BitConverter.ToUInt16(_raw, liveOff + SS_MpMax);
        ch.Agl   = _raw[liveOff + SS_Agl];
        ch.Wis   = _raw[liveOff + SS_Wis];
        ch.Lck   = _raw[liveOff + SS_Lck];

        // Live items: 12 slots of (u16 id, u8 qty, u8 flag) at STR + 0x1C
        for (int s = 0; s < SS_ItemSlots && s < ch.Items.Length; s++)
        {
            int p = liveOff + SS_Items + s * 4;
            if (p + 4 > _raw.Length)
            {
                break;
            }

            ch.Items[s] = new CharItem
            {
                ItemId = (byte)(BitConverter.ToUInt16(_raw, p) & 0xFF),
                Qty    = _raw[p + 2],
                Flag   = _raw[p + 3],
            };
        }
    }

    public void ReadHeroLiveData(Character hero) => ReadLiveStats(hero, _liveStatOffset);

    /// <summary>Read the party bag from its live location (save states only).</summary>
    public void ReadLiveBag()
    {
        for (int i = 0; i < ML1_BAG_SLOTS && i < BagItems.Length; i++)
        {
            int p = ML1_BAG_OFFSET + i * 4;
            if (p + 4 > _raw.Length)
            {
                break;
            }

            BagItems[i] = new BagItem
            {
                ItemId   = (byte)(BitConverter.ToUInt16(_raw, p) & 0xFF),
                Quantity = _raw[p + 2],
            };
        }
    }

    /// <summary>Write a character's stats to their live offset in the save state.</summary>
    public void FlushHeroLiveData(Character ch)
    {
        int liveOff = ch.LiveStatOffset >= 0 ? ch.LiveStatOffset : _liveStatOffset;
        if (liveOff < 0)
        {
            return;
        }

        BitConverter.GetBytes(ch.Exp).CopyTo(_raw, liveOff + SS_Exp);
        _raw[liveOff + SS_Level] = ch.Level;
        BitConverter.GetBytes((ushort)ch.Str).CopyTo(_raw, liveOff + SS_Str);
        BitConverter.GetBytes((ushort)ch.Res).CopyTo(_raw, liveOff + SS_Res);
        BitConverter.GetBytes(ch.HpCur).CopyTo(_raw, liveOff + SS_HpCur);
        BitConverter.GetBytes(ch.HpMax).CopyTo(_raw, liveOff + SS_HpMax);
        BitConverter.GetBytes(ch.MpCur).CopyTo(_raw, liveOff + SS_MpCur);
        BitConverter.GetBytes(ch.MpMax).CopyTo(_raw, liveOff + SS_MpMax);
        _raw[liveOff + SS_Agl] = ch.Agl;
        _raw[liveOff + SS_Wis] = ch.Wis;
        _raw[liveOff + SS_Lck] = ch.Lck;

        // Items are read-only for now — intentionally NOT written, so stacked
        // quantities and equipment state are never disturbed by a stat edit.
    }

    public List<Character> Characters { get; } = [];
    public BagItem[] BagItems { get; } = new BagItem[ML1_BAG_SLOTS];

    /// <summary>Number of bag slots to display for the loaded file's format.</summary>
    public int BagSlotCount => IsSaveState ? ML1_BAG_SLOTS : BagItemSlots;

    private SaveData(byte[] raw, int fileBase, bool isSaveState)
    {
        _raw = raw;
        _base = fileBase;
        IsSaveState = isSaveState;
    }

    // ── Load .sav ─────────────────────────────────────────────────────────────
    public static SaveData Load(string path)
    {
        var raw = File.ReadAllBytes(path);
        if (raw.Length < 0x4000)
        {
            throw new InvalidDataException("File too small — not a valid DQ5 DS save.");
        }

        return LoadCommon(raw, 0, false);
    }

    // ── Load .ml1 (melonDS save state) ────────────────────────────────────────
    // Confirmed offsets for DQ5 DS North American version:
    //   DS main RAM: file offset 0x24
    //   Gold DS addr: 0x0209D7FC → file 0x0009D820
    //   Hero live struct: found by name search in main RAM
    //     name+0x24 EXP(u32), name+0x28 STR(u16), name+0x2A RES(u16),
    //     name+0x2C HP_cur(u16), name+0x2E HP_max(u16),
    //     name+0x30 MP_cur(u16), name+0x32 MP_max(u16),
    //     name+0x34 AGL(u8), name+0x35 WIS(u8), name+0x36 LCK(u8), name+0x37 Level(u8)
    //     name+0x44 Items: (u16 itemId, u8 qty, u8 flag) × 12
    private const int ML1_MAIN_RAM_START = 0x24;
    private const int ML1_GOLD_OFFSET    = 0x0009D820;   // file offset, confirmed

    // ── Confirmed live character stat offsets (from STR field start = anchor) ──
    // All offsets relative to _liveStatOffset which points at the STR field.
    // Verified empirically for DQ5 DS North American version.
    private const int SS_Exp    = -0x20;  // uint32 — EXP
    private const int SS_Level  = -0x15;  // uint8  — Level (3rd byte of 4-byte group at -0x18)
    private const int SS_Str    =  0x00;  // uint16 — STR (ANCHOR)
    private const int SS_Res    =  0x02;  // uint16
    private const int SS_HpCur  =  0x04;  // uint16
    private const int SS_HpMax  =  0x06;  // uint16
    private const int SS_MpCur  =  0x08;  // uint16
    private const int SS_MpMax  =  0x0A;  // uint16
    private const int SS_Agl    =  0x0C;  // uint8
    private const int SS_Wis    =  0x0D;  // uint8
    private const int SS_Lck    =  0x0E;  // uint8
    // Identity id stored ahead of the STR anchor: for humans this equals their
    // roster id (Jack=0x01, Harry=0x07); for a recruited monster it is the
    // monster's species id (confirmed 0x28 for the user's Lv2 companion).
    private const int SS_SpeciesId = -0x28;  // uint16

    // Items in live format: 12 × (u16 itemId, u8 qty, u8 flag) = 48 bytes.
    // Confirmed: item list begins at STR + 0x1C (pointer at STR+0x14 = STR_ds+0x1C,
    // count byte 0x0C at STR+0x18). flag bit0 = equipped.
    private const int SS_Items     = 0x1C;  // offset from STR anchor
    private const int SS_ItemSlots = 12;

    // Party bag: fixed live location (gold is at 0x0009D820, bag at gold+0x20).
    private const int ML1_BAG_OFFSET = 0x0009D840;  // (u16 id, u8 qty, u8 flag) × N
    // The live party bag holds far more than the old .sav-era 24 (the in-game list
    // pages through ~6×6). Read generously; the slots past the contents are zero
    // padding (the region runs clear up to the party array at 0x9DD9C) and the grid
    // simply skips empty slots.
    private const int ML1_BAG_SLOTS  = 80;

    // ── Name-anchored live struct (recruited monsters) ───────────────────────
    // Every recruited monster (wagon AND stored at the monster park) keeps a live
    // record in main RAM whose fields are offset from the NAME start. This is the
    // only copy that holds a stored monster's real EXP/HP/MP. Distinct from the
    // STR-anchored battle array (SS_* above): here the anchor is the name.
    //   name+0x24 EXP(u32), name+0x28 STR(u16), name+0x2A RES(u16),
    //   name+0x2C HPcur, name+0x2E HPmax, name+0x30 MPcur, name+0x32 MPmax,
    //   name+0x34 AGL(u8), name+0x35 WIS(u8), name+0x36 LCK(u8), name+0x37 Level(u8)
    private const int NM_Exp   = 0x24;
    private const int NM_Str   = 0x28;
    private const int NM_Res   = 0x2A;
    private const int NM_HpCur = 0x2C;
    private const int NM_HpMax = 0x2E;
    private const int NM_MpCur = 0x30;
    private const int NM_MpMax = 0x32;
    private const int NM_Agl   = 0x34;
    private const int NM_Wis   = 0x35;
    private const int NM_Lck   = 0x36;
    private const int NM_Level = 0x37;

    // Main RAM occupies file offsets [ML1_MAIN_RAM_START, MainRamEnd). Only edits
    // inside this window take effect when the state is reloaded with F1 (the SRAM
    // image copy that sits beyond it is only re-read on a cold "Continue").
    private const int MainRamEnd = ML1_MAIN_RAM_START + 0x400000;

    // Tracks the file offset of the STR field in the live character data
    private int _liveStatOffset = -1;

    // Tracks every file offset where the hero live struct was found (game keeps duplicates)
    private readonly List<int> _heroLiveOffsets = [];

    public static SaveData LoadSaveState(string path)
    {
        var raw = File.ReadAllBytes(path);
        if (raw.Length < 16 || raw[0] != 'M' || raw[1] != 'E' || raw[2] != 'L' || raw[3] != 'N')
        {
            throw new InvalidDataException("Not a melonDS save state (.ml1) file.");
        }

        // .sav base is not used for live edits in save states, but we still parse the
        // .sav buffer (which sits inside main RAM) for character names and party bag.
        // Find Jack's name with D5 padding to locate the .sav buffer base.
        byte[] d5Pattern = [0x4A, 0x61, 0x63, 0x6B, 0x00,
            0xD5, 0xD5, 0xD5, 0xD5, 0xD5, 0xD5, 0xD5, 0xD5, 0xD5];

        int savBufBase = 0;
        for (int i = 0; i < raw.Length - d5Pattern.Length; i++)
        {
            bool ok = true;
            for (int j = 0; j < d5Pattern.Length; j++)
            {
                if (raw[i + j] != d5Pattern[j]) 
                { 
                    ok = false; break; 
                }
            }

            if (ok) 
            { 
                savBufBase = (i - 1) - RosterStart; break; 
            }
        }

        var save = new SaveData(raw, savBufBase, true);
        save.ParseCharacters();
        save.ParseBagItems();
        save.FindHeroLiveOffsets();    // locate live battle-array data in main RAM
        save.FindMonsterLiveData();    // locate every recruited monster's live copies

        return save;
    }

    private void FindHeroLiveOffsets()
    {
        // ── Party live data array (DQ5 DS North American version) ────────────
        // DS address 0x0209DD78 = party slot 0 (hero) STR field.
        // Stride = 0x51C bytes between party slots. Up to 6 active slots.
        const int partyBase = ML1_MAIN_RAM_START + 0x09DD78;  // 0x0009DD9C
        const int stride    = 0x51C;
        const int maxSlots  = 8;             // wagon holds up to 8 active members
        const int fixedOffset = partyBase;  // slot 0 = hero

        // Verify the offset is plausible (STR in 0–255)
        // ── Slot 0 = hero (hardcoded, always valid) ───────────────────────
        if (fixedOffset + 2 <= _raw.Length)
        {
            _liveStatOffset = fixedOffset;
            _heroLiveOffsets.Add(fixedOffset);

            if (Characters.Count > 0)
            {
                Characters[0].LiveStatOffset = fixedOffset;
            }
        }

        // ── Slots 1..maxSlots-1: match occupied party slots to roster chars ──
        // Each slot stores the character's species id at SS_SpeciesId (= the roster
        // Id byte, unique per roster entry), so that is the authoritative key. STR+AGL
        // is only a fallback. Matching on stats alone was unsafe: two members could
        // share a stat signature, causing an edit to be written into the wrong slot.
        for (int slot = 1; slot < maxSlots; slot++)
        {
            int slotOff = partyBase + slot * stride;
            if (slotOff + 0x10 >= _raw.Length)
            {
                break;
            }

            ushort slotStr     = BitConverter.ToUInt16(_raw, slotOff + SS_Str);
            ushort slotHp      = BitConverter.ToUInt16(_raw, slotOff + SS_HpMax);
            byte   slotAgl     = _raw[slotOff + SS_Agl];
            ushort slotSpecies = BitConverter.ToUInt16(_raw, slotOff + SS_SpeciesId);

            if (slotStr == 0 && slotAgl == 0 && slotHp == 0)
            {
                continue;  // empty slot
            }

            // Primary: species/id AND stat signature both agree (safest — also
            // disambiguates two members that share a species, e.g. two Slimes).
            Character? match = Find(c => c.Id == slotSpecies && c.Str == slotStr && c.Agl == slotAgl)
                            ?? Find(c => c.Id == slotSpecies)          // id only
                            ?? Find(c => c.Str == slotStr && c.Agl == slotAgl);  // legacy stat-only

            if (match != null)
            {
                match.LiveStatOffset = slotOff;
            }

            Character? Find(Func<Character, bool> pred)
            {
                foreach (var ch in Characters)
                {
                    if (ch.LiveStatOffset < 0 && pred(ch))
                    {
                        return ch;
                    }
                }
                return null;
            }
        }
    }

    /// <summary>
    /// Locate every recruited monster's main-RAM copies by searching for its name.
    /// A monster is mirrored across a name-anchored live struct (holds the real
    /// EXP/HP/MP, even for monsters stored at the park) and one or more roster-format
    /// copies. Records the offsets so edits can be written to all of them, and reads
    /// the authoritative live stats for display.
    /// </summary>
    private void FindMonsterLiveData()
    {
        int scanEnd = Math.Min(MainRamEnd, _raw.Length);

        foreach (var ch in Characters)
        {
            if (!ch.IsMonster || ch.IsEmpty)
            {
                continue;
            }

            byte[] name = Encoding.ASCII.GetBytes(ch.Name);
            if (name.Length == 0)
            {
                continue;
            }

            for (int i = 0; i <= scanEnd - name.Length; i++)
            {
                if (_raw[i] != name[0])
                {
                    continue;
                }

                bool match = true;
                for (int j = 1; j < name.Length; j++)
                {
                    if (_raw[i + j] != name[j]) { match = false; break; }
                }
                if (!match)
                {
                    continue;
                }

                // Require a name terminator so we don't match a longer string.
                byte term = i + name.Length < _raw.Length ? _raw[i + name.Length] : (byte)0;
                if (term != 0x00 && term != 0xD5)
                {
                    continue;
                }

                // Classify the copy by its surrounding fields (validated against the
                // roster values, which equal the live values on a consistent save).
                if (IsLiveNameStruct(i, ch))
                {
                    ch.LiveOffsets.Add(i);
                }
                else if (IsRosterCopy(i, ch))
                {
                    ch.ExtraRosterOffsets.Add(i - Off_Name);   // record start
                }
            }

            // Load the authoritative live stats (EXP/HP/MP) for display.
            if (ch.LiveOffsets.Count > 0)
            {
                ReadLiveNameAnchored(ch, ch.LiveOffsets[0]);
            }
        }
    }

    private bool IsLiveNameStruct(int nameOff, Character ch)
    {
        if (nameOff + NM_Level >= _raw.Length)
        {
            return false;
        }

        // Validate several independent fields so a shared name (the player can rename
        // monsters) can never bind one monster's edit onto another's live struct.
        return BitConverter.ToUInt16(_raw, nameOff + NM_Str) == ch.Str
            && BitConverter.ToUInt16(_raw, nameOff + NM_Res) == ch.Res
            && _raw[nameOff + NM_Agl]   == ch.Agl
            && _raw[nameOff + NM_Level] == ch.Level;
    }

    private bool IsRosterCopy(int nameOff, Character ch)
    {
        int rec = nameOff - Off_Name;
        if (rec < 0 || rec + CharStride > _raw.Length)
        {
            return false;
        }

        return _raw[rec] == (byte)ch.SpeciesId
            && _raw[rec + Off_Str] == ch.Str
            && _raw[rec + Off_Level] == ch.Level;
    }

    /// <summary>Read a monster's live stats from a name-anchored struct.</summary>
    private void ReadLiveNameAnchored(Character ch, int nameOff)
    {
        ch.Exp   = BitConverter.ToUInt32(_raw, nameOff + NM_Exp);
        ch.Str   = (byte)BitConverter.ToUInt16(_raw, nameOff + NM_Str);
        ch.Res   = (byte)BitConverter.ToUInt16(_raw, nameOff + NM_Res);
        ch.HpCur = BitConverter.ToUInt16(_raw, nameOff + NM_HpCur);
        ch.HpMax = BitConverter.ToUInt16(_raw, nameOff + NM_HpMax);
        ch.MpCur = BitConverter.ToUInt16(_raw, nameOff + NM_MpCur);
        ch.MpMax = BitConverter.ToUInt16(_raw, nameOff + NM_MpMax);
        ch.Agl   = _raw[nameOff + NM_Agl];
        ch.Wis   = _raw[nameOff + NM_Wis];
        ch.Lck   = _raw[nameOff + NM_Lck];
        ch.Level = _raw[nameOff + NM_Level];
    }

    /// <summary>Write a monster's live stats to a name-anchored struct (name preserved).</summary>
    private void FlushLiveNameAnchored(Character ch, int nameOff)
    {
        if (nameOff < 0 || nameOff + NM_Level >= _raw.Length)
        {
            return;
        }

        BitConverter.GetBytes(ch.Exp).CopyTo(_raw, nameOff + NM_Exp);
        BitConverter.GetBytes((ushort)ch.Str).CopyTo(_raw, nameOff + NM_Str);
        BitConverter.GetBytes((ushort)ch.Res).CopyTo(_raw, nameOff + NM_Res);
        BitConverter.GetBytes(ch.HpCur).CopyTo(_raw, nameOff + NM_HpCur);
        BitConverter.GetBytes(ch.HpMax).CopyTo(_raw, nameOff + NM_HpMax);
        BitConverter.GetBytes(ch.MpCur).CopyTo(_raw, nameOff + NM_MpCur);
        BitConverter.GetBytes(ch.MpMax).CopyTo(_raw, nameOff + NM_MpMax);
        _raw[nameOff + NM_Agl]   = ch.Agl;
        _raw[nameOff + NM_Wis]   = ch.Wis;
        _raw[nameOff + NM_Lck]   = ch.Lck;
        _raw[nameOff + NM_Level] = ch.Level;
    }

    /// <summary>Write only the base attributes into a roster-format record (name/EXP/HP/MP untouched).</summary>
    private void FlushRosterStats(Character ch, int recOff)
    {
        if (recOff < 0 || recOff + Off_Level >= _raw.Length)
        {
            return;
        }

        _raw[recOff + Off_Str]   = ch.Str;
        _raw[recOff + Off_Res]   = ch.Res;
        _raw[recOff + Off_Agl]   = ch.Agl;
        _raw[recOff + Off_Wis]   = ch.Wis;
        _raw[recOff + Off_Lck]   = ch.Lck;
        _raw[recOff + Off_Level] = ch.Level;
    }

    /// <summary>
    /// Display name for a recruited monster. DQ5 generates default monster names
    /// from a ROM species table (not stored in the save unless renamed), so until
    /// that table is mapped we label by species id. Confirmed entries go here.
    /// </summary>
    public static string MonsterName(int species) =>
        MonsterNames.TryGetValue(species, out var n) ? n : $"Monster (sp.{species})";

    private static readonly Dictionary<int, string> MonsterNames = new()
    {
        // Species id → species name. Confirmed against in-game status screens.
        [0x16] = "Slime",            // Gootrude
        [0x19] = "Dracky",           // Vlad
        [0x28] = "Brownie",          // Whacka (40)
        [0x2B] = "Ticking timebomb", // Bomburr
        [0xC4] = "Rotten apple",     // Adams
        [0xC5] = "Rotten apple",     // Johnny (confirmed — same species name as 0xC4)
        [0xD0] = "Funghoul",         // Champ
    };

    private static SaveData LoadCommon(byte[] raw, int fileBase, bool isSaveState)
    {
        var save = new SaveData(raw, fileBase, isSaveState);
        save.ParseCharacters();
        save.ParseBagItems();

        return save;
    }

    private void ParseCharacters()
    {
        // One contiguous 0x44 array: humans (≈17), then the monster den (up to 80
        // slots, the storage Monty manages), then 0xD5 padding for empty den slots,
        // then unrelated data. Releasing a monster can leave a 0xD5 gap mid-den, so
        // skip padding rather than stopping at it; only stop when the array gives
        // way to data that is neither a record nor padding.
        for (int i = 0; i < MaxCharSlots; i++)
        {
            int rosterOff = FO(RosterStart + i * CharStride);

            if (rosterOff + CharStride > _raw.Length)
            {
                break;
            }

            // 0xD5 is the universal padding byte → empty den slot; keep scanning.
            if (_raw[rosterOff] == 0xD5)
            {
                continue;
            }

            // A real record begins with a printable name byte. Anything else means
            // we have run past the den into other save data.
            byte firstNameByte = _raw[rosterOff + Off_Name];
            if (firstNameByte < 0x20 || firstNameByte > 0x7E)
            {
                break;
            }

            int activeOff = (i == 0) ? FO(ActiveSlotBase) : -1;

            var ch = ReadCharacter(rosterOff, activeOff, i);
            if (!ch.IsEmpty)
            {
                Characters.Add(ch);
            }
        }
    }

    private Character ReadCharacter(int rOff, int aOff, int slot)
    {
        var ch = new Character
        {
            SlotIndex = slot,
            RosterOffset = rOff,
            ActiveOffset = aOff,
            Id = _raw[rOff],
        };

        ch.Name = ReadName(rOff + Off_Name, 18);
        ch.PersonalityCode = BitConverter.ToUInt16(_raw, rOff + Off_Personality);

        // Recruited monsters share the human roster but their personality code has
        // a high byte set (0x1005 / 0x2005) vs 0x0005 for humans. Their Id byte is
        // the species id.
        ch.IsMonster = (ch.PersonalityCode & 0xFF00) != 0;
        if (ch.IsMonster)
        {
            ch.SpeciesId = ch.Id;
        }

        ch.Str   = _raw[rOff + Off_Str];
        ch.Res   = _raw[rOff + Off_Res];
        ch.Agl   = _raw[rOff + Off_Agl];
        ch.Wis   = _raw[rOff + Off_Wis];
        ch.Lck   = _raw[rOff + Off_Lck];
        ch.Level = _raw[rOff + Off_Level];
        ch.Unk2  = _raw[rOff + Off_Unk2];

        // Preserve roster trailing bytes unchanged
        ch.RosterTrailing = new byte[RosterTrailingLen];
        Array.Copy(_raw, rOff + Off_RosterTrailing, ch.RosterTrailing, 0, RosterTrailingLen);

        if (aOff >= 0)
        {
            // Hero: live data comes from the dedicated active slot
            ch.Exp   = BitConverter.ToUInt32(_raw, aOff + Off_Exp);
            ch.HpCur = BitConverter.ToUInt16(_raw, aOff + Off_HpCur);
            ch.HpMax = BitConverter.ToUInt16(_raw, aOff + Off_HpMax);
            ch.MpCur = BitConverter.ToUInt16(_raw, aOff + Off_MpCur);
            ch.MpMax = BitConverter.ToUInt16(_raw, aOff + Off_MpMax);
            ch.ActiveSlotByte29 = _raw[aOff + Off_Byte29];
            ch.ActiveSlotByte2A = _raw[aOff + Off_Byte2A];

            for (int s = 0; s < ItemSlots; s++)
            {
                int pos = aOff + Off_Items + s * 2;
                ch.Items[s] = new CharItem { ItemId = _raw[pos], Flag = _raw[pos + 1] };
            }
        }
        else
        {
            ch.Exp   = SanitizeU32(_raw, rOff + Off_Exp);
            ch.HpCur = SanitizeU16(_raw, rOff + Off_HpCur);
            ch.HpMax = SanitizeU16(_raw, rOff + Off_HpMax);
            ch.MpCur = SanitizeU16(_raw, rOff + Off_MpCur);
            ch.MpMax = SanitizeU16(_raw, rOff + Off_MpMax);

            for (int s = 0; s < ItemSlots; s++)
            {
                ch.Items[s] = new CharItem();
            }
        }

        return ch;
    }

    private static uint   SanitizeU32(byte[] raw, int off) { var v = BitConverter.ToUInt32(raw, off); return v == 0xD5D5D5D5 ? 0u : v; }
    private static ushort SanitizeU16(byte[] raw, int off) { var v = BitConverter.ToUInt16(raw, off); return v == 0xD5D5 ? (ushort)0 : v; }

    private string ReadName(int offset, int maxLen)
    {
        var sb = new StringBuilder();

        for (int i = 0; i < maxLen; i++)
        {
            byte b = _raw[offset + i];
            if (b == 0x00 || b == 0xD5) break;
            sb.Append((char)b);
        }

        return sb.ToString();
    }

    private void ParseBagItems()
    {
        for (int i = 0; i < BagItemSlots; i++)
        {
            int off = FO(BagOffset) + i * 2;
            BagItems[i] = new BagItem { ItemId = _raw[off], Quantity = _raw[off + 1] };
        }
    }

    // ── Flush ────────────────────────────────────────────────────────────────
    public void FlushCharacter(Character ch)
    {
        // Recruited monsters are mirrored across several RAM structures. To make an
        // edit take effect on an F1 reload we write every located main-RAM copy:
        //   • name-anchored live structs  → full live data (EXP/HP/MP + attributes)
        //   • roster-format copies        → base attributes + level
        //   • the STR-anchored battle array (wagon members) → full live data
        // The .sav-buffer roster record (RosterOffset) is also written so a cold
        // "Continue" from SRAM stays consistent.
        if (ch.IsMonster)
        {
            foreach (int nameOff in ch.LiveOffsets)
            {
                FlushLiveNameAnchored(ch, nameOff);
            }

            FlushRosterStats(ch, ch.RosterOffset);
            foreach (int recOff in ch.ExtraRosterOffsets)
            {
                FlushRosterStats(ch, recOff);
            }

            if (ch.LiveStatOffset >= 0)
            {
                FlushHeroLiveData(ch);
            }

            return;
        }

        // In a save state, any character with a live party slot (hero OR a recruited
        // party member) must have stats written to live game memory — that's what
        // the game actually reads. Slot 0 = hero; others matched by STR+AGL on load.
        bool hasLiveSlot = IsSaveState &&
                           (ch.SlotIndex == 0 ? HasLiveHeroData : ch.LiveStatOffset >= 0);

        if (hasLiveSlot)
        {
            FlushHeroLiveData(ch);   // writes to ch.LiveStatOffset (hero uses its own)
            FlushRoster(ch);         // keep the .sav buffer copy consistent
        }
        else
        {
            FlushRoster(ch);
            FlushActiveSlot(ch);
        }
    }

    private void FlushRoster(Character ch)
    {
        int o = ch.RosterOffset;
        _raw[o] = ch.Id;

        for (int i = 0; i < 18; i++)
        {
            _raw[o + Off_Name + i] = 0xD5;
        }

        byte[] nb = Encoding.ASCII.GetBytes(ch.Name);
        int nl = Math.Min(nb.Length, 17);
        Array.Copy(nb, 0, _raw, o + Off_Name, nl);
        _raw[o + Off_Name + nl] = 0x00;

        BitConverter.GetBytes(ch.PersonalityCode).CopyTo(_raw, o + Off_Personality);
        _raw[o + Off_Str]   = ch.Str;
        _raw[o + Off_Res]   = ch.Res;
        _raw[o + Off_Agl]   = ch.Agl;
        _raw[o + Off_Wis]   = ch.Wis;
        _raw[o + Off_Lck]   = ch.Lck;
        _raw[o + Off_Level] = ch.Level;
        _raw[o + Off_Unk2]  = ch.Unk2;
        _raw[o + Off_Sentinel] = 0xD5;

        // For non-hero characters, EXP/HP/MP live in the roster trailing bytes
        if (ch.ActiveOffset < 0)
        {
            BitConverter.GetBytes(ch.Exp).CopyTo(_raw, o + Off_Exp);
            BitConverter.GetBytes(ch.HpCur).CopyTo(_raw, o + Off_HpCur);
            BitConverter.GetBytes(ch.HpMax).CopyTo(_raw, o + Off_HpMax);
            BitConverter.GetBytes(ch.MpCur).CopyTo(_raw, o + Off_MpCur);
            BitConverter.GetBytes(ch.MpMax).CopyTo(_raw, o + Off_MpMax);
        }
        else
        {
            // Hero: restore the unchanged roster trailing bytes as-is
            Array.Copy(ch.RosterTrailing, 0, _raw, o + Off_RosterTrailing, ch.RosterTrailing.Length);
        }
    }

    private void FlushActiveSlot(Character ch)
    {
        int a = ch.ActiveOffset;
        if (a < 0 || a + CharStride > _raw.Length)
        {
            return;
        }

        BitConverter.GetBytes(ch.Exp).CopyTo(_raw, a + Off_Exp);
        BitConverter.GetBytes(ch.HpCur).CopyTo(_raw, a + Off_HpCur);
        BitConverter.GetBytes(ch.HpMax).CopyTo(_raw, a + Off_HpMax);
        BitConverter.GetBytes(ch.MpCur).CopyTo(_raw, a + Off_MpCur);
        BitConverter.GetBytes(ch.MpMax).CopyTo(_raw, a + Off_MpMax);

        _raw[a + Off_Byte29] = ch.ActiveSlotByte29;
        _raw[a + Off_Byte2A] = ch.ActiveSlotByte2A;

        for (int s = 0; s < ItemSlots; s++)
        {
            int pos = a + Off_Items + s * 2;
            _raw[pos]     = ch.Items[s].ItemId;
            _raw[pos + 1] = ch.Items[s].Flag;
        }
    }

    public void FlushBagItem(int slot)
    {
        int off = FO(BagOffset) + slot * 2;
        _raw[off]     = BagItems[slot].ItemId;
        _raw[off + 1] = BagItems[slot].Quantity;
    }

    public void Save(string path)
    {
        foreach (var ch in Characters)
        {
            FlushCharacter(ch);
        }

        for (int i = 0; i < BagItemSlots; i++)
        {
            FlushBagItem(i);
        }

        File.WriteAllBytes(path, _raw);
    }
}
