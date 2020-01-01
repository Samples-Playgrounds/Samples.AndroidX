using System;
using System.Collections.Generic;
using System.Text;

using pdftron.Common;
using pdftron.SDF;

using TRN_PDFDocViewPrefs = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_Obj = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> PDFDocViewPrefs is a high-level utility class that can be 
    /// used to control the way the document is to be presented on 
    /// the screen or in print.
    /// 
    /// PDFDocViewPrefs class corresponds to PageMode, PageLayout, and
    /// ViewerPreferences entries in the document’s catalog. For more 
    /// details please refer to section 8.1 'Viewer Preferences' in 
    /// PDF Reference Manual.
    /// </summary>
    public class PDFDocViewPrefs : IDisposable
    {
        internal TRN_PDFDocViewPrefs mp_prefs = IntPtr.Zero;
        internal Object m_ref;

        /// <summary> Releases all resources used by the PDFDocViewPrefs </summary>
        ~PDFDocViewPrefs()
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
            if (mp_prefs != IntPtr.Zero)
            {
                mp_prefs = IntPtr.Zero;
            }
        }

        public PDFDocViewPrefs() { }

        public PDFDocViewPrefs(Obj tr)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocViewPrefsCreate(tr.mp_obj, ref mp_prefs));
            m_ref = tr.GetRefHandleInternal();
        }

        /// <summary> A utility method used to set the fist page displayed after
	    /// the document is opened. This method is equivalent to
	    /// <c>PDFDoc::SetOpenAction(goto_action)</c>
	    /// 
	    /// If OpenAction is not specified the document should be
	    /// opened to the top of the first page at the default magnification
	    /// factor.		
	    /// </summary>
	    /// <param name="dest">A value specifying the page destination to be
	    /// displayed when the document is opened.		
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <example>
	    /// <code>  
	    /// Destination dest = Destination::CreateFit(page);
	    /// pdfdoc.GetViewPrefs().SetInitialPage(dest);
        /// </code>
        /// </example>
        public void SetInitialPage(Destination dest)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocViewPrefsSetInitialPage(mp_prefs, dest.mp_dest));
        }

	    /// <summary> Sets PageMode property and change the value of the
	    /// PageMode key in the Catalog dictionary.
	    /// 
	    /// </summary>
	    /// <param name="mode">New PageMode setting. Default value is e_UseNone.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetPageMode(PageMode mode)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocViewPrefsSetPageMode(mp_prefs, mode));
        }

	    /// <summary> Gets the page mode.
	    /// 
	    /// </summary>
	    /// <returns> The value of currently selected PageMode property.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public PageMode GetPageMode()
        {
            PageMode result = PageMode.e_UseNone;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocViewPrefsGetPageMode(mp_prefs, ref result));
            return result;
        }

	    /// <summary> Sets PageLayout property and change the value of the
	    /// PageLayout key in the Catalog dictionary.
	    /// 
	    /// </summary>
	    /// <param name="layout">the new layout mode
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetLayoutMode(PageLayout layout)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocViewPrefsSetLayoutMode(mp_prefs, layout));
        }
	    /// <summary> Gets the layout mode.
	    /// 
	    /// </summary>
	    /// <returns> The value of currently selected PageLayout property.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public PageLayout GetLayoutMode()
        {
            PageLayout result = PageLayout.e_Default;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocViewPrefsGetLayoutMode(mp_prefs, ref result));
            return result;
        }

	    /// <summary> Sets the value of given ViewerPref property.		
	    /// </summary>
	    /// <param name="pref">the ViewerPref property type to modifiy.
	    /// </param>
	    /// <param name="value">The new value for the property.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetPref(ViewerPref pref, bool value)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocViewPrefsSetPref(mp_prefs, pref, value));
        }
	    /// <summary> Gets the pref.
	    /// 
	    /// </summary>
	    /// <param name="pref">the ViewerPref property type to query.
	    /// </param>
	    /// <returns> the value of given ViewerPref property.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool GetPref(ViewerPref pref)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocViewPrefsGetPref(mp_prefs, pref, ref result));
            return result;
        }

	    /// <summary> Set the document’s page mode, specifying how to display the
	    /// document on exiting full-screen mode.
	    /// 
	    /// </summary>
	    /// <param name="mode">PageMode used after exiting full-screen mode.
	    /// Default value: e_UseNone.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks> This entry is meaningful only if the value of the 
        /// PageMode is set to e_FullScreen; it is ignored otherwise.
        /// </remarks>
        public void SetNonFullScreenPageMode(PageMode mode)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocViewPrefsSetNonFullScreenPageMode(mp_prefs, mode));
        }
	    /// <summary> Gets the non full screen page mode.
	    /// 
	    /// </summary>
	    /// <returns> the PageMode used after exiting full-screen mode.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks> This entry is meaningful only if the value of the 
        /// PageMode is set to e_FullScreen; it is ignored otherwise.
        /// </remarks>
        public PageMode GetNonFullScreenPageMode()
        {
            PageMode result = PageMode.e_UseNone;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocViewPrefsGetNonFullScreenPageMode(mp_prefs, ref result));
            return result;
        }
	    /// <summary> Sets the predominant reading order for text.
	    /// 
	    /// This flag has no direct effect on the document’s contents
	    /// or page numbering but can be used to determine the relative
	    /// positioning of pages when displayed side by side or
	    /// printed n-up.
	    /// 
	    /// </summary>
	    /// <param name="left_to_right">- true if the predominant reading
	    /// order for text is from left to right and false if it is
	    /// right to left (including vertical writing systems, such
	    /// as Chinese, Japanese, and Korean).		
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetDirection(bool left_to_right)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocViewPrefsSetDirection(mp_prefs, left_to_right));
        }
	    /// <summary> Gets the direction.
	    /// 
	    /// </summary>
	    /// <returns> true is the predominant reading order for text
	    /// is left to right, false otherwise. See SetDirection() for
	    /// more information.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool GetDirection()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocViewPrefsGetDirection(mp_prefs, ref result));
            return result;
        }
	    /// <summary> Sets the page boundary representing the area of a page
	    /// to be displayed when viewing the document on the screen.
	    /// 
	    /// </summary>
	    /// <param name="box">page boundary displayed when viewing the document
	    /// on the screen. By defualt, PDF viewers will display the
	    /// crop-box.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetViewArea(Page.Box box)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocViewPrefsSetViewArea(mp_prefs, box));
        }
	    /// <summary> Gets the view area.
	    /// 
	    /// </summary>
	    /// <returns> the page boundary representing the area of a page
	    /// to be displayed when viewing the document on the screen.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Page.Box GetViewArea()
        {
            Page.Box result = Page.Box.e_media;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocViewPrefsGetViewArea(mp_prefs, ref result));
            return result;
        }
	    /// <summary> Sets the page boundary to which the contents of a page are
	    /// to be clipped when viewing the document on the screen.
	    /// 
	    /// </summary>
	    /// <param name="box">screen clip region. The default value is
	    /// page crop-box.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetViewClip(Page.Box box)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocViewPrefsSetViewClip(mp_prefs, box));
        }
	    /// <summary> Gets the view clip.
	    /// 
	    /// </summary>
	    /// <returns> the page boundary to which the contents of a page
	    /// are to be clipped when viewing the document on the screen.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Page.Box GetViewClip()
        {
            Page.Box result = Page.Box.e_media;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocViewPrefsGetViewClip(mp_prefs, ref result));
            return result;
        }
	    /// <summary> Sets the page boundary representing the area of a page to
	    /// be rendered when printing the document.
	    /// 
	    /// </summary>
	    /// <param name="box">printing region. The default value is page
	    /// crop-box.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetPrintArea(Page.Box box)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocViewPrefsSetPrintArea(mp_prefs, box));
        }
	    /// <summary> Gets the prints the area.
	    /// 
	    /// </summary>
	    /// <returns> the page boundary representing the area of a page
	    /// to be rendered when printing the document.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Page.Box GetPrintArea()
        {
            Page.Box result = Page.Box.e_media;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocViewPrefsGetPrintArea(mp_prefs, ref result));
            return result;
        }
	    /// <summary> Sets the page boundary to which the contents of a page are
	    /// to be clipped when printing the document.
	    /// 
	    /// </summary>
	    /// <param name="box">printing clip region. The default value is page
	    /// crop-box.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetPrintClip(Page.Box box)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocViewPrefsSetPrintClip(mp_prefs, box));
        }
	    /// <summary> Gets the prints the clip.
	    /// 
	    /// </summary>
	    /// <returns> the page boundary to which the contents of a page
	    /// are to be clipped when printing the document.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Page.Box GetPrintClip()
        {
            Page.Box result = Page.Box.e_media;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocViewPrefsGetPrintClip(mp_prefs, ref result));
            return result;
        }
	    /// <summary> Gets the SDFObj.
	    /// 
	    /// </summary>
	    /// <returns> document’s SDF/Cos 'ViewerPreferences' dictionary
	    /// or NULL if the object is not present.
	    /// </returns>
        public Obj GetSDFObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocViewPrefsGetSDFObj(mp_prefs, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
	
        // Nested Types
        /// <summary> PageMode specifies how the document should be displayed
	    /// when opened</summary>		
	    public enum PageMode 
	    {
		    ///<summary>Displays document, but neither thumbnails nor 
		    /// bookmarks.</summary>
		    e_UseNone,
		    ///<summary>Displays document plus thumbnails.</summary>
		    e_UseThumbs,
		    ///<summary>Displays document plus bookmarks.</summary>
		    e_UseBookmarks,
		    ///<summary>Displays document in full-screen viewing mode.</summary>
		    e_FullScreen,
		    ///<summary>Displays Optional Content (OC) group panel.</summary>
		    e_UseOC,
		    ///<summary>Displays attachments panel.</summary>
		    e_UseAttachments
	    };

	    /// <summary> PageLayout specifies the page layout to be used when the
	    /// document is opened</summary>		
	    public enum PageLayout 
	    {
		    ///<summary>PageLayout is not explicitly specified, use user defined
		    /// preferred layout.</summary>
		    e_Default,
		    ///<summary>Display one page at a time </summary>
		    e_SinglePage,
		    ///<summary>Display the pages in one-column 
		    /// mode.</summary>
		    e_OneColumn,
		    ///<summary>Display the pages in two-column continuous 
		    /// mode with first page on left (i.e. all odd numbered pages 
		    /// on the left).</summary>
		    e_TwoColumnLeft,
		    ///<summary>Display the pages in two-column 
		    /// continuous mode with first page on right (i.e. all odd 
		    /// numbered pages on the right).</summary>
		    e_TwoColumnRight,
		    ///<summary>Display the pages two at a time, with 
		    /// odd-numbered pages on the left.</summary>
		    e_TwoPageLeft,
		    ///<summary>Display the pages two at a time, with 
		    /// odd-numbered pages on the right.</summary>
		    e_TwoPageRight
	    };

        /// <summary> ViewerPref enumeration specifies how various GUI elements 
	    /// should behave when the document is opened.</summary>
	    public enum ViewerPref 
	    {
		    ///<summary>A enumerator specifying whether to hide the 
		    /// viewer application’s toolbars when the document is active. </summary>
		    e_HideToolbar, 
		    ///<summary>A enumerator specifying whether to hide the viewer 
		    /// application’s menu bar when the document is active. </summary>
		    e_HideMenubar, 
		    ///<summary>A enumerator specifying whether to hide user 
		    /// interface elements in the document’s window (such as scroll 
		    /// bars and navigation controls), leaving only the document’s 
		    /// contents displayed</summary>
		    e_HideWindowUI,
		    ///<summary>A enumerator specifying whether to resize the 
		    /// document’s window to fit the size of the first displayed 
		    /// page.</summary>
		    e_FitWindow,
		    ///<summary>A enumerator specifying whether to resize the 
		    /// document’s window to fit the size of the first displayed 
		    /// page.</summary>
		    e_CenterWindow,
		    ///<summary>A flag specifying whether the window’s 
		    /// title bar should display the document title taken from the 
		    /// Title entry of the document information dictionary. 
		    /// If false, the title bar should instead display the name of 
		    /// the PDF file containing the document.</summary>
		    e_DisplayDocTitle,
	    };
    }
}
