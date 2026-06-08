using QRCoder;

namespace UrlShortener.API.Services;

public class QrCodeService
{
    public byte[] GenerateQrCode(string url)
    {
        using var qrGenerator = new QRCodeGenerator();

        using var qrData =
            qrGenerator.CreateQrCode(
                url,
                QRCodeGenerator.ECCLevel.Q);

        var pngQrCode =
            new PngByteQRCode(qrData);

        return pngQrCode.GetGraphic(20);
    }
}