using SimulationCore.MathTool.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimulationCore.MathTool.Statistics
{
    /// <summary>
    /// Class for basic statistic values of a given sample
    /// </summary>
    public class StatisticsSample
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="sample">A collection of the sample values</param>
        /// <param name="confIntType">Type of 95-% confidence interval to be computed</param>
        /// <param name="quantiles">A list of extra quantile values to be computed</param>
        public StatisticsSample(ICollection<double> sample,
            ConfidenceIntervalTypes confIntType,
            List<double> quantiles = null)
        {
            // for sample sizes smaller than or equal to 1 statistics cannot be computed
            if (sample.Count <= 1)
                return;

            List<double> sortedSample = sample.ToList();

            // sort sample
            sortedSample.Sort();

            // compute mean
            _mean = sortedSample.Sum() / sortedSample.Count;

            // compute variance
            foreach (double val in sortedSample)
            {
                _variance += 1d / (sortedSample.Count - 1) * Math.Pow(val - _mean, 2);
            } // end foreach

            // compute median
            _median = sortedSample[(int)Math.Ceiling(sample.Count() / 2d)];

            // confidence computed via standard deviation
            if (confIntType == ConfidenceIntervalTypes.StandardDeviation)
            {
                _cI95Lower = _mean - 1.96 * Math.Sqrt(_variance / sample.Count);
                _cI95Upper = _mean + 1.96 * Math.Sqrt(_variance / sample.Count);
            }
            // confidence intercal via quantiles
            else if (confIntType == ConfidenceIntervalTypes.Quantiles)
            {
                int sortLength = sortedSample.Count;

                _cI95Lower = sortedSample[(int)(sortLength * 0.025)];
                _cI95Upper = sortedSample[(int)(sortLength * 0.975)];
            }

            _quantiles = new Dictionary<double, double>();

            // compute specified quantiles
            if (quantiles != null)
            {
                for (int i = 0; i < quantiles.Count; i++)
                {
                    _quantiles.Add(quantiles[i], sortedSample[(int)(sortedSample.Count * quantiles[i])]);
                } // end for
            } // end if
        } // end of StatisticsSample

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region Mean

        private double _mean;

        public double Mean
        {
            get
            {
                return _mean;
            }
        } // end of Mean

        #endregion Mean

        #region Median

        private double _median;

        public double Median
        {
            get
            {
                return _median;
            }
        } // end of Median

        #endregion Median

        #region Variance

        private double _variance;

        public double Variance
        {
            get
            {
                return _variance;
            }
        } // end of Variance

        #endregion Variance

        #region CI95Lower

        private double _cI95Lower;

        public double CI95Lower
        {
            get
            {
                return _cI95Lower;
            }
        } // end of CI95Lower

        #endregion CI95Lower

        #region Ci95Upper

        private double _cI95Upper;

        public double Ci95Upper
        {
            get
            {
                return _cI95Upper;
            }
        } // end of Ci95Upper

        #endregion Ci95Upper

        #region Quantiles

        private Dictionary<double, double> _quantiles;

        public Dictionary<double, double> Quantiles
        {
            get
            {
                return _quantiles;
            }
        } // end of Quantiles

        #endregion Quantiles
    } // end of StatisticsSample
}