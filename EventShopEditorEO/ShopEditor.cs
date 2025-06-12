using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EventShopEditorEO
{
    public partial class ShopEditor : Form
    {
        public string MySqlConnString { get; private set; } = string.Empty;
        public MySqlConnection MySqlConn { get; private set; }

        private readonly string iniFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "setting.ini");
        private Dictionary<string, string> iniData = new();

        private bool isConnected = false;

        // Drag & drop vars (Astra shop)
        private int rowIndexFromMouseDown;
        private DataGridViewRow draggedRow;

        // ContextMenu untuk right-click
        private ContextMenuStrip astraMenu;

        private DataGridViewRow _lastRowAstra = null;
        private DataGridViewRow _lastRowHonor = null;
        private DataGridViewRow _lastRowPlane = null;

        public ShopEditor()
        {
            InitializeComponent();
            RegisterNumberOnlyHandlers();
            btncnt.Click += btncnt_Click;
            btnclntpath.Click += btnclntpath_Click;

            // Auto-load config
            LoadSettings();

            // === DRAG & DROP SETUP (Astra) ===
            dgvAstra.AllowDrop = true;
            dgvAstra.MouseDown += dgvAstra_MouseDown;
            dgvAstra.MouseMove += dgvAstra_MouseMove;
            dgvAstra.DragOver += dgvAstra_DragOver;
            dgvAstra.DragDrop += dgvAstra_DragDrop;

            // === CONTEXT MENU (Astra) ===
            astraMenu = new ContextMenuStrip();
            astraMenu.Items.Add("Move to Top", null, MoveRowToTop_Click);
            astraMenu.Items.Add("Move to Bottom", null, MoveRowToBottom_Click);
            astraMenu.Items.Add("Delete", null, DeleteRowAstra_Click);   // <-- Tambah sini

            dgvAstra.ContextMenuStrip = astraMenu;

            dgvAstra.CellMouseDown += dgvAstra_CellMouseDown;

            // === DRAG & DROP SETUP (Honor) ===
            dgvHonor.AllowDrop = true;
            dgvHonor.MouseDown += dgvHonor_MouseDown;
            dgvHonor.MouseMove += dgvHonor_MouseMove;
            dgvHonor.DragOver += dgvHonor_DragOver;
            dgvHonor.DragDrop += dgvHonor_DragDrop;

            // === CONTEXT MENU (Honor) ===
            var honorMenu = new ContextMenuStrip();
            honorMenu.Items.Add("Move to Top", null, MoveRowToTopHonor_Click);
            honorMenu.Items.Add("Move to Bottom", null, MoveRowToBottomHonor_Click);
            honorMenu.Items.Add("Delete", null, DeleteRowHonor_Click);  // <-- Tambah sini

            dgvHonor.ContextMenuStrip = honorMenu;

            dgvHonor.CellMouseDown += dgvHonor_CellMouseDown;

            // === DRAG & DROP SETUP (Plane) ===
            dgvPlane.AllowDrop = true;
            dgvPlane.MouseDown += dgvPlane_MouseDown;
            dgvPlane.MouseMove += dgvPlane_MouseMove;
            dgvPlane.DragOver += dgvPlane_DragOver;
            dgvPlane.DragDrop += dgvPlane_DragDrop;

            // === CONTEXT MENU (Plane) ===
            var planeMenu = new ContextMenuStrip();
            planeMenu.Items.Add("Move to Top", null, MoveRowToTopPlane_Click);
            planeMenu.Items.Add("Move to Bottom", null, MoveRowToBottomPlane_Click);
            planeMenu.Items.Add("Delete", null, DeleteRowPlane_Click);  // <-- Tambah sini

            dgvPlane.ContextMenuStrip = planeMenu;

            dgvPlane.CellMouseDown += dgvPlane_CellMouseDown;

            // Disable sorting semua grid
            foreach (DataGridViewColumn col in dgvAstra.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            foreach (DataGridViewColumn col in dgvHonor.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            foreach (DataGridViewColumn col in dgvPlane.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;


            dgvAstra.SelectionChanged += dgvAstra_SelectionChanged;
            dgvHonor.SelectionChanged += dgvHonor_SelectionChanged;
            dgvPlane.SelectionChanged += dgvPlane_SelectionChanged;

            btnnew.Click += btnnew_Click;

        }

        private void LoadSettings()
        {
            if (!File.Exists(iniFile))
            {
                File.WriteAllText(iniFile,
@"[database]
hostname=localhost
port=3306
user=test
password=test123
database=newdb1
clientpath=
");
            }

            iniData.Clear();
            string? section = null;
            foreach (var line in File.ReadAllLines(iniFile))
            {
                var l = line.Trim();
                if (string.IsNullOrEmpty(l) || l.StartsWith(";") || l.StartsWith("#")) continue;
                if (l.StartsWith("[") && l.EndsWith("]"))
                {
                    section = l.Trim('[', ']');
                    continue;
                }
                if (section == "database" && l.Contains('='))
                {
                    var idx = l.IndexOf('=');
                    var key = l.Substring(0, idx).Trim();
                    var val = l[(idx + 1)..].Trim();
                    iniData[key.ToLower()] = val;
                }
            }

            txthost.Text = iniData.GetValueOrDefault("hostname", "");
            txtport.Text = iniData.GetValueOrDefault("port", "3306");
            txtuser.Text = iniData.GetValueOrDefault("user", "");
            txtpass.Text = iniData.GetValueOrDefault("password", "");
            txtdb.Text = iniData.GetValueOrDefault("database", "");
            txtclientpath.Text = iniData.GetValueOrDefault("clientpath", "");
        }

        private void SaveSettings()
        {
            iniData["hostname"] = txthost.Text;
            iniData["port"] = txtport.Text;
            iniData["user"] = txtuser.Text;
            iniData["password"] = txtpass.Text;
            iniData["database"] = txtdb.Text;
            iniData["clientpath"] = txtclientpath.Text;

            using (var sw = new StreamWriter(iniFile, false))
            {
                sw.WriteLine("[database]");
                sw.WriteLine($"hostname={iniData["hostname"]}");
                sw.WriteLine($"port={iniData["port"]}");
                sw.WriteLine($"user={iniData["user"]}");
                sw.WriteLine($"password={iniData["password"]}");
                sw.WriteLine($"database={iniData["database"]}");
                sw.WriteLine($"clientpath={iniData["clientpath"]}");
            }
        }

        private void btnclntpath_Click(object? sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Select your client folder";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtclientpath.Text = dlg.SelectedPath;

                    string iniShopPath = FindActivityNewShopIni();
                    if (iniShopPath == null)
                    {
                        MessageBox.Show("activitynewshop.ini not found in the 'ini' folder!");
                        return;
                    }
                }
            }
        }

        private string? FindActivityNewShopIni()
        {
            string iniFolder = Path.Combine(txtclientpath.Text, "ini");
            if (!Directory.Exists(iniFolder)) return null;

            var files = Directory.GetFiles(iniFolder, "*.ini", SearchOption.TopDirectoryOnly);
            return files.FirstOrDefault(f => Path.GetFileName(f).Equals("activitynewshop.ini", StringComparison.OrdinalIgnoreCase));
        }

        private void btncnt_Click(object? sender, EventArgs e)
        {
            if (!isConnected)
            {
                // Validate fields
                if (string.IsNullOrWhiteSpace(txthost.Text) ||
                    string.IsNullOrWhiteSpace(txtport.Text) ||
                    string.IsNullOrWhiteSpace(txtuser.Text) ||
                    string.IsNullOrWhiteSpace(txtdb.Text) ||
                    string.IsNullOrWhiteSpace(txtclientpath.Text))
                {
                    MessageBox.Show("Please fill all MySQL fields and select the client folder.", "Input Missing");
                    return;
                }

                // Check activitynewshop.ini exists (auto-detect dalam folder 'ini')
                string iniShopPath = FindActivityNewShopIni();
                if (iniShopPath == null)
                {
                    MessageBox.Show("activitynewshop.ini not found in the 'ini' folder of the selected client folder!");
                    return;
                }

                MySqlConnString = $"server={txthost.Text};port={txtport.Text};user={txtuser.Text};password={txtpass.Text};database={txtdb.Text};";
                try
                {
                    MySqlConn = new MySqlConnection(MySqlConnString);
                    MySqlConn.Open();

                    SaveSettings();

                    MessageBox.Show("MySQL Connected! Files loaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Lock field supaya user tak ubah
                    txthost.Enabled = txtport.Enabled = txtuser.Enabled = txtpass.Enabled = txtdb.Enabled = false;
                    txtclientpath.Enabled = btnclntpath.Enabled = false;

                    btncnt.Text = "Disconnect";
                    isConnected = true;

                    tabControl1.SelectedTab = tabPage2;

                    // Disable sorting untuk semua grid
                    foreach (DataGridViewColumn col in dgvAstra.Columns)
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    foreach (DataGridViewColumn col in dgvHonor.Columns)
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    foreach (DataGridViewColumn col in dgvPlane.Columns)
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;

                    LoadShopData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("MySQL Error: " + ex.Message, "MySQL Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MySqlConn?.Dispose();
                    MySqlConn = null;
                    isConnected = false;
                }
            }
            else
            {
                // Disconnect
                try
                {
                    MySqlConn?.Close();
                    MySqlConn?.Dispose();
                    MySqlConn = null;
                }
                catch { }

                txthost.Enabled = txtport.Enabled = txtuser.Enabled = txtpass.Enabled = txtdb.Enabled = true;
                txtclientpath.Enabled = btnclntpath.Enabled = true;

                btncnt.Text = "Connect";
                isConnected = false;
            }
        }

        private void LoadShopData()
        {
            string iniShopPath = FindActivityNewShopIni();
            if (iniShopPath == null)
            {
                MessageBox.Show("activitynewshop.ini not found!");
                return;
            }

            dgvAstra.Rows.Clear();
            dgvHonor.Rows.Clear();
            dgvPlane.Rows.Clear();

            foreach (var line in File.ReadAllLines(iniShopPath))
            {
                var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 8) continue;

                int shopType = int.Parse(parts[0]);
                string id = parts[2];
                string price = parts[7];

                string itemName = GetItemNameFromDB(id);

                if (shopType == 1)
                {
                    int idx = dgvAstra.Rows.Add(id, itemName, price);
                    dgvAstra.Rows[idx].Tag = parts; // Simpan full array dalam Tag
                }
                else if (shopType == 2)
                {
                    int idx = dgvHonor.Rows.Add(id, itemName, price);
                    dgvHonor.Rows[idx].Tag = parts;
                }
                else if (shopType == 4)
                {
                    int idx = dgvPlane.Rows.Add(id, itemName, price);
                    dgvPlane.Rows[idx].Tag = parts;
                }
            }
        }


        private string GetItemNameFromDB(string itemId)
        {
            string name = "";
            try
            {
                if (MySqlConn.State == System.Data.ConnectionState.Closed)
                    MySqlConn.Open();

                using (var cmd = new MySqlCommand("SELECT name FROM cq_itemtype WHERE id=@id", MySqlConn))
                {
                    cmd.Parameters.AddWithValue("@id", itemId);
                    var result = cmd.ExecuteScalar();
                    if (result != null) name = result.ToString();
                }
                MySqlConn.Close();
            }
            catch { }
            return name;
        }

        // ===== DRAG & DROP ROW UNTUK ASTRA SHOP =====

        private void dgvAstra_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var hti = dgvAstra.HitTest(e.X, e.Y);
                rowIndexFromMouseDown = hti.RowIndex;
                if (rowIndexFromMouseDown != -1)
                {
                    draggedRow = dgvAstra.Rows[rowIndexFromMouseDown];
                }
            }
        }

        private void dgvAstra_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left && draggedRow != null)
            {
                dgvAstra.DoDragDrop(draggedRow, DragDropEffects.Move);
            }
        }

        private void dgvAstra_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void dgvAstra_DragDrop(object sender, DragEventArgs e)
        {
            var clientPoint = dgvAstra.PointToClient(new Point(e.X, e.Y));
            var hit = dgvAstra.HitTest(clientPoint.X, clientPoint.Y);
            int targetRowIndex = hit.RowIndex;

            if (targetRowIndex >= 0 && rowIndexFromMouseDown != targetRowIndex)
            {
                DataGridViewRow rowToMove = (DataGridViewRow)e.Data.GetData(typeof(DataGridViewRow));
                dgvAstra.Rows.Remove(rowToMove);
                dgvAstra.Rows.Insert(targetRowIndex, rowToMove);
                rowToMove.Selected = true;
            }
            draggedRow = null;
        }

        // ===== CONTEXT MENU: MOVE TOP/BOTTOM UNTUK ASTRA SHOP =====

        private void dgvAstra_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Pastikan row yang diklik kanan akan jadi selected
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                dgvAstra.ClearSelection();
                dgvAstra.Rows[e.RowIndex].Selected = true;
            }
        }

        private void MoveRowToTop_Click(object sender, EventArgs e)
        {
            if (dgvAstra.SelectedRows.Count > 0)
            {
                var row = dgvAstra.SelectedRows[0];
                int idx = row.Index;
                if (idx > 0)
                {
                    dgvAstra.Rows.RemoveAt(idx);
                    dgvAstra.Rows.Insert(0, row);
                    row.Selected = true;
                }
            }
        }

        private void MoveRowToBottom_Click(object sender, EventArgs e)
        {
            if (dgvAstra.SelectedRows.Count > 0)
            {
                var row = dgvAstra.SelectedRows[0];
                int idx = row.Index;
                if (idx < dgvAstra.Rows.Count - 1)
                {
                    dgvAstra.Rows.RemoveAt(idx);
                    dgvAstra.Rows.Add(row);
                    row.Selected = true;
                }
            }
        }

        // ====== Tambah logic sama pada dgvHonor, dgvPlane kalau nak ======
        private int rowIndexFromMouseDownHonor;
        private DataGridViewRow draggedRowHonor;

        private void dgvHonor_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var hti = dgvHonor.HitTest(e.X, e.Y);
                rowIndexFromMouseDownHonor = hti.RowIndex;
                if (rowIndexFromMouseDownHonor != -1)
                {
                    draggedRowHonor = dgvHonor.Rows[rowIndexFromMouseDownHonor];
                }
            }
        }

        private void dgvHonor_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left && draggedRowHonor != null)
            {
                dgvHonor.DoDragDrop(draggedRowHonor, DragDropEffects.Move);
            }
        }

        private void dgvHonor_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void dgvHonor_DragDrop(object sender, DragEventArgs e)
        {
            var clientPoint = dgvHonor.PointToClient(new Point(e.X, e.Y));
            var hit = dgvHonor.HitTest(clientPoint.X, clientPoint.Y);
            int targetRowIndex = hit.RowIndex;

            if (targetRowIndex >= 0 && rowIndexFromMouseDownHonor != targetRowIndex)
            {
                DataGridViewRow rowToMove = (DataGridViewRow)e.Data.GetData(typeof(DataGridViewRow));
                dgvHonor.Rows.Remove(rowToMove);
                dgvHonor.Rows.Insert(targetRowIndex, rowToMove);
                rowToMove.Selected = true;
            }
            draggedRowHonor = null;
        }

        private void dgvHonor_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                dgvHonor.ClearSelection();
                dgvHonor.Rows[e.RowIndex].Selected = true;
            }
        }

        private void MoveRowToTopHonor_Click(object sender, EventArgs e)
        {
            if (dgvHonor.SelectedRows.Count > 0)
            {
                var row = dgvHonor.SelectedRows[0];
                int idx = row.Index;
                if (idx > 0)
                {
                    dgvHonor.Rows.RemoveAt(idx);
                    dgvHonor.Rows.Insert(0, row);
                    row.Selected = true;
                }
            }
        }

        private void MoveRowToBottomHonor_Click(object sender, EventArgs e)
        {
            if (dgvHonor.SelectedRows.Count > 0)
            {
                var row = dgvHonor.SelectedRows[0];
                int idx = row.Index;
                if (idx < dgvHonor.Rows.Count - 1)
                {
                    dgvHonor.Rows.RemoveAt(idx);
                    dgvHonor.Rows.Add(row);
                    row.Selected = true;
                }
            }
        }

        private int rowIndexFromMouseDownPlane;
        private DataGridViewRow draggedRowPlane;

        private void dgvPlane_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var hti = dgvPlane.HitTest(e.X, e.Y);
                rowIndexFromMouseDownPlane = hti.RowIndex;
                if (rowIndexFromMouseDownPlane != -1)
                {
                    draggedRowPlane = dgvPlane.Rows[rowIndexFromMouseDownPlane];
                }
            }
        }

        private void dgvPlane_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left && draggedRowPlane != null)
            {
                dgvPlane.DoDragDrop(draggedRowPlane, DragDropEffects.Move);
            }
        }

        private void dgvPlane_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void dgvPlane_DragDrop(object sender, DragEventArgs e)
        {
            var clientPoint = dgvPlane.PointToClient(new Point(e.X, e.Y));
            var hit = dgvPlane.HitTest(clientPoint.X, clientPoint.Y);
            int targetRowIndex = hit.RowIndex;

            if (targetRowIndex >= 0 && rowIndexFromMouseDownPlane != targetRowIndex)
            {
                DataGridViewRow rowToMove = (DataGridViewRow)e.Data.GetData(typeof(DataGridViewRow));
                dgvPlane.Rows.Remove(rowToMove);
                dgvPlane.Rows.Insert(targetRowIndex, rowToMove);
                rowToMove.Selected = true;
            }
            draggedRowPlane = null;
        }

        private void dgvPlane_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                dgvPlane.ClearSelection();
                dgvPlane.Rows[e.RowIndex].Selected = true;
            }
        }

        private void MoveRowToTopPlane_Click(object sender, EventArgs e)
        {
            if (dgvPlane.SelectedRows.Count > 0)
            {
                var row = dgvPlane.SelectedRows[0];
                int idx = row.Index;
                if (idx > 0)
                {
                    dgvPlane.Rows.RemoveAt(idx);
                    dgvPlane.Rows.Insert(0, row);
                    row.Selected = true;
                }
            }
        }

        private void MoveRowToBottomPlane_Click(object sender, EventArgs e)
        {
            if (dgvPlane.SelectedRows.Count > 0)
            {
                var row = dgvPlane.SelectedRows[0];
                int idx = row.Index;
                if (idx < dgvPlane.Rows.Count - 1)
                {
                    dgvPlane.Rows.RemoveAt(idx);
                    dgvPlane.Rows.Add(row);
                    row.Selected = true;
                }
            }
        }

        private void DeleteRowAstra_Click(object sender, EventArgs e)
        {
            if (dgvAstra.SelectedRows.Count > 0)
            {
                int idx = dgvAstra.SelectedRows[0].Index;
                dgvAstra.Rows.RemoveAt(idx);
            }
        }


        private void DeleteRowHonor_Click(object sender, EventArgs e)
        {
            if (dgvHonor.SelectedRows.Count > 0)
            {
                int idx = dgvHonor.SelectedRows[0].Index;
                dgvHonor.Rows.RemoveAt(idx);
            }
        }


        private void DeleteRowPlane_Click(object sender, EventArgs e)
        {
            if (dgvPlane.SelectedRows.Count > 0)
            {
                int idx = dgvPlane.SelectedRows[0].Index;
                dgvPlane.Rows.RemoveAt(idx);
            }
        }


        private void dgvAstra_SelectionChanged(object sender, EventArgs e)
        {
            // 1. Simpan value ke row lama (auto apply)
            if (_lastRowAstra != null && _lastRowAstra.Tag is string[] lastParts && lastParts.Length >= 16)
            {
                lastParts[3] = txtprity.Text;
                lastParts[4] = txtlvltyp.Text;
                lastParts[5] = txtnedlvl.Text;
                lastParts[6] = txtmnply.Text;
                lastParts[7] = txttc.Text;
                lastParts[8] = txteudcrncy.Text;
                lastParts[9] = txtitmcrcy.Text;
                lastParts[10] = txtamttyp.Text;
                lastParts[11] = txtamtlmt.Text;
                lastParts[12] = txtbgntm.Text;
                lastParts[13] = txtendtm.Text;
                lastParts[14] = txtnewtm.Text;
                lastParts[15] = txtvs.Text;

                _lastRowAstra.Cells[2].Value = lastParts[7]; // Kolum 2 = price (Talent_coin)

            }

            // 2. Load value row baru ke textbox
            if (dgvAstra.SelectedRows.Count == 0)
            {
                _lastRowAstra = null;
                return;
            }
            var row = dgvAstra.SelectedRows[0];
            if (row.Tag is not string[] parts) return;

            txtprity.Text = parts.Length > 3 ? parts[3] : "";
            txtlvltyp.Text = parts.Length > 4 ? parts[4] : "";
            txtnedlvl.Text = parts.Length > 5 ? parts[5] : "";
            txtmnply.Text = parts.Length > 6 ? parts[6] : "";
            txttc.Text = parts.Length > 7 ? parts[7] : "";
            txteudcrncy.Text = parts.Length > 8 ? parts[8] : "";
            txtitmcrcy.Text = parts.Length > 9 ? parts[9] : "";
            txtamttyp.Text = parts.Length > 10 ? parts[10] : "";
            txtamtlmt.Text = parts.Length > 11 ? parts[11] : "";
            txtbgntm.Text = parts.Length > 12 ? parts[12] : "";
            txtendtm.Text = parts.Length > 13 ? parts[13] : "";
            txtnewtm.Text = parts.Length > 14 ? parts[14] : "";
            txtvs.Text = parts.Length > 15 ? parts[15] : "";

            _lastRowAstra = row;
        }


        private void dgvHonor_SelectionChanged(object sender, EventArgs e)
        {
            if (_lastRowHonor != null && _lastRowHonor.Tag is string[] lastParts && lastParts.Length >= 16)
            {
                lastParts[3] = txtprity.Text;
                lastParts[4] = txtlvltyp.Text;
                lastParts[5] = txtnedlvl.Text;
                lastParts[6] = txtmnply.Text;
                lastParts[7] = txttc.Text;
                lastParts[8] = txteudcrncy.Text;
                lastParts[9] = txtitmcrcy.Text;
                lastParts[10] = txtamttyp.Text;
                lastParts[11] = txtamtlmt.Text;
                lastParts[12] = txtbgntm.Text;
                lastParts[13] = txtendtm.Text;
                lastParts[14] = txtnewtm.Text;
                lastParts[15] = txtvs.Text;

                _lastRowHonor.Cells[2].Value = lastParts[7]; // Kolum 2 = price (Talent_coin)

            }

            if (dgvHonor.SelectedRows.Count == 0)
            {
                _lastRowHonor = null;
                return;
            }
            var row = dgvHonor.SelectedRows[0];
            if (row.Tag is not string[] parts) return;

            txtprity.Text = parts.Length > 3 ? parts[3] : "";
            txtlvltyp.Text = parts.Length > 4 ? parts[4] : "";
            txtnedlvl.Text = parts.Length > 5 ? parts[5] : "";
            txtmnply.Text = parts.Length > 6 ? parts[6] : "";
            txttc.Text = parts.Length > 7 ? parts[7] : "";
            txteudcrncy.Text = parts.Length > 8 ? parts[8] : "";
            txtitmcrcy.Text = parts.Length > 9 ? parts[9] : "";
            txtamttyp.Text = parts.Length > 10 ? parts[10] : "";
            txtamtlmt.Text = parts.Length > 11 ? parts[11] : "";
            txtbgntm.Text = parts.Length > 12 ? parts[12] : "";
            txtendtm.Text = parts.Length > 13 ? parts[13] : "";
            txtnewtm.Text = parts.Length > 14 ? parts[14] : "";
            txtvs.Text = parts.Length > 15 ? parts[15] : "";

            _lastRowHonor = row;
        }


        private void dgvPlane_SelectionChanged(object sender, EventArgs e)
        {
            if (_lastRowPlane != null && _lastRowPlane.Tag is string[] lastParts && lastParts.Length >= 16)
            {
                lastParts[3] = txtprity.Text;
                lastParts[4] = txtlvltyp.Text;
                lastParts[5] = txtnedlvl.Text;
                lastParts[6] = txtmnply.Text;
                lastParts[7] = txttc.Text;
                lastParts[8] = txteudcrncy.Text;
                lastParts[9] = txtitmcrcy.Text;
                lastParts[10] = txtamttyp.Text;
                lastParts[11] = txtamtlmt.Text;
                lastParts[12] = txtbgntm.Text;
                lastParts[13] = txtendtm.Text;
                lastParts[14] = txtnewtm.Text;
                lastParts[15] = txtvs.Text;

                _lastRowPlane.Cells[2].Value = lastParts[7]; // Kolum 2 = price (Talent_coin)

            }

            if (dgvPlane.SelectedRows.Count == 0)
            {
                _lastRowPlane = null;
                return;
            }
            var row = dgvPlane.SelectedRows[0];
            if (row.Tag is not string[] parts) return;

            txtprity.Text = parts.Length > 3 ? parts[3] : "";
            txtlvltyp.Text = parts.Length > 4 ? parts[4] : "";
            txtnedlvl.Text = parts.Length > 5 ? parts[5] : "";
            txtmnply.Text = parts.Length > 6 ? parts[6] : "";
            txttc.Text = parts.Length > 7 ? parts[7] : "";
            txteudcrncy.Text = parts.Length > 8 ? parts[8] : "";
            txtitmcrcy.Text = parts.Length > 9 ? parts[9] : "";
            txtamttyp.Text = parts.Length > 10 ? parts[10] : "";
            txtamtlmt.Text = parts.Length > 11 ? parts[11] : "";
            txtbgntm.Text = parts.Length > 12 ? parts[12] : "";
            txtendtm.Text = parts.Length > 13 ? parts[13] : "";
            txtnewtm.Text = parts.Length > 14 ? parts[14] : "";
            txtvs.Text = parts.Length > 15 ? parts[15] : "";

            _lastRowPlane = row;
        }


        private void btnsave_Click(object sender, EventArgs e)
        {
            string iniShopPath = FindActivityNewShopIni();
            if (iniShopPath == null)
            {
                MessageBox.Show("activitynewshop.ini not found!");
                return;
            }

            var lines = new List<string>();

            // Gabung semua grid, id berterusan
            int idx = 1;
            foreach (DataGridViewRow row in dgvAstra.Rows)
            {
                if (row.Tag is not string[] parts || parts.Length < 16) continue;
                parts[1] = idx++.ToString();
                lines.Add(string.Join(" ", parts));
            }
            foreach (DataGridViewRow row in dgvHonor.Rows)
            {
                if (row.Tag is not string[] parts || parts.Length < 16) continue;
                parts[1] = idx++.ToString();
                lines.Add(string.Join(" ", parts));
            }
            foreach (DataGridViewRow row in dgvPlane.Rows)
            {
                if (row.Tag is not string[] parts || parts.Length < 16) continue;
                parts[1] = idx++.ToString();
                lines.Add(string.Join(" ", parts));
            }

            // 1. Save ke file dulu
            File.WriteAllLines(iniShopPath, lines, Encoding.UTF8);

            // 2. Update ke database
            try
            {
                if (MySqlConn.State == System.Data.ConnectionState.Closed)
                    MySqlConn.Open();

                // Truncate dulu
                using (var cmd = new MySqlCommand("TRUNCATE TABLE cq_activitynewshop", MySqlConn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Masukkan data satu-satu
                foreach (var line in lines)
                {
                    var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length < 16) continue;

                    // Tulis INSERT ikut column dalam cq_activitynewshop
                    // Column dan order contoh: shop_type, id, itemtype, priority, level_type, need_level, monopoly, talent_coin, eudemon_currency, item_currency, amount_type, amount_limit, begin_time, end_time, new_time, version, ... (kalau ada lebih column, tambah)
                    string sql = @"INSERT INTO cq_activitynewshop 
                (shop_type, id, itemtype, priority, level_type, need_level, monopoly, talent_coin, eudemon_currency, item_currency, amount_type, amount_limit, begin_time, end_time, new_time, version) 
                VALUES (@shop_type, @id, @itemtype, @priority, @level_type, @need_level, @monopoly, @talent_coin, @eudemon_currency, @item_currency, @amount_type, @amount_limit, @begin_time, @end_time, @new_time, @version)";

                    using (var cmd = new MySqlCommand(sql, MySqlConn))
                    {
                        cmd.Parameters.AddWithValue("@shop_type", parts[0]);
                        cmd.Parameters.AddWithValue("@id", parts[1]);
                        cmd.Parameters.AddWithValue("@itemtype", parts[2]);
                        cmd.Parameters.AddWithValue("@priority", parts[3]);
                        cmd.Parameters.AddWithValue("@level_type", parts[4]);
                        cmd.Parameters.AddWithValue("@need_level", parts[5]);
                        cmd.Parameters.AddWithValue("@monopoly", parts[6]);
                        cmd.Parameters.AddWithValue("@talent_coin", parts[7]);
                        cmd.Parameters.AddWithValue("@eudemon_currency", parts[8]);
                        cmd.Parameters.AddWithValue("@item_currency", parts[9]);
                        cmd.Parameters.AddWithValue("@amount_type", parts[10]);
                        cmd.Parameters.AddWithValue("@amount_limit", parts[11]);
                        cmd.Parameters.AddWithValue("@begin_time", parts[12]);
                        cmd.Parameters.AddWithValue("@end_time", parts[13]);
                        cmd.Parameters.AddWithValue("@new_time", parts[14]);
                        cmd.Parameters.AddWithValue("@version", parts[15]);
                        cmd.ExecuteNonQuery();
                    }
                }

                MySqlConn.Close();
                MessageBox.Show("Saved! All data written to file and database (table truncated & refreshed).", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show("MySQL update error: " + ex.Message, "DB ERROR");
            }
        }


        private void btnnew_Click(object sender, EventArgs e)
        {
            if (MySqlConn.State == ConnectionState.Closed) MySqlConn.Open();
            using (var frm = new NewItemForm(MySqlConn))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    // === Compose default parts
                    string[] parts = new string[16];
                    parts[0] = frm.SelectedShopType;
                    // parts[1] (id) akan di-assign masa save/reindex
                    parts[2] = frm.SelectedItemId;
                    parts[3] = "0"; // priority default
                    parts[4] = "0"; // level_type default
                    parts[5] = "0"; // need_level default
                    parts[6] = frm.InputMonopoly;
                    parts[7] = frm.InputPrice;
                    parts[8] = "0"; // eudemon_currency
                    parts[9] = "0"; // item_currency
                    parts[10] = "0"; // amount_type
                    parts[11] = "0"; // amount_limit
                    parts[12] = "0"; // begin_time
                    parts[13] = "0"; // end_time
                    parts[14] = "0"; // new_time
                    parts[15] = frm.InputVersion;

                    // === Add ke grid pilihan shop
                    if (frm.SelectedShopType == "1")
                    {
                        int idx = dgvAstra.Rows.Add(parts[2], frm.SelectedItemName, parts[7]);
                        dgvAstra.Rows[idx].Tag = parts;
                        dgvAstra.Rows[idx].Selected = true;
                    }
                    else if (frm.SelectedShopType == "2")
                    {
                        int idx = dgvHonor.Rows.Add(parts[2], frm.SelectedItemName, parts[7]);
                        dgvHonor.Rows[idx].Tag = parts;
                        dgvHonor.Rows[idx].Selected = true;
                    }
                    else if (frm.SelectedShopType == "4")
                    {
                        int idx = dgvPlane.Rows.Add(parts[2], frm.SelectedItemName, parts[7]);
                        dgvPlane.Rows[idx].Tag = parts;
                        dgvPlane.Rows[idx].Selected = true;
                    }
                }
            }
            MySqlConn.Close();
        }


        // 1. Function ini letak dalam class ShopEditor (atau mana-mana form kau guna textbox tu)
        private void txtOnlyNumbers_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Benarkan nombor & control keys (macam backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Block selain nombor
            }
        }

        // 2. Assign ke SEMUA kotak angka (letak dalam constructor / selepas InitializeComponent())
        private void RegisterNumberOnlyHandlers()
        {
            txtprity.KeyPress += txtOnlyNumbers_KeyPress;
            txtlvltyp.KeyPress += txtOnlyNumbers_KeyPress;
            txtnedlvl.KeyPress += txtOnlyNumbers_KeyPress;
            txtmnply.KeyPress += txtOnlyNumbers_KeyPress;
            txttc.KeyPress += txtOnlyNumbers_KeyPress;
            txteudcrncy.KeyPress += txtOnlyNumbers_KeyPress;
            txtitmcrcy.KeyPress += txtOnlyNumbers_KeyPress;
            txtamttyp.KeyPress += txtOnlyNumbers_KeyPress;
            txtamtlmt.KeyPress += txtOnlyNumbers_KeyPress;
            txtbgntm.KeyPress += txtOnlyNumbers_KeyPress;
            txtendtm.KeyPress += txtOnlyNumbers_KeyPress;
            txtnewtm.KeyPress += txtOnlyNumbers_KeyPress;
            txtvs.KeyPress += txtOnlyNumbers_KeyPress;
            // Tambah kotak lain kalau ada...
        }

    }




}
