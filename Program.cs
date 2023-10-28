using SkiaSharp;

namespace Projeto {
	class Program {
		static void Main(string[] args) {
			using (SKBitmap bitmapEntrada = SKBitmap.Decode("C:\\Users\\joaol\\Documents\\GitHub\\TestCorrector\\img\\Gabarito Correto 1.png"),
				bitmapSaida = new SKBitmap(new SKImageInfo(bitmapEntrada.Width, bitmapEntrada.Height, SKColorType.Gray8))) {

				Console.WriteLine(bitmapEntrada.ColorType);
				Console.WriteLine(bitmapSaida.ColorType);

				unsafe {
					byte* entrada = (byte*)bitmapEntrada.GetPixels();
					byte* saida = (byte*)bitmapSaida.GetPixels();

					long pixelsTotais = bitmapEntrada.Width * bitmapEntrada.Height;
					long media = 0;

					for (int e = 0, s = 0; s < pixelsTotais; e += 4, s++) {
						saida[s] = (byte)((entrada[e] + entrada[e + 1] + entrada[e + 2]) / 3);
						media += saida[s];
					}

					media = (byte)(media / pixelsTotais);
					for(int p = 0; p < pixelsTotais; p++)
					{
						if(saida[p] > media)
							saida[p] = 0;
						else
							saida[p] = 255;
					}

					for(int y = 0; y < bitmapSaida.Height; y++)
					{
						for(int x = 0; x < bitmapSaida.Width; x++)
						{
							if(saida[y * bitmapSaida.Width + x] == 255)
								FloodFill(bitmapSaida, x, y);
						}
					}
				}

				using (FileStream stream = new FileStream("C:\\Users\\joaol\\Documents\\GitHub\\TestCorrector\\img\\Gabarito Correto Out 1.png", FileMode.OpenOrCreate, FileAccess.Write)) {
					bitmapSaida.Encode(stream, SKEncodedImageFormat.Png, 100);
				}
			}
		}

		static void FloodFill(SKBitmap bitmap, int x, int y) {

		}
	}
}