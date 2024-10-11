using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.GeneralClasses.ActionTypesAndPaths;
using GeneralHealthCareElements.ResourceHandling;
using GeneralHealthCareElements.StaffHandling;
using System;

namespace GeneralHealthCareElements.Input
{
    /// <summary>
    /// Interface to define input requirements of a health care department model
    /// </summary>
    public interface IInputHealthCareDepartment
    {
        /// <summary>
        /// Returns the time consumed for an action type, patient and consuming resources
        /// </summary>
        /// <param name="patient">Patient of action</param>
        /// <param name="resources">Resource set of action</param>
        /// <param name="actionType">Type of action</param>
        /// <returns></returns>
        TimeSpan PatientActionTime(EntityPatient patient,
            ResourceSet resources,
            ActionTypeClass actionType);

        /// <summary>
        /// Defines the staff availability for the operational health care control unit
        /// </summary>
        IStaffHandling StaffHandler { get; }

        /// <summary>
        /// StructuralArea Identifiers
        /// </summary>
        /// <returns></returns>
        string[] GetStructuralAreaIdentifiers();

        /// <summary>
        /// Return TreatmentFacilities of control Unit
        /// </summary>
        /// <returns></returns>
        ResourceAssignmentPhysical<EntityTreatmentFacility>[] GetTreatmentFacilities();

        /// <summary>
        /// Return WaitingRooms for Patients of control Unit
        /// </summary>
        /// <returns></returns>
        ResourceAssignmentPhysical<EntityWaitingArea>[] GetWaitingRoomPatients();

        /// <summary>
        /// Return WaitingRooms for Staff of control Unit
        /// </summary>
        /// <returns></returns>
        ResourceAssignmentPhysical<EntityWaitingArea>[] GetWaitingRoomsStaff();
    } // end of IInputHealthCare
}