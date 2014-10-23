using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Reflection;
using LaserDynamics.Common;
using System.IO;
using ZedGraph;
using System.Drawing.Imaging;
using Jenyay.Mathematics;

namespace LaserDynamics.Clients.WinForms
{
    public partial class Main : Form
    {
        bool IsSelectedIndexChanged = true;
        bool IsExit = false;
        Presenter _presenter;
        Dictionary<string, Control[]> ControlElements = new Dictionary<string, Control[]>();
        public Main()
        {
            InitializeComponent();
            _presenter = new Presenter();
            CreateDefault();
            CreateMenuItem.Click += (s,e) => CreateDefault();
            OpenMenuItem.Click += (s, e) => Open();
            SaveMenuItem.Click += (s, e) => Save(_presenter.CurrentCalculation);
            SaveAsMenuItem.Click += (s, e) => SaveAs(_presenter.CurrentCalculation);
            CloseMenuItem.Click += (s, e) => Close(CalculationsTabControl.SelectedTab.Name);
            ExitMenuItem.Click += (s, e) => Exit();
            RenameMenuItem.Click += (s, e) => Rename();
            CalculationsTabControl.SelectedIndexChanged += (s, e) => OnSelectedPageChanged();
            FileTreeInit();
            ModelTreeInit();
        }

        #region UI - functions
        void OnSelectedPageChanged()
        {
            if (!IsSelectedIndexChanged)
                return;
            if (CalculationsTabControl.SelectedTab == CalculationsTabControl.TabPages["PlusTabPage"])
                CreateDefault();
            else _presenter.CurrentCalculation = _presenter.OpenCalculations.FirstOrDefault(c => c.CalculationId == CalculationsTabControl.SelectedTab.Name);
        }
        void Rename()
        {
            var oldName = _presenter.CurrentCalculation.Name;
            var newName = Microsoft.VisualBasic.Interaction.InputBox("New name:", "Rename calculation", oldName);
            if (string.IsNullOrEmpty(newName) || newName == oldName)
                return;
            CalculationsTabControl.SelectedTab.Text = newName;
            _presenter.CurrentCalculation.Name = newName;
        }
        void Open()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "clc files (*.clc)|*.clc|All files (*.*)|*.*";
            dialog.RestoreDirectory = true;
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (var path in dialog.FileNames)
                {
                    var calc = _presenter.OpenCalculation(dialog.FileName);
                    _presenter.AddCalculation(calc);
                    Create(calc);
                }
            }
        }
        void Save(ICalculation calc)
        {
            if (calc.Path != null)
            {
                _presenter.SaveCalculation(calc, calc.Path);
                CalculationSaved(calc);
                return;
            }
            SaveAs(calc);
        }
        void SaveAs(ICalculation calc)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "clc files (*.clc)|*.clc|All files (*.*)|*.*";
            dialog.RestoreDirectory = true;
            dialog.FileName = calc.Name;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                calc.Path = dialog.FileName;
                _presenter.SaveCalculation(calc, dialog.FileName);
                CalculationSaved(calc);
            }
        }
        void Close(string id)
        {
            var calc = _presenter.OpenCalculations.FirstOrDefault(c => c.CalculationId == id);
            if (!calc.IsSaved)
                Save(calc);
            _presenter.RemoveCalculation(id);
            ControlElements.Remove(id);
            var page = CalculationsTabControl.TabPages[id];
            int index = CalculationsTabControl.TabPages.IndexOf(page);
            CalculationsTabControl.TabPages.Remove(page);
            CalculationsTabControl.SelectTab(index - 1 > -1 ? index - 1 : 0);
        }
        void Exit()
        {
            IsExit = true;
            var notSavedCalc = _presenter.OpenCalculations.Where(c => !c.IsSaved).ToList();
            if (notSavedCalc == null || !notSavedCalc.Any())
            {
                this.Close();
                return;
            }
            var notSavedCalcsString = String.Join(",\n", notSavedCalc.Select(c => c.Name));
            if (notSavedCalc != null && MessageBox.Show("Folowing calculations not saved:\n" + notSavedCalcsString + "\nSave changes?", "LaserDynamics", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                foreach (var calc in notSavedCalc)
                {
                    Close(calc.CalculationId);
                }
            }
        }
        void Create(ICalculation calc)
        {
            IsSelectedIndexChanged = false;
            var newP = CreateNewTabPage(calc);
            PushTabPage(newP);
            IsSelectedIndexChanged = true;
        }
        void CreateDefault()
        {
            
            string name = "Calculation " + CalculationsTabControl.TabPages.Count;
            var calc = _presenter.AddDefaultCalculation(name);
            Create(calc);
        }
        void CreateByTemplate(ICalculation calc)
        {
            string name = "Calculation " + CalculationsTabControl.TabPages.Count;
            calc = _presenter.AddCalculationByTemplate(calc);
            calc.Name = name;
            Create(calc);
            
        }
        void PushTabPage(TabPage page)
        {
            CalculationsTabControl.TabPages.Add(page);
            CalculationsTabControl.TabPages.Remove(PlusTabPage);
            CalculationsTabControl.TabPages.Add(PlusTabPage);
            CalculationsTabControl.SelectedTab = page;
        }
        protected override void OnClosed(EventArgs e)
        {
            if(!IsExit)
                Exit();
            base.OnClosed(e);
        }
        #endregion

        #region Event handlers for Combo controls selecting calculation types
        void OnModelChanged(string id)
        {
            IsSelectedIndexChanged = false;
            var controls = ControlElements.FirstOrDefault(ce => ce.Key == id);
             
            var modelCombo = controls.Value.FirstOrDefault(c => c.Name == "ModelTitle") as ComboBox;
            var calcTypeCombo = controls.Value.FirstOrDefault(c => c.Name == "CalculationType") as ComboBox;
            calcTypeCombo = RefreshComboBoxItems(calcTypeCombo, _presenter.GetCalcTypes(modelCombo.SelectedItem.ToString()));
            var numMethodCombo = controls.Value.FirstOrDefault(c => c.Name == "NumMethod") as ComboBox;
            numMethodCombo = RefreshComboBoxItems(numMethodCombo, _presenter.GetNumMethods(modelCombo.SelectedItem.ToString(), calcTypeCombo.SelectedItem.ToString()));

            ReplaceCalculationPanel(modelCombo.SelectedItem.ToString(), calcTypeCombo.SelectedItem.ToString(), numMethodCombo.SelectedItem.ToString());
            IsSelectedIndexChanged = true;
        }
        void OnCalculationTypeChanged(string id)
        {
            if (!IsSelectedIndexChanged)
                return;
            IsSelectedIndexChanged = false;

            var controls = ControlElements.FirstOrDefault(ce => ce.Key == id);

            var modelCombo = controls.Value.FirstOrDefault(c => c.Name == "ModelTitle") as ComboBox;
            var calcTypeCombo = controls.Value.FirstOrDefault(c => c.Name == "CalculationType") as ComboBox;
            var numMethodCombo = controls.Value.FirstOrDefault(c => c.Name == "NumMethod") as ComboBox;
            numMethodCombo = RefreshComboBoxItems(numMethodCombo, _presenter.GetNumMethods(modelCombo.SelectedItem.ToString(), calcTypeCombo.SelectedItem.ToString()));

            ReplaceCalculationPanel(modelCombo.SelectedItem.ToString(), calcTypeCombo.SelectedItem.ToString(), numMethodCombo.SelectedItem.ToString());
            IsSelectedIndexChanged = true;
        }
        void OnNumMethodChanged(string id)
        {
            if (!IsSelectedIndexChanged)
                return;
            IsSelectedIndexChanged = false;

            var controls = ControlElements.FirstOrDefault(ce => ce.Key == id);

            var modelCombo = controls.Value.FirstOrDefault(c => c.Name == "ModelTitle") as ComboBox;
            var calcTypeCombo = controls.Value.FirstOrDefault(c => c.Name == "CalculationType") as ComboBox;
            var numMethodCombo = controls.Value.FirstOrDefault(c => c.Name == "NumMethod") as ComboBox;
            
            ReplaceCalculationPanel(modelCombo.SelectedItem.ToString(), calcTypeCombo.SelectedItem.ToString(), numMethodCombo.SelectedItem.ToString());
            IsSelectedIndexChanged = true;
        }
        void ReplaceCalculationPanel(string model,string calcType, string numMethod)
        {
            var template = _presenter.GetTemplate(model, calcType, numMethod);
            _presenter.ReplaceCalculation(_presenter.CurrentCalculation, template);
            var smallTable = CreateCalculationPanel(template.View);
            var name = CalculationsTabControl.SelectedTab.Name;
            SetCalculationEvents(name);
            (CalculationsTabControl.TabPages[name].Controls[0].Controls["StateLabel"] as System.Windows.Forms.Label).SetTextInvoke("Ready");
            var table = CalculationsTabControl.SelectedTab.Controls[0];
            var flowPanel = table.Controls["CalculationPanel"];
            if (flowPanel != null)
                table.Controls.Remove(flowPanel);
            table.Controls.Add(smallTable);
        }
        #endregion

        #region Creating TabPages & panel - methods
        TabPage CreateNewTabPage(ICalculation calc)
        {
            var newPage = new TabPage();
            var name = calc.Name;
            var template = calc.View;
            newPage.Name = calc.CalculationId;
            newPage.Text = name;
            newPage.Padding = new Padding(1, 1, 1, 1);
            newPage.AutoScroll = true;
            var tablePanel = CreateTableLayoutPanel(name, 5, 1);
            tablePanel.Margin = new Padding(1, 1, 1, 1);
            tablePanel.Padding = new Padding(0, 0, 0, 0);
            tablePanel.RowStyles[1] = new RowStyle(SizeType.Percent, 100F);
            tablePanel.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 100F);
            newPage.Controls.Add(tablePanel);
            var newGroup = CreateControlElementsPanel(newPage.Name);
            newGroup.Margin = new Padding(0, 0, 0, 0);
            tablePanel.Controls.Add(newGroup);
            var smallTable = CreateCalculationPanel(template);
            tablePanel.Controls.Add(smallTable, 0, 1);
            var stateLabel = CreateLabel("Ready");
            stateLabel.Name = "StateLabel";
            tablePanel.Controls.Add(stateLabel, 0, 4);
            return newPage;
        }
        #endregion

        #region Creating Calculation panel - methods
        TableLayoutPanel CreateControlElementsPanel(string id)
        {
            var bigTable = CreateTableLayoutPanel("", 1, 8);
            bigTable.Margin = new Padding(0, 0, 0, 0);
            bigTable.Dock = DockStyle.None;
            var topTable = CreateTableLayoutPanel("", 1, 2);
            topTable.Padding = new Padding(0, 0, 0, 0);
            topTable.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 100F);
            topTable.AutoScroll = false;
            bigTable.AutoScroll = false;
            topTable.Controls.Add(bigTable);

            var StartBtn = CreateButton("Start");
            StartBtn.Text = "";
            StartBtn.Image = new Bitmap(@"startEnable.png");
            StartBtn.Size = new Size(28, 20);
            CalculationsTabControl.SelectedTab.Controls.Add(StartBtn);
            bigTable.Controls.Add(StartBtn);

            var StopBtn = CreateButton("Stop");
            StopBtn.Text = "";
            StopBtn.Image = new Bitmap(@"stopEnable.png");
            StopBtn.Size = new Size(28, 20);
            StopBtn.Enabled = false;
            CalculationsTabControl.SelectedTab.Controls.Add(StopBtn);
            bigTable.Controls.Add(StopBtn);

            var CloseBtn = CreateButton("Close");
            CloseBtn.Text = "";
            CloseBtn.Image = new Bitmap(@"close.png");
            CloseBtn.Size = new Size(20, 20);
            topTable.Controls.Add(CloseBtn);

            // ComboBoxes
            var newLabel = CreateLabel("Модель");
            bigTable.Controls.Add(newLabel);

            var modelCombo = CreateComboBox("ModelTitle", _presenter.CalculationTypes
                .Select(c => c.View.ModelTitle).Distinct().ToArray());
            modelCombo.SelectedIndexChanged += (s, e) => OnModelChanged(id);
            bigTable.Controls.Add(modelCombo);
            modelCombo.Anchor = AnchorStyles.Top & AnchorStyles.Bottom;

            newLabel = CreateLabel("Тип расчетов");
            bigTable.Controls.Add(newLabel);

            var calcCombo = CreateComboBox("CalculationType", _presenter.CalculationTypes
                .Where(c => c.View.ModelTitle == modelCombo.Items[0].ToString())
                .Select(c => c.View.CalculationType).ToArray());
            calcCombo.SelectedIndexChanged += (s, e) => OnCalculationTypeChanged(id);
            calcCombo.Anchor = AnchorStyles.Top & AnchorStyles.Bottom;
            bigTable.Controls.Add(calcCombo);

            newLabel = CreateLabel("Численный метод");
            bigTable.Controls.Add(newLabel);

            var numMethodCombo = CreateComboBox("NumMethod", _presenter.CalculationTypes
                .Where(c => c.View.ModelTitle == modelCombo.Items[0].ToString() && c.View.CalculationType == calcCombo.Items[0].ToString())
                .Select(c => c.View.NumMethod).ToArray());
            numMethodCombo.SelectedIndexChanged += (s, e) => OnNumMethodChanged(id);
            numMethodCombo.Anchor = AnchorStyles.Top & AnchorStyles.Bottom;
            bigTable.Controls.Add(numMethodCombo);

            ControlElements.Add(id, new Control[] { StartBtn, StopBtn, CloseBtn, modelCombo, calcCombo, numMethodCombo });
            SetControlEvent(id);
            SetCalculationEvents(id);
            return topTable;
        }
        FlowLayoutPanel CreateCalculationPanel(ICalculationView tempate)
        {
            List<GroupBox> GroupBoxes = new List<GroupBox>();
            List<CheckedListBox> CLB = new List<CheckedListBox>();
            List<TableLayoutPanel> Tables = new List<TableLayoutPanel>();
            foreach (var member in tempate.GetType().GetProperties())
            {
                var attr = (DisplayTitleAttribute)member.GetCustomAttribute(typeof(DisplayTitleAttribute));
                if (attr == null)
                    continue;
                switch (attr.Type)
                {
                    case DataType.Bool:
                        {
                            if (!String.IsNullOrEmpty(attr.Subgroup))
                            {
                                var clb = CLB.FirstOrDefault(c => c.Name == attr.Subgroup);
                                var group = GroupBoxes.FirstOrDefault(g => g.Name == attr.GroupBox);
                                var table = Tables.FirstOrDefault(g => g.Name == attr.GroupBox);
                                if (group == null)
                                {
                                    group = CreateGroupBox(attr.GroupBox);
                                    GroupBoxes.Add(group);
                                    table = CreateTableLayoutPanel(attr.GroupBox, 1, 2);
                                    table.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
                                    Tables.Add(table);
                                    group.Controls.Add(table);
                                }
                                if (clb == null)
                                {
                                    var newLbl = CreateLabel(attr.Subgroup);
                                    clb = CreateCheckListBox(attr.Subgroup, attr.GroupBox);
                                    var tbl = CreateTableLayoutPanel("", 2, 1);
                                    table.Controls.Add(tbl);
                                    tbl.Controls.Add(newLbl);
                                    tbl.Controls.Add(clb);
                                    CLB.Add(clb);
                                    clb.ItemCheck += (s, e) => CalculationChanged(_presenter.CurrentCalculation);
                                }
                                clb.Items.Add(attr.Title, (bool)member.GetValue(tempate));
                            }
                            else
                            {
                                var newLabel = CreateLabel(attr.Title);

                                var newCB = new CheckBox();
                                newCB.Name = member.Name;
                                var str = "";
                                newCB.CheckedChanged += (s, e) => TrySetValueToView(member, attr, newCB.Checked.ToString(), out str);
                                newCB.Checked = (bool)member.GetValue(tempate);

                                var group = GroupBoxes.FirstOrDefault(g => g.Name == attr.GroupBox);
                                var table = Tables.FirstOrDefault(g => g.Name == attr.GroupBox);
                                if (group == null)
                                {
                                    group = CreateGroupBox(attr.GroupBox);
                                    GroupBoxes.Add(group);
                                    table = CreateTableLayoutPanel(attr.GroupBox, 1, 2);
                                    Tables.Add(table);
                                    group.Controls.Add(table);
                                }
                                if (table.Controls.Count > 0)
                                    table.RowCount++;
                                table.Controls.Add(newLabel);
                                table.Controls.Add(newCB);
                                newCB.CheckedChanged += (s, e) => CalculationChanged(_presenter.CurrentCalculation);
                            }
                            break;
                        }
                    case DataType.Discrete:
                        {
                            var newCombo = CreateComboBox(member.Name, attr.ComboValues);
                            var str = "";
                            newCombo.SelectedIndexChanged += (s, e) => TrySetValueToView(member, attr, newCombo.SelectedItem.ToString(), out str);
                            newCombo.SelectedItem = member.GetValue(tempate);
                            var newLabel = CreateLabel(attr.Title);

                            var group = GroupBoxes.FirstOrDefault(g => g.Name == attr.GroupBox);
                            var table = Tables.FirstOrDefault(g => g.Name == attr.GroupBox);
                            if (group == null)
                            {
                                group = CreateGroupBox(attr.GroupBox);
                                GroupBoxes.Add(group);
                                table = CreateTableLayoutPanel(attr.GroupBox, 1, 2);
                                Tables.Add(table);
                                group.Controls.Add(table);
                            }
                            if (table.Controls.Count > 0)
                                table.RowCount++;
                            table.Controls.Add(newLabel);
                            table.Controls.Add(newCombo);
                            newCombo.SelectedIndexChanged += (s, e) => CalculationChanged(_presenter.CurrentCalculation);
                            break;
                        }
                    default:
                        {
                            var newLabel = CreateLabel(attr.Title);
                            var newTB = CreateTextBox(member.Name);
                            newTB.Text = member.GetValue(tempate).ToString();

                            var group = GroupBoxes.FirstOrDefault(g => g.Name == attr.GroupBox);
                            var table = Tables.FirstOrDefault(g => g.Name == attr.GroupBox);

                            if (group == null)
                            {
                                group = CreateGroupBox(attr.GroupBox);
                                table = CreateTableLayoutPanel(attr.GroupBox, 1, 2);
                                GroupBoxes.Add(group);
                                Tables.Add(table);
                                group.Controls.Add(table);
                            }
                            if (table.Controls.Count > 0)
                                table.RowCount++;

                            table.Controls.Add(newLabel, 0, table.RowCount - 1);
                            table.Controls.Add(newTB, 1, table.RowCount - 1);
                            var errorMessage = "";
                            newTB.TextChanged += (s, e) => CalculationChanged(_presenter.CurrentCalculation);
                            newTB.Leave += (s, e) =>
                            {
                                if (!TrySetValueToView(member, attr, newTB.Text, out errorMessage))
                                {
                                    newTB.BackColor = Color.LightCoral;
                                    ErrorToolTip.SetToolTip(newTB, errorMessage);
                                    newTB.Enter += (send, ev) => newTB.BackColor = Color.White;
                                }
                            };

                            break;
                        };
                }
            }
            var smallTable = CreateFlowLayoutPanel("CalculationPanel");
            smallTable.Padding = new Padding(1, 1, 1, 1);
            smallTable.Controls.AddRange(GroupBoxes.ToArray());
            return smallTable;
        }
        void ReverseButtonsState(params Button[] btns)
        {
            foreach (var btn in btns)
            {
                btn.SetEnableInvoke(!btn.Enabled);
                //string path = btn.Name + (btn.Enabled ? "Enable" : "Disable") + ".png";
                //btn.Image = new Bitmap(path);
            }
        }
        void CalculationSaved(ICalculation calc)
        {
            if (!calc.IsSaved)
            {
                calc.IsSaved = true;
                CalculationsTabControl.TabPages[calc.CalculationId].Text = calc.Name;
                //CalculationsTabControl.SelectedTab.();
            }
        }
        void CalculationChanged(ICalculation calc)
        {
            if (calc.IsSaved)
            {
                calc.IsSaved = false;
                CalculationsTabControl.TabPages[calc.CalculationId].Text = calc.Name + "*";
                //CalculationsTabControl.SelectedTab.();
            }
        }
        void SetControlEvent(string id)
        {
            var controls = ControlElements.FirstOrDefault(ce => ce.Key == id);
            var StartBtn = controls.Value.FirstOrDefault(c => c.Name == "Start") as Button;
            var StopBtn = controls.Value.FirstOrDefault(c => c.Name == "Stop") as Button;
            var CloseBtn = controls.Value.FirstOrDefault(c => c.Name == "Close") as Button;
            
            StartBtn.Click += async (s, e) =>
            {
                ReverseButtonsState(StartBtn, StopBtn);
                (CalculationsTabControl.TabPages[id].Controls[0].Controls["StateLabel"] as System.Windows.Forms.Label).SetTextInvoke("Started...");
                await _presenter.StartCalculation();
            };
            StopBtn.Click += (s, e) =>
            {
                _presenter.StopCalculation();
            };
            CloseBtn.Click += (s, e) => Close(id);
        }
        void SetCalculationEvents(string id)
        {
            var controls = ControlElements.FirstOrDefault(ce => ce.Key == id);
            var StartBtn = controls.Value.FirstOrDefault(c => c.Name == "Start") as Button;
            var StopBtn = controls.Value.FirstOrDefault(c => c.Name == "Stop") as Button;
            
            _presenter.OpenCalculations.FirstOrDefault(c => c.CalculationId == id).OnCalculationValidationFailed += (s, e) =>
            {
                ReverseButtonsState(StartBtn, StopBtn);
                (CalculationsTabControl.TabPages[id].Controls[0].Controls["StateLabel"] as System.Windows.Forms.Label).SetTextInvoke("Validation Error.");
                MessageBox.Show("Не все поля заполнены корректно", "Validation Error");
            };
            _presenter.OpenCalculations.FirstOrDefault(c => c.CalculationId == id).OnCalculationFinish += (s, e) =>
            {
                var calc = _presenter.OpenCalculations.FirstOrDefault(c => c.CalculationId == id);
                var stateLabel = CalculationsTabControl.TabPages[id].Controls[0].Controls["StateLabel"] as System.Windows.Forms.Label;
                if (calc.ErrorMessage != null)
                {
                    stateLabel.SetTextInvoke("Error: " + calc.ErrorMessage);
                }
                else if (calc.Status == CalculationStatus.Stopped)
                {
                    stateLabel.SetTextInvoke("Stopped.");
                }
                else
                {
                    stateLabel.SetTextInvoke("Succesefully performed: " + string.Join(", ", calc.GetStats().Select(st => st.ToString() + " ms")));
                    this.Invoke(new MethodInvoker(() =>
                    {
                        HideResults(id);
                        ShowResults(id);
                    }));
                }
                ReverseButtonsState(StartBtn, StopBtn);
            };
            _presenter.OpenCalculations.FirstOrDefault(c => c.CalculationId == id).OnCalculationError += (s, e) =>
            {
                var calc = _presenter.OpenCalculations.FirstOrDefault(c => c.CalculationId == id);
                if (calc.ErrorMessage != null)
                {
                    (CalculationsTabControl.TabPages[id].Controls[0].Controls["StateLabel"] as System.Windows.Forms.Label).SetTextInvoke("Error: " + calc.ErrorMessage);
                }
                ReverseButtonsState(StartBtn, StopBtn);
            };
            _presenter.OpenCalculations.FirstOrDefault(c => c.CalculationId == id).OnCalculationReport += (s, e) =>
            {
                var calc = _presenter.OpenCalculations.FirstOrDefault(c => c.CalculationId == id);
                (CalculationsTabControl.TabPages[id].Controls[0].Controls["StateLabel"] as System.Windows.Forms.Label).SetTextInvoke("Running: " + calc.ReportMessage + "%");
            };
        }
        #endregion

        #region Creating CalculationResults panel - methods
        TableLayoutPanel CreateResultsControlPanel(string id)
        {
            var table = CreateTableLayoutPanel("CalculationResultsControlPanel", 1, 3);
            table.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 100F);
            var resultLabel = CreateLabel("Results");
            var saveBtn = CreateButton("Save all results");
            saveBtn.Click += (s, e) => SaveAllImages(id);
            var CloseBtn = CreateButton("Close");
            CloseBtn.Text = "";
            CloseBtn.Image = new Bitmap(@"close.png");
            CloseBtn.Size = new Size(20, 20);
            CloseBtn.Click += (s, e) => HideResults(id);
            table.Controls.Add(resultLabel, 0, 0);
            table.Controls.Add(saveBtn, 1, 0);
            table.Controls.Add(CloseBtn, 2, 0);

            return table;
        }
        void ShowResults(string id)
        {
            var view = _presenter.GetView(id);
            var result = _presenter.GetResults(id);
            if (result == null)
                return;
            var propDictionary = new Dictionary<string, object>();
            foreach (var prop in result.GetType().GetProperties())
            {
                var value = prop.GetValue(result);
                if (value != null)
                    propDictionary.Add(prop.Name, value);
            }
            if (!propDictionary.Any())
                return;
            var smallTable = CreateFlowLayoutPanel("CalculationResultsPanel");
            smallTable.Padding = new Padding(1, 1, 1, 1);

            foreach (var member in view.GetType().GetProperties())
            {
                var attr = (DisplayTitleAttribute)member.GetCustomAttribute(typeof(DisplayTitleAttribute));
                var resAttr = (ResultsAttribute)member.GetCustomAttribute(typeof(ResultsAttribute));
                if (attr == null || resAttr == null)
                    continue;
                object value = null;
                if (!propDictionary.TryGetValue(member.Name, out value))
                    continue;
                Control ctrl = null;
                switch (resAttr.DemoType)
                {
                    case DemoType.Plot:
                        ctrl = CreatePlot(attr.Title, value, resAttr.NumType);
                        if (ctrl == null)
                            continue;
                        //var group = CreateGroupBox(member.Name);
                        //group.Controls.Add(plot);
                        break;
                    case DemoType.Image:
                        var pict = CreatePictureBox(attr.Title, value, resAttr.NumType);
                        pict.BorderStyle = BorderStyle.FixedSingle;
                        ctrl = pict;
                        break;
                }
                var saveNums = CreateButton("SaveAsFile");
                saveNums.Click += (s, e) => SaveResultToFile(value, attr.Title +" " +  view.Overview);
                var saveBmp = CreateButton("SaveAsBitMap");
                var bmp = ctrl is ZedGraphControl ? (ctrl as ZedGraphControl).MasterPane.PaneList[0].GetImage() : (ctrl as PictureBox).Image as Bitmap;
                saveBmp.Click += (s, e) => SaveResultAsBitMap(bmp, attr.Title + " " + view.Overview);
                var btnTable = CreateTableLayoutPanel("btnTable", 1, 2);
                btnTable.Controls.Add(saveNums, 0, 0);
                btnTable.Controls.Add(saveBmp, 1, 0);
                var plotTable = CreateTableLayoutPanel("plotTable", 2, 1);
                plotTable.Controls.Add(btnTable, 0, 0);
                plotTable.Controls.Add(ctrl, 0, 1);
                smallTable.Controls.Add(plotTable);
            }
              
            if (smallTable.Controls.Count > 0)
            {
                var table = CalculationsTabControl.TabPages[id].Controls[0] as TableLayoutPanel;
                table.RemoveByKeyControlInvoke("CalculationResultsPanel");
                var controlPanel = CreateResultsControlPanel(id);
                table.Controls.Add(controlPanel, 0, 2);
                table.Controls.Add(smallTable, 0, 3);
                table.RowStyles[1] = new RowStyle(SizeType.Percent, 50F);
                table.RowStyles[3] = new RowStyle(SizeType.Percent, 50F);
            }
        }
        void SaveResultAsBitMap(Bitmap bmp, string fileName)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "*.png|*.png|*.jpg; *.jpeg|*.jpg;*.jpeg|*.bmp|*.bmp|Все файлы|*.*";
            dlg.FileName = fileName;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // Сохраняем картинку средствами класса Bitmap
                // Формат картинки выбирается исходя из имени выбранного файла
                if (dlg.FileName.EndsWith(".png"))
                {
                    bmp.Save(dlg.FileName, ImageFormat.Png);
                }
                else if (dlg.FileName.EndsWith(".jpg") || dlg.FileName.EndsWith(".jpeg"))
                {
                    bmp.Save(dlg.FileName, ImageFormat.Jpeg);
                }
                else if (dlg.FileName.EndsWith(".bmp"))
                {
                    bmp.Save(dlg.FileName, ImageFormat.Bmp);
                }
                else
                {
                    bmp.Save(dlg.FileName);
                }
            }
        }
        void SaveResultToFile(object value, string fileName)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "clr files (*.clr)|*.clr|All files (*.*)|*.*";
            dialog.RestoreDirectory = true;
            dialog.FileName = fileName;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                WriteToFile(value, dialog.FileName);
            }
        }
        void WriteToFile(object value, string path)
        {
            string stringBody = null;
            if (!Presenter.TrySerialize(value, out stringBody))
            {
                return;
            }
            using (StreamWriter file = new StreamWriter(path))
            {
                file.Write(stringBody);
            }
        }
        void HideResults(string id)
        {
            var table = CalculationsTabControl.TabPages[id].Controls[0] as TableLayoutPanel;
            table.Controls.RemoveByKey("CalculationResultsControlPanel");
            table.Controls.RemoveByKey("CalculationResultsPanel");
            table.RowStyles[1] = new RowStyle(SizeType.Percent, 100F);
        }
        #endregion


        #region Reflecting methods
        void SetValuesToView(Dictionary<string,string> namesValues, string group, string subgroup)
        {
            foreach (var p in _presenter.CurrentCalculation.View.GetType().GetProperties())
            {
                var attr = (DisplayTitleAttribute)p.GetCustomAttribute(typeof(DisplayTitleAttribute));
                var str = "";
                if (attr!= null && namesValues.ContainsKey(attr.Title) && attr.GroupBox == group && attr.Subgroup == subgroup)
                    TrySetValueToView(p,attr,namesValues[attr.Title], out str);
            }

        }
        bool TrySetValueToView(PropertyInfo prop, DisplayTitleAttribute attr, string value, out string ErrorMessage)
        {
            ErrorMessage = null;
            value = value.Trim();
            var view = _presenter.CurrentCalculation.View;

            switch (attr.Type)
            {
                case DataType.Bool:
                    {
                        bool val = bool.Parse(value);
                        prop.SetValue(view, val, null);
                        return true;
                    }
                case DataType.Double:
                    {
                        double val;
                        if (!double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out val)
                            || !(val > attr.MoreThan && val < attr.LessThan))
                        {
                            ErrorMessage = attr.ErrorText;
                            return false;
                        }
                        prop.SetValue(view, val, null);
                        return true;
                    }
                case DataType.Integer:
                    {
                        int val;
                        if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out val)
                           || !(val > attr.MoreThan && val < attr.LessThan))
                        {
                            ErrorMessage = attr.ErrorText;
                            return false;
                        }
                        prop.SetValue(view, val, null);
                        return true;
                    }
                default:
                    {
                        prop.SetValue(view, value, null);
                        return true;
                    }
            }
        }
        #endregion
    }
}

