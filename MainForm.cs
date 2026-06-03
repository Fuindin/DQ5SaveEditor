using System.Drawing;

namespace DQ5SaveEditor;

public partial class MainForm : Form
{
    private SaveData?  _save;
    private string?    _filePath;
    private Character? _selectedChar;
    private bool       _isDirty;          // true = unsaved changes in memory

    public MainForm()
    {
        InitializeComponent();
        ApplyDarkTheme();
    }

    // ── Dark theme ────────────────────────────────────────────────────────────
    private void ApplyDarkTheme()
    {
        BackColor = Color.FromArgb(25, 25, 35);
        ForeColor = Color.White;

        _menu.BackColor = Color.FromArgb(35, 35, 45);
        _menu.ForeColor = Color.White;

        foreach (ToolStripMenuItem item in _menu.Items)
        { 
            item.BackColor = Color.FromArgb(35, 35, 45); item.ForeColor = Color.White; 
        }

        _topBar.BackColor = Color.FromArgb(30, 30, 30);

        _status.BackColor = Color.FromArgb(30, 30, 40);
        _statusLabel.ForeColor = Color.LightGray;

        _charList.BackColor = Color.FromArgb(30, 30, 45);
        _charList.ForeColor = Color.White;

        _tabs.BackColor = Color.FromArgb(25, 25, 35);
        foreach (TabPage tp in _tabs.TabPages)
        { 
            tp.BackColor = Color.FromArgb(25, 25, 35); tp.ForeColor = Color.White; 
        }

        foreach (var btn in new[] { _maxStatsBtn, _maxExpBtn, _fullHpMpBtn })
        { 
            btn.BackColor = Color.FromArgb(55, 55, 80); btn.ForeColor = Color.White; 
        }

        StyleGrid(_itemsGrid);
        StyleGrid(_bagGrid);
    }

    private static void StyleGrid(DataGridView g)
    {
        g.BackgroundColor = Color.FromArgb(25, 25, 35);
        g.DefaultCellStyle.BackColor = Color.FromArgb(35, 35, 50);
        g.DefaultCellStyle.ForeColor = Color.White;
        g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(70, 70, 120);
        g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 60);
        g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        g.EnableHeadersVisualStyles = false;
        g.GridColor = Color.FromArgb(50, 50, 70);
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

        e.Graphics.FillRectangle(
            new SolidBrush(sel ? Color.FromArgb(70, 70, 120) : Color.FromArgb(30, 30, 45)),
            e.Bounds);

        using var nameFont = new Font("Segoe UI", 10F, FontStyle.Bold);
        using var infoFont = new Font("Segoe UI", 8F);
        e.Graphics.DrawString(ch.Name, nameFont, Brushes.White, e.Bounds.X + 8, e.Bounds.Y + 2);
        e.Graphics.DrawString(
            $"Lv{ch.Level}  HP:{ch.HpMax}  MP:{ch.MpMax}",
            infoFont, new SolidBrush(Color.FromArgb(160, 200, 160)),
            e.Bounds.X + 8, e.Bounds.Y + 14);
    }

    private void LoadCharIntoEditor(Character ch)
    {
        _charNameLabel.Text = ch.Name;
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

        for (int i = 0; i < SaveData.BagItemSlots; i++)
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

    private void SetStatus(string msg) => _statusLabel.Text = msg;
}
