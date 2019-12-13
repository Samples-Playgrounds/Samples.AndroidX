using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using pdftron.Common;

using TRN_Obj = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_SDFDoc = System.IntPtr;
using TRN_DictIterator = System.IntPtr;
using TRN_Filter = System.IntPtr;

using pdftron;
using pdftronprivate.trn;

namespace pdftron.SDF
{
    /// <summary> Obj is a concrete class for all SDF/Cos objects. Obj hierarchy implements the 
    /// composite design pattern. As a result, you can invoke a member function of any 
    /// 'derived' object through Obj interface. If the member function is not supported 
    /// (e.g. if you invoke Obj.GetNumber() on a boolean object) an Exception will be 
    /// thrown.
    /// 
    /// You can use GetType() or obl.Is???() member functions to find out type-information at 
    /// run time, however most of the time the type can be inferred from the PDF specification.
    /// Therefore when you call Doc.GetTrailer() you can assume that returned object is 
    /// a dictionary. If there is any ambiguity use Is???() methods. 
    /// 
    /// Objects can't be shared across documents, however you can use Doc.ImportObj() 
    /// to copy objects from one document to another. 
    /// 
    /// Objects can be shared within a document provided that they are created as indirect.
    /// Indirect objects are the ones that are referenced in cross-reference table.
    /// To create an object as indirect use Doc.CreateIndirect???() (where ? is the 
    /// Object type).
    /// </summary>
    public class Obj : IDisposable
    {
        internal TRN_Obj mp_obj = IntPtr.Zero;
        private Object m_ref;

        /// <summary> Releases all resources used by the Obj </summary>
			~Obj()
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

        // Methods
        internal Obj(TRN_Obj obj, Object reference)
        {
            this.mp_obj = obj;
            this.m_ref = reference;
        }

        public IntPtr GetHandleInternal()
        {
            return this.mp_obj;
        }

        public Object GetRefHandleInternal()
        {
            return this.m_ref;
        }

        // Methods
        /// <summary> Gets the type.
		/// 
		/// </summary>
		/// <returns> the object type.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this method can be invoked on any Obj. </remarks>
		public new ObjType GetType() 
        {
            ObjType result = ObjType.e_null;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjGetType(mp_obj, ref result));
            return result;
        }
		/// <summary> Checks if is bool.
		/// 
		/// </summary>
		/// <returns> true if this is a Bool object, false otherwise.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this method can be invoked on any Obj. </remarks>
		public bool IsBool() 
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjIsBool(mp_obj, ref result));
            return result;
        }
		/// <summary> Checks if is number.
		/// 
		/// </summary>
		/// <returns> true if this is a Number object, false otherwise.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this method can be invoked on any Obj. </remarks>
		public bool IsNumber() 
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjIsNumber(mp_obj, ref result));
            return result;
        }
		/// <summary> Checks if is null.
		/// 
		/// </summary>
		/// <returns> true if this is a Null object, false otherwise.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this method can be invoked on any Obj. </remarks>
		public bool IsNull()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjIsNull(mp_obj, ref result));
            return result;
        }
		/// <summary> Checks if is string.
		/// 
		/// </summary>
		/// <returns> true if this is a Str (string) object, false otherwise.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this method can be invoked on any Obj. </remarks>
		public bool IsString() 
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjIsString(mp_obj, ref result));
            return result;
        }
		/// <summary> Checks if is name.
		/// 
		/// </summary>
		/// <returns> true if this is Name, false otherwise.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this method can be invoked on any Obj. </remarks>
		public bool IsName() {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjIsName(mp_obj, ref result));
            return result;
        }
		/// <summary> Checks if is indirect.
		/// 
		/// </summary>
		/// <returns> true if this is Indirect object (i.e. object referenced in the
		/// cross-reference table), false otherwise.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this method can be invoked on any Obj. </remarks>
		public bool IsIndirect() {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjIsIndirect(mp_obj, ref result));
            return result;
        }
		/// <summary> Checks if is container.
		/// 
		/// </summary>
		/// <returns> true if this is a Container (a dictionary, array, or a stream),
		/// false otherwise.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this method can be invoked on any Obj. </remarks>
		public bool IsContainer() {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjIsContainer(mp_obj, ref result));
            return result;
        }
		/// <summary> Checks if is dict.
		/// 
		/// </summary>
		/// <returns> true if this is a dictionary (i.e. Dict), false otherwise.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this method can be invoked on any Obj. </remarks>
		public bool IsDict() {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjIsDict(mp_obj, ref result));
            return result;
        }
		/// <summary> Checks if is array.
		/// 
		/// </summary>
		/// <returns> true if this is an Array, false otherwise.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this method can be invoked on any Obj. </remarks>
		public bool IsArray() {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjIsArray(mp_obj, ref result));
            return result;
        }
		/// <summary> Checks if is stream.
		/// 
		/// </summary>
		/// <returns> true if this is a Stream, false otherwise.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this method can be invoked on any Obj. </remarks>
		public bool IsStream() {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjIsStream(mp_obj, ref result));
            return result;
        }

		/// <summary> Gets the doc.
		/// 
		/// </summary>
		/// <returns> the document to which this object belongs.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this method can be invoked on any Obj. </remarks>
		public SDFDoc GetDoc() {
            TRN_SDFDoc temp_doc = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjGetDoc(mp_obj, ref temp_doc));
            return new SDFDoc(temp_doc, m_ref);
        }
		/// <summary> The function writes the Obj to the output stream.
		/// 
		/// </summary>
		/// <param name="stream">- the input stream where the Obj will be written
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this method can be invoked on any Obj. </remarks>
		public void Write(Filters.FilterWriter stream) {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjWrite(mp_obj, stream.mp_imp));
        }
		/// <summary> Size.
		/// 
		/// </summary>
		/// <returns> the 'size' of the object. The definition of 'size' depends on
		/// the object type. In particular:
		/// - For a dictionary or a stream object, the method will return the
		/// number of key/value pairs in the dictionary.
		/// - For an array object the method will return the number of Obj
		/// enties in the array.
		/// - For a string object the method will return the number of bytes
		/// in the string buffer.
		/// - For any other object the method will always return 1.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this method can be invoked on any Obj. </remarks>
		public int Size() {
            UIntPtr result = UIntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjSize(mp_obj, ref result));
            return System.Convert.ToInt32(result.ToUInt32());
        }

		/// <summary> Checks if is equal.
		/// 
		/// </summary>
		/// <param name="to">the to
		/// </param>
		/// <returns> true if two Obj pointers point to the same object.
		/// This method does not compare object content so you should not use
		/// this method to compare two (e.g. Number) objects
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public bool IsEqual(Obj to) {
            if (to == null)
                return false;
            return (this == to);
        }
		/// <summary>Equality operator checks whether two <c>Obj</c> objects are the same.</summary>
		/// <param name="lhs"><c>Obj</c> at the left of the operator
		/// </param>
		/// <param name="rhs"><c>Obj</c> at the right of the operator
		/// </param>
		/// <returns>true if both objects are equals, false otherwise
		/// </returns>
		public static bool operator==(Obj lhs, Obj rhs) {
            if (System.Object.ReferenceEquals(lhs, null) && System.Object.ReferenceEquals(rhs, null))
		        return true;

	        if ( System.Object.ReferenceEquals(lhs, null) && !System.Object.ReferenceEquals(rhs, null))
		        return false;

	        if ( !System.Object.ReferenceEquals(lhs, null) && System.Object.ReferenceEquals(rhs, null))
		        return false;
            
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjIsEqual(lhs.mp_obj, rhs.mp_obj, ref result));
            return result;
        }
		/// <summary>Inequality operator checks whether two <c>Obj</c> objects are different.</summary>
		/// <param name="lhs"><c>Obj</c> at the left of the operator
		/// </param>
		/// <param name="rhs"><c>Obj</c> at the right of the operator
		/// </param>
		/// <returns>true if both objects are not equals, false otherwise
		/// </returns>
		public static bool operator!=(Obj lhs, Obj rhs) {
            return !(lhs==rhs);
        }

		/// <summary> Gets the obj num.
		/// 
		/// </summary>
		/// <returns> object number. If this is not an Indirect object, object number of
		/// a containing indirect object is returned.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this method can be invoked on any Obj. </remarks>
		public int GetObjNum() {
            uint result = uint.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjGetObjNum(mp_obj, ref result));
            return System.Convert.ToInt32(result);
        }
		/// <summary> Gets the gen num.
		/// 
		/// </summary>
		/// <returns> generation number. If this is not an Indirect object, generation number of
		/// a containing indirect object is returned.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this method can be invoked on any Obj. </remarks>
		public int GetGenNum() {
            UInt16 result = UInt16.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjGetGenNum(mp_obj, ref result));
            return System.Convert.ToInt32(result);
        }
		/// <summary> Gets the offset.
		/// 
		/// </summary>
		/// <returns> object offset from the beginning of the file. If this is not an Indirect object,
		/// offset of a containing indirect object is returned.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this method can be invoked on any Obj. </remarks>
		public int GetOffset() {
            UIntPtr result = UIntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjGetOffset(mp_obj, ref result));
            return System.Convert.ToInt32(result.ToUInt32());
        }
		/// <summary> Checks if is free.
		/// 
		/// </summary>
		/// <returns> true if the object is in use or is marked as free.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this method can be invoked on any Obj. </remarks>
		public bool IsFree() {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjIsFree(mp_obj, ref result));
            return result;
        }
		/// <summary> Set the object mark. Mark is a boolean value that can be associated with every
		/// indirect object. This is especially useful when and object graph should be
		/// traversed and an operation should be performed on each node only once.
		/// 
		/// </summary>
		/// <param name="mark">the new mark
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this method can be invoked on any Obj. </remarks>
		public void SetMark(bool mark) {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjSetMark(mp_obj, mark));
        }
		/// <summary> Checks if is marked.
		/// 
		/// </summary>
		/// <returns> true if the object is marked.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this method can be invoked on any Obj. </remarks>
		public bool IsMarked() {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjIsMarked(mp_obj, ref result));
            return result;
        }
		/// <summary> Checks if is loaded.
		/// 
		/// </summary>
		/// <returns> true if the object is loaded in memory.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this method can be invoked on any Obj. </remarks>
		public bool IsLoaded() {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjIsLoaded(mp_obj, ref result));
            return result;
        }

		// bool Specific Methods ----------------------------------------------------
		/// <summary> Gets the bool.
		/// 
		/// </summary>
		/// <returns> bool value if this is Bool.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public bool GetBool() {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjGetBool(mp_obj, ref result));
            return result;
        }
		/// <summary> Sets the bool.
		/// 
		/// </summary>
		/// <param name="b">- bool value used to set Bool object.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public void SetBool(bool b) {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjSetBool(mp_obj, b));
        }

		// Number Specific Methods -----------------------------------------------------
		/// <summary> Gets the number.
		/// 
		/// </summary>
		/// <returns> value, if this is Number.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public double GetNumber() {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjGetNumber(mp_obj, ref result));
            return result;
        }
		/// <summary> Sets the number.
		/// 
		/// </summary>
		/// <param name="n">- value used to set Number object.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public void SetNumber(double n) {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjSetNumber(mp_obj, n));
        }

		// string Specific Methods -----------------------------------------------------
		/// <summary> Gets the buffer.
		/// 
		/// </summary>
		/// <returns> a pointer to the string buffer. Please note that the string may not
		/// be NULL terminated and that it may not be represented in ASCII or Unicode
		/// encoding. For more information on SDF/Cos string objects, please refer to
		/// section 3.2.3 'string Objects' in PDF Reference Manual.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  if SDF/Cos string object is represented as 'PDF Text' (Section 3.8.1 
		/// 'Text Strings' in PDF Reference) you can use GetAsPDFText method to obtain
		/// Unicode representation of the string.
		/// </remarks>
		/// <remarks>  use Size() member function in order to obtain the number of bytes in string buffer. </remarks>
		public byte[] GetBuffer() 
        {
            IntPtr src = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjGetBuffer(mp_obj, ref src));

            byte[] result = new byte[Size()];
            Marshal.Copy(src, result, 0, Size());
            return result;
        }
		/// <summary> Convert the SDF/Cos string object to 'PDF Text string' (a Unicode string).
		/// 
		/// PDF Text Strings are not used to represent page content, however they
		/// are used in text annotations, bookmark names, article names, document
		/// information etc. These strings are encoded in either PDFDocEncoding or
		/// Unicode character encoding. For more information on PDF Text Strings,
		/// please refer to section 3.8.1 'Text Strings' in PDF Reference.
		/// 
		/// </summary>
		/// <returns> the as pdf text
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  Not all SDF/Cos string objects are used to represent 'PDF Text'. 
		/// PDF Reference indicates (on a case by case basis ) where an SDF/Cos string
		/// object can be used as 'PDF Text'.
		/// </remarks>
		public string GetAsPDFText() {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjGetAsPDFText(mp_obj, ref result.mp_impl));
            return result.ConvToManagedStr();
        }
		/// <summary> Sets the string object value.
		/// 
		/// </summary>
		/// <param name="str">the new string
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public void SetString(string str) {
            UString str2 = new UString(str);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjSetUString(mp_obj, str2.mp_impl));
        }
		/// <summary> Sets the string object value.
		/// 
		/// </summary>
		/// <param name="buf">the new string
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetString(byte[] buf) {
            BasicTypes.TRN_String keyString = new BasicTypes.TRN_String();
            int psize = Marshal.SizeOf(buf[0]) * buf.Length;
            IntPtr pnt = Marshal.AllocHGlobal(psize);
            try
            {
                Marshal.Copy(buf, 0, pnt, buf.Length);
                
                keyString.str = pnt;
                keyString.len = System.Convert.ToUInt32(buf.Length);
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjSetString(mp_obj, keyString));
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }
        }

		// Name Specific Methods -------------------------------------------------------
		/// <summary> Gets the name.
		/// 
		/// </summary>
		/// <returns> string representing the Name object.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public string GetName() {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjGetName(mp_obj, ref result));
            string res = Marshal.PtrToStringAnsi(result);
            return res;
        }
		/// <summary> Sets the name.
		/// 
		/// </summary>
		/// <param name="name">- value used to set Name object.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public void SetName(string name) {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjSetName(mp_obj, name));
        }

		// Dictionary Specific Methods -------------------------------------------------
		/// <summary> Gets the dict iterator.
		/// 
		/// </summary>
		/// <returns> an iterator that addresses the first element in the dictionary.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <code>  
		/// DictIterator itr = dict.getDictIterator();
		/// while (itr.hasNext()) {
		/// Obj key = itr.key();
		/// Obj value = itr.value();
		/// // ...
		/// itr.next()
		/// }
		/// }
		/// </code>
		public DictIterator GetDictIterator() {
            TRN_DictIterator result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjGetDictIterator(mp_obj, ref result));
            return new DictIterator(result, this.m_ref);
        }
		/// <summary> Search the dictionary for a given key.
		/// 
		/// </summary>
		/// <param name="key">- a key to search for in the dictionary
		/// </param>
		/// <returns> The iterator to the matching key/value pair or invalid iterator
		/// (i.e. itr.hasNext()==fase) if the if the dictionary does not contain the given key.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>A dictionary entry whose value is Obj.Null is equivalent to an absent entry. </remarks>
		/// <code>  
		/// DictIterator itr = info_dict.find("Info");
		/// if (itr.hasNext()) {
		/// Obj info = itr.value();
		/// if (info.isDict())
		/// info.putString("Producer", "PDFTron PDFNet SDK");
		/// }
		/// </code>
		public DictIterator Find(string key) {
            TRN_DictIterator result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjFind(mp_obj, key, ref result));
            return new DictIterator(result, this.m_ref);
        }
		/// <summary> Search the dictionary for a given key.
		/// 
		/// </summary>
		/// <param name="key">- a key to search for in the dictionary
		/// </param>
		/// <returns> NULL if the dictionary does not contain the specified key.
		/// Otherwise return the corresponding value.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  A dictionary entry whose value is Obj.Null is equivalent to an absent entry. </remarks>
		public Obj FindObj(string key) {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjFindObj(mp_obj, key, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Search the dictionary for a given key and throw an exception if the key is not found.
		/// 
		/// </summary>
		/// <param name="key">- a key to search for in the dictionary
		/// </param>
		/// <returns> Obj.Null object if the value matching specified key is a Obj.Null object.
		/// otherwise return the interator to the matching key/value pair.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public DictIterator Get(string key) {
            TRN_DictIterator result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjGet(mp_obj, key, ref result));
            return new DictIterator(result, this.m_ref);
        }
		/// <summary> Inserts a <c>&lt;key, Obj.Type.e_name&gt;</c> pair in the dictionary.
		/// 
		/// </summary>
		/// <param name="key">The key of the value to set.
		/// </param>
		/// <param name="name">The value of the Obj.Type.e_name object to be inserted into
		/// the dictionary.
		/// </param>
		/// <returns> A newly created name object.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  If a dictionary already contains an entry with the same key, the old entry 
		/// will be deleted and all DictIterators to this entry will be invalidated.</remarks>
		public Obj PutName (string key, string name) {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(mp_obj, key, name, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Inserts a <c>&lt;key, Obj.Type.e_array&gt;</c> pair in the dictionary.
		/// 
		/// </summary>
		/// <param name="key">The key of the value to set.
		/// </param>
		/// <returns> A newly created array object.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  If a dictionary already contains an entry with the same key, the old entry 
		/// will be deleted and all DictIterators to this entry will be invalidated.
		/// </remarks>
		public Obj PutArray (string key) {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutArray(mp_obj, key, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Inserts a <c>&lt;key, Obj.Type.e_bool&gt;</c> pair in the dictionary.
		/// 
		/// </summary>
		/// <param name="key">The key of the value to set.
		/// </param>
		/// <param name="value">The value of the Obj.Type.e_bool object to be inserted into
		/// the dictionary.
		/// </param>
		/// <returns> A newly created boolean object.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  If a dictionary already contains an entry with the same key, the old entry 
		/// will be deleted and all DictIterators to this entry will be invalidated.
		/// </remarks>
		public Obj PutBool (string key, bool value) {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(mp_obj, key, value, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Inserts a <c>&lt;key, Obj.Type.e_dict&gt;</c> pair in the dictionary.
		/// 
		/// </summary>
		/// <param name="key">The key of the value to set.
		/// </param>
		/// <returns> A newly created dictionary.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  If a dictionary already contains an entry with the same key, the old entry 
		/// will be deleted and all DictIterators to this entry will be invalidated.
		/// </remarks>
		public Obj PutDict (string key) {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutDict(mp_obj, key, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Inserts a <c>&lt;key, Obj.Type.e_number&gt;</c> pair in the dictionary.
		/// 
		/// </summary>
		/// <param name="key">The key of the value to set.
		/// </param>
		/// <param name="value">The value of the Obj.Type.e_number object to be inserted into
		/// the dictionary.
		/// </param>
		/// <returns> A newly created number object.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  If a dictionary already contains an entry with the same key, the old entry
		/// will be deleted and all DictIterators to this entry will be invalidated.
		/// </remarks>
		public Obj PutNumber (string key, double value) {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutNumber(mp_obj, key, value, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Inserts a <c>&lt;key, Obj.Type.e_string&gt;</c> pair in the dictionary.
		/// 
		/// </summary>
		/// <param name="key">The key of the value to set.
		/// </param>
		/// <param name="value">The value of the Obj.Type.e_string object to be inserted into
		/// the dictionary.
		/// </param>
		/// <returns> A newly created string object.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  If a dictionary already contains an entry with the same key, the old entry 
		/// will be deleted and all DictIterators to this entry will be invalidated.
		/// </remarks>
		public Obj PutString (string key, string value) {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutString(mp_obj, key, value, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Inserts a <c>&lt;key, Obj.Type.e_string&gt;</c> pair in the dictionary.
		/// 
		/// </summary>
		/// <param name="key">The key of the value to set.
		/// </param>
		/// <param name="buf">The buffer used to set the value of the Obj.Type.e_string
		/// object to be inserted into the dictionary.
		/// </param>
		/// <returns> A newly created string object.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  If a dictionary already contains an entry with the same key, the old entry 
		/// will be deleted and all DictIterators to this entry will be invalidated.</remarks>
		public Obj PutString (string key, byte[] buf) {
            TRN_Obj result = IntPtr.Zero;

            int psize = Marshal.SizeOf(buf[0]) * buf.Length;
            IntPtr pnt = Marshal.AllocHGlobal(psize);
            try
            {
                Marshal.Copy(buf, 0, pnt, buf.Length);
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutStringWithSize(mp_obj, key, pnt, buf.Length, ref result));
                return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }
        }
		/// <summary> Inserts a <c>&lt;key, Obj.Type.e_string&gt;</c> pair in the dictionary.
		/// 
		/// </summary>
		/// <param name="key">The key of the value to set.
		/// </param>
		/// <param name="value">The value of the Obj.Type.e_string object to be inserted into
		/// the dictionary.
		/// </param>
		/// <returns> A newly created string object.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  PutText will create the string object as a 'PDF Text' object. 
		/// If a dictionary already contains an entry with the same key, the old entry 
		/// will be deleted and all DictIterators to this entry will be invalidated.</remarks>
		public Obj PutText(string key, string value) {
            TRN_Obj result = IntPtr.Zero;
            UString str = new UString(value);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutText(mp_obj, key, str.mp_impl, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Inserts a <c>&lt;key, Obj.Type.e_null&gt;</c> pair in the dictionary.
		/// 
		/// </summary>
		/// <param name="key">The key of the value to set.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The effect of calling this method is essentially the same as dict.Erase(key) . </remarks>
		public void PutNull (string key) {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutNull(mp_obj, key));
        }
		/// <summary> Inserts a <c>&lt;key, Obj&gt;</c> pair in the dictionary.
		/// 
		/// </summary>
		/// <param name="key">The key of the value to set.
		/// </param>
		/// <param name="obj">The value to be inserted into the dictionary. If 'obj' is
		/// indirect (i.e. is a shared) object it will be inserted by reference,
		/// otherwise the object will be cloned and then inserted into the dictionary.
		/// </param>
		/// <returns> A newly inserted object.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public Obj Put(string key, Obj obj) {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPut(mp_obj, key, obj.mp_obj, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Inserts a <c>&lt;key, [x1,y1,x2,y2]&gt;</c> pair in the dictionary.
		/// 
		/// </summary>
		/// <param name="key">The key of the value to set.
		/// </param>
		/// <param name="x1">the x1
		/// </param>
		/// <param name="y1">the y1
		/// </param>
		/// <param name="x2">the x2
		/// </param>
		/// <param name="y2">the y2
		/// </param>
		/// <returns> A newly created array object.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  If a dictionary already contains an entry with the same key, the old entry
		/// will be deleted and all DictIterators to this entry will be invalidated.
		/// </remarks>
		public Obj PutRect (string key, double x1, double y1, double x2, double y2) {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutRect(mp_obj, key, x1, y1, x2, y2, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Inserts a <c>&lt;key, [a,b,c,d,h,v]&gt;</c> pair in the dictionary.
		/// </summary>
		/// <param name="key">The key of the value to set.
		/// </param>
		/// <param name="value">- A matrix used to set the values in an array of six numbers.
		/// The resulting array will be inserted into the dictionary.
		/// </param>
		/// <returns> A newly created array object.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  If a dictionary already contains an entry with the same key, the old entry
		/// will be deleted and all DictIterators to this entry will be invalidated.
		/// </remarks>
		public Obj PutMatrix(string key, Common.Matrix2D value) {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutMatrix(mp_obj, key, ref value.mp_mtx, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }

		/// <summary> Removes an element in the dictionary that matches the given key.
		/// 
		/// </summary>
		/// <param name="key">the key
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public void Erase(string key) {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjEraseFromKey(mp_obj, key));
        }
		/// <summary> Removes an element in the dictionary from specified position.
		/// 
		/// </summary>
		/// <param name="pos">the pos
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public void Erase(DictIterator pos) {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjErase(mp_obj, pos.mp_impl));
        }
		/// <summary> Change the key value of a dictionary entry.
		/// The key can't be renamed if another key with the same name already exists
		/// in the dictionary. In this case Rename returns false.
		/// 
		/// </summary>
		/// <param name="old_key">the old_key
		/// </param>
		/// <param name="new_key">the new_key
		/// </param>
		/// <returns> true, if successful
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public bool Rename(string old_key, string new_key) {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjRename(mp_obj, old_key, new_key, ref result));
            return result;
        }

		// Array Specific Methods ------------------------------------------------------
		/// <summary> Gets the at.
		/// 
		/// </summary>
		/// <param name="index">- The array element to obtain. The first element in an array has an index of zero.
		/// </param>
		/// <returns> the at
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public Obj GetAt(int index) {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjGetAt(mp_obj, new UIntPtr(System.Convert.ToUInt32(index)), ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Inserts an Obj.Type.e_name object in the array.
		/// 
		/// </summary>
		/// <param name="pos">- The location in the array to insert the object . The object is inserted
		/// before the specified location. The first element in an array has a pos of
		/// zero. If <c>pos &gt;= Array-&gt;Length()</c>, appends obj to array.
		/// </param>
		/// <param name="name">The value of the Obj.Type.e_name object to be inserted.
		/// </param>
		/// <returns> A newly created name object.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public Obj InsertName (int pos, string name) {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjInsertName(mp_obj, new UIntPtr(System.Convert.ToUInt32(pos)), name, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Inserts an <c>Obj.Type.e_array</c> object in the array.
		/// 
		/// </summary>
		/// <param name="pos">- The location in the array to insert the object . The object is inserted
		/// before the specified location. The first element in an array has a pos of
		/// zero. If <c>pos &gt;= Array-&gt;Length()</c>, appends obj to array.
		/// </param>
		/// <returns> A newly created array object.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public Obj InsertArray (int pos) {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjInsertArray(mp_obj, new UIntPtr(System.Convert.ToUInt32(pos)), ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Inserts an <c>Obj.Type.e_bool</c> object in the array.
		/// 
		/// </summary>
		/// <param name="pos">- The location in the array to insert the object . The object is inserted
		/// before the specified location. The first element in an array has a pos of
		/// zero. If <c>pos &gt;= Array-&gt;Length()</c>, appends obj to array.
		/// </param>
		/// <param name="value">The value of the Obj.Type.e_bool object to be inserted.
		/// </param>
		/// <returns> A newly created boolean object.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public Obj InsertBool (int pos, bool value) {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjInsertBool(mp_obj, new UIntPtr(System.Convert.ToUInt32(pos)), value, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Inserts an Obj.Type.e_dict object in the array.
		/// 
		/// </summary>
		/// <param name="pos">- The location in the array to insert the object . The object is inserted
		/// before the specified location. The first element in an array has a pos of
		/// zero. If pos >= Array->Length(), appends obj to array.
		/// </param>
		/// <returns> A newly created dictionary object.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public Obj InsertDict (int pos) {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjInsertDict(mp_obj, new UIntPtr(System.Convert.ToUInt32(pos)), ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Inserts an Obj.Type.e_number object in the array.
		/// 
		/// </summary>
		/// <param name="pos">- The location in the array to insert the object . The object is inserted
		/// before the specified location. The first element in an array has a pos of
		/// zero. If pos >= Array->Length(), appends obj to array.
		/// </param>
		/// <param name="value">The value of the Obj.Type.e_number object to be inserted.
		/// </param>
		/// <returns> A newly created number object.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public Obj InsertNumber (int pos, double value) {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjInsertNumber(mp_obj, new UIntPtr(System.Convert.ToUInt32(pos)), value, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Inserts an Obj.Type.e_string object in the array.
		/// 
		/// </summary>
		/// <param name="pos">- The location in the array to insert the object . The object is inserted
		/// before the specified location. The first element in an array has a pos of
		/// zero. If pos >= Array->Length(), appends obj to array.
		/// </param>
		/// <param name="value">The value of the Obj.Type.e_string object to be inserted.
		/// </param>
		/// <returns> A newly created string object.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public Obj InsertString (int pos, string value) {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjInsertString(mp_obj, new UIntPtr(System.Convert.ToUInt32(pos)), value, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Inserts an Obj.Type.e_string object in the array.
		/// 
		/// </summary>
		/// <param name="pos">- The location in the array to insert the object . The object is inserted
		/// before the specified location. The first element in an array has a pos of
		/// zero. If pos >= Array->Length(), appends obj to array.
		/// </param>
		/// <param name="buf">The buffer used to set the value of the Obj.Type.e_string
		/// object to be inserted.
		/// </param>
		/// <returns> A newly created string object.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public Obj InsertString (int pos, byte[] buf) {
            TRN_Obj result = IntPtr.Zero;

            int psize = Marshal.SizeOf(buf[0]) * buf.Length;
            IntPtr pnt = Marshal.AllocHGlobal(psize);
            try
            {
                Marshal.Copy(buf, 0, pnt, buf.Length);
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjInsertStringWithSize(mp_obj, new UIntPtr(System.Convert.ToUInt32(pos)), pnt, buf.Length, ref result));
                return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }
        }
		/// <summary> Inserts an Obj.Type.e_string object in the array.
		/// 
		/// </summary>
		/// <param name="pos">- The location in the array to insert the object . The object is inserted
		/// before the specified location. The first element in an array has a pos of
		/// zero. If pos >= Array->Length(), appends obj to array.
		/// </param>
		/// <param name="value">The value of the Obj.Type.e_string object to be inserted.
		/// </param>
		/// <returns> A newly created string object.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  InsertText will create the string object as a 'PDF Text' object. </remarks>
		public Obj InsertText(int pos, string value) {
            TRN_Obj result = IntPtr.Zero;
            UString str = new UString(value);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjInsertText(mp_obj, new UIntPtr(System.Convert.ToUInt32(pos)), str.mp_impl, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Inserts an Obj.Type.e_null object in the array.
		/// 
		/// </summary>
		/// <param name="pos">- The location in the array to insert the object . The object is inserted
		/// before the specified location. The first element in an array has a pos of
		/// zero. If pos >= Array->Length(), appends obj to array.
		/// </param>
		/// <returns> A newly created null object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj InsertNull(int pos)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjInsertNull(mp_obj, new UIntPtr(System.Convert.ToUInt32(pos)), ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Inserts an array of 4 numbers in this array.
		/// 
		/// </summary>
		/// <param name="pos">- The location in the array to insert the object . The object is inserted
		/// before the specified location. The first element in an array has a pos of
		/// zero. If pos >= Array->Length(), appends obj to array.
		/// </param>
		/// <param name="x1">the x1
		/// </param>
		/// <param name="y1">the y1
		/// </param>
		/// <param name="x2">the x2
		/// </param>
		/// <param name="y2">the y2
		/// </param>
		/// <returns> A newly created array object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj InsertRect(int pos, double x1, double y1, double x2, double y2)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjInsertRect(mp_obj, new UIntPtr(System.Convert.ToUInt32(pos)), x1, y1, x2, y2, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Inserts an array of 6 numbers in this array.
		/// 
		/// </summary>
		/// <param name="pos">- The location in the array to insert the object . The object is inserted
		/// before the specified location. The first element in an array has a pos of
		/// zero. If pos >= Array->Length(), appends obj to array.
		/// </param>
		/// <param name="value">- A matrix used to set the values in an array of six numbers.
		/// The resulting array will be then inserted in this array.
		/// </param>
		/// <returns> A newly created array object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj InsertMatrix(int pos, Common.Matrix2D value)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjInsertMatrix(mp_obj, new UIntPtr(System.Convert.ToUInt32(pos)), ref value.mp_mtx, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Inserts an existing Obj in this array.
		/// 
		/// </summary>
		/// <param name="pos">- The location in the array to insert the object . The object is inserted
		/// before the specified location. The first element in an array has a pos of
		/// zero. If pos >= Array->Length(), appends obj to array.
		/// </param>
		/// <param name="obj">The value to be inserted into the dictionary. If 'obj' is
		/// indirect (i.e. is a shared) object it will be inserted by reference,
		/// otherwise the object will be cloned and then inserted.
		/// </param>
		/// <returns> A newly inserted object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj Insert(int pos, Obj obj)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjInsert(mp_obj, new UIntPtr(System.Convert.ToUInt32(pos)), obj.mp_obj, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }

		/// <summary> Appends a new Obj.Type.e_name object at the end of the array.
		/// 
		/// </summary>
		/// <param name="name">- The value of the Obj.Type.e_name object.
		/// </param>
		/// <returns> The new array object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj PushBackName(string name)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPushBackName(mp_obj, name, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Appends a new Obj.Type.e_array object at the end of the array.
		/// 
		/// </summary>
		/// <returns> The new array object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj PushBackArray()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPushBackArray(mp_obj, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Appends a new Obj.Type.e_bool object at the end of the array.
		/// 
		/// </summary>
		/// <param name="value">the value
		/// </param>
		/// <returns> The new boolean object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj PushBackBool(bool value)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPushBackBool(mp_obj, value, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Appends a new Obj.Type.e_dict object at the end of the array.
		/// 
		/// </summary>
		/// <returns> The new dictionary object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj PushBackDict()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPushBackDict(mp_obj, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Appends a new Obj.Type.e_number object at the end of the array.
		/// 
		/// </summary>
		/// <param name="value">- The value of the Obj.Type.e_number object.
		/// </param>
		/// <returns> The new number object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj PushBackNumber(double value)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPushBackNumber(mp_obj, value, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Appends a new Obj.Type.e_string object at the end of the array.
		/// 
		/// </summary>
		/// <param name="value">- The value of the Obj.Type.e_string object.
		/// </param>
		/// <returns> The new string object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj PushBackString(string value)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPushBackString(mp_obj, value, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Appends a new Obj.Type.e_string object at the end of the array.
		/// 
		/// </summary>
		/// <param name="buf">The buffer used to set the value of the Obj.Type.e_string
		/// object to be inserted.
		/// </param>
		/// <returns> The new string object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj PushBackString(byte[] buf)
        {
            TRN_Obj result = IntPtr.Zero;

            int psize = Marshal.SizeOf(buf[0]) * buf.Length;
            IntPtr pnt = Marshal.AllocHGlobal(psize);
            try
            {
                Marshal.Copy(buf, 0, pnt, buf.Length);
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPushBackStringWithSize(mp_obj, pnt, buf.Length, ref result));
                return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }
        }
		/// <summary> Appends a new Obj.Type.e_string object at the end of the array.
		/// 
		/// </summary>
		/// <param name="value">The value of the Obj.Type.e_string object to be inserted.
		/// </param>
		/// <returns> The new string object.
		/// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  InsertText will create the string object as a 'PDF Text' object. </remarks>
        public Obj PushBackText(string value)
        {
            TRN_Obj result = IntPtr.Zero;
            UString str = new UString(value);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPushBackText(mp_obj, str.mp_impl, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Appends a new Obj.Type.e_null object at the end of the array.
		/// 
		/// </summary>
		/// <returns> The new null object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj PushBackNull()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPushBackNull(mp_obj, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Appends an existing Obj at the end of the array.
		/// 
		/// </summary>
		/// <param name="obj">The value to be inserted into the dictionary. If 'obj' is
		/// indirect (i.e. is a shared) object it will be inserted by reference,
		/// otherwise the object will be cloned and then appended.
		/// </param>
		/// <returns> A newly appended object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj PushBack(Obj obj)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPushBack(mp_obj, obj.mp_obj, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Appends an array of 4 numbers at the end of the array.
		/// 
		/// </summary>
		/// <param name="x1">the x1
		/// </param>
		/// <param name="y1">the y1
		/// </param>
		/// <param name="x2">the x2
		/// </param>
		/// <param name="y2">the y2
		/// </param>
		/// <returns> A newly appended array object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj PushBackRect(double x1, double y1, double x2, double y2)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPushBackRect(mp_obj, x1, y1, x2, y2, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary> Appends an array of 6 numbers at the end of the array.
		/// 
		/// </summary>
		/// <param name="value">- A matrix used to set the values in an array of six numbers.
		/// The resulting array will be then inserted in this array.
		/// </param>
		/// <returns> A newly appended array object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj PushBackMatrix(Common.Matrix2D value)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPushBackMatrix(mp_obj, ref value.mp_mtx, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }

		/// <summary> Checks whether the position is within the array bounds and then removes it from the
		/// array and moves each subsequent element to the slot with the next smaller index and
		/// decrements the arrays length by 1.
		/// 
		/// </summary>
		/// <param name="pos">the pos
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void EraseAt(int pos)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjEraseAt(mp_obj, new UIntPtr(System.Convert.ToUInt32(pos))));
        }

		// Stream Specific Methods -----------------------------------------------------
		/// <summary> Gets the raw stream length.
		/// 
		/// </summary>
		/// <returns> the length of the raw/encoded stream equal to the Lenght parameter
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public int GetRawStreamLength()
        {
            UIntPtr result = UIntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjGetRawStreamLength(mp_obj, ref result));
            return System.Convert.ToInt32(result.ToUInt32());
        }
		/// <summary> Gets the raw stream.
		/// 
		/// </summary>
		/// <param name="decrypt">- If true decrypt the stream if the stream is encrypted.
		/// </param>
		/// <returns> - A filter to the encoded stream
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Filters.Filter GetRawStream(bool decrypt)
        {
            TRN_Filter result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjGetRawStream(mp_obj, decrypt, ref result));
            return new Filters.Filter(result, null);
        }
		/// <summary> Gets the decoded stream.
		/// 
		/// </summary>
		/// <returns> - A filter to the decoded stream
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Filters.Filter GetDecodedStream()
        {
            TRN_Filter result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjGetDecodedStream(mp_obj, ref result));
            return new Filters.Filter(result, null);
        }

		/// <summary>allows to replace the content stream with a new one without creating a new object
		/// </summary>
        /// <param name="buf">buffer contain new content stream
        /// </param>
        public void SetStreamData(byte[] buf)
        {
            SetStreamData(buf, null);
        }
		/// <summary>allows to replace the content stream with a new one without creating a new object
		/// </summary>
		/// <param name="buf">buffer contain new content stream
		/// </param>
        /// <param name="filter">
        /// </param>
        public void SetStreamData(byte[] buf, Filters.Filter filter)
        {
            if (filter != null)
            {
                filter.setRefHandleInternal(this);
            }

            int psize = Marshal.SizeOf(buf[0]) * buf.Length;
            IntPtr pnt = Marshal.AllocHGlobal(psize);
            try
            {
                Marshal.Copy(buf, 0, pnt, buf.Length);
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjSetStreamDataWithFilter(mp_obj, pnt,System.Convert.ToUInt32(buf.Length), filter != null ? filter.mp_imp : IntPtr.Zero));
                filter.mp_imp = IntPtr.Zero;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }
        }

        // Nested Types
        // Common methods for Obj hierarchy ---------------------------------------------
        ///<summary>Obj types</summary>
        public enum ObjType
        {
            ///<summary>null object</summary>
            e_null,
            ///<summary>bool object</summary>
            e_bool,
            ///<summary>number object</summary>
            e_number,
            ///<summary>name object</summary>
            e_name,
            ///<summary>string object</summary>
            e_string,
            ///<summary>dictionary object</summary>
            e_dict,
            ///<summary>array object</summary>
            e_array,
            ///<summary>stream object</summary>
            e_stream
        }

    }
}
