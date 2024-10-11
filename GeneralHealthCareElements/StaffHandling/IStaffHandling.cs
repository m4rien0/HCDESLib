using GeneralHealthCareElements.ControlUnits;
using GeneralHealthCareElements.ResourceHandling;
using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;

namespace GeneralHealthCareElements.StaffHandling
{
    public interface IStaffHandling
    {
        /// <summary>
        /// Returns staff at the start time of a simulation
        /// </summary>
        /// <param name="startTime">STart time of the simulation</param>
        /// <returns></returns>
        List<ResourceAssignmentStaff> GetStartingStaff(DateTime startTime);

        // gets an event that represents actions for the next point in time
        // where staffing levels change

        /// <summary>
        /// Gets an event that represents actions for the next point in time
        /// where staffing levels change
        /// </summary>
        /// <param name="parentControl">Control for which the staff hanlder is called</param>
        /// <param name="time">Time for which the next event (in the future with respect to this time) is requested</param>
        /// <param name="timeToOccur">Out parameter to represent the time the next change will occur</param>
        /// <returns>An event that triggers the staff change</returns>
        Event GetNextStaffChangingEvent(ControlUnitHealthCareDepartment parentControl, DateTime time, out DateTime timeToOccur);
    } // end if IStaffHandling
}