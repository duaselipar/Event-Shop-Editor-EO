namespace EventShopEditorEO
{
    partial class NewItemForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Controls declaration
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.DataGridView dgvSearch;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;

        // RadioButton controls
        private System.Windows.Forms.GroupBox groupShopType;
        private System.Windows.Forms.RadioButton rbAstra;
        private System.Windows.Forms.RadioButton rbHonor;
        private System.Windows.Forms.RadioButton rbPlane;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtSearch = new TextBox();
            dgvSearch = new DataGridView();
            btnAdd = new Button();
            btnCancel = new Button();
            groupBox1 = new GroupBox();
            groupShopType = new GroupBox();
            rbAstra = new RadioButton();
            rbHonor = new RadioButton();
            rbPlane = new RadioButton();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dgvSearch).BeginInit();
            groupBox1.SuspendLayout();
            groupShopType.SuspendLayout();
            SuspendLayout();
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(12, 12);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "Search item id/name...";
            txtSearch.Size = new Size(333, 23);
            txtSearch.TabIndex = 1;
            // 
            // dgvSearch
            // 
            dgvSearch.AllowUserToAddRows = false;
            dgvSearch.AllowUserToDeleteRows = false;
            dgvSearch.AllowUserToResizeColumns = false;
            dgvSearch.AllowUserToResizeRows = false;
            dgvSearch.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvSearch.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2 });
            dgvSearch.Location = new Point(6, 22);
            dgvSearch.MultiSelect = false;
            dgvSearch.Name = "dgvSearch";
            dgvSearch.ReadOnly = true;
            dgvSearch.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dgvSearch.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSearch.Size = new Size(319, 484);
            dgvSearch.TabIndex = 2;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(12, 620);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(80, 27);
            btnAdd.TabIndex = 3;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(98, 620);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(80, 27);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(dgvSearch);
            groupBox1.Location = new Point(12, 97);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(333, 512);
            groupBox1.TabIndex = 5;
            groupBox1.TabStop = false;
            groupBox1.Text = "Select Item (Double Click To Select)";
            // 
            // groupShopType
            // 
            groupShopType.Controls.Add(rbAstra);
            groupShopType.Controls.Add(rbHonor);
            groupShopType.Controls.Add(rbPlane);
            groupShopType.Location = new Point(12, 41);
            groupShopType.Name = "groupShopType";
            groupShopType.Size = new Size(333, 50);
            groupShopType.TabIndex = 6;
            groupShopType.TabStop = false;
            groupShopType.Text = "Shop Type";
            // 
            // rbAstra
            // 
            rbAstra.AutoSize = true;
            rbAstra.Checked = true;
            rbAstra.Location = new Point(22, 20);
            rbAstra.Name = "rbAstra";
            rbAstra.Size = new Size(82, 19);
            rbAstra.TabIndex = 0;
            rbAstra.TabStop = true;
            rbAstra.Tag = "1";
            rbAstra.Text = "Astra Shop";
            // 
            // rbHonor
            // 
            rbHonor.AutoSize = true;
            rbHonor.Location = new Point(119, 20);
            rbHonor.Name = "rbHonor";
            rbHonor.Size = new Size(89, 19);
            rbHonor.TabIndex = 1;
            rbHonor.Tag = "2";
            rbHonor.Text = "Honor Shop";
            // 
            // rbPlane
            // 
            rbPlane.AutoSize = true;
            rbPlane.Location = new Point(227, 20);
            rbPlane.Name = "rbPlane";
            rbPlane.Size = new Size(84, 19);
            rbPlane.TabIndex = 2;
            rbPlane.Tag = "4";
            rbPlane.Text = "Plane Shop";
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.HeaderText = "Item ID";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            dataGridViewTextBoxColumn1.Width = 80;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.HeaderText = "Name";
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.ReadOnly = true;
            dataGridViewTextBoxColumn2.Width = 175;
            // 
            // NewItemForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(355, 660);
            Controls.Add(groupBox1);
            Controls.Add(groupShopType);
            Controls.Add(btnCancel);
            Controls.Add(btnAdd);
            Controls.Add(txtSearch);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "NewItemForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Add New Shop Item";
            ((System.ComponentModel.ISupportInitialize)dgvSearch).EndInit();
            groupBox1.ResumeLayout(false);
            groupShopType.ResumeLayout(false);
            groupShopType.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    }
}
