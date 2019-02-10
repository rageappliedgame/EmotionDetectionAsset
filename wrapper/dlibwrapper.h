/*
* Copyright 2016 Open University of the Netherlands
*
* Cite this work as:
* Bahreini, K., van der Vegt, W. & Westera, W. Multimedia Tools and Applications (2019). https://doi.org/10.1007/s11042-019-7250-z
* 
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* This project has received funding from the European Union’s Horizon
* 2020 research and innovation programme under grant agreement No 644187.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

#pragma once

#include <WinDef.h>

// TEST START
#include <dlib/image_processing/generic_image.h>
#include <dlib/image_processing.h>
#include <dlib/image_io.h>
#include <fstream>
#include <string>
// TEST END
// 
#define verbose false

/// <summary>
/// Init the face detector.
/// </summary>
extern "C" __declspec(dllexport) void InitDetector(void);

/// <summary>
/// Init database.
/// </summary>
///
/// <param name="fname">	[in,out] If non-null, filename of the file. </param>
extern "C" __declspec(dllexport) void InitDatabase(char* fname);

/// <summary>
/// Set the Image to detect faces and emotions in to a raw BMP.
/// </summary>
///
/// <param name="bytes">	[in,out] If non-null, the bytes. </param>
/// <param name="size"> 	The size. </param>
///
/// <returns>
/// True if it succeeds, false if it fails.
/// </returns>
extern "C" __declspec(dllexport) bool SetImageToBmp(byte* bytes, int size);

/// <summary>
/// Set the Image to detect faces and emotions in to an RGBA Array.
/// </summary>
///
/// <param name="bytes"> 	[in,out] If non-null, the bytes. </param>
/// <param name="width"> 	The width. </param>
/// <param name="height">	The height. </param>
///
/// <returns>
/// True if it succeeds, false if it fails.
/// </returns>
extern "C" __declspec(dllexport) bool SetImageToRGBA(byte* bytes, int width, int height, bool flip);

/// <summary>
/// Set the Image to detect faces and emotions in to an RGB Array.
/// </summary>
///
/// <param name="bytes"> 	[in,out] If non-null, the bytes. </param>
/// <param name="width"> 	The width. </param>
/// <param name="height">	The height. </param>
///
/// <returns>
/// True if it succeeds, false if it fails.
/// </returns>
extern "C" __declspec(dllexport) bool SetImageToRGB(byte* bytes, int width, int height, bool flip);

/// <summary>
/// Detect faces in an image.
/// 
/// see https://limbioliong.wordpress.com/2011/08/14/returning-an-array-of-strings-from-c-to-c-part-1
/// </summary>
///
/// <param name="faces">		[in,out] If non-null, the faces. </param>
/// <param name="facecount">	[in,out] If non-null, the facecount. </param>
extern "C"	__declspec(dllexport) void DetectFaces(RECT*** faces, int* facecount);

/// <summary>
/// Detect faces in an image.
/// 
/// see https://limbioliong.wordpress.com/2011/08/14/returning-an-array-of-strings-from-c-to-c-part-1
/// </summary>
///
/// <param name="img">			[in,out] The image. </param>
/// <param name="size">			The size. </param>
/// <param name="faces">		[in,out] If non-null, the faces. </param>
/// <param name="facecount">	[in,out] If non-null, the facecount. </param>
extern "C"	__declspec(dllexport) void DetectFacesOld(byte* img, int size, RECT*** faces, int* facecount);

/// <summary>
/// Detect landmarks in a section of an image.
/// </summary>
///
/// <param name="face">			The RECT to process. </param>
/// <param name="landmarks">	[in,out] If non-null, the landmarks. </param>
/// <param name="markcount">	[in,out] If non-null, the markcount. </param>
extern "C"	__declspec(dllexport) void DetectLandmarks(RECT face, POINT*** landmarks, int* markcount);

// TEST START

// 
// See https://www.codeproject.com/Questions/426387/Cplusplus-d-array-to-bitmap for this an dother examples

// TEST START

//-------------------------------------------------------------------------- 
// This little helper is to write little-endian values to file.
//
struct lwrite
{
	unsigned long value;
	unsigned      size;
	lwrite(unsigned long value, unsigned size) :
		value(value), size(size)
	{ }
};

//--------------------------------------------------------------------------
inline std::ostream& operator << (std::ostream& outs, const lwrite& v)
{
	unsigned long value = v.value;
	for (unsigned cntr = 0; cntr < v.size; cntr++, value >>= 8)
		outs.put(static_cast <char> (value & 0xFF));
	return outs;
}

//extern bool intarray2bmp(
//	const std::string& filename,
//	dlib::array2d<dlib::rgb_pixel> &intarray,
//	unsigned           rows,
//	unsigned           columns);

//--------------------------------------------------------------------------
// Take an array2d<dlib::rgb_pixel> array and convert it into a color image.
//
//template <typename IntType>
inline bool intarray2bmp(
	const std::string& filename,
	dlib::array2d<dlib::rgb_pixel> &intarray,
	unsigned           rows,
	unsigned           columns) {
	// This is the difference between each color based upon
	// the number of distinct values in the input array.
	//double granularity = 360.0 / ((double)(max_value - min_value) + 1);

	// Open the output BMP file
	std::ofstream f(filename.c_str(),
		std::ios::out | std::ios::trunc | std::ios::binary);
	if (!f) return false;

	// Some basic
	unsigned long headers_size = 14  // sizeof( BITMAPFILEHEADER )
		+ 40; // sizeof( BITMAPINFOHEADER )
	unsigned long padding_size = (4 - ((columns * 3) % 4)) % 4;
	unsigned long pixel_data_size = rows * ((columns * 3) + padding_size);

	// Write the BITMAPFILEHEADER
	f.put('B').put('M');                           // bfType
	f << lwrite(headers_size + pixel_data_size, 4);  // bfSize
	f << lwrite(0, 2);  // bfReserved1
	f << lwrite(0, 2);  // bfReserved2
	f << lwrite(headers_size, 4);  // bfOffBits

								   // Write the BITMAPINFOHEADER
	f << lwrite(40, 4);  // biSize
	f << lwrite(columns, 4);  // biWidth
	f << lwrite(rows, 4);  // biHeight
	f << lwrite(1, 2);  // biPlanes
	f << lwrite(24, 2);  // biBitCount
	f << lwrite(0, 4);  // biCompression=BI_RGB
	f << lwrite(pixel_data_size, 4);  // biSizeImage
	f << lwrite(0, 4);  // biXPelsPerMeter
	f << lwrite(0, 4);  // biYPelsPerMeter
	f << lwrite(0, 4);  // biClrUsed
	f << lwrite(0, 4);  // biClrImportant

						// Write the pixel data
	for (unsigned row = rows; row; row--)           // bottom-to-top
	{
		for (unsigned col = 0; col < columns; col++)  // left-to-right
		{
			unsigned char red, green, blue;
#undef c
			red = intarray[row - 1][col].red;
			green = intarray[row - 1][col].green;
			blue = intarray[row - 1][col].blue;

			f
				.put((blue))
				.put((green))
				.put((red));
		}

		if (padding_size) {
			f << lwrite(0, padding_size);
		}
	}

	// All done!
	return f.good();
}

//#endif

// TEST END
