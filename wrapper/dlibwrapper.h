/*
* Copyright 2016 Open University of the Netherlands
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
