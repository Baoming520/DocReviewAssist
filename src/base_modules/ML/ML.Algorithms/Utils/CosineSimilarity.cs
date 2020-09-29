namespace ML.Algorithms.Utils
{
    // Namespaces
    #region Namespaces
    using System;
    #endregion

    public class CosineSimilarity
    {
        public CosineSimilarity(double[] vector1, double[] vector2)
        {
            this.Vector1 = vector1;
            this.Vector2 = vector2;
        }

        public double[] Vector1 { get; private set; }
        public double[] Vector2 { get; private set; }

        public double CosineAlpha(bool normalization = false)
        {
            if (null == this.Vector1 || null == this.Vector2 || 
                this.Vector1.Length != this.Vector2.Length)
            {
                return -1.0;
            }

            double dotProduct = 0.0;
            double norm_x1 = 0.0;
            double norm_x2 = 0.0;
            for (int i = 0; i < this.Vector1.Length; i++)
            {
                dotProduct += this.Vector1[i] * this.Vector2[i];
                norm_x1 += Math.Pow(this.Vector1[i], 2);
                norm_x2 += Math.Pow(this.Vector2[i], 2);
            }

            double res = dotProduct / (Math.Sqrt(norm_x1) * Math.Sqrt(norm_x2));
            if (!normalization)
            {
                return res;
            }

            return 1.0 / (1.0 + 1.0 / Math.Pow(Math.E, res));
        }

        public double CosineDistance()
        {
            return 1.0 - this.CosineAlpha();
        }
    }
}
