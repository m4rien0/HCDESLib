using Enums;
using GeneralHealthCareElements.BookingModels;
using SimulationCore.HCCMElements;
using System.Collections.Generic;

namespace GeneralHealthCareElements.SpecialFacility
{
    /// <summary>
    /// Delagate that sends slot availabilities for requests of special service
    /// </summary>
    public class DelegateAvailabilitiesForRequest : IDelegate
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="serviceControl">Control providing the special service</param>
        /// <param name="originalServiceRequest">The original request that was filed</param>
        /// <param name="bookingType">The booking type associated, either scheduled or immediate</param>
        /// <param name="slots">A list of available slots if not immediate</param>
        public DelegateAvailabilitiesForRequest(ControlUnitSpecialServiceModel serviceControl,
            RequestSpecialFacilitiyService originalServiceRequest,
            SpecialServiceBookingTypes bookingType,
            List<Slot> slots)
        {
            _serviceControl = serviceControl;
            _originalServiceRequest = originalServiceRequest;
            _bookingType = bookingType;
            _slots = slots;
        } // end of

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // OriginalRequest
        //--------------------------------------------------------------------------------------------------

        #region OriginalServiceRequest

        private RequestSpecialFacilitiyService _originalServiceRequest;

        /// <summary>
        /// The original request that was filed
        /// </summary>
        public RequestSpecialFacilitiyService OriginalServiceRequest
        {
            get
            {
                return _originalServiceRequest;
            }
        } // end of OriginalServiceRequest

        #endregion OriginalServiceRequest

        //--------------------------------------------------------------------------------------------------
        // Parameters
        //--------------------------------------------------------------------------------------------------

        #region ServiceControl

        private ControlUnitSpecialServiceModel _serviceControl;

        /// <summary>
        /// Control providing the special service
        /// </summary>
        public ControlUnitSpecialServiceModel ServiceControl
        {
            get
            {
                return _serviceControl;
            }
            set
            {
                _serviceControl = value;
            }
        } // end of ServiceControl

        #endregion ServiceControl

        #region BookingType

        private SpecialServiceBookingTypes _bookingType;

        /// <summary>
        /// The booking type associated, either scheduled or immediate
        /// </summary>
        public SpecialServiceBookingTypes BookingType
        {
            get
            {
                return _bookingType;
            }
        } // end of BookingType

        #endregion BookingType

        #region Slots

        private List<Slot> _slots;

        /// <summary>
        /// A list of available slots if not immediate
        /// </summary>
        public List<Slot> Slots
        {
            get
            {
                return _slots;
            }
        } // end of Times

        #endregion Slots

        //--------------------------------------------------------------------------------------------------
        // Interface Properties
        //--------------------------------------------------------------------------------------------------

        #region OriginControlUnit

        /// <summary>
        /// Control unit sending the delegate
        /// </summary>
        public ControlUnit OriginControlUnit
        {
            get
            {
                return _serviceControl;
            }
        } // end of OriginControlUnit

        #endregion OriginControlUnit
    } // end of DelegateAvailabilitiesForRequest
}