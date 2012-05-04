using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace PhotoLibrary.Helpers {
  public class MvcImages {
    // manipulate an image
    /// <summary>
    /// Create resized and/or compressed copy of and image
    /// </summary>
    /// <param name="filePath">Full path to an image file</param>
    /// <param name="maxWidth">New maximum width</param>
    /// <param name="maxHeight">New maximum height</param>
    /// <param name="compression">Set new image compression - 1L(max) - 100L(no compression)</param>
    /// <param name="makeSmallerOnly">Make pictures smaller but not lagrer</param>
    public static void CreateResizedOrCompressedCopyIfImage
      (string filePath, int maxWidth, int maxHeight, long compression, bool makeSmallerOnly) {
      string imageFileNameEnding = "_" + maxHeight + "_" + maxWidth + "_" + compression;
      string newFilePath = _buildFilePathWithEnding(filePath, imageFileNameEnding);
      ResizeImage(filePath, newFilePath, maxWidth, maxHeight, compression, makeSmallerOnly);
    }

    /// <summary>
    /// Compress an image
    /// </summary>
    /// <param name="originalFilePath">Full path to an image file</param>
    /// <param name="compression">Set image compression - 1L(max) - 100L(no compression)</param>
    public static void CompressImage(string originalFilePath, long compression) {
      ResizeImage(originalFilePath, originalFilePath, null, null, compression, false);
    }

    /// <summary>
    /// Resize an image
    /// </summary>
    /// <param name="originalFilePath">Full path to an image file</param>
    /// <param name="maxWidth">New maximum width</param>
    /// <param name="maxHeight">New maximum height</param>
    public static void ResizeImage(string originalFilePath, int maxWidth, int maxHeight) {
      ResizeImage(originalFilePath, originalFilePath, maxWidth, maxHeight, 100L, false);
    }

    /// <summary>
    /// Resize or/and compress an image
    /// </summary>
    /// <param name="originalFilePath">Full path to an image file</param>
    /// <param name="newFilePath">Full path to a new image file (use 'originalFilePath' here if you want to resize original file)</param>
    /// <param name="maxWidth">New maximum width</param>
    /// <param name="maxHeight">New maximum height</param>
    /// <param name="compression">Set image compression - 1L(max) - 100L(no compression)</param>
    /// <param name="makeSmallerOnly">Make pictures smaller but not lagrer</param>
    public static void ResizeImage(string originalFilePath, string newFilePath,
                                   int? maxWidth, int? maxHeight, long compression, bool makeSmallerOnly) {
      _resizeAndCompressImage(originalFilePath, newFilePath, maxWidth, maxHeight, compression, makeSmallerOnly);
    }


    // information about an image
    /// <summary>
    /// Get existing image height
    /// </summary>
    /// <param name="pathToImage">Full path to an image file</param>
    /// <returns>Image height</returns>
    public static string GetImageHeight(string pathToImage) {
      if (File.Exists(pathToImage)) {
        try {
          Image currentImage = Image.FromFile(pathToImage);
          return currentImage.Height.ToString();
        }
        catch (Exception) {
        }
      }
      return "";
    }

    /// <summary>
    /// Get existing image width
    /// </summary>
    /// <param name="pathToImage">Full path to an image file</param>
    /// <returns>Image width</returns>
    public static string GetImageWidth(string pathToImage) {
      if (File.Exists(pathToImage)) {
        try {
          Image currentImage = Image.FromFile(pathToImage);
          return currentImage.Width.ToString();
        }
        catch (Exception) {
        }
      }
      return "";
    }


    /// private methods
    private static void _resizeAndCompressImage(string originalFilePath, string newFilePath,
                                                int? maxWidth, int? maxHeight, long compression, bool makeSmallerOnly) {
      if (string.IsNullOrEmpty(originalFilePath) ||
          !File.Exists(originalFilePath) ||
          !MvcFiles.CheckIfFileIsImage(originalFilePath)) return;

      // load image from file path
      Image _originalImage = Image.FromFile(originalFilePath);

      // prevent using images internal thumbnail
      //_originalImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
      //_originalImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

      // perform additional calculations
      if (maxWidth == null) maxWidth = _originalImage.Width;
      if (maxHeight == null) maxHeight = _originalImage.Height;
      if (makeSmallerOnly && _originalImage.Width <= maxWidth)
        maxWidth = _originalImage.Width;
      int? _newHeight = _originalImage.Height*maxWidth/_originalImage.Width;
      if (_newHeight > maxHeight) {
        maxWidth = _originalImage.Width*maxHeight/_originalImage.Height;
        _newHeight = maxHeight;
      }

      // resize image
      Image _resizedImage = _originalImage.GetThumbnailImage
        (Convert.ToInt32(maxWidth), Convert.ToInt32(_newHeight), null, IntPtr.Zero);

      // compress (or not) image and save
      if (compression < 1L) compression = 1L;
      if (compression > 100L) compression = 100L;
      if (compression == 100L)
        _resizedImage.Save(newFilePath);
      else
        _saveCompressedImageAsJPG(_resizedImage, newFilePath, compression);

      // clear handles to files
      _originalImage.Dispose();
      _resizedImage.Dispose();
    }

    private static void _saveCompressedImageAsJPG(Image image,
                                                  string newFilePath, long compression) {
      // get the desired Encoder and Quality
      var encoderParameters = new EncoderParameters(1);
      // set compression parameter: 1L(max) - 100L(no compression)
      encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, compression);
      // set encodet to output *.jpg file
      ImageCodecInfo encoderInfo = _getEncoderInfo("image/jpeg");
      // save new compressed file
      image.Save(newFilePath, encoderInfo, encoderParameters);
    }

    private static ImageCodecInfo _getEncoderInfo(String mimeType) {
      // Get the desired output encoder info
      ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
      return codecs.SingleOrDefault(codec => codec.MimeType == mimeType);
    }

    private static string _buildFilePathWithEnding(string imageFilePath, string imageFileNameEnding) {
      if (!string.IsNullOrEmpty(imageFilePath) && Path.HasExtension(imageFilePath))
        return Path.Combine(Path.GetDirectoryName(imageFilePath),
                            Path.GetFileNameWithoutExtension(imageFilePath) +
                            imageFileNameEnding +
                            Path.GetExtension(imageFilePath).ToLower());
      return "";
    }


    protected static string GetFilePath(string filename) {
      string fileFolder = ConfigurationManager.AppSettings.Get("localFilesFolder");
      if (string.IsNullOrWhiteSpace(fileFolder))
        fileFolder = "Files";

      return Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, fileFolder, filename);
    }

    public static void ResizeImage(string jpgFilename, string newFilename, int maxWidth, int maxHeight) {
      Image image = Image.FromFile(GetFilePath(jpgFilename));

      double widthScale = maxWidth/(double) image.Width;
      double heightScale = maxHeight/(double) image.Height;

      double scaleFactor = widthScale < heightScale ? widthScale : heightScale;

      string newImagePath = GetFilePath(newFilename);

      // Delete File if Exists
      if (File.Exists(newImagePath))
        File.Delete(newImagePath);

      var newWidth = (int) (image.Width*scaleFactor);
      var newHeight = (int) (image.Height*scaleFactor);
      var thumbnailBitmap = new Bitmap(newWidth, newHeight);

      Graphics thumbnailGraph = Graphics.FromImage(thumbnailBitmap);
      thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
      thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
      thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

      var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
      thumbnailGraph.DrawImage(image, imageRectangle);

      thumbnailBitmap.Save(newImagePath, image.RawFormat);

      thumbnailGraph.Dispose();
      thumbnailBitmap.Dispose();
      image.Dispose();
    }

    public static void CreateThumbnail(string jpgFilename, string newFilename, int thumbnailWidth, int thumbnailHeight) {
      Image image = Image.FromFile(GetFilePath(jpgFilename));

      double widthScale = thumbnailWidth / (double)image.Width;
      double heightScale = thumbnailHeight / (double)image.Height;

      double scaleFactor = widthScale > heightScale ? widthScale : heightScale;

      string newImagePath = GetFilePath(newFilename);

      // Delete File if Exists
      if (File.Exists(newImagePath))
        File.Delete(newImagePath);

      var newWidth = (int)(image.Width * scaleFactor);
      var newHeight = (int)(image.Height * scaleFactor);
      //var thumbnailBitmap = new Bitmap(newWidth, newHeight);
      var thumbnailBitmap = new Bitmap(thumbnailWidth, thumbnailHeight);

      Graphics thumbnailGraph = Graphics.FromImage(thumbnailBitmap);
      thumbnailGraph.CompositingQuality = CompositingQuality.HighQuality;
      thumbnailGraph.SmoothingMode = SmoothingMode.HighQuality;
      thumbnailGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

      int x = (thumbnailWidth - newWidth) / 2;
      int y = (thumbnailHeight - newHeight) / 2;
      var imageRectangle = new Rectangle(x, y, newWidth, newHeight);
      thumbnailGraph.DrawImage(image, imageRectangle);
      var cropRectangle = new Rectangle(0, 0, thumbnailWidth, thumbnailHeight);
      

      

      thumbnailBitmap.Save(newImagePath, image.RawFormat);

      thumbnailGraph.Dispose();
      thumbnailBitmap.Dispose();
      image.Dispose();
    }
  }
}