using ImageMagick;
using System.Drawing;
using System.IO.Compression;

namespace mipmap2jpg
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DirectoryInfo di = new DirectoryInfo(args[0]);

            foreach (FileInfo fi in di.GetFiles("*.zip"))
            {
                Console.WriteLine(fi.Name);

                ZipArchive zipFile = new ZipArchive(new FileStream(fi.FullName, FileMode.Open, FileAccess.Read));

                List<string> filePaths = new List<string>();
                foreach (ZipArchiveEntry zae in zipFile.Entries)
                {
                    string filePath = Path.GetTempFileName();
                    zae.ExtractToFile(filePath, true);

                    filePaths.Add(filePath);
                }

                List<Size> sizes = filePaths.Select(fp => Image.FromFile(fp)).Select(i => new Size(i.Width, i.Height)).ToList();

                int width = sizes.Count(s => s.Height != 512);
                int height = sizes.Count(s => s.Width != 512);

                int imageWidth = 496 * (width - 1) + sizes.First(s => s.Width != 512).Width;
                int imageHeight = 496 * (height - 1) + sizes.First(s => s.Height != 512).Height;

                MagickImage output = new MagickImage(MagickColors.White, (uint)imageWidth, (uint)imageHeight);

                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        {
                            output.Composite(new MagickImage(filePaths[i + width * j]), i * 496, j * 496);
                        }
                    }
                }

                output.Write(fi.FullName.Replace(".zip", ".jpg"));
            }
        }
    }
}
