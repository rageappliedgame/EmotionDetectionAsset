Implementation of 'Emotion Detection Asset' v1.0.

Authors: Kiavash Bahreini, Wim van der Vegt.

Organization: Open University of the Netherlands (OUNL).

Task: T2.3 of the RAGE project. The project website is http://rageproject.eu.

For any questions please contact: 

Kiavash Bahreini via kiavash.bahreini [AT] ou [DOT] nl
and/or
Wim van der Vegt via wim.vandervegt [AT] ou [DOT] nl

Cite this work as:
Bahreini, K., van der Vegt, W. & Westera, W. Multimedia Tools and Applications (2019). https://doi.org/10.1007/s11042-019-7250-z

The solution consists of seven projects:
	- 'DLIB Wrapper' is a C# wrapper over the DLIB C++ library. Dlib's open source licensing allows you to use it in any application even in commercial applications, free of charge. It is freely available at http://dlib.net.
	- 'EmotionDetectionAsset' is an asset for realtime emotion detection through loading an static image file, a recorded video file, or a live webcam stream.
	- 'EmotionDetectionAsset_Demo' provides a demo version of the asset.
	- 'EmotionDetectionAsset' provides portable version of the asset.
	- 'EmotionDetectionAsset_Test' provides ...TBC... . The 'EmotionDetectionAsset_Test' project is not necessary and can be safely removed.
	- 'AssetManager' is a package that the emotion detection asset dependent on it. You can also download it separately from https://github.com/rageappliedgame/AssetManager.
	- 'RAGEAssetManager_Portable' provides portable version of the AssetManager. 

For more implementation and integration details of the RAGE project assets you may refer to the software design document (https://rage.ou.nl/filedepot?fid=501).

Summary of most important changes from the previous version of the TwoA asset:
- The README.txt file has been updated.
- The lincence texts were added to the source codes.
- The user inteface of the 'EmotionDetectionAsset_Demo' has been updated.
