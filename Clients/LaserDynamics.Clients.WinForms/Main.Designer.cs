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
            this.ExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RenameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calculationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FileTreeView = new System.Windows.Forms.TreeView();
            this.LeftMainTable = new System.Windows.Forms.TableLayoutPanel();
            this.ErrorToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.CalculationsTabControl = new System.Windows.Forms.TabControl();
            this.PlusTabPage = new System.Windows.Forms.TabPage();
            this.MainPanel = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.ImageList = new System.Windows.Forms.ImageList(this.components);
            this.mainMenu.SuspendLayout();
            this.LeftMainTable.SuspendLayout();
            this.CalculationsTabControl.SuspendLayout();
            this.MainPanel.SuspendLayout();
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
            // ExitMenuItem
            // 
            this.ExitMenuItem.Name = "ExitMenuItem";
            this.ExitMenuItem.Size = new System.Drawing.Size(162, 22);
            this.ExitMenuItem.Text = "Выйти";
            // 
            // EditMenuItem
            // 
            this.EditMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RenameMenuItem});
            this.EditMenuItem.Name = "EditMenuItem";
            this.EditMenuItem.Size = new System.Drawing.Size(99, 20);
            this.EditMenuItem.Text = "Редактировать";
            // 
            // RenameMenuItem
            // 
            this.RenameMenuItem.Name = "RenameMenuItem";
            this.RenameMenuItem.Size = new System.Drawing.Size(232, 22);
            this.RenameMenuItem.Text = "Переименовать вычисление";
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
            this.startToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.startToolStripMenuItem.Text = "Запустить";
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("stopToolStripMenuItem.Image")));
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.stopToolStripMenuItem.Text = "Остановить";
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.showToolStripMenuItem.Text = "Показать";
            // 
            // FileTreeView
            // 
            this.FileTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FileTreeView.ImageIndex = 0;
            this.FileTreeView.ImageList = this.ImageList;
            this.FileTreeView.Location = new System.Drawing.Point(3, 3);
            this.FileTreeView.Name = "FileTreeView";
            this.FileTreeView.SelectedImageIndex = 0;
            this.FileTreeView.Size = new System.Drawing.Size(221, 252);
            this.FileTreeView.TabIndex = 0;
            // 
            // LeftMainTable
            // 
            this.LeftMainTable.ColumnCount = 1;
            this.LeftMainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.LeftMainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.LeftMainTable.Controls.Add(this.FileTreeView, 0, 0);
            this.LeftMainTable.Dock = System.Windows.Forms.DockStyle.Left;
            this.LeftMainTable.Location = new System.Drawing.Point(0, 0);
            this.LeftMainTable.Name = "LeftMainTable";
            this.LeftMainTable.RowCount = 2;
            this.LeftMainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40.25157F));
            this.LeftMainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 59.74843F));
            this.LeftMainTable.Size = new System.Drawing.Size(227, 642);
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
            // CalculationsTabControl
            // 
            this.CalculationsTabControl.Controls.Add(this.PlusTabPage);
            this.CalculationsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CalculationsTabControl.Location = new System.Drawing.Point(227, 0);
            this.CalculationsTabControl.Name = "CalculationsTabControl";
            this.CalculationsTabControl.SelectedIndex = 0;
            this.CalculationsTabControl.Size = new System.Drawing.Size(942, 642);
            this.CalculationsTabControl.TabIndex = 1;
            // 
            // PlusTabPage
            // 
            this.PlusTabPage.Location = new System.Drawing.Point(4, 22);
            this.PlusTabPage.Name = "PlusTabPage";
            this.PlusTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.PlusTabPage.Size = new System.Drawing.Size(934, 616);
            this.PlusTabPage.TabIndex = 1;
            this.PlusTabPage.Text = "   +";
            this.PlusTabPage.UseVisualStyleBackColor = true;
            // 
            // MainPanel
            // 
            this.MainPanel.Controls.Add(this.splitter1);
            this.MainPanel.Controls.Add(this.CalculationsTabControl);
            this.MainPanel.Controls.Add(this.LeftMainTable);
            this.MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPanel.Location = new System.Drawing.Point(0, 24);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(1169, 642);
            this.MainPanel.TabIndex = 60;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(227, 0);
            this.splitter1.Margin = new System.Windows.Forms.Padding(1);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 642);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // ImageList
            // 
            this.ImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImageList.ImageStream")));
            this.ImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.ImageList.Images.SetKeyName(0, "drive.jpg");
            this.ImageList.Images.SetKeyName(1, "CloseDir.png");
            this.ImageList.Images.SetKeyName(2, "OpenDir.png");
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1169, 666);
            this.Controls.Add(this.MainPanel);
            this.Controls.Add(this.mainMenu);
            this.Name = "Main";
            this.Text = "Лазерная динамика";
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.LeftMainTable.ResumeLayout(false);
            this.CalculationsTabControl.ResumeLayout(false);
            this.MainPanel.ResumeLayout(false);
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
        private System.Windows.Forms.TreeView FileTreeView;
        private System.Windows.Forms.TableLayoutPanel LeftMainTable;
        private System.Windows.Forms.ToolTip ErrorToolTip;
        private System.Windows.Forms.ToolStripMenuItem ExitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RenameMenuItem;
        private System.Windows.Forms.TabControl CalculationsTabControl;
        private System.Windows.Forms.TabPage PlusTabPage;
        private System.Windows.Forms.Panel MainPanel;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ImageList ImageList;
    }
}