using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.SDF;

using TRN_Annot = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_Obj = System.IntPtr;
using TRN_Page = System.IntPtr;
using TRN_AnnotBorderStyle = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> Annot is a base class for different types of annotations. For annotation 
    /// specific properties, please refer to derived classes. 
    /// 
    /// An annotation is an interactive object placed on a page, such as a text note, a link, 
    /// or an embedded file. PDF includes a wide variety of standard annotation types. 
    /// An annotation associates an object such as a widget, note, or movie with a location
    /// on a page of a PDF document, or provides a means of interacting with the user
    /// via the mouse and keyboard. For more details on PDF annotations please refer to 
    /// section 12.5 in the PDF Reference Manual and the 
    /// </summary>
    public class Annot : IDisposable
    {
        // Fields
        internal TRN_Annot mp_annot = IntPtr.Zero;
        internal Object m_ref;

        /// <summary> Releases all resources used by the Annot </summary>
        ~Annot()
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
            if (mp_annot != IntPtr.Zero)
            {
                mp_annot = IntPtr.Zero;
            }
        }

        // Methods
		internal Annot(TRN_Annot imp, Object reference)
        {
            this.mp_annot = imp;
            this.m_ref = reference;
        }
        public IntPtr GetHandleInternal()
        {
            return mp_annot;
        }

        public Object GetRefHandleInternal()
        {
            return this.m_ref;
        }
        /// <summary>
        /// Create a new annotation object using native impl pointer
        /// For internal use only.
        /// </summary>
        /// <param name="imp">impl pointer from PDFNet core.</param>
        public static Annot CreateInternal(TRN_Annot imp, Object reference) {
			return new Annot (imp, reference);
		}
			
        /// <summary> Creates a new annotation of a given type, in the specified document.
        /// Because the newly created annotation does not contain any properties specific
        /// to a given annotation type, it is faster to create an annotation using type specific
        /// Annot.Create method.
        /// 
        /// </summary>
        /// <param name="doc">A document to which the annotation is added.
        /// </param>
        /// <param name="type">Subtype of annotation to create.
        /// </param>
        /// <param name="pos">A rectangle specifying the annotation's bounds, specified in
        /// user space coordinates.
        /// </param>
        /// <returns> A newly created blank annotation for the given annotation type.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Annot Create(SDFDoc doc, Type type, Rect pos)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotCreate(doc.mp_doc, type, ref pos.mp_imp, ref result));
            return result != IntPtr.Zero ? new Annot(result, doc) : null;
        }

        //Annot (Obj d);
        /// <summary> Create an annotation and initialize it using given Cos/SDF object.
        /// 
        /// </summary>
        /// <param name="b">a <c>Obj</c> object
        /// </param>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public Annot(Obj b)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotCreateFromObj(b.mp_obj, ref mp_annot));
            this.m_ref = b.GetRefHandleInternal();
        }
        ///<summary>Assignment operator</summary>
        ///<param name="r">a <c>Annot</c> object
        ///</param>
        ///<returns>a <c>Annot</c>object equals to the given one
        ///</returns>
        public Annot op_Assign(Annot r)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotCopy(r.mp_annot, ref mp_annot));
            return this;
        }

        ///<summary>Equality operator checks whether two Annot objects are the same</summary>
        ///<param name="l">the <c>Annot</c> object on the left of operator
        ///</param>
        ///<param name="r">the <c>Annot</c> object on the right of operator
        ///</param>
        ///<returns>true if both objects are equal, false otherwise
        ///</returns>
        public static bool operator ==(Annot l, Annot r)
        {
            if (System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null))
                return true;
            if (System.Object.ReferenceEquals(l, null) && !System.Object.ReferenceEquals(r, null))
                return false;
            if (!System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null))
                return false;

            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotCompare(l.mp_annot, r.mp_annot, ref result));
            return result;
        }

        ///<summary>Inequality operator checks whether two Annot objects are different</summary>
        ///<param name="l">the <c>Annot</c> object on the left of operator
        ///</param>
        ///<param name="r">the <c>Annot</c> object on the right of operator
        ///</param>
        ///<returns>true if both objects are not equal, false otherwise
        ///</returns>
        public static bool operator !=(Annot l, Annot r)
        {
            return !(l == r);
        }

        /// <param name="o">a given <c>Object</c>
        /// </param>
        /// <returns>true, if equals to the given object
        /// </returns>
        public override bool Equals(Object o)
        {
            if (o == null) return false;
            Annot i = o as Annot;
            if (i == null) return false;
            bool b = mp_annot == i.mp_annot;
            return b;
        }

        /// <summary> Checks if is valid.
        /// 
        /// </summary>
        /// <returns> True if this is a valid (non-null) annotation, false otherwise.
        /// If the function returns false the underlying SDF/Cos object is null or is
        /// not valid and the annotation object should be treated as a null object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool IsValid()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotIsValid(mp_annot, ref result));
            return result;
        }

        /// <summary> Gets the SDFObj.
        /// 
        /// </summary>
        /// <returns> The underlying SDF/Cos object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetSDFObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotGetSDFObj(mp_annot, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }

        /// <summary> Gets the type.
        /// 
        /// </summary>
        /// <returns> The type of this annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public new Type GetType()
        {
            Type result = Type.e_Unknown;
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotGetType(mp_annot, ref result));
            return result;
        }

        /// <summary> Checks if is markup.
        /// 
        /// </summary>
        /// <returns> true, if is markup
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool IsMarkup()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotIsMarkup(mp_annot, ref result));
            return result;
        }

        /// <summary> Gets the rect.
        /// 
        /// </summary>
        /// <returns> Annotation’s bounding rectangle, specified in user space coordinates.
        /// 
        /// The meaning of the rectangle depends on the annotation type. For Link and RubberStamp
        /// annotations, the rectangle specifies the area containing the hyperlink area or stamp.
        /// For Note annotations, the rectangle is describing the popup window when it's opened.
        /// When it's closed, the icon is positioned at lower left corner.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Rect GetRect()
        {
            BasicTypes.TRN_Rect result = new BasicTypes.TRN_Rect();
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotGetRect(mp_annot, ref result));
            return new Rect(result);
        }

        /// <summary> It is possible during viewing that GetRect does not return the most accurate bounding box
        /// of what is actually rendered. This method calculates the bounding box, rather than relying
        /// on what is specified in the PDF document. The bounding box is defined as the smallest
        /// rectangle that includes all the visible content on the annotation.
        /// 
        /// </summary>
        /// <returns> the bounding box for this annotation. The dimensions are specified in user space
        /// coordinates.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Rect GetVisibleContentBox()
        {
            BasicTypes.TRN_Rect result = new BasicTypes.TRN_Rect();
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotGetVisibleContentBox(mp_annot, ref result));
            return new Rect(result);
        }

        //void SetRect(Rect& p);
        /// <summary> Sets the size and location of an annotation on its page.
        /// 
        /// </summary>
        /// <param name="pos">the new rect
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetRect(Rect pos)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotSetRect(mp_annot, ref pos.mp_imp));
        }

        /// <summary> Gets the contents.
        /// 
        /// </summary>
        /// <returns> the contents
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public string GetContents()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotGetContents(mp_annot, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

        /// <summary> Sets the contents.
        /// 
        /// </summary>
        /// <param name="contents">the new contents
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetContents(string contents)
        {
            using (UString str = new UString(contents))
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotSetContents(mp_annot, str.mp_impl));
            }
        }

        /// <summary> Gets the page the annotation is associated with.
        /// 
        /// </summary>
        /// <returns> A Page object or NULL if the page reference is not available.
        /// The page object returned is an indirect reference to the page object with which
        /// this annotation is associated.
        /// This entry shall be present in screen annotations associated with rendition actions.
        /// 
        /// Optional. PDF 1.3 PDF 1.4 PDF 1.5 not used in FDF files.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Page GetPage()
        {
            TRN_Page result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotGetPage(mp_annot, ref result));
            return result != IntPtr.Zero ? new Page(result, this.m_ref) : null;
        }

        /// <summary> Sets the reference to a page the annotation is associated with.
        /// (Optional PDF 1.3; not used in FDF files)
        /// 
        /// </summary>
        /// <param name="page">the new page
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The parameter should be an indirect reference to the page object with 
        /// which this annotation is associated. This entry shall be present in screen
        /// annotations associated with rendition actions</remarks>
        public void SetPage(Page page)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotSetPage(mp_annot, page.mp_page));
        }

        /// <summary> Gets the unique ID.
        /// 
        /// </summary>
        /// <returns> The unique identifier for this annotation, or NULL if the identifier is not
        /// available. The returned value is a string object and is the value of the "NM"
        /// field, which was added as an optional attribute in PDF 1.4.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetUniqueID()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotGetUniqueID(mp_annot, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }

        /// <summary> Sets the unique identifier for this annotation.
        /// 
        /// </summary>
        /// <param name="data">the new unique id
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  It is necessary to ensure that the unique ID generated is actually unique. </remarks>
        public void SetUniqueID(string data)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotSetUniqueID(mp_annot, data, 0));
        }

        /// <summary> Gets an annotation's last modified date.
        /// 
        /// </summary>
        /// <returns> The annotation's time and date. If the annotation has no associated date
        /// structure, the returned date is not valid (date.IsValid() returns false). Corresponds 
        /// to the "M" entry of the annotation dictionary.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Date GetDate()
        {
            BasicTypes.TRN_Date result = new BasicTypes.TRN_Date();
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotGetDate(mp_annot, ref result));
            return new Date(result);
        }

        //void SetDate(Date& date);
        /// <summary> Sets an annotation's last modified date.
        /// 
        /// </summary>
        /// <param name="date"> The annotation's last modified time and date. Corresponds to the 
        /// "M" entry of the annotation dictionary.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetDate(Date date)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotSetDate(mp_annot, ref date.mp_date));
        }
        /// <summary> Gets the flag.
        /// 
        /// </summary>
        /// <param name="flag">The Flag property to query.
        /// </param>
        /// <returns> The value of given Flag
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool GetFlag(Flag flag)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotGetFlag(mp_annot, flag, ref result));
            return result;
        }

        /// <summary> Sets the value of given Flag.
        /// 
        /// </summary>
        /// <param name="flag">The Flag property to modify.
        /// </param>
        /// <param name="value">The new value for the property.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetFlag(Flag flag, bool value)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotSetFlag(mp_annot, flag, value));
        }
        /// <summary> Gets the border style for the annotation. Typically used for Link annotations.
        /// 
        /// </summary>
        /// <returns> Annotation's border style.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public BorderStyle GetBorderStyle()
        {
            TRN_AnnotBorderStyle result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotGetBorderStyle(mp_annot, ref result));
            return new BorderStyle(result);
        }
        /// <summary> Sets the border style for the annotation. Typically used for Link annotations.
        /// 
        /// </summary>
        /// <param name="bs">New border style for this annotation.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the pDF net exception </exception>
        public void SetBorderStyle(BorderStyle bs)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotSetBorderStyle(mp_annot, bs.mp_bs, false));
        }
        /// <summary> Gets the annotation’s appearance for the given combination of annotation
        /// and appearance states.
        /// 
        /// </summary>
        /// <returns> The appearance stream for this annotation, or NULL if the annotation
        /// does not have an appearance for the given combination of annotation and
        /// appearance states.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetAppearance()
        {
            return GetAppearance(AnnotationState.e_normal, null);
        }
        /// <summary> Gets the annotation’s appearance for the given combination of annotation
        /// and appearance states.
        /// 
        /// </summary>
        /// <param name="annot_state">The annotation’s appearance state, which selects the applicable
        /// appearance stream from the appearance sub-dictionary. An annotation can define as many
        /// as three separate appearances: The normal, rollover, and down appearance.
        /// </param>
        /// <returns> the appearance
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetAppearance(AnnotationState annot_state)
        {
            return GetAppearance(annot_state, null);
        }
        /// <summary> Gets the annotation’s appearance for the given combination of annotation
        /// and appearance states.
        /// 
        /// </summary>
        /// <param name="annot_state">The annotation’s appearance state, which selects the applicable
        /// appearance stream from the appearance sub-dictionary. An annotation can define as many
        /// as three separate appearances: The normal, rollover, and down appearance.
        /// </param>
        /// <param name="app_state">Is an optional parameter specifying the appearance state
        /// to look for (e.g. "Off", "On", etc). If appearance_state is NULL, the choice
        /// between different appearance states is determined by the AS (Appearance State)
        /// entry in the annotation dictionary.
        /// </param>
        /// <returns> The appearance stream for this annotation, or NULL if the annotation
        /// does not have an appearance for the given combination of annotation and
        /// appearance states.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetAppearance(AnnotationState annot_state, string app_state)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotGetAppearance(mp_annot, annot_state, app_state, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }

        //void SetAppearance(Obj app_stream, AnnotationState annot_state = e_normal, string app_state = 0);
        /// <summary> Sets the annotation’s appearance for the given combination of annotation
        /// and appearance states.
        /// (Optional; PDF 1.2)
        /// 
        /// </summary>
        /// <param name="app_stream">a content stream defining the new appearance.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetAppearance(Obj app_stream)
        {
            AnnotationState annot_state = AnnotationState.e_normal;
            string app_state = null;
            SetAppearance(app_stream, annot_state, app_state);
        }
        /// <summary> Removes the annotation's appearance for the given combination of annotation
        /// and appearance states.
        /// 
        /// </summary>
        /// <param name="app_stream">a content stream defining the new appearance.
        /// </param>
        /// <param name="annot_state">is an optional parameter specifying the appearance state
        /// (e.g. "Off", "On", etc) under which the new appearance should be stored. If
        /// appearance_state is NULL, the annotation will have only one annotaion state.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetAppearance(Obj app_stream, AnnotationState annot_state)
        {
            string app_state = null;
            SetAppearance(app_stream, annot_state, app_state);
        }
        /// <summary> Sets the annotation’s appearance for the given combination of annotation
        /// and appearance states.
        /// (Optional; PDF 1.2)
        /// 
        /// </summary>
        /// <param name="app_stream">a content stream defining the new appearance.
        /// </param>
        /// <param name="annot_state">the annotation’s appearance state, which selects the applicable
        /// appearance stream from the appearance sub-dictionary. An annotation can define as many
        /// as three separate appearances: The normal, rollover, and down appearance.
        /// </param>
        /// <param name="app_state">is an optional parameter specifying the appearance state
        /// (e.g. "Off", "On", etc) under which the new appearance should be stored. If
        /// appearance_state is NULL, the annotation will have only one annotaion state.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetAppearance(Obj app_stream, AnnotationState annot_state, string app_state)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotSetAppearance(mp_annot, app_stream.mp_obj, annot_state, app_state));
        }

        // void RemoveAppearance(AnnotationState annot_state = e_normal, string app_state = 0);
        /// <summary> Removes the annotation's appearance 			
        /// </summary>
        public void RemoveAppearance()
        {
            AnnotationState annot_state = AnnotationState.e_normal;
            string app_state = null;
            RemoveAppearance(annot_state, app_state);
        }
        /// <summary> Removes the annotation's appearance for the given combination of annotation
        /// and appearance states.
        /// 
        /// </summary>
        /// <param name="annot_state">the annotation's appearance state, which selects the applicable
        /// appearance stream from the appearance sub-dictionary. An annotation can define as many
        /// as three separate appearances: The normal, rollover, and down appearance.
        /// </param>			
        /// <exception cref="PDFNetException">  PDFNetException the pDF net exception </exception>
        public void RemoveAppearance(AnnotationState annot_state)
        {
            string app_state = null;
            RemoveAppearance(annot_state, app_state);
        }
        /// <summary> Removes the annotation's appearance for the given combination of annotation
        /// and appearance states.
        /// 
        /// </summary>
        /// <param name="annot_state">the annotation's appearance state, which selects the applicable
        /// appearance stream from the appearance sub-dictionary. An annotation can define as many
        /// as three separate appearances: The normal, rollover, and down appearance.
        /// </param>
        /// <param name="app_state">is an optional parameter specifying the appearance state
        /// (e.g. "Off", "On", etc) under which the new appearance should be stored. If
        /// appearance_state is NULL, the annotation will have only one annotaion state.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void RemoveAppearance(AnnotationState annot_state, string app_state)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotRemoveAppearance(mp_annot, annot_state, app_state));
        }

        //void Flatten(class Page page);
        /// <summary> Flatten/Merge the existing annotation appearances with the page content and
        /// delete this annotation from a given page.
        /// 
        /// Annotation 'flattening' refers to the operation that changes active annotations
        /// (such as markup, widgets, 3D models, etc.) into a static area that is part of the
        /// PDF document, just like the other text and images in the document.
        /// 
        /// </summary>
        /// <param name="page">the page
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  an alternative approach to set the annotation as read only is using </remarks>
        /// <remarks> Annot.SetFlag(Annot.e_read_only, true) method. Unlike Annot.SetFlag(...),
        /// the result of Flatten() operation can not be programatically reversed.</remarks>
        public void Flatten(Page page)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotFlatten(mp_annot, page.mp_page));
        }

        //cli.array<Byte> GetUniqueID();
        //string GetActiveAppearanceState() ;
        /// <summary> Gets the annotation’s active appearance state.
        /// 
        /// </summary>
        /// <returns> the name of the active state.
        /// The annotation’s appearance state, which
        /// selects the applicable appearance stream from an appearance subdictionary.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public string GetActiveAppearanceState()
        {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotGetActiveAppearanceState(mp_annot, ref result));
            return Marshal.PtrToStringUTF8(result);
        }

        //void SetUniqueID(cli.array<Byte> data);
        //void SetActiveAppearanceState(string astate);
        /// <summary> Sets the annotation’s active appearance state.
        /// (Required if the appearance dictionary AP contains one or more subdictionaries; PDF 1.2)
        /// 
        /// </summary>
        /// <param name="astate">astate Charactor string representing the name of the active appearance state.
        /// The string used to select the annotation’s appearance state, which
        /// selects the applicable appearance stream from an appearance subdictionary.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetActiveAppearanceState(string astate)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotSetActiveAppearanceState(mp_annot, astate));
        }

        /// <summary> Gets an annotation's color.
        /// 
        /// </summary>
        /// <returns> A ColorPt object containing an array of three numbers in the range 0.0 to 1.0,
        /// representing an RGB colour used for the following purposes:
        /// The background of the annotation’s icon when closed
        /// The title bar of the annotation’s pop-up window
        /// The border of a link annotation
        /// If the annotation does not specify an explicit color, a default color is returned.
        /// Text annotations return 'default yellow;' all others return black.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public ColorPt GetColorAsRGB()
        {
            ColorPt result = new ColorPt(0, 0, 0, 0);
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotGetColorAsRGB(mp_annot, result.mp_colorpt));
            return result;
        }
        /// <summary> Gets an annotation's color.
        /// 
        /// </summary>
        /// <returns> A ColorPt object containing an array of four numbers in the range 0.0 to 1.0,
        /// representing a CMYK colour used for the following purposes:
        /// The background of the annotation’s icon when closed
        /// The title bar of the annotation’s pop-up window
        /// The border of a link annotation
        /// If the annotation does not specify an explicit color, a default color is returned.
        /// Text annotations return 'default yellow;' all others return black.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public ColorPt GetColorAsCMYK()
        {
            ColorPt result = new ColorPt(0, 0, 0, 0);
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotGetColorAsCMYK(mp_annot, result.mp_colorpt));
            return result;
        }
        /// <summary> Gets an annotation's color.
        /// 
        /// </summary>
        /// <returns> A ColorPt object containing a number in the range 0.0 to 1.0,
        /// representing a Gray Scale colour used for the following purposes:
        /// The background of the annotation’s icon when closed
        /// The title bar of the annotation’s pop-up window
        /// The border of a link annotation
        /// If the annotation does not specify an explicit color, a default color is returned.
        /// Text annotations return 'default yellow;' all others return black.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public ColorPt GetColorAsGray()
        {
            ColorPt result = new ColorPt(0, 0, 0, 0);
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotGetColorAsGray(mp_annot, result.mp_colorpt));
            return result;
        }

        /// <summary> Gets the color space the annotation's color is represented in.
        /// 
        /// </summary>
        /// <returns> An integer that is either 1(for DeviceGray), 3(DeviceRGB), or 4(DeviceCMYK).
        /// If the annotation has no color, i.e. is transparent, 0 will be returned.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public int GetColorCompNum()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotGetColorCompNum(mp_annot, ref result));
            return result;
        }

        /// <summary> Sets an annotation's color.
        /// (Optional; PDF 1.1)
        /// 
        /// </summary>
        /// <param name="cpt">the color point
        /// </param>
        /// <param name="comp_num">The color component number that indirectly implies the color space the color is from.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetColor(ColorPt cpt, int comp_num)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotSetColor(mp_annot, cpt.mp_colorpt, comp_num));
        }
        /// <summary> Sets an annotation's color.
        /// (Optional; PDF 1.1)
        /// 
        /// </summary>
        /// <param name="cpt">the color point
        /// </param>			
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetColor(ColorPt cpt)
        {
            int comp_num = 3;
            SetColor(cpt, comp_num);
        }

        /// <summary> Gets the struct parent of an annotation.
        /// (Required if the annotation is a structural content item; PDF 1.3)
        /// 
        /// </summary>
        /// <returns> An integer which is the integer key of the annotation’s entry
        /// in the structural parent tree.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <summary>  The StructParent is the integer key of the annotation’s entry
        /// in the structural parent tree.</summary>
        public int GetStructParent()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotGetStructParent(mp_annot, ref result));
            return result;
        }

        /// <summary> Sets the struct parent of an annotation.
        /// (Required if the annotation is a structural content item; PDF 1.3)
        /// 
        /// </summary>
        /// <param name="keyval">An integer which is the integer key of the
        /// annotation’s entry in the structural parent tree.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The StructParent is the integer key of the annotation’s entry
        /// in the structural parent tree.</remarks>
        public void SetStructParent(int keyval)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotSetStructParent(mp_annot, keyval));
        }

        /// <summary> Gets optional content of an annotation.
        /// 
        /// </summary>
        /// <returns> An SDF object corresponding to the grup of optional properties.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>The return value is an (optional content) group or (optional content) membership 
        /// dictionary (PDF.OCG.OCMD)specifying the optional content properties for
        /// the annotation. Before the annotation is drawn, its visibility
        /// shall be determined based on this entry as well as the annotation
        /// flags specified in the Flag entry . If it is determined to be invisible,
        /// the annotation shall be skipped, as if it were not in the document.</remarks>
        public Obj GetOptionalContent()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotGetOptionalContent(mp_annot, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }

        /// <summary> Sets optional content of an annotation.
        /// (Optional, PDF1.5). 
        /// 
        /// </summary>
        /// <param name="oc">An SDF object corresponding to the optional content,
        /// a PDF.OCG.Group or membership dictionary specifying the PDF.OCG.Group properties for 
        /// the annotation. Before the annotation is drawn, its visibility 
        /// shall be determined based on this entry as well as the annotation 
        /// flags specified in the Flag entry . If it is determined to be invisible, 
        /// the annotation shall be skipped, as if it were not in the document.
        /// </param>
        public void SetOptionalContent(Obj oc)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotSetOptionalContent(mp_annot, oc.mp_obj));
        }

        /// <summary> Resize.
        /// 
        /// </summary>
        /// <param name="newrect">the pos
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Resize(Rect newrect)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotResize(mp_annot, ref newrect.mp_imp));
        }
        /// <summary> Gets the rotation value of the annotation. The Rotation specifies the number of degrees by which the
        /// annotation shall be rotated counterclockwise relative to the page.
        /// The value shall be a multiple of 90.
        /// 
        /// </summary>
        /// <returns> An integer representing the rotation value of the annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>
        /// This property is part of the appearance characteristics dictionary, this dictionary 
        /// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.</remarks>
        public int GetRotation()
        {
            int result = int.MinValue;
			PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotGetRotation(mp_annot, ref result));
            return result;
        }

        /// <summary> Sets the rotation value of the annotation. The Rotation specifies the number of degrees by which the
        /// annotation shall be rotated counterclockwise relative to the page.
        /// The value shall be a multiple of 90.
        /// (Optional)
        /// 
        /// </summary>
        /// <param name="rot">An integer representing the rotation value of the annotation.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>
        /// This property is part of the appearance characteristics dictionary, this dictionary
        /// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.
        /// </remarks>
        public void SetRotation(int rot)
        {
			PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotSetRotation(mp_annot, rot));
        }
        /// <summary> Regenerates the appearance stream for the annotation. 	
        /// This method can be used to auto-generate the annotation appearance after creating 
        /// or modifying the annotation without providing an explicit appearance or 
        /// setting the "NeedAppearances" flag in the AcroForm dictionary.
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  If this annotation contains text, and has been added to a rotated page, the text in 
        /// the annotation may be rotated. If RefreshAppearance is called *after* the annotation is added 
        /// to a rotated page, then any text will be rotated in the opposite direction of the page 
        /// rotation. If this method is called *before* the annotation is added to any rotated page, then 
        /// no counter rotation will be applied. If you wish to call RefreshAppearance on an annotation 
        /// already added to a rotated page, but you don't want the text to be rotated, you can do one 
        /// of the following; temporarily un-rotate the page, or, temporarily remove the "Rotate" object 
        /// from the annotation. To support users adding text annotations while using a PDF viewer,
        /// you can also add any viewer rotation to the annotations Rotate object, to have text always
        /// rotated correctly, from the users perspective.</remarks>
        public void RefreshAppearance()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotRefreshAppearance(mp_annot));
        }

        /// <summary> Returns custom data associated with the given key.
        /// </summary>
        /// <param name="key"> The key for which to retrieve the associated data.
        /// </param>
        /// <returns> the custom data string. If no data is available an empty string is returned.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public String GetCustomData(String key)
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotGetCustomData(mp_annot, UString.ConvertToUString(key).mp_impl, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

        /// <summary> Sets the custom data associated with the specified key.
        /// </summary>
        /// <param name="key">The key under which to store this custom data.
        /// </param>
        /// <param name="value">The custom data string to store.
        /// </param>	
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetCustomData(String key, String value)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotSetCustomData(mp_annot, UString.ConvertToUString(key).mp_impl, UString.ConvertToUString(value).mp_impl));
        }

        /// <summary> Deletes custom data associated with the given key.
        /// </summary>
        /// <param name="key">The key for which to delete the associated data.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void DeleteCustomData(String key)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotDeleteCustomData(mp_annot, UString.ConvertToUString(key).mp_impl));
        }


        ///<summary>create and initialize <c>Annot</c> object from a given one
        ///</summary>
        ///<param name="b">another <c>Annot</c> object
        ///</param>
        public Annot(Annot b)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotCopy(b.mp_annot, ref mp_annot));
        }

        // Nested Types

        /// <summary>
        /// BorderStyle structure specifies the characteristics of the annotation's border.
        /// The border is specified as a rounded rectangle.
        /// </summary>
        public class BorderStyle
        {
            internal TRN_AnnotBorderStyle mp_bs = IntPtr.Zero;

            internal BorderStyle(TRN_AnnotBorderStyle imp)
            {
                this.mp_bs = imp;
            }

            // Methods
            /// <summary> Creates a new border style with given parameters.
            /// 
            /// </summary>
            /// <param name="s">The border style.
            /// </param>
            /// <param name="b_width">The border width expressed in the default user space.
            /// </param>
            public BorderStyle(Style s, double b_width)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotBorderStyleCreate(s, b_width, 0, 0, ref mp_bs));

                this.border_style = s;
                this.hr = 0;
                this.vr = 0;
                this.width = b_width;
                //this.dash = null;

            }
            /// <summary> Creates a new border style with given parameters.
            /// 
            /// </summary>
            /// <param name="s">The border style.
            /// </param>
            /// <param name="b_width">The border width expressed in the default user space.
            /// </param>
            /// <param name="b_hr">The horizontal corner radius expressed in the default user space.
            /// </param>
            /// <param name="b_vr">The vertical corner radius expressed in the default user space.
            /// in drawing the border. The dash array is specified in the same format as in the line
            /// dash pattern parameter of the graphics state except that the phase is assumed to be 0.
            /// </param>
            /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
            /// <remarks> If the corner radii are 0, the border has square (not rounded) corners; if
            /// the border width is 0, no border is drawn.</remarks>
            public BorderStyle(Style s, double b_width, double b_hr, double b_vr)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotBorderStyleCreate(s, b_width, b_hr, b_vr, ref mp_bs));

                this.border_style = s;
                this.hr = b_hr;
                this.vr = b_vr;
                this.width = b_width;
                //this.dash = null;

            }

            /// <summary> Creates a new border style with given parameters.
            /// 
            /// </summary>
            /// <param name="s">The border style.
            /// </param>
            /// <param name="b_width">The border width expressed in the default user space.
            /// </param>
            /// <param name="b_hr">The horizontal corner radius expressed in the default user space.
            /// </param>
            /// <param name="b_vr">The vertical corner radius expressed in the default user space.
            /// </param>
            /// <param name="dash_array">An array of numbers defining a pattern of dashes and gaps to be used
            /// in drawing the border. The dash array is specified in the same format as in the line
            /// dash pattern parameter of the graphics state except that the phase is assumed to be 0.
            /// </param>
            /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
            /// <remarks>  If the corner radii are 0, the border has square (not rounded) corners; if 
            /// the border width is 0, no border is drawn.</remarks>
            public BorderStyle(Style s, double b_width, double b_hr, double b_vr, double[] dash_array)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotBorderStyleCreateWithDashPattern(s, b_width, b_hr, b_vr, dash_array, dash_array.Length, ref mp_bs));

                this.border_style = s;
                this.hr = b_hr;
                this.vr = b_vr;
                this.width = b_width;
                this.dash = dash_array;
            }

            // Properties
            /// <summary>the border style</summary>
            public Style border_style
            {
                /// <summary> Gets the style.
                /// 
                /// </summary>
                /// <returns> the border style.
                /// </returns>
                /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
                get
                {
                    Annot.BorderStyle.Style result = new Annot.BorderStyle.Style();
                    PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotBorderStyleGetStyle(mp_bs, ref result));
                    return result;
                }
                /// <summary> Sets the border style.
                /// 
                /// </summary>
                /// <param name="value">the new style
                /// </param>
                /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
                set
                {
                    PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotBorderStyleSetStyle(mp_bs, value));
                }
            }
            /// <summary> the border dash pattern </summary>
            public double[] dash
            {
                /// <summary> Gets the dash.
                /// 
                /// </summary>
                /// <returns> the border dash pattern.
                /// </returns>
                /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
                get
                {
                    int buf_length = 0;
                    IntPtr src = IntPtr.Zero;
                    PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotBorderStyleGetDashPattern(mp_bs, ref src, ref buf_length));
                    double[] dest = new double[buf_length];
                    if (buf_length > 0) 
                    {
                        System.Runtime.InteropServices.Marshal.Copy(src, dest, 0, buf_length);
                    }                    
                    return dest;
                }
                /// <summary> Sets the border dash pattern.
                /// 
                /// </summary>
                /// <param name="value">the new dash
                /// </param>
                /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
                set
                {
                    IntPtr pnt = IntPtr.Zero;
                    if (value.Length < 1) 
                    {
                        // dash array is empty
                        PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotBorderStyleSetDashPattern(mp_bs, pnt, value.Length));
                    } 
                    else
                    {
                        int psize = System.Runtime.InteropServices.Marshal.SizeOf(value[0]) * value.Length;
                        pnt = System.Runtime.InteropServices.Marshal.AllocHGlobal(psize);
                        try
                        {
                            System.Runtime.InteropServices.Marshal.Copy(value, 0, pnt, value.Length);
                            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotBorderStyleSetDashPattern(mp_bs, pnt, value.Length));
                        }
                        finally
                        {
                            // Free unmanaged memory
                            System.Runtime.InteropServices.Marshal.FreeHGlobal(pnt);
                        }                        
                    }
                }
            }
            /// <summary>horizontal corner radius</summary>
            public double hr
            {
                /// <summary> Gets the hR.
                /// 
                /// </summary>
                /// <returns> horizontal corner radius.
                /// </returns>
                /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
                get
                {
                    double result = double.MinValue;
                    PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotBorderStyleGetHR(mp_bs, ref result));
                    return result;
                }
                /// <summary> Sets horizontal corner radius.
                /// 
                /// </summary>
                /// <param name="value">the new hR
                /// </param>
                /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
                set
                {
                    PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotBorderStyleSetHR(mp_bs, value));
                }
            }
            /// <summary> vertical corner radius </summary>
            public double vr
            {
                /// <summary> Gets the vR.
                /// 
                /// </summary>
                /// <returns> vertical corner radius.
                /// </returns>
                /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
                get
                {
                    double result = double.MinValue;
                    PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotBorderStyleGetVR(mp_bs, ref result));
                    return result;
                }
                /// <summary> Sets vertical corner radius.
                /// 
                /// </summary>
                /// <param name="value">the new vR
                /// </param>
                /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
                set
                {
                    PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotBorderStyleSetVR(mp_bs, value));
                }
            }
            /// <summary> the border width </summary>
            public double width
            {
                /// <summary> Gets the width.
                /// 
                /// </summary>
                /// <returns> the border width.
                /// </returns>
                /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
                get
                {
                    double result = double.MinValue;
                    PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotBorderStyleGetWidth(mp_bs, ref result));
                    return result;
                }
                /// <summary> Sets the border width.
                /// 
                /// </summary>
                /// <param name="value">the new width
                /// </param>
                /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
                set
                {
                    PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotBorderStyleSetWidth(mp_bs, value));
                }
            }


            // Nested Types
            /// <summary> The border style </summary>
            public enum Style
            {
                /// <summary>A solid rectangle surrounding the annotation.</summary>
                e_solid,
                /// <summary>A dashed rectangle surrounding the annotation.</summary>
                e_dashed,
                /// <summary>A simulated embossed rectangle that appears to be raised above the surface of the page.</summary>
                e_beveled,
                /// <summary>A simulated engraved rectangle that appears to be recessed below the surface of the page.</summary>
                e_inset,
                /// <summary>A single line along the bottom of the annotation rectangle.</summary>
                e_underline
            }

        }
        /// <summary>annotation appearances types</summary>
        public enum AnnotationState
        {
            ///<summary>The normal appearance is used when the annotation is not interacting 
            /// with the user. This appearance is also used for printing the annotation.</summary>
            e_normal,
            ///<summary>The rollover appearance is used when the user moves the cursor into the 
            /// annotation’s active area without pressing the mouse button.</summary>
            e_rollover,
            ///<summary>The down appearance is used when the mouse button is pressed or held down
            /// within the annotation’s active area.</summary>
            e_down
        }
        ///<summary>Flags specifying various characteristics of the annotation.</summary>
        public enum Flag
        {
            ///<summary>If e_invisible is set, do not display the annotation if it does not belong to 
            /// one of the standard annotation types and no annotation handler is available. If clear, 
            /// display such an unknown annotation using an appearance stream specified by its appearance 
            /// dictionary, if any. </summary>
            e_invisible,         // PDF 1.2
            ///<summary>If e_hidden is set, do not display or print the annotation or allow it to interact
            /// with the user, regardless of its annotation type or whether an annotation handler 
            /// is available.</summary>
            e_hidden,            // PDF 1.2
            ///<summary>If e_print is set, print the annotation when the page is printed. If clear, never
            /// print the annotation, regardless of whether it is displayed on the screen. This can be 
            /// useful, for example, for annotations representing interactive pushbuttons, which would 
            /// serve no meaningful purpose on the printed page.</summary>
            e_print,             // PDF 1.2
            ///<summary>If e_no_zoom is set, do not scale the annotation’s appearance to match the 
            /// magnification of the page.</summary>
            e_no_zoom,           // PDF 1.3
            ///<summary>If e_no_rotate is set, do not rotate the annotation’s appearance to match the rotation 
            /// of the page.</summary>
            e_no_rotate,         // PDF 1.3
            ///<summary>If e_no_view is set, do not display the annotation on the screen or allow it to
            /// interact with the user. The annotation may be printed (depending on the setting of the 
            /// Print flag) but should be considered hidden for purposes of on-screen display and user 
            /// interaction.</summary>
            e_no_view,           // PDF 1.3
            ///<summary>If e_read_only is set, do not allow the annotation to interact with the user. The
            /// annotation may be displayed or printed (depending on the settings of the NoView and Print 
            /// flags) but should not respond to mouse clicks or change its appearance in response to 
            /// mouse motions.</summary>
            e_read_only,         // PDF 1.3
            ///<summary>If e_locked is set, do not allow the annotation to be deleted or its properties 
            /// (including position and size) to be modified by the user. However, this flag does not 
            /// restrict changes to the annotation’s contents, such as the value of a form field.</summary>
            e_locked,            // PDF 1.4
            ///<summary>If e_toggle_no_view is set, invert the interpretation of the NoView flag for certain 
            /// events. A typical use is to have an annotation that appears only when a mouse cursor is
            /// held over it.</summary>
            e_toggle_no_view,     // PDF 1.5
            ///<summary>If e_locked_contents is set, do not allow the contents of the annotation to be modified 
            /// by the user. This flag does not restrict deletion of the annotation or changes to other 
            /// annotation properties, such as position and size.	</summary>
            e_locked_contents    // PDF 1.7
        }
        ///<summary>Annotation types</summary>
        public enum Type
        {
            ///<summary>Text annotation</summary>
            e_Text,
            ///<summary>Link annotation</summary>
            e_Link,
            ///<summary>Free text annotation</summary>
            e_FreeText,
            ///<summary>Line annotation</summary>
            e_Line,
            ///<summary>Square annotation</summary>
            e_Square,
            ///<summary>Circle annotation</summary>
            e_Circle,
            ///<summary>Polygon annotation</summary>
            e_Polygon,
            ///<summary>Polyline annotation</summary>
            e_Polyline,
            ///<summary>Highlight annotation</summary>
            e_Highlight,
            ///<summary>Underline annotation</summary>
            e_Underline,
            ///<summary>Squiggly-underline annotation</summary>
            e_Squiggly,
            ///<summary>Strikeout annotation</summary>
            e_StrikeOut,
            ///<summary>Rubber stamp annotation</summary>
            e_Stamp,
            ///<summary>Caret annotation</summary>
            e_Caret,
            ///<summary>Ink annotation</summary>
            e_Ink,
            ///<summary>Pop-up annotation</summary>
            e_Popup,
            ///<summary>File attachment annotation</summary>
            e_FileAttachment,
            ///<summary>Sound annotation</summary>
            e_Sound,
            ///<summary>Movie annotation</summary>
            e_Movie,
            ///<summary>Widget annotation</summary>
            e_Widget,
            ///<summary>Screen annotation</summary>
            e_Screen,
            ///<summary>Printer’s mark annotation</summary>
            e_PrinterMark,
            ///<summary>Trap network annotation</summary>
            e_TrapNet,
            ///<summary>Watermark annotation</summary>
            e_Watermark,
            ///<summary>3D annotation</summary>
            e_3D,
            ///<summary>Redact annotation</summary>
            e_Redact,
            ///<summary>Projection annotation, Adobe supplement to ISO 32000 </summary>
            e_Projection,
            ///<summary>Rich Media annotation, Adobe supplement to ISO 32000 </summary>
            e_RichMedia,
            ///<summary>Unknown annotation type</summary>
            e_Unknown
        }
    }
}
