using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ImageMagick;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string incomingFilePath = AppContext.BaseDirectory + "Files/cat.mp4";

            using (var videoFrames = new MagickImageCollection(incomingFilePath, MagickFormat.Mp4))
            {
                string fName = Guid.NewGuid() + "_compose.mp4";
                using (var str = Process(videoFrames, true))
                {
                    using (var fileStream = File.Create(AppContext.BaseDirectory + fName))
                    {
                        str.Seek(0, SeekOrigin.Begin);
                        str.CopyTo(fileStream);
                    }
                }
                Console.WriteLine("Finished with compose() " + fName);
            }

            using (var videoFrames = new MagickImageCollection(incomingFilePath, MagickFormat.Mp4))
            {
                string fName2 = Guid.NewGuid() + ".mp4";
                using (var str = Process(videoFrames, false))
                {
                    using (var fileStream = File.Create(AppContext.BaseDirectory + fName2))
                    {
                        str.Seek(0, SeekOrigin.Begin);
                        str.CopyTo(fileStream);
                    }
                }
                Console.WriteLine("Finished without compose() " + fName2);
            }

        }

        public static Stream Process(MagickImageCollection incomingImageFrames, bool isWithFullFrames)
        {
            if (isWithFullFrames)
            {
                // making full frames
                for (var i = 1; i < incomingImageFrames.Count; i++)
                {
                    var prevFrame = incomingImageFrames[i - 1].Clone();
                    prevFrame.Composite(incomingImageFrames[i], incomingImageFrames[i].Page.X, incomingImageFrames[i].Page.Y, CompositeOperator.Over);
                    incomingImageFrames[i] = prevFrame;
                }
            }

            Console.WriteLine(" total frames " + incomingImageFrames.Count);

            var resultImageStream = new MemoryStream();

            incomingImageFrames.Write(resultImageStream, MagickFormat.Mp4);
            resultImageStream.Position = 0;
            return resultImageStream;
        }
    }
}
