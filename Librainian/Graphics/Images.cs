// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries,
// repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper licenses and/or copyrights.
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Images.cs" last formatted on 2022-02-06 at 6:52 AM by Protiguous.

namespace Librainian.Graphics;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text;

public static class Images {

	internal static class FileNameExtension {

		/// <summary>
		///     <para>.tif</para>
		/// </summary>
		/// <see cref="http://wikipedia.org/wiki/TIFF" />
		public static String Tiff => ".tif";
	}

	internal static class PropertyList {

		public const Int32 DateTimeDigitized = 0x9004;

		public const Int32 DateTimeOriginal = 0x9003;

		public const Int32 PropertyTagDateTime = 0x132;
	}

	/// <summary>
	///     <para>Exif 2.2 Standard (reference: <see cref="http://www.exiv2.org/tags.html" />)</para>
	/// </summary>
	/// Pulled from project "ExifOrganizer" @
	/// <see cref="http://github.com/RiJo/ExifOrganizer/blob/master/MetaParser/Parsers/ExifParser.cs" />
	public enum ExifId {

		/// <summary>
		/// The name and version of the software used to post-process the picture.
		/// </summary>
		ImageProcessingSoftware = 0x000b,

		/// <summary>
		/// A general indication of the kind of data contained in this subfile.
		/// </summary>
		ImageNewSubfileType = 0x00fe,

		/// <summary>
		/// A general indication of the kind of data contained in this subfile. This field is deprecated. The NewSubfileType field should be used instead.
		/// </summary>
		ImageSubfileType =
			0x00ff,

		/// <summary>
		/// The number of columns of image data, equal to the number of pixels per row. In JPEG compressed data a JPEG marker is used instead of this tag.
		/// </summary>
		ImageImageWidth =
			0x0100,

		/// <summary>
		/// The number of rows of image data. In JPEG compressed data a JPEG marker is used instead of this tag.
		/// </summary>
		ImageImageLength = 0x0101,

		/// <summary>
		/// The number of bits per image component. In this standard each component of the image is 8 bits, so the value for this tag is 8. See also <SamplesPerPixel>. In JPEG compressed data a JPEG marker is used instead of this tag.
		/// </summary>
		ImageBitsPerSample =
			0x0102,

		/// <summary>
		/// The compression scheme used for the image data. When a primary image is JPEG compressed, this designation is not necessary and is omitted. When thumbnails use JPEG compression, this tag value is set to 6.
		/// </summary>
		ImageCompression =
			0x0103,

		/// <summary>
		/// The pixel composition. In JPEG compressed data a JPEG marker is used instead of this tag.
		/// </summary>
		ImagePhotometricInterpretation = 0x0106,

		/// <summary>
		/// For black and white TIFF files that represent shades of gray, the technique used to convert from gray to black and white pixels.
		/// </summary>
		ImageThreshholding = 0x0107,

		/// <summary>
		/// The width of the dithering or halftoning matrix used to create a dithered or halftoned bilevel file.
		/// </summary>
		ImageCellWidth = 0x0108,

		/// <summary>
		/// The length of the dithering or halftoning matrix used to create a dithered or halftoned bilevel file.
		/// </summary>
		ImageCellLength = 0x0109,

		/// <summary>
		/// The logical order of bits within a byte
		/// </summary>
		ImageFillOrder = 0x010a,

		/// <summary>
		/// The name of the document from which this image was scanned
		/// </summary>
		ImageDocumentName = 0x010d,

		/// <summary>
		/// A character string giving the title of the Image It may be a comment such as "1988 company picnic" or the like. Two-bytes character codes cannot be used. When a 2-bytes code is necessary, the Exif Private tag <UserComment> is to be used.
		/// </summary>
		ImageImageDescription =
			0x010e,

		/// <summary>
		/// The manufacturer of the recording equipment. This is the manufacturer of the DSC, scanner, video digitizer or other equipment that generated the Image When the field is left blank, it is treated as unknown.
		/// </summary>
		ImageMake =
			0x010f,

		/// <summary>
		/// The model name or model number of the equipment. This is the model name or number of the DSC, scanner, video digitizer or other equipment that generated the Image When the field is left blank, it is treated as unknown.
		/// </summary>
		ImageModel =
			0x0110,

		/// <summary>
		/// For each strip, the byte offset of that strip. It is recommended that this be selected so the number of strip bytes does not exceed 64 Kbytes. With JPEG compressed data this designation is not needed and is omitted. See also <RowsPerStrip> and <StripByteCounts>.
		/// </summary>
		ImageStripOffsets =
			0x0111,

		/// <summary>
		/// The image orientation viewed in terms of rows and columns.
		/// </summary>
		ImageOrientation = 0x0112,

		/// <summary>
		/// The number of components per pixel. Since this standard applies to RGB and YCbCr images, the value set for this tag is 3. In JPEG compressed data a JPEG marker is used instead of this tag.
		/// </summary>
		ImageSamplesPerPixel =
			0x0115,

		/// <summary>
		/// The number of rows per strip. This is the number of rows in the image of one strip when an image is divided into strips. With JPEG compressed data this designation is not needed and is omitted. See also <StripOffsets> and <StripByteCounts>.
		/// </summary>
		ImageRowsPerStrip =
			0x0116,

		/// <summary>
		/// The total number of bytes in each strip. With JPEG compressed data this designation is not needed and is omitted.
		/// </summary>
		ImageStripByteCounts = 0x0117,

		/// <summary>
		/// The number of pixels per <ResolutionUnit> in the <ImageWidth> direction. When the image resolution is unknown, 72 [dpi] is designated.
		/// </summary>
		ImageXResolution = 0x011a,

		/// <summary>
		/// The number of pixels per <ResolutionUnit> in the <ImageLength> direction. The same value as <XResolution> is designated.
		/// </summary>
		ImageYResolution = 0x011b,

		/// <summary>
		/// Indicates whether pixel components are recorded in a chunky or planar format. In JPEG compressed files a JPEG marker is used instead of this tag. If this field does not exist, the TIFF default of 1 (chunky) is assumed.
		/// </summary>
		ImagePlanarConfiguration =
			0x011c,

		/// <summary>
		/// The precision of the information contained in the GrayResponseCurve.
		/// </summary>
		ImageGrayResponseUnit = 0x0122,

		/// <summary>
		/// For grayscale data, the optical density of each possible pixel value.
		/// </summary>
		ImageGrayResponseCurve = 0x0123,

		/// <summary>
		/// T.4-encoding options.
		/// </summary>
		ImageT4Options = 0x0124,

		/// <summary>
		/// T.6-encoding options.
		/// </summary>
		ImageT6Options = 0x0125,

		/// <summary>
		/// The unit for measuring <XResolution> and <YResolution>. The same unit is used for both <XResolution> and <YResolution>. If the image resolution is unknown, 2 (inches) is designated.
		/// </summary>
		ImageResolutionUnit =
			0x0128,

		/// <summary>
		/// A transfer function for the image, described in tabular style. Normally this tag is not necessary, since color space is specified in the color space information tag (<ColorSpace>).
		/// </summary>
		ImageTransferFunction =
			0x012d,

		/// <summary>
		/// This tag records the name and version of the software or firmware of the camera or image input device used to generate the Image The detailed format is not specified, but it is recommended that the example shown below be followed. When the field is left blank, it is treated as unknown.
		/// </summary>
		ImageSoftware =
			0x0131,

		/// <summary>
		/// The date and time of image creation. In Exif standard, it is the date and time the file was changed.
		/// </summary>
		ImageDateTime = 0x0132,

		/// <summary>
		/// This tag records the name of the camera owner, photographer or image creator. The detailed format is not specified, but it is recommended that the information be written as in the example below for ease of Interoperability. When the field is left blank, it is treated as unknown. Ex.) "Camera owner, John Smith; Photographer, Michael Brown; Image creator, Ken James"
		/// </summary>
		ImageArtist =
			0x013b,

		/// <summary>
		/// This tag records information about the host computer used to generate the Image
		/// </summary>
		ImageHostComputer = 0x013c,

		/// <summary>
		/// A predictor is a mathematical operator that is applied to the image data before an encoding scheme is applied.
		/// </summary>
		ImagePredictor = 0x013d,

		/// <summary>
		/// The chromaticity of the white point of the Image Normally this tag is not necessary, since color space is specified in the colorspace information tag (<ColorSpace>).
		/// </summary>
		ImageWhitePoint =
			0x013e,

		/// <summary>
		/// The chromaticity of the three primary colors of the Image Normally this tag is not necessary, since colorspace is specified in the colorspace information tag (<ColorSpace>).
		/// </summary>
		ImagePrimaryChromaticities =
			0x013f,

		/// <summary>
		/// A color map for palette color images. This field defines a Red-Green-Blue color map (often called a lookup table) for palette-color images. In a palette-color image, a pixel value is used to index into an RGB lookup table.
		/// </summary>
		ImageColorMap =
			0x0140,

		/// <summary>
		/// The purpose of the HalftoneHints field is to convey to the halftone function the range of gray levels within a colorimetrically-specified image that should retain tonal detail.
		/// </summary>
		ImageHalftoneHints =
			0x0141,

		/// <summary>
		/// The tile width in pixels. This is the number of columns in each tile.
		/// </summary>
		ImageTileWidth = 0x0142,

		/// <summary>
		/// The tile length (height) in pixels. This is the number of rows in each tile.
		/// </summary>
		ImageTileLength = 0x0143,

		/// <summary>
		/// For each tile, the byte offset of that tile, as compressed and stored on disk. The offset is specified with respect to the beginning of the TIFF file. Note that this implies that each tile has a location independent of the locations of other tiles.
		/// </summary>
		ImageTileOffsets =
			0x0144,

		/// <summary>
		/// For each tile, the number of (compressed) bytes in that tile. See TileOffsets for a description of how the byte counts are ordered.
		/// </summary>
		ImageTileByteCounts = 0x0145,

		/// <summary>
		/// Defined by Adobe Corporation to enable TIFF Trees within a TIFF file.
		/// </summary>
		ImageSubIfDs = 0x014a,

		/// <summary>
		/// The set of inks used in a separated (PhotometricInterpretation=5) Image
		/// </summary>
		ImageInkSet = 0x014c,

		/// <summary>
		/// The name of each ink used in a separated (PhotometricInterpretation=5) Image
		/// </summary>
		ImageInkNames = 0x014d,

		/// <summary>
		/// The number of inks. Usually equal to SamplesPerPixel, unless there are extra samples.
		/// </summary>
		ImageNumberOfInks = 0x014e,

		/// <summary>
		/// The component values that correspond to a 0% dot and 100% dot.
		/// </summary>
		ImageDotRange = 0x0150,

		/// <summary>
		/// A description of the printing environment for which this separation is intended.
		/// </summary>
		ImageTargetPrinter = 0x0151,

		/// <summary>
		/// Specifies that each pixel has m extra components whose interpretation is defined by one of the values listed below.
		/// </summary>
		ImageExtraSamples = 0x0152,

		/// <summary>
		/// This field specifies how to interpret each data sample in a pixel.
		/// </summary>
		ImageSampleFormat = 0x0153,

		/// <summary>
		/// This field specifies the minimum sample value.
		/// </summary>
		ImageSMinSampleValue = 0x0154,

		/// <summary>
		/// This field specifies the maximum sample value.
		/// </summary>
		ImageSMaxSampleValue = 0x0155,

		/// <summary>
		/// Expands the range of the TransferFunction
		/// </summary>
		ImageTransferRange = 0x0156,

		/// <summary>
		/// A TIFF ClipPath is intended to mirror the essentials of PostScript's path creation functionality.
		/// </summary>
		ImageClipPath = 0x0157,

		/// <summary>
		/// The number of units that span the width of the image, in terms of integer ClipPath coordinates.
		/// </summary>
		ImageXClipPathUnits = 0x0158,

		/// <summary>
		/// The number of units that span the height of the image, in terms of integer ClipPath coordinates.
		/// </summary>
		ImageYClipPathUnits = 0x0159,

		/// <summary>
		/// Indexed images are images where the 'pixels' do not represent color values, but rather an index (usually 8-bit) into a separate color table, the ColorMap.
		/// </summary>
		ImageIndexed =
			0x015a,

		/// <summary>
		/// This optional tag may be used to encode the JPEG quantization andHuffman tables for subsequent use by the JPEG decompression process.
		/// </summary>
		ImageJpegTables = 0x015b,

		/// <summary>
		/// OPIProxy gives information concerning whether this image is a low-resolution proxy of a high-resolution image (Adobe OPI).
		/// </summary>
		ImageOpiProxy = 0x015f,

		/// <summary>
		/// This field indicates the process used to produce the compressed data
		/// </summary>
		ImageJpegProc = 0x0200,

		/// <summary>
		/// The offset to the start byte (SOI) of JPEG compressed thumbnail data. This is not used for primary image JPEG data.
		/// </summary>
		ImageJpegInterchangeFormat = 0x0201,

		/// <summary>
		/// The number of bytes of JPEG compressed thumbnail data. This is not used for primary image JPEG data. JPEG thumbnails are not divided but are recorded as a continuous JPEG bitstream from SOI to EOI. Appn and COM markers should not be recorded. Compressed thumbnails must be recorded in no more than 64 Kbytes, including all other data to be recorded in APP1.
		/// </summary>
		ImageJpegInterchangeFormatLength =
			0x0202,

		/// <summary>
		/// This Field indicates the length of the restart interval used in the compressed image data.
		/// </summary>
		ImageJpegRestartInterval = 0x0203,

		/// <summary>
		/// This Field points to a list of lossless predictor-selection values, one per component.
		/// </summary>
		ImageJpegLosslessPredictors = 0x0205,

		/// <summary>
		/// This Field points to a list of point transform values, one per component.
		/// </summary>
		ImageJpegPointTransforms = 0x0206,

		/// <summary>
		/// This Field points to a list of offsets to the quantization tables, one per component.
		/// </summary>
		ImageJpegqTables = 0x0207,

		/// <summary>
		/// This Field points to a list of offsets to the DC Huffman tables or the lossless Huffman tables, one per component.
		/// </summary>
		ImageJpegdcTables = 0x0208,

		/// <summary>
		/// This Field points to a list of offsets to the Huffman AC tables, one per component.
		/// </summary>
		ImageJpegacTables = 0x0209,

		/// <summary>
		/// The matrix coefficients for transformation from RGB to YCbCr image data. No default is given in TIFF; but here the value given in Appendix E, "Color Space Guidelines", is used as the default. The color space is declared in a color space information tag, with the default being the value that gives the optimal image characteristics Interoperability this condition.
		/// </summary>
		ImageYCbCrCoefficients =
			0x0211,

		/// <summary>
		/// The sampling ratio of chrominance components in relation to the luminance component. In JPEG compressed data a JPEG marker is used instead of this tag.
		/// </summary>
		ImageYCbCrSubSampling =
			0x0212,

		/// <summary>
		/// The position of chrominance components in relation to the luminance component. This field is designated only for JPEG compressed data or uncompressed YCbCr data. The TIFF default is 1 (centered); but when Y:Cb:Cr = 4:2:2 it is recommended in this standard that 2 (co-sited) be used to record data, in order to improve the image quality when viewed on TV systems. When this field does not exist, the reader shall assume the TIFF default. In the case of Y:Cb:Cr = 4:2:0, the TIFF default (centered) is recommended. If the reader does not have the capability of supporting both kinds of <YCbCrPositioning>, it shall follow the TIFF default regardless of the value in this field. It is preferable that readers be able to support both centered and co-sited positioning.
		/// </summary>
		ImageYCbCrPositioning =
			0x0213,

		/// <summary>
		/// The reference black point value and reference white point value. No defaults are given in TIFF, but the values below are given as defaults here. The color space is declared in a color space information tag, with the default being the value that gives the optimal image characteristics Interoperability these conditions.
		/// </summary>
		ImageReferenceBlackWhite =
			0x0214,

		/// <summary>
		/// XMP Metadata (Adobe technote 9-14-02)
		/// </summary>
		ImageXMLPacket = 0x02bc,

		/// <summary>
		/// Rating tag used by Windows
		/// </summary>
		ImageRating = 0x4746,

		/// <summary>
		/// Rating tag used by Windows, value in percent
		/// </summary>
		ImageRatingPercent = 0x4749,

		/// <summary>
		/// ImageID is the full pathname of the original, high-resolution image, or any other identifying string that uniquely identifies the original image (Adobe OPI).
		/// </summary>
		ImageImageID =
			0x800d,

		/// <summary>
		/// Contains two values representing the minimum rows and columns to define the repeating patterns of the color filter array
		/// </summary>
		ImageCfaRepeatPatternDim = 0x828d,

		/// <summary>
		/// Indicates the color filter array (CFA) geometric pattern of the image sensor when a one-chip color area sensor is used. It does not apply to all sensing methods
		/// </summary>
		ImageCfaPattern =
			0x828e,

		/// <summary>
		/// Contains a value of the battery level as a fraction or string
		/// </summary>
		ImageBatteryLevel = 0x828f,

		/// <summary>
		/// Copyright information. In this standard the tag is used to indicate both the photographer and editor copyrights. It is the copyright notice of the person or organization claiming rights to the Image The Interoperability copyright statement including date and rights should be written in this field; e.g., "Copyright, John Smith, 19xx. All rights reserved.". In this standard the field records both the photographer and editor copyrights, with each recorded in a separate part of the statement. When there is a clear distinction between the photographer and editor copyrights, these are to be written in the order of photographer followed by editor copyright, separated by NULL (in this case since the statement also ends with a NULL, there are two NULL codes). When only the photographer copyright is given, it is terminated by one NULL code . When only the editor copyright is given, the photographer copyright part consists of one space followed by a terminating NULL code, then the editor copyright is given. When the field is left blank, it is treated as unknown.
		/// </summary>
		ImageCopyright =
			0x8298,

		/// <summary>
		/// Exposure time, given in seconds.
		/// </summary>
		ImageExposureTime = 0x829a,

		/// <summary>
		/// The F number.
		/// </summary>
		ImageFNumber = 0x829d,

		/// <summary>
		/// Contains an IPTC/NAA record
		/// </summary>
		ImageIptcnaa = 0x83bb,

		/// <summary>
		/// Contains information embedded by the Adobe Photoshop application
		/// </summary>
		ImageImageResources = 0x8649,

		/// <summary>
		/// A pointer to the Exif IFD. Interoperability, Exif IFD has the same structure as that of the IFD specified in TIFF. ordinarily, however, it does not contain image data as in the case of TIFF.
		/// </summary>
		ImageExifTag =
			0x8769,

		/// <summary>
		/// Contains an InterColor Consortium (ICC) format color space characterization/profile
		/// </summary>
		ImageInterColorProfile = 0x8773,

		/// <summary>
		/// The class of the program used by the camera to set exposure when the picture is taken.
		/// </summary>
		ImageExposureProgram = 0x8822,

		/// <summary>
		/// Indicates the spectral sensitivity of each channel of the camera used.
		/// </summary>
		ImageSpectralSensitivity = 0x8824,

		/// <summary>
		/// A pointer to the GPS Info IFD. The Interoperability structure of the GPS Info IFD, like that of Exif IFD, has no image data.
		/// </summary>
		ImageGpsTag = 0x8825,

		/// <summary>
		/// Indicates the ISO Speed and ISO Latitude of the camera or input device as specified in ISO 12232.
		/// </summary>
		ImageIsoSpeedRatings = 0x8827,

		/// <summary>
		/// Indicates the Opto-Electric Conversion Function (OECF) specified in ISO 14524.
		/// </summary>
		ImageOecf = 0x8828,

		/// <summary>
		/// Indicates the field number of multifield images.
		/// </summary>
		ImageInterlace = 0x8829,

		/// <summary>
		/// This optional tag encodes the time zone of the camera clock (relativeto Greenwich Mean Time) used to create the DataTimeOriginal tag-valuewhen the picture was taken. It may also contain the time zone offsetof the clock used to create the DateTime tag-value when the image wasmodified.
		/// </summary>
		ImageTimeZoneOffset =
			0x882a,

		/// <summary>
		/// Number of seconds image capture was delayed from button press.
		/// </summary>
		ImageSelfTimerMode = 0x882b,

		/// <summary>
		/// The date and time when the original image data was generated.
		/// </summary>
		ImageDateTimeOriginal = 0x9003,

		/// <summary>
		/// Specific to compressed data; states the compressed bits per pixel.
		/// </summary>
		ImageCompressedBitsPerPixel = 0x9102,

		/// <summary>
		/// Shutter speed.
		/// </summary>
		ImageShutterSpeedValue = 0x9201,

		/// <summary>
		/// The lens aperture.
		/// </summary>
		ImageApertureValue = 0x9202,

		/// <summary>
		/// The value of brightness.
		/// </summary>
		ImageBrightnessValue = 0x9203,

		/// <summary>
		/// The exposure bias.
		/// </summary>
		ImageExposureBiasValue = 0x9204,

		/// <summary>
		/// The smallest F number of the lens.
		/// </summary>
		ImageMaxApertureValue = 0x9205,

		/// <summary>
		/// The distance to the subject, given in meters.
		/// </summary>
		ImageSubjectDistance = 0x9206,

		/// <summary>
		/// The metering mode.
		/// </summary>
		ImageMeteringMode = 0x9207,

		/// <summary>
		/// The kind of light source.
		/// </summary>
		ImageLightSource = 0x9208,

		/// <summary>
		/// Indicates the status of flash when the image was shot.
		/// </summary>
		ImageFlash = 0x9209,

		/// <summary>
		/// The actual focal length of the lens, in mm.
		/// </summary>
		ImageFocalLength = 0x920a,

		/// <summary>
		/// Amount of flash energy (BCPS).
		/// </summary>
		ImageFlashEnergy = 0x920b,

		/// <summary>
		/// SFR of the camera.
		/// </summary>
		ImageSpatialFrequencyResponse = 0x920c,

		/// <summary>
		/// Noise measurement values.
		/// </summary>
		ImageNoise = 0x920d,

		/// <summary>
		/// Number of pixels per FocalPlaneResolutionUnit (37392) in ImageWidth direction for main Image
		/// </summary>
		ImageFocalPlaneXResolution = 0x920e,

		/// <summary>
		/// Number of pixels per FocalPlaneResolutionUnit (37392) in ImageLength direction for main Image
		/// </summary>
		ImageFocalPlaneYResolution = 0x920f,

		/// <summary>
		/// Unit of measurement for FocalPlaneXResolution(37390) and FocalPlaneYResolution(37391).
		/// </summary>
		ImageFocalPlaneResolutionUnit = 0x9210,

		/// <summary>
		/// Number assigned to an image, e.g., in a chained image burst.
		/// </summary>
		ImageImageNumber = 0x9211,

		/// <summary>
		/// Security classification assigned to the Image
		/// </summary>
		ImageSecurityClassification = 0x9212,

		/// <summary>
		/// Record of what has been done to the Image
		/// </summary>
		ImageImageHistory = 0x9213,

		/// <summary>
		/// Indicates the location and area of the main subject in the overall scene.
		/// </summary>
		ImageSubjectLocation = 0x9214,

		/// <summary>
		/// Encodes the camera exposure index setting when image was captured.
		/// </summary>
		ImageExposureIndex = 0x9215,

		/// <summary>
		/// Contains four ASCII characters representing the TIFF/EP standard version of a TIFF/EP file, eg '1', '0', '0', '0'
		/// </summary>
		ImageTiffepStandardID = 0x9216,

		/// <summary>
		/// Type of image sensor.
		/// </summary>
		ImageSensingMethod = 0x9217,

		/// <summary>
		/// Title tag used by Windows, encoded in UCS2
		/// </summary>
		ImageXpTitle = 0x9c9b,

		/// <summary>
		/// Comment tag used by Windows, encoded in UCS2
		/// </summary>
		ImageXpComment = 0x9c9c,

		/// <summary>
		/// Author tag used by Windows, encoded in UCS2
		/// </summary>
		ImageXpAuthor = 0x9c9d,

		/// <summary>
		/// Keywords tag used by Windows, encoded in UCS2
		/// </summary>
		ImageXpKeywords = 0x9c9e,

		/// <summary>
		/// Subject tag used by Windows, encoded in UCS2
		/// </summary>
		ImageXpSubject = 0x9c9f,

		/// <summary>
		/// Print Image Matching, description needed.
		/// </summary>
		ImagePrintImageMatching = 0xc4a5,

		/// <summary>
		/// This tag encodes the DNG four-tier version number. For files compliant with version 1.1.0.0 of the DNG specification, this tag should contain the bytes: 1, 1, 0, 0.
		/// </summary>
		ImageDngVersion =
			0xc612,

		/// <summary>
		/// This tag specifies the oldest version of the Digital Negative specification for which a file is compatible. Readers shouldnot attempt to read a file if this tag specifies a version number that is higher than the version number of the specification the reader was based on. In addition to checking the version tags, readers should, for all tags, check the types, counts, and values, to verify it is able to correctly read the file.
		/// </summary>
		ImageDngBackwardVersion =
			0xc613,

		/// <summary>
		/// Defines a unique, non-localized name for the camera model that created the image in the raw file. This name should include the manufacturer's name to avoid conflicts, and should not be localized, even if the camera name itself is localized for different markets (see LocalizedCameraModel). This string may be used by reader software to index into per-model preferences and replacement profiles.
		/// </summary>
		ImageUniqueCameraModel =
			0xc614,

		/// <summary>
		/// Similar to the UniqueCameraModel field, except the name can be localized for different markets to match the localization of the camera name.
		/// </summary>
		ImageLocalizedCameraModel =
			0xc615,

		/// <summary>
		/// Provides a mapping between the values in the CFAPattern tag and the plane numbers in LinearRaw space. This is a required tag for non-RGB CFA images.
		/// </summary>
		ImageCfaPlaneColor =
			0xc616,

		/// <summary>
		/// Describes the spatial layout of the CFA.
		/// </summary>
		ImageCfaLayout = 0xc617,

		/// <summary>
		/// Describes a lookup table that maps stored values into linear values. This tag is typically used to increase compression ratios by storing the raw data in a non-linear, more visually uniform space with fewer total encoding levels. If SamplesPerPixel is not equal to one, this single table applies to all the samples for each pixel.
		/// </summary>
		ImageLinearizationTable =
			0xc618,

		/// <summary>
		/// Specifies repeat pattern size for the BlackLevel tag.
		/// </summary>
		ImageBlackLevelRepeatDim = 0xc619,

		/// <summary>
		/// Specifies the zero light (a.k.a. thermal black or black current) encoding level, as a repeating pattern. The origin of this pattern is the top-left corner of the ActiveArea rectangle. The values are stored in row-column-sample scan order.
		/// </summary>
		ImageBlackLevel =
			0xc61a,

		/// <summary>
		/// If the zero light encoding level is a function of the image column, BlackLevelDeltaH specifies the difference between the zero light encoding level for each column and the baseline zero light encoding level. If SamplesPerPixel is not equal to one, this single table applies to all the samples for each pixel.
		/// </summary>
		ImageBlackLevelDeltaH =
			0xc61b,

		/// <summary>
		/// If the zero light encoding level is a function of the image row, this tag specifies the difference between the zero light encoding level for each row and the baseline zero light encoding level. If SamplesPerPixel is not equal to one, this single table applies to all the samples for each pixel.
		/// </summary>
		ImageBlackLevelDeltaV =
			0xc61c,

		/// <summary>
		/// This tag specifies the fully saturated encoding level for the raw sample values. Saturation is caused either by the sensor itself becoming highly non-linear in response, or by the camera's analog to digital converter clipping.
		/// </summary>
		ImageWhiteLevel =
			0xc61d,

		/// <summary>
		/// DefaultScale is required for cameras with non-square pixels. It specifies the default scale factors for each direction to convert the image to square pixels. Typically these factors are selected to approximately preserve total pixel count. For CFA images that use CFALayout equal to 2, 3, 4, or 5, such as the Fujifilm SuperCCD, these two values should usually differ by a factor of 2.0.
		/// </summary>
		ImageDefaultScale =
			0xc61e,

		/// <summary>
		/// Raw images often store extra pixels around the edges of the final Image These extra pixels help prevent interpolation artifacts near the edges of the final Image DefaultCropOrigin specifies the origin of the final image area, in raw image coordinates (i.e., before the DefaultScale has been applied), relative to the top-left corner of the ActiveArea rectangle.
		/// </summary>
		ImageDefaultCropOrigin =
			0xc61f,

		/// <summary>
		/// Raw images often store extra pixels around the edges of the final Image These extra pixels help prevent interpolation artifacts near the edges of the final Image DefaultCropSize specifies the size of the final image area, in raw image coordinates (i.e., before the DefaultScale has been applied).
		/// </summary>
		ImageDefaultCropSize =
			0xc620,

		/// <summary>
		/// ColorMatrix1 defines a transformation matrix that converts XYZ values to reference camera native color space values, under the first calibration illuminant. The matrix values are stored in row scan order. The ColorMatrix1 tag is required for all non-monochrome DNG files.
		/// </summary>
		ImageColorMatrix1 =
			0xc621,

		/// <summary>
		/// ColorMatrix2 defines a transformation matrix that converts XYZ values to reference camera native color space values, under the second calibration illuminant. The matrix values are stored in row scan order.
		/// </summary>
		ImageColorMatrix2 =
			0xc622,

		/// <summary>
		/// CameraClalibration1 defines a calibration matrix that transforms reference camera native space values to individual camera native space values under the first calibration illuminant. The matrix is stored in row scan order. This matrix is stored separately from the matrix specified by the ColorMatrix1 tag to allow raw converters to swap in replacement color matrices based on UniqueCameraModel tag, while still taking advantage of any per-individual camera calibration performed by the camera manufacturer.
		/// </summary>
		ImageCameraCalibration1 =
			0xc623,

		/// <summary>
		/// CameraCalibration2 defines a calibration matrix that transforms reference camera native space values to individual camera native space values under the second calibration illuminant. The matrix is stored in row scan order. This matrix is stored separately from the matrix specified by the ColorMatrix2 tag to allow raw converters to swap in replacement color matrices based on UniqueCameraModel tag, while still taking advantage of any per-individual camera calibration performed by the camera manufacturer.
		/// </summary>
		ImageCameraCalibration2 =
			0xc624,

		/// <summary>
		/// ReductionMatrix1 defines a dimensionality reduction matrix for use as the first stage in converting color camera native space values to XYZ values, under the first calibration illuminant. This tag may only be used if ColorPlanes is greater than 3. The matrix is stored in row scan order.
		/// </summary>
		ImageReductionMatrix1 =
			0xc625,

		/// <summary>
		/// ReductionMatrix2 defines a dimensionality reduction matrix for use as the first stage in converting color camera native space values to XYZ values, under the second calibration illuminant. This tag may only be used if ColorPlanes is greater than 3. The matrix is stored in row scan order.
		/// </summary>
		ImageReductionMatrix2 =
			0xc626,

		/// <summary>
		/// Normally the stored raw values are not white balanced, since any digital white balancing will reduce the dynamic range of the final image if the user decides to later adjust the white balance; however, if camera hardware is capable of white balancing the color channels before the signal is digitized, it can improve the dynamic range of the final Image AnalogBalance defines the gain, either analog (recommended) or digital (not recommended) that has been applied the stored raw values.
		/// </summary>
		ImageAnalogBalance =
			0xc627,

		/// <summary>
		/// Specifies the selected white balance at time of capture, encoded as the coordinates of a perfectly neutral color in linear reference space values. The inclusion of this tag precludes the inclusion of the AsShotWhiteXY tag.
		/// </summary>
		ImageAsShotNeutral =
			0xc628,

		/// <summary>
		/// Specifies the selected white balance at time of capture, encoded as x-y chromaticity coordinates. The inclusion of this tag precludes the inclusion of the AsShotNeutral tag.
		/// </summary>
		ImageAsShotWhiteXy =
			0xc629,

		/// <summary>
		/// Camera models vary in the trade-off they make between highlight headroom and shadow noise. Some leave a significant amount of highlight headroom during a normal exposure. This allows significant negative exposure compensation to be applied during raw conversion, but also means normal exposures will contain more shadow noise. Other models leave less headroom during normal exposures. This allows for less negative exposure compensation, but results in lower shadow noise for normal exposures. Because of these differences, a raw converter needs to vary the zero point of its exposure compensation control from model to model. BaselineExposure specifies by how much (in EV units) to move the zero point. Positive values result in brighter default results, while negative values result in darker default results.
		/// </summary>
		ImageBaselineExposure =
			0xc62a,

		/// <summary>
		/// Specifies the relative noise level of the camera model at a baseline ISO value of 100, compared to a reference camera model. Since noise levels tend to vary approximately with the square root of the ISO value, a raw converter can use this value, combined with the current ISO, to estimate the relative noise level of the current Image
		/// </summary>
		ImageBaselineNoise =
			0xc62b,

		/// <summary>
		/// Specifies the relative amount of sharpening required for this camera model, compared to a reference camera model. Camera models vary in the strengths of their anti-aliasing filters. Cameras with weak or no filters require less sharpening than cameras with strong anti-aliasing filters.
		/// </summary>
		ImageBaselineSharpness =
			0xc62c,

		/// <summary>
		/// Only applies to CFA images using a Bayer pattern filter array. This tag specifies, in arbitrary units, how closely the values of the green pixels in the blue/green rows track the values of the green pixels in the red/green rows. A value of zero means the two kinds of green pixels track closely, while a non-zero value means they sometimes diverge. The useful range for this tag is from 0 (no divergence) to about 5000 (quite large divergence).
		/// </summary>
		ImageBayerGreenSplit =
			0xc62d,

		/// <summary>
		/// Some sensors have an unpredictable non-linearity in their response as they near the upper limit of their encoding range. This non-linearity results in color shifts in the highlight areas of the resulting image unless the raw converter compensates for this effect. LinearResponseLimit specifies the fraction of the encoding range above which the response may become significantly non-linear.
		/// </summary>
		ImageLinearResponseLimit =
			0xc62e,

		/// <summary>
		/// CameraSerialNumber contains the serial number of the camera or camera body that captured the Image
		/// </summary>
		ImageCameraSerialNumber = 0xc62f,

		/// <summary>
		/// Contains information about the lens that captured the Image If the minimum f-stops are unknown, they should be encoded as 0/0.
		/// </summary>
		ImageLensInfo = 0xc630,

		/// <summary>
		/// ChromaBlurRadius provides a hint to the DNG reader about how much chroma blur should be applied to the Image If this tag is omitted, the reader will use its default amount of chroma blurring. Normally this tag is only included for non-CFA images, since the amount of chroma blur required for mosaic images is highly dependent on the de-mosaic algorithm, in which case the DNG reader's default value is likely optimized for its particular de-mosaic algorithm.
		/// </summary>
		ImageChromaBlurRadius =
			0xc631,

		/// <summary>
		/// Provides a hint to the DNG reader about how strong the camera's anti-alias filter is. A value of 0.0 means no anti-alias filter (i.e., the camera is prone to aliasing artifacts with some subjects), while a value of 1.0 means a strong anti-alias filter (i.e., the camera almost never has aliasing artifacts).
		/// </summary>
		ImageAntiAliasStrength =
			0xc632,

		/// <summary>
		/// This tag is used by Adobe Camera Raw to control the sensitivity of its 'Shadows' slider.
		/// </summary>
		ImageShadowScale = 0xc633,

		/// <summary>
		/// Provides a way for camera manufacturers to store private data in the DNG file for use by their own raw converters, and to have that data preserved by programs that edit DNG files.
		/// </summary>
		ImageDngPrivateData =
			0xc634,

		/// <summary>
		/// MakerNoteSafety lets the DNG reader know whether the EXIF MakerNote tag is safe to preserve along with the rest of the EXIF data. File browsers and other image management software processing an image with a preserved MakerNote should be aware that any thumbnail image embedded in the MakerNote may be stale, and may not reflect the current state of the full size Image
		/// </summary>
		ImageMakerNoteSafety =
			0xc635,

		/// <summary>
		/// The illuminant used for the first set of color calibration tags (ColorMatrix1, CameraCalibration1, ReductionMatrix1). The legal values for this tag are the same as the legal values for the LightSource EXIF tag.
		/// </summary>
		ImageCalibrationIlluminant1 =
			0xc65a,

		/// <summary>
		/// The illuminant used for an optional second set of color calibration tags (ColorMatrix2, CameraCalibration2, ReductionMatrix2). The legal values for this tag are the same as the legal values for the CalibrationIlluminant1 tag; however, if both are included, neither is allowed to have a value of 0 (unknown).
		/// </summary>
		ImageCalibrationIlluminant2 =
			0xc65b,

		/// <summary>
		/// For some cameras, the best possible image quality is not achieved by preserving the total pixel count during conversion. For example, Fujifilm SuperCCD images have maximum detail when their total pixel count is doubled. This tag specifies the amount by which the values of the DefaultScale tag need to be multiplied to achieve the best quality image size.
		/// </summary>
		ImageBestQualityScale =
			0xc65c,

		/// <summary>
		/// This tag contains a 16-byte unique identifier for the raw image data in the DNG file. DNG readers can use this tag to recognize a particular raw image, even if the file's name or the metadata contained in the file has been changed. If a DNG writer creates such an identifier, it should do so using an algorithm that will ensure that it is very unlikely two different images will end up having the same identifier.
		/// </summary>
		ImageRawDataUniqueID =
			0xc65d,

		/// <summary>
		/// If the DNG file was converted from a non-DNG raw file, then this tag contains the file name of that original raw file.
		/// </summary>
		ImageOriginalRawFileName = 0xc68b,

		/// <summary>
		/// If the DNG file was converted from a non-DNG raw file, then this tag contains the compressed contents of that original raw file. The contents of this tag always use the big-endian byte order. The tag contains a sequence of data blocks. Future versions of the DNG specification may define additional data blocks, so DNG readers should ignore extra bytes when parsing this tag. DNG readers should also detect the case where data blocks are missing from the end of the sequence, and should assume a default value for all the missing blocks. There are no padding or alignment bytes between data blocks.
		/// </summary>
		ImageOriginalRawFileData =
			0xc68c,

		/// <summary>
		/// This rectangle defines the active (non-masked) pixels of the sensor. The order of the rectangle coordinates is: top, left, bottom, right.
		/// </summary>
		ImageActiveArea = 0xc68d,

		/// <summary>
		/// This tag contains a list of non-overlapping rectangle coordinates of fully masked pixels, which can be optionally used by DNG readers to measure the black encoding level. The order of each rectangle's coordinates is: top, left, bottom, right. If the raw image data has already had its black encoding level subtracted, then this tag should not be used, since the masked pixels are no longer useful.
		/// </summary>
		ImageMaskedAreas =
			0xc68e,

		/// <summary>
		/// This tag contains an ICC profile that, in conjunction with the AsShotPreProfileMatrix tag, provides the camera manufacturer with a way to specify a default color rendering from camera color space coordinates (linear reference values) into the ICC profile connection space. The ICC profile connection space is an output referred colorimetric space, whereas the other color calibration tags in DNG specify a conversion into a scene referred colorimetric space. This means that the rendering in this profile should include any desired tone and gamut mapping needed to convert between scene referred values and output referred values.
		/// </summary>
		ImageAsShotIccProfile =
			0xc68f,

		/// <summary>
		/// This tag is used in conjunction with the AsShotICCProfile tag. It specifies a matrix that should be applied to the camera color space coordinates before processing the values through the ICC profile specified in the AsShotICCProfile tag. The matrix is stored in the row scan order. If ColorPlanes is greater than three, then this matrix can (but is not required to) reduce the dimensionality of the color data down to three components, in which case the AsShotICCProfile should have three rather than ColorPlanes input components.
		/// </summary>
		ImageAsShotPreProfileMatrix =
			0xc690,

		/// <summary>
		/// This tag is used in conjunction with the CurrentPreProfileMatrix tag. The CurrentICCProfile and CurrentPreProfileMatrix tags have the same purpose and usage as the AsShotICCProfile and AsShotPreProfileMatrix tag pair, except they are for use by raw file editors rather than camera manufacturers.
		/// </summary>
		ImageCurrentIccProfile =
			0xc691,

		/// <summary>
		/// This tag is used in conjunction with the CurrentICCProfile tag. The CurrentICCProfile and CurrentPreProfileMatrix tags have the same purpose and usage as the AsShotICCProfile and AsShotPreProfileMatrix tag pair, except they are for use by raw file editors rather than camera manufacturers.
		/// </summary>
		ImageCurrentPreProfileMatrix =
			0xc692,

		/// <summary>
		/// The DNG color model documents a transform between camera colors and CIE XYZ values. This tag describes the colorimetric reference for the CIE XYZ values. 0 = The XYZ values are scene-referred. 1 = The XYZ values are output-referred, using the ICC profile perceptual dynamic range. This tag allows output-referred data to be stored in DNG files and still processed correctly by DNG readers.
		/// </summary>
		ImageColorimetricReference =
			0xc6bf,

		/// <summary>
		/// A UTF-8 encoded string associated with the CameraCalibration1 and CameraCalibration2 tags. The CameraCalibration1 and CameraCalibration2 tags should only be used in the DNG color transform if the string stored in the CameraCalibrationSignature tag exactly matches the string stored in the ProfileCalibrationSignature tag for the selected camera profile.
		/// </summary>
		ImageCameraCalibrationSignature =
			0xc6f3,

		/// <summary>
		/// A UTF-8 encoded string associated with the camera profile tags. The CameraCalibration1 and CameraCalibration2 tags should only be used in the DNG color transfer if the string stored in the CameraCalibrationSignature tag exactly matches the string stored in the ProfileCalibrationSignature tag for the selected camera profile.
		/// </summary>
		ImageProfileCalibrationSignature =
			0xc6f4,

		/// <summary>
		/// A UTF-8 encoded string containing the name of the "as shot" camera profile, if any.
		/// </summary>
		ImageAsShotProfileName = 0xc6f6,

		/// <summary>
		/// This tag indicates how much noise reduction has been applied to the raw data on a scale of 0.0 to 1.0. A 0.0 value indicates that no noise reduction has been applied. A 1.0 value indicates that the "ideal" amount of noise reduction has been applied, i.e. that the DNG reader should not apply additional noise reduction by default. A value of 0/0 indicates that this parameter is unknown.
		/// </summary>
		ImageNoiseReductionApplied =
			0xc6f7,

		/// <summary>
		/// A UTF-8 encoded string containing the name of the camera profile. This tag is optional if there is only a single camera profile stored in the file but is required for all camera profiles if there is more than one camera profile stored in the file.
		/// </summary>
		ImageProfileName =
			0xc6f8,

		/// <summary>
		/// This tag specifies the number of input samples in each dimension of the hue/saturation/value mapping tables. The data for these tables are stored in ProfileHueSatMapData1 and ProfileHueSatMapData2 tags. The most common case has ValueDivisions equal to 1, so only hue and saturation are used as inputs to the mapping table.
		/// </summary>
		ImageProfileHueSatMapDims =
			0xc6f9,

		/// <summary>
		/// This tag contains the data for the first hue/saturation/value mapping table. Each entry of the table contains three 32-bit IEEE floating-point values. The first entry is hue shift in degrees; the second entry is saturation scale factor; and the third entry is a value scale factor. The table entries are stored in the tag in nested loop order, with the value divisions in the outer loop, the hue divisions in the middle loop, and the saturation divisions in the inner loop. All zero input saturation entries are required to have a value scale factor of 1.0.
		/// </summary>
		ImageProfileHueSatMapData1 =
			0xc6fa,

		/// <summary>
		/// This tag contains the data for the second hue/saturation/value mapping table. Each entry of the table contains three 32-bit IEEE floating-point values. The first entry is hue shift in degrees; the second entry is a saturation scale factor; and the third entry is a value scale factor. The table entries are stored in the tag in nested loop order, with the value divisions in the outer loop, the hue divisions in the middle loop, and the saturation divisions in the inner loop. All zero input saturation entries are required to have a value scale factor of 1.0.
		/// </summary>
		ImageProfileHueSatMapData2 =
			0xc6fb,

		/// <summary>
		/// This tag contains a default tone curve that can be applied while processing the image as a starting point for user adjustments. The curve is specified as a list of 32-bit IEEE floating-point value pairs in linear gamma. Each sample has an input value in the range of 0.0 to 1.0, and an output value in the range of 0.0 to 1.0. The first sample is required to be (0.0, 0.0), and the last sample is required to be (1.0, 1.0). Interpolated the curve using a cubic spline.
		/// </summary>
		ImageProfileToneCurve =
			0xc6fc,

		/// <summary>
		/// This tag contains information about the usage rules for the associated camera profile.
		/// </summary>
		ImageProfileEmbedPolicy = 0xc6fd,

		/// <summary>
		/// A UTF-8 encoded string containing the copyright information for the camera profile. This string always should be preserved along with the other camera profile tags.
		/// </summary>
		ImageProfileCopyright =
			0xc6fe,

		/// <summary>
		/// This tag defines a matrix that maps white balanced camera colors to XYZ D50 colors.
		/// </summary>
		ImageForwardMatrix1 = 0xc714,

		/// <summary>
		/// This tag defines a matrix that maps white balanced camera colors to XYZ D50 colors.
		/// </summary>
		ImageForwardMatrix2 = 0xc715,

		/// <summary>
		/// A UTF-8 encoded string containing the name of the application that created the preview stored in the IFD.
		/// </summary>
		ImagePreviewApplicationName = 0xc716,

		/// <summary>
		/// A UTF-8 encoded string containing the version number of the application that created the preview stored in the IFD.
		/// </summary>
		ImagePreviewApplicationVersion = 0xc717,

		/// <summary>
		/// A UTF-8 encoded string containing the name of the conversion settings (for example, snapshot name) used for the preview stored in the IFD.
		/// </summary>
		ImagePreviewSettingsName =
			0xc718,

		/// <summary>
		/// A unique ID of the conversion settings (for example, MD5 digest) used to render the preview stored in the IFD.
		/// </summary>
		ImagePreviewSettingsDigest = 0xc719,

		/// <summary>
		/// This tag specifies the color space in which the rendered preview in this IFD is stored. The default value for this tag is sRGB for color previews and Gray Gamma 2.2 for monochrome previews.
		/// </summary>
		ImagePreviewColorSpace =
			0xc71a,

		/// <summary>
		/// This tag is an ASCII string containing the name of the date/time at which the preview stored in the IFD was rendered. The date/time is encoded using ISO 8601 format.
		/// </summary>
		ImagePreviewDateTime =
			0xc71b,

		/// <summary>
		/// This tag is an MD5 digest of the raw image data. All pixels in the image are processed in row-scan order. Each pixel is zero padded to 16 or 32 bits deep (16-bit for data less than or equal to 16 bits deep, 32-bit otherwise). The data for each pixel is processed in little-endian byte order.
		/// </summary>
		ImageRawImageDigest =
			0xc71c,

		/// <summary>
		/// This tag is an MD5 digest of the data stored in the OriginalRawFileData tag.
		/// </summary>
		ImageOriginalRawFileDigest = 0xc71d,

		/// <summary>
		/// Normally, the pixels within a tile are stored in simple row-scan order. This tag specifies that the pixels within a tile should be grouped first into rectangular blocks of the specified size. These blocks are stored in row-scan order. Within each block, the pixels are stored in row-scan order. The use of a non-default value for this tag requires setting the DNGBackwardVersion tag to at least 1.2.0.0.
		/// </summary>
		ImageSubTileBlockSize =
			0xc71e,

		/// <summary>
		/// This tag specifies that rows of the image are stored in interleaved order. The value of the tag specifies the number of interleaved fields. The use of a non-default value for this tag requires setting the DNGBackwardVersion tag to at least 1.2.0.0.
		/// </summary>
		ImageRowInterleaveFactor =
			0xc71f,

		/// <summary>
		/// This tag specifies the number of input samples in each dimension of a default "look" table. The data for this table is stored in the ProfileLookTableData tag.
		/// </summary>
		ImageProfileLookTableDims =
			0xc725,

		/// <summary>
		/// This tag contains a default "look" table that can be applied while processing the image as a starting point for user adjustment. This table uses the same format as the tables stored in the ProfileHueSatMapData1 and ProfileHueSatMapData2 tags, and is applied in the same color space. However, it should be applied later in the processing pipe, after any exposure compensation and/or fill light stages, but before any tone curve stage. Each entry of the table contains three 32-bit IEEE floating-point values. The first entry is hue shift in degrees, the second entry is a saturation scale factor, and the third entry is a value scale factor. The table entries are stored in the tag in nested loop order, with the value divisions in the outer loop, the hue divisions in the middle loop, and the saturation divisions in the inner loop. All zero input saturation entries are required to have a value scale factor of 1.0.
		/// </summary>
		ImageProfileLookTableData =
			0xc726,

		/// <summary>
		/// Specifies the list of opcodes that should be applied to the raw image, as read directly from the file.
		/// </summary>
		ImageOpcodeList1 = 0xc740,

		/// <summary>
		/// Specifies the list of opcodes that should be applied to the raw image, just after it has been mapped to linear reference values.
		/// </summary>
		ImageOpcodeList2 = 0xc741,

		/// <summary>
		/// Specifies the list of opcodes that should be applied to the raw image, just after it has been demosaiced.
		/// </summary>
		ImageOpcodeList3 = 0xc74e,

		/// <summary>
		/// NoiseProfile describes the amount of noise in a raw Image Specifically, this tag models the amount of signal-dependent photon (shot) noise and signal-independent sensor readout noise, two common sources of noise in raw images. The model assumes that the noise is white and spatially independent, ignoring fixed pattern effects and other sources of noise (e.g., pixel response non-uniformity, spatially-dependent thermal effects, etc.).
		/// </summary>
		ImageNoiseProfile =
			0xc761,

		/// <summary>
		/// Exposure time, given in seconds (sec).
		/// </summary>
		PhotoExposureTime = 0x829a,

		/// <summary>
		/// The F number.
		/// </summary>
		PhotoFNumber = 0x829d,

		/// <summary>
		/// The class of the program used by the camera to set exposure when the picture is taken.
		/// </summary>
		PhotoExposureProgram = 0x8822,

		/// <summary>
		/// Indicates the spectral sensitivity of each channel of the camera used. The tag value is an ASCII string compatible with the standard developed by the ASTM Technical Committee.
		/// </summary>
		PhotoSpectralSensitivity =
			0x8824,

		/// <summary>
		/// Indicates the ISO Speed and ISO Latitude of the camera or input device as specified in ISO 12232.
		/// </summary>
		PhotoIsoSpeedRatings = 0x8827,

		/// <summary>
		/// Indicates the Opto-Electoric Conversion Function (OECF) specified in ISO 14524. <OECF> is the relationship between the camera optical input and the image values.
		/// </summary>
		PhotoOecf =
			0x8828,

		/// <summary>
		/// The SensitivityType tag indicates PhotographicSensitivity tag. which one of the parameters of ISO12232 is the Although it is an optional tag, it should be recorded when a PhotographicSensitivity tag is recorded. Value = 4, 5, 6, or 7 may be used in case that the values of plural parameters are the same.
		/// </summary>
		PhotoSensitivityType =
			0x8830,

		/// <summary>
		/// This tag indicates the standard output sensitivity value of a camera or input device defined in ISO 12232. When recording this tag, the PhotographicSensitivity and SensitivityType tags shall also be recorded.
		/// </summary>
		PhotoStandardOutputSensitivity =
			0x8831,

		/// <summary>
		/// This tag indicates the recommended exposure index value of a camera or input device defined in ISO 12232. When recording this tag, the PhotographicSensitivity and SensitivityType tags shall also be recorded.
		/// </summary>
		PhotoRecommendedExposureIndex =
			0x8832,

		/// <summary>
		/// This tag indicates the ISO speed value of a camera or input device that is defined in ISO 12232. When recording this tag, the PhotographicSensitivity and SensitivityType tags shall also be recorded.
		/// </summary>
		PhotoIsoSpeed =
			0x8833,

		/// <summary>
		/// This tag indicates the ISO speed latitude yyy value of a camera or input device that is defined in ISO 12232. However, this tag shall not be recorded without ISOSpeed and ISOSpeedLatitudezzz.
		/// </summary>
		PhotoIsoSpeedLatitudeyyy =
			0x8834,

		/// <summary>
		/// This tag indicates the ISO speed latitude zzz value of a camera or input device that is defined in ISO 12232. However, this tag shall not be recorded without ISOSpeed and ISOSpeedLatitudeyyy.
		/// </summary>
		PhotoIsoSpeedLatitudezzz =
			0x8835,

		/// <summary>
		/// The version of this standard supported. Nonexistence of this field is taken to mean nonconformance to the standard.
		/// </summary>
		PhotoExifVersion = 0x9000,

		/// <summary>
		/// The date and time when the original image data was generated. For a digital still camera the date and time the picture was taken are recorded.
		/// </summary>
		PhotoDateTimeOriginal =
			0x9003,

		/// <summary>
		/// The date and time when the image was stored as digital data.
		/// </summary>
		PhotoDateTimeDigitized = 0x9004,

		/// <summary>
		/// Information specific to compressed data. The channels of each component are arranged in order from the 1st component to the 4th. For uncompressed data the data arrangement is given in the <PhotometricInterpretation> tag. However, since <PhotometricInterpretation> can only express the order of Y, Cb and Cr, this tag is provided for cases when compressed data uses components other than Y, Cb, and Cr and to enable support of other sequences.
		/// </summary>
		PhotoComponentsConfiguration =
			0x9101,

		/// <summary>
		/// Information specific to compressed data. The compression mode used for a compressed image is indicated in unit bits per pixel.
		/// </summary>
		PhotoCompressedBitsPerPixel = 0x9102,

		/// <summary>
		/// Shutter speed. The unit is the APEX (Additive System of Photographic Exposure) setting.
		/// </summary>
		PhotoShutterSpeedValue = 0x9201,

		/// <summary>
		/// The lens aperture. The unit is the APEX value.
		/// </summary>
		PhotoApertureValue = 0x9202,

		/// <summary>
		/// The value of brightness. The unit is the APEX value. Ordinarily it is given in the range of -99.99 to 99.99.
		/// </summary>
		PhotoBrightnessValue = 0x9203,

		/// <summary>
		/// The exposure bias. The units is the APEX value. Ordinarily it is given in the range of -99.99 to 99.99.
		/// </summary>
		PhotoExposureBiasValue = 0x9204,

		/// <summary>
		/// The smallest F number of the lens. The unit is the APEX value. Ordinarily it is given in the range of 00.00 to 99.99, but it is not limited to this range.
		/// </summary>
		PhotoMaxApertureValue =
			0x9205,

		/// <summary>
		/// The distance to the subject, given in meters.
		/// </summary>
		PhotoSubjectDistance = 0x9206,

		/// <summary>
		/// The metering mode.
		/// </summary>
		PhotoMeteringMode = 0x9207,

		/// <summary>
		/// The kind of light source.
		/// </summary>
		PhotoLightSource = 0x9208,

		/// <summary>
		/// This tag is recorded when an image is taken using a strobe light (flash).
		/// </summary>
		PhotoFlash = 0x9209,

		/// <summary>
		/// The actual focal length of the lens, in mm. Conversion is not made to the focal length of a 35 mm film camera.
		/// </summary>
		PhotoFocalLength = 0x920a,

		/// <summary>
		/// This tag indicates the location and area of the main subject in the overall scene.
		/// </summary>
		PhotoSubjectArea = 0x9214,

		/// <summary>
		/// A tag for manufacturers of Exif writers to record any desired information. The contents are up to the manufacturer.
		/// </summary>
		PhotoMakerNote = 0x927c,

		/// <summary>
		/// A tag for Exif users to write keywords or comments on the image besides those in <ImageDescription>, and without the character code limitations of the <ImageDescription> tag.
		/// </summary>
		PhotoUserComment =
			0x9286,

		/// <summary>
		/// A tag used to record fractions of seconds for the <DateTime> tag.
		/// </summary>
		PhotoSubSecTime = 0x9290,

		/// <summary>
		/// A tag used to record fractions of seconds for the <DateTimeOriginal> tag.
		/// </summary>
		PhotoSubSecTimeOriginal = 0x9291,

		/// <summary>
		/// A tag used to record fractions of seconds for the <DateTimeDigitized> tag.
		/// </summary>
		PhotoSubSecTimeDigitized = 0x9292,

		/// <summary>
		/// The FlashPix format version supported by a FPXR file.
		/// </summary>
		PhotoFlashpixVersion = 0xa000,

		/// <summary>
		/// The color space information tag is always recorded as the color space specifier. Normally sRGB is used to define the color space based on the PC monitor conditions and environment. If a color space other than sRGB is used, Uncalibrated is set. Image data recorded as Uncalibrated can be treated as sRGB when it is converted to FlashPix.
		/// </summary>
		PhotoColorSpace =
			0xa001,

		/// <summary>
		/// Information specific to compressed data. When a compressed file is recorded, the valid width of the meaningful image must be recorded in this tag, whether or not there is padding data or a restart marker. This tag should not exist in an uncompressed file.
		/// </summary>
		PhotoPixelXDimension =
			0xa002,

		/// <summary>
		/// Information specific to compressed data. When a compressed file is recorded, the valid height of the meaningful image must be recorded in this tag, whether or not there is padding data or a restart marker. This tag should not exist in an uncompressed file. Since data padding is unnecessary in the vertical direction, the number of lines recorded in this valid image height tag will in fact be the same as that recorded in the SOF.
		/// </summary>
		PhotoPixelYDimension =
			0xa003,

		/// <summary>
		/// This tag is used to record the name of an audio file related to the image data. The only relational information recorded here is the Exif audio file name and extension (an ASCII string consisting of 8 characters + '.' + 3 characters). The path is not recorded.
		/// </summary>
		PhotoRelatedSoundFile =
			0xa004,

		/// <summary>
		/// Interoperability IFD is composed of tags which stores the information to ensure the Interoperability and pointed by the following tag located in Exif IFD. The Interoperability structure of Interoperability IFD is the same as TIFF defined IFD structure but does not contain the image data characteristically compared with normal TIFF IFD.
		/// </summary>
		PhotoInteroperabilityTag =
			0xa005,

		/// <summary>
		/// Indicates the strobe energy at the time the image is captured, as measured in Beam Candle Power Seconds (BCPS).
		/// </summary>
		PhotoFlashEnergy = 0xa20b,

		/// <summary>
		/// This tag records the camera or input device spatial frequency table and SFR values in the direction of image width, image height, and diagonal direction, as specified in ISO 12233.
		/// </summary>
		PhotoSpatialFrequencyResponse =
			0xa20c,

		/// <summary>
		/// Indicates the number of pixels in the image width (X) direction per <FocalPlaneResolutionUnit> on the camera focal plane.
		/// </summary>
		PhotoFocalPlaneXResolution = 0xa20e,

		/// <summary>
		/// Indicates the number of pixels in the image height (V) direction per <FocalPlaneResolutionUnit> on the camera focal plane.
		/// </summary>
		PhotoFocalPlaneYResolution = 0xa20f,

		/// <summary>
		/// Indicates the unit for measuring <FocalPlaneXResolution> and <FocalPlaneYResolution>. This value is the same as the <ResolutionUnit>.
		/// </summary>
		PhotoFocalPlaneResolutionUnit =
			0xa210,

		/// <summary>
		/// Indicates the location of the main subject in the scene. The value of this tag represents the pixel at the center of the main subject relative to the left edge, prior to rotation processing as per the <Rotation> tag. The first value indicates the X column number and second indicates the Y row number.
		/// </summary>
		PhotoSubjectLocation =
			0xa214,

		/// <summary>
		/// Indicates the exposure index selected on the camera or input device at the time the image is captured.
		/// </summary>
		PhotoExposureIndex = 0xa215,

		/// <summary>
		/// Indicates the image sensor type on the camera or input device.
		/// </summary>
		PhotoSensingMethod = 0xa217,

		/// <summary>
		/// Indicates the image source. If a DSC recorded the image, this tag value of this tag always be set to 3, indicating that the image was recorded on a DSC.
		/// </summary>
		PhotoFileSource =
			0xa300,

		/// <summary>
		/// Indicates the type of scene. If a DSC recorded the image, this tag value must always be set to 1, indicating that the image was directly photographed.
		/// </summary>
		PhotoSceneType =
			0xa301,

		/// <summary>
		/// Indicates the color filter array (CFA) geometric pattern of the image sensor when a one-chip color area sensor is used. It does not apply to all sensing methods.
		/// </summary>
		PhotoCfaPattern =
			0xa302,

		/// <summary>
		/// This tag indicates the use of special processing on image data, such as rendering geared to output. When special processing is performed, the reader is expected to disable or minimize any further processing.
		/// </summary>
		PhotoCustomRendered =
			0xa401,

		/// <summary>
		/// This tag indicates the exposure mode set when the image was shot. In auto-bracketing mode, the camera shoots a series of frames of the same scene at different exposure settings.
		/// </summary>
		PhotoExposureMode =
			0xa402,

		/// <summary>
		/// This tag indicates the white balance mode set when the image was shot.
		/// </summary>
		PhotoWhiteBalance = 0xa403,

		/// <summary>
		/// This tag indicates the digital zoom ratio when the image was shot. If the numerator of the recorded value is 0, this indicates that digital zoom was not used.
		/// </summary>
		PhotoDigitalZoomRatio =
			0xa404,

		/// <summary>
		/// This tag indicates the equivalent focal length assuming a 35mm film camera, in mm. A value of 0 means the focal length is unknown. Note that this tag differs from the <FocalLength> tag.
		/// </summary>
		PhotoFocalLengthIn35MmFilm =
			0xa405,

		/// <summary>
		/// This tag indicates the type of scene that was shot. It can also be used to record the mode in which the image was shot. Note that this differs from the <SceneType> tag.
		/// </summary>
		PhotoSceneCaptureType =
			0xa406,

		/// <summary>
		/// This tag indicates the degree of overall image gain adjustment.
		/// </summary>
		PhotoGainControl = 0xa407,

		/// <summary>
		/// This tag indicates the direction of contrast processing applied by the camera when the image was shot.
		/// </summary>
		PhotoContrast = 0xa408,

		/// <summary>
		/// This tag indicates the direction of saturation processing applied by the camera when the image was shot.
		/// </summary>
		PhotoSaturation = 0xa409,

		/// <summary>
		/// This tag indicates the direction of sharpness processing applied by the camera when the image was shot.
		/// </summary>
		PhotoSharpness = 0xa40a,

		/// <summary>
		/// This tag indicates information on the picture-taking conditions of a particular camera model. The tag is used only to indicate the picture-taking conditions in the reader.
		/// </summary>
		PhotoDeviceSettingDescription =
			0xa40b,

		/// <summary>
		/// This tag indicates the distance to the subject.
		/// </summary>
		PhotoSubjectDistanceRange = 0xa40c,

		/// <summary>
		/// This tag indicates an identifier assigned uniquely to each Image It is recorded as an ASCII string equivalent to hexadecimal notation and 128-bit fixed length.
		/// </summary>
		PhotoImageUniqueID =
			0xa420,

		/// <summary>
		/// This tag records the owner of a camera used in photography as an ASCII string.
		/// </summary>
		PhotoCameraOwnerName = 0xa430,

		/// <summary>
		/// This tag records the serial number of the body of the camera that was used in photography as an ASCII string.
		/// </summary>
		PhotoBodySerialNumber = 0xa431,

		/// <summary>
		/// This tag notes minimum focal length, maximum focal length, minimum F number in the minimum focal length, and minimum F number in the maximum focal length, which are specification information for the lens that was used in photography. When the minimum F number is unknown, the notation is 0/0
		/// </summary>
		PhotoLensSpecification =
			0xa432,

		/// <summary>
		/// This tag records the lens manufactor as an ASCII string.
		/// </summary>
		PhotoLensMake = 0xa433,

		/// <summary>
		/// This tag records the lens's model name and model number as an ASCII string.
		/// </summary>
		PhotoLensModel = 0xa434,

		/// <summary>
		/// This tag records the serial number of the interchangeable lens that was used in photography as an ASCII string.
		/// </summary>
		PhotoLensSerialNumber = 0xa435,

		/// <summary>
		/// Indicates the identification of the Interoperability rule. Use "R98" for stating ExifR98 Rules. Four bytes used including the termination code (NULL). see the separate volume of Recommended Exif Interoperability Rules (ExifR98) for other tags used for ExifR98.
		/// </summary>
		IopInteroperabilityIndex =
			0x0001,

		/// <summary>
		/// Interoperability version
		/// </summary>
		IopInteroperabilityVersion = 0x0002,

		/// <summary>
		/// File format of image file
		/// </summary>
		IopRelatedImageFileFormat = 0x1000,

		/// <summary>
		/// Image width
		/// </summary>
		IopRelatedImageWidth = 0x1001,

		/// <summary>
		/// Image height
		/// </summary>
		IopRelatedImageLength = 0x1002,

		/// <summary>
		/// Indicates the version of <GPSInfoIFD>. The version is given as 2.0.0.0. This tag is mandatory when <GPSInfo> tag is present. (Note: The <GPSVersionID> tag is given in bytes, unlike the <ExifVersion> tag. When the version is 2.0.0.0, the tag value is 02000000.H).
		/// </summary>
		GpsVersionID =
			0x0000,

		/// <summary>
		/// Indicates whether the latitude is north or south latitude. The ASCII value 'N' indicates north latitude, and 'S' is south latitude.
		/// </summary>
		GpsLatitudeRef = 0x0001,

		/// <summary>
		/// Indicates the latitude. The latitude is expressed as three RATIONAL values giving the degrees, minutes, and seconds, respectively. When degrees, minutes and seconds are expressed, the format is dd/1,mm/1,ss/1. When degrees and minutes are used and, for example, fractions of minutes are given up to two decimal places, the format is dd/1,mmmm/100,0/1.
		/// </summary>
		GpsLatitude =
			0x0002,

		/// <summary>
		/// Indicates whether the longitude is east or west longitude. ASCII 'E' indicates east longitude, and 'W' is west longitude.
		/// </summary>
		GpsLongitudeRef = 0x0003,

		/// <summary>
		/// Indicates the longitude. The longitude is expressed as three RATIONAL values giving the degrees, minutes, and seconds, respectively. When degrees, minutes and seconds are expressed, the format is ddd/1,mm/1,ss/1. When degrees and minutes are used and, for example, fractions of minutes are given up to two decimal places, the format is ddd/1,mmmm/100,0/1.
		/// </summary>
		GpsLongitude =
			0x0004,

		/// <summary>
		/// Indicates the altitude used as the reference altitude. If the reference is sea level and the altitude is above sea level, 0 is given. If the altitude is below sea level, a value of 1 is given and the altitude is indicated as an absolute value in the GSPAltitude tag. The reference unit is meters. Note that this tag is BYTE type, unlike other reference tags.
		/// </summary>
		GpsAltitudeRef =
			0x0005,

		/// <summary>
		/// Indicates the altitude based on the reference in GPSAltitudeRef. Altitude is expressed as one RATIONAL value. The reference unit is meters.
		/// </summary>
		GpsAltitude = 0x0006,

		/// <summary>
		/// Indicates the time as UTC (Coordinated Universal Time). <TimeStamp> is expressed as three RATIONAL values giving the hour, minute, and second (atomic clock).
		/// </summary>
		GpsTimeStamp =
			0x0007,

		/// <summary>
		/// Indicates the GPS satellites used for measurements. This tag can be used to describe the number of satellites, their ID number, angle of elevation, azimuth, SNR and other information in ASCII notation. The format is not specified. If the GPS receiver is incapable of taking measurements, value of the tag is set to NULL.
		/// </summary>
		GpsSatellites =
			0x0008,

		/// <summary>
		/// Indicates the status of the GPS receiver when the image is recorded. "A" means measurement is in progress, and "V" means the measurement is Interoperability.
		/// </summary>
		GpsStatus =
			0x0009,

		/// <summary>
		/// Indicates the GPS measurement mode. "2" means two-dimensional measurement and "3" means three-dimensional measurement is in progress.
		/// </summary>
		GpsMeasureMode = 0x000a,

		/// <summary>
		/// Indicates the GPS DOP (data degree of precision). An HDOP value is written during two-dimensional measurement, and PDOP during three-dimensional measurement.
		/// </summary>
		Gpsdop =
			0x000b,

		/// <summary>
		/// Indicates the unit used to express the GPS receiver speed of movement. "K" "M" and "N" represents kilometers per hour, miles per hour, and knots.
		/// </summary>
		GpsSpeedRef =
			0x000c,

		/// <summary>
		/// Indicates the speed of GPS receiver movement.
		/// </summary>
		GpsSpeed = 0x000d,

		/// <summary>
		/// Indicates the reference for giving the direction of GPS receiver movement. "T" denotes true direction and "M" is magnetic direction.
		/// </summary>
		GpsTrackRef = 0x000e,

		/// <summary>
		/// Indicates the direction of GPS receiver movement. The range of values is from 0.00 to 359.99.
		/// </summary>
		GpsTrack = 0x000f,

		/// <summary>
		/// Indicates the reference for giving the direction of the image when it is captured. "T" denotes true direction and "M" is magnetic direction.
		/// </summary>
		GpsImgDirectionRef =
			0x0010,

		/// <summary>
		/// Indicates the direction of the image when it was captured. The range of values is from 0.00 to 359.99.
		/// </summary>
		GpsImgDirection = 0x0011,

		/// <summary>
		/// Indicates the geodetic survey data used by the GPS receiver. If the survey data is restricted to Japan, the value of this tag is "TOKYO" or "WGS-84".
		/// </summary>
		GpsMapDatum =
			0x0012,

		/// <summary>
		/// Indicates whether the latitude of the destination point is north or south latitude. The ASCII value "N" indicates north latitude, and "S" is south latitude.
		/// </summary>
		GpsDestLatitudeRef =
			0x0013,

		/// <summary>
		/// Indicates the latitude of the destination point. The latitude is expressed as three RATIONAL values giving the degrees, minutes, and seconds, respectively. If latitude is expressed as degrees, minutes and seconds, a typical format would be dd/1,mm/1,ss/1. When degrees and minutes are used and, for example, fractions of minutes are given up to two decimal places, the format would be dd/1,mmmm/100,0/1.
		/// </summary>
		GpsDestLatitude =
			0x0014,

		/// <summary>
		/// Indicates whether the longitude of the destination point is east or west longitude. ASCII "E" indicates east longitude, and "W" is west longitude.
		/// </summary>
		GpsDestLongitudeRef =
			0x0015,

		/// <summary>
		/// Indicates the longitude of the destination point. The longitude is expressed as three RATIONAL values giving the degrees, minutes, and seconds, respectively. If longitude is expressed as degrees, minutes and seconds, a typical format would be ddd/1,mm/1,ss/1. When degrees and minutes are used and, for example, fractions of minutes are given up to two decimal places, the format would be ddd/1,mmmm/100,0/1.
		/// </summary>
		GpsDestLongitude =
			0x0016,

		/// <summary>
		/// Indicates the reference used for giving the bearing to the destination point. "T" denotes true direction and "M" is magnetic direction.
		/// </summary>
		GpsDestBearingRef = 0x0017,

		/// <summary>
		/// Indicates the bearing to the destination point. The range of values is from 0.00 to 359.99.
		/// </summary>
		GpsDestBearing = 0x0018,

		/// <summary>
		/// Indicates the unit used to express the distance to the destination point. "K", "M" and "N" represent kilometers, miles and knots.
		/// </summary>
		GpsDestDistanceRef = 0x0019,

		/// <summary>
		/// Indicates the distance to the destination point.
		/// </summary>
		GpsDestDistance = 0x001a,

		/// <summary>
		/// A character string recording the name of the method used for location finding. The first byte indicates the character code used, and this is followed by the name of the method.
		/// </summary>
		GpsProcessingMethod =
			0x001b,

		/// <summary>
		/// A character string recording the name of the GPS area. The first byte indicates the character code used, and this is followed by the name of the GPS area.
		/// </summary>
		GpsAreaInformation =
			0x001c,

		/// <summary>
		/// A character string recording date and time information relative to UTC (Coordinated Universal Time). The format is "YYYY:MM:DD.".
		/// </summary>
		GpsDateStamp = 0x001d,

		/// <summary>
		/// Indicates whether differential correction is applied to the GPS receiver.
		/// </summary>
		GpsDifferential = 0x001e,

		/// <summary>
		/// Missing
		/// </summary>
		ThumbnailImageHeight = 0x5021,

		ThumbnailImageWidth = 0x5020,

		ThumbnailBitsPerSample = 0x5022,

		ThumbnailCompression = 0x5023,

		ThumbnailPhotometricInterpretation = 0x5024,

		ThumbnailImageDescription = 0x5025,

		ThumbnailMake = 0x5026,

		ThumbnailModel = 0x5027,

		ThumbnailStripOffsets = 0x5028,

		ThumbnailOrientation = 0x5029,

		ThumbnailSamplesPerPixel = 0x502A,

		ThumbnailRowsPerStrip = 0x502B,

		ThumbnailStripBytesCount = 0x502C,

		ThumbnailXResolution = 0x502D,

		ThumbnailYResolution = 0x502E,

		ThumbnailPlanarConfig = 0x502F,

		ThumbnailResolutionUnit = 0x5030,

		ThumbnailTransferFunction = 0x5031,

		ThumbnailSoftware = 0x5032,

		ThumbnailDateTime = 0x5033,

		ThumbnailArtist = 0x5034,

		ThumbnailWhitePoint = 0x5035,

		ThumbnailPrimaryChromaticities = 0x5036,

		ThumbnailYCbCrCoefficients = 0x5037,

		ThumbnailYCbCrSubSampling = 0x5038,

		ThumbnailYCbCrPositioning = 0x5039,

		ThumbnailReferenceBlackWhite = 0x503A,

		ThumbnailCopyright = 0x503B,

		ThumbnailColorDepth = 0x5015,

		ThumbnailCompressedSize = 0x5019,

		ThumbnailData = 0x501B,

		ThumbnailFormat = 0x5012,

		ThumbnailHeight = 0x5014,

		ThumbnailPlanes = 0x5016,

		ThumbnailRawBytes = 0x5017,

		Thumbnail = 0x5018,

		ThumbnailWidth = 0x5013,

		JpegQuality = 0x5010,

		JpegRestartInterval = 0x0203,

		LoopCount = 0x5101,

		LuminanceTable = 0x5090,

		PaletteHistogram = 0x5113,

		PixelPerUnitX = 0x5111,

		PixelPerUnitY = 0x5112,

		PixelUnit = 0x5110,

		PrintFlags = 0x5005,

		PrintFlagsBleedWidth = 0x5008,

		PrintFlagsBleedWidthScale = 0x5009,

		PrintFlagsCrop = 0x5007,

		PrintFlagsVersion = 0x5006,

		ResolutionXLengthUnit = 0x5003,

		ResolutionXUnit = 0x5001,

		ResolutionYLengthUnit = 0x5004,

		ResolutionYUnit = 0x5002,

		InteroperabilityIndex = 0x5041,

		ChrominanceTable = 0x5091,

		ColorTransferFunction = 0x501A,

		GridSize = 0x5011,

		HalftoneDegree = 0x500C,

		HalftoneLpi = 0x500A,

		HalftoneLpiUnit = 0x500B,

		HalftoneMisc = 0x500E,

		HalftoneScreen = 0x500F,

		HalftoneShape = 0x500D
	}

	public static Matrix3X2 ComputeForwardTransform( IList<Point> baselineLocations, IList<Point> registerLocations ) {
		if ( ( baselineLocations.Count < 3 ) || ( registerLocations.Count < 3 ) ) {
			throw new( "Unable to compute the forward transform. A minimum of 3 control point pairs are required." );
		}

		if ( baselineLocations.Count != registerLocations.Count ) {
			throw new( "Unable to compute the forward transform. The number of control point pairs in baseline and registration results must be equal." );
		}

		// To compute Transform = ((X^T * X)^-1 * X^T)U = (X^T * X)^-1 (X^T * U)

		// X^T * X = [ Sum(x_i^2) Sum(x_i*y_i) Sum(x_i) ] [ Sum(x_i*y_i) Sum(y_i^2) Sum(y_i) ] [ Sum(x_i) Sum(y_i) Sum(1)=n ]

		// X^T * U = [ Sum(x_i*u_i) Sum(x_i*v_i) ] [ Sum(y_i*u_i) Sum(y_i*v_i) ] [ Sum(u_i) Sum(v_i) ]

		Double a = 0, b = 0, c = 0, d = 0, e = 0, f = 0, g = 0, h = 0, n = baselineLocations.Count;
		Double p = 0, q = 0, r = 0, s = 0, t = 0, u = 0;

		for ( var i = 0; i < n; i++ ) {
			// Compute sum of squares for X^T * X
			a += baselineLocations[i].X * baselineLocations[i].X;
			b += baselineLocations[i].X * baselineLocations[i].Y;
			c += baselineLocations[i].X;
			d += baselineLocations[i].X * baselineLocations[i].Y;
			e += baselineLocations[i].Y * baselineLocations[i].Y;
			f += baselineLocations[i].Y;
			g += baselineLocations[i].X;
			h += baselineLocations[i].Y;

			// Compute sum of squares for X^T * U
			p += baselineLocations[i].X * registerLocations[i].X;
			q += baselineLocations[i].X * registerLocations[i].Y;
			r += baselineLocations[i].Y * registerLocations[i].X;
			s += baselineLocations[i].Y * registerLocations[i].Y;
			t += registerLocations[i].X;
			u += registerLocations[i].Y;
		}

		// Create matrices from the coefficients
		var uMat = new Matrix3X2( p, q, r, s, t, u );
		var xMat = new Matrix3X3( a, b, c, d, e, f, g, h, n );

		// Invert X
		var xInv = xMat.Inverse();

		// Perform the multiplication to get the transform
		uMat.Multiply( xInv );

		// Matrix uMat now holds the image registration transform to go from the current result to baseline
		return uMat;
	}

	public static DateTime? GetProperteryAsDateTime( this PropertyItem? item ) {
		if ( item is null ) {
			return default( DateTime? );
		}

		var value = Encoding.ASCII.GetString( item.Value );

		if ( value.EndsWith( "\0", StringComparison.Ordinal ) ) {
			value = value.Replace( "\0", String.Empty );
		}

		if ( value == "0000:00:00 00:00:00" ) {
			return default( DateTime? );
		}

		if ( DateTime.TryParse( value, out var result ) ) {
			return result;
		}

		if ( DateTime.TryParseExact( value, "yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out result ) ) {
			return result;
		}

		return default( DateTime? );
	}

	/// <summary>Returns true if the date is 'old' enough.</summary>
	/// <param name="dateTime"></param>
	/// <param name="byYears"> </param>
	/// <remarks>Any time travelers in the house?</remarks>
	public static Boolean IsDateNotTooNew( this DateTime dateTime, Int32 byYears = 5 ) => dateTime.Year <= DateTime.UtcNow.AddYears( byYears ).Year;

	/// <summary>Returns true if the date is 'recent' enough.</summary>
	/// <param name="dateTime"></param>
	public static Boolean IsDateRecentEnough( this DateTime dateTime ) => dateTime.Year >= 1825;
}