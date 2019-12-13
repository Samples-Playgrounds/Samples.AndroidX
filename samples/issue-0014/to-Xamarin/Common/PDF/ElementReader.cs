using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

using pdftron.Common;
using pdftron.SDF;

using TRN_ElementReader = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_Obj = System.IntPtr;
using TRN_Element = System.IntPtr;
using TRN_Iterator = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> ElementReader can be used to parse and process content streams. ElementReader provides a 
    /// convenient interface used to traverse the Element display list of a page. The display list 
    /// representing graphical elements (such as text-runs, paths, images, shadings, forms, etc) is 
    /// accessed using the intrinsic iterator. ElementReader automatically concatenates page contents
    /// spanning multiple streams and provides a mechanism to parse contents of sub-display lists 
    /// (e.g. forms XObjects and Type3 fonts).
    /// </summary>
    /// <example>
    /// For a full sample, please refer to ElementReader and ElementReaderAdv sample projects.
    /// A sample use case for ElementReader is given below:	
    /// <code>  
    /// ...
    /// ElementReader reader=new ElementReader();
    /// reader.Begin(page);
    /// for (Element element=reader.next(); element!=null;element=reader.next()) {
    /// Rect bbox;
    /// if((bbox=element.getBBox())!=null) System.out.println("Bounding Box: " + bbox.getRectangle());
    /// switch (element.getType())	{
    /// case Element.e_path: { // Process path data...
    /// double[] data = element.getPathPoints();
    /// }
    /// break; 
    /// case Element.e_text: 
    /// // ...
    /// break;
    /// }
    /// }
    /// reader.End();	    
    /// </code>	
    /// </example>
    public class ElementReader : IDisposable
    {
        internal TRN_ElementReader mp_reader = IntPtr.Zero;
        internal Object m_ref;

        /// <summary> Releases all resources used by the ElementReader </summary>
	    ~ElementReader()
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
            Destroy();
        }

        public void Destroy()
        {
            if (mp_reader != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_ElementReaderDestroy(mp_reader));
                mp_reader = IntPtr.Zero;
            }
        }

        // Methods
        /// <summary> Instantiates a new element reader.
	    /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public ElementReader()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementReaderCreate(ref mp_reader));
        }

	    /// <summary> Begin processing a page.
	    /// 
	    /// </summary>
	    /// <param name="page">A page to start processing.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  When page processing is completed, make sure to call ElementReader.End(). </remarks>
        public void Begin(Page page)
        {
            Begin(page, null);
        }
	    /// <summary> Begin processing a page.
	    /// 
	    /// </summary>
	    /// <param name="page">A page to start processing.
	    /// </param>
	    /// <param name="ctx">the ctx
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  When page processing is completed, make sure to call ElementReader.End(). </remarks>
        public void Begin(Page page, pdftron.PDF.OCG.Context ctx)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementReaderBeginOnPage(mp_reader, page.mp_page, (ctx != null) ? ctx.mp_impl:IntPtr.Zero));
            this.m_ref = page.m_ref;
        }
	    /// <summary> Begin processing given content stream. The content stream may be
	    /// a Form XObject, Type3 glyph stream, pattern stream or any other content stream.
	    /// 
	    /// </summary>
	    /// <param name="content_stream">- A stream object representing the content stream (usually
	    /// a Form XObject).
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  When page processing is completed, make sure to call ElementReader.End(). </remarks>
        public void Begin(Obj content_stream)
        {
            Begin(content_stream, null, null);
        }
	    /// <summary> Begin processing given content stream. The content stream may be
	    /// a Form XObject, Type3 glyph stream, pattern stream or any other content stream.
	    /// 
	    /// </summary>
	    /// <param name="content_stream">- A stream object representing the content stream (usually
	    /// a Form XObject).
	    /// </param>
	    /// <param name="resource_dict">- An optional '/Resource' dictionary parameter.
	    /// If content stream refers to named resources that are not present in
	    /// the local Resource dictionary, the names are looked up in the supplied
	    /// resource dictionary.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  When page processing is completed, make sure to call ElementReader.End(). </remarks>
        public void Begin(Obj content_stream, Obj resource_dict)
        {
            Begin(content_stream, resource_dict, null);
        }
	    /// <summary> Begin processing given content stream. The content stream may be
	    /// a Form XObject, Type3 glyph stream, pattern stream or any other content stream.
	    /// 
	    /// </summary>
	    /// <param name="content_stream">- A stream object representing the content stream (usually
	    /// a Form XObject).
	    /// </param>
	    /// <param name="resource_dict">- An optional '/Resource' dictionary parameter.
	    /// If content stream refers to named resources that are not present in
	    /// the local Resource dictionary, the names are looked up in the supplied
	    /// resource dictionary.
	    /// </param>
	    /// <param name="ctx">the ctx
	    /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  When page processing is completed, make sure to call ElementReader.End(). </remarks>
        public void Begin(Obj content_stream, Obj resource_dict, pdftron.PDF.OCG.Context ctx)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementReaderBegin(mp_reader, content_stream.mp_obj, (resource_dict!= null) ? resource_dict.mp_obj : IntPtr.Zero, (ctx != null) ? ctx.mp_impl : IntPtr.Zero));
            this.m_ref = content_stream.GetRefHandleInternal();
        }

	    /// <summary> Next.
	    /// 
	    /// </summary>
	    /// <returns> a page Element or a 'NULL' element if the end of current-display list was
	    /// reached. You may use GetType() to determine the type of the returned Element.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  Every call to ElementReader::Next() destroys the current Element.
	    /// Therefore, an Element becomes invalid after subsequent
	    /// ElementReader::Next() operation.</remarks>
        public Element Next()
        {
            //TODO: result doesn't get set
            TRN_Element result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementReaderNext(mp_reader, ref result));
            return (result != IntPtr.Zero) ? new Element(result, this, this.m_ref) : null;
        }
	    /// <summary> Current.
	    /// 
	    /// </summary>
	    /// <returns> the current Element or a 'NULL' Element. The current element is the one
	    /// returned in the last call to Next().
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Every call to ElementReader::Next() destroys the current Element. Therefore, an
        /// Element becomes invalid after subsequent ElementReader::Next() operation.</remarks>
        public Element Current()
        {
            TRN_Element result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementReaderCurrent(mp_reader, ref result));
            return new Element(result, this, this.m_ref);
        }

	    /// <summary> When the current element is a form XObject you have the option to skip form
	    /// processing (by not calling FormBegin()) or to open the form stream and
	    /// continue Element traversal into the form.
	    /// 
	    /// To open a form XObject display list use FormBegin() method. The Next() returned
	    /// Element will be the first Element in the form XObject display list. Subsequent calls to Next()
	    /// will traverse form's display list until NULL is returned. At any point you can
	    /// close the form sub-list using ElementReader::End() method. After the form display
	    /// list is closed (using End()) the processing will return to the parent display list
	    /// at the point where it left off before entering the form XObject.
	    /// 
	    /// </summary>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void FormBegin()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementReaderFormBegin(mp_reader));
        }
	    /// <summary> Pattern begin.
	    /// 
	    /// </summary>
	    /// <param name="fill_pattern">the fill_pattern
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void PatternBegin(bool fill_pattern)
        {
            PatternBegin(fill_pattern, false);
        }
	    /// <summary> A method used to spawn the sub-display list representing the tiling pattern
	    /// of the current element in the ElementReader. You can call this method at any
	    /// point as long as the current element is valid.
	    /// 
	    /// </summary>
	    /// <param name="fill_pattern">If true, the filling pattern of the current element will
	    /// be spawned; otherwise, the stroking pattern of the current element will be
	    /// spawned. Note that the graphics state will be inherited from the parent content
	    /// stream (the content stream in which the pattern is defined as a resource) automatically.
	    /// </param>
	    /// <param name="reset_ctm_tfm">An optional parameter used to indicate whether the pattern's
	    /// display list should set its initial CTM and transformation matrices to identity matrix.
	    /// In general, we should leave it to be false.
	    /// 
	    /// To open a tiling pattern sub-display list use PatternBegin() method.
	    /// The Next() returned Element will be the first Element in the pattern display list.
	    /// Subsequent calls to Next() will traverse pattern's display list until NULL is
	    /// encountered. At any point you can close the pattern sub-list using
	    /// ElementReader::End() method. After the pattern display list is closed,
	    /// the processing will return to the parent display list at the point where
	    /// pattern display list was spawned.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void PatternBegin(bool fill_pattern, bool reset_ctm_tfm)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementReaderPatternBegin(mp_reader, fill_pattern, reset_ctm_tfm));
        }
	    /// <summary> A method used to spawn a sub-display list representing a Type3 Font glyph. You can
	    /// call this method at any point as long as the current element in the ElementReader
	    /// is a text element whose font type is type 3.
	    /// 
	    /// </summary>
	    /// <param name="char_data">The information about the glyph to process. You can get this information
	    /// by dereferencing a CharIterator.
	    /// </param>		
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Type3FontBegin(CharData char_data)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementReaderType3FontBegin(mp_reader, ref char_data.mp_imp, IntPtr.Zero));
        }
	    /// <summary> Close the current display list.
	    /// 
	    /// If the current display list is a sub-list created using FormBegin(), PatternBegin(),
	    /// or Type3FontBegin() methods, the function will end the sub-list and will return
	    /// processing to the parent display list at the point where it left off before
	    /// entering the sub-list.
	    /// 
	    /// </summary>
	    /// <returns> true if the closed display list is a sub-list or false if it is a root
	    /// display list.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool End()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementReaderEnd(mp_reader, ref result));
            return result;
        }

	    /// <summary> Gets the changes iterator.
	    /// 
	    /// </summary>
	    /// <returns> an iterator to the beginning of the list containing identifiers of modified
	    /// graphics state attributes since the last call to ClearChangeList(). The list can
	    /// be consulted to determine which graphics states were modified between two
	    /// Elements. Attributes are ordered in the same way as they are set in the content
	    /// stream. Duplicate attributes are eliminated.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public GSChangesIterator GetChangesIterator()
        {
            TRN_Iterator result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementReaderGetChangesIterator(mp_reader, ref result));
            return new GSChangesIterator(result);
        }
	    /// <summary> Checks if is changed.
	    /// 
	    /// </summary>
	    /// <param name="attrib">the gstate_attrib
	    /// </param>
	    /// <returns> true if given GState attribute was changed since the last call to
	    /// ClearChangeList().
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool IsChanged(GState.GStateAttribute attrib)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementReaderIsChanged(mp_reader, attrib, ref result));
            return result;
        }
	    /// <summary> Clear the list containing identifiers of modified graphics state attributes.
	    /// The list of modified attributes is then accumulated during a subsequent call(s)
	    /// to ElementReader.Next().
	    /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void ClearChangeList()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementReaderClearChangeList(mp_reader));
        }

	    /// <summary> Gets the font.
	    /// 
	    /// </summary>
	    /// <param name="name">the name
	    /// </param>
	    /// <returns> SDF/Cos object matching the specified name in the current resource
	    /// dictionary. For 'Page' the name is looked up in the page's /Resources/&lt;Class&gt;
	    /// dictionary. For Form XObjects, Patterns, and Type3 fonts that have a content
	    /// stream within page content stream the specified resource is first looked-up in the
	    /// resource dictionary of the inner stream. If the resource is not found, the name is
	    /// looked up in the outer content stream’s resource dictionary. The function returns
	    /// NULL if the resource was not found.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetFont(string name)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementReaderGetFont(mp_reader, name, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
	    /// <summary> Gets the x object.
	    /// 
	    /// </summary>
	    /// <param name="name">the name
	    /// </param>
	    /// <returns> the x object
	    /// </returns>
	    /// <remarks>  see ElementReader::GetFont </remarks>
        public Obj GetXObject(string name)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementReaderGetXObject(mp_reader, name, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
	    /// <summary> Gets the shading.
	    /// 
	    /// </summary>
	    /// <param name="name">the name
	    /// </param>
	    /// <returns> the shading
	    /// </returns>
	    /// <remarks>  see ElementReader::GetFont </remarks>
        public Obj GetShading(string name)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementReaderGetShading(mp_reader, name, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
	    /// <summary> Gets the color space.
	    /// 
	    /// </summary>
	    /// <param name="name">the name
	    /// </param>
	    /// <returns> the color space
	    /// </returns>
	    /// <remarks>  see ElementReader::GetFont </remarks>
        public Obj GetColorSpace(string name)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementReaderGetColorSpace(mp_reader, name, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
	    /// <summary> Gets the pattern.
	    /// 
	    /// </summary>
	    /// <param name="name">the name
	    /// </param>
	    /// <returns> the pattern
	    /// </returns>
	    /// <remarks>  see ElementReader::GetFont </remarks>
        public Obj GetPattern(string name)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementReaderGetPattern(mp_reader, name, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
	    /// <summary> Gets the ext g state.
	    /// 
	    /// </summary>
	    /// <param name="name">the name
	    /// </param>
	    /// <returns> the ext g state
	    /// </returns>
	    /// <remarks>  see ElementReader::GetFont </remarks>
        public Obj GetExtGState(string name)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementReaderGetExtGState(mp_reader, name, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
    }
}
