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

/*
	From dlib demo this code is based on:

	This face detector is made using the classic Histogram of Oriented
	Gradients (HOG) feature combined with a linear classifier, an image pyramid,
	and sliding window detection scheme.  The pose estimator was created by
	using dlib's implementation of the paper:
		One Millisecond Face Alignment with an Ensemble of Regression Trees by
		Vahid Kazemi and Josephine Sullivan, CVPR 2014
	and was trained on the iBUG 300-W face landmark dataset.

	Also, note that you can train your own models using dlib's machine learning
	tools.  See train_shape_predictor_ex.cpp to see an example.

	Finally, note that the face detector is fastest when compiled with at least
	SSE2 instructions enabled.  So if you are using a PC with an Intel or AMD
	chip then you should enable at least SSE2 instructions.  If you are using
	cmake to compile this program you can enable them by using one of the
	following commands when you create the build project:
		cmake path_to_dlib_root/examples -DUSE_SSE2_INSTRUCTIONS=ON
		cmake path_to_dlib_root/examples -DUSE_SSE4_INSTRUCTIONS=ON
		cmake path_to_dlib_root/examples -DUSE_AVX_INSTRUCTIONS=ON
	This will set the appropriate compiler options for GCC, clang, Visual
	Studio, or the Intel compiler.  If you are using another compiler then you
	need to consult your compiler's manual to determine how to enable these
	instructions.  Note that AVX is the fastest but requires a CPU from at least
	2011.  SSE4 is the next fastest and is supported by most current machines.
*/

#include <dlib/image_processing/generic_image.h>
#include <dlib/image_processing/frontal_face_detector.h>
#include <dlib/image_processing.h>
#include <dlib/image_io.h>
#include <iostream>
#include <crtdbg.h>

#include "dlibwrapper.h"

using namespace dlib;
using namespace std;

static void LOG2(char *fmt, char* data, double ms) {
	if (verbose) {
		_RPT2(_CRT_WARN, "%s %0.0f [ms]\n", data, ms);
	}
}

#ifndef speedtest__
#define speedtest__(data) for (long blockTime = NULL; (blockTime == NULL ? (blockTime = clock()) != NULL : false); LOG2("%s %0.0f [ms]\n",data, (double) 1000.0* (clock() - blockTime) / CLOCKS_PER_SEC))
#endif

static dlib::frontal_face_detector detector;

static shape_predictor sp;

static dlib::array2d<dlib::rgb_pixel> img;

// ----------------------------------------------------------------------------------------

/// <summary>
/// A membuf.
/// </summary>
struct membuf : std::streambuf {
	membuf(char* base, std::ptrdiff_t n) {
		this->setg(base, base, base + n);
	}
};

/// <summary>
/// We need a face detector.  We will use this to get bounding boxes for each face in an image.
/// </summary>
extern void InitDetector(void) {
	speedtest__("InitDetector: ")
	{
		if (verbose) {
			cout << "InitDetector: " << endl;
		}
		detector = dlib::get_frontal_face_detector();
	}
}

/// <summary>
/// And we also need a shape_predictor.  This is the tool that will predict face landmark
/// positions given an image and face bounding box.  Here we are just loading the model from the
/// shape_predictor_68_face_landmarks.dat file you gave as a command line argument.
/// </summary>
///
/// <param name="pszString">	[in,out] If non-null, the string. </param>
extern void InitDatabase(char* pszString) {
	speedtest__("InitDatabase: ")
	{
		if (verbose) {
			cout << "InitDatabase: '" << pszString << "'" << endl;
		}

		dlib::deserialize(pszString) >> sp;
	}
}

/// <summary>
/// Set the Image to detect faces and emotions in to a raw BMP.
/// </summary>
///
/// <param name="bytes">		[in,out] If non-null, the bytes. </param>
/// <param name="size">			The size. </param>
extern bool SetImageToBmp(byte* bytes, int size) {
	if (verbose) {
		cout << "SetImageToBmp: " << endl;

		_RPT1(_CRT_WARN, "Bitmap Size: %d\n", size);
	}

	std::streamsize ss = size;

	membuf sbuf((char*)bytes, ss);
	std::istream imgstream(&sbuf);
	imgstream.seekg(0);

	speedtest__("loading bitmap: ")
	{
		try {
			load_bmp(img, imgstream);

			//if (verbose) {
			//	for (int row = 0; row < img.nr(); row++) {
			//		rgb_pixel rp = img[row][0];
			//		_RPT4(_CRT_WARN, "row:%4d R:%3d G:%3d B:%3d\n", row, rp.red, rp.green, rp.blue);
			//	}
			//}
		}
		catch (exception e) {
			return false;
		}
	}

	return true;
}

/// <summary>
/// Set the Image to detect faces and emotions in to an RGB Array.
/// </summary>
///
/// <remarks>
/// Unity Textures have the 0,0 coordinate in the lowerleft corner unlike .net (topleft).
/// </remarks>
///
/// <param name="bytes"> 	[in,out] If non-null, the bytes. </param>
/// <param name="width"> 	The size. </param>
/// <param name="height">	The height. </param>
/// <param name="flip">  	True to flip image vertically. </param>
///
/// <returns>
/// True if it succeeds, false if it fails.
/// </returns>
extern bool SetImageToRGB(byte* bytes, int width, int height, bool flip) {
	std::vector<RECT> results;

	// Expect 3 bytes per pixel.
	int size = width * height * 3;

	img.set_size(height, width);

	if (verbose) {
		cout << "SetImageToRGB: " << endl;

		_RPT1(_CRT_WARN, "RGB Size: %d\n", size);
	}

	std::streamsize ss = size;

	membuf sbuf((char*)bytes, ss);
	std::istream imgstream(&sbuf);
	imgstream.seekg(0);

	streambuf& in = *imgstream.rdbuf();

	speedtest__("loading rgb: ")
	{
		unsigned char buf[3];

		int start = height - 1;

		for (int row = 0; row < height; row++) {

			int fr = flip ? start - row : row;

			for (int col = 0; col < width; col++) {
				if (in.sgetn(reinterpret_cast<char*>(buf), 3) != 3)
				{
					//throw image_load_error("bmp load error 21.8: file too short");
					return false;
				}

				rgb_pixel p;

				p.red = buf[0];
				p.green = buf[1];
				p.blue = buf[2];

				assign_pixel(img[fr][col], p);
			}
		}

		//if (verbose) {
		//	for (int row = 0; row < img.nr(); row++) {
		//		rgb_pixel rp = img[row][0];
		//		_RPT4(_CRT_WARN, "row:%4d R:%3d G:%3d B:%3d\n", row, rp.red, rp.green, rp.blue);
		//	}
		//}

		//intarray2bmp("dump.bmp", img, height, width);
	}

	return true;
}

/// <summary>
/// Set the Image to detect faces and emotions in to an RGBA Array.
/// </summary>
///
/// <remarks>
/// Unity Textures have the 0,0 coordinate in the lowerleft corner unlike .net (topleft).
/// </remarks>
///
/// <param name="bytes"> 	[in,out] If non-null, the bytes. </param>
/// <param name="width"> 	The size. </param>
/// <param name="height">	The height. If negative the image is flipped vertically. </param>
/// <param name="flip">  	True to flip image vertically. </param>
///
/// <returns>
/// True if it succeeds, false if it fails.
/// </returns>
extern bool SetImageToRGBA(byte* bytes, int width, int height, bool flip) {
	std::vector<RECT> results;

	img.set_size(height, width);

	// Expect 4 bytes per pixel.
	// 
	int size = width * height * 4;

	if (verbose) {
		cout << "SetImageToRGBA: " << endl;

		_RPT1(_CRT_WARN, "RGBA Size: %d\n", size);
	}

	std::streamsize ss = size;

	membuf sbuf((char*)bytes, ss);
	std::istream imgstream(&sbuf);
	imgstream.seekg(0);

	streambuf& in = *imgstream.rdbuf();

	speedtest__("loading rgba: ")
	{
		unsigned char buf[4];

		int start = height - 1;

		_RPT1(_CRT_WARN, "Flipping Image:%s\n", flip ? "true" : "false");

		// Ignore Alpha Information.
		for (int row = 0; row < height; row++) {

			int fr = flip ? start - row : row;

			for (int col = 0; col < width; col++) {
				if (in.sgetn(reinterpret_cast<char*>(buf), 4) != 4)
				{
					//throw image_load_error("bmp load error 21.8: file too short");
					return false;
				}

				rgb_pixel p;

				p.red = buf[0];
				p.green = buf[1];
				p.blue = buf[2];
				//p.alpha = buf[3];

				assign_pixel(img[fr][col], p);
			}
		}

		//if (verbose) {
		//	for (int row = 0; row < img.nr(); row++) {
		//		rgb_pixel rp = img[row][0];
		//		_RPT4(_CRT_WARN, "row:%4d R:%3d G:%3d B:%3d\n", row, rp.red, rp.green, rp.blue);
		//	}
		//}

		//intarray2bmp("dump.bmp", img, height, width);
	}

	return true;
}

/// <summary>
/// Detect faces.
/// </summary>
///
/// <param name="faces">		[in,out] If non-null, the faces. </param>
/// <param name="facecount">	[in,out] If non-null, the facecount. </param>
extern void DetectFaces(RECT*** faces, int* facecount) {
	std::vector<RECT> results;

	speedtest__("DetectFaces: ")
	{
		// Make the image larger so we can detect small faces.
		// http://stackoverflow.com/questions/32049763/implementing-dlib-pyramid-up-with-cv-image
		// http://docs.opencv.org/3.1.0/d4/d86/group__imgproc__filter.html#gada75b59bdaaca411ed6fee10085eb784

		// pyramid_up(img);

		std::vector<dlib::rectangle> dets;

		speedtest__("detect faces: ")
		{
			dets = detector(img);
		}

		if (verbose) {
			_RPT1(_CRT_WARN, "Number of faces detected: %d\n", dets.size());
			cout << "Number of faces detected: " << dets.size() << endl;
		}

		//http://stackoverflow.com/questions/409348/iteration-over-stdvector-unsigned-vs-signed-index-variable

		for (std::vector<int>::size_type i = 0; i != dets.size(); i++) {

			dlib::rectangle rect = dets[i];

			if (verbose) {
				_RPT4(_CRT_WARN, "Left: %d, Top: %d, Width: %d, Height: %d\n", rect.left(), rect.top(), rect.width(), rect.height());
				cout << "Left: " << rect.left() << ", Top: " << rect.top() << ", Width: " << rect.width() << ", Height: " << rect.height() << endl;
			}

			RECT r;
			r.left = rect.left();
			r.top = rect.top();
			r.right = rect.right();
			r.bottom = rect.bottom();

			results.push_back(r);
		}
	}

	_RPT0(_CRT_WARN, "\n");

	// See https://limbioliong.wordpress.com/2011/08/14/returning-an-array-of-strings-from-c-to-c-part-1/
	// 
	*facecount = results.size();

	if (results.size() != 0) {
		size_t fsize = sizeof(RECT *) * results.size();
		size_t rsize = sizeof(RECT);

		if (verbose) {
			cout << "fsize: " << fsize << " rsize: " << rsize << endl;
		}

		*faces = (RECT**)::CoTaskMemAlloc(fsize);
		memset(*faces, 0, fsize);

		for (size_t i = 0; i < results.size(); i++) {
			(*faces)[i] = (RECT*)::CoTaskMemAlloc(rsize);
			RECT r = results.at(i);
			std::memcpy((*faces)[i], &r, rsize);
		}
	}
}

/// <summary>
/// Detect faces.
/// </summary>
///
/// <param name="bytes">		[in,out] If non-null, the bytes. </param>
/// <param name="size">			The size. </param>
/// <param name="faces">		[in,out] If non-null, the faces. </param>
/// <param name="facecount">	[in,out] If non-null, the facecount. </param>
[[deprecated("Replaced by SetImageToBMP/SetImageToRGB and DetectFaces(faces,facecount)")]]
extern void DetectFacesOld(byte* bytes, int size, RECT*** faces, int* facecount) {
	if (SetImageToBmp(bytes, size)) {
		DetectFaces(faces, facecount);
	}
}

/// <summary>
/// Detect landmarks.
/// </summary>
///
/// <param name="face">			The RECT to process. </param>
/// <param name="landmarks">	[in,out] If non-null, the landmarks. </param>
/// <param name="markcount">	[in,out] If non-null, the markcount. </param>
extern void DetectLandmarks(RECT face, POINT*** landmarks, int* markcount) {
	speedtest__("DetectFaces: ")
	{
		// Now we will go ask the shape_predictor to tell us the pose of
		// each face we detected.
		if (verbose) {
			cout << "DetectFaces: " << endl;
		}

		dlib::rectangle rect(face.left, face.top, face.right, face.bottom);

		full_object_detection shape = sp(img, rect);

		if (verbose) {
			_RPT1(_CRT_WARN, "number of parts: %d\n", shape.num_parts());
			cout << "number of parts: " << shape.num_parts() << endl;
		}

		// See https://limbioliong.wordpress.com/2011/08/14/returning-an-array-of-strings-from-c-to-c-part-1/
		// 
		*markcount = shape.num_parts();

		if (shape.num_parts() != 0) {
			size_t lsize = sizeof(POINT *) * shape.num_parts();
			size_t psize = sizeof(POINT);

			if (verbose) {
				cout << "lsize: " << lsize << " psize: " << psize << endl;
			}

			*landmarks = (POINT**)::CoTaskMemAlloc(lsize);
			memset(*landmarks, 0, lsize);

			for (unsigned long i = 0; i < shape.num_parts(); i++) {
				(*landmarks)[i] = (POINT*)::CoTaskMemAlloc(psize);
				POINT p;
				p.x = shape.part(i).x();
				p.y = shape.part(i).y();
				// TODO z?
				std::memcpy((*landmarks)[i], &p, psize);
			}
		}
	}
}
