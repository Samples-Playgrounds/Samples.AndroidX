using System;
using System.Collections.Generic;
using System.Text;

using pdftron.Common;
using pdftron.SDF;
using pdftronprivate.trn;

using TRN_Page = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_Obj = System.IntPtr;
using TRN_Annot = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> Page is a high-level class representing PDF page object (see 'Page Objects' in 
    /// Section 3.6.2, 'Page Tree,' in PDF Reference Manual).
    /// 
    /// Among other associated objects, a page object contains: 
    /// <list type="bullet">
    /// <item><description>A series of objects representing the objects drawn on the page (See Element and 
    /// ElementReader class for examples of how to extract page content).</description>
    /// </item>
    /// <item><description>A list of resources used in drawing the page 
    /// </description>
    /// </item>
    /// <item><description>Annotations 
    /// </description>
    /// </item>
    /// <item><description>Beads, private metadata, optional thumbnail image, etc.
    /// </description>
    /// </item>
    /// </list>
    /// </summary>
    public class Page
    {
        internal TRN_Page mp_page = IntPtr.Zero;
        internal Object m_ref;

        // Methods
        internal Page()
        {
            this.mp_page = IntPtr.Zero;
        }
        internal Page(TRN_Page page, Object reference)
        {
            this.mp_page = page;
            this.m_ref = reference;
        }

        /// <summary>
        /// Create a new page object using native impl pointer
        /// For internal use only.
        /// </summary>
        /// <param name="imp">impl pointer from PDFNet core.</param>
        public static Page CreateInternal(TRN_Annot imp, Object reference)
        {
            return new Page(imp, reference);
        }

        /// <summary> Instantiates a new page.
	    /// 
	    /// </summary>
	    /// <param name="page_dict">the page_dict
	    /// </param>
	    public Page(Obj page_dict) 
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageCreate(page_dict.mp_obj, ref mp_page));
            this.m_ref = page_dict.GetRefHandleInternal();
        }

	    /// <summary>Sets the page to given <c>Page</c> object
	    /// </summary>
	    /// <param name="p">a given <c>Page</c> object
	    /// </param>
	    public void Set(Page p) 
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageCopy(p.mp_page, ref mp_page));
        }
	    /// <summary>Assignment operator</summary>
	    /// <param name="r">a given <c>Page</c> object
	    /// </param>
	    /// <returns>a <c>Page</c> equals to the given <c>Page</c> object
	    /// </returns>
	    public Page op_Assign(Page r) 
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageCopy(r.mp_page, ref this.mp_page));
            return this;
        }
	    /// <summary> Checks if is valid.		
	    /// </summary>
	    /// <returns> true if this is a valid (non-null) page, false otherwise.
	    /// If the function returns false the underlying SDF/Cos object is null
	    /// or is not valid.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    public bool IsValid() 
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageIsValid(mp_page, ref result));
            return result;
        }

	    /// <summary> Gets the index.
	    /// 
	    /// </summary>
	    /// <returns> the Page number indication the position of this Page in document's page sequence.
	    /// Document's page sequence is indexed from 1. Page number 0 means that the page is not part
	    /// of document's page sequence or that the page is not valid.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    public int GetIndex() 
        {
            int result = Int32.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageGetIndex(mp_page, ref result));
            return result;
        }

        ///<summary>Gets the specified box</summary>
	    ///<param name="type">The type of the page bounding box. Possible values are: e_media, e_crop,
	    /// e_bleed, e_trim, and e_art.</param>
	    /// <returns> the box specified for the page object intersected with the media box.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    public Rect GetBox(Box type) 
        {
            BasicTypes.TRN_Rect result = new BasicTypes.TRN_Rect();
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageGetBox(mp_page, type, ref result));
            return new Rect(result);
        }
	    ///<summary>Sets the specified box</summary>
	    ///<param name="type">The type of the page bounding box. Possible values are: e_media, e_crop,
	    /// e_bleed, e_trim, and e_art.</param>
	    ///<param name="box">A rectangle specifying the coordinates to set for the box. The coordinates are
	    /// specified in user space units.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    public void SetBox(Box type, Rect box) 
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageSetBox(mp_page, type, ref box.mp_imp));
        }
	    /// <summary> Gets the crop box.
	    /// 
	    /// </summary>
	    /// <returns> the crop box for this page. The page dimensions are specified in user space
	    /// coordinates.
	    /// 
	    /// The crop box is the region of the page to display and print.
	    /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  this method is equivalent to GetBox(Page::e_crop) </remarks>
        public Rect GetCropBox()
        {
            BasicTypes.TRN_Rect result = new BasicTypes.TRN_Rect();
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageGetCropBox(mp_page, ref result));
            return new Rect(result);
        }
	    /// <summary> Sets the crop box for this page. The crop box is the region of the page to
	    /// display and print.
	    /// 
	    /// </summary>
	    /// <param name="box">the new crop box for this page. The page dimensions are specified in user space
	    /// coordinates.
	    /// 
	    /// The crop box defines the region to which the contents of the page are to be clipped (cropped)
	    /// when displayed or printed.
	    /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  this method is equivalent to SetBox(Page::e_crop) </remarks>
        public void SetCropBox(Rect box)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageSetCropBox(mp_page, ref box.mp_imp));
        }
	    /// <summary> Gets the media box.
	    /// 
	    /// </summary>
	    /// <returns> the media box for this page. The page dimensions are specified in user space
	    /// coordinates.
	    /// 
	    /// The media box defines the boundaries of the physical medium on which the page is to
	    /// be printed. It may include any extended area surrounding the finished page for bleed,
	    /// printing marks, or other such purposes.
	    /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  this method is equivalent to GetBox(Page::e_media) </remarks>
        public Rect GetMediaBox()
        {
            BasicTypes.TRN_Rect result = new BasicTypes.TRN_Rect();
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageGetMediaBox(mp_page, ref result));
            return new Rect(result);
        }
	    /// <summary> Sets the media box for this page.
	    /// 
	    /// </summary>
	    /// <param name="box">the new media box for this page. The page dimensions are specified in user space
	    /// coordinates.
	    /// 
	    /// The media box defines the boundaries of the physical medium on which the page is to
	    /// be printed. It may include any extended area surrounding the finished page for bleed,
	    /// printing marks, or other such purposes.
	    /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  this method is equivalent to SetBox(Page::e_media) </remarks>
        public void SetMediaBox(Rect box)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageSetMediaBox(mp_page, ref box.mp_imp));
        }
	    /// <summary>The bounding box is defined as the smallest rectangle that includes all the visible 
	    /// content on the page.</summary>
        /// <returns>the bounding box for this page. The page dimensions are specified in user space 
        /// coordinates.</returns>
        public Rect GetVisibleContentBox()
        {
            BasicTypes.TRN_Rect result = new BasicTypes.TRN_Rect();
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageGetVisibleContentBox(mp_page, ref result));
            return new Rect(result);
        }

        /// <summary> Rotate r0 clockwise by r1
        /// 
        /// </summary>
        /// <param name="r0">r0 first rotation.
        /// </param>
        /// <param name="r1">r1 second rotation.
        /// </param>
        /// <returns> returns r0 + r1
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Rotate AddRotations(Rotate r0, Rotate r1)
        {
            Rotate result = Rotate.e_0;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageAddRotations(r0, r1, ref result));
            return result;
        }

        /// <summary> Rotate r0 counter clockwise by r1.
        /// 
        /// </summary>
        /// <param name="r0">r0 first rotation.
        /// </param>
        /// <param name="r1">r1 second rotation.
        /// </param>
        /// <returns> returns r0 - r1
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Rotate SubtractRotations(Rotate r0, Rotate r1)
        {
            Rotate result = Rotate.e_0;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageSubtractRotations(r0, r1, ref result));
            return result;
        }

        /// <summary>  Convert a rotation to a number.
        /// 
        /// </summary>
        /// <param name="r"> rotation to convert to number
        /// </param>
        /// <returns> one of four numbers; 0, 90, 180 or 270.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static int RotationToDegree(Rotate r)
        {
            int result = Int32.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageRotationToDegree(r, ref result));
            return result;
        }

        /// <summary> Convert a number that represents rotation in degrees to a rotation enum.
        /// 
        /// </summary>
        /// <param name="r"> degree to convert to rotation. Valid numbers are multiples of 90.
        /// </param>
        /// <returns> one of four angles; e_0, e_90, e_180 or e_270. Returns e_0 if input is
        /// not a multiple 90.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Rotate DegreeToRotation(int r)
        {
            Rotate result = Rotate.e_0;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageDegreeToRotation(r, ref result));
            return result;
        }

        /// <summary> Gets the rotation.
	    /// 
	    /// </summary>
	    /// <returns> the rotation value for this page.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Rotate GetRotation()
        {
            Rotate result = Rotate.e_0;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageGetRotation(mp_page, ref result));
            return result;
        }
	    /// <summary> Sets the rotation value for this page.
	    /// 
	    /// </summary>
	    /// <param name="angle">the new rotation
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetRotation(Rotate angle)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageSetRotation(mp_page, angle));
        }

	    /// <summary> Gets the default matrix.
	    /// 
	    /// </summary>
	    /// <returns> the matrix that transforms user space coordinates to rotated and cropped coordinates.
	    /// The origin of this space is the bottom-left of the rotated, cropped page.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Matrix2D GetDefaultMatrix()
        {
            bool flip_y = false;
            Box box_type = Box.e_crop;
            Rotate angle = Rotate.e_0;
            return GetDefaultMatrix(flip_y, box_type, angle);
        }
	    /// <summary> Gets the default matrix.
	    /// 
	    /// </summary>
	    /// <param name="flip">this parameter can be used to mirror the page. if 'flip_y' is true the Y
	    /// axis is not flipped and it is increasing, otherwise Y axis is decreasing.
	    /// </param>			
	    /// <returns> the matrix that transforms user space coordinates to rotated and cropped coordinates.
	    /// The origin of this space is the bottom-left of the rotated, cropped page.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Matrix2D GetDefaultMatrix(bool flip)
        {
            Box box_type = Box.e_crop;
            Rotate angle = Rotate.e_0;
            return GetDefaultMatrix(flip, box_type, angle);
        }
	    /// <summary> Gets the default matrix.
	    /// 
	    /// </summary>
	    /// <param name="flip">this parameter can be used to mirror the page. if 'flip_y' is true the Y
	    /// axis is not flipped and it is increasing, otherwise Y axis is decreasing.
	    /// </param>
	    /// <param name="region">an optional parameter used to specify the page box/region that the matrix
	    /// should map to. By default, the function transforms user space coordinates to cropped
	    /// coordinates.
	    /// </param>
	    /// <returns> the matrix that transforms user space coordinates to rotated and cropped coordinates.
	    /// The origin of this space is the bottom-left of the rotated, cropped page.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Matrix2D GetDefaultMatrix(bool flip, Box region)
        {
            Rotate angle = Rotate.e_0;
            return GetDefaultMatrix(flip, region, angle);
        }
	    /// <summary> Gets the default matrix.
	    /// 
	    /// </summary>
	    /// <param name="flip">this parameter can be used to mirror the page. if 'flip_y' is true the Y
	    /// axis is not flipped and it is increasing, otherwise Y axis is decreasing.
	    /// </param>
	    /// <param name="region">an optional parameter used to specify the page box/region that the matrix
	    /// should map to. By default, the function transforms user space coordinates to cropped
	    /// coordinates.
	    /// </param>
	    /// <param name="rot">the rot_type
	    /// </param>
	    /// <returns> the matrix that transforms user space coordinates to rotated and cropped coordinates.
	    /// The origin of this space is the bottom-left of the rotated, cropped page.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Matrix2D GetDefaultMatrix(bool flip, Box region, Rotate rot)
        {
            BasicTypes.TRN_Matrix2D result = new BasicTypes.TRN_Matrix2D();
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageGetDefaultMatrix(mp_page, flip, region, rot, ref result));
            return new Matrix2D(result);
        }

	    /// <summary> Gets the page width.
	    /// 
	    /// </summary>
	    /// <returns> the width for the given page region/box taking into account page
	    /// rotation attribute (i.e. /Rotate entry in page dictionary).
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public double GetPageWidth()
        {
            double result = Double.NaN;
            Box box_type = Box.e_crop;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageGetPageWidth(mp_page, box_type, ref result));
            return result;
        }
	    /// <summary> Gets the page height.
	    /// 
	    /// </summary>
	    /// <returns> the height for the given page region/box taking into account page
	    /// rotation attribute (i.e. /Rotate entry in page dictionary).
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public double GetPageHeight()
        {
            double result = Double.NaN;
            Box box_type = Box.e_crop;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageGetPageHeight(mp_page, box_type, ref result));
            return result;
        }
	    /// <summary> Gets the page width.
	    /// 
	    /// </summary>
	    /// <param name="region">the box_type
	    /// </param>
	    /// <returns> the page width
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public double GetPageWidth(Box region)
        {
            double result = Double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageGetPageWidth(mp_page, region, ref result));
            return result;
        }
	    /// <summary> Gets the page height.
	    /// 
	    /// </summary>
	    /// <param name="region">the box_type
	    /// </param>
	    /// <returns> the page height
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public double GetPageHeight(Box region)
        {
            double result = Double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageGetPageHeight(mp_page, region, ref result));
            return result;
        }
	    /// <summary> Tests whether this page has a transition.
	    /// 
	    /// </summary>
	    /// <returns> true, if successful
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool HasTransition()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageHasTransition(mp_page, ref result));
            return result;
        }

	    /// <summary> Gets the resource dictionary.
	    /// 
	    /// </summary>
	    /// <returns> a pointer to the page resource dictionary.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetResourceDict()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageGetResourceDict(mp_page, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
	    /// <summary> Gets the contents.
	    /// 
	    /// </summary>
	    /// <returns> NULL if page is empty, otherwise a single stream or an array of streams.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetContents()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageGetContents(mp_page, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }

        public Obj GetThumb()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageGetThumb(mp_page, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }

	    /// <summary>Gets the annotation on the page.			
	    /// </summary>
	    /// <returns>Annotation object. If the index is out of range returned Annot object
	    /// is not valid &#40;i.e. annot.IsValid&#40;&#41; returns false&#41;.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetAnnots()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageGetAnnots(mp_page, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
	    /// <summary>Gets the number of annotations on a page. Widget annotations &#40;form fields&#41; are
	    /// included in the count.			
	    /// </summary>
	    /// <returns>number of annotations on a page.
        /// </returns>
        /// <exception cref="PDFNetException">PDFNetException the PDFNet exception </exception>
        public int GetNumAnnots()
        {
            uint result = UInt32.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageGetNumAnnots(mp_page, ref result));
            return System.Convert.ToInt32(result);
        }
	    /// <summary>Gets the annotation on the page.
	    /// </summary>
	    /// <param name="index">index of the annotation to get on a page
	    /// </param>
	    /// <returns> Annotation object. If the index is out of range returned Annot object
	    /// is not valid &#40;i.e. annot.IsValid&#40;&#41; returns false&#41;.
	    /// </returns>
        /// <exception cref="PDFNetException">PDFNetException the PDFNet exception </exception>
        /// <remarks>The first annotation on a page has an index of zero</remarks>
        public Annot GetAnnot(int index)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageGetAnnot(mp_page,System.Convert.ToUInt32(index), ref result));
            return result != IntPtr.Zero ? new Annot(result, this.m_ref) : null;
        }
	    /// <summary> Adds an annotation at the specified location in a page's annotation array.
	    /// 
	    /// </summary>
	    /// <param name="pos">location in the array to insert the object. The object is inserted
	    /// before the specified location. The first element in an array has a pos of zero.
	    /// If pos >= GetNumAnnots(), the method appends the annotation to the array.
	    /// </param>
	    /// <param name="annot">annotation to add.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void AnnotInsert(int pos, Annot annot)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageAnnotInsert(mp_page,System.Convert.ToUInt32(pos), annot.mp_annot));
        }
	    /// <summary> Adds an annotation to the end of a page's annotation array.
	    /// 
	    /// </summary>
	    /// <param name="annot">- The annotation to prepend in a page's annotation array.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void AnnotPushBack(Annot annot)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageAnnotPushBack(mp_page, annot.mp_annot));
        }
	    /// <summary> Adds an annotation to the beginning of a page's annotation array.
	    /// 
	    /// </summary>
	    /// <param name="annot">- The annotation to append in a page's annotation array.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void AnnotPushFront(Annot annot)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageAnnotPushFront(mp_page, annot.mp_annot));
        }
	    /// <summary> Removes the given annoation from the page.
	    /// 
	    /// </summary>
	    /// <param name="annot">the annot
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  Removing the annotation invalidates the given Annot object. </remarks>
	    public void AnnotRemove(Annot annot)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageAnnotRemove(mp_page, annot.mp_annot));
        }
	    
        /// <summary> Removes the annoation at a given location.
	    /// 
	    /// </summary>
	    /// <param name="pos">- A zero based index of the annotation to remove.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  Removing the annotation invalidates any associated Annot object. </remarks>
	    public void AnnotRemove(int pos)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageAnnotRemoveByIndex(mp_page,System.Convert.ToUInt32(pos)));
        }

	    /// <summary> Gets the UserUnit value for the page. A UserUnit is a positive number giving
	    /// the size of default user space units, in multiples of 1/72 inch.
	    /// 
	    /// </summary>
	    /// <returns> the UserUnit value for the page. If the key is not present in the
	    /// page dictionary the default of 1.0 is returned.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    public double GetUserUnitSize()
        {
            double result = Double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageGetUserUnitSize(mp_page, ref result));
            return result;
        }
	    /// <summary> Sets the UserUnit value for a page.
	    /// 
	    /// </summary>
	    /// <param name="unit_size">A positive number giving the size of default user space
	    /// units, in multiples of 1/72 inch.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  This is a PDF 1.6 feature. See the implementation note 171 in PDF Reference for details.</remarks>	
	    public void SetUserUnitSize(double unit_size)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageSetUserUnitSize(mp_page, unit_size));
        }

	    /// <summary> A utility method used to scale physical dimensions of the page including
	    /// all page content.
	    /// 
	    /// </summary>
	    /// <param name="sc">A number greater than 0 which is used as a scale factor.
	    /// For example, calling page.Scale(0.5) will reduce physical dimensions of the
	    /// page to half its original size, whereas page.Scale(2) will double the physical
	    /// dimensions of the page and will rescale all page content appropriately.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  Unlike SetUserUnitSize(unit_size) which is only supported in PDF 1.6 
	    /// (i.e. Acrobat 7) and above, page.Scale(sc) supports all PDF versions.
	    /// </remarks>
	    public void Scale(double sc) 
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageScale(mp_page, sc));
        }

	    /// <summary> Flatten/Merge existing form field appearances with the page content and
	    /// remove widget annotation.
	    /// 
	    /// Form 'flattening' refers to the operation that changes active form fields
	    /// into a static area that is part of the PDF document, just like the other
	    /// text and images in the document. A completely flattened PDF form does not
	    /// have any widget annotations or interactive fields.
	    /// 
	    /// </summary>
	    /// <param name="field">the field
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>An alternative approach to set the field as read only is using <c>Field.SetFlag(Field::e_read_only, true)</c>
	    /// method. Unlike Field.SetFlag(...), the result of FlattenField() operation can not be programatically reversed.
	    /// </remarks>
	    public void FlattenField(Field field)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageFlattenField(mp_page, ref field.mp_field));
        }

	    /// <summary> Gets the page dictionary.
	    /// 
	    /// </summary>
	    /// <returns> the object to the underlying SDF/Cos object.
	    /// </returns>
	    public Obj GetSDFObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageGetSDFObj(mp_page, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
	    /// <summary> Some of the page attributes are designated as inheritable.
	    /// If such an attribute is omitted from a page object, its value is inherited
	    /// from an ancestor node in the page tree. If the attribute is a required one,
	    /// a value must be supplied in an ancestor node; if it is optional and no
	    /// inherited value is specified, the default value should be used.
	    /// 
	    /// The function walks up the page inhritance tree in search for specified
	    /// attribute.
	    /// 
	    /// </summary>
	    /// <param name="attrib">the attrib
	    /// </param>
	    /// <returns> if the attribute was found return a pointer to the value. otherwise
	    /// the function return NULL.
	    /// 
	    /// Resources dictionary (Required; inheritable)
	    /// MediaBox rectangle (Required; inheritable)
	    /// CropBox rectangle (Optional; inheritable)
	    /// Rotate integer (Optional; inheritable)
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    public Obj FindInheritedAttribute(String attrib)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageFindInheritedAttribute(mp_page, attrib, ref result));
            return result != IntPtr.Zero ? new Obj(result, attrib) : null;
        }

        // Nested Types
        /// <summary> PDF page can define as many as five separate boundaries to control various aspects of the 
	    /// imaging process (for more details please refer to Section 10.10.1 'Page Boundaries' in PDF 
	    /// Reference Manual).		
	    /// </summary>
	    public enum Box
        {
            ///<summary>The media box defines the boundaries of the physical medium on which the page is to be 
		    /// printed. It may include any extended area surrounding the finished page for bleed, printing 
		    /// marks, or other such purposes. It may also include areas close to the edges of the medium 
		    /// that cannot be marked because of physical limitations of the output device. Content falling 
		    /// outside this boundary can safely be discarded without affecting the meaning of the PDF file.
		    ///</summary>
		    e_media = 0,
		    ///<summary>The crop box defines the region to which the contents of the page are to be clipped (cropped) 
		    /// when displayed or printed. Unlike the other boxes, the crop box has no defined meaning in 
		    /// terms of physical page geometry or intended use; it merely imposes clipping on the page 
		    /// contents. The default value is the page’s media box.
            /// </summary>
            e_crop = 1,
		    ///<summary>The bleed box defines the region to which the contents of the page should be clipped when 
		    /// output in a production environment. This may include any extra bleed area needed to 
		    /// accommodate the physical limitations of cutting, folding, and trimming equipment. 
		    /// The default value is the page’s crop box.
            /// </summary>
            e_bleed = 2,
		    ///<summary>The trim box defines the intended dimensions of the finished page after trimming. It may 
		    /// be smaller than the media box to allow for production related content, such as printing 
		    /// instructions, cut marks, or color bars. The default value is the page’s crop box.
            /// </summary>
            e_trim = 3,
		    ///<summary>The art box defines the extent of the page’s meaningful content (including potential 
		    /// white space) as intended by the page’s creator. The default value is the page’s crop box.
            /// </summary>
            e_art = 4,
            /// <summary>
            /// he user crop box defines a custom region to which the contents of the 
            /// page are to be clipped (cropped) when displayed by PDFNet
            /// It must be fully contained within the e_crop box. 
            /// The default value is the page's crop box.
            /// </summary>
            e_user_crop = 5
        };

        ///<summary>specify page rotations in degrees</summary>
	    public enum Rotate
        {
            ///<summary>0 degrees clockwise rotation</summary>
		    e_0,
		    ///<summary>90 degrees clockwise rotation</summary>
		    e_90,
		    ///<summary>180 degrees clockwise rotation</summary>
		    e_180,
		    ///<summary>270 degrees clockwise rotation</summary>
		    e_270
        };

    }
}
