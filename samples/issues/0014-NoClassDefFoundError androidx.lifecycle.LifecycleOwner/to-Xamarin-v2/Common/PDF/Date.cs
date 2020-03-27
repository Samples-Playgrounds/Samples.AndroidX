using System;
using System.Runtime.InteropServices;

using pdftron.Common;
using pdftronprivate.trn;

using TRN_Exception = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> The Date class is a utility class used to simplify work with PDF date objects.
    /// 
    /// PDF defines a standard date format, which closely follows international
    /// standard ASN.1 (Abstract Syntax Notation One), A date is a string of the form
    /// (D:YYYYMMDDHHmmSSOHH'mm'); See PDF Reference Manual for details. 
    /// 
    /// Date can be associated with a SDF/Cos date string using Date(Obj*) constructor 
    /// or later using Date::Attach(Obj*) or Date::Update(Obj*) methods. 
    /// 
    /// Date keeps a local date/time cache so it is necessary to call Date::Update() 
    /// method if the changes to the Date should be saved in the attached Cos/SDF string.
    /// </summary>
    public class Date : IDisposable
    {
        internal BasicTypes.TRN_Date mp_date;

        /// <summary> Releases all resources used by the Date </summary>
        ~Date()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (disposing)
            {
                // Dispose managed resources.
            }

            // Clean up native resources
            if (mp_date.mp_obj != IntPtr.Zero)
            {
                mp_date = new BasicTypes.TRN_Date();
                mp_date.mp_obj = IntPtr.Zero;
            }
        }

        // Methods
        internal Date(BasicTypes.TRN_Date imp)
        {
            this.mp_date = imp;
        }

        /// <summary> Date default constructor.
        /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Date()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_DateInit(2008, 1, 1, 0, 0, 0, ref mp_date));
        }
        /// <summary> Create a Date and initialize it using given Cos/SDF string date object.
        /// String date object is attached to this Date.
        /// 
        /// </summary>
        /// <param name="d">the <c>SDF::Obj</c> date object
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Date(SDF.Obj d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_DateAttach(ref mp_date, d.mp_obj));
        }
        /// <summary> Create a Date and initialize it using specified parameters.
        /// The Date is not attached to any Cos/SDF object.
        /// 
        /// </summary>
        /// <param name="year">the year
        /// </param>
        /// <param name="month">the month
        /// </param>
        /// <param name="day">the day
        /// </param>
        /// <param name="hour">the hour
        /// </param>
        /// <param name="minute">the minute
        /// </param>
        /// <param name="second">the second
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Date(short year, byte month, byte day, byte hour, byte minute, byte second)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_DateInit(System.Convert.ToUInt16(year), month, day, hour, minute, second, ref mp_date));
        }

        /// <summary>Creates a <c>Date</c> object from specified <c>Date</c> object
	    /// </summary>
	    /// <param name="d"><c>Date</c> object
	    /// </param>
	    public Date(Date d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_DateAssign(ref mp_date, ref d.mp_date));
        }

        /// <summary>Sets value to given <c>Date</c> object
	    /// </summary>
        /// <param name="p">given <c>Date</c> object
        /// </param>
        public void Set(Date p)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_DateAssign(ref mp_date, ref p.mp_date));
        }
	    //static Date Assign(Date l, Date r);
	    /// <summary>Assignment operator</summary>
	    /// <param name="r">object at the right of the operator
	    /// </param>
        /// <returns>object equals to the given object
        /// </returns>
        public Date op_Assign(Date r)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_DateAssign(ref mp_date, ref r.mp_date));
            return this;
        }

        /// <summary>Indicates whether the Date is valid (non-null).</summary>
        /// <returns>True if this is a valid (non-null) Date; otherwise false.</returns>
        /// <remarks>If this method returns false the underlying SDF/Cos object is null and 
        /// the Date object should be treated as null as well.</remarks>
        public Boolean IsValid()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DateIsValid(ref mp_date, ref result));
            return result;
        }

        /// <summary> Attach the Cos/SDF object to the Date.
        /// 
        /// </summary>
        /// <param name="d">- underlying Cos/SDF object. Must be an SDF::Str containing
        /// a PDF date object.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Attach(SDF.Obj d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_DateAttach(ref mp_date, d.mp_obj));
        }
	    /// <summary> Saves changes made to the Date object in the attached (or specified) SDF/Cos string.
	    /// 
	    /// </summary>
	    /// <returns> true if the attached Cos/SDF string was successfully updated, false otherwise.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool Update()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DateUpdate(ref mp_date, IntPtr.Zero, ref result));
            return result;
        }
	    /// <summary> Update.
	    /// 
	    /// </summary>
	    /// <param name="d">the d
	    /// </param>
	    /// <returns> true, if successful
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool Update(SDF.Obj d)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DateUpdate(ref mp_date, d.mp_obj, ref result));
            return result;
        }
	    /// <summary> Sets the current time.</summary>
        public void SetCurrentTime()
        {
            PDFNetPINVOKE.TRN_DateSetCurrentTime(ref mp_date);
        }

        // Properties
        public byte day
        {
            get { return PDFNetPINVOKE.TRN_DateGetDay(ref mp_date); }
            set { mp_date.day = value; }
        }
        public byte hour
        {
            get { return PDFNetPINVOKE.TRN_DateGetHour(ref mp_date); }
            set { mp_date.hour = value; }
        }
        public byte minute
        {
            get { return PDFNetPINVOKE.TRN_DateGetMinute(ref mp_date); }
            set { mp_date.minute = value; }
        }
        public byte month
        {
            get { return PDFNetPINVOKE.TRN_DateGetMonth(ref mp_date); }
            set { mp_date.month = value; }
        }
        public byte second
        {
            get { return PDFNetPINVOKE.TRN_DateGetSecond(ref mp_date); }
            set { mp_date.second = value; }
        }
        public byte UT
        {
            get { return PDFNetPINVOKE.TRN_DateGetUT(ref mp_date); }
            set { mp_date.UT = value; }
        }
        public byte UT_hour
        {
            get { return PDFNetPINVOKE.TRN_DateGetUTHour(ref mp_date); }
            set { mp_date.UT_hour = value; }
        }
        public byte UT_minutes
        {
            get { return PDFNetPINVOKE.TRN_DateGetUTMin(ref mp_date); }
            set { mp_date.UT_minutes = value; }
        }
        public short year
        {
            get { return System.Convert.ToInt16(PDFNetPINVOKE.TRN_DateGetYear(ref mp_date)); }
            set { mp_date.year = System.Convert.ToUInt16(value); }
        }

    }
}
