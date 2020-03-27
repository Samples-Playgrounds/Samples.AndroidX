using System;
using System.Collections.Generic;
using System.Text;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.SDF;

using TRN_Bookmark = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_Obj = System.IntPtr;
using TRN_Action = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> A PDF document may optionally display a document outline on the screen, allowing
    /// the user to navigate interactively from one part of the document to another.
    /// The outline consists of a tree-structured hierarchy of Bookmarks (sometimes
    /// called outline items), which serve as a 'visual table of contents' to display the 
    /// document’s structure to the user. 
    /// 
    /// Each Bookmark has a title that appears on screen, and an Action that specifies 
    /// what happens when a user clicks on the Bookmark. The typical action for a 
    /// user-created Bookmark is to move to another location in the current document, 
    /// although any action (see PDF.Action) can be specified.
    /// 
    /// Bookmark is a utility class used to simplify work with PDF bookmarks (or 
    /// outlines; see section 8.2.2 'Document Outline' in PDF Reference Manual for 
    /// more details).
    /// </summary>
    public class Bookmark : IDisposable
    {
        internal TRN_Bookmark mp_obj = IntPtr.Zero;
        internal Object m_ref;

        /// <summary> Releases all resources used by the Bookmark </summary>
        ~Bookmark()
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
            if (mp_obj != IntPtr.Zero)
            {
                mp_obj = IntPtr.Zero;
            }
        }

        public IntPtr GetHandleInternal()
        {
            return mp_obj;
        }

        public Object GetRefHandleInternal()
        {
            return this.m_ref;
        }
        /// <summary>
        /// Create a new object using native impl pointer
        /// For internal use only.
        /// </summary>
        /// <param name="imp">impl pointer from PDFNet core.</param>
        public static Bookmark CreateInternal(TRN_Bookmark imp, Object reference)
        {
            return new Bookmark(imp, reference);
        }

        // Methods
        internal Bookmark(TRN_Bookmark imp, Object reference) 
        {
            this.mp_obj = imp;
            this.m_ref = reference;
        }
        /// <summary> A constructor. Creates a bookmark from specified <c>SDF.Obj</c></summary>
	    /// <param name="b"><c>SDF.Obj</c> object
	    /// </param>
        public Bookmark(Obj b)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkCreateFromObj(b.mp_obj, ref mp_obj));
            this.m_ref = b.GetRefHandleInternal();
        }

        /// <summary> Creates a new valid Bookmark with given title in the
        /// specified document.
        /// 
        /// </summary>
        /// <param name="doc">The document in which a Bookmark is to be created.
        /// </param>
        /// <param name="text">The title string value of the new Bookmark.
        /// </param>
        /// <returns> The new Bookmark.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The new Bookmark is not linked to the outline tree. 
        /// Use AddChild()/AddNext()/AddPrev() methods in order to link the Bookmark
        /// to the outline tree</remarks>
        public static Bookmark Create(PDFDoc doc, string text)
        {
            TRN_Bookmark result = IntPtr.Zero;
            UString str = new UString(text);
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkCreate(doc.mp_doc, str.mp_impl, ref result));
            return result != IntPtr.Zero ? new Bookmark(result, doc) : null;
        }

        /// <summary>Sets value to the specified <c>Bookmark</c>
	    /// </summary>
        /// <param name="p">given <c>Bookmark</c> object
        /// </param>
        public void Set(Bookmark p)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkCopy(p.mp_obj, ref mp_obj));

        }
	    //static Bookmark Assign(Bookmark l, Bookmark r);
	    /// <summary> Assignment Operator </summary>
	    /// <param name="r">a <c>Bookmark</c> object
	    /// </param>
	    /// <returns>A <c>Bookmark</c> object equals to the specified <c>Bookmark</c> object
	    /// </returns>
        public Bookmark op_Assign(Bookmark r)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkCopy(r.mp_obj, ref mp_obj));
            return this;
        }
	    /// <summary> Euqality operator checks whether two Bookmark objects are equal </summary>
	    /// <param name="l"><c>Bookmark</c> object at the left of the operator
	    /// </param>
	    /// <param name="r"><c>Bookmark</c> object at the right of the operator
	    /// </param>
        /// <returns>true, if both <c>Bookmark</c> objects are equal, false otherwise.
        /// </returns>
        public static bool operator ==(Bookmark l, Bookmark r)
        {
            if (System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null))
		        return true;
	        if (System.Object.ReferenceEquals(l, null) && !System.Object.ReferenceEquals(r, null))
		        return false;
	        if (!System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null))
		        return false;

            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkCompare(l.mp_obj, r.mp_obj, ref result));
            return result;
        }
	    /// <summary> Ineuqality operator checks whether two Bookmark objects are different </summary>
	    /// <param name="l"><c>Bookmark</c> object at the left of the operator
	    /// </param>
	    /// <param name="r"><c>Bookmark</c> object at the right of the operator
	    /// </param>
        /// <returns>true, if both <c>Bookmark</c> objects are not equal, false otherwise
        /// </returns>
        public static bool operator !=(Bookmark l, Bookmark r) 
        {
            return !(l == r);
        }
	    /// <summary> Check whether the given object is the equal to this Bookmark object </summary>
	    /// <param name="o">a given <c>Object</c>
	    /// </param>
        /// <returns>true, if equals to the given object
        /// </returns>
        public override bool Equals(Object o)
        {
            if (o == null) return false;
            Bookmark i = o as Bookmark;
            if (i == null) return false;
            bool b = mp_obj == i.mp_obj;
            return b;
        }

	    /// <summary> Indicates whether the Bookmark is valid (non-null).
	    /// 
	    /// </summary>
	    /// <returns> True if this is a valid (non-null) Bookmark; otherwise false.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  If this method returns false the underlying SDF/Cos object is null and
        /// the Bookmark object should be treated as null as well.</remarks>
        public bool IsValid()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkIsValid(mp_obj, ref result));
            return result; 
        }
	    /// <summary> Indicates whether the Bookmark has children.
	    /// 
	    /// </summary>
	    /// <returns> True if the Bookmark has children; otherwise false.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool HasChildren()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkHasChildren(mp_obj, ref result));
            return result; 
        }

	    /// <summary> Returns the Bookmark's next (right) sibling.
	    /// 
	    /// </summary>
	    /// <returns> The Bookmark’s next (right) sibling.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Bookmark GetNext()
        {
            TRN_Bookmark result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkGetNext(mp_obj, ref result));
            return new Bookmark(result, this.m_ref);
        }
	    /// <summary> Returns the Bookmark's previous (left) sibling.
	    /// 
	    /// </summary>
	    /// <returns> The Bookmark’s previous (left) sibling.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Bookmark GetPrev()
        {
            TRN_Bookmark result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkGetPrev(mp_obj, ref result));
            return new Bookmark(result, this.m_ref);
        }
	    /// <summary> Returns the Bookmark's first child.
	    /// 
	    /// </summary>
	    /// <returns> The Bookmark’s first child.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Bookmark GetFirstChild()
        {
            TRN_Bookmark result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkGetFirstChild(mp_obj, ref result));
            return result != IntPtr.Zero ? new Bookmark(result, this.m_ref) : null; 
        }
	    /// <summary> Returns the Bookmark's last child.
	    /// 
	    /// </summary>
	    /// <returns> The Bookmark’s last child.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Bookmark GetLastChild()
        {
            TRN_Bookmark result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkGetLastChild(mp_obj, ref result));
            return result != IntPtr.Zero ? new Bookmark(result, this.m_ref) : null; 
        }
	    /// <summary> Returns the Bookmark's parent Bookmark.
	    /// 
	    /// </summary>
	    /// <returns> The Bookmark’s parent Bookmark.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Bookmark GetParent()
        {
            TRN_Bookmark result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkGetParent(mp_obj, ref result));
            return result != IntPtr.Zero ? new Bookmark(result, this.m_ref) : null; 
        }

	    /// <summary> Gets the Bookmark specified by the given title string.
	    /// 
	    /// </summary>
	    /// <param name="title">The title string value of the Bookmark to find.
	    /// </param>
	    /// <returns> A Bookmark matching the title string value specified.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Bookmark Find(String title)
        {
            TRN_Bookmark result = IntPtr.Zero;
            UString str = new UString(title);
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkFind(mp_obj, str.mp_impl, ref result));
            return result != IntPtr.Zero ? new Bookmark(result, this.m_ref) : null; 
        }
	    /// <summary> Adds a new Bookmark as the new last child of this Bookmark.
	    /// 
	    /// </summary>
	    /// <param name="text">The title string value of the new Bookmark.
	    /// </param>
	    /// <returns> The newly created child Bookmark.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  If this Bookmark previously had no children, it will be open
        /// after the child is added.</remarks>
        public Bookmark AddChild(String text)
        {
            TRN_Bookmark result = IntPtr.Zero;
            UString str = new UString(text);
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkAddNewChild(mp_obj, str.mp_impl, ref result));
            return result != IntPtr.Zero ? new Bookmark(result, this.m_ref) : null; 
        }
	    /// <summary> Adds the specified Bookmark as the new last child of this Bookmark.
	    /// 
	    /// </summary>
	    /// <param name="bookmark">The Bookmark object to be added as a last child of this Bookmark.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Parameter in_bookmark must not be linked to a bookmark tree.
        /// If this Bookmark previously had no children, it will be open after the child is added.</remarks>
        public void AddChild(Bookmark bookmark)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkAddChild(mp_obj, bookmark.mp_obj));
        }
	    /// <summary> Adds a new Bookmark to the tree containing this Bookmark, as the
	    /// new right sibling.
	    /// 
	    /// </summary>
	    /// <param name="text">The title string value of the new Bookmark.
	    /// </param>
	    /// <returns> The newly created sibling Bookmark.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Bookmark AddNext(String text)
        {
            TRN_Bookmark result = IntPtr.Zero;
            UString str = new UString(text);
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkAddNewNext(mp_obj, str.mp_impl, ref result));
            return result != IntPtr.Zero ? new Bookmark(result, this.m_ref) : null;
        }
	    /// <summary> Adds the specified Bookmark as the new right sibling to this Bookmark,
	    /// adjusting the tree containing this Bookmark appropriately.
	    /// 
	    /// </summary>
	    /// <param name="bookmark">The Bookmark object to be added to this Bookmark.
	    /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Parameter in_bookmark must not be linked to a bookmark tree. </remarks>
        public void AddNext(Bookmark bookmark)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkAddNext(mp_obj, bookmark.mp_obj));
        }
	    /// <summary> Adds a new Bookmark to the tree containing this Bookmark, as the
	    /// new left sibling.
	    /// 
	    /// </summary>
	    /// <param name="text">The title string value of the new Bookmark.
	    /// </param>
	    /// <returns> The newly created sibling Bookmark.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Bookmark AddPrev(String text)
        {
            TRN_Bookmark result = IntPtr.Zero;
            UString str = new UString(text);
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkAddNewPrev(mp_obj, str.mp_impl, ref result));
            return result != IntPtr.Zero ? new Bookmark(result, this.m_ref) : null;
        }
	    /// <summary> Adds the specified Bookmark as the new left sibling to this Bookmark,
	    /// adjusting the tree containing this Bookmark appropriately.
	    /// 
	    /// </summary>
	    /// <param name="bookmark">The Bookmark object to be added to this Bookmark.
	    /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Parameter in_bookmark must not be linked to a bookmark tree. </remarks>
        public void AddPrev(Bookmark bookmark)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkAddPrev(mp_obj, bookmark.mp_obj));
        }
	    /// <summary> Removes the Bookmark's subtree from the bookmark tree containing it.
	    /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Delete()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkDelete(mp_obj));
        }
	    /// <summary> Unlinks this Bookmark from the bookmark tree that contains it, and
	    /// adjusts the tree appropriately.
	    /// 
	    /// </summary>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  After the bookmark is unlinked is can be moved to another place
        /// in the bookmark tree located in the same document.</remarks>
        public void Unlink()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkUnlink(mp_obj));
        }

	    /// <summary> Gets the indentation level of the Bookmark in its containing tree.
	    /// 
	    /// </summary>
	    /// <returns> The indentation level of the Bookmark in its containing tree.
	    /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The root level has an indentation level of zero. </remarks>
        public int GetIndent()
        {
            int result = Int32.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkGetIndent(mp_obj, ref result));
            return result;
        }
	    /// <summary> Indicates whether the Bookmark is open.
	    /// 
	    /// </summary>
	    /// <returns> True if this Bookmark is open; otherwise false.
	    /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  An open Bookmark shows all its children. </remarks>
        public bool IsOpen()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkIsOpen(mp_obj, ref result));
            return result;
        }
	    /// <summary> Opens or closes the Bookmark.
	    /// </summary>		
	    /// <param name="is_open">If true, the Bookmark is opened. Otherwise the Bookmark is closed.
	    /// </param>
        /// <exception cref="PDFNetException"> PDFNetException the PDFNet exception </exception>
        /// <remarks>An opened Bookmark shows its children, while a closed Bookmark does not. </remarks>
        public void SetOpen(bool is_open)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkSetOpen(mp_obj, is_open));
        }
	    /// <summary> Gets the number of opened bookmarks in this subtree.
	    /// 
	    /// </summary>
	    /// <returns> The number of opened bookmarks in this subtree (not including
	    /// this Bookmark). If the item is closed, a negative integer whose
	    /// absolute value specifies how many descendants would appear if the
	    /// item were reopened.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public int GetOpenCount()
        {
            int result = Int32.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkGetOpenCount(mp_obj, ref result));
            return result;
        }

	    /// <summary> Gets the Bookmark's title string.
	    /// 
	    /// </summary>
	    /// <returns> The Bookmark’s title string).
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public String GetTitle()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkGetTitle(mp_obj, ref result.mp_impl));
            return result.ConvToManagedStr();
        }
	    /// <summary> Gets the Bookmark's title string object.
	    /// 
	    /// </summary>
	    /// <returns> The Bookmark's title string object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetTitleObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkGetTitleObj(mp_obj, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
	    /// <summary> Sets the Bookmark’s title string.
	    /// 
	    /// </summary>
	    /// <param name="title">The new title string for the bookmark.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetTitle(String title)
        {
            UString str = new UString(title);
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkSetTitle(mp_obj, str.mp_impl));
        }

	    /// <summary> Gets the Bookmark's action.
	    /// 
	    /// </summary>
	    /// <returns> The bookmark’s action.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Action GetAction()
        {
            TRN_Action result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkGetAction(mp_obj, ref result));
            return result != IntPtr.Zero ? new Action(result, this.m_ref) : null;
        }
	    /// <summary> Sets the Bookmark’s action.
	    /// 
	    /// </summary>
	    /// <param name="action">The new Action for the Bookmark.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetAction(Action action)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkSetAction(mp_obj, action.mp_action));
        }
	    /// <summary> Removes the Bookmark’s action.
	    /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void RemoveAction()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkRemoveAction(mp_obj));
        }

	    /// <summary> Gets the Bookmark's flags.
	    /// 
	    /// </summary>
	    /// <returns> The flags of the Bookmark object.
	    /// Bit 1 (least-significant bit) indicates italic font whereas
	    /// bit 2 indicates bold font.
	    /// Therefore, 0 indicates normal, 1 is italic, 2 is bold, and 3 is bold-italic.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public int GetFlags()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkGetFlags(mp_obj, ref result));
            return result;
        }
	    /// <summary> Sets the Bookmark's flags.
	    /// 
	    /// </summary>
	    /// <param name="flags">The new bookmark flags.
	    /// Bit 1 (the least-significant bit) indicates italic font whereas
	    /// bit 2 indicates bold font.
	    /// Therefore, 0 indicates normal, 1 is italic, 2 is bold, and 3 is bold-italic.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetFlags(int flags)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkSetFlags(mp_obj, flags));
        }

	    /// <summary> Gets the Bookmark's RGB color value.
	    /// 
	    /// </summary>
	    /// <param name="out_r">red component in the DeviceRGB color space</param>
	    /// <param name="out_b">blue component in the DeviceRGB color space</param>
	    /// <param name="out_g">green component in the DeviceRGB color space</param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <example>The three numbers  out_r, out_g, and out_b are in the range 0.0 to 1.0, 
	    /// representing the components in the DeviceRGB color space of the color
	    /// to be used for the Bookmark’s text.
	    /// <code>
	    /// double red, green, blue;
	    /// bookmark.GetColor(red, green, blue);
        /// </code>
        /// </example>	
        public void GetColor(double out_r, double out_g, double out_b)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkGetColor(mp_obj, ref out_r, ref out_g, ref out_b));
        }
	    /// <summary> Sets the color.
	    /// 
	    /// </summary>
	    /// <param name="r">red component in the DeviceRGB color space
	    /// </param>
	    /// <param name="g">green component in the DeviceRGB color space
	    /// </param>
	    /// <param name="b">blue component in the DeviceRGB color space
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetColor(double r, double g, double b)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkSetColor(mp_obj, r, g, b));
        }

	    /// <summary> Returns the underlying SDF/Cos object.
	    /// 
	    /// </summary>
	    /// <returns> The underlying SDF/Cos object.
	    /// 
	    /// </returns>
	    /// <remarks>  A null (non-valid) bookmark returns a null object. </remarks>
        public Obj GetSDFObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_BookmarkGetSDFObj(mp_obj, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
    }
}
