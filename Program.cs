using SkiaSharp;

namespace Projeto {
	class Program {
		static void Main(string[] args) {
			using (
				SKBitmap bitmapEntrada = SKBitmap.Decode("C:\\Users\\joaol\\Documents\\GitHub\\TestCorrector\\img\\Gabarito Correto 1.png"),
				bitmapSaida = new SKBitmap(new SKImageInfo(bitmapEntrada.Width, bitmapEntrada.Height, SKColorType.Gray8))) {

				Console.WriteLine(bitmapEntrada.ColorType);
				Console.WriteLine(bitmapSaida.ColorType);

				unsafe {
					byte* entrada = (byte*)bitmapEntrada.GetPixels();
					byte* saida = (byte*)bitmapSaida.GetPixels();

					long pixelsTotais = bitmapEntrada.Width * bitmapEntrada.Height;
					long media = 0;

					// RGB para escala de cinza
					for (int e = 0, s = 0; s < pixelsTotais; e += 4, s++) {
						saida[s] = (byte)((entrada[e] + entrada[e + 1] + entrada[e + 2]) / 3);
						media += saida[s];
					}

					media = (byte)(media / pixelsTotais);
					// Limiarização
					for(int p = 0; p < pixelsTotais; p++)
					{
						if(saida[p] > media)
							saida[p] = 0;
						else
							saida[p] = 255;
					}

					// Erosão
					Erodir(saida, bitmapSaida.Width, bitmapSaida.Height, 7, entrada);

					// Reconhecimento de formas e floodfill
					





					// int tamanhoJanela = 1;
					// int metadeJanela = tamanhoJanela / 2;
					// entrada = (byte*)bitmapSaida.GetPixels();
					// for(int y = 0; y < bitmapSaida.Height; y++)
					// {
					// 	for(int x = 0; x < bitmapSaida.Width; x++)
					// 	{
					// 		int i = (y * bitmapEntrada.Width) + x;
					// 		saida[i] = Erodir1(entrada, bitmapEntrada.Width, bitmapSaida.Height, x, y, metadeJanela);
					// 	}
					// }
				}

				using (FileStream stream = new FileStream("C:\\Users\\joaol\\Documents\\GitHub\\TestCorrector\\img\\Gabarito Correto Out 1.png", FileMode.OpenOrCreate, FileAccess.Write)) {
					bitmapSaida.Encode(stream, SKEncodedImageFormat.Png, 100);
				}
			}
		}

		static unsafe void Erodir(byte* imagem, int largura, int altura, int tamanhoJanela, byte* temp) {
			if ((tamanhoJanela & 1) == 0) {
				throw new Exception("O tamanho deve ser um valor ímpar");
			}

			if (tamanhoJanela < 3) {
				throw new Exception("O tamanho deve ser >= 3");
			}

			int metade = tamanhoJanela >> 1;

			// Primeira passada (vertical)
			for (int y = 0; y < altura; y++) {
				for (int x = 0; x < largura; x++) {
					int valor = 255;

					int yInicial = y - metade;
					int tamanhoValido = tamanhoJanela;
					if (yInicial < 0) {
						tamanhoValido = tamanhoValido + yInicial;
						yInicial = 0;
					}
					if ((y + metade) > (altura - 1)) {
						tamanhoValido = tamanhoValido - ((y + metade) - (altura - 1));
					}

					int indice = (yInicial * largura) + x;
					for (int i = 0; i < tamanhoValido; i++, indice += largura) {
						if (valor > imagem[indice]) {
							valor = imagem[indice];
						}
					}

					temp[(y * largura) + x] = (byte)valor;
				}
			}

			// Segunda passada (horizontal)
			for (int y = 0; y < altura; y++) {
				for (int x = 0; x < largura; x++) {
					int valor = 255;

					int xInicial = x - metade;
					int tamanhoValido = tamanhoJanela;
					if (xInicial < 0) {
						tamanhoValido = tamanhoValido + xInicial;
						xInicial = 0;
					}
					if ((x + metade) > (largura - 1)) {
						tamanhoValido = tamanhoValido - ((x + metade) - (largura - 1));
					}

					int indice = (y * largura) + xInicial;
					for (int i = 0; i < tamanhoValido; i++, indice++) {
						if (valor > temp[indice]) {
							valor = temp[indice];
						}
					}

					imagem[(y * largura) + x] = (byte)valor;
				}
			}
		}
	}
}