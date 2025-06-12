using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace EventShopEditorEO
{
    public partial class NewItemForm : Form
    {
        public string SelectedShopType { get; private set; } // "1", "2", "4"
        public string SelectedItemId { get; private set; }
        public string SelectedItemName { get; private set; }
        public string InputPrice { get; private set; } = "100";
        public string InputMonopoly { get; private set; } = "1";
        public string InputVersion { get; private set; } = "49213";

        private MySqlConnection conn;

        public NewItemForm(MySqlConnection conn)
        {
            InitializeComponent();
            this.conn = conn;

            // === Search function ===
            txtSearch.TextChanged += TxtSearch_TextChanged;

            // === Double-click add ===
            dgvSearch.CellDoubleClick += DgvSearch_CellDoubleClick;

            // Add & Cancel button
            btnAdd.Click += BtnAdd_Click;
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            // Ensure columns always present (for manual instantiate)
            if (dgvSearch.Columns.Count == 0)
            {
                dgvSearch.Columns.Add("ItemID", "Item ID");
                dgvSearch.Columns.Add("Name", "Name");
            }
        }

        // ===== RADIO BUTTON LOGIC =====
        private string GetSelectedShopType()
        {
            if (rbAstra.Checked) return "1";
            if (rbHonor.Checked) return "2";
            if (rbPlane.Checked) return "4";
            return "1"; // default Astra
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            dgvSearch.Rows.Clear();
            // Safety: Add columns if missing (for runtime add)
            if (dgvSearch.Columns.Count == 0)
            {
                dgvSearch.Columns.Add("ItemID", "Item ID");
                dgvSearch.Columns.Add("Name", "Name");
            }

            if (string.IsNullOrWhiteSpace(txtSearch.Text)) return;
            string sql = "SELECT id, name FROM cq_itemtype WHERE LOWER(id) LIKE LOWER(@kw) OR LOWER(name) LIKE LOWER(@kw) Limit 300";
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@kw", "%" + txtSearch.Text + "%");
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        dgvSearch.Rows.Add(reader[0].ToString(), reader[1].ToString());
                }
            }
        }

        private void DgvSearch_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                SelectedShopType = GetSelectedShopType();
                SelectedItemId = dgvSearch.Rows[e.RowIndex].Cells[0].Value?.ToString();
                SelectedItemName = dgvSearch.Rows[e.RowIndex].Cells[1].Value?.ToString();
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (dgvSearch.SelectedRows.Count > 0)
            {
                SelectedShopType = GetSelectedShopType();
                SelectedItemId = dgvSearch.SelectedRows[0].Cells[0].Value?.ToString();
                SelectedItemName = dgvSearch.SelectedRows[0].Cells[1].Value?.ToString();
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Please select an item from the list.");
            }
        }

        // (Optional) Kalau nak auto-load all items on form load, call this in constructor:
        // private void LoadItemList(string filter = "")
        // {
        //     dgvSearch.Rows.Clear();
        //     if (dgvSearch.Columns.Count == 0)
        //     {
        //         dgvSearch.Columns.Add("ItemID", "Item ID");
        //         dgvSearch.Columns.Add("Name", "Name");
        //     }
        //     string sql;
        //     if (string.IsNullOrWhiteSpace(filter))
        //         sql = "SELECT id, name FROM cq_itemtype LIMIT 100";
        //     else
        //         sql = "SELECT id, name FROM cq_itemtype WHERE LOWER(id) LIKE LOWER(@kw) OR LOWER(name) LIKE LOWER(@kw) LIMIT 100";
        //     using (var cmd = new MySqlCommand(sql, conn))
        //     {
        //         if (!string.IsNullOrWhiteSpace(filter))
        //             cmd.Parameters.AddWithValue("@kw", "%" + filter + "%");
        //         using (var reader = cmd.ExecuteReader())
        //         {
        //             while (reader.Read())
        //                 dgvSearch.Rows.Add(reader[0].ToString(), reader[1].ToString());
        //         }
        //     }
        // }
    }
}
