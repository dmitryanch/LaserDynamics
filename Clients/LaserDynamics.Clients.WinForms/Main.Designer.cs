namespace LaserDynamics.Clients.WinForms
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.FileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CreateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CloseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calculationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.CalculationsTabControl = new System.Windows.Forms.TabControl();
            this.PlusTabPage = new System.Windows.Forms.TabPage();
            this.MainTable = new System.Windows.Forms.TableLayoutPanel();
            this.RightMainTable = new System.Windows.Forms.TableLayoutPanel();
            this.LeftMainTable = new System.Windows.Forms.TableLayoutPanel();
            this.ErrorToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.ExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RenameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenu.SuspendLayout();
            this.CalculationsTabControl.SuspendLayout();
            this.MainTable.SuspendLayout();
            this.RightMainTable.SuspendLayout();
            this.LeftMainTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenuItem,
            this.EditMenuItem,
            this.calculationToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(1169, 24);
            this.mainMenu.TabIndex = 58;
            this.mainMenu.Text = "menuStrip1";
            // 
            // FileMenuItem
            // 
            this.FileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CreateMenuItem,
            this.OpenMenuItem,
            this.SaveMenuItem,
            this.SaveAsMenuItem,
            this.CloseMenuItem,
            this.ExitMenuItem});
            this.FileMenuItem.Name = "FileMenuItem";
            this.FileMenuItem.Size = new System.Drawing.Size(48, 20);
            this.FileMenuItem.Text = "Файл";
            // 
            // CreateMenuItem
            // 
            this.CreateMenuItem.Name = "CreateMenuItem";
            this.CreateMenuItem.Size = new System.Drawing.Size(162, 22);
            this.CreateMenuItem.Text = "Создать";
            // 
            // OpenMenuItem
            // 
            this.OpenMenuItem.Name = "OpenMenuItem";
            this.OpenMenuItem.Size = new System.Drawing.Size(162, 22);
            this.OpenMenuItem.Text = "Открыть";
            // 
            // SaveMenuItem
            // 
            this.SaveMenuItem.Name = "SaveMenuItem";
            this.SaveMenuItem.Size = new System.Drawing.Size(162, 22);
            this.SaveMenuItem.Text = "Сохранить";
            // 
            // SaveAsMenuItem
            // 
            this.SaveAsMenuItem.Name = "SaveAsMenuItem";
            this.SaveAsMenuItem.Size = new System.Drawing.Size(162, 22);
            this.SaveAsMenuItem.Text = "Сохранить как...";
            // 
            // CloseMenuItem
            // 
            this.CloseMenuItem.Name = "CloseMenuItem";
            this.CloseMenuItem.Size = new System.Drawing.Size(162, 22);
            this.CloseMenuItem.Text = "Закрыть";
            // 
            // EditMenuItem
            // 
            this.EditMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RenameMenuItem});
            this.EditMenuItem.Name = "EditMenuItem";
            this.EditMenuItem.Size = new System.Drawing.Size(99, 20);
            this.EditMenuItem.Text = "Редактировать";
            // 
            // calculationToolStripMenuItem
            // 
            this.calculationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.showToolStripMenuItem});
            this.calculationToolStripMenuItem.Name = "calculationToolStripMenuItem";
            this.calculationToolStripMenuItem.Size = new System.Drawing.Size(88, 20);
            this.calculationToolStripMenuItem.Text = "Вычисление";
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("startToolStripMenuItem.Image")));
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.startToolStripMenuItem.Text = "Запустить";
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("stopToolStripMenuItem.Image")));
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.stopToolStripMenuItem.Text = "Остановить";
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.showToolStripMenuItem.Text = "Показать";
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(3, 3);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(221, 248);
            this.treeView1.TabIndex = 0;
            // 
            // CalculationsTabControl
            // 
            this.CalculationsTabControl.Controls.Add(this.PlusTabPage);
            this.CalculationsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CalculationsTabControl.Location = new System.Drawing.Point(3, 3);
            this.CalculationsTabControl.Name = "CalculationsTabControl";
            this.CalculationsTabControl.SelectedIndex = 0;
            this.CalculationsTabControl.Size = new System.Drawing.Size(924, 630);
            this.CalculationsTabControl.TabIndex = 1;
            // 
            // PlusTabPage
            // 
            this.PlusTabPage.Location = new System.Drawing.Point(4, 22);
            this.PlusTabPage.Name = "PlusTabPage";
            this.PlusTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.PlusTabPage.Size = new System.Drawing.Size(916, 604);
            this.PlusTabPage.TabIndex = 1;
            this.PlusTabPage.Text = "   +";
            this.PlusTabPage.UseVisualStyleBackColor = true;
            // 
            // MainTable
            // 
            this.MainTable.ColumnCount = 2;
            this.MainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.MainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.MainTable.Controls.Add(this.RightMainTable, 1, 0);
            this.MainTable.Controls.Add(this.LeftMainTable, 0, 0);
            this.MainTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTable.Location = new System.Drawing.Point(0, 24);
            this.MainTable.Name = "MainTable";
            this.MainTable.RowCount = 1;
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainTable.Size = new System.Drawing.Size(1169, 642);
            this.MainTable.TabIndex = 63;
            // 
            // RightMainTable
            // 
            this.RightMainTable.ColumnCount = 1;
            this.RightMainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.RightMainTable.Controls.Add(this.CalculationsTabControl, 0, 0);
            this.RightMainTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RightMainTable.Location = new System.Drawing.Point(236, 3);
            this.RightMainTable.Name = "RightMainTable";
            this.RightMainTable.RowCount = 1;
            this.RightMainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.RightMainTable.Size = new System.Drawing.Size(930, 636);
            this.RightMainTable.TabIndex = 0;
            // 
            // LeftMainTable
            // 
            this.LeftMainTable.ColumnCount = 1;
            this.LeftMainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.LeftMainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.LeftMainTable.Controls.Add(this.treeView1, 0, 0);
            this.LeftMainTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LeftMainTable.Location = new System.Drawing.Point(3, 3);
            this.LeftMainTable.Name = "LeftMainTable";
            this.LeftMainTable.RowCount = 2;
            this.LeftMainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.LeftMainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.LeftMainTable.Size = new System.Drawing.Size(227, 636);
            this.LeftMainTable.TabIndex = 1;
            // 
            // ErrorToolTip
            // 
            this.ErrorToolTip.AutomaticDelay = 200;
            this.ErrorToolTip.ForeColor = System.Drawing.Color.LightCoral;
            this.ErrorToolTip.ShowAlways = true;
            this.ErrorToolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.ErrorToolTip.ToolTipTitle = "Info";
            // 
            // ExitMenuItem
            // 
            this.ExitMenuItem.Name = "ExitMenuItem";
            this.ExitMenuItem.Size = new System.Drawing.Size(162, 22);
            this.ExitMenuItem.Text = "Выйти";
            // 
            // RenameMenuItem
            // 
            this.RenameMenuItem.Name = "RenameMenuItem";
            this.RenameMenuItem.Size = new System.Drawing.Size(232, 22);
            this.RenameMenuItem.Text = "Переименовать вычисление";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1169, 666);
            this.Controls.Add(this.MainTable);
            this.Controls.Add(this.mainMenu);
            this.Name = "Main";
            this.Text = "Лазерная динамика";
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.CalculationsTabControl.ResumeLayout(false);
            this.MainTable.ResumeLayout(false);
            this.RightMainTable.ResumeLayout(false);
            this.LeftMainTable.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem FileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CreateMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveAsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CloseMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditMenuItem;
        private System.Windows.Forms.ToolStripMenuItem calculationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TabControl CalculationsTabControl;
        private System.Windows.Forms.TabPage PlusTabPage;
        private System.Windows.Forms.TableLayoutPanel MainTable;
        private System.Windows.Forms.TableLayoutPanel RightMainTable;
        private System.Windows.Forms.TableLayoutPanel LeftMainTable;
        private System.Windows.Forms.ToolTip ErrorToolTip;
        private System.Windows.Forms.ToolStripMenuItem ExitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RenameMenuItem;
    }
}