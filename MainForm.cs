using System.Drawing;
using System.Drawing.Drawing2D;

namespace DQ5SaveEditor;

public partial class MainForm : Form
{
    private SaveData?  _save;
    private string?    _filePath;
    private Character? _selectedChar;
    private bool       _isDirty;          // true = unsaved changes in memory

    private readonly string? _initialFile;

    public MainForm() : this(null) { }

    public MainForm(string? initialFile)
    {
        InitializeComponent();
        ApplyDqTheme();
        _initialFile = initialFile;
    }

    private void LoadInitialFile()
    {
        if (_initialFile == null)
        {
            return;
        }

        try
        {
            _save = SaveData.LoadSaveState(_initialFile);
            _filePath = _initialFile;
            _isDirty = false;
            PopulateUI();
            SetStatus($"Loaded: {Path.GetFileName(_filePath)}  —  {_save.Characters.Count} characters found.");
            Text = $"DQ5 Save Editor — {Path.GetFileName(_filePath)}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load save state:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ── Dragon Quest V "command window" theme ──────────────────────────────────
    private void ApplyDqTheme()
    {
        BackColor = Theme.Backdrop;
        ForeColor = Theme.Text;
        Font = new Font(Theme.FontName, 9F);

        // Menu — navy with a custom renderer so dropdowns match.
        _menu.Renderer = new DqMenuRenderer();
        _menu.BackColor = Theme.Backdrop;
        _menu.ForeColor = Theme.Text;
        StyleMenuItems(_menu.Items);

        // Top bar (gold) and status strip.
        Theme.FrameContainer(_topBar, radius: 10, pad: 8);
        _goldLabel.ForeColor = Theme.Gold;
        _goldField.BackColor = Theme.Window;
        _goldField.ForeColor = Theme.Text;
        _goldField.BorderStyle = BorderStyle.FixedSingle;
        Theme.StyleButton(_applyGoldBtn);

        _status.BackColor = Theme.Backdrop;
        _statusLabel.ForeColor = Theme.SubText;

        // Character list window (Split.Panel1).
        _split.BackColor = Theme.Backdrop;
        _split.Panel1.BackColor = Theme.Backdrop;
        _split.Panel2.BackColor = Theme.Backdrop;
        Theme.FrameContainer(_split.Panel1, radius: 12, pad: 7);
        _listHeader.BackColor = Color.FromArgb(10, 22, 62);
        _listHeader.ForeColor = Theme.Gold;
        _charList.BackColor = Theme.Window;
        _charList.ForeColor = Theme.Text;
        _charList.BorderStyle = BorderStyle.None;

        // Tabs — owner-drawn navy/gold headers, framed pages.
        _tabs.BackColor = Theme.Backdrop;
        _tabs.DrawMode = TabDrawMode.OwnerDrawFixed;
        _tabs.SizeMode = TabSizeMode.Fixed;
        _tabs.ItemSize = new Size(120, 30);
        _tabs.Padding = new Point(0, 0);
        _tabs.DrawItem += Tabs_DrawItem;
        foreach (TabPage tp in _tabs.TabPages)
        {
            // The tab control supplies the border; just navy-fill the page. (A custom
            // Paint frame on a TabPage suppresses its child controls, so avoid it here.)
            tp.BackColor = Theme.Window;
            tp.ForeColor = Theme.Text;
            tp.Padding = new Padding(6);
        }

        // Stats tab inner panels.
        _charNameLabel.BackColor = Color.FromArgb(10, 22, 62);
        _charNameLabel.ForeColor = Theme.Gold;
        _statsRows.BackColor = Theme.Window;
        _bottomStrip.BackColor = Theme.Window;

        foreach (var btn in new[] { _maxStatsBtn, _maxExpBtn, _fullHpMpBtn })
        {
            Theme.StyleButton(btn);
        }
        Theme.StyleButton(_applyStatsBtn, primary: true);
        _applyStatsBtn.Text = "▶ Apply to Character";

        // Read-only footer buttons stay subdued.
        foreach (var btn in new[] { _applyItemsBtn, _applyBagBtn })
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = Color.FromArgb(10, 22, 62);
            btn.ForeColor = Theme.SubText;
            btn.FlatAppearance.BorderColor = Theme.FrameDim;
        }
        _itemsNote.ForeColor = Theme.SubText;
        _itemsHost.BackColor = Theme.Window;
        _itemsHeader.BackColor = Color.FromArgb(10, 22, 62);
        _itemsHeader.ForeColor = Theme.Gold;
        _itemsHeader.Font = new Font(Theme.FontName, 9.5F, FontStyle.Bold);

        Theme.StyleGrid(_itemsGrid);
        Theme.StyleGrid(_bagGrid);

        _statsTab.Resize += (s, e) => LayoutStatsColumns();
    }

    /// <summary>
    /// Size the stat column to just fit the "Stat [value]" pair (the items list fills
    /// the rest), clamped so it never swallows more than ~55% on a narrow window.
    /// </summary>
    private void LayoutStatsColumns()
    {
        if (_statsTab.ClientSize.Width <= 0)
        {
            return;
        }

        int want = (int)(370 * (DeviceDpi / 96.0));
        int cap = (int)(_statsTab.ClientSize.Width * 0.55);
        _statsRows.Width = Math.Min(want, cap);
    }

    private static void StyleMenuItems(ToolStripItemCollection items)
    {
        foreach (ToolStripItem item in items)
        {
            item.BackColor = Theme.Window;
            item.ForeColor = Theme.Text;
            if (item is ToolStripMenuItem mi && mi.HasDropDownItems)
            {
                StyleMenuItems(mi.DropDownItems);
            }
        }
    }

    // Owner-draw the tab headers as DQ command tabs (gold cursor on the active tab).
    private void Tabs_DrawItem(object? sender, DrawItemEventArgs e)
    {
        var tab = _tabs.GetTabRect(e.Index);
        bool active = _tabs.SelectedIndex == e.Index;

        using (var bg = new SolidBrush(active ? Theme.Window : Color.FromArgb(9, 18, 52)))
        {
            e.Graphics.FillRectangle(bg, tab);
        }

        if (active)
        {
            using var pen = new Pen(Theme.Gold, 2f);
            e.Graphics.DrawLine(pen, tab.Left + 6, tab.Bottom - 2, tab.Right - 6, tab.Bottom - 2);
        }

        TextRenderer.DrawText(
            e.Graphics, _tabs.TabPages[e.Index].Text,
            new Font(Theme.FontName, 9.5F, active ? FontStyle.Bold : FontStyle.Regular),
            tab, active ? Theme.Gold : Theme.SubText,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }

    // ── Menu events ───────────────────────────────────────────────────────────
    private void OpenMenuItem_Click(object? sender, EventArgs e)   => OpenFile();
    private void SaveMenuItem_Click(object? sender, EventArgs e)   => SaveFile();
    private void SaveAsMenuItem_Click(object? sender, EventArgs e) => SaveFileAs();
    private void ExitMenuItem_Click(object? sender, EventArgs e)   => Close();

    // ── Gold ──────────────────────────────────────────────────────────────────
    private void ApplyGoldBtn_Click(object? sender, EventArgs e)
    {
        if (_save == null)
        {
            return;
        }

        _save.Gold = (uint)_goldField.Value;
        MarkDirty($"Gold set to {_goldField.Value:N0}.  Use File → Save (Ctrl+S) to write to disk.");
    }

    // ── Character list ────────────────────────────────────────────────────────
    private void CharList_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_charList.SelectedItem is Character ch)
        {
            _selectedChar = ch;
            LoadCharIntoEditor(ch);
        }
    }

    private void CharList_DrawItem(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0 || _save == null)
        {
            return;
        }

        var ch = (Character)_charList.Items[e.Index];
        bool sel = (e.State & DrawItemState.Selected) != 0;

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Selected row: a brighter navy "highlight" pill with a gold cursor arrow.
        using (var bg = new SolidBrush(sel ? Color.FromArgb(40, 72, 146) : Theme.Window))
        {
            e.Graphics.FillRectangle(bg, e.Bounds);
        }
        if (sel)
        {
            using var arrow = new SolidBrush(Theme.Gold);
            int cy = e.Bounds.Y + e.Bounds.Height / 2;
            e.Graphics.FillPolygon(arrow, new[]
            {
                new Point(e.Bounds.X + 4, cy - 6),
                new Point(e.Bounds.X + 12, cy),
                new Point(e.Bounds.X + 4, cy + 6),
            });
        }

        using var nameFont = new Font(Theme.FontName, 10F, FontStyle.Bold);
        using var infoFont = new Font(Theme.FontName, 8F);

        // Lay the two lines out from the actual font height so the row scales with
        // DPI instead of relying on fixed pixel offsets (which overlap at >100%).
        int x = e.Bounds.X + 18;
        int top = e.Bounds.Y + 2;
        int nameHeight = (int)Math.Ceiling(nameFont.GetHeight(e.Graphics));

        // Tint monster names so they stand out from the human roster.
        Color nameColor = ch.IsMonster ? Theme.MonsterName : Theme.Text;
        using (var nameBrush = new SolidBrush(nameColor))
        {
            e.Graphics.DrawString(ch.Name, nameFont, nameBrush, x, top);
        }

        string info = $"Lv{ch.Level}  HP:{ch.HpMax}  MP:{ch.MpMax}";
        if (ch.IsMonster && ch.SpeciesName.Length > 0)
        {
            info += $"  ·  {ch.SpeciesName}";
        }

        using (var infoBrush = new SolidBrush(Theme.InfoGreen))
        {
            e.Graphics.DrawString(info, infoFont, infoBrush, x, top + nameHeight);
        }
    }

    /// <summary>
    /// Size each character-list row to fit both text lines at the current DPI.
    /// Called on load and whenever the DPI changes; fixed heights overlapped on
    /// high-DPI displays.
    /// </summary>
    private void UpdateCharListItemHeight()
    {
        if (!_charList.IsHandleCreated)
        {
            return;
        }

        using var nameFont = new Font("Segoe UI", 10F, FontStyle.Bold);
        using var infoFont = new Font("Segoe UI", 8F);
        using var g = _charList.CreateGraphics();
        _charList.ItemHeight = (int)Math.Ceiling(nameFont.GetHeight(g) + infoFont.GetHeight(g)) + 8;
        _charList.Invalidate();
    }

    private void LoadCharIntoEditor(Character ch)
    {
        _charNameLabel.Text = ch.IsMonster && ch.SpeciesName.Length > 0
            ? $"{ch.Name}  ({ch.SpeciesName})"
            : ch.Name;
        _levelField.Value  = ch.Level;
        _strField.Value    = ch.Str;
        _resField.Value    = ch.Res;
        _aglField.Value    = ch.Agl;
        _wisField.Value    = ch.Wis;
        _lckField.Value    = ch.Lck;
        _expField.Value    = Math.Min(ch.Exp, 9_999_999);
        _hpCurField.Value  = ch.HpCur;
        _hpMaxField.Value  = ch.HpMax;
        _mpCurField.Value  = ch.MpCur;
        _mpMaxField.Value  = ch.MpMax;
        PopulateItemsGrid(ch);
    }

    // ── Stats buttons ─────────────────────────────────────────────────────────
    private void MaxStatsBtn_Click(object? sender, EventArgs e)
    {
        _strField.Value = _resField.Value = _aglField.Value =
        _wisField.Value = _lckField.Value = 255;
        _levelField.Value = 99;
    }

    private void MaxExpBtn_Click(object? sender, EventArgs e) => _expField.Value = 9_999_999;

    private void FullHpMpBtn_Click(object? sender, EventArgs e)
    {
        if (_hpMaxField.Value > 0)
        {
            _hpCurField.Value = _hpMaxField.Value;
        }

        if (_mpMaxField.Value > 0)
        {
            _mpCurField.Value = _mpMaxField.Value;
        }
    }

    private void ApplyStatsBtn_Click(object? sender, EventArgs e)
    {
        if (_selectedChar == null || _save == null)
        {
            return;
        }

        _selectedChar.Level  = (byte)_levelField.Value;
        _selectedChar.Str    = (byte)_strField.Value;
        _selectedChar.Res    = (byte)_resField.Value;
        _selectedChar.Agl    = (byte)_aglField.Value;
        _selectedChar.Wis    = (byte)_wisField.Value;
        _selectedChar.Lck    = (byte)_lckField.Value;
        _selectedChar.Exp    = (uint)_expField.Value;
        _selectedChar.HpCur  = (ushort)_hpCurField.Value;
        _selectedChar.HpMax  = (ushort)_hpMaxField.Value;
        _selectedChar.MpCur  = (ushort)_mpCurField.Value;
        _selectedChar.MpMax  = (ushort)_mpMaxField.Value;
        _save.FlushCharacter(_selectedChar);
        _charList.Invalidate();
        MarkDirty($"Changes applied to {_selectedChar.Name}.  Use File → Save (Ctrl+S) to write to disk.");
    }

    // ── Items grid ────────────────────────────────────────────────────────────
    private void PopulateItemsGrid(Character ch)
    {
        _itemsGrid.Rows.Clear();
        for (int s = 0; s < ch.Items.Length; s++)
        {
            var item = ch.Items[s];
            if (item == null || item.IsEmpty)
            {
                continue;   // skip empty slots
            }

            string name = item.ItemName + (item.Qty > 1 ? $"  x{item.Qty}" : "");
            _itemsGrid.Rows.Add(s + 1, name, $"0x{item.ItemId:X2}",
                item.IsEquipped ? "✔" : "");
        }
    }

    // ── Bag / item display is read-only; no edit handlers needed. ──────────────
    private void CommitBag() { /* read-only: no-op */ }

    // ── File operations ───────────────────────────────────────────────────────
    private void OpenFile()
    {
        using var dlg = new OpenFileDialog
        {
            Title = "Open melonDS Save State",
            Filter = "melonDS Save State (*.ml1)|*.ml1|All Files (*.*)|*.*",
            InitialDirectory = @"D:\Emulation\DS\Saves"
        };

        if (dlg.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        try
        {
            if (_isDirty && !ConfirmDiscardChanges())
            {
                return;
            }

            _save = SaveData.LoadSaveState(dlg.FileName);
            _filePath = dlg.FileName;
            _isDirty = false;
            PopulateUI();
            SetStatus($"Loaded: {Path.GetFileName(_filePath)}  —  {_save.Characters.Count} characters found.  " +
                      $"(Close melonDS before editing, then load state with F1)");
            Text = $"DQ5 Save Editor — {Path.GetFileName(_filePath)}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load save state:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SaveFile()
    {
        if (_save == null || _filePath == null) 
        { 
            SaveFileAs(); 
            return; 
        }

        // Editor is .ml1 focused — no checksum needed for save states.

        ApplyStatsBtn_Click(null, EventArgs.Empty);
        CommitBag();
        _save.Gold = (uint)_goldField.Value;
        string bak = _filePath + ".bak";
        if (!File.Exists(bak))
        {
            File.Copy(_filePath, bak);
        }

        _save.Save(_filePath);
        ClearDirty($"Saved — load with F1 in melonDS to apply changes.");
    }

    private void SaveFileAs()
    {
        if (_save == null)
        {
            return;
        }
        
        using var dlg = new SaveFileDialog
        {
            Title = "Save melonDS Save State",
            Filter = "melonDS Save State (*.ml1)|*.ml1|All Files (*.*)|*.*",
            FileName = _filePath ?? "save.ml1",
            DefaultExt = "ml1",
        };

        if (dlg.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        ApplyStatsBtn_Click(null, EventArgs.Empty);
        CommitBag();
        _save!.Gold = (uint)_goldField.Value;
        _save.Save(dlg.FileName);
        _filePath = dlg.FileName;
        ClearDirty($"Saved to {Path.GetFileName(_filePath)}");
        Text = $"DQ5 Save Editor — {Path.GetFileName(_filePath)}";
    }

    // ── Populate UI ───────────────────────────────────────────────────────────
    private void PopulateUI()
    {
        if (_save == null)
        {
            return;
        }

        _goldField.Value = Math.Min(_save.Gold, 9_999_999);

        _charList.Items.Clear();
        foreach (var ch in _save.Characters)
        {
            _charList.Items.Add(ch);
        }

        // For save states, read live in-game data for every party character + bag
        if (_save.IsSaveState && _save.HasLiveHeroData)
        {
            foreach (var ch in _save.Characters)
            {
                if (ch.SlotIndex == 0)
                {
                    _save.ReadHeroLiveData(ch);
                }
                else if (ch.LiveStatOffset >= 0)
                {
                    _save.ReadLiveStats(ch, ch.LiveStatOffset);
                }
            }
            _save.ReadLiveBag();
        }

        if (_charList.Items.Count > 0)
        {
            _charList.SelectedIndex = 0;
        }

        PopulateBagGrid();
    }

    private void PopulateBagGrid()
    {
        if (_save == null)
        {
            return;
        }

        _bagGrid.Rows.Clear();

        for (int i = 0; i < _save.BagSlotCount; i++)
        {
            var item = _save.BagItems[i];
            if (item == null || item.IsEmpty)
            {
                continue;   // skip empty slots
            }

            _bagGrid.Rows.Add(i + 1, item.ItemName, $"0x{item.ItemId:X2}",
                item.Quantity.ToString());
        }
    }

    // Shared DataError handler — suppresses the default error dialog for both grids.
    private void Grid_DataError(object? sender, DataGridViewDataErrorEventArgs e)
    {
        e.Cancel = true;
    }

    // ── Dirty-state helpers ───────────────────────────────────────────────────
    private void MarkDirty(string statusMsg)
    {
        _isDirty = true;
        _statusLabel.ForeColor = Color.Gold;
        SetStatus("⚠ " + statusMsg);

        if (_filePath != null && !Text.StartsWith('*'))
        {
            Text = "* " + Text;
        }
    }

    private void ClearDirty(string statusMsg)
    {
        _isDirty = false;
        _statusLabel.ForeColor = Color.LightGray;
        SetStatus(statusMsg);

        if (_filePath != null)
        {
            Text = $"DQ5 Save Editor — {Path.GetFileName(_filePath)}";
        }
    }

    private bool ConfirmDiscardChanges()
    {
        var r = MessageBox.Show(
            "You have unsaved changes. Save before continuing?",
            "Unsaved Changes",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Warning);

        if (r == DialogResult.Yes) 
        { 
            SaveFile(); return !_isDirty; 
        }

        return r == DialogResult.No;
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (_isDirty && !ConfirmDiscardChanges())
        {
            e.Cancel = true;
        }

        base.OnFormClosing(e);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        UpdateCharListItemHeight();
        LoadInitialFile();

        // Keep the character list at a constant, readable width (FixedPanel.Panel1);
        // clamp in case DPI scaling pushed the designer value out of range.
        int target = (int)(330 * (DeviceDpi / 96.0));
        int maxDist = _split.Width - _split.Panel2MinSize - _split.SplitterWidth;
        if (maxDist > _split.Panel1MinSize)
        {
            _split.SplitterDistance = Math.Clamp(target, _split.Panel1MinSize, maxDist);
        }

        LayoutStatsColumns();
    }

    protected override void OnDpiChanged(DpiChangedEventArgs e)
    {
        base.OnDpiChanged(e);
        UpdateCharListItemHeight();
    }

    private void SetStatus(string msg) => _statusLabel.Text = msg;
}
