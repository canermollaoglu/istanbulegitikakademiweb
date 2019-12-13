using System.ComponentModel;

namespace NitelikliBilisim.Core.Enums
{
    public enum EducationMediaType
    {
        [Description("Galeri Fotoğrafı")]
        GalleryPhoto = 1010,
        [Description("Küçük Resim (Thumbnail)")]
        Tumbnail = 1020,
        [Description("Reklam Fotoğrafı")]
        AdPhoto = 1030,
        [Description("Ön İzleme Videosu")]
        PreviewVideo = 1040,
        [Description("Öz İzleme Fotoğrafı")]
        PreviewPhoto = 1050,
        [Description("Banner")]
        Banner = 1060,
        [Description("256 x 256")]
        Photo_256x256 = 256256,
        [Description("128 x 128")]
        Photo_128x128 = 128128,
        [Description("64 x 64")]
        Photo_64x64 = 6464,
        [Description("32 x 32")]
        Photo_32x32 = 3232,
        [Description("8 x 8")]
        Photo_8x8 = 88
    }
}