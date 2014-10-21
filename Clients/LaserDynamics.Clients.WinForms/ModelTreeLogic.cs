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
        void ModelTreeInit()
        {
            ModelTreeView.BeforeExpand += (s, e) => OnBeforeExpand(s, e);
            //ModelTreeView.AfterExpand += (s, e) => OnAfteExpand(s, e);
            //ModelTreeView.AfterCollapse += (s, e) => OnAfterCollapse(s, e);
            ModelTreeView.NodeMouseDoubleClick += (s, e) => OnModelDoubleClick(s,e);
            var models = _presenter.CalculationTypes.Select(c => c.View.ModelTitle).Distinct();

            ModelTreeView.BeginUpdate();
            ModelTreeView.Nodes.Clear();

            foreach (string model in models)
            {
                TreeNode modelNode = new TreeNode(model);//, 0, 0);
                modelNode.Name = model;
                ModelTreeView.Nodes.Add(modelNode);

                GetCalcTypes(modelNode);
            }


            ModelTreeView.EndUpdate();
        }
        void GetCalcTypes(TreeNode node)
        {
            var calcTypes = _presenter.CalculationTypes.Where(c => c.View.ModelTitle == node.Name).Select(c => c.View.CalculationType);
                                                        //.Where(c=> c.View.Title.StartsWith(node.FullPath))
                                                        //.GroupBy(c=> c.View.CalculationType)
                                                        //.Select(g => g.GroupBy(c=>c.View.NumMethod));
            foreach (var calcType in calcTypes)
            {
                //foreach (var numMethod in calcType)
                {
                    TreeNode calcNode = new TreeNode(calcType);
                    //dir.SelectedImageIndex = 1;
                    //dir.ImageIndex = 1;
                    calcNode.Name = calcType;
                    //if (!node.Nodes.Find(calcNode.Name, false).Any())
                    //{
                        node.Nodes.Add(calcNode);
                        GetCalcMethods(calcNode, node.Name);
                    //}
                    //else
                    //{
                        //if (node.IsExpanded && node.Nodes[calcType].IsExpanded)
                        //{
                            //GetCalcMethods(node.Nodes[calcType]);
                        //}
                    //}
                }
            }
        }
        void GetCalcMethods(TreeNode node, string model)
        {
            var calcMethods = _presenter.CalculationTypes.Where(c => c.View.ModelTitle == model && c.View.CalculationType == node.Name).Select(c => c.View.NumMethod);
            foreach (var calc in calcMethods)
            {
                TreeNode calcNode = new TreeNode(calc);
                calcNode.Name = calc;
                node.Nodes.Add(calcNode);
            }
        }
        void OnModelDoubleClick(object s,TreeNodeMouseClickEventArgs e)
        {
            var strings = e.Node.FullPath.Split('\\');
            if (strings.Length != 3)
                return;
            var calc = _presenter.CalculationTypes.FirstOrDefault(c => c.View.ModelTitle == strings[0] && c.View.CalculationType == strings[1] && c.View.NumMethod == strings[2]);
            if (calc == null)
                return;
            CreateByTemplate(calc);
        }
    }
}
