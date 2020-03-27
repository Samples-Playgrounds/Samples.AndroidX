using System;
using System.Collections.Generic;
using System.Text;

using TRN_ProgressMonitor = System.IntPtr;

namespace pdftron.Common
{
    /// <summary> ProgressMonitor is an interface that an application can use to indicate the
    /// progress of a lengthy operation (such as PDFDoc.Save()).
    /// 
    /// ProgressMonitor has a range and a current position. The range represents the
    /// entire duration of the operation, and the current position represents the
    /// progress the application has made toward completing the operation.
    /// </summary>
    public abstract class ProgressMonitor : IDisposable
    {
        internal TRN_ProgressMonitor mp_imp = IntPtr.Zero;

        ~ProgressMonitor()
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
            if (mp_imp != IntPtr.Zero)
            {
                mp_imp = IntPtr.Zero;
            }
        }
        /// <summary> Instantiates a default ProgressMonitor object </summary>
        public ProgressMonitor() { }
        /// <summary> Get the current position of the progress monitor.
        /// 
        /// </summary>
        /// <returns> position of the progress monitor
        /// </returns>
        public abstract int GetPos();
        /// <summary> get upper and lower limit of the progress monitor
        /// 
        /// </summary>
        /// <param name="start">lower limit of the range
        /// </param>
        /// <param name="finish">upper limit of the range
        /// </param>
        public abstract void GetRange(ref int start, ref int finish);
        /// <summary> Advances the current position of a progress monitor by a specified
        /// increment and redraws the bar to reflect the new position.
        /// 
        /// </summary>
        /// <param name="offset">advances the current position of a progress bar control by a
        /// specified increment and redraws the bar to reflect the new
        /// position.
        /// </param>
        /// <returns>the previous position of the progress monitor
        /// </returns>
        public abstract int OffsetPos(int offset);
        /// <summary> Set the current position for a progress monitor and redraws the bar to
        /// reflect the new ranges.
        /// 
        /// </summary>
        /// <param name="pos"> the new position of the progress monitor
        /// </param>
        /// <returns> the previous position of the progress monitor
        /// </returns>
        public abstract int SetPos(int pos);
        /// <summary> set upper and lower limit of the progress monitor
        /// 
        /// </summary>
        /// <param name="start">lower limit of the range
        /// </param>
        /// <param name="finish">upper limit of the range
        /// </param>
        public abstract void SetRange(int start, int finish);
        /// <summary> Specifies the step increment for a progress bar monitor.
        /// 
        /// </summary>
        /// <param name="nstep">new step increment.
        /// </param>
        /// <returns>the previous step increment
        /// </returns>
        public abstract int SetStep(int nstep);
        /// <summary> Advances the current position for a progress monitor by the step
        /// increment (see SetStep) and redraws the monitor to reflect the new
        /// position.
        /// 
        /// </summary>
        /// <returns> new postion after stepping
        /// </returns>
        public abstract int StepIt();

    }
}
