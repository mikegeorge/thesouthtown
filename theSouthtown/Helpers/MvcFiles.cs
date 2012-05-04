using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace PhotoLibrary.Helpers {
  // MVC Upload tutorial - 'http://haacked.com/archive/2010/07/16/uploading-files-with-aspnetmvc.aspx'
  // Following code can be added to the web.config file to define
  // 'Default local files folder'
  //<appSettings>
  //  <add key="localFilesFolder" value="Files"/>
  //</appSettings>
  public class MvcFiles {
    // Default settings
    private const int _maxFileSize = 1000000000;
    private const string _defaultlocalFilesFolder = "Files";
    private const string _localFilesFolderWebConfigName = "localFilesFolder";
    // will check extension against 'AllowedExtensions' list, if not empty
    // thumbnail parameters (compression range is 1L(max) - 100L(no compression))
    public const string ThumbnailNameEnding = "_t";
    private const int _thumbMaxWidth = 150;
    private const int _thumbnailMaxHeight = 150;
    private const long _thumbCompression = 70L;
    public static string[] AllowedExtensions = {".pdf", ".doc", ".jpg", ".png", ".gif", ".psd", ".ai"};
    public static string[] ImageExtensions = {".gif", ".jpg", ".jpeg", ".png", ".bmp", ".tif", ".tiff"};


    /// PUBLIC METHODS
    public static string GetLocalFilesFolder {
      get {
        string fileFolder = ConfigurationManager.AppSettings.Get(_localFilesFolderWebConfigName);
        if (string.IsNullOrEmpty(fileFolder)) fileFolder = _defaultlocalFilesFolder;
        string folderPath = Path.Combine(GetFullAppllicationPath, fileFolder);
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        return fileFolder;
      }
    }

    public static string GetFullAppllicationPath {
      get { return HttpContext.Current.Request.PhysicalApplicationPath; }
    }

    public static string GetFullPathToLocalFilesFolder {
      get { return Path.Combine(GetFullAppllicationPath, GetLocalFilesFolder); }
    }

    public static string GetFullPathToLocalFile(string fileName) {
      string path = Path.Combine(GetFullPathToLocalFilesFolder, fileName);
      return File.Exists(path) ? path : "";
    }


    // universal upload methods
    /// <summary>
    /// Upload a file to the default directory on the server
    /// </summary>
    /// <param name="file">System.Web.HttpPostedFileBase</param>
    /// <returns>Uploaded file name</returns>
    public static string Upload(HttpPostedFileBase file) {
      return Upload(file, GetLocalFilesFolder, false, null);
    }

    /// <summary>
    /// Upload a file to the custom set directory on the server
    /// </summary>
    /// <param name="file">System.Web.HttpPostedFileBase</param>
    /// <param name="saveToFolder">Set local folder manually</param>
    /// <returns>Uploaded file name</returns>
    public static string Upload(HttpPostedFileBase file, string saveToFolder) {
      return Upload(file, saveToFolder, false, null);
    }

    /// <summary>
    /// Upload a file to the custom set directory on the server
    /// </summary>
    /// <param name="file">System.Web.HttpPostedFileBase</param>
    /// <param name="replaceOldFile">Set to 'true' if you want to replace a file with the same name on upload</param>
    /// <returns>Uploaded file name</returns>
    public static string Upload(HttpPostedFileBase file, bool replaceOldFile) {
      return Upload(file, GetLocalFilesFolder, replaceOldFile, null);
    }

    /// <summary>
    /// Upload a file to the default directory on the server
    /// </summary>
    /// <param name="file">System.Web.HttpPostedFileBase</param>
    /// <param name="replaceOldFile">Set to 'true' if you want to replace a file with the same name on upload</param>
    /// <param name="replaceFilePath">Path to an older file to be replaced</param>
    /// <returns>Uploaded file name</returns>
    public static string Upload(HttpPostedFileBase file, bool replaceOldFile, string replaceFilePath) {
      return Upload(file, GetLocalFilesFolder, replaceOldFile, replaceFilePath);
    }

    /// <summary>
    /// Upload a file to the custom set directory on the server
    /// </summary>
    /// <param name="file">System.Web.HttpPostedFileBase</param>
    /// <param name="saveToFolder">Set local folder manually</param>
    /// <param name="replaceOldFile">Set to 'true' if you want to replace a file with the same name on upload</param>
    /// <param name="replaceFilePath">Path to an older file to be replaced</param>
    /// <returns>Uploaded file name</returns>
    public static string Upload(HttpPostedFileBase file, string saveToFolder, bool replaceOldFile,
                                string replaceFilePath) {
      if (ValidateUploadSend(file)) {
        if (string.IsNullOrEmpty(saveToFolder)) saveToFolder = GetLocalFilesFolder;
        if (replaceOldFile) {
          if (string.IsNullOrEmpty(replaceFilePath))
            replaceFilePath = Path.Combine(GetFullAppllicationPath, saveToFolder, file.FileName);
          DeleteFile(replaceFilePath);
        }
        return UploadFileAndGetFileName(file, saveToFolder);
      }
      return "";
    }



    // image upload methods
    /// <summary>
    /// Upload a file to the default directory on the server and create thumbnail (if image)
    /// </summary>
    /// <param name="file">System.Web.HttpPostedFileBase</param>
    /// <returns>Uploaded file name</returns>
    public static string UploadAndCreateThumbnail(HttpPostedFileBase file) {
      return UploadAndCreateThumbnail(file, GetLocalFilesFolder, false, null);
    }

    /// <summary>
    /// Upload a file to the custom set directory on the server and create thumbnail (if image)
    /// </summary>
    /// <param name="file">System.Web.HttpPostedFileBase</param>
    /// <param name="saveToFolder">Set local folder manually</param>
    /// <returns>Uploaded file name</returns>
    public static string UploadAndCreateThumbnail(HttpPostedFileBase file, string saveToFolder) {
      return UploadAndCreateThumbnail(file, saveToFolder, false, null);
    }

    /// <summary>
    /// Upload a file to the custom set directory on the server
    /// </summary>
    /// <param name="file">System.Web.HttpPostedFileBase</param>
    /// <param name="replaceOldFile">Set to 'true' if you want to replace a file with the same name on upload</param>
    /// <returns>Uploaded file name</returns>
    public static string UploadAndCreateThumbnail(HttpPostedFileBase file, bool replaceOldFile) {
      return UploadAndCreateThumbnail(file, GetLocalFilesFolder, replaceOldFile, null);
    }

    /// <summary>
    /// Upload a file to the default directory on the server and create thumbnail (if image)
    /// </summary>
    /// <param name="file">System.Web.HttpPostedFileBase</param>
    /// <param name="replaceOldFile">Set to 'true' if you want to replace a file with the same name on upload</param>
    /// <param name="replaceFilePath">Path to an older file to be replaced</param>
    /// <returns>Uploaded file name</returns>
    public static string UploadAndCreateThumbnail(HttpPostedFileBase file, bool replaceOldFile, string replaceFilePath) {
      return UploadAndCreateThumbnail(file, GetLocalFilesFolder, replaceOldFile, replaceFilePath);
    }

    /// <summary>
    /// Upload a file to the custom set directory on the server and create thumbnail (if image)
    /// </summary>
    /// <param name="file">System.Web.HttpPostedFileBase</param>
    /// <param name="saveToFolder">Set local folder manually</param>
    /// <param name="replaceOldFile">Set to 'true' if you want to replace a file with the same name on upload</param>
    /// <param name="replaceFilePath">Path to an older file to be replaced</param>
    /// <returns>Uploaded file name</returns>
    public static string UploadAndCreateThumbnail(HttpPostedFileBase file, string saveToFolder, bool replaceOldFile,
                                                  string replaceFilePath) {
      if (ValidateUploadSend(file)) {
        if (string.IsNullOrEmpty(saveToFolder)) saveToFolder = GetLocalFilesFolder;
        if (replaceOldFile) {
          if (string.IsNullOrEmpty(replaceFilePath))
            replaceFilePath = Path.Combine(GetFullAppllicationPath, saveToFolder, file.FileName);
          DeleteFile(replaceFilePath);
          DeleteFile(GenerateFullPathToThumbnail(replaceFilePath));
        }
        string fileName = UploadFileAndGetFileName(file, saveToFolder);
        // If file is an image, create a thumbnail
        string filePath = Path.Combine(GetFullAppllicationPath, Path.Combine(saveToFolder, fileName));
        CreateThumbnailIfImage(filePath);
        // get correct file name
        return fileName;
      }
      return "";
    }


    // removal methods
    /// <summary>
    /// Delete any file by path
    /// </summary>
    /// <param name="filePath">Full path to file to delete</param>
    public static void Delete(string filePath) {
      DeleteFile(filePath);
    }

    /// <summary>
    /// Delete file from default upload folder
    /// </summary>
    /// <param name="fileName">File name to delete in default folder</param>
    public static void DeleteFromDefaultLocalFolder(string fileName) {
      DeleteFile(Path.Combine(GetFullPathToLocalFilesFolder, fileName));
    }


    // other methods
    /// <summary>
    /// Check if a file is an image file
    /// </summary>
    /// <param name="filePathOrName">File name or path to check</param>
    /// <returns>'true' if it is an image</returns>
    public static bool CheckIfFileIsImage(string filePathOrName) {
      return Path.HasExtension(filePathOrName) &&
             ImageExtensions.Any(e => e.Equals(Path.GetExtension(filePathOrName).ToLower()));
    }

    /// <summary>
    /// Get file name of a thumbnail for the given image file
    /// </summary>
    /// <param name="imageFileName">File name of an image</param>
    /// <returns>File name of a thumbnail</returns>
    public static string GetThumbImageFileName(string imageFileName) {
      return !string.IsNullOrEmpty(imageFileName) ? BuildFileNameWithEnding(imageFileName, ThumbnailNameEnding) : "";
    }


    /// PRIVATE METHODS
    // general file methods
    private static string UploadFileAndGetFileName(HttpPostedFileBase file, string fileFolder) {
      if (file != null && file.ContentLength > 0) {
        string folderPath = Path.Combine(GetFullAppllicationPath, fileFolder);
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        string pathToNewFile = Path.Combine(GetFullAppllicationPath, fileFolder, file.FileName);
        string newPathToNewFile = CheckIfFileExistsAndGetNewPath(pathToNewFile);
        file.SaveAs(newPathToNewFile);
        return Path.GetFileName(newPathToNewFile);
      }
      return "";
    }


    private static bool ValidateUploadSend(HttpPostedFileBase file) {
      if (file != null && file.ContentLength > 0) {
        bool isValid = true;

        // File without extension not allowed
        if (!Path.HasExtension(file.FileName)) isValid = false;

        // Limits the file size
        if (file.ContentLength > _maxFileSize) isValid = false;

        // File matches/not matches allowed extensions
        if (Path.HasExtension(file.FileName) &&
            !AllowedExtensions.Any(e => e.Equals(Path.GetExtension(file.FileName).ToLower()))) isValid = false;

        return isValid;
      }
      return false;
    }

    private static string CheckIfFileExistsAndGetNewPath(string filePath) {
      if (string.IsNullOrEmpty(filePath)) return "";
      if (!File.Exists(filePath)) return filePath;
      int count = 1;
      while (File.Exists(filePath)) {
        string oldFileName = Path.GetFileNameWithoutExtension(filePath);
        int removeInd = oldFileName.LastIndexOf('(');
        if (removeInd > 0) oldFileName = oldFileName.Remove(removeInd);
        string newFileName = String.Format("{0}({1}){2}",
                                           oldFileName, count, Path.GetExtension(filePath));
        filePath = filePath.Replace(Path.GetFileName(filePath), newFileName);
        count++;
      }
      return filePath;
    }

    private static string BuildFileNameWithEnding(string imageFileNameOrPath, string imageFileNameEnding) {
      if (!string.IsNullOrEmpty(imageFileNameOrPath) && Path.HasExtension(imageFileNameOrPath))
        return Path.GetFileNameWithoutExtension(imageFileNameOrPath) + imageFileNameEnding +
               Path.GetExtension(imageFileNameOrPath).ToLower();
      return "";
    }

    private static void DeleteFile(string filePath) {
      if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath)) File.Delete(filePath);
    }


    // image file methods
    private static string GenerateFullPathToThumbnail(string filePath) {
      return Path.Combine(Path.GetDirectoryName(filePath),
                          BuildFileNameWithEnding(Path.GetFileName(filePath), ThumbnailNameEnding));
    }

    private static void CreateThumbnailIfImage(string filePath) {
      if (string.IsNullOrEmpty(filePath) || !CheckIfFileIsImage(filePath)) return;
      MvcImages.ResizeImage(filePath, GenerateFullPathToThumbnail(filePath), _thumbMaxWidth, _thumbnailMaxHeight,
                            _thumbCompression, true);
    }
  }
}