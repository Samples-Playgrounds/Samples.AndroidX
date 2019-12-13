// Generated code. Do not modify!
//
// Copyright (c) 2001-2019 by PDFTron Systems Inc. All Rights Reserved.
//

//----------------------------------------------------------------------------------------------------------------------
// This sample demonstrates the basic usage of high-level digital signature API in PDFNet.
//
// The following steps reflect typical intended usage of the digital signature API:
//
//	0.	Start with a PDF with or without form fields in it that one would like to lock (or, one can add a field, see (1)).
//	
//	1.	EITHER: 
//		(a) Call doc.CreateDigitalSignatureField, optionally providing a name. You receive a DigitalSignatureField.
//		-OR-
//		(b) If you didn't just create the digital signature field that you want to sign/certify, find the existing one within the 
//		document by using PDFDoc.DigitalSignatureFieldIterator or by using PDFDoc.GetField to get it by its fully qualified name.
//	
//	2.	Create a signature widget annotation, and pass the DigitalSignatureField that you just created or found. 
//		If you want it to be visible, provide a Rect argument with a non-zero width or height, and don't set the
//		NoView and Hidden flags. [Optionally, add an appearance to the annotation when you wish to sign/certify.]
//		
//	[3. (OPTIONAL) Add digital signature restrictions to the document using the field modification permissions (SetFieldPermissions) 
//		or document modification permissions functions (SetDocumentPermissions) of DigitalSignatureField. These features disallow 
//		certain types of changes to be made to the document without invalidating the cryptographic digital signature's hash once it
//		is signed.]
//		
//	4. 	Call either CertifyOnNextSave or SignOnNextSave. There are three overloads for each one (six total):
//		a.	Taking a PKCS #12 keyfile path and its password
//		b.	Taking a buffer containing a PKCS #12 private keyfile and its password
//		c.	Taking a unique identifier of a signature handler registered with the PDFDoc. This overload is to be used
//			in the following fashion: 
//			i)		Extend and implement a new SignatureHandler. The SignatureHandler will be used to add or 
//					validate/check a digital signature.
//			ii)		Create an instance of the implemented SignatureHandler and register it with PDFDoc with 
//					pdfdoc.AddSignatureHandler(). The method returns a SignatureHandlerId.
//			iii)	Call SignOnNextSaveWithCustomHandler/CertifyOnNextSaveWithCustomHandler with the SignatureHandlerId.
//		NOTE: It is only possible to sign/certify one signature per call to the Save function.
//	
//	5.	Call pdfdoc.Save(). This will also create the digital signature dictionary and write a cryptographic hash to it.
//		IMPORTANT: If there are already signed/certified digital signature(s) in the document, you must save incrementally
//		so as to not invalidate the other signature's('s) cryptographic hashes. 
//
// Additional processing can be done before document is signed. For example, UseSignatureHandler() returns an instance
// of SDF dictionary which represents the signature dictionary (or the /V entry of the form field). This can be used to
// add additional information to the signature dictionary (e.g. Name, Reason, Location, etc.).
//
// Although the steps above describes extending the SignatureHandler class, this sample demonstrates the use of
// StdSignatureHandler (a built-in SignatureHandler in PDFNet) to sign a PDF file.
//----------------------------------------------------------------------------------------------------------------------

// In order to use .NET Framework's Cryptography library, define "USE_DOTNET_CRYPTO" and then add System.Security to
// references list.

using System;
using System.Collections.Generic;
using System.IO;

using pdftron;
using pdftron.Common;
using pdftron.PDF;
using pdftron.PDF.Annots;
using pdftron.SDF;

using NUnit.Framework;

namespace MiscellaneousSamples
{
	//////////////////// Here follows an example of how to implement a custom signature handler. //////////
	////////// End of the DotNetCryptoSignatureHandler custom handler code. ////////////////////

	[TestFixture]
	public class DigitalSignaturesTest
	{
		const string input_path =  "TestFiles/";
		
		static void CertifyPDF(string in_docpath,
			string in_cert_field_name,
			string in_private_key_file_path,
			string in_keyfile_password,
			string in_appearance_image_path,
			string in_outpath)
		{
			Console.Out.WriteLine("================================================================================");
			Console.Out.WriteLine("Certifying PDF document");

			// Open an existing PDF
			using (PDFDoc doc = new PDFDoc(in_docpath))
			{
				Console.Out.WriteLine("PDFDoc has " + (doc.HasSignatures() ? "signatures" : "no signatures"));

				Page page1 = doc.GetPage(1);

				// Create a random text field that we can lock using the field permissions feature.
				TextWidget annot1 = TextWidget.Create(doc, new Rect(50, 550, 350, 600), "asdf_test_field");
				page1.AnnotPushBack(annot1);

				/* Create new signature form field in the PDFDoc. The name argument is optional;
				leaving it empty causes it to be auto-generated. However, you may need the name for later.
				Acrobat doesn't show digsigfield in side panel if it's without a widget. Using a
				Rect with 0 width and 0 height, or setting the NoPrint/Invisible flags makes it invisible. */
				DigitalSignatureField certification_sig_field = doc.CreateDigitalSignatureField(in_cert_field_name);
				SignatureWidget widgetAnnot = SignatureWidget.Create(doc, new Rect(0, 100, 200, 150), certification_sig_field);
				page1.AnnotPushBack(widgetAnnot);

				// (OPTIONAL) Add an appearance.

				// Widget AP from image
				Image img = Image.Create(doc, in_appearance_image_path);
				widgetAnnot.CreateSignatureAppearance(img);
				// End of optional appearance-adding code.

				// Add permissions. Lock the random text field.
				Console.Out.WriteLine("Adding document permissions.");
				certification_sig_field.SetDocumentPermissions(DigitalSignatureField.DocumentPermissions.e_annotating_formfilling_signing_allowed);
				Console.Out.WriteLine("Adding field permissions.");
				string[] fields_to_lock = new string[1];
				fields_to_lock[0] = "asdf_test_field";
				certification_sig_field.SetFieldPermissions(DigitalSignatureField.FieldPermissions.e_include, fields_to_lock);

				certification_sig_field.CertifyOnNextSave(in_private_key_file_path, in_keyfile_password);

				///// (OPTIONAL) Add more information to the signature dictionary.
				certification_sig_field.SetLocation("Vancouver, BC");
				certification_sig_field.SetReason("Document certification.");
				certification_sig_field.SetContactInfo("www.pdftron.com");
				///// End of optional sig info code.

				// Save the PDFDoc. Once the method below is called, PDFNetC will also sign the document using the information provided.
				doc.Save(in_outpath, 0);
			}

			Console.Out.WriteLine("================================================================================");
		}

        static void SignPDF(string in_docpath,
			string in_approval_field_name,
			string in_private_key_file_path,
			string in_keyfile_password,
			string in_appearance_img_path,
			string in_outpath)
		{
			Console.Out.WriteLine("================================================================================");
			Console.Out.WriteLine("Signing PDF document");

			// Open an existing PDF
			using (PDFDoc doc = new PDFDoc(in_docpath))
			{
				// Sign the approval signature.
				Field found_approval_field = doc.GetField(in_approval_field_name);
				DigitalSignatureField found_approval_signature_digsig_field = new DigitalSignatureField(found_approval_field);
				Image img2 = Image.Create(doc, in_appearance_img_path);
				SignatureWidget found_approval_signature_widget = new SignatureWidget(found_approval_field.GetSDFObj());
				found_approval_signature_widget.CreateSignatureAppearance(img2);

				found_approval_signature_digsig_field.SignOnNextSave(in_private_key_file_path, in_keyfile_password);

				doc.Save(in_outpath, SDFDoc.SaveOptions.e_incremental);
			}
			Console.Out.WriteLine("================================================================================");
		}

        static void ClearSignature(string in_docpath,
			string in_digsig_field_name,
			string in_outpath)
		{
			Console.Out.WriteLine("================================================================================");
			Console.Out.WriteLine("Clearing certification signature");

			using (PDFDoc doc = new PDFDoc(in_docpath))
			{
				DigitalSignatureField digsig = new DigitalSignatureField(doc.GetField(in_digsig_field_name));
				
				Console.Out.WriteLine("Clearing signature: " + in_digsig_field_name);
				digsig.ClearSignature();

				if (!digsig.HasCryptographicSignature())
				{
					Console.Out.WriteLine("Cryptographic signature cleared properly.");
				}

				// Save incrementally so as to not invalidate other signatures' hashes from previous saves.
				doc.Save(in_outpath, SDFDoc.SaveOptions.e_incremental);
			}

			Console.Out.WriteLine("================================================================================");
		}

        static void PrintSignaturesInfo(string in_docpath)
		{
			Console.Out.WriteLine("================================================================================");
			Console.Out.WriteLine("Reading and printing digital signature information");

			using (PDFDoc doc = new PDFDoc(in_docpath))
			{
				if (!doc.HasSignatures())
				{
					Console.Out.WriteLine("Doc has no signatures.");
					Console.Out.WriteLine("================================================================================");
					return;
				}
				else
				{
					Console.Out.WriteLine("Doc has signatures.");
				}

				
				for (FieldIterator fitr = doc.GetFieldIterator(); fitr.HasNext(); fitr.Next())
				{
                    if (fitr.Current().IsLockedByDigitalSignature())
                    {
                        Console.Out.WriteLine("==========\nField locked by a digital signature");
                    }
                    else
                    {
                        Console.Out.WriteLine("==========\nField not locked by a digital signature");
                    }

					Console.Out.WriteLine("Field name: " + fitr.Current().GetName());
					Console.Out.WriteLine("==========");
				}

				Console.Out.WriteLine("====================\nNow iterating over digital signatures only.\n====================");

				DigitalSignatureFieldIterator digsig_fitr = doc.GetDigitalSignatureFieldIterator();
				for (; digsig_fitr.HasNext(); digsig_fitr.Next())
				{
					Console.Out.WriteLine("==========");
					Console.Out.WriteLine("Field name of digital signature: " + new Field(digsig_fitr.Current().GetSDFObj()).GetName());

					DigitalSignatureField digsigfield = digsig_fitr.Current();
					if (!digsigfield.HasCryptographicSignature())
					{
						Console.Out.WriteLine("Either digital signature field lacks a digital signature dictionary, " +
							"or digital signature dictionary lacks a cryptographic hash entry. " +
							"Digital signature field is not presently considered signed.\n" +
							"==========");
						continue;
					}

					int cert_count = digsigfield.GetCertCount();
					Console.Out.WriteLine("Cert count: " + cert_count);
					for (int i = 0; i < cert_count; ++i)
					{
						byte[] cert = digsigfield.GetCert(i);
						Console.Out.WriteLine("Cert #" + i + " size: " + cert.Length);
					}

					DigitalSignatureField.SubFilterType subfilter = digsigfield.GetSubFilter();

					Console.Out.WriteLine("Subfilter type: " + (int)subfilter);

					if (subfilter != DigitalSignatureField.SubFilterType.e_ETSI_RFC3161)
					{
						Console.Out.WriteLine("Signature's signer: " + digsigfield.GetSignatureName());

						Date signing_time = digsigfield.GetSigningTime();
						if (signing_time.IsValid())
						{
							Console.Out.WriteLine("Signing day: " + (int)signing_time.day);
						}

						Console.Out.WriteLine("Location: " + digsigfield.GetLocation());
						Console.Out.WriteLine("Reason: " + digsigfield.GetReason());
						Console.Out.WriteLine("Contact info: " + digsigfield.GetContactInfo());
					}
					else
					{
						Console.Out.WriteLine("SubFilter == e_ETSI_RFC3161 (DocTimeStamp; no signing info)\n");
					}

					Console.Out.WriteLine(((digsigfield.HasVisibleAppearance()) ? "Visible" : "Not visible"));

					DigitalSignatureField.DocumentPermissions digsig_doc_perms = digsigfield.GetDocumentPermissions();
					string[] locked_fields = digsigfield.GetLockedFields();
					foreach (string field_name in locked_fields)
					{
						Console.Out.WriteLine("This digital signature locks a field named: " + field_name);
					}

					switch (digsig_doc_perms)
					{
					case DigitalSignatureField.DocumentPermissions.e_no_changes_allowed:
						Console.Out.WriteLine("No changes to the document can be made without invalidating this digital signature.");
						break;
					case DigitalSignatureField.DocumentPermissions.e_formfilling_signing_allowed:
						Console.Out.WriteLine("Page template instantiation, form filling, and signing digital signatures are allowed without invalidating this digital signature.");
						break;
					case DigitalSignatureField.DocumentPermissions.e_annotating_formfilling_signing_allowed:
						Console.Out.WriteLine("Annotating, page template instantiation, form filling, and signing digital signatures are allowed without invalidating this digital signature.");
						break;
					case DigitalSignatureField.DocumentPermissions.e_unrestricted:
						Console.Out.WriteLine("Document not restricted by this digital signature.");
						break;
					default:
						throw new Exception("Unrecognized digital signature document permission level.");
					}
					Console.Out.WriteLine("==========");
				}
			}

			Console.Out.WriteLine("================================================================================");
		}
		

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[Test]
		public static void Sample()
		{
			// Initialize PDFNetC

			bool result = true;

			//////////////////// TEST 0: 
			/* Create an approval signature field that we can sign after certifying.
			(Must be done before calling CertifyOnNextSave/SignOnNextSave/WithCustomHandler.) */
			try
			{
				using (PDFDoc doc = new PDFDoc(Utils.GetAssetTempFile(input_path + "tiger.pdf"))) 
				{
					DigitalSignatureField approval_signature_field = doc.CreateDigitalSignatureField("PDFTronApprovalSig");
					SignatureWidget widgetAnnotApproval = SignatureWidget.Create(doc, new Rect(300, 300, 500, 200), approval_signature_field);
					Page page1 = doc.GetPage(1);
					page1.AnnotPushBack(widgetAnnotApproval);
					doc.Save(Utils.CreateExternalFile("tiger_withApprovalField_output.pdf"), SDFDoc.SaveOptions.e_remove_unused);
				}				
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(e);
				Assert.True(false);
				result = false;
			}

			//////////////////// TEST 1: certify a PDF.
			try
			{
				CertifyPDF(Utils.GetAssetTempFile(input_path + "tiger_withApprovalField.pdf"),
					"PDFTronCertificationSig",
					Utils.GetAssetTempFile(input_path + "pdftron.pfx"),
					"password",
					Utils.GetAssetTempFile(input_path + "pdftron.bmp"),
					Utils.CreateExternalFile("tiger_withApprovalField_certified_output.pdf"));
				PrintSignaturesInfo(Utils.CreateExternalFile("tiger_withApprovalField_certified_output.pdf"));
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(e);
				Assert.True(false);
				result = false;
			}

			//////////////////// TEST 2: sign a PDF with a certification and an unsigned signature field in it.
			try
			{
				SignPDF(Utils.GetAssetTempFile(input_path + "tiger_withApprovalField_certified.pdf"),
					"PDFTronApprovalSig",
					Utils.GetAssetTempFile(input_path + "pdftron.pfx"),
					"password",
					Utils.GetAssetTempFile(input_path + "signature.jpg"),
					Utils.CreateExternalFile("tiger_withApprovalField_certified_approved_output.pdf"));
				PrintSignaturesInfo(Utils.CreateExternalFile("tiger_withApprovalField_certified_approved_output.pdf"));
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(e);
				Assert.True(false);
				result = false;
			}

			//////////////////// TEST 3: Clear a certification from a document that is certified and has two approval signatures.
			try
			{
				ClearSignature(Utils.GetAssetTempFile(input_path + "tiger_withApprovalField_certified_approved.pdf"),
					"PDFTronCertificationSig",
					Utils.CreateExternalFile("tiger_withApprovalField_certified_approved_certcleared_output.pdf"));
				PrintSignaturesInfo(Utils.CreateExternalFile("tiger_withApprovalField_certified_approved_certcleared_output.pdf"));
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(e);
				Assert.True(false);
				result = false;
			}

			//////////////////// End of tests. ////////////////////

			if (result)
			{
				Console.Out.WriteLine("Tests successful.\n==========");
			}
			else
			{
				Console.Out.WriteLine("Tests FAILED!!!\n==========");
			}
		}
	}
}
