using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaserDynamics.Clients.WinForms
{
    partial class Main
    {
        public void DriveTreeInit()
        {
            FileTreeView.BeforeExpand += (s, e) => OnBeforeExpand(s,e);
            FileTreeView.AfterExpand += (s, e) => OnAfteExpand(s, e);
            FileTreeView.AfterSelect += (s, e) => OnAfterSelect(s, e);
            FileTreeView.AfterCollapse += (s, e) => OnAfterCollapse(s, e);
            string[] drivesArray = Directory.GetLogicalDrives();

            FileTreeView.BeginUpdate();
            FileTreeView.Nodes.Clear();

            foreach (string s in drivesArray)
            {
                TreeNode drive = new TreeNode(s, 0, 0);
                FileTreeView.Nodes.Add(drive);

                GetDirs(drive);
            }


            FileTreeView.EndUpdate();
        }
        /// <summary>
        /// Получение списка каталогов
        /// </summary>
        public void GetDirs(TreeNode node)
        {
            DirectoryInfo[] diArray;

            string fullPath = node.FullPath;
            DirectoryInfo di = new DirectoryInfo(fullPath);

            try
            {
                diArray = di.GetDirectories();
            }
            catch
            {
                return;
            }
            foreach (DirectoryInfo dirinfo in diArray)
            {
                TreeNode dir = new TreeNode(dirinfo.Name);
                dir.SelectedImageIndex = 1;
                dir.ImageIndex = 1;
                dir.Name = dirinfo.Name;
                if (!node.Nodes.Find(dir.Name,false).Any())
                {
                    node.Nodes.Add(dir);
                }
                else
                {
                    if(node.IsExpanded && node.Nodes[dirinfo.Name].IsExpanded)
                    {
                        GetDirs(node.Nodes[dirinfo.Name]);
                    }
                }
            }
        }
        public void OnBeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            FileTreeView.BeginUpdate();
            foreach (TreeNode node in e.Node.Nodes)
            {
                //ChooseImage(node);
                GetDirs(node);
            }
            FileTreeView.EndUpdate();
        }
        public void OnAfteExpand(object sender, TreeViewEventArgs e)
        {
            FileTreeView.BeginUpdate();
            ChooseImage(e.Node);
            FileTreeView.EndUpdate();
        }
        public void OnAfterSelect(object sender, TreeViewEventArgs e)
        {
            FileTreeView.BeginUpdate();
            ChooseImage(e.Node);
            FileTreeView.EndUpdate();
        }
        public void OnAfterCollapse(object sender, TreeViewEventArgs e)
        {
            FileTreeView.BeginUpdate();
            ChooseImage(e.Node);
            FileTreeView.EndUpdate();
        }
        void ChooseImage(TreeNode node)
        {
            if (node.Parent != null)
                if (node.IsExpanded)
                {
                    node.ImageIndex = 2;
                    node.SelectedImageIndex = 2;
                }
                else
                {
                    node.ImageIndex = 1;
                    node.SelectedImageIndex = 1;
                }
        }
        
    }

}
