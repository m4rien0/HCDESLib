using SimulationCore.MathTool.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimulationCore.MathTool.Distributions
{
    /// <summary>
    /// A collection of distribution classes.
    /// </summary>

    #region ConfidenceIntervalTypes

    /// Defines the type of confidence intervals used
    public enum ConfidenceIntervalTypes
    {
        StandardDeviation,
        Quantiles,
        Bootstrap
    } // end of ConfidenceIntervalTypes

    #endregion ConfidenceIntervalTypes

    #region EmpiricalDiscreteDistribution

    /// <summary>
    /// Defines a empirical distribution of a chosen type. Only needs the realizations and corresponding probabilities
    /// and automatically defines the resulting distribution.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EmpiricalDiscreteDistribution<T>
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor, takes a sample and corresponding proabilities
        /// </summary>
        /// <param name="proabilities">Probabilities of values</param>
        /// <param name="values">Values of sample of type T</param>
        public EmpiricalDiscreteDistribution(double[] proabilities, T[] values)
        {
            if (proabilities.Length != values.Length)
                throw new InvalidOperationException("Discrete Distribution: values and proabilities must have same dimensions!");

            _probabilities = new double[proabilities.Length];
            _values = new T[proabilities.Length];
            _cummulatedProbabilities = new double[proabilities.Length];

            double cumProb = 0;

            // creating cummulated probabilities
            for (int i = 0; i < proabilities.Length; i++)
            {
                _probabilities[i] = proabilities[i];
                _values[i] = values[i];
                cumProb += proabilities[i];
                _cummulatedProbabilities[i] = cumProb;
            } // end for
        } // end of

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region CummulatedProbabilities

        private double[] _cummulatedProbabilities;

        /// <summary>
        /// Holds cummulated probabilities in order of passed values
        /// </summary>
        public double[] CummulatedProbabilities
        {
            get
            {
                return _cummulatedProbabilities;
            }
        } // end of CummulatedProbabilities

        #endregion CummulatedProbabilities

        #region Probabilities

        private double[] _probabilities;

        /// <summary>
        /// Probabilities of values in order of passed values
        /// </summary>
        public double[] Probabilities
        {
            get
            {
                return _probabilities;
            }
        } // end of Probabilities

        #endregion Probabilities

        #region Values

        private T[] _values;

        /// <summary>
        /// Values of sample of type T
        /// </summary>
        public T[] Values
        {
            get
            {
                return _values;
            }
        } // end of Values

        #endregion Values

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region GetRandomValue

        /// <summary>
        /// Returns a randomly sampled value with repsect to passed probabilities. In this mehtod
        /// a value is always returned. If cummulated probabilities don't sum to one and the
        /// sampled number is bigger than the largest cummulated probability the last value
        /// is returned. Use GetNullableRandomValue if that behavior is not desired.
        /// </summary>
        /// <returns></returns>
        public T GetRandomValue()
        {
            double rand = Distributions.Instance.RandomNumberGenerator.NextDouble();

            double lastCum = 0;

            for (int i = 0; i < CummulatedProbabilities.Length; i++)
            {
                if (lastCum <= rand && rand < CummulatedProbabilities[i])
                    return Values[i];
            } // end for

            return Values.Last();
        } // end of GetRandomValue

        #endregion GetRandomValue

        #region GetNullableRandomValue

        /// <summary>
        /// Basically the same functionality of GetRandomValue, a sampled value is returned.
        /// This method allows the possibility that no value is sampled, which needs the cummulated probabilities
        /// to be smaller than 1, in this case the out
        /// variable valueSampled remains false and the default value of T is returned
        /// </summary>
        /// <returns>Either sampled value or default value of T if no sample was generated</returns>
        public T GetNullableRandomValue(out bool valueSampled)
        {
            valueSampled = false;

            double rand = Distributions.Instance.RandomNumberGenerator.NextDouble();

            double lastCum = 0;

            for (int i = 0; i < CummulatedProbabilities.Length; i++)
            {
                if (lastCum <= rand && rand < CummulatedProbabilities[i])
                {
                    valueSampled = true;
                    return Values[i];
                } // end if
            } // end for

            return default(T);
        } // end of GetRandomValue

        #endregion GetNullableRandomValue
    } // end of EmpiricalDiscreteDistribution

    #endregion EmpiricalDiscreteDistribution

    #region GeneralDistributions

    /// <summary>
    /// Singleton that holds basic stochastic distributions
    /// </summary>
    public sealed class Distributions
    {
        private static Distributions instance = null;
        private static readonly object padlock = new object();

        #region Constructor

        /// <summary>
        /// Basic constructor that generates a new Random object
        /// </summary>
        private Distributions()
        {
            _randomNumberGenerator = new Random(2);
        }

        #endregion Constructor

        #region GetInstance

        /// <summary>
        /// The thread-safe singleton instance of the Distribution class
        /// </summary>
        public static Distributions Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Distributions();
                    } // end if
                    return instance;
                } // end lock
            } // end get
        } // end of get Instance

        #endregion GetInstance

        #region Exponential

        /// <summary>
        /// Exponential distribution, version where parameter represents the expected value
        /// and not the inverse
        /// </summary>
        /// <param name="mean">Desired expected value of the exponential distribution</param>
        /// <returns>Sampled exponenial distributed value</returns>
        public double Exponential(double mean)
        {
            double y = RandomNumberGenerator.NextDouble();

            return -mean * Math.Log(1 - y);
        } // end of Exponential

        #endregion Exponential

        #region LogNormal

        /// <summary>
        /// Log-normal distribution
        /// </summary>
        /// <param name="mean">Expected value</param>
        /// <param name="var">Variance</param>
        /// <returns>Log normal distributed sample</returns>
        public double LogNormal(double mean, double var)
        {
            return Math.Exp(GaussianDistribution(Math.Log(Math.E) - Math.Log(var / Math.Exp(2) + 1) / 2, Math.Log(var / Math.Exp(2) + 1)));
        } // end of Lognormal

        #endregion LogNormal

        #region GaussianDistribution

        /// <summary>
        /// Gaussian distribution
        /// </summary>
        /// <param name="mean">Expected value</param>
        /// <param name="var">Variance</param>
        /// <returns>Returns gaussian distributed sample</returns>
        public double GaussianDistribution(double mean, double var)
        {
            double u1 = RandomNumberGenerator.NextDouble();
            double u2 = RandomNumberGenerator.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2);
            return mean + Math.Pow(var, 0.5) * randStdNormal;
        } // end of GaussianDistribution

        #endregion GaussianDistribution

        #region TriangularDistribution

        /// <summary>
        /// General, not necessarily symetric, triangular distribution
        /// </summary>
        /// <param name="lower">Lower boundary of triangular distribution</param>
        /// <param name="peak">Peak value of triangular distribution</param>
        /// <param name="upper">Upper boundary of triangular distribution</param>
        /// <returns>Triangular distributed sample</returns>
        public double TriangularDistribution(double lower, double peak, double upper)
        {
            double rand = RandomNumberGenerator.NextDouble();

            if (upper == lower)
                return lower;

            if (rand <= (peak - lower) / (upper - lower))
                return lower + Math.Sqrt(rand * (upper - lower) * (peak - lower));
            else
                return upper - Math.Sqrt((1 - rand) * (upper - lower) * (upper - peak));
        } // end of TriangularDistribution

        #endregion TriangularDistribution

        #region SymetricTriangularDistribution

        /// <summary>
        /// Symetric triangular distribution where lower and upper boundary have same distance from peak.
        /// </summary>
        /// <param name="mean">Expected and peak value of triangular distribution</param>
        /// <param name="deviaton">Distance of lower and upper boundary from peak</param>
        /// <returns>Symetric triangular distributed sample</returns>
        public double SymetricTriangularDistribution(double mean, double deviaton)
        {
            return TriangularDistribution(mean - deviaton, mean, mean + deviaton);
        } // end of SymetricTriangularDistribution

        #endregion SymetricTriangularDistribution

        #region RandomInteger

        /// <summary>
        /// Random integer (inclusive boundaries)
        /// </summary>
        /// <param name="min">Lower bound</param>
        /// <param name="max">Upper bound</param>
        /// <returns>Random integer between lower and upper bound</returns>
        public int RandomInteger(int min, int max)
        {
            return RandomNumberGenerator.Next(min, max);
        } // end of RandomInteger

        #endregion RandomInteger

        #region RandomNumberGenerator

        private Random _randomNumberGenerator;

        /// <summary>
        /// Random object used by all distributions
        /// </summary>
        public Random RandomNumberGenerator
        {
            get
            {
                return _randomNumberGenerator;
            }
        } // end of RandomNumberGenerator

        #endregion RandomNumberGenerator

        #region GetStatistics

        /// <summary>
        /// Produces statistical measures of a set of double values
        /// </summary>
        /// <param name="sample">Sample values</param>
        /// <param name="confIntType">Type of confidence interval to be used</param>
        /// <param name="quantList">A list of quantiles that should be computed for the sample</param>
        /// <returns>A set of statistical measures for the sample</returns>
        public static StatisticsSample GetStatistics(ICollection<double> sample,
            ConfidenceIntervalTypes confIntType = ConfidenceIntervalTypes.StandardDeviation,
            List<double> quantList = null)
        {
            return new StatisticsSample(sample, confIntType, quantList);
        } // end of GetMeanOfSample

        #endregion GetStatistics

        #region GetMean

        /// <summary>
        /// Produces mean values for a sample of double values
        /// </summary>
        /// <param name="sample">Sampled values</param>
        /// <returns>Expected value of sample</returns>
        public static double GetMean(ICollection<double> sample)
        {
            if (sample.Count == 0)
                return 0;

            return sample.Sum() / sample.Count;
        } // end of GetMean

        #endregion GetMean

        #region GetQuantile

        /// <summary>
        /// Produces a quantile for a given sample
        /// </summary>
        /// <param name="sample">The considered sample</param>
        /// <param name="quantile">The quantile that should be computed</param>
        /// <returns></returns>
        public static double GetQuantile(List<double> sample, double quantile)
        {
            if (quantile > 1 || quantile < 0)
                throw new InvalidOperationException("Quantile must be between 0 and 1!");

            if (sample.Count == 0)
                return 0;

            sample.Sort();

            return sample[(int)(sample.Count * quantile)];
        } // end of GetMean

        #endregion GetQuantile

        #region ResetSeed

        /// <summary>
        /// To reset the seed of the used random object
        /// </summary>
        public void ResetSeed(int seedValue)
        {
            _randomNumberGenerator = new Random(seedValue);
        } // end of ResetSeed

        #endregion ResetSeed
    }

    #endregion GeneralDistributions
}