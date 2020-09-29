namespace ML
{
    // Namespaces
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ML.Algorithms.Models;
    using ML.Algorithms.Utils;
    #endregion

    class Program
    {
        static void Main(string[] args)
        {
            var w2v = new Word2Vector();
            var vec1 = w2v.ToOneHotVector("ClientTimeZone", false, false);
            var vec2 = w2v.ToOneHotVector("ClintTimZone", false, false);

            var cs = new CosineSimilarity(vec1, vec2);
            var res = cs.CosineAlpha();
        }
    }
}