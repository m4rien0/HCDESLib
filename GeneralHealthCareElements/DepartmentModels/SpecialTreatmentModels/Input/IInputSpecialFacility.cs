using GeneralHealthCareElements.DepartmentModels.Outpatient;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.Input;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.SpecialFacility
{
    public interface IInputSpecialFacility : IInputHealthCareDepartment, IInputBookingModel
    {
        /// <summary>
        /// Defines admission types handled by model
        /// </summary>
        /// <returns></returns>
        SpecialServiceAdmissionTypes[] GetAdmissionTypes();

        /// <summary>
        /// Creates special patient path, may use core path creating method of abstract
        /// patient path.
        /// </summary>
        /// <param name="patient">Patient for which path is created</param>
        /// <param name="admission">Admission type for path</param>
        /// <param name="originalRequest">Original service request that is the basis of referral</param>
        /// <returns>New patient path</returns>
        SpecialServicePatientPath CreatePatientPath(SpecialServiceAdmissionTypes admission, 
            EntityPatient patient, 
            RequestSpecialFacilitiyService originalRequest);

    } // end of IInputSpecialTreatment
}
