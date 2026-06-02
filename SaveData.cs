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

    public bool IsEmpty => string.IsNullOrEmpty(Name);

    public override string ToString() => IsEmpty ? $"(empty slot {SlotIndex})" : Name;
}

public class CharItem
{
    public byte ItemId { get; set; }
    public byte Flag { get; set; }      // 0x80 = equipped, 0x00 = in bag

    public bool IsEmpty => ItemId == 0x00 || ItemId == 0xFF || ItemId == 0xD5;
    public bool IsEquipped => Flag == 0x80;

    public string ItemName => ItemNames.TryGetValue(ItemId, out var n) ? n : $"Unknown (0x{ItemId:X2})";

    public static readonly Dictionary<byte, string> ItemNames = new()
    {
        // ── Consumables ──────────────────────────────────────────────────────
        [0x00] = "Nothing",
        [0xA0] = "Medicinal Herb",
        [0xA1] = "Medical Herb",
        [0xA2] = "Strong Medicine",
        [0xA3] = "Special Medicine",
        [0xA4] = "Antidotal Herb",
        [0xA5] = "Coagulant",
        [0xA6] = "Holy Water",
        [0xA7] = "Magic Water",
        [0xA8] = "Chimaera Wing",
        [0xA9] = "Moonwort Bulb",
        [0xAA] = "Amor Seco Essence",
        [0xAB] = "Yggdrasil Dew",
        [0xAC] = "Seed of Strength",
        [0xAD] = "Seed of Agility",
        [0xAE] = "Seed of Resilience",
        [0xAF] = "Seed of Wisdom",
        [0xB0] = "Yggdrasil Leaf",
        [0xB1] = "Dragon Scale",
        [0xB2] = "T 'n' T Ticket",
        [0xB3] = "Iron Ore",
        [0xB4] = "Slime Earrings",
        [0xB5] = "Seed of Life",
        [0xB6] = "Seed of Magic",
        [0xB7] = "Seed of Luck",
        [0xD2] = "Adventurer's Map",

        // ── Weapons ──────────────────────────────────────────────────────────
        [0x01] = "Cypress Stick",
        [0x02] = "Oaken Club",
        [0x03] = "Thorn Whip",
        [0x04] = "Copper Sword",
        [0x05] = "Chain Sickle",
        [0x06] = "Boomerang",
        [0x07] = "Edged Boomerang",
        [0x08] = "Steel Broadsword",
        [0x09] = "Zombiesbane",
        [0x0A] = "Imp Knife",
        [0x0B] = "Falcon Knife",
        [0x0C] = "Blades of Byterian",
        [0x0D] = "Flametang Boomerang",
        [0x0E] = "Cautery Sword",
        [0x0F] = "Magic Knife",
        [0x10] = "Sword of Malice",
        [0x11] = "Magma Staff",
        [0x12] = "Dragonsbane",
        [0x13] = "Spiked Steel Whip",
        [0x14] = "Holy Lance",
        [0x15] = "Multithrust Spear",
        [0x16] = "Dream Blade",
        [0x17] = "Rusty Old Sword",
        [0x18] = "Hela's Hammer",
        [0x19] = "Liquid Metal Sword",
        [0x1A] = "Gringham Whip",
        [0x1B] = "Xenlon Rod",

        // ── Helmets ──────────────────────────────────────────────────────────
        [0x40] = "Hardwood Headwear",
        [0x41] = "Iron Helmet",
        [0x42] = "Happy Hat",
        [0x43] = "Hermes Hat",
        [0x44] = "Leather Hat",
        [0x45] = "Stone Hardhat",
        [0x46] = "Thinking Cap",
        [0x47] = "Mercury's Bandana",
        [0x48] = "Mythril Helm",
        [0x49] = "Great Helm",
        [0x4A] = "Gold Circlet",
        [0x4B] = "Silver Tiara",
        [0x4C] = "Scholar's Cap",

        // ── Shields ──────────────────────────────────────────────────────────
        [0x50] = "Pot Lid",
        [0x51] = "Leather Shield",
        [0x52] = "Bronze Shield",
        [0x53] = "Iron Shield",
        [0x54] = "Magic Shield",
        [0x55] = "Mythril Shield",
        [0x56] = "Flame Shield",
        [0x57] = "Ice Shield",
        [0x58] = "Erdrick's Shield",
        [0x59] = "Metal King Shield",

        // ── Body Armour ───────────────────────────────────────────────────────
        [0x60] = "Plain Clothes",
        [0x61] = "Wayfarer's Clothes",
        [0x62] = "Leather Armour",
        [0x63] = "Scale Armour",
        [0x64] = "Fur Cape",
        [0x65] = "Chain Mail",
        [0x66] = "Cloak of Evasion",
        [0x67] = "Iron Armour",
        [0x68] = "Steel Armour",
        [0x69] = "Magic Armour",
        [0x6A] = "Princess Robe",
        [0x6B] = "Flowing Dress",
        [0x6C] = "Sacred Armour",
        [0x6D] = "Dragon Mail",
        [0x6E] = "Metal King Armour",
        [0x6F] = "Slime Armour",
        [0x86] = "Plain Clothes",

        // ── Accessories ───────────────────────────────────────────────────────
        [0x70] = "Gold Ring",
        [0x71] = "Silver Ring",
        [0x72] = "Life Bracer",
        [0x73] = "Recovery Ring",
        [0x74] = "Protection Ring",
        [0x75] = "Prayer Ring",
        [0x76] = "Goddess Ring",
        [0x77] = "Agility Ring",
        [0x78] = "Intelligence Ring",
        [0x79] = "Speed Ring",
        [0x7A] = "Power Ring",
        [0x7B] = "Elfin Charm",
        [0x7C] = "Meteorite Bracer",

        // ── Key Items ─────────────────────────────────────────────────────────
        [0x80] = "Zenithian Sword",
        [0x81] = "Zenithian Shield",
        [0x82] = "Zenithian Helmet",
        [0x83] = "Zenithian Armour",
        [0x85] = "Ship",
        [0x87] = "Medal",
        [0x8F] = "Flute of Spring",
        [0x90] = "Echo Flute",
        [0x91] = "Wagon",
        [0xD0] = "Fur Hood",
        [0xD1] = "Iron Mask",
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
    private const int MaxCharSlots = 20;

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
    /// Read the hero's live stats from the save state into the Character model.
    /// Anchors to the confirmed STR field position found by FindHeroLiveOffsets.
    /// </summary>
    public void ReadHeroLiveData(Character hero)
    {
        if (_liveStatOffset < 0)
        {
            return;
        }

        int s = _liveStatOffset;
        hero.Exp   = BitConverter.ToUInt32(_raw, s + SS_Exp);
        hero.Level = _raw[s + SS_Level];
        hero.Str   = (byte)BitConverter.ToUInt16(_raw, s + SS_Str);
        hero.Res   = (byte)BitConverter.ToUInt16(_raw, s + SS_Res);
        hero.HpCur = BitConverter.ToUInt16(_raw, s + SS_HpCur);
        hero.HpMax = BitConverter.ToUInt16(_raw, s + SS_HpMax);
        hero.MpCur = BitConverter.ToUInt16(_raw, s + SS_MpCur);
        hero.MpMax = BitConverter.ToUInt16(_raw, s + SS_MpMax);
        hero.Agl   = _raw[s + SS_Agl];
        hero.Wis   = _raw[s + SS_Wis];
        hero.Lck   = _raw[s + SS_Lck];
    }

    /// <summary>Write the hero's edited stats to ALL live copies in the save state.</summary>
    public void FlushHeroLiveData(Character hero)
    {
        foreach (int s in _heroLiveOffsets)
        {
            BitConverter.GetBytes(hero.Exp).CopyTo(_raw, s + SS_Exp);
            _raw[s + SS_Level] = hero.Level;
            BitConverter.GetBytes((ushort)hero.Str).CopyTo(_raw, s + SS_Str);
            BitConverter.GetBytes((ushort)hero.Res).CopyTo(_raw, s + SS_Res);
            BitConverter.GetBytes(hero.HpCur).CopyTo(_raw, s + SS_HpCur);
            BitConverter.GetBytes(hero.HpMax).CopyTo(_raw, s + SS_HpMax);
            BitConverter.GetBytes(hero.MpCur).CopyTo(_raw, s + SS_MpCur);
            BitConverter.GetBytes(hero.MpMax).CopyTo(_raw, s + SS_MpMax);
            _raw[s + SS_Agl] = hero.Agl;
            _raw[s + SS_Wis] = hero.Wis;
            _raw[s + SS_Lck] = hero.Lck;
        }

        // Items (at SS_Items offset from STR)
        if (_liveStatOffset >= 0)
        {
            for (int idx = 0; idx < ItemSlots; idx++)
            {
                int pos = _liveStatOffset + SS_Items + idx * 4;
                if (pos + 4 > _raw.Length) break;
                ushort rawId = hero.Items[idx].ItemId;
                BitConverter.GetBytes(rawId).CopyTo(_raw, pos);
                _raw[pos + 2] = 1;
                _raw[pos + 3] = hero.Items[idx].IsEquipped ? (byte)1 : (byte)0;
            }
        }
    }

    public List<Character> Characters { get; } = [];
    public BagItem[] BagItems { get; } = new BagItem[BagItemSlots];

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

    // Items in live format: 12 × (u16 itemId, u8 qty, u8 equipped=1/0) = 48 bytes
    // Items block is 0x20 bytes after personality (which is 8 bytes before STR)
    // → items at STR - 0x08 + 0x28 = STR + 0x20
    private const int SS_Items  = 0x20;  // 12 × (u16 id, u8 qty, u8 flag)

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
        save.FindHeroLiveOffsets();   // locate live character data in main RAM

        return save;
    }

    private void FindHeroLiveOffsets()
    {
        // ── Hardcoded primary anchor (DQ5 DS North American version) ─────────
        // DS address 0x0209DD78 is Jack's live STR field — confirmed empirically.
        // It is a fixed game-allocated address independent of save content.
        // File offset = ML1_MAIN_RAM_START + (DS_addr - 0x02000000)
        //             = 0x24 + 0x09DD78 = 0x0009DD9C
        const int fixedOffset = ML1_MAIN_RAM_START + 0x09DD78;  // 0x0009DD9C

        // Verify the offset is plausible (STR in 0–255)
        if (fixedOffset + 2 <= _raw.Length &&
            BitConverter.ToUInt16(_raw, fixedOffset) <= 255)
        {
            _liveStatOffset = fixedOffset;
            _heroLiveOffsets.Add(fixedOffset);
        }

        // ── Optional: also find duplicate copies by pattern search ───────────
        // Only search if we have known current stats (might miss if diverged).
        if (Characters.Count > 0)
        {
            var hero = Characters[0];
            byte[] sig = new byte[15];
            BitConverter.GetBytes((ushort)hero.Str).CopyTo(sig, 0);
            BitConverter.GetBytes((ushort)hero.Res).CopyTo(sig, 2);
            BitConverter.GetBytes(hero.HpCur).CopyTo(sig,       4);
            BitConverter.GetBytes(hero.HpMax).CopyTo(sig,       6);
            BitConverter.GetBytes(hero.MpCur).CopyTo(sig,       8);
            BitConverter.GetBytes(hero.MpMax).CopyTo(sig,       10);
            sig[12] = hero.Agl; sig[13] = hero.Wis; sig[14] = hero.Lck;

            int end = Math.Min(_raw.Length - sig.Length, ML1_MAIN_RAM_START + 0x400000);
            for (int i = ML1_MAIN_RAM_START; i < end; i++)
            {
                if (i == fixedOffset) continue;  // already added
                bool match = true;

                for (int j = 0; j < sig.Length; j++)
                {
                    if (_raw[i + j] != sig[j]) { match = false; break; }
                }

                if (match)
                {
                    _heroLiveOffsets.Add(i);
                }
            }
        }
    }

    private static SaveData LoadCommon(byte[] raw, int fileBase, bool isSaveState)
    {
        var save = new SaveData(raw, fileBase, isSaveState);
        save.ParseCharacters();
        save.ParseBagItems();

        return save;
    }

    private void ParseCharacters()
    {
        for (int i = 0; i < MaxCharSlots; i++)
        {
            int rosterOff = FO(RosterStart + i * CharStride);

            if (rosterOff + CharStride > _raw.Length)
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
        if (IsSaveState && ch.SlotIndex == 0 && HasLiveHeroData)
        {
            // Save state: write to live game data (what the game actually reads)
            FlushHeroLiveData(ch);
            // Also update the .sav buffer copy so it stays consistent
            FlushRoster(ch);
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
