using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleTrace.Common
{
    public class MyTree<T>
    {
        public MyTree()
        {
            Children = new List<MyTree<T>>();
        }

        public string Pk { get; set; }
        public string ParentPk { get; set; }

        public int Level
        {
            get
            {
                if (ParentPk == null)
                {
                    return 0;
                }
                return ParentParent.Level + 1;
            }
        }

        public T Value { get; set; }
        public MyTree<T> ParentParent { get; set; }
        public List<MyTree<T>> Children { get; set; }

        public MyTree(string pk, string parentPk, T o)
        {
            Children = new List<MyTree<T>>();
            this.Pk = pk;
            this.ParentPk = parentPk;
            this.Value = o;
        }
    }

    public class MyTreeConvert
    {
        public MyTree<T> MakeTree<T>(IEnumerable<T> models, Func<T, string> getId, Func<T, string> getParentId)
        {
            var makeTrees = MakeTrees(models, getId, getParentId);
            return makeTrees.SingleOrDefault();
        }

        public IList<MyTree<T>> MakeTrees<T>(IEnumerable<T> models, Func<T, string> getId, Func<T, string> getParentId)
        {
            var myTreeRoots = new List<MyTree<T>>();

            var myTrees = ConvertMyTrees(models, getId, getParentId);
            if (myTrees.Count == 0)
            {
                return myTreeRoots;
            }
            
            var rootNodes = myTrees.Where(m =>
                    KeyEquals(m.Pk, m.ParentPk)
                    || string.IsNullOrEmpty(m.ParentPk))
                    .ToList();

            foreach (var rootNode in rootNodes)
            {
                PrepareChild(myTrees, rootNode);
            }

            return rootNodes;
        }

        private IList<MyTree<T>> ConvertMyTrees<T>(IEnumerable<T> models, Func<T, string> getId, Func<T, string> getParentId)
        {
            var trees = new List<MyTree<T>>();
            if (models == null)
            {
                return trees;
            }

            var items = models.ToList();
            if (items.Count == 0)
            {
                return trees;
            }

            foreach (var item in items)
            {
                string id = getId.Invoke(item);
                string pid = getParentId.Invoke(item);
                var node = new MyTree<T>(id, pid, item);
                trees.Add(node);
            }
            return trees;
        }
        private void PrepareChild<T>(IList<MyTree<T>> allNodes, MyTree<T> current)
        {
            var childNodes = allNodes.Where(m => KeyEquals(current.Pk, m.ParentPk) && !KeyEquals(m.Pk, m.ParentPk)).ToList();
            foreach (var childNode in childNodes)
            {
                //item.Parent = node;
                current.Children.Add(childNode);
                PrepareChild(allNodes, childNode);
            }
        }
        private static bool KeyEquals(string key1, string key2, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase, bool trimSpaceBeforeCompare = true)
        {
            if (key1 == null)
            {
                return key2 == null;
            }

            if (key2 == null)
            {
                return false;
            }

            if (trimSpaceBeforeCompare)
            {
                return key1.Trim().Equals(key2.Trim(), stringComparison);
            }

            return key1.Equals(key2, stringComparison);
        }

        #region helpers

        public static MyTreeConvert Instance = new MyTreeConvert();

        #endregion
    }
}