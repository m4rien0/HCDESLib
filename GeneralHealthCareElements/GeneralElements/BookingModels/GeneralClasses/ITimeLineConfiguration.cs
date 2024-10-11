using System;

namespace GeneralHealthCareElements.BookingModels
{
    /// <summary>
    /// Interface to define a time line that is basis for a booking model. A time line represents a stretch of time that hosts slots
    /// for booking, e.g. a day or a week.
    /// </summary>
    public interface ITimeLineConfiguration
    {
        /// <summary>
        /// For initialization the first time line of a configuration is needed
        /// </summary>
        /// <param name="startTime">Start time of the configuration</param>
        /// <returns>Returns the first time line that might include the starting date</returns>
        SinglePerDayTimeLine GetFirstTimeLine(DateTime startTime);

        /// <summary>
        /// Takes the last timeline as an input and generates the followng time line
        /// </summary>
        /// <param name="lastTimeLine">Previous time line</param>
        /// <returns>Next time line</returns>
        SinglePerDayTimeLine GetNextTimeLine(SinglePerDayTimeLine lastTimeLine);
    } // end of ITimeLineConfiguration
}