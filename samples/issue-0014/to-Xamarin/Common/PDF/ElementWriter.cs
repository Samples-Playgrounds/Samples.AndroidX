using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

using pdftron.Common;
using pdftron.SDF;

using TRN_ElementWriter = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_Obj = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> ElementWriter can be used to assemble and write new content to a page, Form XObject, 
    /// Type3 Glyph stream, pattern stream, or any other content stream. 
    /// </summary>
    public class ElementWriter : IDisposable
    {
        internal TRN_ElementWriter mp_writer = IntPtr.Zero;
        internal Object m_ref;

        /// <summary> Releases all resources used by the ElementWriter </summary>
        ~ElementWriter()
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

        internal ElementWriter(TRN_ElementWriter impl)
        {
            this.mp_writer = impl;
        }

        public void Destroy()
        {
            if (mp_writer != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_ElementWriterDestroy(mp_writer));
                mp_writer = IntPtr.Zero;
            }
        }

        /// <summary> Instantiates a new element writer.
        /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public ElementWriter() 
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementWriterCreate(ref mp_writer));
        }

        /// <summary> Begin writing to the given page.
	    /// 
	    /// By default, new content will be appended to the page, as foreground graphics.
	    /// It is possible to add new page content as background graphics by setting the
	    /// second parameter in begin method to 'true' (e.g. writer.Begin(page, true)).
	    /// 
	    /// </summary>
	    /// <param name="page">The page to write content.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Begin(Page page)
        {
            Begin(page, WriteMode.e_overlay, true, true);
        }
	    /// <summary> Begin writing to the given page.
	    /// By default, new content will be appended to the page, as foreground graphics.
	    /// It is possible to add new page content as background graphics by setting the
	    /// second parameter in begin method to 'true' (e.g. writer.Begin(page, true)).
	    /// </summary>
	    /// <param name="page">The page to write content. 
	    /// </param>
	    /// <param name="placement">An optional flag indicating whether the new content should 
	    /// be added as a foreground or background layer to the existing page. By default, the new
	    /// content will appear on top of the existing graphics. 
        /// </param>	
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Begin(Page page, WriteMode placement)
        {
            Begin(page, placement, true, true);
        }
	    /// <summary> Begin writing to the given page.
	    /// By default, new content will be appended to the page, as foreground graphics. 
	    /// It is possible to add new page content as background graphics by setting the 
	    /// second parameter in begin method to 'true' (e.g. writer.Begin(page, true)).
	    /// </summary>
	    /// <param name="page">The page to write content. 
	    /// </param>
	    /// <param name="placement">An optional flag indicating whether the new content should 
	    /// be added as a foreground or background layer to the existing page. By default, the new
	    /// content will appear on top of the existing graphics. 
	    /// </param>	
	    /// <param name="page_coord_sys">An optional flag used to select the target coordinate system.
	    ///  If true (default), the coordinates are relative to the lower-left corner of the page,
	    ///  otherwise the coordinates are defined in PDF user coordinate system (which may, 
	    ///  or may not coincide with the page coordinates).
	    /// </param>
	    /// <param name="compress">An optional flag indicating whether the page content stream 
	    /// should be compressed. This may be useful for debugging content streams. Also 
	    /// some applications need to do a clear text search on strings in the PDF files.
	    /// By default, all content streams are compressed.
        /// </param>	
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Begin(Page page, WriteMode placement, Boolean page_coord_sys, Boolean compress)
        {
            Begin(page, placement, page_coord_sys, compress, null);
        }
        public void Begin(Page page, WriteMode placement, Boolean page_coord_sys, Boolean compress, Obj resources)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementWriterBeginOnPage(mp_writer, page.mp_page, placement, page_coord_sys, compress, resources != null ? resources.mp_obj : IntPtr.Zero));
            m_ref = page.m_ref;
        }
        /// <summary> Begin writing to the given page.
        /// By default, new content will be appended to the page, as foreground graphics.
        /// It is possible to add new page content as background graphics by setting the
        /// second parameter in begin method to 'true' (e.g. writer.Begin(page, true)).
        /// </summary>
        /// <param name="page">The page to write content. 
        /// </param>
        /// <param name="placement">An optional flag indicating whether the new content should 
        /// be added as a foreground or background layer to the existing page. By default, the new
        /// content will appear on top of the existing graphics. 
        /// </param>	
        /// <param name="page_coord_sys">An optional flag used to select the target coordinate system.
        ///  If true (default), the coordinates are relative to the lower-left corner of the page,
        ///  otherwise the coordinates are defined in PDF user coordinate system (which may, 
        ///  or may not coincide with the page coordinates).
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Begin(Page page, WriteMode placement, Boolean page_coord_sys)
        {
            Begin(page, placement, page_coord_sys, true);
        }
	
	    /// <summary> Begin writing an Element sequence to a new stream. Use this function to write
	    /// Elements to a content stream other than the page. For example, you can create
	    /// Form XObjects (See Section '4.9 Form XObjects' in PDF Reference for more details)
	    /// pattern streams, Type3 font glyph streams, etc.
	    /// 
	    /// </summary>
	    /// <param name="doc">- A low-level SDF/Cos document that will contain the new stream. You can
	    /// access low-level document using PDFDoc.GetSDFDoc() or Obj.GetDoc() methods.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  the newly created content stream object is returned when writing operations 
        /// are completed (i.e. after the call to ElementWriter.End()).</remarks>
        public void Begin(SDF.SDFDoc doc)
        {
            Begin(doc, true);
        }
	    /// <summary> Begin writing an Element sequence to a new stream. Use this function to write 
	    /// Elements to a content stream other than the page. For example, you can create
	    /// Form XObjects (See Section '4.9 Form XObjects' in PDF Reference for more details) 
	    /// pattern streams, Type3 font glyph streams, etc.
	    /// </summary>
	    /// <param name="doc">the doc
	    /// </param>
	    /// <param name="compress">the compress
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Begin(SDF.SDFDoc doc, Boolean compress)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementWriterBegin(mp_writer, doc.mp_doc, compress));
            m_ref = doc;
        }
	    /// <summary> Begin writing an Element sequence to a stream. Use this function to write 
	    /// Elements to a content stream which will replace an existing content stream in an
	    /// object passed as a parameter.
	    /// </summary>
	    /// <param name="stream_to_modify">the streamobj_to_update
	    /// </param>
	    /// <param name="compress">the compress
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Begin(SDF.Obj stream_to_modify, bool compress)
        {
            Begin(stream_to_modify, compress, null);
        }
        /// <summary> Begin writing an Element sequence to a stream. Use this function to write 
	    /// Elements to a content stream which will replace an existing content stream in an
	    /// object passed as a parameter.
	    /// </summary>
	    /// <param name="stream_to_modify">the streamobj_to_update
	    /// </param>
	    /// <param name="compress">the compress
        /// </param>
        /// <param name="resources">the resource dictionary in which to store resources for the final page. 
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Begin(SDF.Obj stream_to_modify, bool compress, SDF.Obj resources)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementWriterBeginOnObj(mp_writer, stream_to_modify.mp_obj, compress, resources != null ? resources.mp_obj : IntPtr.Zero));
            m_ref = stream_to_modify;
        }
        /// <summary> Finish writing to a page.
        /// 
        /// </summary>
        /// <returns> A low-level stream object that was used to store Elements.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj End()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementWriterEnd(mp_writer, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }

	    /// <summary> Writes an arbitrary buffer to the content stream.
	    /// This function can be used to insert comments, inline-image data, and
	    /// chunks of arbitrary content to the output stream.
	    /// 
	    /// </summary>
	    /// <param name="buf">the data
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void WriteBuffer(byte[] buf)
        {
            GCHandle pinnedRawData = GCHandle.Alloc(buf, GCHandleType.Pinned);
            IntPtr pnt = pinnedRawData.AddrOfPinnedObject();
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementWriterWriteBuffer(mp_writer, pnt, buf.Length));
        }
	    /// <summary> Writes an arbitrary string to the content stream.
	    /// Serves the same purpose as WriteBuffer().
	    /// 
	    /// </summary>
	    /// <param name="data">the str
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void WriteString(String data)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementWriterWriteString(mp_writer, data));
        }

        public void SetDefaultGState(ElementReader reader)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementWriterSetDefaultGState(mp_writer, reader.mp_reader));
        }

        /// <summary> Writes the Element to the content stream.
        /// 
        /// </summary>
        /// <param name="element">the element
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void WriteElement(Element element)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementWriterWriteElement(mp_writer, element.mp_elem));
        }
	    /// <summary> A utility function that surrounds the given Element with a graphics state
	    /// Save/Restore Element (i.e. in PDF content stream represented as 'q element Q').
	    /// 
	    /// The function is equivalent to calling WriteElement three times:
	    /// WriteElement(eSave);
	    /// WriteElement(element);
	    /// WriteElement(eRestore);
	    /// 
	    /// where eSave is 'e_group_begin' and eRestore is 'e_group_end' Element
	    /// 
	    /// The function is useful when XObjects such as Images and Forms are drawn on
	    /// the page.
	    /// 
	    /// </summary>
	    /// <param name="element">the element
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void WritePlacedElement(Element element)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementWriterWritePlacedElement(mp_writer, element.mp_elem));
        }

	    /// <summary> The Flush method flushes all pending Element writing operations.
	    /// This method is typically only required to be called when intermixing
	    /// direct content writing (i.e. WriteBuffer/WriteString) with Element writing.
	    /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Flush()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementWriterFlush(mp_writer));
        }

        // Nested Types
        ///<summary>Enumeration describing the placement of the element written to a page.</summary>
        public enum WriteMode
        {
            ///<summary>element is put in the background layer of the page</summary>
            e_underlay,
            ///<summary>element appears on top of the existing graphics</summary>
            e_overlay,
            ///<summary>element will replace current page contents</summary>
            e_replacement
        }

    }
}
