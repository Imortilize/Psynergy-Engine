using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;

using LTreesLibrary.Trees;
using LTreesLibrary.Pipeline;

namespace Psynergy.Graphics
{
    public class TreeManager : Singleton<TreeManager>
    {
        private List<TreeProfile> m_TreeProfiles = new List<TreeProfile>();

        public TreeManager() : base()
        {
        }

        public override void Initialise()
        {
            base.Initialise();

            //TreeProfileReader reader = new TreeProfileReader();

            //reader.DefaultTreeShaderAssetName = "Shaders/Shadows/ppshadow";
            //reader.DefaultLeafShaderAssetName = "Shaders/Shadows/ppshadow";
        }

        public override void Load()
        {
            base.Load();

            // Add tree profiles
            LoadTreeProfile("Graywood");
        }

        private void LoadTreeProfile(String treeProfileName)
        {
            TreeProfile treeProfile = null;

            try
            {
                treeProfile = RenderManager.Instance.ContentManager.Load<TreeProfile>("Trees/" + treeProfileName);
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] - " + e.ToString());
            }

            if (treeProfile != null)
                m_TreeProfiles.Add(treeProfile);
        }

        public override void Reset()
        {
            base.Reset();
        }

        public LTree CreateTree(Vector3 position)
        {
            LTree newTree = null;

            Scene scene = SceneManager.Instance.CurrentScene;

            // Create a new tree from the tree profile
            if (m_TreeProfiles.Count > 0)
            {
                Random rand = new Random();

                int profile = rand.Next(0, m_TreeProfiles.Count);

                if (profile < m_TreeProfiles.Count)
                {
                    TreeProfile treeProfile = m_TreeProfiles[profile];
                    SimpleTree newSimpleTree = treeProfile.GenerateSimpleTree();

                    // Create it into one of my objects
                    newTree = new LTree(newSimpleTree);
                    newTree.Initialise();
                    newTree.Load();

                    // Position the tree
                    newTree.transform.Position = position;
                }
            }

            return newTree;
        }
    }
}
