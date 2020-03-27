using System;
using System.Runtime.InteropServices;

using TRN_Obj = System.IntPtr;
using TRN_ElementBuilder = System.IntPtr;

namespace pdftronprivate.trn
{
    internal class BasicTypes
    {
		[StructLayout(LayoutKind.Sequential)]
        public struct TRN_Matrix2D
        {
            public double m_a;
            public double m_b;
            public double m_c;
            public double m_d;
            public double m_h;
            public double m_v;
        }

		[StructLayout(LayoutKind.Sequential)]
        public struct TRN_Rect
        {
            public double x1;
            public double y1;
            public double x2;
            public double y2;
            public TRN_Obj mp_rect;
        }

		[StructLayout(LayoutKind.Sequential)]
        public struct TRN_Date
        {
            public ushort year;
            public byte month;
            public byte day;
            public byte hour;
            public byte minute;
            public byte second;
            public byte UT;
            public byte UT_hour;
            public byte UT_minutes;
            public TRN_Obj mp_obj;
        }
		[StructLayout(LayoutKind.Sequential)]
        public struct TRN_CharData
        {
            public uint char_code;
            public double x;
            public double y;
            public IntPtr char_data;
            public int bytes;
        }
		[StructLayout(LayoutKind.Sequential)]
        public struct TRN_Point
        {
            public double x;
            public double y;

            public TRN_Point(double px, double py)
            {
                x = px;
                y = py;
            }
        }
		[StructLayout(LayoutKind.Sequential)]
        public struct TRN_QuadPoint
        {
            public TRN_Point p1;
            public TRN_Point p2;
            public TRN_Point p3;
            public TRN_Point p4;

            public TRN_QuadPoint(TRN_Point pt1, TRN_Point pt2, TRN_Point pt3, TRN_Point pt4)
            {
                p1 = pt1;
                p2 = pt2;
                p3 = pt3;
                p4 = pt4;
            }
        }
		[StructLayout(LayoutKind.Sequential)]
        public struct TRN_Field
        {
            public TRN_Obj leaf_node;
            public TRN_ElementBuilder builder;
        }
		[StructLayout(LayoutKind.Sequential)]
        public struct TRN_PageLabel
        {
            public TRN_Obj mp_obj;
            public int m_first_page;
            public int m_last_page;
        }
		[StructLayout(LayoutKind.Sequential)]
        public struct TRN_FDFField
        {
            public TRN_Obj mp_leaf_node;
            public TRN_Obj mp_root_array;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct TRN_String
        {
            public IntPtr str;
            public uint len;
        }
		[StructLayout(LayoutKind.Sequential)]
        public struct TRN_ColorPt
        {
            public IntPtr c;
            public IntPtr _c;
        }
		[StructLayout(LayoutKind.Sequential)]
        public struct TRN_SElement
        {
            public TRN_Obj obj;
            public TRN_Obj k;
        }
		[StructLayout(LayoutKind.Sequential)]
        public struct TRN_ContentItem
        {
            public TRN_Obj o;
            public TRN_Obj p;
        }
		[StructLayout(LayoutKind.Sequential)]
        public struct TRN_TextExtractorStyle
        {
            public IntPtr mp_imp;
        }
		[StructLayout(LayoutKind.Sequential)]
        public struct TRN_TextExtractorWord
        {
            public IntPtr line, word, end;
            public IntPtr uni;
            public int num, cur_num;
            public IntPtr mp_bld;
        }
		[StructLayout(LayoutKind.Sequential)]
        public struct TRN_TextExtractorLine
        {
            public IntPtr line;
            public IntPtr uni;
            public int num, cur_num;
            public double m_direction;
            public IntPtr mp_bld;
        }
		[StructLayout(LayoutKind.Sequential)]
        public struct TRN_SignatureData
        {
            public IntPtr data;
            public UIntPtr length;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct TRN_DigitalSignatureField
        {
            public TRN_Obj mp_field_dict_obj;
        }
    }
}
