namespace DQ5SaveEditor;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        // ── Declare everything ────────────────────────────────────────────────
        _menu           = new MenuStrip();
        _fileMenuItem   = new ToolStripMenuItem();
        _openMenuItem   = new ToolStripMenuItem();
        _saveMenuItem   = new ToolStripMenuItem();
        _saveAsMenuItem = new ToolStripMenuItem();
        _menuSep        = new ToolStripSeparator();
        _exitMenuItem   = new ToolStripMenuItem();

        _topBar      = new Panel();
        _goldLabel   = new Label();
        _goldField   = new NumericUpDown();
        _applyGoldBtn = new Button();

        _split      = new SplitContainer();
        _listHeader = new Label();
        _charList   = new ListBox();

        _tabs    = new TabControl();
        _statsTab = new TabPage();
        _itemsTab = new TabPage();
        _bagTab   = new TabPage();

        // Stats tab
        _charNameLabel = new Label();
        _bottomStrip   = new Panel();
        _quickBtnFlow  = new FlowLayoutPanel();
        _maxStatsBtn   = new Button();
        _maxExpBtn     = new Button();
        _fullHpMpBtn   = new Button();
        _applyStatsBtn = new Button();
        _statsRows    = new Panel();
        _rowLevel     = new Panel(); _rowStr  = new Panel(); _rowRes  = new Panel();
        _rowAgl       = new Panel(); _rowWis  = new Panel(); _rowLck  = new Panel();
        _rowExp       = new Panel(); _rowHpCur= new Panel(); _rowHpMax= new Panel();
        _rowMpCur     = new Panel(); _rowMpMax= new Panel();

        _levelLbl  = new Label(); _levelField  = new NumericUpDown();
        _strLbl    = new Label(); _strField    = new NumericUpDown();
        _resLbl    = new Label(); _resField    = new NumericUpDown();
        _aglLbl    = new Label(); _aglField    = new NumericUpDown();
        _wisLbl    = new Label(); _wisField    = new NumericUpDown();
        _lckLbl    = new Label(); _lckField    = new NumericUpDown();
        _expLbl    = new Label(); _expField    = new NumericUpDown();
        _hpCurLbl  = new Label(); _hpCurField  = new NumericUpDown();
        _hpMaxLbl  = new Label(); _hpMaxField  = new NumericUpDown();
        _mpCurLbl  = new Label(); _mpCurField  = new NumericUpDown();
        _mpMaxLbl  = new Label(); _mpMaxField  = new NumericUpDown();

        // Items tab
        _itemsGrid      = new DataGridView();
        _itemsSlotCol   = new DataGridViewTextBoxColumn();
        _itemsNameCol   = new DataGridViewTextBoxColumn();
        _itemsIdCol     = new DataGridViewTextBoxColumn();
        _itemsEquipCol  = new DataGridViewTextBoxColumn();
        _itemsNote      = new Label();
        _applyItemsBtn  = new Button();

        // Bag tab
        _bagGrid       = new DataGridView();
        _bagSlotCol    = new DataGridViewTextBoxColumn();
        _bagNameCol    = new DataGridViewTextBoxColumn();
        _bagIdCol      = new DataGridViewTextBoxColumn();
        _bagQtyCol     = new DataGridViewTextBoxColumn();
        _applyBagBtn   = new Button();

        _status      = new StatusStrip();
        _statusLabel = new ToolStripStatusLabel();

        // ── Suspend layouts ────────────────────────────────────────────────────
        _menu.SuspendLayout();
        _topBar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_goldField).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_split).BeginInit();
        _split.Panel1.SuspendLayout();
        _split.Panel2.SuspendLayout();
        _split.SuspendLayout();
        _tabs.SuspendLayout();
        _statsTab.SuspendLayout();
        _itemsTab.SuspendLayout();
        _bagTab.SuspendLayout();
        _bottomStrip.SuspendLayout();
        _quickBtnFlow.SuspendLayout();
        _statsRows.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_levelField).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_strField).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_resField).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_aglField).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_wisField).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_lckField).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_expField).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_hpCurField).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_hpMaxField).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_mpCurField).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_mpMaxField).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_itemsGrid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)_bagGrid).BeginInit();
        _status.SuspendLayout();
        SuspendLayout();

        // ── Menu ──────────────────────────────────────────────────────────────
        _openMenuItem.Name = "_openMenuItem";
        _openMenuItem.ShortcutKeys = Keys.Control | Keys.O;
        _openMenuItem.Text = "Open .ml1…";
        _openMenuItem.Click += new EventHandler(OpenMenuItem_Click);

        _saveMenuItem.Name = "_saveMenuItem";
        _saveMenuItem.ShortcutKeys = Keys.Control | Keys.S;
        _saveMenuItem.Text = "Save";
        _saveMenuItem.Click += new EventHandler(SaveMenuItem_Click);

        _saveAsMenuItem.Name = "_saveAsMenuItem";
        _saveAsMenuItem.Text = "Save As…";
        _saveAsMenuItem.Click += new EventHandler(SaveAsMenuItem_Click);

        _menuSep.Name = "_menuSep";

        _exitMenuItem.Name = "_exitMenuItem";
        _exitMenuItem.Text = "Exit";
        _exitMenuItem.Click += new EventHandler(ExitMenuItem_Click);

        _fileMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            _openMenuItem, _saveMenuItem, _saveAsMenuItem, _menuSep, _exitMenuItem });
        _fileMenuItem.Name = "_fileMenuItem";
        _fileMenuItem.Text = "File";

        _menu.Items.AddRange(new ToolStripItem[] { _fileMenuItem });
        _menu.Location = new Point(0, 0);
        _menu.Name = "_menu";
        _menu.Size = new Size(1000, 24);
        _menu.TabIndex = 0;

        // ── Top bar ───────────────────────────────────────────────────────────
        _goldLabel.AutoSize = true;
        _goldLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _goldLabel.ForeColor = Color.Gold;
        _goldLabel.Location = new Point(12, 14);
        _goldLabel.Name = "_goldLabel";
        _goldLabel.Text = "Gold:";

        _goldField.Font = new Font("Segoe UI", 11F);
        _goldField.Location = new Point(65, 10);
        _goldField.Maximum = 9999999M;
        _goldField.Name = "_goldField";
        _goldField.Size = new Size(130, 27);
        _goldField.TabIndex = 1;

        _applyGoldBtn.Location = new Point(205, 10);
        _applyGoldBtn.Name = "_applyGoldBtn";
        _applyGoldBtn.Size = new Size(70, 30);
        _applyGoldBtn.TabIndex = 2;
        _applyGoldBtn.Text = "Apply";
        _applyGoldBtn.Click += new EventHandler(ApplyGoldBtn_Click);

        _topBar.Controls.Add(_goldLabel);
        _topBar.Controls.Add(_goldField);
        _topBar.Controls.Add(_applyGoldBtn);
        _topBar.Dock = DockStyle.Top;
        _topBar.Height = 50;
        _topBar.Name = "_topBar";
        _topBar.TabIndex = 1;

        // ── Character list (Split.Panel1) ─────────────────────────────────────
        _listHeader.BackColor = Color.FromArgb(40, 40, 60);
        _listHeader.Dock = DockStyle.Top;
        _listHeader.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        _listHeader.ForeColor = Color.White;
        _listHeader.Height = 28;
        _listHeader.Name = "_listHeader";
        _listHeader.Text = "Characters";
        _listHeader.TextAlign = ContentAlignment.MiddleCenter;

        _charList.Dock = DockStyle.Fill;
        _charList.DrawMode = DrawMode.OwnerDrawFixed;
        _charList.Font = new Font("Segoe UI", 10F);
        _charList.ItemHeight = 26;
        _charList.Name = "_charList";
        _charList.TabIndex = 0;
        _charList.SelectedIndexChanged += new EventHandler(CharList_SelectedIndexChanged);
        _charList.DrawItem += new DrawItemEventHandler(CharList_DrawItem);

        // Fill first, Top last — VS dock engine processes highest-index control first.
        _split.Panel1.Controls.Add(_charList);
        _split.Panel1.Controls.Add(_listHeader);

        // ── Stats tab ─────────────────────────────────────────────────────────
        _charNameLabel.BackColor = Color.FromArgb(35, 35, 50);
        _charNameLabel.Dock = DockStyle.Top;
        _charNameLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        _charNameLabel.ForeColor = Color.Gold;
        _charNameLabel.Height = 38;
        _charNameLabel.Name = "_charNameLabel";
        _charNameLabel.Text = "— select a character —";
        _charNameLabel.TextAlign = ContentAlignment.MiddleCenter;

        // Quick buttons
        _maxStatsBtn.FlatStyle = FlatStyle.Flat;
        _maxStatsBtn.Height = 28;
        _maxStatsBtn.Name = "_maxStatsBtn";
        _maxStatsBtn.Text = "Max All Stats";
        _maxStatsBtn.Width = 110;
        _maxStatsBtn.Click += new EventHandler(MaxStatsBtn_Click);

        _maxExpBtn.FlatStyle = FlatStyle.Flat;
        _maxExpBtn.Height = 28;
        _maxExpBtn.Name = "_maxExpBtn";
        _maxExpBtn.Text = "Max EXP";
        _maxExpBtn.Width = 90;
        _maxExpBtn.Click += new EventHandler(MaxExpBtn_Click);

        _fullHpMpBtn.FlatStyle = FlatStyle.Flat;
        _fullHpMpBtn.Height = 28;
        _fullHpMpBtn.Name = "_fullHpMpBtn";
        _fullHpMpBtn.Text = "Full HP/MP";
        _fullHpMpBtn.Width = 90;
        _fullHpMpBtn.Click += new EventHandler(FullHpMpBtn_Click);

        _quickBtnFlow.BackColor = Color.Transparent;
        _quickBtnFlow.Controls.AddRange(new Control[] { _maxStatsBtn, _maxExpBtn, _fullHpMpBtn });
        _quickBtnFlow.Dock = DockStyle.Top;
        _quickBtnFlow.FlowDirection = FlowDirection.LeftToRight;
        _quickBtnFlow.Height = 36;
        _quickBtnFlow.Name = "_quickBtnFlow";
        _quickBtnFlow.WrapContents = false;

        _applyStatsBtn.BackColor = Color.FromArgb(0, 120, 70);
        _applyStatsBtn.Dock = DockStyle.Bottom;
        _applyStatsBtn.FlatStyle = FlatStyle.Flat;
        _applyStatsBtn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        _applyStatsBtn.ForeColor = Color.White;
        _applyStatsBtn.Height = 34;
        _applyStatsBtn.Name = "_applyStatsBtn";
        _applyStatsBtn.Text = "✔ Apply to Character";
        _applyStatsBtn.Click += new EventHandler(ApplyStatsBtn_Click);

        // Add Top before Bottom inside bottomStrip
        _bottomStrip.BackColor = Color.FromArgb(30, 30, 45);
        _bottomStrip.Controls.Add(_quickBtnFlow);
        _bottomStrip.Controls.Add(_applyStatsBtn);
        _bottomStrip.Dock = DockStyle.Bottom;
        _bottomStrip.Height = 80;
        _bottomStrip.Name = "_bottomStrip";
        _bottomStrip.Padding = new Padding(10, 6, 10, 6);

        // Each stat row = a 32 px Panel containing: NumericUpDown (Dock=Right, 135px)
        // then Label (Dock=Fill, right-aligned text).  No TableLayoutPanel — avoids
        // the .NET 10 edge case where the last label escapes its cell.
        BuildStatRow(_rowLevel,  _levelLbl,  _levelField,  "Level");
        BuildStatRow(_rowStr,    _strLbl,    _strField,    "Strength");
        BuildStatRow(_rowRes,    _resLbl,    _resField,    "Resilience");
        BuildStatRow(_rowAgl,    _aglLbl,    _aglField,    "Agility");
        BuildStatRow(_rowWis,    _wisLbl,    _wisField,    "Wisdom");
        BuildStatRow(_rowLck,    _lckLbl,    _lckField,    "Luck");
        BuildStatRow(_rowExp,    _expLbl,    _expField,    "Experience");
        BuildStatRow(_rowHpCur,  _hpCurLbl,  _hpCurField,  "HP (current)");
        BuildStatRow(_rowHpMax,  _hpMaxLbl,  _hpMaxField,  "HP (max)");
        BuildStatRow(_rowMpCur,  _mpCurLbl,  _mpCurField,  "MP (current)");
        BuildStatRow(_rowMpMax,  _mpMaxLbl,  _mpMaxField,  "MP (max)");

        // Configure each NumericUpDown range (font/width set inside BuildStatRow)
        _levelField.Minimum  = 1;   _levelField.Maximum  = 99;
        _strField.Minimum    = 0;   _strField.Maximum    = 255;
        _resField.Minimum    = 0;   _resField.Maximum    = 255;
        _aglField.Minimum    = 0;   _aglField.Maximum    = 255;
        _wisField.Minimum    = 0;   _wisField.Maximum    = 255;
        _lckField.Minimum    = 0;   _lckField.Maximum    = 255;
        _expField.Minimum    = 0;   _expField.Maximum    = 9999999;
        _hpCurField.Minimum  = 0;   _hpCurField.Maximum  = 9999;
        _hpMaxField.Minimum  = 0;   _hpMaxField.Maximum  = 9999;
        _mpCurField.Minimum  = 0;   _mpCurField.Maximum  = 9999;
        _mpMaxField.Minimum  = 0;   _mpMaxField.Maximum  = 9999;

        // Stack the rows top-to-bottom inside _statsRows (Fill panel, auto-scroll)
        _statsRows.AutoScroll = true;
        _statsRows.BackColor = Color.FromArgb(25, 25, 35);
        _statsRows.Dock = DockStyle.Fill;
        _statsRows.Name = "_statsRows";
        _statsRows.Padding = new Padding(8, 4, 8, 4);
        // Add rows last-first so the dock engine places them top-to-bottom
        _statsRows.Controls.Add(_rowMpMax);
        _statsRows.Controls.Add(_rowMpCur);
        _statsRows.Controls.Add(_rowHpMax);
        _statsRows.Controls.Add(_rowHpCur);
        _statsRows.Controls.Add(_rowExp);
        _statsRows.Controls.Add(_rowLck);
        _statsRows.Controls.Add(_rowWis);
        _statsRows.Controls.Add(_rowAgl);
        _statsRows.Controls.Add(_rowRes);
        _statsRows.Controls.Add(_rowStr);
        _statsRows.Controls.Add(_rowLevel);

        // Fill first, Bottom next, Top last — dock engine processes highest-index first.
        _statsTab.Controls.Add(_statsRows);
        _statsTab.Controls.Add(_bottomStrip);
        _statsTab.Controls.Add(_charNameLabel);
        _statsTab.Name = "_statsTab";
        _statsTab.Padding = new Padding(0);
        _statsTab.Text = "Stats";

        // ── Items tab ─────────────────────────────────────────────────────────
        _itemsSlotCol.HeaderText = "#";
        _itemsSlotCol.Name = "Slot";
        _itemsSlotCol.ReadOnly = true;
        _itemsSlotCol.Width = 30;

        _itemsNameCol.HeaderText = "Item";
        _itemsNameCol.Name = "ItemName";
        _itemsNameCol.ReadOnly = true;
        _itemsNameCol.Width = 220;

        _itemsIdCol.HeaderText = "ID";
        _itemsIdCol.Name = "IdHex";
        _itemsIdCol.ReadOnly = true;
        _itemsIdCol.Width = 55;

        _itemsEquipCol.HeaderText = "Equipped";
        _itemsEquipCol.Name = "Equipped";
        _itemsEquipCol.Width = 70;

        _itemsGrid.AllowUserToAddRows = false;
        _itemsGrid.AllowUserToDeleteRows = false;
        _itemsGrid.ReadOnly = true;   // items are read-only (display) for now
        _itemsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        _itemsGrid.Columns.AddRange(new DataGridViewColumn[] {
            _itemsSlotCol, _itemsNameCol, _itemsIdCol, _itemsEquipCol });
        _itemsGrid.Dock = DockStyle.Fill;
        _itemsGrid.Name = "_itemsGrid";
        _itemsGrid.RowHeadersVisible = false;
        _itemsGrid.TabIndex = 0;
        _itemsGrid.DataError += new DataGridViewDataErrorEventHandler(Grid_DataError);

        _itemsNote.Dock = DockStyle.Bottom;
        _itemsNote.ForeColor = Color.Gray;
        _itemsNote.Font = new Font("Segoe UI", 8F);
        _itemsNote.Height = 24;
        _itemsNote.Name = "_itemsNote";
        _itemsNote.Padding = new Padding(4, 0, 0, 0);
        _itemsNote.Text = "Items shown with ✔ are equipped. Display is read-only for now.";
        _itemsNote.TextAlign = ContentAlignment.MiddleLeft;

        _applyItemsBtn.BackColor = Color.FromArgb(45, 45, 60);
        _applyItemsBtn.Dock = DockStyle.Bottom;
        _applyItemsBtn.Enabled = false;
        _applyItemsBtn.FlatStyle = FlatStyle.Flat;
        _applyItemsBtn.Font = new Font("Segoe UI", 9F);
        _applyItemsBtn.ForeColor = Color.Gray;
        _applyItemsBtn.Height = 28;
        _applyItemsBtn.Name = "_applyItemsBtn";
        _applyItemsBtn.Text = "Items are read-only (display only)";

        // Fill first, then Bottom (note, button) added last so they dock first.
        _itemsTab.Controls.Add(_itemsGrid);
        _itemsTab.Controls.Add(_applyItemsBtn);
        _itemsTab.Controls.Add(_itemsNote);
        _itemsTab.Name = "_itemsTab";
        _itemsTab.Text = "Items";

        // ── Bag tab ───────────────────────────────────────────────────────────
        _bagSlotCol.HeaderText = "#";
        _bagSlotCol.Name = "Slot";
        _bagSlotCol.ReadOnly = true;
        _bagSlotCol.Width = 35;

        _bagNameCol.HeaderText = "Item";
        _bagNameCol.Name = "ItemName";
        _bagNameCol.ReadOnly = true;
        _bagNameCol.Width = 220;

        _bagIdCol.HeaderText = "ID";
        _bagIdCol.Name = "IdHex";
        _bagIdCol.ReadOnly = true;
        _bagIdCol.Width = 55;

        _bagQtyCol.HeaderText = "Qty";
        _bagQtyCol.Name = "Qty";
        _bagQtyCol.Width = 45;

        _bagGrid.AllowUserToAddRows = false;
        _bagGrid.AllowUserToDeleteRows = false;
        _bagGrid.ReadOnly = true;   // read-only display for now
        _bagGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        _bagGrid.Columns.AddRange(new DataGridViewColumn[] {
            _bagSlotCol, _bagNameCol, _bagIdCol, _bagQtyCol });
        _bagGrid.Dock = DockStyle.Fill;
        _bagGrid.Name = "_bagGrid";
        _bagGrid.RowHeadersVisible = false;
        _bagGrid.TabIndex = 0;
        _bagGrid.DataError += new DataGridViewDataErrorEventHandler(Grid_DataError);

        _applyBagBtn.BackColor = Color.FromArgb(45, 45, 60);
        _applyBagBtn.Dock = DockStyle.Bottom;
        _applyBagBtn.Enabled = false;
        _applyBagBtn.FlatStyle = FlatStyle.Flat;
        _applyBagBtn.Font = new Font("Segoe UI", 9F);
        _applyBagBtn.ForeColor = Color.Gray;
        _applyBagBtn.Height = 28;
        _applyBagBtn.Name = "_applyBagBtn";
        _applyBagBtn.Text = "Party bag is read-only (display only)";

        _bagTab.Controls.Add(_bagGrid);
        _bagTab.Controls.Add(_applyBagBtn);
        _bagTab.Name = "_bagTab";
        _bagTab.Text = "Party Bag";

        // ── Tab control (Split.Panel2) ─────────────────────────────────────────
        _tabs.Controls.AddRange(new Control[] { _statsTab, _itemsTab, _bagTab });
        _tabs.Dock = DockStyle.Fill;
        _tabs.Name = "_tabs";
        _tabs.SelectedIndex = 0;
        _tabs.TabIndex = 0;
        _split.Panel2.Controls.Add(_tabs);

        // ── Split container ────────────────────────────────────────────────────
        _split.Dock = DockStyle.Fill;
        _split.Name = "_split";
        _split.SplitterDistance = 220;
        _split.SplitterWidth = 4;
        _split.TabIndex = 2;

        // ── Status strip ───────────────────────────────────────────────────────
        _statusLabel.Name = "_statusLabel";
        _statusLabel.Text = "No file loaded.";
        _status.Items.AddRange(new ToolStripItem[] { _statusLabel });
        _status.Location = new Point(0, 654);
        _status.Name = "_status";
        _status.Size = new Size(1000, 22);
        _status.TabIndex = 3;

        // ── Form ──────────────────────────────────────────────────────────────
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 680);
        Controls.Add(_split);
        Controls.Add(_topBar);
        Controls.Add(_menu);
        Controls.Add(_status);
        MainMenuStrip = _menu;
        MinimumSize = new Size(820, 560);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "DQ5 DS Save Editor";

        // ── Resume layouts ──────────────────────────────────────────────────
        _menu.ResumeLayout(false);
        _menu.PerformLayout();
        _topBar.ResumeLayout(false);
        _topBar.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_goldField).EndInit();
        _split.Panel1.ResumeLayout(false);
        _split.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_split).EndInit();
        _split.ResumeLayout(false);
        _tabs.ResumeLayout(false);
        _statsTab.ResumeLayout(false);
        _itemsTab.ResumeLayout(false);
        _bagTab.ResumeLayout(false);
        _bottomStrip.ResumeLayout(false);
        _quickBtnFlow.ResumeLayout(false);
        _statsRows.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_levelField).EndInit();
        ((System.ComponentModel.ISupportInitialize)_strField).EndInit();
        ((System.ComponentModel.ISupportInitialize)_resField).EndInit();
        ((System.ComponentModel.ISupportInitialize)_aglField).EndInit();
        ((System.ComponentModel.ISupportInitialize)_wisField).EndInit();
        ((System.ComponentModel.ISupportInitialize)_lckField).EndInit();
        ((System.ComponentModel.ISupportInitialize)_expField).EndInit();
        ((System.ComponentModel.ISupportInitialize)_hpCurField).EndInit();
        ((System.ComponentModel.ISupportInitialize)_hpMaxField).EndInit();
        ((System.ComponentModel.ISupportInitialize)_mpCurField).EndInit();
        ((System.ComponentModel.ISupportInitialize)_mpMaxField).EndInit();
        ((System.ComponentModel.ISupportInitialize)_itemsGrid).EndInit();
        ((System.ComponentModel.ISupportInitialize)_bagGrid).EndInit();
        _status.ResumeLayout(false);
        _status.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    /// <summary>
    /// Builds one stat row: a 32 px Panel with the NumericUpDown docked Right
    /// and the Label filling the remaining left space.  No TableLayoutPanel needed.
    /// </summary>
    private static void BuildStatRow(Panel row, Label lbl, NumericUpDown field, string text)
    {
        // Label
        lbl.AutoSize = false;
        lbl.BackColor = Color.FromArgb(25, 25, 35);
        lbl.Dock = DockStyle.Fill;
        lbl.ForeColor = Color.LightGray;
        lbl.Margin = new Padding(0);
        lbl.Text = text;
        lbl.TextAlign = ContentAlignment.MiddleRight;

        // NumericUpDown
        field.Dock = DockStyle.Right;
        field.Font = new Font("Segoe UI", 10F);
        field.Margin = new Padding(0);
        field.Width = 135;

        // Row panel — add field first (Right), then label (Fill)
        row.BackColor = Color.FromArgb(25, 25, 35);
        row.Dock = DockStyle.Top;
        row.Height = 32;
        row.Margin = new Padding(0);
        row.Padding = new Padding(0, 2, 0, 2);
        row.Controls.Add(lbl);
        row.Controls.Add(field);
    }

    #endregion

    // ── Control fields ────────────────────────────────────────────────────────
    private MenuStrip _menu = null!;
    private ToolStripMenuItem _fileMenuItem = null!;
    private ToolStripMenuItem _openMenuItem = null!;
    private ToolStripMenuItem _saveMenuItem = null!;
    private ToolStripMenuItem _saveAsMenuItem = null!;
    private ToolStripSeparator _menuSep = null!;
    private ToolStripMenuItem _exitMenuItem = null!;

    private Panel _topBar = null!;
    private Label _goldLabel = null!;
    private NumericUpDown _goldField = null!;
    private Button _applyGoldBtn = null!;

    private SplitContainer _split = null!;
    private Label _listHeader = null!;
    private ListBox _charList = null!;

    private TabControl _tabs = null!;
    private TabPage _statsTab = null!, _itemsTab = null!, _bagTab = null!;

    private Label _charNameLabel = null!;
    private Panel _bottomStrip = null!;
    private FlowLayoutPanel _quickBtnFlow = null!;
    private Button _maxStatsBtn = null!, _maxExpBtn = null!, _fullHpMpBtn = null!;
    private Button _applyStatsBtn = null!;
    private Panel _statsRows = null!;
    private Panel _rowLevel = null!, _rowStr  = null!, _rowRes  = null!;
    private Panel _rowAgl   = null!, _rowWis  = null!, _rowLck  = null!;
    private Panel _rowExp   = null!, _rowHpCur= null!, _rowHpMax= null!;
    private Panel _rowMpCur = null!, _rowMpMax= null!;

    private Label _levelLbl = null!, _strLbl = null!, _resLbl = null!, _aglLbl = null!;
    private Label _wisLbl = null!, _lckLbl = null!, _expLbl = null!;
    private Label _hpCurLbl = null!, _hpMaxLbl = null!, _mpCurLbl = null!, _mpMaxLbl = null!;

    private NumericUpDown _levelField = null!;
    private NumericUpDown _strField = null!, _resField = null!, _aglField = null!;
    private NumericUpDown _wisField = null!, _lckField = null!, _expField = null!;
    private NumericUpDown _hpCurField = null!, _hpMaxField = null!;
    private NumericUpDown _mpCurField = null!, _mpMaxField = null!;

    private DataGridView _itemsGrid = null!;
    private DataGridViewTextBoxColumn _itemsSlotCol = null!;
    private DataGridViewTextBoxColumn _itemsNameCol = null!;
    private DataGridViewTextBoxColumn _itemsIdCol = null!;
    private DataGridViewTextBoxColumn _itemsEquipCol = null!;
    private Label _itemsNote = null!;
    private Button _applyItemsBtn = null!;

    private DataGridView _bagGrid = null!;
    private DataGridViewTextBoxColumn _bagSlotCol = null!;
    private DataGridViewTextBoxColumn _bagNameCol = null!;
    private DataGridViewTextBoxColumn _bagIdCol = null!;
    private DataGridViewTextBoxColumn _bagQtyCol = null!;
    private Button _applyBagBtn = null!;

    private StatusStrip _status = null!;
    private ToolStripStatusLabel _statusLabel = null!;
}
