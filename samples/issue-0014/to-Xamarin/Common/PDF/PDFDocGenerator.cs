using System;
using System.Text;

using pdftronprivate.trn;
using pdftron.Common;


using TRN_PDFDoc = System.IntPtr;

namespace pdftron.PDF
{
	/// <summary>
	/// A collection of static methods to create blank documents
	/// </summary>
	/// <remark>
	/// No notes :(
	/// </remark>
	public static class PDFDocGenerator
	{


		/// <summary>
        /// Create a new document with one page of blank paper.
        /// </summary>
        /// <param name="width">The page width in inches</param>
        /// <param name="height">The page height in inches</param>
        /// <param name="background_red">The red component of the background color</param>
        /// <param name="background_green">The green component of the background color</param>
        /// <param name="background_blue">The blue component of the background color</param>
        /// <returns></returns>
		public static PDFDoc GenerateBlankPaperDoc(double width, double height, double background_red, double background_green, double background_blue)
		{
			TRN_PDFDoc result = IntPtr.Zero;
			PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGeneratorGenerateBlankPaperDoc(width, height, background_red, background_green, background_blue, ref result));
			return new PDFDoc(result);
		}

		/// <summary>
        /// Create a new document with one page of grid paper.
        /// </summary>
        /// <param name="width">The page width in inches</param>
        /// <param name="height">The page height in inches</param>
        /// <param name="grid_spacing">The grid spacing in inches</param>
        /// <param name="line_thickness">The line thickness in points</param>
        /// <param name="red">The red component of the line color</param>
        /// <param name="green">The green component of the line color</param>
        /// <param name="blue">The blue component of the line color</param>
        /// <param name="background_red">The red component of the background color</param>
        /// <param name="background_green">The green component of the background color</param>
        /// <param name="background_blue">The blue component of the background color</param>
        /// <returns></returns>
		public static PDFDoc GenerateGridPaperDoc(double width, double height, double grid_spacing, double line_thickness, double red, double green, double blue, double background_red, double background_green, double background_blue)
		{
			TRN_PDFDoc result = IntPtr.Zero;
			PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGeneratorGenerateGridPaperDoc(width, height, grid_spacing, line_thickness, red, green, blue, background_red, background_green, background_blue, ref result));
			return new PDFDoc(result);
		}

		/// <summary>
        /// Create a new document with one page of lined paper.
        /// </summary>
        /// <param name="width">The page width in inches</param>
        /// <param name="height">The page height in inches</param>
        /// <param name="line_spacing">The line spacing in inches</param>
        /// <param name="line_thickness">The line thickness in points</param>
        /// <param name="red">The red component of the line color</param>
        /// <param name="green">The green component of the line color</param>
        /// <param name="blue">The blue component of the line color</param>
        /// <param name="left_margin_distance">Distance of the margin from the left side of the page</param>
        /// <param name="left_margin_red">The red component of the left margin color</param>
        /// <param name="left_margin_green">The green component of the left margin color</param>
        /// <param name="left_margin_blue">The blue component of the left margin color</param>
        /// <param name="right_margin_red">The red component of the right margin color</param>
        /// <param name="right_margin_green">The green component of the right margin color</param>
        /// <param name="right_margin_blue">The blue component of the right margin color</param>
        /// <param name="background_red">The red component of the background color</param>
        /// <param name="background_green">The green component of the background color</param>
        /// <param name="background_blue">The blue component of the background color</param>
        /// <param name="top_margin_distance">Distance of the margin from the top of the page</param>
        /// <param name="bottom_margin_distance">Distance of the margin from the bottom of the page</param>
        /// <returns></returns>
		public static PDFDoc GenerateLinedPaperDoc(double width, double height, double line_spacing, double line_thickness, double red, double green, double blue, double left_margin_distance, double left_margin_red, double left_margin_green, double left_margin_blue, double right_margin_red, double right_margin_green, double right_margin_blue, double background_red, double background_green, double background_blue, double top_margin_distance, double bottom_margin_distance)
		{
			TRN_PDFDoc result = IntPtr.Zero;
			PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGeneratorGenerateLinedPaperDoc(width, height, line_spacing, line_thickness, red, green, blue, left_margin_distance, left_margin_red, left_margin_green, left_margin_blue, right_margin_red, right_margin_green, right_margin_blue, background_red, background_green, background_blue, top_margin_distance, bottom_margin_distance, ref result));
			return new PDFDoc(result);
		}

		/// <summary>
        /// Create a new document with one page of graph paper.
        /// </summary>
        /// <param name="width">The page width in inches</param>
        /// <param name="height">The page height in inches</param>
        /// <param name="grid_spacing">The grid spacing in inches</param>
        /// <param name="line_thickness">The line thickness in points</param>
        /// <param name="weighted_line_thickness">The weighted line thickness in points</param>
        /// <param name="weighted_line_freq">Ratio of weighted lines to normal lines</param>
        /// <param name="red">The red component of the line color</param>
        /// <param name="green">The green component of the line color</param>
        /// <param name="blue">The blue component of the line color</param>
        /// <param name="background_red">The red component of the background color</param>
        /// <param name="background_green">The green component of the background color</param>
        /// <param name="background_blue">The blue component of the background color</param>
        /// <returns></returns>
		public static PDFDoc GenerateGraphPaperDoc(double width, double height, double grid_spacing, double line_thickness, double weighted_line_thickness, int weighted_line_freq, double red, double green, double blue, double background_red, double background_green, double background_blue)
		{
			TRN_PDFDoc result = IntPtr.Zero;
			PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGeneratorGenerateGraphPaperDoc(width, height, grid_spacing, line_thickness, weighted_line_thickness, weighted_line_freq, red, green, blue, background_red, background_green, background_blue, ref result));
			return new PDFDoc(result);
		}

		/// <summary>
        /// Create a new document with one page of music paper.
        /// </summary>
        /// <param name="width">The page width in inches</param>
        /// <param name="height">The page height in inches</param>
        /// <param name="margin">The page margin in inches</param>
        /// <param name="staves">Amount of staves on the page.</param>
        /// <param name="linespace_size_pts">The space between lines in points</param>
        /// <param name="line_thickness">The line thickness in points</param>
        /// <param name="red">The red component of the line color</param>
        /// <param name="green">The green component of the line color</param>
        /// <param name="blue">The blue component of the line color</param>
        /// <param name="background_red">The red component of the background color</param>
        /// <param name="background_green">The green component of the background color</param>
        /// <param name="background_blue">The blue component of the background color</param>
        /// <returns></returns>
		public static PDFDoc GenerateMusicPaperDoc(double width, double height, double margin, int staves, double linespace_size_pts, double line_thickness, double red, double green, double blue, double background_red, double background_green, double background_blue)
		{
			TRN_PDFDoc result = IntPtr.Zero;
			PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGeneratorGenerateMusicPaperDoc(width, height, margin, staves, linespace_size_pts, line_thickness, red, green, blue, background_red, background_green, background_blue, ref result));
			return new PDFDoc(result);
		}
	}
}
